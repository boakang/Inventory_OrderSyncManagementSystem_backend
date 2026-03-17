using Microsoft.AspNetCore.Mvc;
using Inventory_OrderSyncManagementSystem.Models;
using Inventory_OrderSyncManagementSystem.Services;

namespace Inventory_OrderSyncManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierService _supplierService;

        public SuppliersController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        [HttpGet]
        public IActionResult GetAllSuppliers()
        {
            var suppliers = _supplierService.GetAllSuppliers();
            return Ok(suppliers);
        }

        [HttpGet("{id}")]
        public IActionResult GetSupplierById(int id)
        {
            var supplier = _supplierService.GetSupplierById(id);
            if (supplier == null)
            {
                return NotFound();
            }
            return Ok(supplier);
        }

        [HttpPost]
        public IActionResult AddSupplier([FromBody] SupplierDto supplierDto)
        {
            var supplier = _supplierService.AddSupplier(supplierDto);
            return CreatedAtAction(nameof(GetSupplierById), new { id = supplier.SupplierID }, supplier);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateSupplier(int id, [FromBody] SupplierDto supplierDto)
        {
            try
            {
                var updatedSupplier = _supplierService.UpdateSupplier(id, supplierDto);
                return Ok(updatedSupplier);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteSupplier(int id)
        {
            var isDeleted = _supplierService.DeleteSupplier(id);
            if (!isDeleted)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
