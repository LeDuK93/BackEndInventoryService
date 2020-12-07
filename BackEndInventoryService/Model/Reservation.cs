using System;
using System.Collections.Generic;

namespace BackEndInventoryService.Model
{
    public class Reservation
    {
        private static int _globalCount;
        public int ReservationId { get; set; }
        public DateTime CreationDate { get; set; }
        public List<OrderLine> OrdersLines { get; set; }
        public bool IsAvailable { get; set; }

        public Reservation(
            DateTime creationDate,
            List<OrderLine> orderLines)
        {
            ReservationId = ++_globalCount;
            CreationDate = creationDate;
            OrdersLines = orderLines;
            IsAvailable = true;
        }
    }
}