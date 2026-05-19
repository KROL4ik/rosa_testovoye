using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using rosa_testovoye.Services;

namespace rosa_testovoye.Pages.Account;

public class LogoutModel : PageModel
{
    public IActionResult OnPost()
    {
        UserSession.Clear(HttpContext.Session);
        return RedirectToPage("/Account/Login");
    }
}
