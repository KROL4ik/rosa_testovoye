using rosa_testovoye.Data.Enums;

namespace rosa_testovoye.Models;

public class CreateEmployeeInput
{
    public string FullName { get; set; } = string.Empty;

    public string? Department { get; set; }

    public UserRole Role { get; set; } = UserRole.Employee;
}
