using CitySalon.Models;
using CitySalon.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CitySalon.Pages.Bookings
{
    /// <summary>
    /// PageModel til redigering af bookinger.
    /// Bruges til at ændre dato, tidspunkt, behandling,
    /// medarbejder og kundens navn på en eksisterende booking.
    /// </summary>
    public class EditBookingModel : PageModel
    {
        // Service til håndtering af booking-data.
        private readonly BookingService _bookingService;

        // Service til håndtering af kunder.
        private readonly CustomerService _customerService;

        // Service til håndtering af behandlinger.
        private readonly TreatmentService _treatmentService;

        // Service til håndtering af medarbejdere.
        private readonly EmployeeService _employeeService;

        /// <summary>
        /// Binder formularfelterne til Booking-objektet ved POST.
        /// Når formularen sendes, udfyldes Booking automatisk.
        /// </summary>
        [BindProperty]
        public Booking Booking { get; set; } = new Booking();

        /// <summary>
        /// Den valgte dato fra formularen.
        /// </summary>
        [BindProperty]
        public DateTime SelectedDate { get; set; }

        /// <summary>
        /// Det valgte tidspunkt fra dropdown-listen.
        /// </summary>
        [BindProperty]
        public string? SelectedTime { get; set; }

        /// <summary>
        /// Kundens navn redigeres separat fra Booking-objektet.
        /// </summary>
        [BindProperty]
        public string CustomerName { get; set; } = string.Empty;

        /// <summary>
        /// Dropdown-data til behandlinger.
        /// </summary>
        public List<SelectListItem> TreatmentOptions { get; set; } = new();

        /// <summary>
        /// Dropdown-data til medarbejdere.
        /// </summary>
        public List<SelectListItem> EmployeeOptions { get; set; } = new();

        /// <summary>
        /// Liste med ledige tider.
        /// </summary>
        public List<string> TimeSlots { get; set; } = new();

        /// <summary>
        /// Constructor injection af services.
        /// ASP.NET Core sender automatisk services ind.
        /// </summary>
        public EditBookingModel(
            BookingService bookingService,
            CustomerService customerService,
            TreatmentService treatmentService,
            EmployeeService employeeService)
        {
            _bookingService = bookingService;
            _customerService = customerService;
            _treatmentService = treatmentService;
            _employeeService = employeeService;
        }

        /// <summary>
        /// Henter bookingen og viser den i formularen.
        /// Kører når siden åbnes første gang.
        /// </summary>
        public IActionResult OnGet(int id)
        {
            try
            {
                // Henter booking ud fra id.
                Booking = _bookingService.GetById(id);

                // Hvis booking ikke findes.
                if (Booking == null)
                {
                    return RedirectToPage("/Bookings/BookingOverview");
                }

                // Henter kundens navn.
                CustomerName = Booking.Customer?.Name ?? "";

                // Gemmer dato og tidspunkt separat.
                SelectedDate = Booking.StartTime.Date;
                SelectedTime = Booking.StartTime.ToString("HH:mm");

                // Indlæser dropdowns og tider.
                LoadFormData();

                return Page();
            }
            catch (Exception)
            {
                return RedirectToPage("/Bookings/BookingOverview");
            }
        }

        /// <summary>
        /// Gemmer ændringerne fra formularen.
        /// Kører når brugeren sender formularen.
        /// </summary>
        public IActionResult OnPost()
        {
            // Genindlæser dropdowns og tider.
            LoadFormData();

            // Fjerner validation-fejl fra navigation properties.
            RemoveModelStateErrors();

            // Beregner bookingens tider.
            if (!SetBookingTime())
            {
                return ReloadPage();
            }

            // Stopper hvis validation fejler.
            if (!ModelState.IsValid)
            {
                return ReloadPage();
            }

            // Tjekker om tiden overlapper en anden booking.
            if (IsTimeTakenByAnotherBooking())
            {
                ModelState.AddModelError(
                    "",
                    "Denne tid er allerede booket.");

                return ReloadPage();
            }

            try
            {
                // Opdaterer kundens navn.
                UpdateCustomerName();

                // Opdaterer selve bookingen.
                _bookingService.Update(Booking);

                return RedirectToPage("/Bookings/BookingOverview");
            }
            catch (Exception ex)
            {
                // Viser fejlbesked fra service-laget.
                ModelState.AddModelError("", ex.Message);

                return ReloadPage();
            }
        }

        /// <summary>
        /// Genindlæser siden hvis der opstår fejl.
        /// Sørger for at dropdowns og kundedata stadig vises.
        /// </summary>
        private IActionResult ReloadPage()
        {
            LoadFormData();

            // Henter kundens navn igen hvis nødvendigt.
            if (string.IsNullOrWhiteSpace(CustomerName) &&
                Booking.CustomerId > 0)
            {
                var customer =
                    _customerService.GetById(
                        Booking.CustomerId);

                CustomerName = customer.Name;
            }

            return Page();
        }

        /// <summary>
        /// Fjerner validation-fejl fra navigation properties.
        /// Disse udfyldes ikke direkte i formularen.
        /// </summary>
        private void RemoveModelStateErrors()
        {
            ModelState.Remove("Booking.Customer");
            ModelState.Remove("Booking.Treatment");
            ModelState.Remove("Booking.Employee");
            ModelState.Remove("Booking.StartTime");
            ModelState.Remove("Booking.EndTime");
        }

        /// <summary>
        /// Beregner bookingens starttid og sluttid.
        /// Tjekker også om data er gyldige.
        /// </summary>
        private bool SetBookingTime()
        {
            // Tjekker kundens navn.
            if (string.IsNullOrWhiteSpace(CustomerName))
            {
                ModelState.AddModelError(
                    "",
                    "Kundens navn skal udfyldes.");

                return false;
            }

            // Tjekker tidspunkt.
            if (string.IsNullOrWhiteSpace(SelectedTime))
            {
                ModelState.AddModelError(
                    "",
                    "Du skal vælge et tidspunkt.");

                return false;
            }

            // Konverterer tidspunkt til TimeSpan.
            if (!TimeSpan.TryParse(
                SelectedTime,
                out TimeSpan time))
            {
                ModelState.AddModelError(
                    "",
                    "Tidspunktet er ikke gyldigt.");

                return false;
            }

            // Henter behandlingen.
            var treatment =
                _treatmentService.GetById(
                    Booking.TreatmentId);

            if (treatment == null)
            {
                ModelState.AddModelError(
                    "",
                    "Behandlingen blev ikke fundet.");

                return false;
            }

            // Beregner starttidspunkt.
            Booking.StartTime =
                SelectedDate.Date.Add(time);

            // Beregner sluttidspunkt ud fra behandlingens varighed.
            Booking.EndTime =
                Booking.StartTime.AddMinutes(
                    treatment.DurationInMinutes);

            // Salonens lukketid.
            DateTime closingTime =
                SelectedDate.Date.Add(
                    new TimeSpan(18, 0, 0));

            // Tjekker om bookingen går over lukketid.
            if (Booking.EndTime > closingTime)
            {
                ModelState.AddModelError(
                    "",
                    "Der er ikke nok tid til den valgte behandling før lukketid.");

                return false;
            }

            return true;
        }

        /// <summary>
        /// Opdaterer kundens navn i databasen.
        /// </summary>
        private void UpdateCustomerName()
        {
            var customer =
                _customerService.GetById(
                    Booking.CustomerId);

            customer.Name = CustomerName;

            _customerService.Update(customer);
        }

        /// <summary>
        /// Tjekker om tidspunktet overlapper en anden booking.
        /// </summary>
        private bool IsTimeTakenByAnotherBooking()
        {
            return _bookingService.GetAll()
                .Any(booking =>

                    // Ignorerer den booking der redigeres.
                    booking.BookingId != Booking.BookingId &&

                    // Samme medarbejder.
                    booking.EmployeeId == Booking.EmployeeId &&

                    // Overlap-logik.
                    Booking.StartTime < booking.EndTime &&
                    Booking.EndTime > booking.StartTime
                );
        }

        /// <summary>
        /// Henter dropdown-data og ledige tider.
        /// </summary>
        private void LoadFormData()
        {
            // Opretter dropdown med behandlinger.
            TreatmentOptions =
                _treatmentService.GetAll()
                .Select(t => new SelectListItem
                {
                    Value = t.TreatmentId.ToString(),
                    Text = t.Name
                })
                .ToList();

            // Opretter dropdown med medarbejdere.
            EmployeeOptions =
                _employeeService.GetAll()
                .Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.Name
                })
                .ToList();

            // Henter ledige tider.
            LoadTimeSlots();
        }

        /// <summary>
        /// Finder ledige tider for den valgte medarbejder og dato.
        /// </summary>
        private void LoadTimeSlots()
        {
            TimeSlots = new List<string>();

            // Salonens åbningstid.
            TimeSpan start = new TimeSpan(9, 0, 0);

            // Salonens lukketid.
            TimeSpan end = new TimeSpan(18, 0, 0);

            // Varighed på valgt behandling.
            int duration = GetSelectedTreatmentDuration();

            // Henter bookinger for medarbejderen på valgt dato.
            var bookingsForEmployeeAndDate =
                _bookingService.GetAll()
                .Where(b =>

                    // Ignorerer nuværende booking.
                    b.BookingId != Booking.BookingId &&

                    b.EmployeeId == Booking.EmployeeId &&
                    b.StartTime.Date == SelectedDate.Date
                )
                .ToList();

            // Genererer alle tider.
            while (start < end)
            {
                DateTime slotStart =
                    SelectedDate.Date.Add(start);

                DateTime slotEnd =
                    slotStart.AddMinutes(duration);

                // Tjekker om tiden går over lukketid.
                bool isAfterClosingTime =
                    slotEnd.TimeOfDay > end;

                // Tjekker overlap med andre bookinger.
                bool overlaps =
                    bookingsForEmployeeAndDate.Any(booking =>

                        slotStart < booking.EndTime &&
                        slotEnd > booking.StartTime
                    );

                // Tilføjer kun ledige tider.
                if (!isAfterClosingTime && !overlaps)
                {
                    TimeSlots.Add(
                        start.ToString(@"hh\:mm"));
                }

                // Går 30 minutter frem.
                start = start.Add(
                    TimeSpan.FromMinutes(30));
            }

            // Sørger for at valgt tid stadig vises.
            AddSelectedTimeIfMissing();
        }

        /// <summary>
        /// Finder varigheden på den valgte behandling.
        /// </summary>
        private int GetSelectedTreatmentDuration()
        {
            // Standardværdi hvis ingen behandling er valgt.
            if (Booking.TreatmentId <= 0)
            {
                return 30;
            }

            var treatment =
                _treatmentService.GetById(
                    Booking.TreatmentId);

            return treatment.DurationInMinutes;
        }

        /// <summary>
        /// Sørger for at den nuværende bookingtid stadig findes i dropdown-listen.
        /// </summary>
        private void AddSelectedTimeIfMissing()
        {
            if (!string.IsNullOrWhiteSpace(SelectedTime) &&
                !TimeSlots.Contains(SelectedTime))
            {
                TimeSlots.Add(SelectedTime);

                // Sorterer tiderne korrekt.
                TimeSlots = TimeSlots
                    .OrderBy(t => TimeSpan.Parse(t))
                    .ToList();
            }
        }
    }
}