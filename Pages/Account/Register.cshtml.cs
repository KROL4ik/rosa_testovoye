using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using rosa_testovoye.Data.Enums;
using rosa_testovoye.Models;
using rosa_testovoye.Services;
using rosa_testovoye.Services.Exceptions;

namespace rosa_testovoye.Pages.Account;

public class RegisterModel(IAuthService authService) : PageModel
{
    [BindProperty]
    public RegisterInput Input { get; set; } = new();

    public List<SelectListItem> RoleOptions { get; private set; } = [];

    public string? ErrorMessage { get; private set; }

    public IActionResult OnGet()
    {
        if (UserSession.IsLoggedIn(HttpContext.Session))
        {
            return RedirectToPage("/Home/Index");
        }

        LoadRoles();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        LoadRoles();

        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var employee = await authService.RegisterAsync(Input);
            UserSession.SetUser(HttpContext.Session, employee.Id, employee.Role);
            return RedirectToPage("/Home/Index");
        }
        catch (ValidationException ex)
        {
            ErrorMessage = ex.Message;
            return Page();
        }
    }

    private void LoadRoles()
    {
        RoleOptions =
        [
            new SelectListItem("Сотрудник", ((int)UserRole.Employee).ToString()),
            new SelectListItem("Бухгалтер", ((int)UserRole.Accountant).ToString())
        ];
    }
}
