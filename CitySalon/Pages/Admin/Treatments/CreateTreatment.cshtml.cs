using CitySalon.Helpers;
using CitySalon.Models;
using CitySalon.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CitySalon.Pages.Treatments
{
    /// <summary>
    /// PageModel til oprettelse af behandlinger.
    /// Kun administratorer har adgang til siden.
    /// </summary>
    public class CreateTreatmentModel : PageModel
    {
        private readonly TreatmentService _treatmentService;

        /// <summary>
        /// Binder formularens inputfelter til Treatment-objektet ved POST.
        /// </summary>
        [BindProperty]
        public Treatment Treatment { get; set; } = new Treatment();

        /// <summary>
        /// Opretter CreateTreatmentModel med adgang til TreatmentService.
        /// </summary>
        /// <param name="treatmentService">
        /// Service til håndtering af behandlinger.
        /// </param>
        public CreateTreatmentModel(TreatmentService treatmentService)
        {
            _treatmentService = treatmentService;
        }

        /// <summary>
        /// Viser siden til oprettelse af behandlinger.
        /// </summary>
        /// <returns>
        /// Redirecter til login hvis brugeren ikke er admin.
        /// Ellers vises siden.
        /// </returns>
        public IActionResult OnGet()
        {
            if (!CanAccessPage())
            {
                return RedirectToPage("/Login/LogInPage");
            }

            return Page();
        }

        /// <summary>
        /// Opretter en ny behandling i systemet.
        /// </summary>
        /// <returns>
        /// Redirecter til behandlingslisten hvis oprettelsen lykkes.
        /// Returnerer Page() hvis validation eller fejl opstår.
        /// </returns>
        public IActionResult OnPost()
        {
            if (!CanAccessPage())
            {
                return RedirectToPage("/Login/LogInPage");
            }

            // Hvis validation fejler,
            // vises siden igen.
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Sender behandlingen til service-laget.
                _treatmentService.Add(Treatment);

                return RedirectToPage("/Admin/Treatments/GetAllTreatments");
            }
            catch (Exception ex)
            {
                // Viser fejlbesked fra service-laget.
                ModelState.AddModelError("", ex.Message);

                return Page();
            }
        }

        /// <summary>
        /// Tjekker om brugeren er administrator.
        /// </summary>
        /// <returns>
        /// True hvis brugeren har adgang til siden.
        /// </returns>
        private bool CanAccessPage()
        {
            return SessionHelper.IsAdmin(HttpContext);
        }
    }
}