# Entity Relationship Diagram (ERD)

## Overview
The database for the Inventory and Order Management System is designed to be in 3rd Normal Form (3NF) to ensure data integrity and minimize redundancy. Below is the high-level structure of the database.

## Entities and Relationships

### 1. Products
- **Table Name**: `Products`
- **Attributes**:
  - `ProductID` (Primary Key)
  - `Name`
  - `Description`
  - `Price`
  - `StockQuantity`
  - `CreatedDate`
  - `ModifiedDate`

### 2. Customers
- **Table Name**: `Customers`
- **Attributes**:
  - `CustomerID` (Primary Key)
  - `FirstName`
  - `LastName`
  - `Email`
  - `Phone`
  - `Address`
  - `CreatedDate`
  - `ModifiedDate`

### 3. Orders
- **Table Name**: `Orders`
- **Attributes**:
  - `OrderID` (Primary Key)
  - `CustomerID` (Foreign Key to `Customers`)
  - `OrderDate`
  - `TotalAmount`
  - `Status`

### 4. OrderDetails
- **Table Name**: `OrderDetails`
- **Attributes**:
  - `OrderDetailID` (Primary Key)
  - `OrderID` (Foreign Key to `Orders`)
  - `ProductID` (Foreign Key to `Products`)
  - `Quantity`
  - `UnitPrice`
  - `TotalPrice`

### 5. InventoryTransactions
- **Table Name**: `InventoryTransactions`
- **Attributes**:
  - `TransactionID` (Primary Key)
  - `ProductID` (Foreign Key to `Products`)
  - `TransactionType` (e.g., "Stock In", "Stock Out")
  - `Quantity`
  - `TransactionDate`

## Relationships
- `Products` has a one-to-many relationship with `OrderDetails`.
- `Customers` has a one-to-many relationship with `Orders`.
- `Orders` has a one-to-many relationship with `OrderDetails`.
- `Products` has a one-to-many relationship with `InventoryTransactions`.

## ERD Diagram

The following entities and relationships are part of the Inventory and Order Management System:

### Entities
1. **Products**
   - Attributes: ProductID, Name, Description, Price, StockQuantity, CreatedDate, ModifiedDate

2. **Customers**
   - Attributes: CustomerID, FirstName, LastName, Email, Phone, Address, CreatedDate, ModifiedDate

3. **Orders**
   - Attributes: OrderID, CustomerID, OrderDate, TotalAmount, Status

4. **OrderDetails**
   - Attributes: OrderDetailID, OrderID, ProductID, Quantity, UnitPrice, TotalPrice

5. **InventoryTransactions**
   - Attributes: TransactionID, ProductID, TransactionType, Quantity, TransactionDate

### Relationships
- Products → OrderDetails (1:N)
- Customers → Orders (1:N)
- Orders → OrderDetails (1:N)
- Products → InventoryTransactions (1:N)

### Diagram
A visual representation of the ERD can be created using tools like dbdiagram.io or similar.