using rosa_testovoye.Data.Enums;

namespace rosa_testovoye.Models;

public class CreateCertificateRequestInput
{
    public int EmployeeId { get; set; }

    public CertificateType Type { get; set; }

    public int CopiesCount { get; set; }

    public string Reason { get; set; } = string.Empty;

    public string? CustomTypeName { get; set; }
}
