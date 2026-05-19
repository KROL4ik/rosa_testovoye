using Microsoft.EntityFrameworkCore;
using rosa_testovoye.Data.Entities;
using rosa_testovoye.Data.Enums;

namespace rosa_testovoye.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(AppDbContext context)
    {
        await context.Database.MigrateAsync();

        var hasAuthUsers = await context.Employees
            .AnyAsync(e => e.UserName != "" && e.PasswordHash.StartsWith("$2"));

        if (hasAuthUsers)
        {
            return;
        }

        await context.Database.ExecuteSqlRawAsync("DELETE FROM RequestStatusHistory");
        await context.Database.ExecuteSqlRawAsync("DELETE FROM CertificateRequests");
        await context.Database.ExecuteSqlRawAsync("DELETE FROM Employees");

        context.Employees.AddRange(
            new Employee
            {
                UserName = "ivanov",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"),
                FullName = "Иванов Иван Иванович",
                Department = "Отдел разработки",
                Role = UserRole.Employee
            },
            new Employee
            {
                UserName = "sidorova",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"),
                FullName = "Сидорова Мария Борисовна",
                Department = "Бухгалтерия",
                Role = UserRole.Accountant
            });

        await context.SaveChangesAsync();
    }
}
