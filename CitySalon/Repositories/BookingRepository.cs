using CitySalon.Data;
using CitySalon.Models;
using Microsoft.EntityFrameworkCore;

namespace CitySalon.Repositories
{
    /// <summary>
    /// Repository til Booking-klassen.
    /// Klassen står for at hente, oprette, opdatere og slette bookinger i databasen.
    /// </summary>
    public class BookingRepository : IBookingRepository
    {
        // DbContext bruges til at arbejde med databasen via Entity Framework Core.
        private readonly CityDbContext _context;

        /// <summary>
        /// Constructor injection af CityDbContext.
        /// ASP.NET Core sender automatisk database-contexten ind.
        /// </summary>
        public BookingRepository(CityDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Henter alle bookinger fra databasen.
        /// Include bruges til også at hente Customer, Treatment og Employee med.
        /// </summary>
        public List<Booking> GetAll()
        {
            return _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Treatment)
                .Include(b => b.Employee)
                .ToList();
        }

        /// <summary>
        /// Henter én booking ud fra bookingens id.
        /// Include bruges til også at hente de tilknyttede objekter med.
        /// </summary>
        public Booking GetById(int id)
        {
            return _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Treatment)
                .Include(b => b.Employee)
                .FirstOrDefault(b => b.BookingId == id);
        }

        /// <summary>
        /// Tilføjer en ny booking til databasen.
        /// SaveChanges gemmer ændringen permanent.
        /// </summary>
        public void Add(Booking booking)
        {
            _context.Bookings.Add(booking);
            _context.SaveChanges();
        }

        /// <summary>
        /// Opdaterer en eksisterende booking i databasen.
        /// Først findes bookingen, derefter ændres dens felter.
        /// </summary>
        public void Update(Booking booking)
        {
            var existingBooking = _context.Bookings.Find(booking.BookingId);

            if (existingBooking != null)
            {
                existingBooking.CustomerId = booking.CustomerId;
                existingBooking.TreatmentId = booking.TreatmentId;
                existingBooking.EmployeeId = booking.EmployeeId;
                existingBooking.StartTime = booking.StartTime;
                existingBooking.EndTime = booking.EndTime;
                existingBooking.Note = booking.Note;
                existingBooking.Status = booking.Status;

                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Sletter en booking fra databasen ud fra id.
        /// </summary>
        public void Delete(int id)
        {
            var booking = _context.Bookings.Find(id);

            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Tjekker om en medarbejder allerede har en booking
        /// der overlapper det ønskede tidsrum.
        /// </summary>
        public bool IsTimeTaken(int employeeId, DateTime startTime, DateTime endTime)
        {
            return _context.Bookings.Any(b =>
                b.EmployeeId == employeeId &&
                b.StartTime < endTime &&
                b.EndTime > startTime
            );
        }
    }
}