using CitySalon.Helpers;
using CitySalon.Models;
using CitySalon.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CitySalon.Pages.Products
{
    /// <summary>
    /// PageModel til produktsiden.
    /// Bruges til at vise alle produkter,
    /// søge efter produkter og opdele produkter efter type/kategori.
    /// </summary>
    public class GetAllProductsModel : PageModel
    {
        // Service bruges til at hente produkter.
        // PageModel skal ikke selv arbejde direkte med databasen.
        private readonly ProductService _productService;

        /// <summary>
        /// Liste med alle produkter, som skal vises på siden.
        /// </summary>
        public List<Product> Products { get; set; } = new();

        /// <summary>
        /// Liste med unikke produkttyper/kategorier.
        /// Bruges til at gruppere produkterne på siden.
        /// </summary>
        public List<string> ProductTypes { get; set; } = new();

        /// <summary>
        /// Søgetekst fra formularen.
        /// BindProperty gør at værdien fra inputfeltet
        /// automatisk bindes til denne property ved POST.
        /// </summary>
        [BindProperty]
        public string SearchString { get; set; } = string.Empty;

        /// <summary>
        /// Constructor injection af ProductService.
        /// ASP.NET Core sender automatisk ProductService ind.
        /// </summary>
        public GetAllProductsModel(ProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Søger efter produkter ud fra navn.
        /// Kører når søgeformularen sendes.
        /// </summary>
        public IActionResult OnPostNameSearch()
        {
            Products = _productService.NameSearch(SearchString);

            LoadProductTypes();

            return Page();
        }

        /// <summary>
        /// Kører når siden åbnes.
        /// Henter produkter asynkront fra databasen.
        /// </summary>
        public async Task OnGetAsync()
        {
            Products = await _productService.GetAllAsync();

            // Finder alle unikke produkttyper.
            LoadProductTypes();
        }

        /// <summary>
        /// Opretter en liste med unikke produkttyper.
        /// Listen sorteres alfabetisk.
        /// </summary>
        private void LoadProductTypes()
        {
            ProductTypes = Products
                .Select(p => p.Type)
                .Distinct()
                .OrderBy(type => type)
                .ToList();
        }
    }
}