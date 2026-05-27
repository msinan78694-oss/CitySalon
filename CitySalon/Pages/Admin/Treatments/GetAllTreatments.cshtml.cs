using CitySalon.Helpers;
using CitySalon.Models;
using CitySalon.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CitySalon.Pages.Admin.Treatments
{
    /// <summary>
    /// PageModel til visning af alle behandlinger.
    /// Kun administratorer har adgang til siden.
    /// </summary>
    public class GetAllTreatmentsModel : PageModel
    {
        private readonly TreatmentService _treatmentService;

        /// <summary>
        /// Liste med alle behandlinger som vises på Razor-siden.
        /// </summary>
        public List<Treatment> Treatments { get; set; } = new();

        /// <summary>
        /// Opretter GetAllTreatmentsModel med adgang til TreatmentService.
        /// </summary>
        /// <param name="treatmentService">
        /// Service til håndtering af behandlinger.
        /// </param>
        public GetAllTreatmentsModel(TreatmentService treatmentService)
        {
            _treatmentService = treatmentService;
        }

        /// <summary>
        /// Henter alle behandlinger og viser siden.
        /// </summary>
        /// <returns>
        /// Redirecter til login hvis brugeren ikke er admin.
        /// Ellers vises siden.
        /// </returns>
        public IActionResult OnGet()
        {
            if (!SessionHelper.IsAdmin(HttpContext))
            {
                return RedirectToPage("/Login/LogInPage");
            }

            Treatments = _treatmentService.GetAll();

            return Page();
        }
    }
}