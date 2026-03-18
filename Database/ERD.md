# Entity Relationship Diagram (ERD)

Tài liệu này mô tả schema hiện tại theo các EF Core migrations trong project.

## Bảng (tables)

- `Categories`
- `Customers`
- `Suppliers`
- `Products`
- `Orders`
- `OrderDetails`
- `InventoryTransactions`

Ghi chú:

- `__EFMigrationsHistory` là bảng hệ thống do EF Core tự tạo để theo dõi migrations đã apply. Đây không phải bảng nghiệp vụ nên không liệt kê trong ERD.

## ERD (Mermaid)

```mermaid
erDiagram
    CATEGORIES {
        int CategoryID PK
        string Name "nullable"
        string Description "nullable"
    }

    CUSTOMERS {
        int CustomerID PK
        string FirstName
        string LastName
        string Email "unique, nvarchar(256)"
        string Phone
        string Address
        datetime CreatedDate
        datetime ModifiedDate
        datetime LastModified
    }

    SUPPLIERS {
        int SupplierID PK
        string Name
        string ContactInfo
        datetime CreatedDate
        datetime ModifiedDate
        datetime LastModified
    }

    PRODUCTS {
        int ProductID PK
        int CategoryID "nullable, FK"
        int SupplierID "nullable, FK"
        string Name
        string Description
        decimal Price
        int StockQuantity
        datetime CreatedDate
        datetime ModifiedDate
        datetime LastModified
    }

    ORDERS {
        int OrderID PK
        int CustomerID FK
        datetime OrderDate
        decimal TotalAmount
        string Status
        datetime LastModified
    }

    ORDERDETAILS {
        int OrderDetailID PK
        int OrderID FK
        int ProductID FK
        int Quantity
        decimal UnitPrice
        decimal TotalPrice
    }

    INVENTORYTRANSACTIONS {
        int TransactionID PK
        int ProductID FK
        string TransactionType
        int Quantity
        datetime TransactionDate
    }

    ORDERS ||--o{ ORDERDETAILS : contains

    CUSTOMERS ||--o{ ORDERS : places
    PRODUCTS ||--o{ ORDERDETAILS : item
    PRODUCTS ||--o{ INVENTORYTRANSACTIONS : logs

    CATEGORIES ||--o{ PRODUCTS : categorizes
    SUPPLIERS ||--o{ PRODUCTS : supplies
```

## Ghi chú quan trọng

### Bảng hệ thống

- `__EFMigrationsHistory`: lưu `MigrationId` và `ProductVersion` (EF Core) để biết DB đang ở phiên bản schema nào. Không nên xoá/sửa thủ công trừ khi bạn hiểu rõ quy trình migrations.

FK được tạo trong DB (đã enforce bằng migration bổ sung):

- `Orders.CustomerID` → `Customers.CustomerID` (Restrict)
- `OrderDetails.OrderID` → `Orders.OrderID` (Cascade)
- `OrderDetails.ProductID` → `Products.ProductID` (Restrict)
- `InventoryTransactions.ProductID` → `Products.ProductID` (Restrict)
- `Products.CategoryID` → `Categories.CategoryID` (Restrict, nullable)
- `Products.SupplierID` → `Suppliers.SupplierID` (Restrict, nullable)

Ràng buộc unique:

- `Customers.Email` (unique index `IX_Customers_Email`)