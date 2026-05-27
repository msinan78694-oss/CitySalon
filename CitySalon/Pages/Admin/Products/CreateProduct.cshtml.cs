using CitySalon.Helpers;
using CitySalon.Models;
using CitySalon.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CitySalon.Pages.Products
{
    /// <summary>
    /// PageModel til oprettelse af produkter.
    /// Kun administratorer har adgang til siden.
    /// </summary>
    public class CreateProductModel : PageModel
    {
        private readonly ProductService _productService;

        /// <summary>
        /// Binder formularens inputfelter til Product-objektet ved POST.
        /// </summary>
        [BindProperty]
        public Product Product { get; set; } = new Product();

        /// <summary>
        /// Opretter CreateProductModel med adgang til ProductService.
        /// </summary>
        /// <param name="productService">
        /// Service til håndtering af produkter.
        /// </param>
        public CreateProductModel(ProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Viser siden til oprettelse af produkter.
        /// </summary>
        /// <returns>
        /// Redirecter til login hvis brugeren ikke er admin.
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
        /// Opretter et nyt produkt i systemet.
        /// </summary>
        /// <returns>
        /// Redirecter til produktlisten hvis oprettelsen lykkes.
        /// Returnerer Page() hvis validation eller fejl opstår.
        /// </returns>
        public IActionResult OnPost()
        {
            if (!CanAccessPage())
            {
                return RedirectToPage("/Login/LogInPage");
            }

            // Hvis validation fejler,
            // vises siden igen.
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Sender produktet til service-laget.
                _productService.Add(Product);

                return RedirectToPage("/Admin/Products/GetAllProducts");
            }
            catch (Exception ex)
            {
                // Viser fejlbesked fra service-laget.
                ModelState.AddModelError("", ex.Message);

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