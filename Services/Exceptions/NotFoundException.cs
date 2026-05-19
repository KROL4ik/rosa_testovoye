namespace rosa_testovoye.Services.Exceptions;

public sealed class NotFoundException(string message) : ServiceException(message);
