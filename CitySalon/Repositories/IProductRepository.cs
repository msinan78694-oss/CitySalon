using CitySalon.Models;

namespace CitySalon.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<List<Product>> GetAllAsync();

        List<Product> NameSearch(string searchString);
    }
}