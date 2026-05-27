using CitySalon.Helpers;
using CitySalon.Models;
using CitySalon.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CitySalon.Pages.Customers
{
    /// <summary>
    /// PageModel til visning af alle kunder.
    /// Indeholder også søgefunktion til filtrering af kunder.
    /// </summary>
    public class GetAllCustomersModel : PageModel
    {
        private readonly CustomerService _customerService;

        /// <summary>
        /// Liste med kunder som vises på Razor-siden.
        /// </summary>
        public List<Customer> Customers { get; set; } = new();

        /// <summary>
        /// Søgetekst hentet fra URL'en ved GET requests.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        /// <summary>
        /// Opretter GetAllCustomersModel med adgang til CustomerService.
        /// </summary>
        /// <param name="customerService">
        /// Service til håndtering af kunder.
        /// </param>
        public GetAllCustomersModel(CustomerService customerService)
        {
            _customerService = customerService;
        }

        /// <summary>
        /// Henter alle kunder og viser siden.
        /// Søger også i listen hvis brugeren har skrevet en søgetekst.
        /// </summary>
        /// <returns>
        /// Redirecter til login hvis brugeren ikke har adgang.
        /// Ellers vises siden.
        /// </returns>
        public IActionResult OnGet()
        {
            if (!SessionHelper.IsAdminOrEmployee(HttpContext))
            {
                return RedirectToPage("/Login/LogInPage");
            }

            Customers = _customerService.GetAll();

            SearchCustomers();

            return Page();
        }

        /// <summary>
        /// Filtrerer kundelisten ud fra søgeteksten.
        /// </summary>
        private void SearchCustomers()
        {
            // Hvis søgefeltet er tomt,
            // vises alle kunder.
            if (string.IsNullOrWhiteSpace(SearchTerm))
            {
                return;
            }

            // Trim fjerner mellemrum.
            // ToLower gør søgningen case-insensitive.
            string search = SearchTerm.Trim().ToLower();

            // LINQ Where filtrerer listen.
            Customers = Customers
                .Where(c =>
                    c.Name.ToLower().Contains(search) ||
                    c.Phone.ToLower().Contains(search) ||
                    c.Email.ToLower().Contains(search) ||
                    c.UserName.ToLower().Contains(search))
                .ToList();
        }
    }
}