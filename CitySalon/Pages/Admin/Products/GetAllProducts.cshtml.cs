using CitySalon.Helpers;
using CitySalon.Models;
using CitySalon.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CitySalon.Pages.Admin.Products
{
    /// <summary>
    /// PageModel til visning af alle produkter.
    /// Kun administratorer har adgang til siden.
    /// </summary>
    public class GetAllProductsModel : PageModel
    {
        private readonly ProductService _productService;

        /// <summary>
        /// Liste med alle produkter som vises på Razor-siden.
        /// </summary>
        public List<Product> Products { get; set; } = new();

        /// <summary>
        /// Minimumspris hentet fra URL'en ved GET requests.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public decimal MinPrice { get; set; }

        /// <summary>
        /// Maksimumspris hentet fra URL'en ved GET requests.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public decimal MaxPrice { get; set; }

        /// <summary>
        /// Opretter GetAllProductsModel med adgang til ProductService.
        /// </summary>
        /// <param name="productService">
        /// Service til håndtering af produkter.
        /// </param>
        public GetAllProductsModel(ProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Henter alle produkter asynkront og viser siden.
        /// </summary>
        /// <returns>
        /// Redirecter til login hvis brugeren ikke er admin.
        /// Ellers vises siden med alle produkter.
        /// </returns>
        public async Task<IActionResult> OnGetAsync()
        {
            // Kun admin må se produktsiden.
            if (!SessionHelper.IsAdmin(HttpContext))
            {
                return RedirectToPage("/Login/LogInPage");
            }

            // await venter på at service-laget henter produkterne.
            Products = await _productService.GetAllAsync();

            return Page();
        }
    }
}