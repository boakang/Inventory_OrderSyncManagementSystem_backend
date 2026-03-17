using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryOrderSyncManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddForeignKeysOrdersProductsInventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Clean up legacy/orphan data before adding FK constraints.
            // This makes the migration safer to apply on DBs that were created
            // before FK constraints existed.
            migrationBuilder.Sql(@"
DELETE od
FROM dbo.OrderDetails od
LEFT JOIN dbo.Products p ON p.ProductID = od.ProductID
WHERE p.ProductID IS NULL;

DELETE it
FROM dbo.InventoryTransactions it
LEFT JOIN dbo.Products p ON p.ProductID = it.ProductID
WHERE p.ProductID IS NULL;

DELETE o
FROM dbo.Orders o
LEFT JOIN dbo.Customers c ON c.CustomerID = o.CustomerID
WHERE c.CustomerID IS NULL;
");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerID",
                table: "Orders",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_ProductID",
                table: "OrderDetails",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactions_ProductID",
                table: "InventoryTransactions",
                column: "ProductID");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryTransactions_Products_ProductID",
                table: "InventoryTransactions",
                column: "ProductID",
                principalTable: "Products",
                principalColumn: "ProductID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Products_ProductID",
                table: "OrderDetails",
                column: "ProductID",
                principalTable: "Products",
                principalColumn: "ProductID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Customers_CustomerID",
                table: "Orders",
                column: "CustomerID",
                principalTable: "Customers",
                principalColumn: "CustomerID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryTransactions_Products_ProductID",
                table: "InventoryTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Products_ProductID",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Customers_CustomerID",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_CustomerID",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_ProductID",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_InventoryTransactions_ProductID",
                table: "InventoryTransactions");
        }
    }
}
