using rosa_testovoye.Data.Enums;

namespace rosa_testovoye.Models;

public class SimilarRequestWarning
{
    public bool HasSimilarActive { get; set; }

    public int? SimilarRequestId { get; set; }

    public CertificateType? SimilarRequestType { get; set; }

    public RequestStatus? SimilarRequestStatus { get; set; }
}
