using Inventory_OrderSyncManagementSystem.Models;
using System.Collections.Generic;
using System.Linq;

namespace Inventory_OrderSyncManagementSystem.Services
{
    public class OrderService : IOrderService
    {
        private readonly List<OrderDto> _orders = new();

        public IEnumerable<OrderDto> GetAllOrders()
        {
            return _orders;
        }

        public OrderDto? GetOrderById(int id)
        {
            return _orders.FirstOrDefault(o => o.OrderID == id);
        }

        public OrderDto CreateOrder(OrderDto orderDto)
        {
            orderDto.OrderID = _orders.Count + 1;
            _orders.Add(orderDto);
            return orderDto;
        }

        public OrderDto? UpdateOrder(int id, OrderDto orderDto)
        {
            var existingOrder = GetOrderById(id);
            if (existingOrder == null)
            {
                return null;
            }

            existingOrder.CustomerID = orderDto.CustomerID;
            existingOrder.OrderDate = orderDto.OrderDate;
            existingOrder.TotalAmount = orderDto.TotalAmount;
            existingOrder.Status = orderDto.Status;
            existingOrder.OrderDetails = orderDto.OrderDetails;

            return existingOrder;
        }

        public bool DeleteOrder(int id)
        {
            var order = GetOrderById(id);
            if (order == null)
            {
                return false;
            }

            _orders.Remove(order);
            return true;
        }
    }
}