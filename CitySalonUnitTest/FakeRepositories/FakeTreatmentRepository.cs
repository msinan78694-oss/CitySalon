using CitySalon.Models;
using CitySalon.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace CitySalon.Tests.FakeRepositories
{
    public class FakeTreatmentRepository : IRepository<Treatment>
    {
        private List<Treatment> treatments = new List<Treatment>();

        public List<Treatment> GetAll()
        {
            return treatments;
        }

        public Treatment GetById(int id)
        {
            return treatments.FirstOrDefault(t => t.TreatmentId == id);
        }

        public void Add(Treatment treatment)
        {
            treatments.Add(treatment);
        }

        public void Update(Treatment treatment)
        {
            Treatment existingTreatment = GetById(treatment.TreatmentId);

            if (existingTreatment != null)
            {
                existingTreatment.Name = treatment.Name;
                existingTreatment.Category = treatment.Category;
                existingTreatment.DurationInMinutes = treatment.DurationInMinutes;
                existingTreatment.Price = treatment.Price;
            }
        }

        public void Delete(int id)
        {
            Treatment treatment = GetById(id);

            if (treatment != null)
            {
                treatments.Remove(treatment);
            }
        }
    }
}