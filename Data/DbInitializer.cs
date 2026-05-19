using Microsoft.EntityFrameworkCore;
using rosa_testovoye.Data.Entities;
using rosa_testovoye.Data.Enums;

namespace rosa_testovoye.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(AppDbContext context)
    {
        if ((await context.Database.GetPendingMigrationsAsync()).Any())
        {
            await context.Database.MigrateAsync();
        }

        if (await context.Employees.AnyAsync())
        {
            return;
        }

        context.Employees.AddRange(
            new Employee
            {
                FullName = "Иванов Иван Иванович",
                Department = "Отдел разработки",
                Role = UserRole.Employee
            },
            new Employee
            {
                FullName = "Сидорова Мария Борисовна",
                Department = "Бухгалтерия",
                Role = UserRole.Accountant
            });

        await context.SaveChangesAsync();
    }
}
