using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using rosa_testovoye.Data.Enums;
using rosa_testovoye.Services;

namespace rosa_testovoye.Pages.Home;

public class IndexModel(IEmployeeService employeeService) : PageModel
{
    public string RoleTitle { get; private set; } = string.Empty;

    public string UserFullName { get; private set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync(string? role)
    {
        if (!TryParseRole(role, out var userRole))
        {
            return RedirectToPage("/Index");
        }

        var employees = await employeeService.GetAllAsync();
        var user = employees.FirstOrDefault(e => e.Role == userRole);
        if (user is null)
        {
            return RedirectToPage("/Index");
        }

        RoleTitle = userRole == UserRole.Accountant ? "Бухгалтер" : "Сотрудник";
        UserFullName = user.FullName;
        return Page();
    }

    private static bool TryParseRole(string? role, out UserRole userRole)
    {
        userRole = default;
        if (string.IsNullOrWhiteSpace(role))
        {
            return false;
        }

        return role.ToLowerInvariant() switch
        {
            "employee" => Assign(UserRole.Employee, out userRole),
            "accountant" => Assign(UserRole.Accountant, out userRole),
            _ => false
        };
    }

    private static bool Assign(UserRole value, out UserRole userRole)
    {
        userRole = value;
        return true;
    }
}
