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

        public IEnumerable<InventoryDto> GetAllInventory()
        {
            return _context.InventoryTransactions
                .Select(it => new
                {
                    it.ProductID,
                    AbsQuantity = it.Quantity < 0 ? -it.Quantity : it.Quantity,
                    SignedQuantity = (it.TransactionType == "Stock Out"
                        || it.TransactionType == "Issue"
                        || it.TransactionType.StartsWith("Sales Order"))
                        ? -(it.Quantity < 0 ? -it.Quantity : it.Quantity)
                        : (it.Quantity < 0 ? -it.Quantity : it.Quantity)
                })
                .GroupBy(x => x.ProductID)
                .Select(g => new InventoryDto
                {
                    ProductID = g.Key,
                    Quantity = g.Sum(x => x.SignedQuantity)
                })
                .ToList();
        }

        public InventoryDto? GetInventoryByProductId(int productId)
        {
            var totalQuantity = _context.InventoryTransactions
                .Where(it => it.ProductID == productId)
                .Select(it => (it.TransactionType == "Stock Out"
                    || it.TransactionType == "Issue"
                    || it.TransactionType.StartsWith("Sales Order"))
                    ? -(it.Quantity < 0 ? -it.Quantity : it.Quantity)
                    : (it.Quantity < 0 ? -it.Quantity : it.Quantity))
                .Sum();

            return new InventoryDto
            {
                ProductID = productId,
                Quantity = totalQuantity
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
            // We don't usually delete inventory history, but for this mock-like service:
            var transactions = _context.InventoryTransactions.Where(it => it.ProductID == productId);
            _context.InventoryTransactions.RemoveRange(transactions);
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