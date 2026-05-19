using rosa_testovoye.Data.Enums;

namespace rosa_testovoye.Services.Exceptions;

public sealed class InvalidStatusTransitionException(RequestStatus from, RequestStatus to)
    : ServiceException($"Недопустимый переход статуса: {from} → {to}.");
