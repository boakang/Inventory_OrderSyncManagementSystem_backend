-- SQL Server database objects installed by EF Core migration:
--   20260317083009_AddUniqueCustomerEmailAndDbObjects
--
-- Contents:
--   - Views: dbo.vw_OrderSummary, dbo.vw_ProductInventory
--   - Function: dbo.fn_GetInventoryBalance
--   - Stored Procedures: dbo.AddCustomer, dbo.AddProduct, dbo.AdjustInventory
--
-- Notes:
--   - This file is provided for inspection/manual execution.
--   - The application uses EF Core migrations as the source of truth.
--   - If you run this manually in SSMS, ensure you're on the correct database.

-- USE InventoryOrderDB;
-- GO

SET NOCOUNT ON;

--------------------------------------------------------------------------------
-- Views
--------------------------------------------------------------------------------

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
GO

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
GO

--------------------------------------------------------------------------------
-- Function
--------------------------------------------------------------------------------

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
GO

--------------------------------------------------------------------------------
-- Stored Procedures
--------------------------------------------------------------------------------

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
GO

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
GO

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
        @ProductID,
        ISNULL(NULLIF(LTRIM(RTRIM(@TransactionType)), N''), N'Adjustment'),
        ABS(@Quantity),
        @Now
    );

    UPDATE dbo.Products
    SET StockQuantity = StockQuantity + @Delta,
        ModifiedDate = @Now,
        LastModified = @Now
    WHERE ProductID = @ProductID;
END;
GO
