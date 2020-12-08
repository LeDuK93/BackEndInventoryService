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
            backEndServiceUnderTest.Products.Add("2", new Inventory("2", 1));

            var orders = new List<OrderLine>
            {
                new OrderLine("1", 1),
                new OrderLine("2", 2)
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

            var reservation = backEndServiceUnderTest.CreateReservation(orders);
            reservation.IsAvailable.Should().BeTrue();
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

            var reservations = backEndServiceUnderTest.GetAvailableReservations(0, 5);

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

        [TestMethod]
        public void adding_enough_products_in_inventory_should_complete_pending_reservation()
        {
            backEndServiceUnderTest.Products.Add("1", new Inventory("1", 10));

            var orders = new List<OrderLine>
            {
                new OrderLine("1", 20)
            };

            backEndServiceUnderTest.CreateReservation(orders);
            backEndServiceUnderTest.PendingReservations.Count.Should().Be(1);
            backEndServiceUnderTest.AvailableReservations.Count.Should().Be(0);

            backEndServiceUnderTest.SetInventory("1", backEndServiceUnderTest.Products["1"].Quantity + 10);
            backEndServiceUnderTest.CompletePendingReservations("1");

            backEndServiceUnderTest.PendingReservations.Count.Should().Be(0);
            backEndServiceUnderTest.AvailableReservations.Count.Should().Be(1);
        }

        [TestMethod]
        public void adding_enough_products_in_inventory_should_not_complete_pending_reservation_with_several_unavailable_products()
        {
            backEndServiceUnderTest.Products.Add("1", new Inventory("1", 10));
            backEndServiceUnderTest.Products.Add("2", new Inventory("2", 10));

            var orders = new List<OrderLine>
            {
                new OrderLine("1", 20),
                new OrderLine("2", 20)
            };

            backEndServiceUnderTest.CreateReservation(orders);
            backEndServiceUnderTest.PendingReservations.Count.Should().Be(1);
            backEndServiceUnderTest.AvailableReservations.Count.Should().Be(0);

            backEndServiceUnderTest.SetInventory("1", backEndServiceUnderTest.Products["1"].Quantity + 10);
            backEndServiceUnderTest.CompletePendingReservations("1");

            backEndServiceUnderTest.PendingReservations.Count.Should().Be(1);
            backEndServiceUnderTest.AvailableReservations.Count.Should().Be(0);
        }

        [TestMethod]
        public void creating_2_pending_reservations_and_adding_enough_inventory_to_complete_first_reservation_should_complete_first_reservation_and_not_the_second_one()
        {
            backEndServiceUnderTest.Products.Add("1", new Inventory("1", 0));

            var orders = new List<OrderLine>
            {
                new OrderLine("1", 1)
            };

            backEndServiceUnderTest.CreateReservation(orders);
            backEndServiceUnderTest.CreateReservation(orders);
            backEndServiceUnderTest.PendingReservations.Count.Should().Be(2);
            backEndServiceUnderTest.AvailableReservations.Count.Should().Be(0);

            backEndServiceUnderTest.SetInventory("1", 1);
            backEndServiceUnderTest.CompletePendingReservations("1");

            backEndServiceUnderTest.PendingReservations.Count.Should().Be(1);
            backEndServiceUnderTest.AvailableReservations.Count.Should().Be(1);
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
