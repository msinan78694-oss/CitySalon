using CitySalon.Models;
using CitySalon.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CitySalon.Pages.Customers
{
    /// <summary>
    /// PageModel til kundens dashboard.
    /// Bruges til at vise kundens navn og sikre,
    /// at kun kunder kan åbne denne side.
    /// </summary>
    public class DashboardModel : PageModel
    {
        // Service til håndtering af kunde-data.
        private readonly CustomerService _customerService;

        /// <summary>
        /// Constructor injection af CustomerService.
        /// ASP.NET Core sender automatisk CustomerService ind.
        /// </summary>
        public DashboardModel(CustomerService customerService)
        {
            _customerService = customerService;
        }

        /// <summary>
        /// Navnet på den kunde, der er logget ind.
        /// Vises på dashboard-siden.
        /// </summary>
        public string? CustomerName { get; set; }

        /// <summary>
        /// Kører når kunden åbner dashboard-siden.
        /// Tjekker først om brugeren er kunde,
        /// og henter derefter kundens navn.
        /// </summary>
        public IActionResult OnGet()
        {
            // Henter brugerens rolle fra session.
            var role = HttpContext.Session.GetString("Role");

            // Hvis brugeren ikke er en Customer, sendes brugeren tilbage til login.
            if (role != typeof(Customer).Name)
            {
                return RedirectToPage("/Login/LogInPage");
            }

            // Finder den kunde, der er logget ind.
            var customer = GetLoggedInCustomer();

            // Hvis kunden ikke findes, sendes brugeren tilbage til login.
            if (customer == null)
            {
                return RedirectToPage("/Login/LogInPage");
            }

            // Gemmer kundens navn, så det kan vises på dashboardet.
            CustomerName = customer.Name;

            return Page();
        }

        /// <summary>
        /// Finder kunden ud fra det brugernavn, der er gemt i session.
        /// </summary>
        private Customer? GetLoggedInCustomer()
        {
            string? username = HttpContext.Session.GetString("UserName");

            return _customerService.GetAll()
                .FirstOrDefault(c => c.UserName == username);
        }
    }
}