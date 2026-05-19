using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using rosa_testovoye.Data.Enums;
using rosa_testovoye.Models;
using rosa_testovoye.Services;

namespace rosa_testovoye.Pages.Home;

public class IndexModel(
    IAuthService authService,
    ICertificateRequestService certificateRequestService) : PageModel
{
    public string RoleTitle { get; private set; } = string.Empty;

    public string UserFullName { get; private set; } = string.Empty;

    public int EmployeeId { get; private set; }

    public bool IsEmployee { get; private set; }

    public bool IsAccountant { get; private set; }

    public CreateRequestFormModel Form { get; set; } = new();

    public List<SelectListItem> CertificateTypeOptions { get; private set; } = [];

    public IReadOnlyList<CertificateRequestListItem> MyRequests { get; private set; } = [];

    public string EmployeeHomeConfigJson { get; private set; } = "{}";

    public IReadOnlyList<CertificateRequestListItem> RequestQueue { get; private set; } = [];

    public string AccountantHomeConfigJson { get; private set; } = "{}";

    public async Task<IActionResult> OnGetAsync()
    {
        if (!UserSession.IsLoggedIn(HttpContext.Session))
        {
            return RedirectToPage("/Account/Login");
        }

        var employeeId = UserSession.GetEmployeeId(HttpContext.Session)!.Value;
        var role = UserSession.GetRole(HttpContext.Session);

        if (role is null)
        {
            UserSession.Clear(HttpContext.Session);
            return RedirectToPage("/Account/Login");
        }

        var employee = await authService.GetByIdAsync(employeeId);
        if (employee is null)
        {
            UserSession.Clear(HttpContext.Session);
            return RedirectToPage("/Account/Login");
        }

        EmployeeId = employee.Id;
        UserFullName = employee.FullName;
        IsEmployee = role == UserRole.Employee;
        IsAccountant = role == UserRole.Accountant;
        RoleTitle = IsAccountant ? "Бухгалтер" : "Сотрудник";

        LoadCertificateTypes();

        if (IsEmployee)
        {
            MyRequests = await certificateRequestService.GetByEmployeeAsync(EmployeeId);
            EmployeeHomeConfigJson = EmployeeHomeScriptConfig.Create(EmployeeId).ToJson();
        }
        else
        {
            RequestQueue = await certificateRequestService.GetQueueAsync();
            AccountantHomeConfigJson = AccountantHomeScriptConfig.Create(EmployeeId).ToJson();
        }

        return Page();
    }

    private void LoadCertificateTypes()
    {
        CertificateTypeOptions = Enum.GetValues<CertificateType>()
            .Select(t => new SelectListItem(CertificateTypeLabels.GetDisplayName(t), ((int)t).ToString()))
            .ToList();
    }
}
