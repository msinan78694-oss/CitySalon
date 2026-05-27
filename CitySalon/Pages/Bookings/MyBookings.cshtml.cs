using CitySalon.Models;
using CitySalon.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CitySalon.Pages.Bookings
{
    /// <summary>
    /// PageModel til kundens egne bookinger.
    /// Viser både kommende og tidligere bookinger for den kunde,
    /// der er logget ind.
    /// </summary>
    public class MyBookingsModel : PageModel
    {
        // Service til håndtering af booking-data.
        private readonly BookingService _bookingService;

        // Service til håndtering af kunde-data.
        private readonly CustomerService _customerService;

        /// <summary>
        /// Liste med kundens kommende bookinger.
        /// </summary>
        public List<Booking> UpcomingBookings { get; set; } = new();

        /// <summary>
        /// Liste med kundens tidligere bookinger.
        /// </summary>
        public List<Booking> PastBookings { get; set; } = new();

        /// <summary>
        /// Bestemmer om tidligere bookinger skal vises.
        /// SupportsGet=true gør at værdien kan komme fra URL'en.
        /// Fx: /Bookings/MyBookings?ShowPastBookings=true
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public bool ShowPastBookings { get; set; }

        /// <summary>
        /// Constructor injection af BookingService og CustomerService.
        /// ASP.NET Core sender automatisk services ind.
        /// </summary>
        public MyBookingsModel(
            BookingService bookingService,
            CustomerService customerService)
        {
            _bookingService = bookingService;
            _customerService = customerService;
        }

        /// <summary>
        /// Viser kundens kommende og tidligere bookinger.
        /// Kører når kunden åbner siden "Mine bookinger".
        /// </summary>
        public IActionResult OnGet()
        {
            var role = HttpContext.Session.GetString("Role");

            // Kun kunder må se "Mine bookinger".
            if (role != typeof(Customer).Name)
            {
                return RedirectToPage("/Login/LogInPage");
            }

            // Finder kunden ud fra sessionens UserName.
            var customer = GetLoggedInCustomer();

            if (customer == null)
            {
                return RedirectToPage("/Login/LogInPage");
            }

            // Henter og opdeler kundens bookinger.
            LoadCustomerBookings(customer.Id);

            return Page();
        }

        /// <summary>
        /// Finder den kunde, der er logget ind.
        /// Kunden findes ved at sammenligne UserName fra session
        /// med kundernes UserName i databasen.
        /// </summary>
        private Customer? GetLoggedInCustomer()
        {
            string? username = HttpContext.Session.GetString("UserName");

            return _customerService.GetAll()
                .FirstOrDefault(c => c.UserName == username);
        }

        /// <summary>
        /// Deler kundens bookinger op i kommende og tidligere tider.
        /// </summary>
        private void LoadCustomerBookings(int customerId)
        {
            var allBookings = _bookingService.GetAll()
                .Where(b => b.CustomerId == customerId)
                .ToList();

            // Kommende bookinger er dem, hvor sluttiden ikke er passeret endnu.
            UpcomingBookings = allBookings
                .Where(b => b.EndTime >= DateTime.Now)
                .OrderBy(b => b.StartTime)
                .ToList();

            // Tidligere bookinger er dem, hvor sluttiden er passeret.
            PastBookings = allBookings
                .Where(b => b.EndTime < DateTime.Now)
                .OrderByDescending(b => b.StartTime)
                .ToList();
        }
    }
}