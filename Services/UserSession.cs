using rosa_testovoye.Data.Enums;

namespace rosa_testovoye.Services;

public static class UserSession
{
    public static void SetUser(ISession session, int employeeId, UserRole role)
    {
        session.SetInt32(SessionKeys.EmployeeId, employeeId);
        session.SetInt32("Role", (int)role);
    }

    public static void Clear(ISession session)
    {
        session.Remove(SessionKeys.EmployeeId);
        session.Remove("Role");
    }

    public static int? GetEmployeeId(ISession session) =>
        session.GetInt32(SessionKeys.EmployeeId);

    public static UserRole? GetRole(ISession session)
    {
        var value = session.GetInt32("Role");
        return value.HasValue && Enum.IsDefined((UserRole)value.Value)
            ? (UserRole)value.Value
            : null;
    }

    public static bool IsLoggedIn(ISession session) =>
        GetEmployeeId(session).HasValue;
}
