using Inventory_OrderSyncManagementSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory_OrderSyncManagementSystem.Controllers
{
    [ApiController]
    [Route("api/dev/maintenance")]
    public class DevMaintenanceController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public DevMaintenanceController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        /// <summary>
        /// Development-only: one-time fix to backfill OrderDetails.TotalPrice for existing rows.
        /// </summary>
        /// <param name="dryRun">If true, only returns how many rows would be updated.</param>
        [HttpPost("recalc-orderdetail-totalprice")]
        public async Task<IActionResult> RecalcOrderDetailTotalPrice(
            [FromQuery] bool dryRun = true,
            CancellationToken cancellationToken = default)
        {
            if (!_environment.IsDevelopment())
            {
                return NotFound();
            }

            // Safer filter: avoid touching rows that are legitimately 0.
            var matchCount = await _context.OrderDetails
                .Where(od => od.TotalPrice == 0 && od.Quantity > 0 && od.UnitPrice > 0)
                .CountAsync(cancellationToken);

            if (dryRun)
            {
                return Ok(new
                {
                    dryRun = true,
                    matchedRows = matchCount,
                    message = "No changes applied. Set dryRun=false to execute the update."
                });
            }

            await using var tx = await _context.Database.BeginTransactionAsync(cancellationToken);

            var updated = await _context.Database.ExecuteSqlRawAsync(
                "UPDATE [OrderDetails] SET [TotalPrice] = [Quantity] * [UnitPrice] WHERE [TotalPrice] = 0 AND [Quantity] > 0 AND [UnitPrice] > 0;",
                cancellationToken);

            await tx.CommitAsync(cancellationToken);

            return Ok(new
            {
                dryRun = false,
                matchedRows = matchCount,
                updatedRows = updated
            });
        }
    }
}
