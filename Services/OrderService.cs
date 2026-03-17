using Inventory_OrderSyncManagementSystem.Data;
using Inventory_OrderSyncManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Inventory_OrderSyncManagementSystem.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<OrderDto> GetAllOrders()
        {
            return _context.Orders
                .Include(o => o.OrderDetails)
                .Select(o => new OrderDto
                {
                    OrderID = o.OrderID,
                    CustomerID = o.CustomerID,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status ?? "Pending",
                    OrderDetails = o.OrderDetails.Select(od => new OrderDetailDto
                    {
                        OrderDetailID = od.OrderDetailID,
                        ProductID = od.ProductID,
                        Quantity = od.Quantity,
                        UnitPrice = od.UnitPrice
                    }).ToList()
                }).ToList();
        }

        public OrderDto? GetOrderById(int id)
        {
            var o = _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefault(o => o.OrderID == id);

            if (o == null) return null;

            return new OrderDto
            {
                OrderID = o.OrderID,
                CustomerID = o.CustomerID,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Status = o.Status ?? "Pending",
                OrderDetails = o.OrderDetails.Select(od => new OrderDetailDto
                {
                    OrderDetailID = od.OrderDetailID,
                    ProductID = od.ProductID,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice
                }).ToList()
            };
        }

        public OrderDto CreateOrder(OrderDto orderDto)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var order = new Order
                {
                    CustomerID = orderDto.CustomerID,
                    OrderDate = DateTime.Now,
                    Status = "Pending",
                    TotalAmount = orderDto.TotalAmount,
                    LastModified = DateTime.Now,
                    OrderDetails = orderDto.OrderDetails.Select(od => new OrderDetail
                    {
                        ProductID = od.ProductID,
                        Quantity = od.Quantity,
                        UnitPrice = od.UnitPrice
                    }).ToList()
                };

                // Deduct stock
                foreach (var detail in order.OrderDetails)
                {
                    var product = _context.Products.Find(detail.ProductID);
                    if (product == null || product.StockQuantity < detail.Quantity)
                    {
                        throw new InvalidOperationException($"Insufficient stock for product {detail.ProductID}");
                    }
                    product.StockQuantity -= detail.Quantity;
                    product.LastModified = DateTime.Now;

                    // Log transaction
                    _context.InventoryTransactions.Add(new InventoryTransaction
                    {
                        ProductID = detail.ProductID,
                        Quantity = detail.Quantity,
                        TransactionDate = DateTime.Now,
                        TransactionType = "Sales Order"
                    });
                }

                _context.Orders.Add(order);
                _context.SaveChanges();
                transaction.Commit();

                orderDto.OrderID = order.OrderID;
                return orderDto;
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        public OrderDto? UpdateOrder(int id, OrderDto orderDto)
        {
            var existingOrder = _context.Orders.Find(id);
            if (existingOrder == null) return null;

            existingOrder.Status = orderDto.Status ?? existingOrder.Status;
            existingOrder.LastModified = DateTime.Now;

            _context.SaveChanges();
            return orderDto;
        }

        public bool DeleteOrder(int id)
        {
            var order = _context.Orders.Find(id);
            if (order == null) return false;

            _context.Orders.Remove(order);
            _context.SaveChanges();
            return true;
        }
    }
}