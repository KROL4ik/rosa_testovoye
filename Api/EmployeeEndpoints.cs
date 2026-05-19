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

        group.MapGet("/", async (IEmployeeService service) =>
            await ApiResultExtensions.ExecuteAsync(async () =>
                Results.Ok(await service.GetAllAsync())));

        group.MapGet("/{id:int}", async (int id, IEmployeeService service) =>
            await ApiResultExtensions.ExecuteAsync(async () =>
            {
                var employee = await service.GetByIdAsync(id);
                return employee is null
                    ? Results.NotFound(new { error = $"Сотрудник #{id} не найден." })
                    : Results.Ok(employee);
            }));

        group.MapPost("/", async (CreateEmployeeInput input, IEmployeeService service) =>
            await ApiResultExtensions.ExecuteAsync(async () =>
            {
                var created = await service.CreateAsync(input);
                return Results.Created($"/api/employees/{created.Id}", created);
            }));

        group.MapPut("/{id:int}", async (int id, UpdateEmployeeInput input, IEmployeeService service) =>
            await ApiResultExtensions.ExecuteAsync(async () =>
                Results.Ok(await service.UpdateAsync(id, input))));

        group.MapDelete("/{id:int}", async (int id, IEmployeeService service) =>
            await ApiResultExtensions.ExecuteAsync(async () =>
            {
                await service.DeleteAsync(id);
                return Results.NoContent();
            }));

        group.MapGet("/{employeeId:int}/requests", async (int employeeId, ICertificateRequestService service) =>
            await ApiResultExtensions.ExecuteAsync(async () =>
                Results.Ok(await service.GetByEmployeeAsync(employeeId))));

        group.MapGet("/{employeeId:int}/requests/similar", async (
            int employeeId,
            CertificateType type,
            ICertificateRequestService service) =>
            await ApiResultExtensions.ExecuteAsync(async () =>
                Results.Ok(await service.CheckSimilarActiveAsync(employeeId, type))));

        return group;
    }
}
