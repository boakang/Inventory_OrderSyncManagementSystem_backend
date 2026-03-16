using Inventory_OrderSyncManagementSystem.Models;
using System;
using System.Collections.Generic;

namespace Inventory_OrderSyncManagementSystem.Services
{
    public class TransactionService
    {
        private readonly IOrderService _orderService;
        private readonly IInventoryService _inventoryService;

        public TransactionService(IOrderService orderService, IInventoryService inventoryService)
        {
            _orderService = orderService;
            _inventoryService = inventoryService;
        }

        public bool ProcessOrderTransaction(OrderDto orderDto)
        {
            try
            {
                // Create the order
                var createdOrder = _orderService.CreateOrder(orderDto);

                // Update inventory for each product in the order
                foreach (var detail in orderDto.OrderDetails)
                {
                    var inventoryItem = _inventoryService.GetInventoryByProductId(detail.ProductID);

                    if (inventoryItem == null || inventoryItem.Quantity < detail.Quantity)
                    {
                        throw new Exception("Insufficient inventory for product ID: " + detail.ProductID);
                    }

                    inventoryItem.Quantity -= detail.Quantity;
                    inventoryItem.TransactionType = "Stock Out";
                    inventoryItem.TransactionDate = DateTime.Now;

                    _inventoryService.UpdateInventory(detail.ProductID, inventoryItem);
                }

                return true;
            }
            catch (Exception)
            {
                // Rollback logic can be implemented here if needed
                return false;
            }
        }
    }
}