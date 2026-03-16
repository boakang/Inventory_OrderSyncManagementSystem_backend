# Database Design

## Overview
The database for the Inventory and Order Management System is designed to be in 3rd Normal Form (3NF) to ensure data integrity and minimize redundancy.

## Tables and Relationships

### Products
- **Attributes**:
  - `ProductID` (Primary Key)
  - `Name`
  - `Description`
  - `Price`
  - `StockQuantity`
  - `CreatedDate`
  - `ModifiedDate`

### Customers
- **Attributes**:
  - `CustomerID` (Primary Key)
  - `FirstName`
  - `LastName`
  - `Email`
  - `Phone`
  - `Address`
  - `CreatedDate`
  - `ModifiedDate`

### Orders
- **Attributes**:
  - `OrderID` (Primary Key)
  - `CustomerID` (Foreign Key to `Customers`)
  - `OrderDate`
  - `TotalAmount`
  - `Status`

### OrderDetails
- **Attributes**:
  - `OrderDetailID` (Primary Key)
  - `OrderID` (Foreign Key to `Orders`)
  - `ProductID` (Foreign Key to `Products`)
  - `Quantity`
  - `UnitPrice`
  - `TotalPrice`

### InventoryTransactions
- **Attributes**:
  - `TransactionID` (Primary Key)
  - `ProductID` (Foreign Key to `Products`)
  - `TransactionType`
  - `Quantity`
  - `TransactionDate`

## Relationships
- `Products` has a one-to-many relationship with `OrderDetails`.
- `Customers` has a one-to-many relationship with `Orders`.
- `Orders` has a one-to-many relationship with `OrderDetails`.
- `Products` has a one-to-many relationship with `InventoryTransactions`.