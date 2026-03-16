namespace Inventory_OrderSyncManagementSystem.Models
{
    public class ProductDto
    {
        public int ProductID { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int Quantity { get; set; }
    }
}