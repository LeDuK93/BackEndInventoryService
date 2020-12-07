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

        public Reservation CreateReservation(List<OrderLine> orders)
        {
            var reservation = new Reservation(DateTime.Now, orders);

            ReservationValidator.ValidateAndThrow(reservation);

            return reservation;
        }

        public List<Inventory> GetInventory(int cursor, int limit)
        {
            throw new NotImplementedException();
        }

        public List<Reservation> GetReservations(int cursor, int limit)
        {
            throw new NotImplementedException();
        }

        public void SetInventory(string productId, int quantity)
        {
            throw new NotImplementedException();
        }
    }
}
