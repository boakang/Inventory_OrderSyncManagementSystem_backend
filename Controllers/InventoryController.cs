using Microsoft.AspNetCore.Mvc;
using Inventory_OrderSyncManagementSystem.Models;
using Inventory_OrderSyncManagementSystem.Services;

namespace Inventory_OrderSyncManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet]
        public IActionResult GetAllInventory()
        {
            var inventory = _inventoryService.GetAllInventoryView();
            return Ok(inventory);
        }

        [HttpGet("{productId}")]
        public IActionResult GetInventoryByProductId(int productId)
        {
            var inventoryItem = _inventoryService.GetInventoryViewByProductId(productId);
            if (inventoryItem == null)
            {
                return NotFound();
            }
            return Ok(inventoryItem);
        }

        [HttpPost]
        public IActionResult AddInventory([FromBody] InventoryDto inventoryDto)
        {
            try
            {
                var inventoryItem = _inventoryService.AddInventory(inventoryDto);
                return CreatedAtAction(nameof(GetInventoryByProductId), new { productId = inventoryItem.ProductID }, inventoryItem);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{productId}")]
        public IActionResult UpdateInventory(int productId, [FromBody] InventoryDto inventoryDto)
        {
            try
            {
                var updatedInventory = _inventoryService.UpdateInventory(productId, inventoryDto);
                if (updatedInventory == null)
                {
                    return NotFound();
                }
                return Ok(updatedInventory);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{productId}")]
        public IActionResult DeleteInventory(int productId)
        {
            var isDeleted = _inventoryService.DeleteInventory(productId);
            if (!isDeleted)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}