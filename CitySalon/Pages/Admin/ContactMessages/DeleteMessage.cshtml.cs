using CitySalon.Helpers;
using CitySalon.Models;
using CitySalon.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CitySalon.Pages.Admin.ContactMessages
{
    /// <summary>
    /// PageModel til sletning af kontaktbeskeder.
    /// Kun administratorer har adgang til siden.
    /// </summary>
    public class DeleteMessageModel : PageModel
    {
        private readonly ContactMessageService _contactMessageService;

        /// <summary>
        /// Opretter DeleteMessageModel med adgang til ContactMessageService.
        /// </summary>
        /// <param name="contactMessageService">
        /// Service til håndtering af kontaktbeskeder.
        /// </param>
        public DeleteMessageModel(ContactMessageService contactMessageService)
        {
            _contactMessageService = contactMessageService;
        }

        /// <summary>
        /// Den kontaktbesked som skal vises og slettes.
        /// </summary>
        public ContactMessage? ContactMessage { get; set; }

        /// <summary>
        /// Binder kontaktbeskedens id fra formularen.
        /// </summary>
        [BindProperty]
        public int ContactMessageId { get; set; }

        /// <summary>
        /// Henter den valgte kontaktbesked og viser siden.
        /// </summary>
        /// <param name="id">Id på beskeden som skal slettes.</param>
        /// <returns>
        /// Redirecter til login hvis brugeren ikke er admin.
        /// Redirecter til oversigten hvis beskeden ikke findes.
        /// Ellers vises siden.
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

            ContactMessageId = ContactMessage.ContactMessageId;

            return Page();
        }

        /// <summary>
        /// Sletter den valgte kontaktbesked.
        /// </summary>
        /// <returns>
        /// Redirecter til login hvis brugeren ikke er admin.
        /// Redirecter tilbage til beskedoversigten efter sletning.
        /// </returns>
        public IActionResult OnPost()
        {
            if (!SessionHelper.IsAdmin(HttpContext))
            {
                return RedirectToPage("/Login/LogInPage");
            }

            _contactMessageService.Delete(ContactMessageId);

            return RedirectToPage("GetAllMessages");
        }
    }
}