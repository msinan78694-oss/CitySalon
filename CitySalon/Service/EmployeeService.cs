using CitySalon.Models;
using CitySalon.Repositories;

namespace CitySalon.Service
{
    /// <summary>
    /// Service-klasse til Employee.
    /// Klassen indeholder forretningslogik for medarbejdere
    /// og kalder repository-laget for databasearbejde.
    /// </summary>
    public class EmployeeService
    {
        // Repository bruges til at hente og gemme medarbejdere i databasen.
        private readonly IRepository<Employee> _employeeRepository;

        /// <summary>
        /// Constructor injection af IRepository<Employee>.
        /// ASP.NET Core sender automatisk repository ind.
        /// </summary>
        public EmployeeService(IRepository<Employee> employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        /// <summary>
        /// Henter alle medarbejdere.
        /// </summary>
        public List<Employee> GetAll()
        {
            return _employeeRepository.GetAll();
        }

        /// <summary>
        /// Finder én medarbejder ud fra medarbejderens id.
        /// Tjekker først om id'et er gyldigt.
        /// </summary>
        public Employee GetById(int id)
        {
            if (id <= 0)
            {
                throw new Exception("Ugyldigt medarbejder-id.");
            }

            var employee = _employeeRepository.GetById(id);

            if (employee == null)
            {
                throw new Exception("Medarbejderen blev ikke fundet.");
            }

            return employee;
        }

        /// <summary>
        /// Opretter en ny medarbejder.
        /// Først valideres medarbejderen,
        /// derefter gemmes medarbejderen i databasen.
        /// </summary>
        public void Add(Employee employee)
        {
            ValidateEmployee(employee);

            _employeeRepository.Add(employee);
        }

        /// <summary>
        /// Opdaterer en eksisterende medarbejder.
        /// Først valideres medarbejderen,
        /// derefter tjekkes det at medarbejderen findes.
        /// </summary>
        public void Update(Employee employee)
        {
            ValidateEmployee(employee);

            if (employee.Id <= 0)
            {
                throw new Exception("Ugyldigt medarbejder-id.");
            }

            GetById(employee.Id);

            _employeeRepository.Update(employee);
        }

        /// <summary>
        /// Sletter en medarbejder ud fra id.
        /// GetById bruges først for at sikre,
        /// at medarbejderen findes.
        /// </summary>
        public void Delete(int id)
        {
            GetById(id);

            _employeeRepository.Delete(id);
        }

        /// <summary>
        /// Tjekker at medarbejderen har gyldige oplysninger.
        /// Bruges før medarbejderen gemmes eller opdateres.
        /// </summary>
        private void ValidateEmployee(Employee employee)
        {
            if (employee == null)
            {
                throw new ArgumentNullException(nameof(employee), "Medarbejder mangler.");
            }

            if (string.IsNullOrWhiteSpace(employee.Name))
            {
                throw new Exception("Navn skal udfyldes.");
            }

            if (string.IsNullOrWhiteSpace(employee.UserName))
            {
                throw new Exception("Brugernavn skal udfyldes.");
            }

            if (string.IsNullOrWhiteSpace(employee.Password))
            {
                throw new Exception("Kodeord skal udfyldes.");
            }

            if (string.IsNullOrWhiteSpace(employee.Role))
            {
                throw new Exception("Rolle skal udfyldes.");
            }
        }
    }
}