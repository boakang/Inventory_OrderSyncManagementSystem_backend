namespace Inventory_OrderSyncManagementSystem.Models
{
    using System.ComponentModel.DataAnnotations;

    public class ProductDto
    {
        public int ProductID { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }
        public int Quantity { get; set; }
    }
}