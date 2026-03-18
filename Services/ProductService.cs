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
                StockQuantity = p.StockQuantity,
                CategoryID = p.CategoryID,
                SupplierID = p.SupplierID
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
                StockQuantity = p.StockQuantity,
                CategoryID = p.CategoryID,
                SupplierID = p.SupplierID
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

            if (productDto.CategoryID != null)
            {
                var categoryExists = _context.Categories.Any(c => c.CategoryID == productDto.CategoryID);
                if (!categoryExists)
                {
                    throw new ArgumentException($"CategoryID {productDto.CategoryID} does not exist.");
                }
            }

            if (productDto.SupplierID != null)
            {
                var supplierExists = _context.Suppliers.Any(s => s.SupplierID == productDto.SupplierID);
                if (!supplierExists)
                {
                    throw new ArgumentException($"SupplierID {productDto.SupplierID} does not exist.");
                }
            }

            var product = new Product
            {
                Name = productDto.Name ?? string.Empty,
                Description = productDto.Description ?? string.Empty,
                Price = productDto.Price,
                StockQuantity = productDto.StockQuantity,
                CategoryID = productDto.CategoryID,
                SupplierID = productDto.SupplierID,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                LastModified = DateTime.Now
            };

            _context.Products.Add(product);
            _context.SaveChanges();

            // Log initial stock as an inventory transaction for audit/reporting.
            // Do not modify StockQuantity here (it's already set on the Product).
            if (product.StockQuantity > 0)
            {
                _context.InventoryTransactions.Add(new InventoryTransaction
                {
                    ProductID = product.ProductID,
                    Quantity = product.StockQuantity,
                    TransactionDate = DateTime.Now,
                    TransactionType = "Initial Stock"
                });
                _context.SaveChanges();
            }

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

            if (productDto.CategoryID != null)
            {
                var categoryExists = _context.Categories.Any(c => c.CategoryID == productDto.CategoryID);
                if (!categoryExists)
                {
                    throw new ArgumentException($"CategoryID {productDto.CategoryID} does not exist.");
                }
            }

            if (productDto.SupplierID != null)
            {
                var supplierExists = _context.Suppliers.Any(s => s.SupplierID == productDto.SupplierID);
                if (!supplierExists)
                {
                    throw new ArgumentException($"SupplierID {productDto.SupplierID} does not exist.");
                }
            }

            existingProduct.Name = productDto.Name ?? existingProduct.Name;
            existingProduct.Description = productDto.Description ?? existingProduct.Description;
            existingProduct.Price = productDto.Price;
            existingProduct.StockQuantity = productDto.StockQuantity;
            existingProduct.CategoryID = productDto.CategoryID;
            existingProduct.SupplierID = productDto.SupplierID;
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
