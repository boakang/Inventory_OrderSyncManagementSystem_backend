USE InventoryOrderDB;
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.check_constraints
    WHERE name = 'CK_Products_Price'
)
BEGIN
    ALTER TABLE dbo.Products
    ADD CONSTRAINT CK_Products_Price CHECK (Price >= 0);
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.check_constraints
    WHERE name = 'CK_Products_StockQuantity'
)
BEGIN
    ALTER TABLE dbo.Products
    ADD CONSTRAINT CK_Products_StockQuantity CHECK (StockQuantity >= 0);
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.check_constraints
    WHERE name = 'CK_OrderDetails_Quantity'
)
BEGIN
    ALTER TABLE dbo.OrderDetails
    ADD CONSTRAINT CK_OrderDetails_Quantity CHECK (Quantity > 0);
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.check_constraints
    WHERE name = 'CK_OrderDetails_UnitPrice'
)
BEGIN
    ALTER TABLE dbo.OrderDetails
    ADD CONSTRAINT CK_OrderDetails_UnitPrice CHECK (UnitPrice >= 0);
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.check_constraints
    WHERE name = 'CK_Orders_TotalAmount'
)
BEGIN
    ALTER TABLE dbo.Orders
    ADD CONSTRAINT CK_Orders_TotalAmount CHECK (TotalAmount >= 0);
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.check_constraints
    WHERE name = 'CK_Orders_Status'
)
BEGIN
    ALTER TABLE dbo.Orders
    ADD CONSTRAINT CK_Orders_Status CHECK (Status IN ('Pending', 'Confirmed', 'Completed', 'Cancelled'));
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.check_constraints
    WHERE name = 'CK_InventoryTransactions_Type'
)
BEGIN
    ALTER TABLE dbo.InventoryTransactions
    ADD CONSTRAINT CK_InventoryTransactions_Type CHECK (TransactionType IN ('IN', 'OUT'));
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.check_constraints
    WHERE name = 'CK_InventoryTransactions_Quantity'
)
BEGIN
    ALTER TABLE dbo.InventoryTransactions
    ADD CONSTRAINT CK_InventoryTransactions_Quantity CHECK (Quantity > 0);
END;
GO