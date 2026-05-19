using rosa_testovoye.Data.Enums;

namespace rosa_testovoye.Models;

public static class CertificateTypeLabels
{
    public static string GetDisplayName(CertificateType type) => type switch
    {
        CertificateType.Ndfl2 => "2-НДФЛ",
        CertificateType.EmploymentAndTenure => "О месте работы и стаже",
        CertificateType.AverageEarnings => "О среднем заработке",
        CertificateType.Custom => "Произвольная справка",
        _ => type.ToString()
    };
}
