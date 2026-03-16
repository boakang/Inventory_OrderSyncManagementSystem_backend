USE InventoryOrderDB;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Customers WHERE Email = N'an@gmail.com')
BEGIN
    INSERT INTO dbo.Customers
    (
        FirstName,
        LastName,
        Email,
        Phone,
        Address
    )
    VALUES
    (
        N'Nguyen',
        N'An',
        N'an@gmail.com',
        N'0901234567',
        N'TP.HCM'
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Customers WHERE Email = N'binh@gmail.com')
BEGIN
    INSERT INTO dbo.Customers
    (
        FirstName,
        LastName,
        Email,
        Phone,
        Address
    )
    VALUES
    (
        N'Tran',
        N'Binh',
        N'binh@gmail.com',
        N'0912345678',
        N'Ha Noi'
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE Name = N'Laptop Dell')
BEGIN
    INSERT INTO dbo.Products
    (
        Name,
        Description,
        Price,
        StockQuantity
    )
    VALUES
    (
        N'Laptop Dell',
        N'Laptop business',
        20000000,
        15
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE Name = N'Chuột Logitech')
BEGIN
    INSERT INTO dbo.Products
    (
        Name,
        Description,
        Price,
        StockQuantity
    )
    VALUES
    (
        N'Chuột Logitech',
        N'Wireless mouse',
        500000,
        50
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE Name = N'Bàn phím cơ')
BEGIN
    INSERT INTO dbo.Products
    (
        Name,
        Description,
        Price,
        StockQuantity
    )
    VALUES
    (
        N'Bàn phím cơ',
        N'Mechanical keyboard',
        1200000,
        30
    );
END;
GO

--- ADD PRODUCT ---
EXEC dbo.AddProduct
    @Name = N'Màn hình LG',
    @Description = N'24 inch IPS',
    @Price = 3500000,
    @StockQuantity = 20;

--- ADD CUSTOMER ---
EXEC dbo.AddProduct
    @Name = N'Màn hình LG',
    @Description = N'24 inch IPS',
    @Price = 3500000,
    @StockQuantity = 20;

--- IMPORT STOCK ---
EXEC dbo.ImportStock
    @ProductID = 1,
    @Quantity = 5;

--- CREATE ORDER ---
DECLARE @Details dbo.OrderDetailType;
DECLARE @Now DATETIME;

SET @Now = GETDATE();

INSERT INTO @Details (ProductID, Quantity, UnitPrice)
VALUES
(1, 1, 20000000),
(2, 2, 500000);

EXEC dbo.CreateOrder
    @CustomerID = 1,
    @OrderDate = @Now,
    @OrderDetails = @Details;

--- DATA ---
SELECT * FROM dbo.Products;
SELECT * FROM dbo.Customers;
SELECT * FROM dbo.Orders;
SELECT * FROM dbo.OrderDetails;
SELECT * FROM dbo.InventoryTransactions;
SELECT * FROM dbo.vw_OrderSummary;
SELECT * FROM dbo.vw_ProductInventory;