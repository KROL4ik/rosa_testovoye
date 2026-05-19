using rosa_testovoye.Models;
using rosa_testovoye.Services;

namespace rosa_testovoye.Api;

public static class CertificateRequestEndpoints
{
    public static RouteGroupBuilder MapCertificateRequestApi(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/requests")
            .WithTags("Certificate requests");

        group.MapGet("/", async (ICertificateRequestService service) =>
            await ApiResultExtensions.ExecuteAsync(async () =>
                Results.Ok(await service.GetQueueAsync())));

        group.MapGet("/{id:int}", async (int id, ICertificateRequestService service) =>
            await ApiResultExtensions.ExecuteAsync(async () =>
            {
                var details = await service.GetDetailsAsync(id);
                return details is null
                    ? Results.NotFound(new { error = $"Заявка #{id} не найдена." })
                    : Results.Ok(details);
            }));

        group.MapPost("/", async (CreateCertificateRequestInput input, ICertificateRequestService service) =>
            await ApiResultExtensions.ExecuteAsync(async () =>
            {
                var result = await service.CreateAsync(input);
                return Results.Created($"/api/requests/{result.Request.Id}", result);
            }));

        group.MapPatch("/{id:int}/status", async (
            int id,
            ChangeStatusInput input,
            [Microsoft.AspNetCore.Mvc.FromQuery] int accountantId,
            ICertificateRequestService service) =>
            await ApiResultExtensions.ExecuteAsync(async () =>
                Results.Ok(await service.ChangeStatusAsync(id, input, accountantId))));

        return group;
    }
}
