using CitySalon.Models;
using CitySalon.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CitySalon.Pages
{
    /// <summary>
    /// PageModel til kontakt-siden.
    /// Bruges til at sende kontaktbeskeder fra brugeren til systemet.
    /// </summary>
    public class ContactUsModel : PageModel
    {
        // Service bruges til at gemme kontaktbeskeder.
        // PageModel skal ikke selv arbejde direkte med databasen.
        private readonly ContactMessageService _contactMessageService;

        /// <summary>
        /// Constructor injection af ContactMessageService.
        /// ASP.NET Core sender automatisk ContactMessageService ind.
        /// </summary>
        public ContactUsModel(ContactMessageService contactMessageService)
        {
            _contactMessageService = contactMessageService;
        }

        /// <summary>
        /// Binder Name-feltet fra formularen til denne property.
        /// </summary>
        [BindProperty]
        public string? Name { get; set; }

        /// <summary>
        /// Binder Email-feltet fra formularen.
        /// </summary>
        [BindProperty]
        public string? Email { get; set; }

        /// <summary>
        /// Binder Message-feltet fra formularen.
        /// </summary>
        [BindProperty]
        public string? Message { get; set; }

        /// <summary>
        /// Bruges til at vise en succesbesked efter formularen er sendt.
        /// </summary>
        public string? SuccessMessage { get; set; }

        /// <summary>
        /// OnGet kører når kontakt-siden åbnes.
        /// </summary>
        public void OnGet()
        {
        }

        /// <summary>
        /// OnPost kører når brugeren sender kontaktformularen.
        /// Opretter og gemmer en ny kontaktbesked.
        /// </summary>
        public IActionResult OnPost()
        {
            try
            {
                // Opretter et nyt ContactMessage objekt
                // med data fra formularen.
                var contactMessage = new ContactMessage
                {
                    // Navn skrevet af brugeren.
                    Name = Name,

                    // Email skrevet af brugeren.
                    Email = Email,

                    // Beskeden fra brugeren.
                    Message = Message,

                    // Gemmer tidspunktet hvor beskeden blev sendt.
                    SentAt = DateTime.Now,

                    // Beskeden er ikke læst endnu.
                    IsRead = false
                };

                // Gemmer beskeden gennem service-laget.
                _contactMessageService.Add(contactMessage);

                // Viser succesbesked på siden.
                SuccessMessage = "Din besked er sendt.";

                // Rydder formularfelterne efter beskeden er sendt.
                ClearForm();

                // Viser siden igen.
                return Page();
            }
            catch (Exception ex)
            {
                // Hvis der sker fejl, vises fejlbeskeden på siden.
                ModelState.AddModelError("", ex.Message);

                return Page();
            }
        }

        /// <summary>
        /// Rydder formularen efter succesfuld afsendelse.
        /// </summary>
        private void ClearForm()
        {
            // Fjerner gamle validation-fejl og ModelState-data.
            ModelState.Clear();

            // Tømmer inputfelterne.
            Name = "";
            Email = "";
            Message = "";
        }
    }
}