namespace rosa_testovoye.Services.Exceptions;

public sealed class ValidationException(string message) : ServiceException(message);
