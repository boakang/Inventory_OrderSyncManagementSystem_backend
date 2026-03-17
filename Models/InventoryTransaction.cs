using System.ComponentModel.DataAnnotations;

namespace Inventory_OrderSyncManagementSystem.Models
{
    public class InventoryTransaction
    {
        [Key]
        public int TransactionID { get; set; }
        public int ProductID { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.Now;

        public virtual Product? Product { get; set; }
    }
}