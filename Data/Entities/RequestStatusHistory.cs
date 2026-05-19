using rosa_testovoye.Data.Enums;

namespace rosa_testovoye.Data.Entities;

public class RequestStatusHistory
{
    public int Id { get; set; }

    public int RequestId { get; set; }

    public CertificateRequest Request { get; set; } = null!;

    public RequestStatus? FromStatus { get; set; }

    public RequestStatus ToStatus { get; set; }

    public int ChangedByEmployeeId { get; set; }

    public Employee ChangedByEmployee { get; set; } = null!;

    public string? Comment { get; set; }

    public DateTime ChangedAtUtc { get; set; }
}
