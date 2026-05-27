using CitySalon.Models;
using CitySalon.Service;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CitySalon.Pages.Treatments
{
    /// <summary>
    /// PageModel til behandlingssiden.
    /// Bruges til at vise alle behandlinger
    /// og opdele dem efter kategori.
    /// </summary>
    public class GetAllTreatmentsModel : PageModel
    {
        // Service bruges til at hente behandlinger.
        private readonly TreatmentService _treatmentService;

        /// <summary>
        /// Indeholder alle behandlinger,
        /// som skal vises på siden.
        /// </summary>
        public List<Treatment> Treatments { get; set; } = new();

        /// <summary>
        /// Indeholder unikke behandlingskategorier.
        /// Bruges til at gruppere behandlingerne på siden.
        /// </summary>
        public List<string> TreatmentCategories { get; set; } = new();

        /// <summary>
        /// Constructor injection af TreatmentService.
        /// ASP.NET Core sender automatisk TreatmentService ind.
        /// </summary>
        public GetAllTreatmentsModel(TreatmentService treatmentService)
        {
            _treatmentService = treatmentService;
        }

        /// <summary>
        /// Kører når siden åbnes.
        /// Henter alle behandlinger og kategorier.
        /// </summary>
        public void OnGet()
        {
            // Henter alle behandlinger.
            Treatments = _treatmentService.GetAll();

            // Finder alle unikke kategorier.
            LoadTreatmentCategories();
        }

        /// <summary>
        /// Opretter en liste med unikke behandlingskategorier.
        /// Kategorienavnene sorteres alfabetisk.
        /// </summary>
        private void LoadTreatmentCategories()
        {
            TreatmentCategories = Treatments

                // Henter kun Category fra hver behandling.
                .Select(t => t.Category)

                // Fjerner dubletter.
                .Distinct()

                // Sorterer alfabetisk.
                .OrderBy(c => c)

                // Konverterer resultatet til en liste.
                .ToList();
        }
    }
}