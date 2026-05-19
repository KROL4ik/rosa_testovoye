using System.Text.Json;
using rosa_testovoye.Data.Enums;
using rosa_testovoye.Models;

namespace rosa_testovoye.Pages.Home;

public sealed class EmployeeHomeScriptConfig
{
    public int CustomType { get; init; }

    public int EmployeeId { get; init; }

    public Dictionary<int, string> TypeLabels { get; init; } = [];

    public Dictionary<int, string> StatusLabels { get; init; } = [];

    public Dictionary<int, string> StatusBadgeClasses { get; init; } = [];

    public static EmployeeHomeScriptConfig Create(int employeeId) => new()
    {
        CustomType = (int)CertificateType.Custom,
        EmployeeId = employeeId,
        TypeLabels = Enum.GetValues<CertificateType>()
            .ToDictionary(t => (int)t, CertificateTypeLabels.GetDisplayName),
        StatusLabels = Enum.GetValues<RequestStatus>()
            .ToDictionary(s => (int)s, RequestStatusLabels.GetDisplayName),
        StatusBadgeClasses = Enum.GetValues<RequestStatus>()
            .ToDictionary(s => (int)s, RequestStatusLabels.GetBadgeClass)
    };

    public string ToJson() => JsonSerializer.Serialize(this);
}
