using CitySalon.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CitySalon.Tests.ModelTests
{
    [TestClass]
    public class CustomerValidationTests
    {
        private bool IsCustomerValid(Customer customer)
        {
            ValidationContext context = new ValidationContext(customer);
            List<ValidationResult> results = new List<ValidationResult>();

            return Validator.TryValidateObject(
                customer,
                context,
                results,
                true);
        }

        [TestMethod]
        public void Customer_Should_Be_Valid_When_All_Properties_Are_Correct()
        {
            Customer customer = new Customer
            {
                UserName = "okan",
                Password = "1234",
                Role = typeof(Customer).Name,
                Name = "Okan Senol",
                Phone = "12345678",
                Email = "okan@test.dk"
            };

            Assert.IsTrue(IsCustomerValid(customer));
        }

        [TestMethod]
        public void Customer_Should_Be_Invalid_When_Name_Is_Missing()
        {
            Customer customer = new Customer
            {
                UserName = "okan",
                Password = "1234",
                Role = typeof(Customer).Name,
                Name = "",
                Phone = "12345678",
                Email = "okan@test.dk"
            };

            Assert.IsFalse(IsCustomerValid(customer));
        }

        [TestMethod]
        public void Customer_Should_Be_Invalid_When_Email_Is_Wrong()
        {
            Customer customer = new Customer
            {
                UserName = "okan",
                Password = "1234",
                Role = typeof(Customer).Name,
                Name = "Okan Senol",
                Phone = "12345678",
                Email = "forkertemail"
            };

            Assert.IsFalse(IsCustomerValid(customer));
        }

        [TestMethod]
        public void Customer_Should_Be_Invalid_When_Phone_Is_Not_8_Characters()
        {
            Customer customer = new Customer
            {
                UserName = "okan",
                Password = "1234",
                Role = typeof(Customer).Name,
                Name = "Okan Senol",
                Phone = "123",
                Email = "okan@test.dk"
            };

            Assert.IsFalse(IsCustomerValid(customer));
        }
    }
}