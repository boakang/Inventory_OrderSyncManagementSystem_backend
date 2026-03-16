using Inventory_OrderSyncManagementSystem.Models;
using System.Collections.Generic;
using System.Linq;

// Moved to Services folder.

namespace Inventory_OrderSyncManagementSystem.Services
{
    public class ProductService : IProductService
    {
        private readonly List<ProductDto> _products = new();

        public IEnumerable<ProductDto> GetAllProducts()
        {
            return _products;
        }

        public ProductDto GetProductById(int id)
        {
            return _products.FirstOrDefault(p => p.ProductID == id)!;
        }

        public ProductDto AddProduct(ProductDto productDto)
        {
            productDto.ProductID = _products.Count + 1;
            _products.Add(productDto);
            return productDto;
        }

        public ProductDto UpdateProduct(int id, ProductDto productDto)
        {
            var existingProduct = GetProductById(id);
            if (existingProduct == null)
            {
                throw new KeyNotFoundException("Product not found.");
            }

            existingProduct.Name = productDto.Name;
            existingProduct.Description = productDto.Description;
            existingProduct.Price = productDto.Price;
            existingProduct.StockQuantity = productDto.StockQuantity;

            return existingProduct;
        }

        public bool DeleteProduct(int id)
        {
            var product = GetProductById(id);
            if (product == null)
            {
                return false;
            }

            _products.Remove(product);
            return true;
        }
    }
}