using CitySalon.Helpers;
using CitySalon.Models;
using CitySalon.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CitySalon.Pages.Customers
{
    /// <summary>
    /// PageModel til redigering af kunder.
    /// Kun administratorer og medarbejdere har adgang til siden.
    /// </summary>
    public class EditCustomerModel : PageModel
    {
        private readonly CustomerService _customerService;

        /// <summary>
        /// Binder formularens inputfelter til Customer-objektet ved POST.
        /// </summary>
        [BindProperty]
        public Customer Customer { get; set; } = new Customer();

        /// <summary>
        /// Opretter EditCustomerModel med adgang til CustomerService.
        /// </summary>
        /// <param name="customerService">Service til håndtering af kunder.</param>
        public EditCustomerModel(CustomerService customerService)
        {
            _customerService = customerService;
        }

        /// <summary>
        /// Henter kunden som skal redigeres og viser siden.
        /// </summary>
        /// <param name="id">Id på kunden som skal redigeres.</param>
        /// <returns>
        /// Redirecter til login hvis brugeren ikke har adgang.
        /// Redirecter til kundelisten hvis kunden ikke findes.
        /// Ellers vises siden.
        /// </returns>
        public IActionResult OnGet(int id)
        {
            if (!CanAccessPage())
            {
                return RedirectToPage("/Login/LogInPage");
            }

            try
            {
                Customer = _customerService.GetById(id);

                return Page();
            }
            catch (Exception)
            {
                return RedirectToPage("/Admin/Customers/GetAllCustomers");
            }
        }

        /// <summary>
        /// Opdaterer den valgte kunde.
        /// </summary>
        /// <returns>
        /// Redirecter til kundelisten hvis opdateringen lykkes.
        /// Returnerer Page() hvis validation eller fejl opstår.
        /// </returns>
        public IActionResult OnPost()
        {
            if (!CanAccessPage())
            {
                return RedirectToPage("/Login/LogInPage");
            }

            // Rollen sættes automatisk, så brugeren ikke selv bestemmer rollen.
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
                _customerService.Update(Customer);

                return RedirectToPage("/Admin/Customers/GetAllCustomers");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                return Page();
            }
        }

        /// <summary>
        /// Tjekker om brugeren er admin eller medarbejder.
        /// </summary>
        /// <returns>True hvis brugeren har adgang til siden.</returns>
        private bool CanAccessPage()
        {
            return SessionHelper.IsAdminOrEmployee(HttpContext);
        }
    }
}