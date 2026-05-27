using CitySalon.Helpers;
using CitySalon.Models;
using CitySalon.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CitySalon.Pages.Treatments
{
    /// <summary>
    /// PageModel til redigering af behandlinger.
    /// Kun administratorer har adgang til siden.
    /// </summary>
    public class EditTreatmentModel : PageModel
    {
        private readonly TreatmentService _treatmentService;

        /// <summary>
        /// Binder formularens inputfelter til Treatment-objektet ved POST.
        /// </summary>
        [BindProperty]
        public Treatment Treatment { get; set; } = new Treatment();

        /// <summary>
        /// Opretter EditTreatmentModel med adgang til TreatmentService.
        /// </summary>
        /// <param name="treatmentService">
        /// Service til håndtering af behandlinger.
        /// </param>
        public EditTreatmentModel(TreatmentService treatmentService)
        {
            _treatmentService = treatmentService;
        }

        /// <summary>
        /// Henter behandlingen som skal redigeres og viser siden.
        /// </summary>
        /// <param name="id">Id på behandlingen som skal redigeres.</param>
        /// <returns>
        /// Redirecter til login hvis brugeren ikke er admin.
        /// Redirecter til behandlingslisten hvis behandlingen ikke findes.
        /// Ellers vises siden.
        /// </returns>
        public IActionResult OnGet(int id)
        {
            if (!CanAccessPage())
            {
                return RedirectToPage("/Login/LogInPage");
            }

            try
            {
                Treatment = _treatmentService.GetById(id);

                return Page();
            }
            catch (Exception)
            {
                return RedirectToPage("/Admin/Treatments/GetAllTreatments");
            }
        }

        /// <summary>
        /// Gemmer ændringer på den valgte behandling.
        /// </summary>
        /// <returns>
        /// Redirecter til behandlingslisten hvis opdateringen lykkes.
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
                // Sender den opdaterede behandling til service-laget.
                _treatmentService.Update(Treatment);

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