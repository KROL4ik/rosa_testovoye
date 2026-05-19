using rosa_testovoye.Data.Enums;

namespace rosa_testovoye.Models;

public class ChangeStatusInput
{
    public RequestStatus NewStatus { get; set; }

    public string? Comment { get; set; }
}
