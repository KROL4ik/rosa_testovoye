using rosa_testovoye.Data.Enums;

namespace rosa_testovoye.Models;

public class CertificateRequestListItem
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public string EmployeeFullName { get; set; } = string.Empty;

    public CertificateType Type { get; set; }

    public int CopiesCount { get; set; }

    public RequestStatus Status { get; set; }

    public DateTime CreatedAtUtc { get; set; }
}
