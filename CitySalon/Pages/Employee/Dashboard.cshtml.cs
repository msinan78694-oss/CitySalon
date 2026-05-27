using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CitySalon.Helpers;

namespace CitySalon.Pages.Employee
{
    /// <summary>
    /// PageModel til medarbejder-dashboardet.
    /// Sørger for at kun medarbejdere kan åbne siden.
    /// </summary>
    public class DashboardModel : PageModel
    {
        /// <summary>
        /// Denne metode kører, når employee åbner dashboard-siden.
        /// Tjekker om brugeren har Employee-rollen.
        /// </summary>
        public IActionResult OnGet()
        {
            // Tjekker om den indloggede bruger har Employee-rollen.
            if (!SessionHelper.IsEmployee(HttpContext))
            {
                // Hvis brugeren ikke er employee,
                // sendes brugeren til login-siden.
                return RedirectToPage("/Login/LogInPage");
            }

            // Hvis brugeren er employee,
            // vises dashboard-siden.
            return Page();
        }
    }
}