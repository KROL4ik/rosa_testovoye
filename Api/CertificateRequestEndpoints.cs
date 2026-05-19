using rosa_testovoye.Data.Enums;
using rosa_testovoye.Models;
using rosa_testovoye.Services;

namespace rosa_testovoye.Api;

public static class CertificateRequestEndpoints
{
    public static RouteGroupBuilder MapCertificateRequestApi(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/requests")
            .WithTags("Certificate requests");

        group.MapGet("/", async (HttpContext http, ICertificateRequestService service) =>
        {
            var denied = SessionAuth.RequireAccountant(http);
            if (denied is not null)
            {
                return denied;
            }

            return await ApiResultExtensions.ExecuteAsync(async () =>
                Results.Ok(await service.GetQueueAsync()));
        });

        group.MapGet("/{id:int}", async (HttpContext http, int id, ICertificateRequestService service) =>
        {
            var denied = SessionAuth.RequireLogin(http);
            if (denied is not null)
            {
                return denied;
            }

            return await ApiResultExtensions.ExecuteAsync(async () =>
            {
                var details = await service.GetDetailsAsync(id);
                if (details is null)
                {
                    return Results.NotFound(new { error = $"Заявка #{id} не найдена." });
                }

                var role = SessionAuth.GetRole(http);
                var userId = SessionAuth.GetEmployeeId(http)!.Value;
                if (role == UserRole.Employee && details.EmployeeId != userId)
                {
                    return Results.Forbid();
                }

                return Results.Ok(details);
            });
        });

        group.MapPost("/", async (HttpContext http, CreateCertificateRequestInput input, ICertificateRequestService service) =>
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

                if (role == UserRole.Employee)
                {
                    input.EmployeeId = userId;
                }

                var result = await service.CreateAsync(input);
                return Results.Created($"/api/requests/{result.Request.Id}", result);
            });
        });

        group.MapPatch("/{id:int}/status", async (
            HttpContext http,
            int id,
            ChangeStatusInput input,
            ICertificateRequestService service) =>
        {
            var denied = SessionAuth.RequireAccountant(http);
            if (denied is not null)
            {
                return denied;
            }

            return await ApiResultExtensions.ExecuteAsync(async () =>
            {
                var accountantId = SessionAuth.GetEmployeeId(http)!.Value;
                return Results.Ok(await service.ChangeStatusAsync(id, input, accountantId));
            });
        });

        return group;
    }
}
