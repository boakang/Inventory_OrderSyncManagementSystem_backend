using Microsoft.AspNetCore.Mvc;
using Inventory_OrderSyncManagementSystem.Models;
using Inventory_OrderSyncManagementSystem.Services;

namespace Inventory_OrderSyncManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public IActionResult GetAllOrders()
        {
            var orders = _orderService.GetAllOrders();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public IActionResult GetOrderById(int id)
        {
            var order = _orderService.GetOrderById(id);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }

        [HttpPost]
        public IActionResult CreateOrder([FromBody] OrderDto orderDto)
        {
            try
            {
                var order = _orderService.CreateOrder(orderDto);
                return CreatedAtAction(nameof(GetOrderById), new { id = order.OrderID }, order);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateOrder(int id, [FromBody] OrderDto orderDto)
        {
            var updatedOrder = _orderService.UpdateOrder(id, orderDto);
            if (updatedOrder == null)
            {
                return NotFound();
            }
            return Ok(updatedOrder);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteOrder(int id)
        {
            var isDeleted = _orderService.DeleteOrder(id);
            if (!isDeleted)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}