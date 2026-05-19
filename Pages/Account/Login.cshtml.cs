using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using rosa_testovoye.Models;
using rosa_testovoye.Services;

namespace rosa_testovoye.Pages.Account;

public class LoginModel(IAuthService authService) : PageModel
{
    [BindProperty]
    public LoginInput Input { get; set; } = new();

    public string? ErrorMessage { get; private set; }

    public IActionResult OnGet()
    {
        if (UserSession.IsLoggedIn(HttpContext.Session))
        {
            return RedirectToPage("/Home/Index");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var employee = await authService.AuthenticateAsync(Input.UserName, Input.Password);
        if (employee is null)
        {
            ErrorMessage = "Неверное имя пользователя или пароль.";
            return Page();
        }

        UserSession.SetUser(HttpContext.Session, employee.Id, employee.Role);
        return RedirectToPage("/Home/Index");
    }
}
