using Microsoft.AspNetCore.Mvc;
using Inventory_OrderSyncManagementSystem.Models;
using Inventory_OrderSyncManagementSystem.Services;
using Microsoft.EntityFrameworkCore;

namespace Inventory_OrderSyncManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public IActionResult GetAllProducts()
        {
            var products = _productService.GetAllProducts();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public IActionResult GetProductById(int id)
        {
            var product = _productService.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPost]
        public IActionResult AddProduct([FromBody] ProductDto productDto)
        {
            try
            {
                var product = _productService.AddProduct(productDto);
                return CreatedAtAction(nameof(GetProductById), new { id = product.ProductID }, product);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, [FromBody] ProductDto productDto)
        {
            try
            {
                var updatedProduct = _productService.UpdateProduct(id, productDto);
                if (updatedProduct == null)
                {
                    return NotFound();
                }
                return Ok(updatedProduct);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            try
            {
                var isDeleted = _productService.DeleteProduct(id);
                if (!isDeleted)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (DbUpdateException)
            {
                return BadRequest(new
                {
                    error = "Cannot delete product because there are related records (orders or inventory transactions)."
                });
            }
        }
    }
}

// Moved to Controllers folder.