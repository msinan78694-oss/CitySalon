using CitySalon.Helpers;
using CitySalon.Models;
using CitySalon.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CitySalon.Pages.Admin.ContactMessages
{
    /// <summary>
    /// PageModel til visning af alle kontaktbeskeder.
    /// Kun administratorer har adgang til siden.
    /// </summary>
    public class GetAllMessagesModel : PageModel
    {
        private readonly ContactMessageService _contactMessageService;

        /// <summary>
        /// Opretter GetAllMessagesModel med adgang til ContactMessageService.
        /// </summary>
        /// <param name="contactMessageService">
        /// Service til håndtering af kontaktbeskeder.
        /// </param>
        public GetAllMessagesModel(ContactMessageService contactMessageService)
        {
            _contactMessageService = contactMessageService;
        }

        /// <summary>
        /// Liste med alle kontaktbeskeder.
        /// Bruges af Razor-siden til at vise beskederne.
        /// </summary>
        public List<ContactMessage> ContactMessages { get; set; } = new();

        /// <summary>
        /// Henter alle kontaktbeskeder og viser siden.
        /// </summary>
        /// <returns>
        /// Redirecter til login hvis brugeren ikke er admin.
        /// Ellers vises siden med alle beskeder.
        /// </returns>
        public IActionResult OnGet()
        {
            if (!SessionHelper.IsAdmin(HttpContext))
            {
                return RedirectToPage("/Login/LogInPage");
            }

            ContactMessages = _contactMessageService.GetAll();

            return Page();
        }
    }
}