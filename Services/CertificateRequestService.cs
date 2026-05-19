using Microsoft.EntityFrameworkCore;
using rosa_testovoye.Data;
using rosa_testovoye.Data.Entities;
using rosa_testovoye.Data.Enums;
using rosa_testovoye.Models;
using rosa_testovoye.Services.Exceptions;

namespace rosa_testovoye.Services;

public class CertificateRequestService(AppDbContext context) : ICertificateRequestService
{
    public async Task<SimilarRequestWarning> CheckSimilarActiveAsync(int employeeId, CertificateType type)
    {
        var similar = await FindSimilarActiveRequestAsync(employeeId, type);
        return ToSimilarWarning(similar);
    }

    public async Task<CreateCertificateRequestResult> CreateAsync(CreateCertificateRequestInput input)
    {
        await EnsureEmployeeExistsAsync(input.EmployeeId, UserRole.Employee);
        ValidateCreateInput(input);

        var similar = await FindSimilarActiveRequestAsync(input.EmployeeId, input.Type);
        var now = DateTime.UtcNow;

        var request = new CertificateRequest
        {
            EmployeeId = input.EmployeeId,
            Type = input.Type,
            CopiesCount = input.CopiesCount,
            Reason = input.Reason.Trim(),
            CustomTypeName = string.IsNullOrWhiteSpace(input.CustomTypeName)
                ? null
                : input.CustomTypeName.Trim(),
            Status = RequestStatus.Submitted,
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };

        context.CertificateRequests.Add(request);

        context.RequestStatusHistory.Add(new RequestStatusHistory
        {
            Request = request,
            FromStatus = null,
            ToStatus = RequestStatus.Submitted,
            ChangedByEmployeeId = input.EmployeeId,
            ChangedAtUtc = now
        });

        await context.SaveChangesAsync();

        var details = await GetDetailsAsync(request.Id)
            ?? throw new InvalidOperationException("Заявка не найдена после создания.");

        return new CreateCertificateRequestResult
        {
            Request = details,
            SimilarActiveWarning = ToSimilarWarning(similar)
        };
    }

    public async Task<IReadOnlyList<CertificateRequestListItem>> GetByEmployeeAsync(int employeeId)
    {
        await EnsureEmployeeExistsAsync(employeeId);

        return await context.CertificateRequests
            .AsNoTracking()
            .Where(r => r.EmployeeId == employeeId)
            .OrderByDescending(r => r.CreatedAtUtc)
            .Select(r => new CertificateRequestListItem
            {
                Id = r.Id,
                EmployeeId = r.EmployeeId,
                EmployeeFullName = r.Employee.FullName,
                Type = r.Type,
                CopiesCount = r.CopiesCount,
                Status = r.Status,
                CreatedAtUtc = r.CreatedAtUtc
            })
            .ToListAsync();
    }

    public async Task<IReadOnlyList<CertificateRequestListItem>> GetQueueAsync()
    {
        return await context.CertificateRequests
            .AsNoTracking()
            .OrderByDescending(r => r.CreatedAtUtc)
            .Select(r => new CertificateRequestListItem
            {
                Id = r.Id,
                EmployeeId = r.EmployeeId,
                EmployeeFullName = r.Employee.FullName,
                Type = r.Type,
                CopiesCount = r.CopiesCount,
                Status = r.Status,
                CreatedAtUtc = r.CreatedAtUtc
            })
            .ToListAsync();
    }

    public async Task<CertificateRequestDetails?> GetDetailsAsync(int requestId)
    {
        var request = await context.CertificateRequests
            .AsNoTracking()
            .Include(r => r.Employee)
            .Include(r => r.StatusHistory)
                .ThenInclude(h => h.ChangedByEmployee)
            .FirstOrDefaultAsync(r => r.Id == requestId);

        if (request is null)
        {
            return null;
        }

        return MapToDetails(request);
    }

    public async Task<CertificateRequestDetails> ChangeStatusAsync(
        int requestId,
        ChangeStatusInput input,
        int accountantEmployeeId)
    {
        await EnsureEmployeeExistsAsync(accountantEmployeeId, UserRole.Accountant);

        var request = await context.CertificateRequests
            .Include(r => r.Employee)
            .FirstOrDefaultAsync(r => r.Id == requestId)
            ?? throw new NotFoundException($"Заявка #{requestId} не найдена.");

        var fromStatus = request.Status;
        var toStatus = input.NewStatus;

        if (RequestStatusRules.IsTerminal(fromStatus))
        {
            throw new InvalidStatusTransitionException(fromStatus, toStatus);
        }

        if (!RequestStatusRules.CanTransition(fromStatus, toStatus))
        {
            throw new InvalidStatusTransitionException(fromStatus, toStatus);
        }

        var now = DateTime.UtcNow;
        request.Status = toStatus;
        request.UpdatedAtUtc = now;

        context.RequestStatusHistory.Add(new RequestStatusHistory
        {
            RequestId = request.Id,
            FromStatus = fromStatus,
            ToStatus = toStatus,
            ChangedByEmployeeId = accountantEmployeeId,
            Comment = string.IsNullOrWhiteSpace(input.Comment) ? null : input.Comment.Trim(),
            ChangedAtUtc = now
        });

        await context.SaveChangesAsync();

        return await GetDetailsAsync(requestId)
            ?? throw new InvalidOperationException("Заявка не найдена после обновления статуса.");
    }

    private async Task<CertificateRequest?> FindSimilarActiveRequestAsync(int employeeId, CertificateType type)
    {
        return await context.CertificateRequests
            .AsNoTracking()
            .Where(r => r.EmployeeId == employeeId
                && r.Type == type
                && (r.Status == RequestStatus.Submitted || r.Status == RequestStatus.InProgress))
            .OrderByDescending(r => r.CreatedAtUtc)
            .FirstOrDefaultAsync();
    }

    private static SimilarRequestWarning ToSimilarWarning(CertificateRequest? similar)
    {
        if (similar is null)
        {
            return new SimilarRequestWarning { HasSimilarActive = false };
        }

        return new SimilarRequestWarning
        {
            HasSimilarActive = true,
            SimilarRequestId = similar.Id,
            SimilarRequestType = similar.Type,
            SimilarRequestStatus = similar.Status
        };
    }

    private static void ValidateCreateInput(CreateCertificateRequestInput input)
    {
        if (input.CopiesCount < 1)
        {
            throw new ValidationException("Количество экземпляров должно быть не меньше 1.");
        }

        if (string.IsNullOrWhiteSpace(input.Reason))
        {
            throw new ValidationException("Укажите причину запроса.");
        }

        if (input.Reason.Length > 1000)
        {
            throw new ValidationException("Причина запроса не должна превышать 1000 символов.");
        }

        if (input.Type == CertificateType.Custom && string.IsNullOrWhiteSpace(input.CustomTypeName))
        {
            throw new ValidationException("Укажите название произвольной справки.");
        }

        if (input.Type != CertificateType.Custom && !string.IsNullOrWhiteSpace(input.CustomTypeName))
        {
            throw new ValidationException("Название справки указывается только для произвольного типа.");
        }
    }

    private async Task EnsureEmployeeExistsAsync(int employeeId, UserRole? requiredRole = null)
    {
        var employee = await context.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == employeeId);

        if (employee is null)
        {
            throw new NotFoundException($"Сотрудник #{employeeId} не найден.");
        }

        if (requiredRole is not null && employee.Role != requiredRole)
        {
            throw new UnauthorizedRoleException(
                requiredRole == UserRole.Accountant
                    ? "Смена статуса доступна только бухгалтеру."
                    : "Действие доступно только сотруднику.");
        }
    }

    private static CertificateRequestDetails MapToDetails(CertificateRequest request)
    {
        return new CertificateRequestDetails
        {
            Id = request.Id,
            EmployeeId = request.EmployeeId,
            EmployeeFullName = request.Employee.FullName,
            EmployeeDepartment = request.Employee.Department,
            Type = request.Type,
            CopiesCount = request.CopiesCount,
            Reason = request.Reason,
            CustomTypeName = request.CustomTypeName,
            Status = request.Status,
            CreatedAtUtc = request.CreatedAtUtc,
            UpdatedAtUtc = request.UpdatedAtUtc,
            StatusHistory = request.StatusHistory
                .OrderBy(h => h.ChangedAtUtc)
                .Select(h => new StatusHistoryItem
                {
                    FromStatus = h.FromStatus,
                    ToStatus = h.ToStatus,
                    ChangedByFullName = h.ChangedByEmployee.FullName,
                    Comment = h.Comment,
                    ChangedAtUtc = h.ChangedAtUtc
                })
                .ToList()
        };
    }
}
