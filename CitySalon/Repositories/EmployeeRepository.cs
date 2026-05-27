using CitySalon.Data;
using CitySalon.Models;

namespace CitySalon.Repositories
{
    /// <summary>
    /// Repository til Employee-klassen.
    /// Klassen bruges til at hente, oprette,
    /// opdatere og slette medarbejdere i databasen.
    /// </summary>
    public class EmployeeRepository : IRepository<Employee>
    {
        // DbContext bruges til at arbejde med databasen via Entity Framework Core.
        private readonly CityDbContext _context;

        /// <summary>
        /// Constructor injection af CityDbContext.
        /// ASP.NET Core sender automatisk database-contexten ind.
        /// </summary>
        public EmployeeRepository(CityDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Henter alle medarbejdere fra databasen.
        /// </summary>
        public List<Employee> GetAll()
        {
            return _context.Employees.ToList();
        }

        /// <summary>
        /// Finder én medarbejder ud fra medarbejderens id.
        /// Find returnerer null hvis medarbejderen ikke findes.
        /// </summary>
        public Employee GetById(int id)
        {
            return _context.Employees.Find(id);
        }

        /// <summary>
        /// Opretter en ny medarbejder i databasen.
        /// SaveChanges gemmer ændringen permanent.
        /// </summary>
        public void Add(Employee employee)
        {
            _context.Employees.Add(employee);
            _context.SaveChanges();
        }

        /// <summary>
        /// Opdaterer en eksisterende medarbejder i databasen.
        /// Først findes medarbejderen, derefter opdateres felterne.
        /// </summary>
        public void Update(Employee employee)
        {
            var existingEmployee = _context.Employees.Find(employee.Id);

            if (existingEmployee != null)
            {
                existingEmployee.Name = employee.Name;
                existingEmployee.UserName = employee.UserName;
                existingEmployee.Password = employee.Password;
                existingEmployee.Role = employee.Role;

                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Sletter en medarbejder fra databasen ud fra medarbejderens id.
        /// </summary>
        public void Delete(int id)
        {
            var employee = _context.Employees.Find(id);

            if (employee != null)
            {
                _context.Employees.Remove(employee);
                _context.SaveChanges();
            }
        }
    }
}