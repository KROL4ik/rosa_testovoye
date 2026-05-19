using rosa_testovoye.Services.Exceptions;

namespace rosa_testovoye.Api;

internal static class ApiResultExtensions
{
    public static IResult FromServiceException(ServiceException ex) => ex switch
    {
        ValidationException => Results.BadRequest(new { error = ex.Message }),
        NotFoundException => Results.NotFound(new { error = ex.Message }),
        UnauthorizedRoleException => Results.Json(new { error = ex.Message }, statusCode: StatusCodes.Status403Forbidden),
        InvalidStatusTransitionException => Results.Conflict(new { error = ex.Message }),
        _ => Results.Problem(ex.Message)
    };

    public static async Task<IResult> ExecuteAsync(Func<Task<IResult>> action)
    {
        try
        {
            return await action();
        }
        catch (ServiceException ex)
        {
            return FromServiceException(ex);
        }
    }
}
