USE InventoryOrderDB;
GO

-- Drop and recreate type safely
IF TYPE_ID(N'dbo.OrderDetailType') IS NOT NULL
    DROP TYPE dbo.OrderDetailType;
GO

CREATE TYPE dbo.OrderDetailType AS TABLE
(
    ProductID INT,
    Quantity INT,
    UnitPrice DECIMAL(18,2)
);
GO

CREATE OR ALTER PROCEDURE dbo.AddProduct
    @Name NVARCHAR(100),
    @Description NVARCHAR(MAX),
    @Price DECIMAL(18,2),
    @StockQuantity INT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Products
    (
        Name,
        Description,
        Price,
        StockQuantity,
        CreatedDate,
        ModifiedDate
    )
    VALUES
    (
        @Name,
        @Description,
        @Price,
        @StockQuantity,
        GETDATE(),
        GETDATE()
    );
END;
GO

CREATE OR ALTER PROCEDURE dbo.AddCustomer
    @FirstName NVARCHAR(50),
    @LastName NVARCHAR(50),
    @Email NVARCHAR(100),
    @Phone NVARCHAR(15),
    @Address NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Customers
    (
        FirstName,
        LastName,
        Email,
        Phone,
        Address,
        CreatedDate,
        ModifiedDate
    )
    VALUES
    (
        @FirstName,
        @LastName,
        @Email,
        @Phone,
        @Address,
        GETDATE(),
        GETDATE()
    );
END;
GO

CREATE OR ALTER PROCEDURE dbo.ImportStock
    @ProductID INT,
    @Quantity INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF NOT EXISTS (
            SELECT 1
            FROM dbo.Products
            WHERE ProductID = @ProductID
        )
        BEGIN
            THROW 50004, 'Product does not exist.', 1;
        END;

        IF @Quantity <= 0
        BEGIN
            THROW 50005, 'Quantity must be greater than 0.', 1;
        END;

        UPDATE dbo.Products
        SET StockQuantity = StockQuantity + @Quantity
        WHERE ProductID = @ProductID;

        INSERT INTO dbo.InventoryTransactions
        (
            ProductID,
            TransactionType,
            Quantity,
            TransactionDate
        )
        VALUES
        (
            @ProductID,
            'IN',
            @Quantity,
            GETDATE()
        );

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH
END;
GO

CREATE OR ALTER PROCEDURE dbo.CreateOrder
    @CustomerID INT,
    @OrderDate DATETIME,
    @OrderDetails dbo.OrderDetailType READONLY
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @OrderID INT;

        IF NOT EXISTS (
            SELECT 1
            FROM dbo.Customers
            WHERE CustomerID = @CustomerID
        )
        BEGIN
            THROW 50001, 'Customer does not exist.', 1;
        END;

        IF EXISTS (
            SELECT 1
            FROM @OrderDetails od
            LEFT JOIN dbo.Products p ON od.ProductID = p.ProductID
            WHERE p.ProductID IS NULL
        )
        BEGIN
            THROW 50002, 'One or more products do not exist.', 1;
        END;

        IF EXISTS (
            SELECT 1
            FROM @OrderDetails od
            INNER JOIN dbo.Products p ON od.ProductID = p.ProductID
            WHERE od.Quantity > p.StockQuantity
        )
        BEGIN
            THROW 50003, 'Insufficient stock for one or more products.', 1;
        END;

        INSERT INTO dbo.Orders
        (
            CustomerID,
            OrderDate,
            TotalAmount,
            Status
        )
        VALUES
        (
            @CustomerID,
            @OrderDate,
            0,
            'Pending'
        );

        SET @OrderID = SCOPE_IDENTITY();

        INSERT INTO dbo.OrderDetails
        (
            OrderID,
            ProductID,
            Quantity,
            UnitPrice
        )
        SELECT
            @OrderID,
            ProductID,
            Quantity,
            UnitPrice
        FROM @OrderDetails;

        UPDATE dbo.Orders
        SET TotalAmount =
        (
            SELECT ISNULL(SUM(TotalPrice), 0)
            FROM dbo.OrderDetails
            WHERE OrderID = @OrderID
        )
        WHERE OrderID = @OrderID;

        UPDATE p
        SET p.StockQuantity = p.StockQuantity - od.Quantity
        FROM dbo.Products p
        INNER JOIN @OrderDetails od
            ON p.ProductID = od.ProductID;

        INSERT INTO dbo.InventoryTransactions
        (
            ProductID,
            TransactionType,
            Quantity,
            TransactionDate
        )
        SELECT
            ProductID,
            'OUT',
            Quantity,
            GETDATE()
        FROM @OrderDetails;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH
END;
GO