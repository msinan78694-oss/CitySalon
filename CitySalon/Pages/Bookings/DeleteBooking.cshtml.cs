using CitySalon.Helpers;
using CitySalon.Models;
using CitySalon.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CitySalon.Pages.Bookings
{
    /// <summary>
    /// PageModel til sletning af en booking.
    /// Bruges af admin og medarbejdere til at se en booking og derefter slette den.
    /// </summary>
    public class DeleteBookingModel : PageModel
    {
        // Service til håndtering af booking-data.
        private readonly BookingService _bookingService;

        /// <summary>
        /// Booking-objektet som siden arbejder med.
        /// BindProperty binder hidden input fra formularen
        /// til Booking-objektet ved POST.
        /// </summary>
        [BindProperty]
        public Booking Booking { get; set; } = new Booking();

        /// <summary>
        /// Constructor injection af BookingService.
        /// ASP.NET Core sender automatisk BookingService ind.
        /// </summary>
        public DeleteBookingModel(BookingService bookingService)
        {
            _bookingService = bookingService;
        }

        /// <summary>
        /// Viser bookingen før sletning.
        /// Kører når brugeren åbner DeleteBooking-siden.
        /// </summary>
        public IActionResult OnGet(int id)
        {
            // Kun admin eller medarbejder må slette bookinger.
            if (!CanAccessPage())
            {
                return RedirectToPage("/Login/LogInPage");
            }

            try
            {
                // Henter bookingen ud fra id'et i URL'en.
                Booking = _bookingService.GetById(id);

                return Page();
            }
            catch (Exception)
            {
                return RedirectToPage("BookingOverview");
            }
        }

        /// <summary>
        /// Sletter bookingen.
        /// Kører når brugeren bekræfter sletning i formularen.
        /// </summary>
        public IActionResult OnPost(int id)
        {
            // Tjekker adgang igen ved POST.
            if (!CanAccessPage())
            {
                return RedirectToPage("/Login/LogInPage");
            }

            try
            {
                // Sletter bookingen ud fra id'et fra route.
                _bookingService.Delete(id);

                return RedirectToPage("BookingOverview");
            }
            catch (Exception ex)
            {
                // Viser fejlbesked fra service-laget.
                ModelState.AddModelError("", ex.Message);

                return LoadBookingOrRedirect(id);
            }
        }

        /// <summary>
        /// Henter bookingen igen, hvis sletning fejler.
        /// Hvis bookingen ikke kan hentes, sendes brugeren tilbage til oversigten.
        /// </summary>
        private IActionResult LoadBookingOrRedirect(int id)
        {
            try
            {
                // Henter bookingen igen,
                // så siden kan vises med fejlbesked og bookinginfo.
                Booking = _bookingService.GetById(id);

                return Page();
            }
            catch
            {
                return RedirectToPage("BookingOverview");
            }
        }

        /// <summary>
        /// Samler adgangstjekket ét sted.
        /// Returnerer true hvis brugeren er admin eller medarbejder.
        /// </summary>
        private bool CanAccessPage()
        {
            return SessionHelper.IsAdminOrEmployee(HttpContext);
        }
    }
}