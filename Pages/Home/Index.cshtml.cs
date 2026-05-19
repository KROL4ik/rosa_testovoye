using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using rosa_testovoye.Data.Enums;
using rosa_testovoye.Models;
using rosa_testovoye.Services;
using rosa_testovoye.Services.Exceptions;

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

    [BindProperty]
    public CreateRequestFormModel Form { get; set; } = new();

    [BindProperty]
    public bool ConfirmDuplicate { get; set; }

    public bool ShowDuplicateWarning { get; private set; }

    public string? SuccessMessage { get; private set; }

    public string? ErrorMessage { get; private set; }

    public List<SelectListItem> CertificateTypeOptions { get; private set; } = [];

    public async Task<IActionResult> OnGetAsync(string? role)
    {
        Role ??= role;
        if (!await LoadUserAsync())
        {
            return RedirectToPage("/Index");
        }

        LoadCertificateTypes();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!await LoadUserAsync())
        {
            return RedirectToPage("/Index");
        }

        LoadCertificateTypes();

        if (!IsEmployee)
        {
            return Page();
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (Form.Type == CertificateType.Custom && string.IsNullOrWhiteSpace(Form.CustomTypeName))
        {
            ModelState.AddModelError($"{nameof(Form)}.{nameof(Form.CustomTypeName)}",
                "Укажите название произвольной справки.");
            return Page();
        }

        if (!ConfirmDuplicate)
        {
            var warning = await certificateRequestService.CheckSimilarActiveAsync(EmployeeId, Form.Type);
            if (warning.HasSimilarActive)
            {
                ShowDuplicateWarning = true;
                return Page();
            }
        }

        try
        {
            var input = new CreateCertificateRequestInput
            {
                EmployeeId = EmployeeId,
                Type = Form.Type,
                CopiesCount = Form.CopiesCount,
                Reason = Form.Reason,
                CustomTypeName = Form.CustomTypeName
            };

            var result = await certificateRequestService.CreateAsync(input);
            SuccessMessage = $"Заявка №{result.Request.Id} успешно отправлена.";
            ConfirmDuplicate = false;
            Form = new CreateRequestFormModel { CopiesCount = 1 };

            if (result.SimilarActiveWarning.HasSimilarActive)
            {
                SuccessMessage += " Обратите внимание: у вас уже была активная заявка того же типа.";
            }
        }
        catch (ValidationException ex)
        {
            ErrorMessage = ex.Message;
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
