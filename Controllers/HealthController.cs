using Inventory_OrderSyncManagementSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory_OrderSyncManagementSystem.Controllers
{
    [ApiController]
    [Route("api/health")]
    public class HealthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public HealthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get() => Ok(new { status = "ok" });

        [HttpGet("db")]
        public async Task<IActionResult> GetDatabaseHealth(CancellationToken cancellationToken)
        {
            var connection = _context.Database.GetDbConnection();
            bool canConnect;
            bool canQuery;
            string? error = null;

            try
            {
                canConnect = await _context.Database.CanConnectAsync(cancellationToken);

                // Run a lightweight query to ensure EF can execute against the DB.
                await _context.Products.AsNoTracking()
                    .Select(p => p.ProductID)
                    .Take(1)
                    .ToListAsync(cancellationToken);
                canQuery = true;
            }
            catch (Exception ex)
            {
                canConnect = false;
                canQuery = false;
                error = ex.Message;
            }

            return Ok(new
            {
                canConnect,
                canQuery,
                dataSource = connection.DataSource,
                database = connection.Database,
                utcNow = DateTime.UtcNow,
                error
            });
        }
    }
}
