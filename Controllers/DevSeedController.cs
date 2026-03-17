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

            // Categories (not currently linked to Product, but available via Categories API)
            await EnsureCategoryAsync("Electronics", "Devices and accessories", cancellationToken);
            await EnsureCategoryAsync("Office", "Office supplies", cancellationToken);

            // Suppliers
            var supplier = await EnsureSupplierAsync(
                name: "Default Supplier",
                contactInfo: "contact@supplier.local",
                now,
                cancellationToken);

            // Customers
            var customer1 = await EnsureCustomerAsync(
                email: "an@gmail.com",
                firstName: "Nguyen",
                lastName: "An",
                phone: "0901234567",
                address: "TP.HCM",
                now,
                cancellationToken);

            await EnsureCustomerAsync(
                email: "binh@gmail.com",
                firstName: "Tran",
                lastName: "Binh",
                phone: "0912345678",
                address: "Ha Noi",
                now,
                cancellationToken);

            // Products + seed stock via InventoryTransactions
            var macbook = await EnsureProductWithSeedStockAsync(
                name: "MacBook Pro 14",
                description: "M3 Pro, 16GB RAM, 512GB",
                price: 1999m,
                stockQuantity: 15,
                now,
                cancellationToken);

            var iphone = await EnsureProductWithSeedStockAsync(
                name: "iPhone 15 Pro",
                description: "Natural Titanium, 128GB",
                price: 999m,
                stockQuantity: 42,
                now,
                cancellationToken);

            var ipad = await EnsureProductWithSeedStockAsync(
                name: "iPad Air",
                description: "M1 Chip, 64GB, Space Gray",
                price: 599m,
                stockQuantity: 0,
                now,
                cancellationToken);

            await EnsureProductWithSeedStockAsync(
                name: "AirPods Pro 2",
                description: "USB-C Charging Case",
                price: 249m,
                stockQuantity: 85,
                now,
                cancellationToken);

            // Optional: create 1 sample order if none exists
            var hasAnyOrder = await _context.Orders.AnyAsync(cancellationToken);
            if (!hasAnyOrder)
            {
                var orderDetails = new List<OrderDetail>
                {
                    new()
                    {
                        ProductID = macbook.ProductID,
                        Quantity = 1,
                        UnitPrice = macbook.Price,
                        TotalPrice = macbook.Price
                    },
                    new()
                    {
                        ProductID = iphone.ProductID,
                        Quantity = 1,
                        UnitPrice = iphone.Price,
                        TotalPrice = iphone.Price
                    }
                };

                var total = orderDetails.Sum(d => d.TotalPrice);

                var order = new Order
                {
                    CustomerID = customer1.CustomerID,
                    OrderDate = now,
                    Status = "Pending",
                    TotalAmount = total,
                    LastModified = now,
                    OrderDetails = orderDetails
                };

                // Apply stock movements (mirror OrderService behavior)
                foreach (var detail in orderDetails)
                {
                    var product = await _context.Products.FirstAsync(p => p.ProductID == detail.ProductID, cancellationToken);
                    product.StockQuantity -= detail.Quantity;
                    product.ModifiedDate = now;
                    product.LastModified = now;

                    _context.InventoryTransactions.Add(new InventoryTransaction
                    {
                        ProductID = detail.ProductID,
                        Quantity = detail.Quantity,
                        TransactionDate = now,
                        TransactionType = "Sales Order (Seed)"
                    });
                }

                _context.Orders.Add(order);
            }

            await _context.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);

            var summary = new
            {
                categories = await _context.Categories.CountAsync(cancellationToken),
                suppliers = await _context.Suppliers.CountAsync(cancellationToken),
                customers = await _context.Customers.CountAsync(cancellationToken),
                products = await _context.Products.CountAsync(cancellationToken),
                orders = await _context.Orders.CountAsync(cancellationToken),
                inventoryTransactions = await _context.InventoryTransactions.CountAsync(cancellationToken)
            };

            return Ok(new { seeded = true, summary });
        }

        private async Task EnsureCategoryAsync(string name, string description, CancellationToken cancellationToken)
        {
            var exists = await _context.Categories.AnyAsync(c => c.Name == name, cancellationToken);
            if (exists) return;

            _context.Categories.Add(new Category { Name = name, Description = description });
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
            DateTime now,
            CancellationToken cancellationToken)
        {
            var existing = await _context.Products.FirstOrDefaultAsync(p => p.Name == name, cancellationToken);
            if (existing != null)
            {
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
