using CitySalon.Models;
using CitySalon.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CitySalon.Pages.Login
{
    /// <summary>
    /// PageModel til login-siden.
    /// Bruges til at logge admin, medarbejder og kunder ind i systemet.
    /// </summary>
    public class LogInPageModel : PageModel
    {
        // Service-laget bruges til at håndtere login-logikken.
        // PageModel skal ikke selv arbejde direkte med databasen.
        private readonly UserService _userService;

        /// <summary>
        /// Constructor injection af UserService.
        /// ASP.NET Core sender automatisk UserService ind.
        /// </summary>
        public LogInPageModel(UserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Brugernavn fra login-formularen.
        /// BindProperty betyder, at værdien fra inputfeltet i formularen
        /// automatisk bliver gemt i denne property, når brugeren trykker Login.
        /// </summary>
        [BindProperty]
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Kodeord fra login-formularen.
        /// </summary>
        [BindProperty]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Bruges til at vise fejlbeskeder på login-siden.
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;

        /// <summary>
        /// Kører når login-siden åbnes første gang.
        /// </summary>
        public void OnGet()
        {
        }

        /// <summary>
        /// Kører når brugeren sender login-formularen.
        /// Tjekker input, forsøger login og sender brugeren videre efter rolle.
        /// </summary>
        public IActionResult OnPost()
        {
            // Først tjekker vi, om brugernavn og kodeord er udfyldt.
            if (!ValidateInput())
            {
                return Page();
            }

            try
            {
                // Vi sender brugernavn og kodeord til UserService,
                // som tjekker om brugeren findes.
                var user = _userService.Login(UserName, Password);

                // Hvis UserService ikke finder en bruger, vises login-siden igen.
                if (user == null)
                {
                    ErrorMessage = "Forkert brugernavn eller kodeord.";
                    return Page();
                }

                // Hvis login lykkes, gemmes brugerens oplysninger i session.
                SaveUserInSession(user);

                // Brugeren sendes videre afhængigt af sin rolle.
                return RedirectUserByRole(user.Role);
            }
            catch (Exception ex)
            {
                // Hvis der sker en fejl i service-laget, vises beskeden på siden.
                ErrorMessage = ex.Message;
                return Page();
            }
        }

        /// <summary>
        /// Tjekker at brugeren har skrevet både brugernavn og kodeord.
        /// Returnerer false hvis noget mangler.
        /// </summary>
        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(UserName))
            {
                ErrorMessage = "Brugernavn skal udfyldes.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Kodeord skal udfyldes.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gemmer de vigtigste brugeroplysninger i session,
        /// så andre sider kan se, hvem der er logget ind.
        /// </summary>
        private void SaveUserInSession(User user)
        {
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserName", user.UserName);
            HttpContext.Session.SetString("Role", user.Role);
        }

        /// <summary>
        /// Sender brugeren til det rigtige dashboard ud fra brugerens rolle.
        /// Admin sendes til admin-dashboard.
        /// Employee sendes til employee-dashboard.
        /// Customer sendes til customer-dashboard.
        /// </summary>
        private IActionResult RedirectUserByRole(string role)
        {
            if (role == typeof(CitySalon.Models.Admin).Name)
            {
                return RedirectToPage("/Admin/Dashboard");
            }

            if (role == typeof(CitySalon.Models.Employee).Name)
            {
                return RedirectToPage("/Employee/Dashboard");
            }

            if (role == typeof(Customer).Name)
            {
                return RedirectToPage("/Customers/Dashboard");
            }

            return RedirectToPage("/Index");
        }
    }
}