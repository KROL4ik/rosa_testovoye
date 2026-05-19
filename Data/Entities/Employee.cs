using rosa_testovoye.Data.Enums;

namespace rosa_testovoye.Data.Entities;

public class Employee
{
    public int Id { get; set; }

    public required string FullName { get; set; }

    public string? Department { get; set; }

    public UserRole Role { get; set; }

    public ICollection<CertificateRequest> Requests { get; set; } = [];

    public ICollection<RequestStatusHistory> StatusChanges { get; set; } = [];
}
