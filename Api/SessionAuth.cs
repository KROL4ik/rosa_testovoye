using rosa_testovoye.Data.Enums;
using rosa_testovoye.Services;

namespace rosa_testovoye.Api;

internal static class SessionAuth
{
    public static int? GetEmployeeId(HttpContext http) =>
        UserSession.GetEmployeeId(http.Session);

    public static UserRole? GetRole(HttpContext http) =>
        UserSession.GetRole(http.Session);

    public static IResult? RequireLogin(HttpContext http)
    {
        if (!UserSession.IsLoggedIn(http.Session))
        {
            return Results.Unauthorized();
        }

        return null;
    }

    public static IResult? RequireAccountant(HttpContext http)
    {
        var login = RequireLogin(http);
        if (login is not null)
        {
            return login;
        }

        return GetRole(http) == UserRole.Accountant ? null : Results.Forbid();
    }
}
