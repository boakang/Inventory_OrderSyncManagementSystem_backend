using Inventory_OrderSyncManagementSystem.Data;
using Inventory_OrderSyncManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory_OrderSyncManagementSystem.Controllers
{
    [ApiController]
    [Route("api/dev/seed")]
    public class DevSeedController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public DevSeedController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [HttpPost]
        public async Task<IActionResult> Seed(CancellationToken cancellationToken)
        {
            if (!_environment.IsDevelopment())
            {
                return NotFound();
            }

            // Idempotent seed: create only missing records.
            var now = DateTime.Now;

            await using var tx = await _context.Database.BeginTransactionAsync(cancellationToken);

            // Categories (requested)
            await EnsureCategoryAsync("Đồ gia dụng & Nhà bếp", string.Empty, cancellationToken);
            await EnsureCategoryAsync("Vật dụng vệ sinh & Chăm sóc nhà cửa", string.Empty, cancellationToken);
            await EnsureCategoryAsync("Hàng tiêu dùng nhanh (FMCG)", string.Empty, cancellationToken);
            var defaultCategory = await EnsureCategoryAsync("Thiết bị điện gia dụng nhỏ", string.Empty, cancellationToken);
            await EnsureCategoryAsync("Đồ dùng gia đình khác", string.Empty, cancellationToken);

            // Suppliers
            var defaultSupplier = await EnsureSupplierAsync(
                name: "Central Retail",
                contactInfo: string.Empty,
                now,
                cancellationToken);

            await EnsureSupplierAsync("Saigon Co.op", string.Empty, now, cancellationToken);
            await EnsureSupplierAsync("WinCommerce", string.Empty, now, cancellationToken);
            await EnsureSupplierAsync("AEON", string.Empty, now, cancellationToken);
            await EnsureSupplierAsync("Lock&Lock", string.Empty, now, cancellationToken);
            await EnsureSupplierAsync("Sunhouse", string.Empty, now, cancellationToken);
            await EnsureSupplierAsync("SDHome", string.Empty, now, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);

            var summary = new
            {
                categories = await _context.Categories.CountAsync(cancellationToken),
                suppliers = await _context.Suppliers.CountAsync(cancellationToken)
            };

            return Ok(new { seeded = true, summary });
        }

        private async Task<Category> EnsureCategoryAsync(string name, string description, CancellationToken cancellationToken)
        {
            var existing = await _context.Categories.FirstOrDefaultAsync(c => c.Name == name, cancellationToken);
            if (existing != null) return existing;

            var category = new Category { Name = name, Description = description };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync(cancellationToken);
            return category;
        }

        private async Task<Supplier> EnsureSupplierAsync(string name, string contactInfo, DateTime now, CancellationToken cancellationToken)
        {
            var existing = await _context.Suppliers.FirstOrDefaultAsync(s => s.Name == name, cancellationToken);
            if (existing != null) return existing;

            var supplier = new Supplier
            {
                Name = name,
                ContactInfo = contactInfo,
                CreatedDate = now,
                ModifiedDate = now,
                LastModified = now
            };

            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync(cancellationToken);
            return supplier;
        }

        private async Task<Customer> EnsureCustomerAsync(
            string email,
            string firstName,
            string lastName,
            string phone,
            string address,
            DateTime now,
            CancellationToken cancellationToken)
        {
            var existing = await _context.Customers.FirstOrDefaultAsync(c => c.Email == email, cancellationToken);
            if (existing != null) return existing;

            var customer = new Customer
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                Phone = phone,
                Address = address,
                CreatedDate = now,
                ModifiedDate = now,
                LastModified = now
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync(cancellationToken);
            return customer;
        }

        private async Task<Product> EnsureProductWithSeedStockAsync(
            string name,
            string description,
            decimal price,
            int stockQuantity,
            int? categoryId,
            int? supplierId,
            DateTime now,
            CancellationToken cancellationToken)
        {
            var existing = await _context.Products.FirstOrDefaultAsync(p => p.Name == name, cancellationToken);
            if (existing != null)
            {
                if (categoryId != null && existing.CategoryID != categoryId)
                {
                    existing.CategoryID = categoryId;
                }

                if (supplierId != null && existing.SupplierID != supplierId)
                {
                    existing.SupplierID = supplierId;
                }

                // Ensure at least one seed transaction exists for this product.
                var hasSeedTx = await _context.InventoryTransactions.AnyAsync(
                    it => it.ProductID == existing.ProductID && it.TransactionType == "Seed Stock",
                    cancellationToken);

                if (!hasSeedTx)
                {
                    _context.InventoryTransactions.Add(new InventoryTransaction
                    {
                        ProductID = existing.ProductID,
                        Quantity = existing.StockQuantity,
                        TransactionDate = now,
                        TransactionType = "Seed Stock"
                    });
                }

                return existing;
            }

            var product = new Product
            {
                Name = name,
                Description = description,
                Price = price,
                StockQuantity = stockQuantity,
                CategoryID = categoryId,
                SupplierID = supplierId,
                CreatedDate = now,
                ModifiedDate = now,
                LastModified = now
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync(cancellationToken);

            _context.InventoryTransactions.Add(new InventoryTransaction
            {
                ProductID = product.ProductID,
                Quantity = stockQuantity,
                TransactionDate = now,
                TransactionType = "Seed Stock"
            });

            return product;
        }
    }
}
