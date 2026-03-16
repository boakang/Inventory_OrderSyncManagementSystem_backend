using Inventory_OrderSyncManagementSystem.Models;
using System.Collections.Generic;

namespace Inventory_OrderSyncManagementSystem.Services
{
    public interface IInventoryService
    {
        IEnumerable<InventoryDto> GetAllInventory();
        InventoryDto? GetInventoryByProductId(int productId);
        InventoryDto AddInventory(InventoryDto inventoryDto);
        InventoryDto? UpdateInventory(int productId, InventoryDto inventoryDto);
        bool DeleteInventory(int productId);
    }
}