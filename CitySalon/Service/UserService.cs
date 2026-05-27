using CitySalon.Models;
using CitySalon.Repositories;

namespace CitySalon.Service
{
    /// <summary>
    /// Service-klasse til User.
    /// Klassen indeholder forretningslogik for brugere
    /// og kalder repository-laget for databasearbejde.
    /// </summary>
    public class UserService
    {
        // Repository bruges til at hente og gemme brugere i databasen.
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Constructor injection af IUserRepository.
        /// ASP.NET Core sender automatisk repository ind.
        /// </summary>
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Logger en bruger ind.
        /// Tjekker først om brugernavn og kodeord er udfyldt.
        /// </summary>
        public User Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new Exception("Brugernavn skal udfyldes.");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new Exception("Kodeord skal udfyldes.");
            }

            return _userRepository.Login(username, password);
        }

        /// <summary>
        /// Henter alle brugere.
        /// </summary>
        public List<User> GetAll()
        {
            return _userRepository.GetAll();
        }

        /// <summary>
        /// Finder én bruger ud fra brugerens id.
        /// Tjekker først om id'et er gyldigt.
        /// </summary>
        public User GetById(int id)
        {
            if (id <= 0)
            {
                throw new Exception("Ugyldigt bruger-id.");
            }

            var user = _userRepository.GetById(id);

            if (user == null)
            {
                throw new Exception("Brugeren blev ikke fundet.");
            }

            return user;
        }

        /// <summary>
        /// Opretter en ny bruger.
        /// Først valideres brugeren,
        /// derefter gemmes brugeren i databasen.
        /// </summary>
        public void Add(User user)
        {
            ValidateUser(user);

            _userRepository.Add(user);
        }

        /// <summary>
        /// Opdaterer en eksisterende bruger.
        /// Først valideres brugeren,
        /// derefter tjekkes det at brugeren findes.
        /// </summary>
        public void Update(User user)
        {
            ValidateUser(user);

            if (user.Id <= 0)
            {
                throw new Exception("Ugyldigt bruger-id.");
            }

            GetById(user.Id);

            _userRepository.Update(user);
        }

        /// <summary>
        /// Sletter en bruger ud fra id.
        /// GetById bruges først for at sikre,
        /// at brugeren findes.
        /// </summary>
        public void Delete(int id)
        {
            GetById(id);

            _userRepository.Delete(id);
        }

        /// <summary>
        /// Tjekker at brugeren har gyldige oplysninger.
        /// Bruges før brugeren gemmes eller opdateres.
        /// </summary>
        private void ValidateUser(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Bruger mangler.");
            }

            if (string.IsNullOrWhiteSpace(user.UserName))
            {
                throw new Exception("Brugernavn skal udfyldes.");
            }

            if (string.IsNullOrWhiteSpace(user.Password))
            {
                throw new Exception("Kodeord skal udfyldes.");
            }

            if (string.IsNullOrWhiteSpace(user.Role))
            {
                throw new Exception("Rolle skal udfyldes.");
            }
        }
    }
}