using CitySalon.Models;
using CitySalon.Repositories;

namespace CitySalon.Service
{
    /// <summary>
    /// Service-klasse til Booking.
    /// Klassen indeholder forretningslogik for bookinger,
    /// fx validering, tidskontrol og kald til repository-laget.
    /// </summary>
    public class BookingService
    {
        // Repository bruges til at hente og gemme booking-data i databasen.
        private readonly IBookingRepository _bookingRepository;

        /// <summary>
        /// Constructor injection af IBookingRepository.
        /// ASP.NET Core sender automatisk repository ind.
        /// </summary>
        public BookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        /// <summary>
        /// Henter alle bookinger.
        /// Kalder repository-laget, som henter data fra databasen.
        /// </summary>
        public List<Booking> GetAll()
        {
            return _bookingRepository.GetAll();
        }

        /// <summary>
        /// Henter én booking ud fra bookingens id.
        /// Tjekker først om id'et er gyldigt.
        /// </summary>
        public Booking GetById(int id)
        {
            if (id <= 0)
            {
                throw new Exception("Ugyldigt booking-id.");
            }

            var booking = _bookingRepository.GetById(id);

            if (booking == null)
            {
                throw new Exception("Bookingen blev ikke fundet.");
            }

            return booking;
        }

        /// <summary>
        /// Opretter en ny booking.
        /// Først valideres bookingen, derefter tjekkes om tiden allerede er optaget.
        /// </summary>
        public void Add(Booking booking)
        {
            ValidateBooking(booking);

            if (IsTimeTaken(booking.EmployeeId, booking.StartTime, booking.EndTime))
            {
                throw new Exception("Medarbejderen er allerede booket i dette tidsrum.");
            }

            _bookingRepository.Add(booking);
        }

        /// <summary>
        /// Opdaterer en eksisterende booking.
        /// Først valideres bookingen, og derefter tjekkes at bookingen findes.
        /// </summary>
        public void Update(Booking booking)
        {
            ValidateBooking(booking);

            if (booking.BookingId <= 0)
            {
                throw new Exception("Ugyldigt booking-id.");
            }

            GetById(booking.BookingId);

            _bookingRepository.Update(booking);
        }

        /// <summary>
        /// Sletter en booking ud fra id.
        /// GetById bruges først for at sikre, at bookingen findes.
        /// </summary>
        public void Delete(int id)
        {
            GetById(id);

            _bookingRepository.Delete(id);
        }

        /// <summary>
        /// Tjekker om en medarbejder allerede er booket
        /// i det valgte tidsrum.
        /// </summary>
        public bool IsTimeTaken(int employeeId, DateTime startTime, DateTime endTime)
        {
            if (employeeId <= 0)
            {
                throw new Exception("Ugyldigt medarbejder-id.");
            }

            if (startTime == default || endTime == default)
            {
                throw new Exception("Starttidspunkt og sluttidspunkt skal vælges.");
            }

            if (endTime <= startTime)
            {
                throw new Exception("Sluttidspunkt skal være efter starttidspunkt.");
            }

            return _bookingRepository.IsTimeTaken(employeeId, startTime, endTime);
        }

        /// <summary>
        /// Validerer at en booking har de nødvendige værdier.
        /// Metoden bruges både ved oprettelse og opdatering.
        /// </summary>
        private void ValidateBooking(Booking booking)
        {
            if (booking == null)
            {
                throw new ArgumentNullException(nameof(booking), "Booking mangler.");
            }

            if (booking.CustomerId <= 0)
            {
                throw new Exception("Der skal vælges en kunde.");
            }

            if (booking.TreatmentId <= 0)
            {
                throw new Exception("Der skal vælges en behandling.");
            }

            if (booking.EmployeeId <= 0)
            {
                throw new Exception("Der skal vælges en medarbejder.");
            }

            if (booking.StartTime == default)
            {
                throw new Exception("Starttidspunkt skal vælges.");
            }

            if (booking.EndTime == default)
            {
                throw new Exception("Sluttidspunkt skal vælges.");
            }

            if (booking.EndTime <= booking.StartTime)
            {
                throw new Exception("Sluttidspunkt skal være efter starttidspunkt.");
            }
        }
    }
}