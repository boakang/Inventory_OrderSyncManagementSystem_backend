USE InventoryOrderDB;
GO

CREATE OR ALTER TRIGGER dbo.trg_UpdateProductModifiedDate
ON dbo.Products
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE p
    SET p.ModifiedDate = GETDATE()
    FROM dbo.Products p
    INNER JOIN inserted i
        ON p.ProductID = i.ProductID;
END;
GO

CREATE OR ALTER TRIGGER dbo.trg_UpdateCustomerModifiedDate
ON dbo.Customers
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE c
    SET c.ModifiedDate = GETDATE()
    FROM dbo.Customers c
    INNER JOIN inserted i
        ON c.CustomerID = i.CustomerID;
END;
GO