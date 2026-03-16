using Inventory_OrderSyncManagementSystem.Models;
using System.Collections.Generic;

namespace Inventory_OrderSyncManagementSystem.Services
{
    public interface IOrderService
    {
        IEnumerable<OrderDto> GetAllOrders();
        OrderDto? GetOrderById(int id);
        OrderDto CreateOrder(OrderDto orderDto);
        OrderDto? UpdateOrder(int id, OrderDto orderDto);
        bool DeleteOrder(int id);
    }
}