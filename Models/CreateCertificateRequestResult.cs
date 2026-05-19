namespace rosa_testovoye.Models;

public class CreateCertificateRequestResult
{
    public CertificateRequestDetails Request { get; set; } = null!;

    public SimilarRequestWarning SimilarActiveWarning { get; set; } = new();
}
