using System;

using System.Collections.Generic;

namespace Inventory_OrderSyncManagementSystem.Models
{
    public class Supplier
    {
        public int SupplierID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ContactInfo { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;
        public DateTime LastModified { get; set; } = DateTime.Now;

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
