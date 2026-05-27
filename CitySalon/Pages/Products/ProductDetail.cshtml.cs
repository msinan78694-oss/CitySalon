using CitySalon.Models;
using CitySalon.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CitySalon.Pages.Products
{
    /// <summary>
    /// PageModel til produktdetalje-siden.
    /// Bruges til at vise ét bestemt produkt
    /// og en liste med lignende produkter.
    /// </summary>
    public class ProductDetailModel : PageModel
    {
        // Service bruges til at hente produkter.
        private readonly ProductService _productService;

        /// <summary>
        /// Indeholder det valgte produkt,
        /// som skal vises på detaljesiden.
        /// </summary>
        public Product Product { get; set; } = new Product();

        /// <summary>
        /// Indeholder lignende produkter,
        /// som vises nederst på siden.
        /// </summary>
        public List<Product> RelatedProducts { get; set; } = new();

        /// <summary>
        /// Constructor injection af ProductService.
        /// ASP.NET Core sender automatisk ProductService ind.
        /// </summary>
        public ProductDetailModel(ProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Kører når siden åbnes.
        /// id kommer fra URL'en og bruges til at finde produktet.
        /// </summary>
        public IActionResult OnGet(int id)
        {
            try
            {
                // Henter produktet ud fra id.
                Product = _productService.GetById(id);

                // Hvis produktet ikke findes, sendes brugeren tilbage.
                if (Product == null)
                {
                    return RedirectToPage("GetAllProducts");
                }

                // Henter lignende produkter.
                LoadRelatedProducts(id);

                return Page();
            }
            catch (Exception)
            {
                // Hvis der sker fejl, sendes brugeren tilbage.
                return RedirectToPage("GetAllProducts");
            }
        }

        /// <summary>
        /// Finder andre produkter som kan vises nederst på siden.
        /// Det nuværende produkt fjernes fra listen.
        /// </summary>
        private void LoadRelatedProducts(int id)
        {
            RelatedProducts = _productService.GetAll()

                // Fjerner det nuværende produkt.
                .Where(p => p.ProductId != id)

                // Sorterer efter produkter med nærmest id.
                .OrderBy(p => Math.Abs(p.ProductId - id))

                // Tager kun 5 produkter.
                .Take(5)

                // Laver resultatet om til en liste.
                .ToList();
        }
    }
}