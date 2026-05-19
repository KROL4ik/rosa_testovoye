using rosa_testovoye.Data.Enums;

namespace rosa_testovoye.Models;

public class CertificateRequestDetails
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public string EmployeeFullName { get; set; } = string.Empty;

    public string? EmployeeDepartment { get; set; }

    public CertificateType Type { get; set; }

    public int CopiesCount { get; set; }

    public string Reason { get; set; } = string.Empty;

    public string? CustomTypeName { get; set; }

    public RequestStatus Status { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime UpdatedAtUtc { get; set; }

    public SimilarRequestWarning? SimilarActiveWarning { get; set; }

    public IReadOnlyList<StatusHistoryItem> StatusHistory { get; set; } = [];
}
