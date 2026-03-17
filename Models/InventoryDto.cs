namespace Inventory_OrderSyncManagementSystem.Models
{
    using System.ComponentModel.DataAnnotations;

    public class InventoryDto
    {
        public int ProductID { get; set; }

        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
        public string TransactionType { get; set; } = string.Empty; // e.g., "Stock In", "Stock Out"
        public DateTime TransactionDate { get; set; }
    }
}