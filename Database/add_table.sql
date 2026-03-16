IF DB_ID('InventoryOrderDB') IS NULL
BEGIN
    CREATE DATABASE InventoryOrderDB;
END;
GO

-- Products
IF OBJECT_ID('dbo.Products', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Products (
        ProductID INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL,
        Description NVARCHAR(MAX),
        Price DECIMAL(18,2) NOT NULL,
        StockQuantity INT NOT NULL,
        CreatedDate DATETIME DEFAULT GETDATE(),
        ModifiedDate DATETIME DEFAULT GETDATE()
    );
END;
GO

-- Customers
IF OBJECT_ID('dbo.Customers', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Customers (
        CustomerID INT IDENTITY(1,1) PRIMARY KEY,
        FirstName NVARCHAR(50) NOT NULL,
        LastName NVARCHAR(50) NOT NULL,
        Email NVARCHAR(100) NOT NULL UNIQUE,
        Phone NVARCHAR(15),
        Address NVARCHAR(MAX),
        CreatedDate DATETIME DEFAULT GETDATE(),
        ModifiedDate DATETIME DEFAULT GETDATE()
    );
END;
GO

-- Orders
IF OBJECT_ID('dbo.Orders', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Orders (
        OrderID INT IDENTITY(1,1) PRIMARY KEY,
        CustomerID INT NOT NULL,
        OrderDate DATETIME DEFAULT GETDATE(),
        TotalAmount DECIMAL(18,2) NOT NULL,
        Status NVARCHAR(20) NOT NULL,
        CONSTRAINT FK_Orders_Customers
            FOREIGN KEY (CustomerID) REFERENCES dbo.Customers(CustomerID)
    );
END;
GO

-- OrderDetails
IF OBJECT_ID('dbo.OrderDetails', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.OrderDetails (
        OrderDetailID INT IDENTITY(1,1) PRIMARY KEY,
        OrderID INT NOT NULL,
        ProductID INT NOT NULL,
        Quantity INT NOT NULL,
        UnitPrice DECIMAL(18,2) NOT NULL,
        TotalPrice AS (Quantity * UnitPrice) PERSISTED,
        CONSTRAINT FK_OrderDetails_Orders
            FOREIGN KEY (OrderID) REFERENCES dbo.Orders(OrderID),
        CONSTRAINT FK_OrderDetails_Products
            FOREIGN KEY (ProductID) REFERENCES dbo.Products(ProductID)
    );
END;
GO

-- InventoryTransactions
IF OBJECT_ID('dbo.InventoryTransactions', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.InventoryTransactions (
        TransactionID INT IDENTITY(1,1) PRIMARY KEY,
        ProductID INT NOT NULL,
        TransactionType NVARCHAR(20) NOT NULL,
        Quantity INT NOT NULL,
        TransactionDate DATETIME DEFAULT GETDATE(),
        CONSTRAINT FK_InventoryTransactions_Products
            FOREIGN KEY (ProductID) REFERENCES dbo.Products(ProductID)
    );
END;
GO