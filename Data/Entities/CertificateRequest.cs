using rosa_testovoye.Data.Enums;

namespace rosa_testovoye.Data.Entities;

public class CertificateRequest
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public Employee Employee { get; set; } = null!;

    public CertificateType Type { get; set; }

    public int CopiesCount { get; set; }

    public required string Reason { get; set; }

    public string? CustomTypeName { get; set; }

    public RequestStatus Status { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime UpdatedAtUtc { get; set; }

    public ICollection<RequestStatusHistory> StatusHistory { get; set; } = [];
}
