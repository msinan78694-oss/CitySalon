using CitySalon.Data;
using CitySalon.Models;
using Microsoft.EntityFrameworkCore;

namespace CitySalon.Repositories
{
    /// <summary>
    /// Repository til Product-klassen.
    /// Klassen bruges til at hente, oprette,
    /// opdatere, søge og slette produkter i databasen.
    /// </summary>
    public class ProductRepository : IProductRepository
    {
        // DbContext bruges til at arbejde med databasen via Entity Framework Core.
        private readonly CityDbContext _context;

        /// <summary>
        /// Constructor injection af CityDbContext.
        /// ASP.NET Core sender automatisk database-contexten ind.
        /// </summary>
        public ProductRepository(CityDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Henter alle produkter fra databasen.
        /// </summary>
        public List<Product> GetAll()
        {
            return _context.Products.ToList();
        }

        /// <summary>
        /// Henter alle produkter asynkront fra databasen.
        /// async/await gør at programmet kan arbejde videre,
        /// mens databasen henter data.
        /// </summary>
        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products.ToListAsync();
        }

        /// <summary>
        /// Finder ét produkt ud fra produktets id.
        /// Find returnerer null hvis produktet ikke findes.
        /// </summary>
        public Product GetById(int id)
        {
            return _context.Products.Find(id);
        }

        /// <summary>
        /// Opretter et nyt produkt i databasen.
        /// SaveChanges gemmer ændringen permanent.
        /// </summary>
        public void Add(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
        }

        /// <summary>
        /// Opdaterer et eksisterende produkt i databasen.
        /// Først findes produktet, derefter opdateres felterne.
        /// </summary>
        public void Update(Product product)
        {
            var existingProduct = _context.Products.Find(product.ProductId);

            if (existingProduct != null)
            {
                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.Type = product.Type;
                existingProduct.Size = product.Size;
                existingProduct.Brand = product.Brand;
                existingProduct.Description = product.Description;
                existingProduct.ImageUrl = product.ImageUrl;

                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Søger efter produkter ud fra produktnavn.
        /// Hvis søgefeltet er tomt, returneres alle produkter.
        /// </summary>
        public List<Product> NameSearch(string searchString)
        {
            // Hvis søgefeltet er tomt,
            // returneres alle produkter.
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return GetAll();
            }

            List<Product> searchResults = new List<Product>();

            // Gennemgår alle produkter.
            foreach (Product product in GetAll())
            {
                // Tjekker om produktnavnet indeholder søgeteksten.
                if (!string.IsNullOrWhiteSpace(product.Name) &&
                    product.Name.ToLower().Contains(searchString.ToLower()))
                {
                    searchResults.Add(product);
                }
            }

            return searchResults;
        }


        /// <summary>
        /// Sletter et produkt fra databasen ud fra produktets id.
        /// </summary>
        public void Delete(int id)
        {
            var product = _context.Products.Find(id);

            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
        }
    }
}