namespace Inventory_OrderSyncManagementSystem.Models
{
    public class OrderDetailDto
    {
        public int OrderDetailID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
