using CitySalon.Data;
using CitySalon.Models;

namespace CitySalon.Repositories
{
    /// <summary>
    /// Repository til ContactMessage-klassen.
    /// Klassen bruges til at hente, oprette, slette
    /// og opdatere kontaktbeskeder i databasen.
    /// </summary>
    public class ContactMessageRepository : IContactMessageRepository
    {
        // DbContext bruges til at arbejde med databasen via Entity Framework Core.
        private readonly CityDbContext _context;

        /// <summary>
        /// Constructor injection af CityDbContext.
        /// ASP.NET Core sender automatisk database-contexten ind.
        /// </summary>
        public ContactMessageRepository(CityDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Henter alle kontaktbeskeder fra databasen.
        /// Beskederne sorteres så de nyeste vises først.
        /// </summary>
        public List<ContactMessage> GetAll()
        {
            return _context.ContactMessages
                .OrderByDescending(m => m.SentAt)
                .ToList();
        }

        /// <summary>
        /// Finder én besked ud fra beskedens id.
        /// FirstOrDefault returnerer null hvis beskeden ikke findes.
        /// </summary>
        public ContactMessage? GetById(int id)
        {
            return _context.ContactMessages
                .FirstOrDefault(m => m.ContactMessageId == id);
        }

        /// <summary>
        /// Opretter en ny kontaktbesked i databasen.
        /// SaveChanges gemmer ændringen permanent.
        /// </summary>
        public void Add(ContactMessage message)
        {
            _context.ContactMessages.Add(message);
            _context.SaveChanges();
        }

        /// <summary>
        /// Sletter en kontaktbesked fra databasen.
        /// </summary>
        public void Delete(int id)
        {
            var message = GetById(id);

            if (message != null)
            {
                _context.ContactMessages.Remove(message);
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Marker en besked som læst.
        /// IsRead sættes til true og gemmes i databasen.
        /// </summary>
        public void MarkAsRead(int id)
        {
            var message = GetById(id);

            if (message != null)
            {
                message.IsRead = true;
                _context.SaveChanges();
            }
        }
    }
}