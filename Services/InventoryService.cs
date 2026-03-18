using Inventory_OrderSyncManagementSystem.Data;
using Inventory_OrderSyncManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Inventory_OrderSyncManagementSystem.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly AppDbContext _context;

        private static bool IsOutboundTransactionType(string? transactionType)
        {
            if (string.IsNullOrWhiteSpace(transactionType)) return false;
            var t = transactionType.Trim();

            return t.Equals("Stock Out", StringComparison.OrdinalIgnoreCase)
                || t.Equals("Issue", StringComparison.OrdinalIgnoreCase)
                || t.StartsWith("Sales Order", StringComparison.OrdinalIgnoreCase);
        }

        public InventoryService(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<InventoryViewDto> GetAllInventoryView()
        {
            // Projection join via navigation properties (left joins) to provide
            // a UI-friendly inventory view in a single call.
            return _context.Products
                .Select(p => new InventoryViewDto
                {
                    ProductID = p.ProductID,
                    Quantity = p.StockQuantity,
                    ProductName = p.Name,
                    CategoryID = p.CategoryID,
                    SupplierID = p.SupplierID,
                    CategoryName = p.Category != null ? p.Category.Name : null,
                    SupplierName = p.Supplier != null ? p.Supplier.Name : null,
                })
                .ToList();
        }

        public InventoryViewDto? GetInventoryViewByProductId(int productId)
        {
            return _context.Products
                .Where(p => p.ProductID == productId)
                .Select(p => new InventoryViewDto
                {
                    ProductID = p.ProductID,
                    Quantity = p.StockQuantity,
                    ProductName = p.Name,
                    CategoryID = p.CategoryID,
                    SupplierID = p.SupplierID,
                    CategoryName = p.Category != null ? p.Category.Name : null,
                    SupplierName = p.Supplier != null ? p.Supplier.Name : null,
                })
                .FirstOrDefault();
        }

        public IEnumerable<InventoryDto> GetAllInventory()
        {
            // Inventory levels should reflect the current on-hand stock.
            // StockQuantity on Product is the source of truth and includes products with no transactions yet.
            return _context.Products
                .Select(p => new InventoryDto
                {
                    ProductID = p.ProductID,
                    Quantity = p.StockQuantity
                })
                .ToList();
        }

        public InventoryDto? GetInventoryByProductId(int productId)
        {
            var product = _context.Products.Find(productId);
            if (product == null) return null;

            return new InventoryDto
            {
                ProductID = productId,
                Quantity = product.StockQuantity
            };
        }

        public InventoryDto AddInventory(InventoryDto inventoryDto)
        {
            if (inventoryDto.Quantity < 0)
            {
                throw new ArgumentException("Quantity must be >= 0.");
            }

            var transactionType = string.IsNullOrWhiteSpace(inventoryDto.TransactionType)
                ? "Adjustment"
                : inventoryDto.TransactionType.Trim();

            var absQuantity = Math.Abs(inventoryDto.Quantity);
            var isOutbound = IsOutboundTransactionType(transactionType);
            var delta = isOutbound ? -absQuantity : absQuantity;

            var transaction = new InventoryTransaction
            {
                ProductID = inventoryDto.ProductID,
                Quantity = absQuantity,
                TransactionDate = DateTime.Now,
                TransactionType = transactionType
            };

            _context.InventoryTransactions.Add(transaction);

            var product = _context.Products.Find(inventoryDto.ProductID);
            if (product == null)
            {
                throw new KeyNotFoundException($"Product {inventoryDto.ProductID} not found.");
            }

            if (product.StockQuantity + delta < 0)
            {
                throw new InvalidOperationException("Insufficient stock. Stock cannot go below 0.");
            }

            product.StockQuantity += delta;
            product.LastModified = DateTime.Now;

            _context.SaveChanges();
            return inventoryDto;
        }

        public InventoryDto? UpdateInventory(int productId, InventoryDto inventoryDto)
        {
            // UpdateInventory usually means creating a new transaction to adjust the quantity
            inventoryDto.ProductID = productId;
            return AddInventory(inventoryDto);
        }

        public bool DeleteInventory(int productId)
        {
            // Clear inventory history and reset on-hand stock for the product.
            var product = _context.Products.Find(productId);
            if (product == null) return false;

            var transactions = _context.InventoryTransactions.Where(it => it.ProductID == productId);
            _context.InventoryTransactions.RemoveRange(transactions);
            product.StockQuantity = 0;
            product.LastModified = DateTime.Now;
            _context.SaveChanges();
            return true;
        }

        public bool IssueInventory(int productId, int quantity)
        {
            var product = _context.Products.Find(productId);
            if (product == null || product.StockQuantity < quantity)
            {
                return false;
            }

            var transaction = new InventoryTransaction
            {
                ProductID = productId,
                Quantity = quantity,
                TransactionDate = DateTime.Now,
                TransactionType = "Issue"
            };

            _context.InventoryTransactions.Add(transaction);
            product.StockQuantity -= quantity;
            product.LastModified = DateTime.Now;

            _context.SaveChanges();
            return true;
        }
    }
}