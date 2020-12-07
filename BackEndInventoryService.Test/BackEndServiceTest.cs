using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using FluentAssertions;
using System.Linq;
using BackEndInventoryService.Service;
using BackEndInventoryService.Model;

namespace BackEndInventoryService.Test
{
    [TestClass]
    public class BackEndServiceTest
    {
        private BackEndService backEndServiceUnderTest { get; set; }

        [TestInitialize]
        public void Setup()
        {
            backEndServiceUnderTest = new BackEndService();
        }

        [TestMethod]
        public void service_should_throw_argument_exception_if_user_tries_to_create_a_reservation_with_an_unknown_product()
        {
            backEndServiceUnderTest.Products.Add("1", new Inventory("1", 1));

            var orders = new List<OrderLine>
            {
                new OrderLine("1", 1),
                new OrderLine("2", 2)
            };

            Action execute = () => backEndServiceUnderTest.CreateReservation(orders);

            execute.Should().Throw<ArgumentException>().WithMessage("ProductId [2] does not exist");
        }

        [TestMethod]
        public void creating_reservation_should_set_reservation_availability_to_false_if_one_of_its_product_is_not_available()
        {
             backEndServiceUnderTest.Products.Add("1", new Inventory("1", 1));

            var orders = new List<OrderLine>
            {
                new OrderLine("1", 2)
            };

            var reservation = backEndServiceUnderTest.CreateReservation(orders);

            reservation.IsAvailable.Should().BeFalse();
        }

        [TestMethod]
        public void creating_reservation_should_set_reservation_availability_to_true_if_all_products_are_available()
        {
            backEndServiceUnderTest.Products.Add("1", new Inventory("1", 1));
            backEndServiceUnderTest.Products.Add("2", new Inventory("2", 2));

            var orders = new List<OrderLine>
            {
                new OrderLine("1", 1),
                new OrderLine("2", 2)
            };

            Reservation resa = backEndServiceUnderTest.CreateReservation(orders);

            resa.IsAvailable.Should().BeTrue();
        }


        [TestMethod]
        public void inventory_pagination_should_return_products_between_cursor_and_limit()
        {
            CreateProducts(10);

            var products = backEndServiceUnderTest.GetInventory(5, 10).Where(p => Int64.Parse(p.ProductId) >= 5 && Int64.Parse(p.ProductId) <= 10);
            products.Count().Should().Be(5);
        }

        [TestMethod]
        public void reservation_pagination_should_return_products_between_cursor_and_limit()
        {
            CreateProducts(10);

            for (int i = 1; i < 11; i++)
            {
                var orders = new List<OrderLine>()
                {
                    new OrderLine(i.ToString(), i)
                };
                backEndServiceUnderTest.CreateReservation(orders);
            }

            var reservations = backEndServiceUnderTest.GetReservations(0, 5);

            reservations.Count().Should().Be(5);
        }

        [TestMethod]
        public void creating_reservation_should_change_inventory_quantity()
        {
            backEndServiceUnderTest.Products.Add("1", new Inventory("1", 100));

            var orders = new List<OrderLine>
            {
                new OrderLine("1", 1)
            };

            backEndServiceUnderTest.CreateReservation(orders);

            backEndServiceUnderTest.Products["1"].Quantity.Should().Be(99);
        }

        private void CreateProducts(int range)
        {
            for (int i = 1; i <= range; ++i)
            {
                backEndServiceUnderTest.Products.Add(i.ToString(), new Inventory($"{i}", i));
            }
        }
    }
}
