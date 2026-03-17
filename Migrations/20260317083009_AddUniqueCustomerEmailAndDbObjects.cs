using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryOrderSyncManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueCustomerEmailAndDbObjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Safety checks: fail fast if existing data would violate the new constraint.
            // NOTE: SQL Server cannot index nvarchar(max), hence the column length change.
            migrationBuilder.Sql(@"
IF EXISTS (
    SELECT 1
    FROM dbo.Customers
    GROUP BY Email
    HAVING COUNT(*) > 1
)
    RAISERROR('Cannot apply unique constraint: duplicate values exist in Customers.Email. Fix duplicates and re-run migration.', 16, 1);

IF EXISTS (
    SELECT 1
    FROM dbo.Customers
    WHERE LEN(Email) > 256
)
    RAISERROR('Cannot apply unique constraint: some Customers.Email values exceed 256 characters. Shorten them and re-run migration.', 16, 1);
");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Customers",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Email",
                table: "Customers",
                column: "Email",
                unique: true);

            // Views
            migrationBuilder.Sql(@"
CREATE OR ALTER VIEW dbo.vw_OrderSummary
AS
SELECT
    o.OrderID,
    o.OrderDate,
    c.CustomerID,
    (c.FirstName + N' ' + c.LastName) AS CustomerName,
    o.TotalAmount,
    o.Status,
    o.LastModified
FROM dbo.Orders o
INNER JOIN dbo.Customers c
    ON o.CustomerID = c.CustomerID;
");

            migrationBuilder.Sql(@"
CREATE OR ALTER VIEW dbo.vw_ProductInventory
AS
SELECT
    p.ProductID,
    p.Name,
    p.Price,
    p.StockQuantity,
    p.CreatedDate,
    p.ModifiedDate,
    p.LastModified
FROM dbo.Products p;
");

            // Function
            migrationBuilder.Sql(@"
CREATE OR ALTER FUNCTION dbo.fn_GetInventoryBalance (@ProductID INT)
RETURNS INT
AS
BEGIN
    DECLARE @Balance INT;

    SELECT @Balance = ISNULL(SUM(
        CASE
            WHEN it.TransactionType IN (N'Stock Out', N'Issue') OR it.TransactionType LIKE N'Sales Order%'
                THEN -ABS(it.Quantity)
            ELSE ABS(it.Quantity)
        END
    ), 0)
    FROM dbo.InventoryTransactions it
    WHERE it.ProductID = @ProductID;

    RETURN @Balance;
END;
");

            // Stored procedures
            migrationBuilder.Sql(@"
CREATE OR ALTER PROCEDURE dbo.AddCustomer
    @FirstName NVARCHAR(100),
    @LastName NVARCHAR(100),
    @Email NVARCHAR(256),
    @Phone NVARCHAR(50),
    @Address NVARCHAR(400)
AS
BEGIN
    SET NOCOUNT ON;

    IF (@Email IS NULL OR LTRIM(RTRIM(@Email)) = N'')
    BEGIN
        RAISERROR('Email is required.', 16, 1);
        RETURN;
    END;

    IF EXISTS (SELECT 1 FROM dbo.Customers WHERE Email = @Email)
    BEGIN
        RAISERROR('Email already exists.', 16, 1);
        RETURN;
    END;

    DECLARE @Now DATETIME2 = SYSDATETIME();

    INSERT INTO dbo.Customers
    (
        FirstName, LastName, Email, Phone, Address,
        CreatedDate, ModifiedDate, LastModified
    )
    VALUES
    (
        @FirstName, @LastName, @Email, @Phone, @Address,
        @Now, @Now, @Now
    );
END;
");

            migrationBuilder.Sql(@"
CREATE OR ALTER PROCEDURE dbo.AddProduct
    @Name NVARCHAR(200),
    @Description NVARCHAR(MAX),
    @Price DECIMAL(18,2),
    @StockQuantity INT
AS
BEGIN
    SET NOCOUNT ON;

    IF (@Price < 0) OR (@StockQuantity < 0)
    BEGIN
        RAISERROR('Price and StockQuantity must be >= 0.', 16, 1);
        RETURN;
    END;

    DECLARE @Now DATETIME2 = SYSDATETIME();

    INSERT INTO dbo.Products
    (
        Name, Description, Price, StockQuantity,
        CreatedDate, ModifiedDate, LastModified
    )
    VALUES
    (
        @Name, ISNULL(@Description, N''), @Price, @StockQuantity,
        @Now, @Now, @Now
    );

    DECLARE @ProductID INT = SCOPE_IDENTITY();

    INSERT INTO dbo.InventoryTransactions
    (
        ProductID, TransactionType, Quantity, TransactionDate
    )
    VALUES
    (
        @ProductID, N'Stock In', ABS(@StockQuantity), @Now
    );
END;
");

            migrationBuilder.Sql(@"
CREATE OR ALTER PROCEDURE dbo.AdjustInventory
    @ProductID INT,
    @TransactionType NVARCHAR(100),
    @Quantity INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE ProductID = @ProductID)
    BEGIN
        RAISERROR('Product not found.', 16, 1);
        RETURN;
    END;

    IF (@Quantity IS NULL) OR (@Quantity < 0)
    BEGIN
        RAISERROR('Quantity must be >= 0.', 16, 1);
        RETURN;
    END;

    DECLARE @Now DATETIME2 = SYSDATETIME();
    DECLARE @Delta INT = CASE
        WHEN @TransactionType IN (N'Stock Out', N'Issue') OR @TransactionType LIKE N'Sales Order%'
            THEN -ABS(@Quantity)
        ELSE ABS(@Quantity)
    END;

    DECLARE @CurrentStock INT;
    SELECT @CurrentStock = StockQuantity FROM dbo.Products WHERE ProductID = @ProductID;

    IF (@CurrentStock + @Delta < 0)
    BEGIN
        RAISERROR('Insufficient stock. Stock cannot go below 0.', 16, 1);
        RETURN;
    END;

    INSERT INTO dbo.InventoryTransactions
    (
        ProductID, TransactionType, Quantity, TransactionDate
    )
    VALUES
    (
        @ProductID, ISNULL(NULLIF(LTRIM(RTRIM(@TransactionType)), N''), N'Adjustment'), ABS(@Quantity), @Now
    );

    UPDATE dbo.Products
    SET StockQuantity = StockQuantity + @Delta,
        ModifiedDate = @Now,
        LastModified = @Now
    WHERE ProductID = @ProductID;
END;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop DB objects (optional cleanup)
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS dbo.AdjustInventory;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS dbo.AddProduct;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS dbo.AddCustomer;");
            migrationBuilder.Sql("DROP VIEW IF EXISTS dbo.vw_ProductInventory;");
            migrationBuilder.Sql("DROP VIEW IF EXISTS dbo.vw_OrderSummary;");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS dbo.fn_GetInventoryBalance;");

            migrationBuilder.DropIndex(
                name: "IX_Customers_Email",
                table: "Customers");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);
        }
    }
}
