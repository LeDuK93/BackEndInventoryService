using BackEndInventoryService.Model;
using BackEndInventoryService.Validator;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BackEndInventoryService.Service
{
    public class BackEndService : IBackEndService
    {
        private IValidator<Reservation> ReservationValidator = new ReservationValidator();

        public Dictionary<string, Inventory> Products = new Dictionary<string, Inventory>();

        public List<Reservation> AvailableReservations = new List<Reservation>();
        public List<Reservation> PendingReservations = new List<Reservation>();

        public Reservation CreateReservation(List<OrderLine> orders)
        {
            var reservation = new Reservation(DateTime.Now, orders);

            ReservationValidator.ValidateAndThrow(reservation);

            foreach (OrderLine order in orders)
            {
                if (Products.TryGetValue(order.ProductId, out Inventory inventory))
                {
                    reservation.IsAvailable &= Products[order.ProductId].Availability && inventory.Quantity >= order.Quantity;

                    if (reservation.IsAvailable)
                    {
                        SetInventory(order.ProductId, inventory.Quantity - order.Quantity);
                    }
                    else
                    {
                        Products[order.ProductId].Availability = false;
                    }
                }
                else
                {
                    throw new ArgumentException($"ProductId [{order.ProductId}] does not exist");
                }
            }

            AddReservation(reservation);

            return reservation;
        }

        public List<Inventory> GetInventory(int cursor, int limit)
        {
            return Products.Values.Skip(cursor).Take(limit).ToList();
        }

        public List<Reservation> GetReservations(int cursor, int limit)
        {
            return AvailableReservations.OrderBy(r => r.ReservationId).Skip(cursor).Take(limit).ToList();
        }

        public void SetInventory(string productId, int quantity)
        {
            Inventory inventory = Products[productId];

            inventory.Quantity = quantity;
            inventory.Availability = inventory.Quantity > 0;
        }

        private void AddReservation(Reservation reservation)
        {
            if (reservation.IsAvailable)
            {
                AvailableReservations.Add(reservation);
            }
            else
            {
                PendingReservations.Add(reservation);
            }
        }
    }
}
