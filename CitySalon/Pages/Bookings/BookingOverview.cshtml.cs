using CitySalon.Helpers;
using CitySalon.Models;
using CitySalon.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CitySalon.Pages.Bookings
{
    /// <summary>
    /// PageModel til bookingoversigten.
    /// Viser bookinger, kalender, søgning og ledige tider for admin og medarbejdere.
    /// </summary>
    public class BookingOverviewModel : PageModel
    {
        private readonly BookingService _bookingService;
        private readonly EmployeeService _employeeService;

        /// <summary>
        /// Opretter BookingOverviewModel med adgang til BookingService og EmployeeService.
        /// </summary>
        /// <param name="bookingService">Service til håndtering af bookinger.</param>
        /// <param name="employeeService">Service til håndtering af medarbejdere.</param>
        public BookingOverviewModel(
            BookingService bookingService,
            EmployeeService employeeService)
        {
            _bookingService = bookingService;
            _employeeService = employeeService;
        }

        /// <summary>
        /// Liste med bookinger for den valgte dato.
        /// </summary>
        public List<Booking> Bookings { get; set; } = new();

        /// <summary>
        /// Liste med søgeresultater fra bookinglisten.
        /// </summary>
        public List<Booking> SearchResults { get; set; } = new();

        /// <summary>
        /// Liste med medarbejdere til dropdown-menuen.
        /// </summary>
        public List<CitySalon.Models.Employee> Employees { get; set; } = new();

        /// <summary>
        /// Liste med ledige tider for valgt dato og medarbejder.
        /// </summary>
        public List<string> AvailableTimes { get; set; } = new();

        /// <summary>
        /// Valgt dato hentet fra URL/query string.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public DateTime SelectedDate { get; set; } = DateTime.Today;

        /// <summary>
        /// Valgt medarbejder-id hentet fra URL/query string.
        /// Værdien 0 betyder, at ingen medarbejder er valgt.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public int SelectedEmployeeId { get; set; } = 0;

        /// <summary>
        /// Søgetekst hentet fra URL/query string.
        /// Bruges til at søge efter bookinger ud fra kundeoplysninger.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        /// <summary>
        /// Antal dage i den valgte måned.
        /// Bruges til kalenderen på siden.
        /// </summary>
        public int DaysInMonth { get; set; }

        /// <summary>
        /// Antal tomme felter før månedens første dag i kalenderen.
        /// Bruges så kalenderen starter korrekt med mandag som første dag.
        /// </summary>
        public int FirstDayOffset { get; set; }

        /// <summary>
        /// Alle mulige tider i bookingsystemet.
        /// Bruges som udgangspunkt til at beregne ledige tider.
        /// </summary>
        private List<string> AllTimes { get; set; } = new()
        {
            "09:00", "09:30",
            "10:00", "10:30",
            "11:00", "11:30",
            "12:00", "12:30",
            "13:00", "13:30",
            "14:00", "14:30",
            "15:00", "15:30",
            "16:00", "16:30",
            "17:00", "17:30"
        };

        /// <summary>
        /// Henter og viser bookingoversigten.
        /// Indlæser bookinger, medarbejdere, kalenderinformation,
        /// søgeresultater og ledige tider.
        /// </summary>
        /// <returns>
        /// Redirecter til login hvis brugeren ikke er admin eller medarbejder.
        /// Ellers vises bookingoversigten.
        /// </returns>
        public IActionResult OnGet()
        {
            // Kun admin eller medarbejder må se bookingsiden.
            if (!SessionHelper.IsAdminOrEmployee(HttpContext))
            {
                return RedirectToPage("/Login/LogInPage");
            }

            // Henter alle bookinger fra databasen.
            var allBookings = _bookingService.GetAll();

            // Henter alle medarbejdere.
            Employees = _employeeService.GetAll();

            // Beregner kalenderinformation.
            LoadCalendarInfo();

            // Henter bookinger for valgt dato.
            LoadBookingsForSelectedDate(allBookings);

            // Søger efter bookinger.
            LoadSearchResults(allBookings);

            // Finder ledige tider.
            AvailableTimes = GetAvailableTimes(allBookings);

            return Page();
        }

        /// <summary>
        /// Beregner kalenderoplysninger for den valgte måned.
        /// Finder antal dage i måneden og hvor mange tomme felter kalenderen skal starte med.
        /// </summary>
        private void LoadCalendarInfo()
        {
            // Finder antal dage i måneden.
            DaysInMonth = DateTime.DaysInMonth(
                SelectedDate.Year,
                SelectedDate.Month
            );

            // Finder månedens første dag.
            var firstDay = new DateTime(
                SelectedDate.Year,
                SelectedDate.Month,
                1
            );

            // Beregner offset så mandag bliver første dag i kalenderen.
            FirstDayOffset = ((int)firstDay.DayOfWeek + 6) % 7;
        }

        /// <summary>
        /// Henter bookinger for den valgte dato.
        /// Hvis en medarbejder er valgt, filtreres bookingerne også efter medarbejder.
        /// </summary>
        /// <param name="allBookings">Liste med alle bookinger.</param>
        private void LoadBookingsForSelectedDate(List<Booking> allBookings)
        {
            Bookings = allBookings

                // Kun bookinger på valgt dato.
                .Where(b => b.StartTime.Date == SelectedDate.Date)

                // Hvis medarbejder er valgt, filtreres der også efter medarbejder.
                .Where(b => SelectedEmployeeId == 0 ||
                            b.EmployeeId == SelectedEmployeeId)

                // Sorterer efter tidspunkt.
                .OrderBy(b => b.StartTime)

                .ToList();
        }

        /// <summary>
        /// Søger efter bookinger ud fra kundens navn, telefonnummer eller email.
        /// </summary>
        /// <param name="allBookings">Liste med alle bookinger.</param>
        private void LoadSearchResults(List<Booking> allBookings)
        {
            // Stopper hvis søgefeltet er tomt.
            if (string.IsNullOrWhiteSpace(SearchTerm))
            {
                return;
            }

            // Gør søgningen case-insensitive.
            string search = SearchTerm.Trim().ToLower();

            SearchResults = allBookings
                .Where(b =>

                    // Tjekker at Customer ikke er null.
                    b.Customer != null &&

                    (
                        b.Customer.Name.ToLower().Contains(search) ||
                        b.Customer.Phone.ToLower().Contains(search) ||
                        b.Customer.Email.ToLower().Contains(search)
                    ))

                // Sorterer resultaterne efter tidspunkt.
                .OrderBy(b => b.StartTime)

                .ToList();
        }

        /// <summary>
        /// Finder ledige tider for den valgte dato og medarbejder.
        /// </summary>
        /// <param name="allBookings">Liste med alle bookinger.</param>
        /// <returns>
        /// En liste med ledige tider som tekst.
        /// Returnerer en tom liste hvis ingen medarbejder er valgt.
        /// </returns>
        private List<string> GetAvailableTimes(List<Booking> allBookings)
        {
            // Hvis ingen medarbejder er valgt,
            // returneres tom liste.
            if (SelectedEmployeeId == 0)
            {
                return new List<string>();
            }

            // Finder alle bookinger for valgt medarbejder og dato.
            var bookingsForEmployeeAndDate = allBookings
                .Where(b => b.EmployeeId == SelectedEmployeeId &&
                            b.StartTime.Date == SelectedDate.Date)
                .ToList();

            // Returnerer kun tider som er ledige.
            return AllTimes
                .Where(time =>
                    IsTimeAvailable(
                        time,
                        bookingsForEmployeeAndDate))
                .ToList();
        }

        /// <summary>
        /// Tjekker om et bestemt tidspunkt er ledigt.
        /// Et tidspunkt er ikke ledigt, hvis det overlapper med en eksisterende booking.
        /// </summary>
        /// <param name="time">Tidspunktet som tekst, f.eks. "09:30".</param>
        /// <param name="bookingsForEmployeeAndDate">
        /// Liste med bookinger for den valgte medarbejder og dato.
        /// </param>
        /// <returns>
        /// True hvis tidspunktet er ledigt.
        /// False hvis tidspunktet overlapper med en eksisterende booking.
        /// </returns>
        private bool IsTimeAvailable(
            string time,
            List<Booking> bookingsForEmployeeAndDate)
        {
            // Konverterer fx "09:30" til TimeSpan.
            TimeSpan slotTime = TimeSpan.Parse(time);

            // Finder starttidspunkt.
            DateTime slotStart =
                SelectedDate.Date.Add(slotTime);

            // Alle tider varer 30 minutter.
            DateTime slotEnd =
                slotStart.AddMinutes(30);

            // Tjekker om tidspunktet overlapper
            // med eksisterende bookinger.
            bool overlapsExistingBooking =
                bookingsForEmployeeAndDate.Any(booking =>

                    slotStart < booking.EndTime &&
                    slotEnd > booking.StartTime

                );

            // Returnerer true hvis der IKKE er overlap.
            return !overlapsExistingBooking;
        }
    }
}