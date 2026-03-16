namespace Inventory_OrderSyncManagementSystem.Models
{
    public class InventoryDto
    {
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public string TransactionType { get; set; } = string.Empty; // e.g., "Stock In", "Stock Out"
        public DateTime TransactionDate { get; set; }
    }
}