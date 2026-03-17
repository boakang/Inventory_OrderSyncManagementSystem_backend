-- Enforce non-negative quantity/stock constraints (SQL Server)
-- Safe to re-run (checks for existing constraints).

SET NOCOUNT ON;

BEGIN TRY
    BEGIN TRAN;

    -- Normalize legacy data that may have negative quantities.
    UPDATE dbo.InventoryTransactions
    SET Quantity = ABS(Quantity)
    WHERE Quantity < 0;

    UPDATE dbo.OrderDetails
    SET Quantity = ABS(Quantity)
    WHERE Quantity < 0;

    UPDATE dbo.Products
    SET StockQuantity = 0
    WHERE StockQuantity < 0;

    UPDATE dbo.Products
    SET Price = 0
    WHERE Price < 0;

    -- Add CHECK constraints if missing.
    IF NOT EXISTS (
        SELECT 1
        FROM sys.check_constraints
        WHERE name = N'CK_Products_StockQuantity_NonNegative'
          AND parent_object_id = OBJECT_ID(N'dbo.Products')
    )
    BEGIN
        ALTER TABLE dbo.Products WITH CHECK
        ADD CONSTRAINT CK_Products_StockQuantity_NonNegative
        CHECK (StockQuantity >= 0);
    END;

    IF NOT EXISTS (
        SELECT 1
        FROM sys.check_constraints
        WHERE name = N'CK_Products_Price_NonNegative'
          AND parent_object_id = OBJECT_ID(N'dbo.Products')
    )
    BEGIN
        ALTER TABLE dbo.Products WITH CHECK
        ADD CONSTRAINT CK_Products_Price_NonNegative
        CHECK (Price >= 0);
    END;

    IF NOT EXISTS (
        SELECT 1
        FROM sys.check_constraints
        WHERE name = N'CK_InventoryTransactions_Quantity_NonNegative'
          AND parent_object_id = OBJECT_ID(N'dbo.InventoryTransactions')
    )
    BEGIN
        ALTER TABLE dbo.InventoryTransactions WITH CHECK
        ADD CONSTRAINT CK_InventoryTransactions_Quantity_NonNegative
        CHECK (Quantity >= 0);
    END;

    IF NOT EXISTS (
        SELECT 1
        FROM sys.check_constraints
        WHERE name = N'CK_OrderDetails_Quantity_NonNegative'
          AND parent_object_id = OBJECT_ID(N'dbo.OrderDetails')
    )
    BEGIN
        ALTER TABLE dbo.OrderDetails WITH CHECK
        ADD CONSTRAINT CK_OrderDetails_Quantity_NonNegative
        CHECK (Quantity >= 0);
    END;

    COMMIT;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK;

    DECLARE @Err NVARCHAR(4000) = ERROR_MESSAGE();
    RAISERROR(@Err, 16, 1);
END CATCH;

-- Quick verification
SELECT TOP 50 TransactionID, ProductID, TransactionType, Quantity, TransactionDate
FROM dbo.InventoryTransactions
ORDER BY TransactionID DESC;

SELECT TOP 50 ProductID, Name, StockQuantity
FROM dbo.Products
ORDER BY ProductID DESC;
