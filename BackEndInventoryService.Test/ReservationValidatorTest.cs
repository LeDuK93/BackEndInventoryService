using BackEndInventoryService.Model;
using BackEndInventoryService.Validator;
using FluentValidation;
using FluentValidation.TestHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace BackEndInventoryService.Tests
{
    [TestClass]
    public class ReservationValidatorTest
    {
        private IValidator<Reservation> ValidatorUnderTest { get; set; }

        [TestInitialize]
        public void Setup()
        {
            ValidatorUnderTest = new ReservationValidator();
        }

        [TestMethod]
        public void reservation_validator_should_throw_if_a_product_is_ordered_many_times_in_a_single_reservation()
        {
            var orders = new List<OrderLine>
            {
                new OrderLine("1", 1),
                new OrderLine("2", 2),
                new OrderLine("2", 2)
            };

            var resa = new Reservation(DateTime.Now, orders);

            ValidatorUnderTest.TestValidate(resa)
                .ShouldHaveValidationErrorFor(r => r.OrdersLines);
        }

        [TestMethod]
        public void reservation_validator_should_not_throw_if_all_products_are_ordered_only_once()
        {
            var orders = new List<OrderLine>
            {
                new OrderLine("1", 1),
                new OrderLine("2", 2),
                new OrderLine("3", 3)
            };

            var resa = new Reservation(DateTime.Now, orders);

            ValidatorUnderTest.TestValidate(resa)
                .ShouldNotHaveAnyValidationErrors();
        }
    }
}
