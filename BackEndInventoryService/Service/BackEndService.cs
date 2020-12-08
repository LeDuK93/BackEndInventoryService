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
                    reservation.IsAvailable &= inventory.Quantity >= order.Quantity;

                    if (reservation.IsAvailable)
                    {
                        SetInventory(order.ProductId, inventory.Quantity - order.Quantity);
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

        public List<Reservation> GetAvailableReservations(int cursor, int limit)
        {
            return AvailableReservations.OrderBy(r => r.ReservationId).Skip(cursor).Take(limit).ToList();
        }

        public void SetInventory(string productId, int quantity)
        {
            Inventory inventory = Products[productId];

            inventory.Quantity = quantity;
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

        public void CompletePendingReservations(string productId)
        {
            var pendingReservationsToRemove = new List<Reservation>();

            foreach (var reservation in PendingReservations)
            {
                var order = reservation.OrdersLines.Where(o => o.ProductId == productId).SingleOrDefault();

                if (order != null && AreAllProductsAvailable(reservation.OrdersLines))
                {
                    pendingReservationsToRemove.Add(reservation);
                    AvailableReservations.Add(reservation);
                    SetInventory(productId, Products[productId].Quantity - order.Quantity);
                }
            }

            pendingReservationsToRemove.ForEach(r => PendingReservations.Remove(r));
        }

        private bool AreAllProductsAvailable(List<OrderLine> orders)
        {
            return orders.All(o => Products[o.ProductId].Quantity >= o.Quantity);
        }
    }
}
