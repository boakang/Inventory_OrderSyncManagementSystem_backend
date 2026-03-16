using Inventory_OrderSyncManagementSystem.Models;
using System.Collections.Generic;
using System.Linq;

namespace Inventory_OrderSyncManagementSystem.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly List<InventoryDto> _inventory = new();

        public IEnumerable<InventoryDto> GetAllInventory()
        {
            return _inventory;
        }

        public InventoryDto? GetInventoryByProductId(int productId)
        {
            return _inventory.FirstOrDefault(i => i.ProductID == productId);
        }

        public InventoryDto AddInventory(InventoryDto inventoryDto)
        {
            _inventory.Add(inventoryDto);
            return inventoryDto;
        }

        public InventoryDto? UpdateInventory(int productId, InventoryDto inventoryDto)
        {
            var existingInventory = GetInventoryByProductId(productId);
            if (existingInventory == null)
            {
                return null;
            }

            existingInventory.Quantity = inventoryDto.Quantity;
            existingInventory.TransactionType = inventoryDto.TransactionType;
            existingInventory.TransactionDate = inventoryDto.TransactionDate;

            return existingInventory;
        }

        public bool DeleteInventory(int productId)
        {
            var inventoryItem = GetInventoryByProductId(productId);
            if (inventoryItem == null)
            {
                return false;
            }

            _inventory.Remove(inventoryItem);
            return true;
        }

        public bool IssueInventory(int productId, int quantity)
        {
            var inventoryItem = _inventory.FirstOrDefault(i => i.ProductID == productId);
            if (inventoryItem == null || inventoryItem.Quantity < quantity)
            {
                // Prevent over-issuance
                return false;
            }

            inventoryItem.Quantity -= quantity;
            return true;
        }
    }
}