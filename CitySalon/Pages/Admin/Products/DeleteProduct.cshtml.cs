using CitySalon.Helpers;
using CitySalon.Models;
using CitySalon.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CitySalon.Pages.Products
{
    /// <summary>
    /// PageModel til sletning af produkter.
    /// Kun administratorer har adgang til siden.
    /// </summary>
    public class DeleteProductModel : PageModel
    {
        private readonly ProductService _productService;

        /// <summary>
        /// Produktet som vises på siden før sletning.
        /// </summary>
        [BindProperty]
        public Product Product { get; set; } = new Product();

        /// <summary>
        /// Opretter DeleteProductModel med adgang til ProductService.
        /// </summary>
        /// <param name="productService">
        /// Service til håndtering af produkter.
        /// </param>
        public DeleteProductModel(ProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Henter produktet som skal slettes og viser siden.
        /// </summary>
        /// <param name="id">Id på produktet som skal slettes.</param>
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
        /// Sletter det valgte produkt.
        /// </summary>
        /// <param name="id">Id på produktet som skal slettes.</param>
        /// <returns>
        /// Redirecter til produktlisten efter sletning.
        /// Returnerer Page() hvis der opstår fejl.
        /// </returns>
        public IActionResult OnPost(int id)
        {
            if (!CanAccessPage())
            {
                return RedirectToPage("/Login/LogInPage");
            }

            try
            {
                // Sletter produktet ud fra id'et fra route.
                _productService.Delete(id);

                return RedirectToPage("/Admin/Products/GetAllProducts");
            }
            catch (Exception ex)
            {
                // Viser fejlbesked fra service-laget.
                ModelState.AddModelError("", ex.Message);

                return LoadProductOrRedirect(id);
            }
        }

        /// <summary>
        /// Henter produktet igen hvis sletning fejler.
        /// </summary>
        /// <param name="id">Id på produktet.</param>
        /// <returns>
        /// Returnerer Page() hvis produktet findes.
        /// Ellers redirectes der til produktlisten.
        /// </returns>
        private IActionResult LoadProductOrRedirect(int id)
        {
            try
            {
                Product = _productService.GetById(id);

                return Page();
            }
            catch
            {
                return RedirectToPage("/Admin/Products/GetAllProducts");
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