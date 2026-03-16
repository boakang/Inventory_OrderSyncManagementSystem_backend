using Inventory_OrderSyncManagementSystem.Data;
using Inventory_OrderSyncManagementSystem.Models;
using System.Collections.Generic;
using System.Linq;

namespace Inventory_OrderSyncManagementSystem.Services
{
    public class SynchronizationService
    {
        private readonly AppDbContext _context;

        public SynchronizationService(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<object> GetUpdatedData(string lastModified)
        {
            var lastModifiedDate = DateTime.Parse(lastModified);

            var updatedProducts = _context.Products
                .Where(p => p.LastModified >= lastModifiedDate)
                .Select(p => new
                {
                    p.ProductID,
                    p.Name,
                    p.Description,
                    p.LastModified
                })
                .ToList();

            var updatedCustomers = _context.Customers
                .Where(c => c.LastModified >= lastModifiedDate)
                .Select(c => new
                {
                    c.CustomerID,
                    c.FirstName,
                    c.LastName,
                    c.Email,
                    c.LastModified
                })
                .ToList();

            var updatedOrders = _context.Orders
                .Where(o => o.LastModified >= lastModifiedDate)
                .Select(o => new
                {
                    o.OrderID,
                    o.CustomerID,
                    o.OrderDate,
                    o.Status,
                    o.LastModified
                })
                .ToList();

            return updatedProducts.Cast<object>()
                .Concat(updatedCustomers.Cast<object>())
                .Concat(updatedOrders.Cast<object>());
        }

        public void UploadChanges(IEnumerable<object> changes)
        {
            foreach (var change in changes)
            {
                // Example: Handle product updates
                if (change is Product updatedProduct)
                {
                    var existingProduct = _context.Products.FirstOrDefault(p => p.ProductID == updatedProduct.ProductID);
                    if (existingProduct != null)
                    {
                        existingProduct.Name = updatedProduct.Name;
                        existingProduct.Description = updatedProduct.Description;
                        existingProduct.LastModified = updatedProduct.LastModified;
                    }
                    else
                    {
                        _context.Products.Add(updatedProduct);
                    }
                }

                // Add similar logic for other entities (e.g., Customer, Order)
            }

            _context.SaveChanges();
        }

        // ...additional synchronization methods can be added here...
    }
}