using CitySalon.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CitySalon.Tests.ModelTests
{
    [TestClass]
    public class TreatmentValidationTests
    {
        private bool IsTreatmentValid(Treatment treatment)
        {
            ValidationContext context = new ValidationContext(treatment);
            List<ValidationResult> results = new List<ValidationResult>();

            return Validator.TryValidateObject(
                treatment,
                context,
                results,
                true);
        }

        [TestMethod]
        public void Treatment_Should_Be_Valid_When_All_Properties_Are_Correct()
        {
            Treatment treatment = new Treatment
            {
                Name = "Herreklip",
                Category = "Klipning",
                DurationInMinutes = 30,
                Price = 250
            };

            Assert.IsTrue(IsTreatmentValid(treatment));
        }

        [TestMethod]
        public void Treatment_Should_Be_Invalid_When_Name_Is_Missing()
        {
            Treatment treatment = new Treatment
            {
                Name = "",
                Category = "Klipning",
                DurationInMinutes = 30,
                Price = 250
            };

            Assert.IsFalse(IsTreatmentValid(treatment));
        }

        [TestMethod]
        public void Treatment_Should_Be_Invalid_When_Duration_Is_Zero()
        {
            Treatment treatment = new Treatment
            {
                Name = "Herreklip",
                Category = "Klipning",
                DurationInMinutes = 0,
                Price = 250
            };

            Assert.IsFalse(IsTreatmentValid(treatment));
        }

        [TestMethod]
        public void Treatment_Should_Be_Invalid_When_Price_Is_Zero()
        {
            Treatment treatment = new Treatment
            {
                Name = "Herreklip",
                Category = "Klipning",
                DurationInMinutes = 30,
                Price = 0
            };

            Assert.IsFalse(IsTreatmentValid(treatment));
        }
    }
}