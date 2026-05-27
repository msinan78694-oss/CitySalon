using CitySalon.Data;
using CitySalon.Models;

namespace CitySalon.Repositories
{
    /// <summary>
    /// Repository til Treatment-klassen.
    /// Klassen bruges til at hente, oprette,
    /// opdatere og slette behandlinger i databasen.
    /// </summary>
    public class TreatmentRepository : IRepository<Treatment>
    {
        // DbContext bruges til at arbejde med databasen via Entity Framework Core.
        private readonly CityDbContext _context;

        /// <summary>
        /// Constructor injection af CityDbContext.
        /// ASP.NET Core sender automatisk database-contexten ind.
        /// </summary>
        public TreatmentRepository(CityDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Henter alle behandlinger fra databasen.
        /// </summary>
        public List<Treatment> GetAll()
        {
            return _context.Treatments.ToList();
        }

        /// <summary>
        /// Finder én behandling ud fra behandlingens id.
        /// Find returnerer null hvis behandlingen ikke findes.
        /// </summary>
        public Treatment GetById(int id)
        {
            return _context.Treatments.Find(id);
        }

        /// <summary>
        /// Opretter en ny behandling i databasen.
        /// SaveChanges gemmer ændringen permanent.
        /// </summary>
        public void Add(Treatment treatment)
        {
            _context.Treatments.Add(treatment);
            _context.SaveChanges();
        }

        /// <summary>
        /// Opdaterer en eksisterende behandling i databasen.
        /// Først findes behandlingen, derefter opdateres felterne.
        /// </summary>
        public void Update(Treatment treatment)
        {
            var existingTreatment = _context.Treatments.Find(treatment.TreatmentId);

            if (existingTreatment != null)
            {
                existingTreatment.Name = treatment.Name;
                existingTreatment.DurationInMinutes = treatment.DurationInMinutes;
                existingTreatment.Category = treatment.Category;
                existingTreatment.Price = treatment.Price;

                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Sletter en behandling fra databasen ud fra behandlingens id.
        /// </summary>
        public void Delete(int id)
        {
            var treatment = _context.Treatments.Find(id);

            if (treatment != null)
            {
                _context.Treatments.Remove(treatment);
                _context.SaveChanges();
            }
        }
    }
}