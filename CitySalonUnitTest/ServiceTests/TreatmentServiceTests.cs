using CitySalon.Models;
using CitySalon.Service;
using CitySalon.Tests.FakeRepositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CitySalon.Tests.ServiceTests
{
    [TestClass]
    public class TreatmentServiceTests
    {
        [TestMethod]
        public void Add_Should_Add_Treatment_When_Data_Is_Valid()
        {
            FakeTreatmentRepository fakeRepository = new FakeTreatmentRepository();

            TreatmentService service =
                new TreatmentService(fakeRepository);

            Treatment treatment = new Treatment
            {
                Name = "Herreklip",
                Category = "Klipning",
                DurationInMinutes = 30,
                Price = 250
            };

            service.Add(treatment);

            Assert.AreEqual(1, fakeRepository.GetAll().Count);
        }

        [TestMethod]
        public void Add_Should_Throw_Exception_When_Name_Is_Missing()
        {
            FakeTreatmentRepository fakeRepository = new FakeTreatmentRepository();

            TreatmentService service =
                new TreatmentService(fakeRepository);

            Treatment treatment = new Treatment
            {
                Name = "",
                Category = "Klipning",
                DurationInMinutes = 30,
                Price = 250
            };

            bool exceptionThrown = false;

            try
            {
                service.Add(treatment);
            }
            catch (Exception)
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown);
        }

        [TestMethod]
        public void Add_Should_Throw_Exception_When_Duration_Is_Zero()
        {
            FakeTreatmentRepository fakeRepository = new FakeTreatmentRepository();

            TreatmentService service =
                new TreatmentService(fakeRepository);

            Treatment treatment = new Treatment
            {
                Name = "Fade",
                Category = "Klipning",
                DurationInMinutes = 0,
                Price = 250
            };

            bool exceptionThrown = false;

            try
            {
                service.Add(treatment);
            }
            catch (Exception)
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown);
        }

        [TestMethod]
        public void Add_Should_Throw_Exception_When_Price_Is_Zero()
        {
            FakeTreatmentRepository fakeRepository = new FakeTreatmentRepository();

            TreatmentService service =
                new TreatmentService(fakeRepository);

            Treatment treatment = new Treatment
            {
                Name = "Fade",
                Category = "Klipning",
                DurationInMinutes = 30,
                Price = 0
            };

            bool exceptionThrown = false;

            try
            {
                service.Add(treatment);
            }
            catch (Exception)
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown);
        }

        [TestMethod]
        public void GetById_Should_Throw_Exception_When_Id_Is_Zero()
        {
            FakeTreatmentRepository fakeRepository = new FakeTreatmentRepository();

            TreatmentService service =
                new TreatmentService(fakeRepository);

            bool exceptionThrown = false;

            try
            {
                service.GetById(0);
            }
            catch (Exception)
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown);
        }
    }
}