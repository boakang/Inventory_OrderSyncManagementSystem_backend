namespace Inventory_OrderSyncManagementSystem.Models
{
    using System.Collections.Generic;

    public class Product
    {
        public int ProductID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;
        public DateTime LastModified { get; set; }

        public int? CategoryID { get; set; }
        public virtual Category? Category { get; set; }

        public int? SupplierID { get; set; }
        public virtual Supplier? Supplier { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        public virtual ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();
    }
}