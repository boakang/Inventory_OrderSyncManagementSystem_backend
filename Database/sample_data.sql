USE InventoryOrderDB;
GO

-- This script matches the EF Core schema created by migrations in this repo.
-- It does NOT require stored procedures, table types, triggers, or views.

DECLARE @Now DATETIME2 = SYSDATETIME();

-- Categories
IF NOT EXISTS (SELECT 1 FROM dbo.Categories WHERE Name = N'Electronics')
BEGIN
    INSERT INTO dbo.Categories (Name, Description)
    VALUES (N'Electronics', N'Devices and accessories');
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Categories WHERE Name = N'Office')
BEGIN
    INSERT INTO dbo.Categories (Name, Description)
    VALUES (N'Office', N'Office supplies');
END;

-- Suppliers
IF NOT EXISTS (SELECT 1 FROM dbo.Suppliers WHERE Name = N'Default Supplier')
BEGIN
    INSERT INTO dbo.Suppliers (Name, ContactInfo, CreatedDate, ModifiedDate, LastModified)
    VALUES (N'Default Supplier', N'contact@supplier.local', @Now, @Now, @Now);
END;

-- Customers
IF NOT EXISTS (SELECT 1 FROM dbo.Customers WHERE Email = N'an@gmail.com')
BEGIN
    INSERT INTO dbo.Customers
    (
        FirstName, LastName, Email, Phone, Address,
        CreatedDate, ModifiedDate, LastModified
    )
    VALUES
    (
        N'Nguyen', N'An', N'an@gmail.com', N'0901234567', N'TP.HCM',
        @Now, @Now, @Now
    );
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Customers WHERE Email = N'binh@gmail.com')
BEGIN
    INSERT INTO dbo.Customers
    (
        FirstName, LastName, Email, Phone, Address,
        CreatedDate, ModifiedDate, LastModified
    )
    VALUES
    (
        N'Tran', N'Binh', N'binh@gmail.com', N'0912345678', N'Ha Noi',
        @Now, @Now, @Now
    );
END;

-- Products
IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE Name = N'MacBook Pro 14')
BEGIN
    INSERT INTO dbo.Products
    (
        Name, Description, Price, StockQuantity,
        CreatedDate, ModifiedDate, LastModified
    )
    VALUES
    (
        N'MacBook Pro 14', N'M3 Pro, 16GB RAM, 512GB', 1999, 15,
        @Now, @Now, @Now
    );
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE Name = N'iPhone 15 Pro')
BEGIN
    INSERT INTO dbo.Products
    (
        Name, Description, Price, StockQuantity,
        CreatedDate, ModifiedDate, LastModified
    )
    VALUES
    (
        N'iPhone 15 Pro', N'Natural Titanium, 128GB', 999, 42,
        @Now, @Now, @Now
    );
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE Name = N'iPad Air')
BEGIN
    INSERT INTO dbo.Products
    (
        Name, Description, Price, StockQuantity,
        CreatedDate, ModifiedDate, LastModified
    )
    VALUES
    (
        N'iPad Air', N'M1 Chip, 64GB, Space Gray', 599, 0,
        @Now, @Now, @Now
    );
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE Name = N'AirPods Pro 2')
BEGIN
    INSERT INTO dbo.Products
    (
        Name, Description, Price, StockQuantity,
        CreatedDate, ModifiedDate, LastModified
    )
    VALUES
    (
        N'AirPods Pro 2', N'USB-C Charging Case', 249, 85,
        @Now, @Now, @Now
    );
END;

-- InventoryTransactions: seed stock (only if no transactions exist yet)
IF NOT EXISTS (SELECT 1 FROM dbo.InventoryTransactions)
BEGIN
    INSERT INTO dbo.InventoryTransactions (ProductID, TransactionType, Quantity, TransactionDate)
    SELECT ProductID, N'Seed Stock', StockQuantity, @Now
    FROM dbo.Products;
END;

-- Show results
SELECT TOP 50 * FROM dbo.Products ORDER BY ProductID DESC;
SELECT TOP 50 * FROM dbo.Customers ORDER BY CustomerID DESC;
SELECT TOP 50 * FROM dbo.Suppliers ORDER BY SupplierID DESC;
SELECT TOP 50 * FROM dbo.Categories ORDER BY CategoryID DESC;
SELECT TOP 50 * FROM dbo.InventoryTransactions ORDER BY TransactionID DESC;