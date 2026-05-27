using CitySalon.Data;
using CitySalon.Models;

namespace CitySalon.Repositories
{
    /// <summary>
    /// Repository til User-klassen.
    /// Klassen bruges til at håndtere login
    /// samt hente, oprette, opdatere og slette brugere i databasen.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        // DbContext bruges til at arbejde med databasen via Entity Framework Core.
        private readonly CityDbContext _context;

        /// <summary>
        /// Constructor injection af CityDbContext.
        /// ASP.NET Core sender automatisk database-contexten ind.
        /// </summary>
        public UserRepository(CityDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Finder en bruger med brugernavn og kodeord.
        /// FirstOrDefault returnerer null hvis brugeren ikke findes.
        /// </summary>
        public User Login(string username, string password)
        {
            return _context.Users
                .FirstOrDefault(u => u.UserName == username && u.Password == password);
        }

        /// <summary>
        /// Henter alle brugere fra databasen.
        /// </summary>
        public List<User> GetAll()
        {
            return _context.Users.ToList();
        }

        /// <summary>
        /// Finder én bruger ud fra brugerens id.
        /// Find returnerer null hvis brugeren ikke findes.
        /// </summary>
        public User GetById(int id)
        {
            return _context.Users.Find(id);
        }

        /// <summary>
        /// Opretter en ny bruger i databasen.
        /// SaveChanges gemmer ændringen permanent.
        /// </summary>
        public void Add(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        /// <summary>
        /// Opdaterer en eksisterende bruger i databasen.
        /// Først findes brugeren, derefter opdateres felterne.
        /// </summary>
        public void Update(User user)
        {
            var existingUser = _context.Users.Find(user.Id);

            if (existingUser != null)
            {
                existingUser.UserName = user.UserName;
                existingUser.Password = user.Password;
                existingUser.Role = user.Role;

                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Sletter en bruger fra databasen ud fra brugerens id.
        /// </summary>
        public void Delete(int id)
        {
            var user = _context.Users.Find(id);

            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }
    }
}