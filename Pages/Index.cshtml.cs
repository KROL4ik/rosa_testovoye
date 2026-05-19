using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using rosa_testovoye.Services;

namespace rosa_testovoye.Pages;

public class IndexModel : PageModel
{
    public IActionResult OnGet()
    {
        if (UserSession.IsLoggedIn(HttpContext.Session))
        {
            return RedirectToPage("/Home/Index");
        }

        return Page();
    }
}
