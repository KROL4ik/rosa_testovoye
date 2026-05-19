using rosa_testovoye.Data.Enums;
using rosa_testovoye.Models;

namespace rosa_testovoye.Services;

public interface ICertificateRequestService
{
    Task<SimilarRequestWarning> CheckSimilarActiveAsync(int employeeId, CertificateType type);

    Task<CreateCertificateRequestResult> CreateAsync(CreateCertificateRequestInput input);

    Task<IReadOnlyList<CertificateRequestListItem>> GetByEmployeeAsync(int employeeId);

    Task<IReadOnlyList<CertificateRequestListItem>> GetQueueAsync();

    Task<CertificateRequestDetails?> GetDetailsAsync(int requestId);

    Task<CertificateRequestDetails> ChangeStatusAsync(
        int requestId,
        ChangeStatusInput input,
        int accountantEmployeeId);
}
