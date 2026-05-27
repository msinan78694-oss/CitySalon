using CitySalon.Helpers;
using CitySalon.Models;
using CitySalon.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CitySalon.Pages.Treatments
{
    /// <summary>
    /// PageModel til sletning af behandlinger.
    /// Kun administratorer har adgang til siden.
    /// </summary>
    public class DeleteTreatmentModel : PageModel
    {
        private readonly TreatmentService _treatmentService;

        /// <summary>
        /// Behandlingen som vises på siden før sletning.
        /// </summary>
        [BindProperty]
        public Treatment Treatment { get; set; } = new Treatment();

        /// <summary>
        /// Opretter DeleteTreatmentModel med adgang til TreatmentService.
        /// </summary>
        /// <param name="treatmentService">
        /// Service til håndtering af behandlinger.
        /// </param>
        public DeleteTreatmentModel(TreatmentService treatmentService)
        {
            _treatmentService = treatmentService;
        }

        /// <summary>
        /// Henter behandlingen som skal slettes og viser siden.
        /// </summary>
        /// <param name="id">Id på behandlingen som skal slettes.</param>
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
        /// Sletter den valgte behandling.
        /// </summary>
        /// <param name="id">Id på behandlingen som skal slettes.</param>
        /// <returns>
        /// Redirecter til behandlingslisten efter sletning.
        /// Returnerer Page() hvis der opstår fejl.
        /// </returns>
        public IActionResult OnPost(int id)
        {
            if (!CanAccessPage())
            {
                return RedirectToPage("/Login/LogInPage");
            }

            try
            {
                // Sletter behandlingen ud fra id'et fra route.
                _treatmentService.Delete(id);

                return RedirectToPage("/Admin/Treatments/GetAllTreatments");
            }
            catch (Exception ex)
            {
                // Viser fejlbesked fra service-laget.
                ModelState.AddModelError("", ex.Message);

                return LoadTreatmentOrRedirect(id);
            }
        }

        /// <summary>
        /// Henter behandlingen igen hvis sletning fejler.
        /// </summary>
        /// <param name="id">Id på behandlingen.</param>
        /// <returns>
        /// Returnerer Page() hvis behandlingen findes.
        /// Ellers redirectes der til behandlingslisten.
        /// </returns>
        private IActionResult LoadTreatmentOrRedirect(int id)
        {
            try
            {
                Treatment = _treatmentService.GetById(id);

                return Page();
            }
            catch
            {
                return RedirectToPage("/Admin/Treatments/GetAllTreatments");
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