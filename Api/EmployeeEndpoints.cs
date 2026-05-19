using rosa_testovoye.Data.Enums;
using rosa_testovoye.Models;
using rosa_testovoye.Services;

namespace rosa_testovoye.Api;

public static class EmployeeEndpoints
{
    public static RouteGroupBuilder MapEmployeeApi(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/employees")
            .WithTags("Employees");

        group.MapGet("/{employeeId:int}/requests", async (
            HttpContext http,
            int employeeId,
            ICertificateRequestService service) =>
        {
            var denied = SessionAuth.RequireLogin(http);
            if (denied is not null)
            {
                return denied;
            }

            return await ApiResultExtensions.ExecuteAsync(async () =>
            {
                var role = SessionAuth.GetRole(http);
                var userId = SessionAuth.GetEmployeeId(http)!.Value;

                if (role == UserRole.Employee && employeeId != userId)
                {
                    return Results.Forbid();
                }

                return Results.Ok(await service.GetByEmployeeAsync(employeeId));
            });
        });

        group.MapGet("/{employeeId:int}/requests/similar", async (
            HttpContext http,
            int employeeId,
            CertificateType type,
            ICertificateRequestService service) =>
        {
            var denied = SessionAuth.RequireLogin(http);
            if (denied is not null)
            {
                return denied;
            }

            return await ApiResultExtensions.ExecuteAsync(async () =>
            {
                var role = SessionAuth.GetRole(http);
                var userId = SessionAuth.GetEmployeeId(http)!.Value;

                if (role == UserRole.Employee && employeeId != userId)
                {
                    return Results.Forbid();
                }

                return Results.Ok(await service.CheckSimilarActiveAsync(employeeId, type));
            });
        });

        return group;
    }
}
