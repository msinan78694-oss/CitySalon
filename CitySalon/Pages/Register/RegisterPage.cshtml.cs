using CitySalon.Models;
using CitySalon.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CitySalon.Pages.Register
{
    /// <summary>
    /// PageModel til registreringssiden.
    /// Bruges til at oprette en ny kunde i systemet.
    /// </summary>
    public class RegisterPageModel : PageModel
    {
        // Service bruges til at oprette og hente kunder.
        private readonly CustomerService _customerService;

        /// <summary>
        /// Constructor injection af CustomerService.
        /// ASP.NET Core sender automatisk CustomerService ind.
        /// </summary>
        public RegisterPageModel(CustomerService customerService)
        {
            _customerService = customerService;
        }

        /// <summary>
        /// Hele formularen bindes til Customer-objektet.
        /// Når formularen sendes, udfyldes Customer automatisk.
        /// </summary>
        [BindProperty]
        public Customer Customer { get; set; } = new Customer();

        /// <summary>
        /// Viser register-siden.
        /// Kører når siden åbnes første gang.
        /// </summary>
        public void OnGet()
        {
        }

        /// <summary>
        /// Kører når brugeren sender formularen.
        /// Forbereder kunden, validerer input og gemmer kunden.
        /// </summary>
        public IActionResult OnPost()
        {
            // Sætter værdier som ikke kommer fra formularen.
            PrepareCustomer();

            // Fjerner valideringsfejl for felter brugeren ikke udfylder.
            RemoveModelStateErrors();

            // Laver ekstra tjek på login-felter.
            ValidateLoginFields();

            // Tjekker om brugernavn eller email allerede findes.
            ValidateUniqueCustomer();

            // Hvis der er fejl, vises siden igen.
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Gemmer kunden via service-laget.
                _customerService.Add(Customer);

                // Sender brugeren til login efter oprettelse.
                return RedirectToPage("/Login/LogInPage");
            }
            catch (Exception ex)
            {
                // Viser fejl fra service-laget på siden.
                ModelState.AddModelError("", ex.Message);
                return Page();
            }
        }

        /// <summary>
        /// Sætter standardværdier for nye kunder.
        /// Role og Bookings kommer ikke direkte fra formularen.
        /// </summary>
        private void PrepareCustomer()
        {
            Customer.Role = typeof(Customer).Name;
            Customer.Bookings = new List<Booking>();
        }

        /// <summary>
        /// Fjerner fejl fra properties som ikke skrives direkte i formularen.
        /// Det forhindrer ModelState i at fejle på værdier,
        /// som programmet selv sætter.
        /// </summary>
        private void RemoveModelStateErrors()
        {
            ModelState.Remove("Customer.Role");
            ModelState.Remove("Customer.Bookings");
        }

        /// <summary>
        /// Tjekker at brugernavn og kodeord er udfyldt.
        /// Tilføjer fejl til ModelState hvis noget mangler.
        /// </summary>
        private void ValidateLoginFields()
        {
            if (string.IsNullOrWhiteSpace(Customer.UserName))
            {
                ModelState.AddModelError("", "Brugernavn skal udfyldes.");
            }

            if (string.IsNullOrWhiteSpace(Customer.Password))
            {
                ModelState.AddModelError("", "Kodeord skal udfyldes.");
            }
        }

        /// <summary>
        /// Tjekker om der allerede findes en kunde
        /// med samme brugernavn eller email.
        /// </summary>
        private void ValidateUniqueCustomer()
        {
            var customers = _customerService.GetAll();

            bool usernameExists = customers
                .Any(c => c.UserName == Customer.UserName);

            if (usernameExists)
            {
                ModelState.AddModelError("", "Der findes allerede en bruger med dette brugernavn.");
            }

            bool emailExists = customers
                .Any(c => c.Email == Customer.Email);

            if (emailExists)
            {
                ModelState.AddModelError("", "Der findes allerede en bruger med denne email.");
            }
        }
    }
}