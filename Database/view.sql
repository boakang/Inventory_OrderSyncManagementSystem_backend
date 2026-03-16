USE InventoryOrderDB;
GO

CREATE OR ALTER VIEW dbo.vw_OrderSummary
AS
SELECT
    o.OrderID,
    o.OrderDate,
    c.FirstName + N' ' + c.LastName AS CustomerName,
    o.TotalAmount,
    o.Status
FROM dbo.Orders o
INNER JOIN dbo.Customers c
    ON o.CustomerID = c.CustomerID;
GO

CREATE OR ALTER VIEW dbo.vw_ProductInventory
AS
SELECT
    ProductID,
    Name,
    Price,
    StockQuantity,
    CreatedDate,
    ModifiedDate
FROM dbo.Products;
GO