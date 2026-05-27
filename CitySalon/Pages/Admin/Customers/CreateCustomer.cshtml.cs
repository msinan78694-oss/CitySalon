using CitySalon.Helpers;
using CitySalon.Models;
using CitySalon.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CitySalon.Pages.Customers
{
    /// <summary>
    /// PageModel til oprettelse af kunder.
    /// Kun administratorer og medarbejdere har adgang til siden.
    /// </summary>
    public class CreateCustomerModel : PageModel
    {
        private readonly CustomerService _customerService;

        /// <summary>
        /// Binder formularens felter til Customer-objektet ved POST.
        /// </summary>
        [BindProperty]
        public Customer Customer { get; set; } = new Customer();

        /// <summary>
        /// Opretter CreateCustomerModel med adgang til CustomerService.
        /// </summary>
        /// <param name="customerService">
        /// Service til håndtering af kunder.
        /// </param>
        public CreateCustomerModel(CustomerService customerService)
        {
            _customerService = customerService;
        }

        /// <summary>
        /// Viser siden til oprettelse af kunder.
        /// </summary>
        /// <returns>
        /// Redirecter til login hvis brugeren ikke har adgang.
        /// Ellers vises siden.
        /// </returns>
        public IActionResult OnGet()
        {
            if (!CanAccessPage())
            {
                return RedirectToPage("/Login/LogInPage");
            }

            return Page();
        }

        /// <summary>
        /// Opretter en ny kunde i systemet.
        /// </summary>
        /// <returns>
        /// Redirecter til kundelisten hvis oprettelsen lykkes.
        /// Returnerer Page() hvis validation eller fejl opstår.
        /// </returns>
        public IActionResult OnPost()
        {
            // Tjekker adgang igen ved POST,
            // da brugeren kan forsøge at sende formularen direkte.
            if (!CanAccessPage())
            {
                return RedirectToPage("/Login/LogInPage");
            }

            // Rollen sættes automatisk i koden.
            Customer.Role = typeof(Customer).Name;

            // Role findes ikke som inputfelt i formularen.
            // Derfor fjernes den fra ModelState.
            ModelState.Remove("Customer.Role");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Sender kunden videre til service-laget.
                _customerService.Add(Customer);

                return RedirectToPage("/Admin/Customers/GetAllCustomers");
            }
            catch (Exception ex)
            {
                // Viser fejl fra service-laget på siden.
                ModelState.AddModelError("", ex.Message);

                return Page();
            }
        }

        /// <summary>
        /// Tjekker om brugeren er admin eller medarbejder.
        /// </summary>
        /// <returns>
        /// True hvis brugeren har adgang til siden.
        /// </returns>
        private bool CanAccessPage()
        {
            return SessionHelper.IsAdminOrEmployee(HttpContext);
        }
    }
}