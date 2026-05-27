using CitySalon.Helpers;
using CitySalon.Models;
using CitySalon.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CitySalon.Pages.Products
{
    /// <summary>
    /// PageModel til redigering af produkter.
    /// Kun administratorer har adgang til siden.
    /// </summary>
    public class EditProductModel : PageModel
    {
        private readonly ProductService _productService;

        /// <summary>
        /// Binder formularens inputfelter til Product-objektet ved POST.
        /// </summary>
        [BindProperty]
        public Product Product { get; set; } = new Product();

        /// <summary>
        /// Opretter EditProductModel med adgang til ProductService.
        /// </summary>
        /// <param name="productService">
        /// Service til håndtering af produkter.
        /// </param>
        public EditProductModel(ProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Henter produktet som skal redigeres og viser siden.
        /// </summary>
        /// <param name="id">Id på produktet som skal redigeres.</param>
        /// <returns>
        /// Redirecter til login hvis brugeren ikke er admin.
        /// Redirecter til produktlisten hvis produktet ikke findes.
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
                Product = _productService.GetById(id);

                return Page();
            }
            catch (Exception)
            {
                return RedirectToPage("/Admin/Products/GetAllProducts");
            }
        }

        /// <summary>
        /// Gemmer ændringer på det valgte produkt.
        /// </summary>
        /// <returns>
        /// Redirecter til produktlisten hvis opdateringen lykkes.
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
                // Sender det opdaterede produkt til service-laget.
                _productService.Update(Product);

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