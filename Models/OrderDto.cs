namespace Inventory_OrderSyncManagementSystem.Models
{
    public class OrderDto
    {
        public int OrderID { get; set; }
        public int CustomerID { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Status { get; set; }
        public List<OrderDetailDto> OrderDetails { get; set; } = new();
    }
}