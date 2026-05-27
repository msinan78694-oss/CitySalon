using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CitySalon.Pages.Login
{
    /// <summary>
    /// PageModel til logout.
    /// Bruges til at logge brugeren ud af systemet.
    /// </summary>
    public class LogOutPageModel : PageModel
    {
        /// <summary>
        /// Logger brugeren ud og sender brugeren tilbage til forsiden.
        /// Session.Clear() fjerner alle session-data,
        /// så brugeren ikke længere er logget ind.
        /// </summary>
        public IActionResult OnGet()
        {
            // Fjerner alle værdier gemt i session.
            HttpContext.Session.Clear();

            // Sender brugeren tilbage til forsiden.
            return RedirectToPage("/Index");
        }
    }
}