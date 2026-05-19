using rosa_testovoye.Data.Enums;

namespace rosa_testovoye.Services;

public static class RequestStatusRules
{
    public static bool IsTerminal(RequestStatus status) =>
        status is RequestStatus.Ready or RequestStatus.Rejected;

    public static bool IsActive(RequestStatus status) =>
        status is RequestStatus.Submitted or RequestStatus.InProgress;

    public static bool CanTransition(RequestStatus from, RequestStatus to)
    {
        if (from == to)
        {
            return false;
        }

        return from switch
        {
            RequestStatus.Submitted => to is RequestStatus.InProgress or RequestStatus.Rejected,
            RequestStatus.InProgress => to is RequestStatus.Ready or RequestStatus.Rejected,
            _ => false
        };
    }
}
