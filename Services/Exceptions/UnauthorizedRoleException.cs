namespace rosa_testovoye.Services.Exceptions;

public sealed class UnauthorizedRoleException(string message) : ServiceException(message);
