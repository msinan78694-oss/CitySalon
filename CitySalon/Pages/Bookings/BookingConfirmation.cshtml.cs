using CitySalon.Models;
using CitySalon.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CitySalon.Pages.Bookings
{
    public class BookingConfirmationModel : PageModel
    {
        private readonly BookingService _bookingService;

        public BookingConfirmationModel(BookingService bookingService)
        {
            _bookingService = bookingService;
        }

        public Booking? Booking { get; set; }

        public IActionResult OnGet(int id)
        { 
            var role = HttpContext.Session.GetString("Role");

            if (role != typeof(Customer).Name)
            {
                return RedirectToPage("/Login/LogInPage");
            }

            Booking = _bookingService.GetById(id);

            if (Booking == null)
            {
                return RedirectToPage("/Bookings/MyBookings");
            }

            return Page();
        }
    }
}