using rosa_testovoye.Data.Entities;
using rosa_testovoye.Models;

namespace rosa_testovoye.Services;

public interface IAuthService
{
    Task<Employee> RegisterAsync(RegisterInput input);

    Task<Employee?> AuthenticateAsync(string userName, string password);

    Task<Employee?> GetByIdAsync(int id);
}
