using CitySalon.Data;
using CitySalon.Models;

namespace CitySalon.Repositories
{
    /// <summary>
    /// Repository til Customer-klassen.
    /// Klassen bruges til at hente, oprette,
    /// opdatere og slette kunder i databasen.
    /// </summary>
    public class CustomerRepository : IRepository<Customer>
    {
        // DbContext bruges til at arbejde med databasen via Entity Framework Core.
        private readonly CityDbContext _context;

        /// <summary>
        /// Constructor injection af CityDbContext.
        /// ASP.NET Core sender automatisk database-contexten ind.
        /// </summary>
        public CustomerRepository(CityDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Henter alle kunder fra databasen.
        /// </summary>
        public List<Customer> GetAll()
        {
            return _context.Customers.ToList();
        }

        /// <summary>
        /// Finder én kunde ud fra kundens id.
        /// Find returnerer null hvis kunden ikke findes.
        /// </summary>
        public Customer GetById(int id)
        {
            return _context.Customers.Find(id);
        }

        /// <summary>
        /// Opretter en ny kunde i databasen.
        /// SaveChanges gemmer ændringen permanent.
        /// </summary>
        public void Add(Customer customer)
        {
            _context.Customers.Add(customer);
            _context.SaveChanges();
        }

        /// <summary>
        /// Opdaterer en eksisterende kunde i databasen.
        /// Først findes kunden, derefter opdateres felterne.
        /// </summary>
        public void Update(Customer customer)
        {
            var existingCustomer = _context.Customers.Find(customer.Id);

            if (existingCustomer != null)
            {
                existingCustomer.Name = customer.Name;
                existingCustomer.Phone = customer.Phone;
                existingCustomer.Email = customer.Email;
                existingCustomer.UserName = customer.UserName;
                existingCustomer.Password = customer.Password;

                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Sletter en kunde fra databasen ud fra kundens id.
        /// </summary>
        public void Delete(int id)
        {
            var customer = _context.Customers.Find(id);

            if (customer != null)
            {
                _context.Customers.Remove(customer);
                _context.SaveChanges();
            }
        }
    }
}