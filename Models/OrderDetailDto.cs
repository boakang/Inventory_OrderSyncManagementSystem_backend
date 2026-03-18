namespace Inventory_OrderSyncManagementSystem.Models
{
    using System.ComponentModel.DataAnnotations;

    public class OrderDetailDto
    {
        public int OrderDetailID { get; set; }
        public int ProductID { get; set; }

        public string? ProductName { get; set; }

        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public decimal TotalPrice { get; set; }
    }
}
