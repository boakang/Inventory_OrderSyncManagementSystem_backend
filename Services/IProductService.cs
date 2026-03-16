using Inventory_OrderSyncManagementSystem.Models;
using System.Collections.Generic;

namespace Inventory_OrderSyncManagementSystem.Services
{
    public interface IProductService
    {
        IEnumerable<ProductDto> GetAllProducts();
        ProductDto? GetProductById(int id);
        ProductDto AddProduct(ProductDto productDto);
        ProductDto? UpdateProduct(int id, ProductDto productDto);
        bool DeleteProduct(int id);
    }
}