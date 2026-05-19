using rosa_testovoye.Data.Enums;

namespace rosa_testovoye.Models;

public class StatusHistoryItem
{
    public RequestStatus? FromStatus { get; set; }

    public RequestStatus ToStatus { get; set; }

    public string ChangedByFullName { get; set; } = string.Empty;

    public string? Comment { get; set; }

    public DateTime ChangedAtUtc { get; set; }
}
