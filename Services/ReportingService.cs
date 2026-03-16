using Inventory_OrderSyncManagementSystem.Data;
using Inventory_OrderSyncManagementSystem.Models;
using System.Collections.Generic;
using System.Linq;

namespace Inventory_OrderSyncManagementSystem.Services
{
    public class ReportingService
    {
        private readonly AppDbContext _context;

        public ReportingService(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<ProductDto> GetTopSellingProducts(int topN)
        {
            var topProducts = _context.OrderDetails
                .GroupBy(od => od.ProductID)
                .Select(g => new
                {
                    ProductID = g.Key,
                    Quantity = g.Sum(od => od.Quantity)
                })
                .OrderByDescending(p => p.Quantity)
                .Take(topN)
                .ToList();

            var result = topProducts.Select(tp => new ProductDto
            {
                ProductID = tp.ProductID,
                Name = _context.Products.FirstOrDefault(p => p.ProductID == tp.ProductID)?.Name ?? "Unknown",
                Description = _context.Products.FirstOrDefault(p => p.ProductID == tp.ProductID)?.Description ?? "",
                Quantity = tp.Quantity
            });

            return result;
        }

        public IEnumerable<InventoryDto> GetCurrentInventoryLevels()
        {
            return _context.InventoryTransactions
                .GroupBy(it => it.ProductID)
                .Select(g => new InventoryDto
                {
                    ProductID = g.Key,
                    Quantity = g.Sum(it => it.Quantity)
                })
                .ToList();
        }

        public IEnumerable<object> GetRevenueReport(string period)
        {
            if (period == "daily")
            {
                return _context.Orders
                    .GroupBy(o => o.OrderDate.Date)
                    .Select(g => new
                    {
                        Date = g.Key,
                        TotalRevenue = g.Sum(o => o.TotalAmount)
                    })
                    .OrderByDescending(r => r.Date)
                    .ToList();
            }
            else if (period == "monthly")
            {
                return _context.Orders
                    .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                    .Select(g => new
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        TotalRevenue = g.Sum(o => o.TotalAmount)
                    })
                    .OrderByDescending(r => r.Year).ThenByDescending(r => r.Month)
                    .ToList();
            }

            throw new ArgumentException("Invalid period. Use 'daily' or 'monthly'.");
        }

        public IEnumerable<object> GetProductTransactionHistory(int productId)
        {
            return _context.InventoryTransactions
                .Where(it => it.ProductID == productId)
                .Select(it => new
                {
                    it.TransactionID,
                    it.ProductID,
                    it.Quantity,
                    it.TransactionDate,
                    it.TransactionType
                })
                .OrderByDescending(it => it.TransactionDate)
                .ToList();
        }

        public IEnumerable<object> GetDelayedOrders()
        {
            return _context.Orders
                .Where(o => o.Status == "Pending" && o.OrderDate < DateTime.Now.AddDays(-7))
                .Select(o => new
                {
                    o.OrderID,
                    o.CustomerID,
                    o.OrderDate,
                    o.Status,
                    o.TotalAmount
                })
                .OrderBy(o => o.OrderDate)
                .ToList();
        }

        // Additional reporting methods will be added here.
    }
}