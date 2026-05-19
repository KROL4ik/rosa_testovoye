using rosa_testovoye.Data.Enums;
using rosa_testovoye.Models;
using rosa_testovoye.Services;

namespace rosa_testovoye.Pages.Home;

public sealed class AccountantHomeScriptConfig
{
    public int CustomType { get; init; }

    public int AccountantId { get; init; }

    public Dictionary<int, string> TypeLabels { get; init; } = [];

    public Dictionary<int, string> StatusLabels { get; init; } = [];

    public Dictionary<int, string> StatusBadgeClasses { get; init; } = [];

    public Dictionary<int, int[]> AllowedTransitions { get; init; } = [];

    public static AccountantHomeScriptConfig Create(int accountantId) => new()
    {
        CustomType = (int)CertificateType.Custom,
        AccountantId = accountantId,
        TypeLabels = Enum.GetValues<CertificateType>()
            .ToDictionary(t => (int)t, CertificateTypeLabels.GetDisplayName),
        StatusLabels = Enum.GetValues<RequestStatus>()
            .ToDictionary(s => (int)s, RequestStatusLabels.GetDisplayName),
        StatusBadgeClasses = Enum.GetValues<RequestStatus>()
            .ToDictionary(s => (int)s, RequestStatusLabels.GetBadgeClass),
        AllowedTransitions = BuildAllowedTransitions()
    };

    public string ToJson() => HomeScriptJson.Serialize(this);

    private static Dictionary<int, int[]> BuildAllowedTransitions()
    {
        return Enum.GetValues<RequestStatus>()
            .ToDictionary(
                from => (int)from,
                from => Enum.GetValues<RequestStatus>()
                    .Where(to => RequestStatusRules.CanTransition(from, to))
                    .Select(to => (int)to)
                    .ToArray());
    }
}
