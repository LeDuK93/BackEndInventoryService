using BackEndInventoryService.Model;
using System.Collections.Generic;

namespace BackEndInventoryService
{
    public interface IBackEndService
    {
        Reservation CreateReservation(List<OrderLine> order);
        List<Reservation> GetAvailableReservations(int cursor, int limit);
        void SetInventory(string productId, int quantity);
        List<Inventory> GetInventory(int cursor, int limit);
    }
}
