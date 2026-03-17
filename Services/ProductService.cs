using Inventory_OrderSyncManagementSystem.Data;
using Inventory_OrderSyncManagementSystem.Models;
using System.Collections.Generic;
using System.Linq;

namespace Inventory_OrderSyncManagementSystem.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<ProductDto> GetAllProducts()
        {
            return _context.Products.Select(p => new ProductDto
            {
                ProductID = p.ProductID,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                StockQuantity = p.StockQuantity
            }).ToList();
        }

        public ProductDto GetProductById(int id)
        {
            var p = _context.Products.Find(id);
            if (p == null) return null!;

            return new ProductDto
            {
                ProductID = p.ProductID,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                StockQuantity = p.StockQuantity
            };
        }

        public ProductDto AddProduct(ProductDto productDto)
        {
            if (productDto.Price < 0)
            {
                throw new ArgumentException("Price must be >= 0.");
            }
            if (productDto.StockQuantity < 0)
            {
                throw new ArgumentException("StockQuantity must be >= 0.");
            }

            var product = new Product
            {
                Name = productDto.Name ?? string.Empty,
                Description = productDto.Description ?? string.Empty,
                Price = productDto.Price,
                StockQuantity = productDto.StockQuantity,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                LastModified = DateTime.Now
            };

            _context.Products.Add(product);
            _context.SaveChanges();

            productDto.ProductID = product.ProductID;
            return productDto;
        }

        public ProductDto UpdateProduct(int id, ProductDto productDto)
        {
            var existingProduct = _context.Products.Find(id);
            if (existingProduct == null)
            {
                throw new KeyNotFoundException("Product not found.");
            }

            if (productDto.Price < 0)
            {
                throw new ArgumentException("Price must be >= 0.");
            }
            if (productDto.StockQuantity < 0)
            {
                throw new ArgumentException("StockQuantity must be >= 0.");
            }

            existingProduct.Name = productDto.Name ?? existingProduct.Name;
            existingProduct.Description = productDto.Description ?? existingProduct.Description;
            existingProduct.Price = productDto.Price;
            existingProduct.StockQuantity = productDto.StockQuantity;
            existingProduct.ModifiedDate = DateTime.Now;
            existingProduct.LastModified = DateTime.Now;

            _context.SaveChanges();

            return productDto;
        }

        public bool DeleteProduct(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return false;
            }

            _context.Products.Remove(product);
            _context.SaveChanges();
            return true;
        }
    }
}
