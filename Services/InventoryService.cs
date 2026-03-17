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

        public InventoryService(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<InventoryDto> GetAllInventory()
        {
            return _context.InventoryTransactions
                .GroupBy(it => it.ProductID)
                .Select(g => new InventoryDto
                {
                    ProductID = g.Key,
                    Quantity = g.Sum(it => it.Quantity)
                }).ToList();
        }

        public InventoryDto? GetInventoryByProductId(int productId)
        {
            var totalQuantity = _context.InventoryTransactions
                .Where(it => it.ProductID == productId)
                .Sum(it => it.Quantity);

            return new InventoryDto
            {
                ProductID = productId,
                Quantity = totalQuantity
            };
        }

        public InventoryDto AddInventory(InventoryDto inventoryDto)
        {
            var transaction = new InventoryTransaction
            {
                ProductID = inventoryDto.ProductID,
                Quantity = inventoryDto.Quantity,
                TransactionDate = DateTime.Now,
                TransactionType = inventoryDto.TransactionType ?? "Adjustment"
            };

            _context.InventoryTransactions.Add(transaction);

            var product = _context.Products.Find(inventoryDto.ProductID);
            if (product != null)
            {
                product.StockQuantity += inventoryDto.Quantity;
                product.LastModified = DateTime.Now;
            }

            _context.SaveChanges();
            return inventoryDto;
        }

        public InventoryDto? UpdateInventory(int productId, InventoryDto inventoryDto)
        {
            // UpdateInventory usually means creating a new transaction to adjust the quantity
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
                Quantity = -quantity,
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