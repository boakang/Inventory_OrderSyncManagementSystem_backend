namespace Inventory_OrderSyncManagementSystem.Models
{
    public class InventoryViewDto
    {
        public int ProductID { get; set; }
        public int Quantity { get; set; }

        public string ProductName { get; set; } = string.Empty;
        public string? CategoryName { get; set; }
        public string? SupplierName { get; set; }

        public int? CategoryID { get; set; }
        public int? SupplierID { get; set; }
    }
}
