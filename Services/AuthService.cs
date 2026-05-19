using Microsoft.EntityFrameworkCore;
using rosa_testovoye.Data;
using rosa_testovoye.Data.Entities;
using rosa_testovoye.Data.Enums;
using rosa_testovoye.Models;
using rosa_testovoye.Services.Exceptions;

namespace rosa_testovoye.Services;

public class AuthService(AppDbContext context) : IAuthService
{
    public async Task<Employee> RegisterAsync(RegisterInput input)
    {
        var userName = input.UserName.Trim().ToLowerInvariant();
        if (await context.Employees.AnyAsync(e => e.UserName == userName))
        {
            throw new ValidationException("Пользователь с таким именем уже существует.");
        }

        if (!Enum.IsDefined(input.Role))
        {
            throw new ValidationException("Укажите корректную роль.");
        }

        var employee = new Employee
        {
            UserName = userName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(input.Password),
            FullName = input.FullName.Trim(),
            Department = string.IsNullOrWhiteSpace(input.Department) ? null : input.Department.Trim(),
            Role = input.Role
        };

        context.Employees.Add(employee);
        await context.SaveChangesAsync();
        return employee;
    }

    public async Task<Employee?> AuthenticateAsync(string userName, string password)
    {
        var normalized = userName.Trim().ToLowerInvariant();
        var employee = await context.Employees
            .FirstOrDefaultAsync(e => e.UserName == normalized);

        if (employee is null)
        {
            return null;
        }

        return BCrypt.Net.BCrypt.Verify(password, employee.PasswordHash)
            ? employee
            : null;
    }

    public async Task<Employee?> GetByIdAsync(int id) =>
        await context.Employees.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
}
