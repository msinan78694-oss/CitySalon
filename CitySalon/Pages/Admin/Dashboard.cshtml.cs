using CitySalon.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CitySalon.Pages.Admin
{
    /// <summary>
    /// PageModel til administratorens dashboard.
    /// Kun administratorer har adgang til siden.
    /// </summary>
    public class DashboardModel : PageModel
    {
        /// <summary>
        /// Viser dashboard-siden hvis brugeren er administrator.
        /// </summary>
        /// <returns>
        /// Redirecter til login hvis brugeren ikke er admin.
        /// Ellers vises dashboardet.
        /// </returns>
        public IActionResult OnGet()
        {
            // Tjekker om den nuværende bruger er admin.
            if (!SessionHelper.IsAdmin(HttpContext))
            {
                return RedirectToPage("/Login/LogInPage");
            }

            return Page();
        }
    }
}