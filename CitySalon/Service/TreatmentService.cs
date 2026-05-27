using CitySalon.Models;
using CitySalon.Repositories;

namespace CitySalon.Service
{
    /// <summary>
    /// Service-klasse til Treatment.
    /// Klassen indeholder forretningslogik for behandlinger
    /// og kalder repository-laget for databasearbejde.
    /// </summary>
    public class TreatmentService
    {
        // Repository bruges til at hente og gemme behandlinger i databasen.
        private readonly IRepository<Treatment> _treatmentRepository;

        /// <summary>
        /// Constructor injection af IRepository<Treatment>.
        /// ASP.NET Core sender automatisk repository ind.
        /// </summary>
        public TreatmentService(IRepository<Treatment> treatmentRepository)
        {
            _treatmentRepository = treatmentRepository;
        }

        /// <summary>
        /// Henter alle behandlinger.
        /// </summary>
        public List<Treatment> GetAll()
        {
            return _treatmentRepository.GetAll();
        }

        /// <summary>
        /// Finder én behandling ud fra behandlingens id.
        /// Tjekker først om id'et er gyldigt.
        /// </summary>
        public Treatment GetById(int id)
        {
            if (id <= 0)
            {
                throw new Exception("Ugyldigt behandlings-id.");
            }

            var treatment = _treatmentRepository.GetById(id);

            if (treatment == null)
            {
                throw new Exception("Behandlingen blev ikke fundet.");
            }

            return treatment;
        }

        /// <summary>
        /// Opretter en ny behandling.
        /// Først valideres behandlingen,
        /// derefter gemmes behandlingen i databasen.
        /// </summary>
        public void Add(Treatment treatment)
        {
            ValidateTreatment(treatment);

            _treatmentRepository.Add(treatment);
        }

        /// <summary>
        /// Opdaterer en eksisterende behandling.
        /// Først valideres behandlingen,
        /// derefter tjekkes det at behandlingen findes.
        /// </summary>
        public void Update(Treatment treatment)
        {
            ValidateTreatment(treatment);

            if (treatment.TreatmentId <= 0)
            {
                throw new Exception("Ugyldigt behandlings-id.");
            }

            GetById(treatment.TreatmentId);

            _treatmentRepository.Update(treatment);
        }

        /// <summary>
        /// Sletter en behandling ud fra id.
        /// GetById bruges først for at sikre,
        /// at behandlingen findes.
        /// </summary>
        public void Delete(int id)
        {
            GetById(id);

            _treatmentRepository.Delete(id);
        }

        /// <summary>
        /// Tjekker at behandlingen har gyldige oplysninger.
        /// Bruges før behandlingen gemmes eller opdateres.
        /// </summary>
        private void ValidateTreatment(Treatment treatment)
        {
            if (treatment == null)
            {
                throw new ArgumentNullException(nameof(treatment), "Behandling mangler.");
            }

            if (string.IsNullOrWhiteSpace(treatment.Name))
            {
                throw new Exception("Navn på behandling skal udfyldes.");
            }

            if (string.IsNullOrWhiteSpace(treatment.Category))
            {
                throw new Exception("Kategori skal udfyldes.");
            }

            if (treatment.DurationInMinutes <= 0)
            {
                throw new Exception("Varighed skal være større end 0.");
            }

            if (treatment.Price <= 0)
            {
                throw new Exception("Pris skal være større end 0.");
            }
        }
    }
}