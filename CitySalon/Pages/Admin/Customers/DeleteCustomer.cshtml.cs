using CitySalon.Helpers;
using CitySalon.Models;
using CitySalon.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CitySalon.Pages.Customers
{
    /// <summary>
    /// PageModel til sletning af kunder.
    /// Kun administratorer har adgang til siden.
    /// </summary>
    public class DeleteCustomerModel : PageModel
    {
        private readonly CustomerService _customerService;

        /// <summary>
        /// Den kunde som vises på siden før sletning.
        /// </summary>
        public Customer? Customer { get; set; }

        /// <summary>
        /// Opretter DeleteCustomerModel med adgang til CustomerService.
        /// </summary>
        /// <param name="customerService">
        /// Service til håndtering af kunder.
        /// </param>
        public DeleteCustomerModel(CustomerService customerService)
        {
            _customerService = customerService;
        }

        /// <summary>
        /// Henter kunden som skal slettes og viser siden.
        /// </summary>
        /// <param name="id">Id på kunden som skal slettes.</param>
        /// <returns>
        /// Redirecter til login hvis brugeren ikke er admin.
        /// Redirecter til kundelisten hvis kunden ikke findes.
        /// Ellers vises siden.
        /// </returns>
        public IActionResult OnGet(int id)
        {
            if (!CanAccessPage())
            {
                return RedirectToPage("/Login/LogInPage");
            }

            Customer = _customerService.GetById(id);

            if (Customer == null)
            {
                return RedirectToPage("/Admin/Customers/GetAllCustomers");
            }

            return Page();
        }

        /// <summary>
        /// Sletter den valgte kunde.
        /// </summary>
        /// <param name="id">Id på kunden som skal slettes.</param>
        /// <returns>
        /// Redirecter til kundelisten efter sletning.
        /// Returnerer Page() hvis der opstår fejl.
        /// </returns>
        public IActionResult OnPost(int id)
        {
            // Tjekker adgang igen ved POST,
            // da brugeren kan forsøge at sende formularen direkte.
            if (!CanAccessPage())
            {
                return RedirectToPage("/Login/LogInPage");
            }

            try
            {
                _customerService.Delete(id);

                return RedirectToPage("/Admin/Customers/GetAllCustomers");
            }
            catch (Exception ex)
            {
                // Viser fejlbesked fra service-laget.
                ModelState.AddModelError("", ex.Message);

                // Henter kunden igen,
                // så siden stadig kan vise kundeoplysningerne.
                Customer = _customerService.GetById(id);

                if (Customer == null)
                {
                    return RedirectToPage("/Admin/Customers/GetAllCustomers");
                }

                return Page();
            }
        }

        /// <summary>
        /// Tjekker om brugeren er administrator.
        /// </summary>
        /// <returns>
        /// True hvis brugeren har adgang til siden.
        /// </returns>
        private bool CanAccessPage()
        {
            return SessionHelper.IsAdmin(HttpContext);
        }
    }
}