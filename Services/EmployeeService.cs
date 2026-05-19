using Microsoft.EntityFrameworkCore;
using rosa_testovoye.Data;
using rosa_testovoye.Data.Entities;
using rosa_testovoye.Data.Enums;
using rosa_testovoye.Models;
using rosa_testovoye.Services.Exceptions;

namespace rosa_testovoye.Services;

public class EmployeeService(AppDbContext context) : IEmployeeService
{
    public async Task<IReadOnlyList<EmployeeDto>> GetAllAsync()
    {
        return await context.Employees
            .AsNoTracking()
            .OrderBy(e => e.FullName)
            .Select(e => new EmployeeDto
            {
                Id = e.Id,
                FullName = e.FullName,
                Department = e.Department,
                Role = e.Role
            })
            .ToListAsync();
    }

    public async Task<EmployeeDto?> GetByIdAsync(int id)
    {
        var employee = await context.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);

        return employee is null ? null : MapToDto(employee);
    }

    public async Task<EmployeeDto> CreateAsync(CreateEmployeeInput input)
    {
        ValidateInput(input.FullName, input.Department, input.Role);

        var employee = new Employee
        {
            FullName = input.FullName.Trim(),
            Department = string.IsNullOrWhiteSpace(input.Department) ? null : input.Department.Trim(),
            Role = input.Role
        };

        context.Employees.Add(employee);
        await context.SaveChangesAsync();

        return MapToDto(employee);
    }

    public async Task<EmployeeDto> UpdateAsync(int id, UpdateEmployeeInput input)
    {
        ValidateInput(input.FullName, input.Department, input.Role);

        var employee = await context.Employees.FirstOrDefaultAsync(e => e.Id == id)
            ?? throw new NotFoundException($"Сотрудник #{id} не найден.");

        employee.FullName = input.FullName.Trim();
        employee.Department = string.IsNullOrWhiteSpace(input.Department) ? null : input.Department.Trim();
        employee.Role = input.Role;

        await context.SaveChangesAsync();

        return MapToDto(employee);
    }

    public async Task DeleteAsync(int id)
    {
        var employee = await context.Employees.FirstOrDefaultAsync(e => e.Id == id)
            ?? throw new NotFoundException($"Сотрудник #{id} не найден.");

        var hasRequests = await context.CertificateRequests.AnyAsync(r => r.EmployeeId == id);
        if (hasRequests)
        {
            throw new ValidationException(
                "Нельзя удалить сотрудника: есть связанные заявки на справки.");
        }

        var hasStatusChanges = await context.RequestStatusHistory.AnyAsync(h => h.ChangedByEmployeeId == id);
        if (hasStatusChanges)
        {
            throw new ValidationException(
                "Нельзя удалить сотрудника: есть записи в истории смены статусов.");
        }

        context.Employees.Remove(employee);
        await context.SaveChangesAsync();
    }

    private static void ValidateInput(string fullName, string? department, UserRole role)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new ValidationException("Укажите ФИО.");
        }

        if (fullName.Length > 200)
        {
            throw new ValidationException("ФИО не должно превышать 200 символов.");
        }

        if (department?.Length > 200)
        {
            throw new ValidationException("Название отдела не должно превышать 200 символов.");
        }

        if (!Enum.IsDefined(role))
        {
            throw new ValidationException("Укажите корректную роль (Employee или Accountant).");
        }
    }

    private static EmployeeDto MapToDto(Employee employee) => new()
    {
        Id = employee.Id,
        FullName = employee.FullName,
        Department = employee.Department,
        Role = employee.Role
    };
}
