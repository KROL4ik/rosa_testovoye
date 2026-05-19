using rosa_testovoye.Data.Enums;

namespace rosa_testovoye.Models;

public static class RequestStatusLabels
{
    public static string GetDisplayName(RequestStatus status) => status switch
    {
        RequestStatus.Submitted => "Подана",
        RequestStatus.InProgress => "В работе",
        RequestStatus.Ready => "Готова",
        RequestStatus.Rejected => "Отклонена",
        _ => status.ToString()
    };

    public static string GetBadgeClass(RequestStatus status) => status switch
    {
        RequestStatus.Submitted => "text-bg-secondary",
        RequestStatus.InProgress => "text-bg-primary",
        RequestStatus.Ready => "text-bg-success",
        RequestStatus.Rejected => "text-bg-danger",
        _ => "text-bg-secondary"
    };
}
