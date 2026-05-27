using CitySalon.Models;
using CitySalon.Repositories;

namespace CitySalon.Service
{
    /// <summary>
    /// Service-klasse til Customer.
    /// Klassen indeholder forretningslogik for kunder
    /// og kalder repository-laget for databasearbejde.
    /// </summary>
    public class CustomerService
    {
        // Repository bruges til at hente og gemme kunder i databasen.
        private readonly IRepository<Customer> _customerRepository;

        /// <summary>
        /// Constructor injection af IRepository<Customer>.
        /// ASP.NET Core sender automatisk repository ind.
        /// </summary>
        public CustomerService(IRepository<Customer> customerRepository)
        {
            _customerRepository = customerRepository;
        }

        /// <summary>
        /// Henter alle kunder.
        /// </summary>
        public List<Customer> GetAll()
        {
            return _customerRepository.GetAll();
        }

        /// <summary>
        /// Finder én kunde ud fra kundens id.
        /// Tjekker først om id'et er gyldigt.
        /// </summary>
        public Customer GetById(int id)
        {
            if (id <= 0)
            {
                throw new Exception("Ugyldigt kunde-id.");
            }

            var customer = _customerRepository.GetById(id);

            if (customer == null)
            {
                throw new Exception("Kunden blev ikke fundet.");
            }

            return customer;
        }

        /// <summary>
        /// Opretter en ny kunde.
        /// Først valideres kunden, derefter gemmes kunden i databasen.
        /// </summary>
        public void Add(Customer customer)
        {
            ValidateCustomer(customer);

            _customerRepository.Add(customer);
        }

        /// <summary>
        /// Opdaterer en eksisterende kunde.
        /// Først valideres kunden, derefter tjekkes at kunden findes.
        /// </summary>
        public void Update(Customer customer)
        {
            ValidateCustomer(customer);

            if (customer.Id <= 0)
            {
                throw new Exception("Ugyldigt kunde-id.");
            }

            GetById(customer.Id);

            _customerRepository.Update(customer);
        }

        /// <summary>
        /// Sletter en kunde ud fra id.
        /// GetById bruges først for at sikre, at kunden findes.
        /// </summary>
        public void Delete(int id)
        {
            GetById(id);

            _customerRepository.Delete(id);
        }

        /// <summary>
        /// Tjekker at kunden har gyldige oplysninger.
        /// Bruges før kunden gemmes eller opdateres.
        /// </summary>
        private void ValidateCustomer(Customer customer)
        {
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer), "Kunde mangler.");
            }

            if (string.IsNullOrWhiteSpace(customer.Name))
            {
                throw new Exception("Kundens navn skal udfyldes.");
            }

            if (string.IsNullOrWhiteSpace(customer.Phone))
            {
                throw new Exception("Telefonnummer skal udfyldes.");
            }

            if (customer.Phone.Length != 8)
            {
                throw new Exception("Telefonnummer skal være 8 cifre.");
            }

            if (string.IsNullOrWhiteSpace(customer.Email))
            {
                throw new Exception("Email skal udfyldes.");
            }

            if (!customer.Email.Contains("@"))
            {
                throw new Exception("Email skal være gyldig.");
            }
        }
    }
}