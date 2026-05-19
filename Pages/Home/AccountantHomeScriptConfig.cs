using rosa_testovoye.Data.Enums;
using rosa_testovoye.Models;

namespace rosa_testovoye.Pages.Home;

public sealed class AccountantHomeScriptConfig
{
    public int CustomType { get; init; }

    public Dictionary<int, string> TypeLabels { get; init; } = [];

    public Dictionary<int, string> StatusLabels { get; init; } = [];

    public Dictionary<int, string> StatusBadgeClasses { get; init; } = [];

    public static AccountantHomeScriptConfig Create() => new()
    {
        CustomType = (int)CertificateType.Custom,
        TypeLabels = Enum.GetValues<CertificateType>()
            .ToDictionary(t => (int)t, CertificateTypeLabels.GetDisplayName),
        StatusLabels = Enum.GetValues<RequestStatus>()
            .ToDictionary(s => (int)s, RequestStatusLabels.GetDisplayName),
        StatusBadgeClasses = Enum.GetValues<RequestStatus>()
            .ToDictionary(s => (int)s, RequestStatusLabels.GetBadgeClass)
    };

    public string ToJson() => HomeScriptJson.Serialize(this);
}
