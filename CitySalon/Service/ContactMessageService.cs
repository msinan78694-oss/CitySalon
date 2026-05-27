using CitySalon.Models;
using CitySalon.Repositories;

namespace CitySalon.Service
{
    /// <summary>
    /// Service-klasse til ContactMessage.
    /// Klassen indeholder forretningslogik for kontaktbeskeder
    /// og kalder repository-laget for databasearbejde.
    /// </summary>
    public class ContactMessageService
    {
        // Repository bruges til at hente og gemme kontaktbeskeder i databasen.
        private readonly IContactMessageRepository _repository;

        /// <summary>
        /// Constructor injection af IContactMessageRepository.
        /// ASP.NET Core sender automatisk repository ind.
        /// </summary>
        public ContactMessageService(IContactMessageRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Henter alle kontaktbeskeder.
        /// </summary>
        public List<ContactMessage> GetAll()
        {
            return _repository.GetAll();
        }

        /// <summary>
        /// Finder én kontaktbesked ud fra beskedens id.
        /// Tjekker først om id'et er gyldigt.
        /// </summary>
        public ContactMessage GetById(int id)
        {
            if (id <= 0)
            {
                throw new Exception("Ugyldigt besked-id.");
            }

            var message = _repository.GetById(id);

            if (message == null)
            {
                throw new Exception("Beskeden blev ikke fundet.");
            }

            return message;
        }

        /// <summary>
        /// Opretter en ny kontaktbesked.
        /// Først valideres beskeden, derefter gemmes den i databasen.
        /// </summary>
        public void Add(ContactMessage message)
        {
            ValidateMessage(message);

            _repository.Add(message);
        }

        /// <summary>
        /// Sletter en kontaktbesked ud fra id.
        /// GetById bruges først for at sikre, at beskeden findes.
        /// </summary>
        public void Delete(int id)
        {
            GetById(id);

            _repository.Delete(id);
        }

        /// <summary>
        /// Markerer en besked som læst.
        /// GetById bruges først for at sikre, at beskeden findes.
        /// </summary>
        public void MarkAsRead(int id)
        {
            GetById(id);

            _repository.MarkAsRead(id);
        }

        /// <summary>
        /// Tjekker at kontaktbeskeden har gyldige oplysninger.
        /// Bruges før beskeden gemmes i databasen.
        /// </summary>
        private void ValidateMessage(ContactMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message), "Besked mangler.");
            }

            if (string.IsNullOrWhiteSpace(message.Name))
            {
                throw new Exception("Navn skal udfyldes.");
            }

            if (string.IsNullOrWhiteSpace(message.Email))
            {
                throw new Exception("Email skal udfyldes.");
            }

            if (!message.Email.Contains("@"))
            {
                throw new Exception("Email skal være gyldig.");
            }

            if (string.IsNullOrWhiteSpace(message.Message))
            {
                throw new Exception("Besked skal udfyldes.");
            }
        }
    }
}