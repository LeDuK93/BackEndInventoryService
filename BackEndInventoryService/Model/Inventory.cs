namespace BackEndInventoryService.Model
{
    public class Inventory
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public bool IsAvailable { get; set; }

        public Inventory(
            string productId, 
            int quantity)
        {
            ProductId = productId;
            Quantity = quantity;
            IsAvailable = quantity > 0;
        }
    }
}