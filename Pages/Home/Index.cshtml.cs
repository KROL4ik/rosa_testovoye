using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using rosa_testovoye.Data.Enums;
using rosa_testovoye.Models;
using rosa_testovoye.Services;

namespace rosa_testovoye.Pages.Home;

public class IndexModel(
    IEmployeeService employeeService,
    ICertificateRequestService certificateRequestService) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string? Role { get; set; }

    public string RoleTitle { get; private set; } = string.Empty;

    public string UserFullName { get; private set; } = string.Empty;

    public int EmployeeId { get; private set; }

    public bool IsEmployee { get; private set; }

    public CreateRequestFormModel Form { get; set; } = new();

    public List<SelectListItem> CertificateTypeOptions { get; private set; } = [];

    public IReadOnlyList<CertificateRequestListItem> MyRequests { get; private set; } = [];

    public string EmployeeHomeConfigJson { get; private set; } = "{}";

    public async Task<IActionResult> OnGetAsync(string? role)
    {
        Role ??= role;
        if (!await LoadUserAsync())
        {
            return RedirectToPage("/Index");
        }

        LoadCertificateTypes();

        if (IsEmployee)
        {
            MyRequests = await certificateRequestService.GetByEmployeeAsync(EmployeeId);
            EmployeeHomeConfigJson = EmployeeHomeScriptConfig.Create(EmployeeId).ToJson();
        }

        return Page();
    }

    private async Task<bool> LoadUserAsync()
    {
        if (!TryParseRole(Role, out var userRole))
        {
            return false;
        }

        var employees = await employeeService.GetAllAsync();
        var user = employees.FirstOrDefault(e => e.Role == userRole);
        if (user is null)
        {
            return false;
        }

        EmployeeId = user.Id;
        RoleTitle = userRole == UserRole.Accountant ? "Бухгалтер" : "Сотрудник";
        UserFullName = user.FullName;
        IsEmployee = userRole == UserRole.Employee;
        Role = userRole == UserRole.Accountant ? "accountant" : "employee";
        return true;
    }

    private void LoadCertificateTypes()
    {
        CertificateTypeOptions = Enum.GetValues<CertificateType>()
            .Select(t => new SelectListItem(CertificateTypeLabels.GetDisplayName(t), ((int)t).ToString()))
            .ToList();
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
