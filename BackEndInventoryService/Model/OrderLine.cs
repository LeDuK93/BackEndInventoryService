namespace BackEndInventoryService.Model
{
    public class OrderLine
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }

        public OrderLine(
            string productId, 
            int quantity)
        {
            ProductId = productId;
            Quantity = quantity;
        }
    }
}