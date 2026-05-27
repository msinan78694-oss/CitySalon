using CitySalon.Helpers;
using CitySalon.Models;
using CitySalon.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CitySalon.Pages.Admin.ContactMessages
{
    /// <summary>
    /// PageModel til visning af detaljer for en kontaktbesked.
    /// Kun administratorer har adgang til siden.
    /// </summary>
    public class MessageDetailsModel : PageModel
    {
        private readonly ContactMessageService _contactMessageService;

        /// <summary>
        /// Opretter MessageDetailsModel med adgang til ContactMessageService.
        /// </summary>
        /// <param name="contactMessageService">
        /// Service til håndtering af kontaktbeskeder.
        /// </param>
        public MessageDetailsModel(ContactMessageService contactMessageService)
        {
            _contactMessageService = contactMessageService;
        }

        /// <summary>
        /// Den kontaktbesked som vises på siden.
        /// </summary>
        public ContactMessage? ContactMessage { get; set; }

        /// <summary>
        /// Henter den valgte kontaktbesked og viser detaljesiden.
        /// Beskeden markeres samtidig som læst.
        /// </summary>
        /// <param name="id">Id på den besked som skal vises.</param>
        /// <returns>
        /// Redirecter til login hvis brugeren ikke er admin.
        /// Redirecter til oversigten hvis beskeden ikke findes.
        /// Ellers vises detaljesiden.
        /// </returns>
        public IActionResult OnGet(int id)
        {
            if (!SessionHelper.IsAdmin(HttpContext))
            {
                return RedirectToPage("/Login/LogInPage");
            }

            ContactMessage = _contactMessageService.GetById(id);

            if (ContactMessage == null)
            {
                return RedirectToPage("GetAllMessages");
            }

            // Marker beskeden som læst i databasen.
            _contactMessageService.MarkAsRead(id);

            // Opdaterer objektet på siden,
            // så status vises som læst med det samme.
            ContactMessage.IsRead = true;

            return Page();
        }
    }
}