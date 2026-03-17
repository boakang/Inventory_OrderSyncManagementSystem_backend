using Inventory_OrderSyncManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_OrderSyncManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly ReportingService _reportingService;

        public ReportsController(ReportingService reportingService)
        {
            _reportingService = reportingService;
        }

        [HttpGet("top-selling")]
        public IActionResult GetTopSelling([FromQuery] int topN = 5)
        {
            if (topN <= 0) topN = 5;

            var data = _reportingService
                .GetTopSellingProducts(topN)
                .Select(p => new
                {
                    productID = p.ProductID,
                    name = p.Name ?? "Unknown",
                    sales = p.Quantity
                })
                .ToList();

            return Ok(data);
        }

        [HttpGet("inventory")]
        public IActionResult GetInventoryLevels()
        {
            return Ok(_reportingService.GetCurrentInventoryLevels());
        }

        [HttpGet("revenue")]
        public IActionResult GetRevenue([FromQuery] string period = "monthly")
        {
            period = (period ?? "monthly").Trim().ToLowerInvariant();
            if (period != "daily" && period != "monthly")
            {
                return BadRequest(new { error = "Invalid period. Use 'daily' or 'monthly'." });
            }

            return Ok(_reportingService.GetRevenuePoints(period));
        }
    }
}
