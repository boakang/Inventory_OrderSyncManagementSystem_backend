# Data Dictionary

## Overview
This document provides a detailed description of the tables, columns, and relationships in the Inventory and Order Management System database.

## Tables

### 1. Products
- **Description**: Stores product information.
- **Columns**:
  - `ProductID` (INT, Primary Key): Unique identifier for each product.
  - `Name` (NVARCHAR(100)): Name of the product.
  - `Description` (NVARCHAR(MAX)): Description of the product.
  - `Price` (DECIMAL(18,2)): Price of the product.
  - `StockQuantity` (INT): Quantity available in stock.
  - `CreatedDate` (DATETIME): Date the product was created.
  - `ModifiedDate` (DATETIME): Date the product was last modified.

### 2. Customers
- **Description**: Stores customer information.
- **Columns**:
  - `CustomerID` (INT, Primary Key): Unique identifier for each customer.
  - `FirstName` (NVARCHAR(50)): First name of the customer.
  - `LastName` (NVARCHAR(50)): Last name of the customer.
  - `Email` (NVARCHAR(100)): Email address of the customer.
  - `Phone` (NVARCHAR(15)): Phone number of the customer.
  - `Address` (NVARCHAR(MAX)): Address of the customer.
  - `CreatedDate` (DATETIME): Date the customer was created.
  - `ModifiedDate` (DATETIME): Date the customer was last modified.

### 3. Orders
- **Description**: Stores order information.
- **Columns**:
  - `OrderID` (INT, Primary Key): Unique identifier for each order.
  - `CustomerID` (INT, Foreign Key): References `Customers.CustomerID`.
  - `OrderDate` (DATETIME): Date the order was placed.
  - `TotalAmount` (DECIMAL(18,2)): Total amount of the order.
  - `Status` (NVARCHAR(20)): Status of the order (e.g., Pending, Completed).

### 4. OrderDetails
- **Description**: Stores details of each order.
- **Columns**:
  - `OrderDetailID` (INT, Primary Key): Unique identifier for each order detail.
  - `OrderID` (INT, Foreign Key): References `Orders.OrderID`.
  - `ProductID` (INT, Foreign Key): References `Products.ProductID`.
  - `Quantity` (INT): Quantity of the product ordered.
  - `UnitPrice` (DECIMAL(18,2)): Price per unit of the product.
  - `TotalPrice` (COMPUTED): Calculated as `Quantity * UnitPrice`.

### 5. InventoryTransactions
- **Description**: Stores inventory transaction history.
- **Columns**:
  - `TransactionID` (INT, Primary Key): Unique identifier for each transaction.
  - `ProductID` (INT, Foreign Key): References `Products.ProductID`.
  - `TransactionType` (NVARCHAR(20)): Type of transaction (e.g., Stock In, Stock Out).
  - `Quantity` (INT): Quantity involved in the transaction.
  - `TransactionDate` (DATETIME): Date of the transaction.