using Inventory_OrderSyncManagementSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory_OrderSyncManagementSystem.Controllers
{
    [ApiController]
    [Route("api/dev/reset")]
    public class DevResetController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public DevResetController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        /// <summary>
        /// Development-only: clears transactional/business data while keeping database schema.
        /// By default, preserves reference data (Categories and Suppliers).
        /// </summary>
        /// <param name="reseedIdentities">If true, reseeds SQL Server IDENTITY columns back to 1.</param>
        /// <param name="preserveReferenceData">If true, keeps Categories and Suppliers.</param>
        [HttpPost]
        public async Task<IActionResult> Reset(
            [FromQuery] bool reseedIdentities = true,
            [FromQuery] bool preserveReferenceData = true,
            CancellationToken cancellationToken = default)
        {
            if (!_environment.IsDevelopment())
            {
                return NotFound();
            }

            await using var tx = await _context.Database.BeginTransactionAsync(cancellationToken);

            // Delete in FK-safe order.
            var affected = new Dictionary<string, int>();

            affected["OrderDetails"] = await _context.Database.ExecuteSqlRawAsync("DELETE FROM [OrderDetails];", cancellationToken);
            affected["Orders"] = await _context.Database.ExecuteSqlRawAsync("DELETE FROM [Orders];", cancellationToken);
            affected["InventoryTransactions"] = await _context.Database.ExecuteSqlRawAsync("DELETE FROM [InventoryTransactions];", cancellationToken);
            affected["Products"] = await _context.Database.ExecuteSqlRawAsync("DELETE FROM [Products];", cancellationToken);
            affected["Customers"] = await _context.Database.ExecuteSqlRawAsync("DELETE FROM [Customers];", cancellationToken);

            if (preserveReferenceData)
            {
                affected["Suppliers"] = 0;
                affected["Categories"] = 0;
            }
            else
            {
                affected["Suppliers"] = await _context.Database.ExecuteSqlRawAsync("DELETE FROM [Suppliers];", cancellationToken);
                affected["Categories"] = await _context.Database.ExecuteSqlRawAsync("DELETE FROM [Categories];", cancellationToken);
            }

            if (reseedIdentities)
            {
                // Reseed to 0 so the next insert becomes 1.
                await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('[OrderDetails]', RESEED, 0) WITH NO_INFOMSGS;", cancellationToken);
                await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('[Orders]', RESEED, 0) WITH NO_INFOMSGS;", cancellationToken);
                await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('[InventoryTransactions]', RESEED, 0) WITH NO_INFOMSGS;", cancellationToken);
                await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('[Products]', RESEED, 0) WITH NO_INFOMSGS;", cancellationToken);
                await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('[Customers]', RESEED, 0) WITH NO_INFOMSGS;", cancellationToken);

                if (!preserveReferenceData)
                {
                    await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('[Suppliers]', RESEED, 0) WITH NO_INFOMSGS;", cancellationToken);
                    await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('[Categories]', RESEED, 0) WITH NO_INFOMSGS;", cancellationToken);
                }
            }

            await tx.CommitAsync(cancellationToken);

            var summary = new
            {
                deleted = affected,
                reseedIdentities,
                preserveReferenceData,
                counts = new
                {
                    categories = await _context.Categories.CountAsync(cancellationToken),
                    suppliers = await _context.Suppliers.CountAsync(cancellationToken),
                    customers = await _context.Customers.CountAsync(cancellationToken),
                    products = await _context.Products.CountAsync(cancellationToken),
                    orders = await _context.Orders.CountAsync(cancellationToken),
                    orderDetails = await _context.OrderDetails.CountAsync(cancellationToken),
                    inventoryTransactions = await _context.InventoryTransactions.CountAsync(cancellationToken)
                }
            };

            return Ok(new { reset = true, summary });
        }
    }
}
