using CitySalon.Models;
using CitySalon.Repositories;

namespace CitySalon.Service
{
    /// <summary>
    /// Service-klasse til Product.
    /// Klassen indeholder forretningslogik for produkter
    /// og kalder repository-laget for databasearbejde.
    /// </summary>
    public class ProductService
    {
        // Repository bruges til at hente og gemme produkter i databasen.
        private readonly IProductRepository _productRepository;

        /// <summary>
        /// Constructor injection af IProductRepository.
        /// ASP.NET Core sender automatisk repository ind.
        /// </summary>
        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        /// <summary>
        /// Henter alle produkter.
        /// </summary>
        public List<Product> GetAll()
        {
            return _productRepository.GetAll();
        }

        /// <summary>
        /// Henter alle produkter asynkront.
        /// async/await gør at programmet kan arbejde videre,
        /// mens databasen henter data.
        /// </summary>
        public async Task<List<Product>> GetAllAsync()
        {
            return await _productRepository.GetAllAsync();
        }

        /// <summary>
        /// Finder ét produkt ud fra produktets id.
        /// Tjekker først om id'et er gyldigt.
        /// </summary>
        public Product GetById(int id)
        {
            if (id <= 0)
            {
                throw new Exception("Ugyldigt produkt-id.");
            }

            var product = _productRepository.GetById(id);

            if (product == null)
            {
                throw new Exception("Produktet blev ikke fundet.");
            }

            return product;
        }

        /// <summary>
        /// Opretter et nyt produkt.
        /// Først valideres produktet,
        /// derefter gemmes produktet i databasen.
        /// </summary>
        public void Add(Product product)
        {
            ValidateProduct(product);

            _productRepository.Add(product);
        }

        /// <summary>
        /// Opdaterer et eksisterende produkt.
        /// Først valideres produktet,
        /// derefter tjekkes det at produktet findes.
        /// </summary>
        public void Update(Product product)
        {
            ValidateProduct(product);

            if (product.ProductId <= 0)
            {
                throw new Exception("Ugyldigt produkt-id.");
            }

            GetById(product.ProductId);

            _productRepository.Update(product);
        }

        /// <summary>
        /// Sletter et produkt ud fra id.
        /// GetById bruges først for at sikre,
        /// at produktet findes.
        /// </summary>
        public void Delete(int id)
        {
            GetById(id);

            _productRepository.Delete(id);
        }

        /// <summary>
        /// Søger efter produkter ud fra produktnavn.
        /// Returnerer en liste med matchende produkter.
        /// </summary>
        public List<Product> NameSearch(string searchString)
        {
            return _productRepository.NameSearch(searchString);
        }

        /// <summary>
        /// Tjekker at produktet har gyldige oplysninger.
        /// Bruges før produktet gemmes eller opdateres.
        /// </summary>
        private void ValidateProduct(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product), "Produkt mangler.");
            }

            if (string.IsNullOrWhiteSpace(product.Name))
            {
                throw new Exception("Produktnavn skal udfyldes.");
            }

            if (product.Price <= 0)
            {
                throw new Exception("Pris skal være større end 0.");
            }

            if (string.IsNullOrWhiteSpace(product.Type))
            {
                throw new Exception("Type skal udfyldes.");
            }

            if (string.IsNullOrWhiteSpace(product.Size))
            {
                throw new Exception("Størrelse skal udfyldes.");
            }

            if (string.IsNullOrWhiteSpace(product.Brand))
            {
                throw new Exception("Brand skal udfyldes.");
            }

            if (string.IsNullOrWhiteSpace(product.Description))
            {
                throw new Exception("Beskrivelse skal udfyldes.");
            }
        }
    }
}