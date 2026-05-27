using CitySalon.Models;
using CitySalon.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CitySalon.Pages.Bookings
{
    /// <summary>
    /// PageModel til oprettelse af bookinger.
    /// Bruges af kunder, medarbejdere og admins til at booke tider.
    /// </summary>
    public class CreateBookingModel : PageModel
    {
        // Service til håndtering af bookinger.
        private readonly BookingService _bookingService;

        // Service til håndtering af kunder.
        private readonly CustomerService _customerService;

        // Service til håndtering af behandlinger.
        private readonly TreatmentService _treatmentService;

        // Service til håndtering af medarbejdere.
        private readonly EmployeeService _employeeService;

        /// <summary>
        /// Binder formularens felter til Booking-objektet.
        /// Når formularen sendes, udfyldes Booking automatisk.
        /// </summary>
        [BindProperty]
        public Booking Booking { get; set; } = new Booking();

        /// <summary>
        /// Telefonnummer brugt til kundesøgning.
        /// </summary>
        [BindProperty]
        public string? CustomerPhone { get; set; }

        /// <summary>
        /// Liste med kunder fundet via telefonsøgning.
        /// </summary>
        public List<Customer> FoundCustomers { get; set; } = new();

        /// <summary>
        /// Id på den kunde der vælges fra søgeresultaterne.
        /// </summary>
        [BindProperty]
        public int SelectedCustomerId { get; set; }

        /// <summary>
        /// Valgt dato fra query string eller formular.
        /// SupportsGet=true gør at værdien kan læses fra URL'en.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public DateTime SelectedDate { get; set; } = DateTime.Today;

        /// <summary>
        /// Valgt tidspunkt fra dropdown-listen.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string? SelectedTime { get; set; }

        /// <summary>
        /// Valgt medarbejder fra query string.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public int EmployeeId { get; set; }

        /// <summary>
        /// Navn på den loggede ind kunde.
        /// </summary>
        public string? LoggedInCustomerName { get; set; }

        /// <summary>
        /// Email på den loggede ind kunde.
        /// </summary>
        public string? LoggedInCustomerEmail { get; set; }

        /// <summary>
        /// True hvis en kunde er logget ind.
        /// </summary>
        public bool IsCustomerLoggedIn { get; set; }

        /// <summary>
        /// Dropdown-liste med behandlinger.
        /// </summary>
        public List<SelectListItem> TreatmentOptions { get; set; } = new();

        /// <summary>
        /// Dropdown-liste med medarbejdere.
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
        public CreateBookingModel(
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
        /// Kører når bookingsiden åbnes første gang.
        /// Indlæser data og tjekker adgang.
        /// </summary>
        public IActionResult OnGet()
        {
            // Henter brugerens rolle fra session.
            var role = HttpContext.Session.GetString("Role");

            // Kun admin, medarbejder og kunde må bruge bookingsiden.
            if (!IsAllowedRole(role))
            {
                return RedirectToPage("/Login/LogInPage");
            }

            // Hvis medarbejder er valgt via URL,
            // sættes den på booking-objektet.
            if (EmployeeId != 0)
            {
                Booking.EmployeeId = EmployeeId;
            }

            // Indlæser dropdown-data og tider.
            LoadFormData();

            // Indlæser info om logget ind kunde.
            LoadLoggedInCustomer();

            return Page();
        }

        /// <summary>
        /// Søger efter kunder ud fra telefonnummer.
        /// Bruges af admin og medarbejdere.
        /// </summary>
        public IActionResult OnPostFindCustomer()
        {
            var role = HttpContext.Session.GetString("Role");

            // Kun admin og medarbejder må søge kunder.
            if (role != typeof(CitySalon.Models.Admin).Name &&
                role != typeof(CitySalon.Models.Employee).Name)
            {
                return RedirectToPage("/Login/LogInPage");
            }

            // Fjerner gamle validation-fejl.
            ModelState.Clear();

            // Gemmer valgt medarbejder.
            EmployeeId = Booking.EmployeeId;

            // Genindlæser dropdowns og kundeinfo.
            LoadFormData();
            LoadLoggedInCustomer();

            // Tjekker om telefonnummer er udfyldt.
            if (string.IsNullOrWhiteSpace(CustomerPhone))
            {
                ModelState.AddModelError(
                    "",
                    "Du skal indtaste et telefonnummer.");

                return Page();
            }

            // Søger efter kunder med matchende telefonnummer.
            FoundCustomers = _customerService.GetAll()
                .Where(c =>
                    c.Phone.Contains(CustomerPhone.Trim()))
                .ToList();

            // Hvis ingen kunder blev fundet.
            if (!FoundCustomers.Any())
            {
                ModelState.AddModelError(
                    "",
                    "Der blev ikke fundet en kunde med dette telefonnummer.");
            }

            return Page();
        }

        /// <summary>
        /// Opretter selve bookingen.
        /// Kører når formularen sendes.
        /// </summary>
        public IActionResult OnPost()
        {
            var role = HttpContext.Session.GetString("Role");

            // Tjekker adgang.
            if (!IsAllowedRole(role))
            {
                return RedirectToPage("/Login/LogInPage");
            }

            // Gemmer valgt medarbejder.
            EmployeeId = Booking.EmployeeId;

            // Genindlæser dropdowns og tider.
            LoadFormData();

            // Henter logget ind kunde.
            LoadLoggedInCustomer();

            // Genindlæser søgeresultater.
            LoadFoundCustomers();

            // Fjerner validation-fejl fra navigation properties.
            RemoveModelStateErrors();

            // Sætter korrekt kunde på bookingen.
            if (!SetCustomerForBooking(role))
            {
                return Page();
            }

            // Beregner start- og sluttidspunkt.
            if (!SetBookingTime())
            {
                return Page();
            }

            // Stopper hvis validation fejler.
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Nye bookinger starter som Pending.
            Booking.Status = "Pending";

            try
            {
                // Gemmer bookingen i databasen.
                _bookingService.Add(Booking);

                // Kunde sendes til bekræftelsessiden.
                if (role == typeof(Customer).Name)
                {
                    return RedirectToPage(
                        "/Bookings/BookingConfirmation",
                        new { id = Booking.BookingId });
                }

                // Admin/medarbejder sendes tilbage til oversigten.
                return RedirectToPage(
                    "/Bookings/BookingOverview",
                    new
                    {
                        SelectedDate,
                        SelectedEmployeeId = Booking.EmployeeId
                    });
            }
            catch (Exception ex)
            {
                // Viser fejlbesked fra service-laget.
                ModelState.AddModelError("", ex.Message);

                return Page();
            }
        }

        /// <summary>
        /// Tjekker om rollen har adgang til bookingsiden.
        /// </summary>
        private bool IsAllowedRole(string? role)
        {
            return role == typeof(CitySalon.Models.Admin).Name ||
                   role == typeof(CitySalon.Models.Employee).Name ||
                   role == typeof(Customer).Name;
        }

        /// <summary>
        /// Fjerner validation-fejl fra navigation properties.
        /// De udfyldes ikke direkte i formularen.
        /// </summary>
        private void RemoveModelStateErrors()
        {
            ModelState.Remove("Booking.Customer");
            ModelState.Remove("Booking.CustomerId");
            ModelState.Remove("Booking.Treatment");
            ModelState.Remove("Booking.Employee");
            ModelState.Remove("Booking.StartTime");
            ModelState.Remove("Booking.EndTime");
        }

        /// <summary>
        /// Sætter hvilken kunde bookingen tilhører.
        /// </summary>
        private bool SetCustomerForBooking(string? role)
        {
            // Hvis en kunde selv booker.
            if (role == typeof(Customer).Name)
            {
                var customer = GetLoggedInCustomer();

                if (customer == null)
                {
                    ModelState.AddModelError(
                        "",
                        "Kunne ikke finde den indloggede kunde.");

                    return false;
                }

                Booking.CustomerId = customer.Id;

                return true;
            }

            // Hvis admin eller medarbejder booker.
            if (role == typeof(CitySalon.Models.Admin).Name ||
                role == typeof(CitySalon.Models.Employee).Name)
            {
                // Kunde skal vælges først.
                if (SelectedCustomerId <= 0)
                {
                    ModelState.AddModelError(
                        "",
                        "Du skal først finde og vælge en kunde.");

                    return false;
                }

                Booking.CustomerId = SelectedCustomerId;
            }

            return true;
        }

        /// <summary>
        /// Beregner bookingens start- og sluttid.
        /// Tjekker også åbningstid og ugyldige tider.
        /// </summary>
        private bool SetBookingTime()
        {
            // Tjekker at tidspunkt er valgt.
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

            // Forhindrer booking i fortiden.
            if (Booking.StartTime < DateTime.Now)
            {
                ModelState.AddModelError(
                    "",
                    "Du kan ikke booke en tid, der allerede er passeret.");

                return false;
            }

            // Beregner sluttid ud fra behandlingens varighed.
            Booking.EndTime =
                Booking.StartTime.AddMinutes(
                    treatment.DurationInMinutes);

            // Salonens lukketid.
            DateTime closingTime =
                SelectedDate.Date.Add(
                    new TimeSpan(18, 0, 0));

            // Tjekker om behandlingen går over lukketid.
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
        /// Genindlæser kundesøgning efter postback.
        /// </summary>
        private void LoadFoundCustomers()
        {
            if (!string.IsNullOrWhiteSpace(CustomerPhone))
            {
                FoundCustomers = _customerService.GetAll()
                    .Where(c =>
                        c.Phone.Contains(
                            CustomerPhone.Trim()))
                    .ToList();
            }
        }

        /// <summary>
        /// Henter information om den loggede ind kunde.
        /// </summary>
        private void LoadLoggedInCustomer()
        {
            var role = HttpContext.Session.GetString("Role");

            // Kun kunder får automatisk udfyldt information.
            if (role == typeof(Customer).Name)
            {
                IsCustomerLoggedIn = true;

                var customer = GetLoggedInCustomer();

                if (customer != null)
                {
                    Booking.CustomerId = customer.Id;
                    LoggedInCustomerName = customer.Name;
                    LoggedInCustomerEmail = customer.Email;
                }
            }
        }

        /// <summary>
        /// Finder den loggede ind kunde via username i session.
        /// </summary>
        private Customer? GetLoggedInCustomer()
        {
            string? username =
                HttpContext.Session.GetString("UserName");

            return _customerService.GetAll()
                .FirstOrDefault(c =>
                    c.UserName == username);
        }

        /// <summary>
        /// Indlæser dropdown-data og ledige tider.
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
        /// Finder ledige tider for valgt medarbejder og dato.
        /// </summary>
        private void LoadTimeSlots()
        {
            TimeSlots = new List<string>();

            int selectedEmployeeId = Booking.EmployeeId != 0
                ? Booking.EmployeeId
                : EmployeeId;

            // Stopper hvis ingen medarbejder er valgt.
            if (selectedEmployeeId == 0)
            {
                return;
            }

            // Salonens åbningstid.
            TimeSpan start = new TimeSpan(9, 0, 0);
            TimeSpan end = new TimeSpan(18, 0, 0);

            // Henter bookinger for medarbejderen på den valgte dato.
            List<Booking> bookingsForEmployeeAndDate =
                _bookingService.GetAll()
                .Where(b =>
                    b.EmployeeId == selectedEmployeeId &&
                    b.StartTime.Date == SelectedDate.Date)
                .ToList();

            // Gennemgår alle tidsintervaller.
            while (start < end)
            {
                DateTime slotStart = SelectedDate.Date.Add(start);
                DateTime slotEnd = slotStart.AddMinutes(30);

                // Tjekker om tiden overlapper en booking.
                bool isTaken =
                    bookingsForEmployeeAndDate.Any(booking =>
                        slotStart < booking.EndTime &&
                        slotEnd > booking.StartTime);

                // Tilføjer kun tider der ikke er optaget.
                if (!isTaken)
                {
                    TimeSlots.Add(start.ToString(@"hh\:mm"));
                }

                // Går videre til næste halve time.
                start = start.Add(TimeSpan.FromMinutes(30));
            }
        }

        /// <summary>
        /// Vælger den faste drop-in kunde.
        /// </summary>
        public IActionResult OnPostUseDropInCustomer()
        {
            // Finder kunden med telefonnummer 00000000.
            var customer = _customerService.GetAll()
                .FirstOrDefault(c =>
                    c.Phone == "00000000");

            if (customer == null)
            {
                ModelState.AddModelError(
                    "",
                    "Drop-in kunden blev ikke fundet.");

                LoadFormData();

                return Page();
            }

            // Fjerner gamle validation-fejl.
            ModelState.Clear();

            EmployeeId = Booking.EmployeeId;
            CustomerPhone = customer.Phone;
            SelectedCustomerId = customer.Id;

            // Viser drop-in kunden som søgeresultat.
            FoundCustomers = new List<Customer>
            {
                customer
            };

            // Genindlæser dropdowns og tider.
            LoadFormData();

            return Page();
        }
    }
}