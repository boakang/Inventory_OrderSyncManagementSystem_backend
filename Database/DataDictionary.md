# Data Dictionary

Tài liệu này mô tả chi tiết bảng/cột theo EF Core migration `InitialCreate` (20260317044512).

Ghi chú:

- Kiểu dữ liệu string trong migration hiện tại là `nvarchar(max)`.
- Cột thời gian dùng `datetime2`.
- `OrderDetails.TotalPrice` hiện là cột lưu (không phải computed column).
- Các ràng buộc không âm có thể bật bằng script `db_constraints.sql`.

## 1) Categories

| Column | Type | Null | Key | Description |
|---|---|---:|---|---|
| CategoryID | int (IDENTITY) | NO | PK | Khoá chính danh mục |
| Name | nvarchar(max) | YES |  | Tên danh mục |
| Description | nvarchar(max) | YES |  | Mô tả |

## 2) Suppliers

| Column | Type | Null | Key | Description |
|---|---|---:|---|---|
| SupplierID | int (IDENTITY) | NO | PK | Khoá chính NCC |
| Name | nvarchar(max) | NO |  | Tên nhà cung cấp |
| ContactInfo | nvarchar(max) | NO |  | Thông tin liên hệ |
| CreatedDate | datetime2 | NO |  | Ngày tạo |
| ModifiedDate | datetime2 | NO |  | Ngày sửa |
| LastModified | datetime2 | NO |  | Dùng cho delta sync |

## 3) Customers

| Column | Type | Null | Key | Description |
|---|---|---:|---|---|
| CustomerID | int (IDENTITY) | NO | PK | Khoá chính khách hàng |
| FirstName | nvarchar(max) | NO |  | Họ |
| LastName | nvarchar(max) | NO |  | Tên |
| Email | nvarchar(256) | NO | UQ | Email (unique index `IX_Customers_Email`) |
| Phone | nvarchar(max) | NO |  | SĐT |
| Address | nvarchar(max) | NO |  | Địa chỉ |
| CreatedDate | datetime2 | NO |  | Ngày tạo |
| ModifiedDate | datetime2 | NO |  | Ngày sửa |
| LastModified | datetime2 | NO |  | Dùng cho delta sync |

## 4) Products

| Column | Type | Null | Key | Description |
|---|---|---:|---|---|
| ProductID | int (IDENTITY) | NO | PK | Khoá chính sản phẩm |
| Name | nvarchar(max) | NO |  | Tên sản phẩm |
| Description | nvarchar(max) | NO |  | Mô tả |
| Price | decimal(18,2) | NO |  | Giá bán (khuyến nghị `>= 0`) |
| StockQuantity | int | NO |  | Tồn kho hiện tại (khuyến nghị `>= 0`) |
| CreatedDate | datetime2 | NO |  | Ngày tạo |
| ModifiedDate | datetime2 | NO |  | Ngày sửa |
| LastModified | datetime2 | NO |  | Dùng cho delta sync |

## 5) Orders

| Column | Type | Null | Key | Description |
|---|---|---:|---|---|
| OrderID | int (IDENTITY) | NO | PK | Khoá chính đơn hàng |
| CustomerID | int | NO | FK | FK tới Customers.CustomerID (Restrict) |
| OrderDate | datetime2 | NO |  | Ngày đặt hàng |
| TotalAmount | decimal(18,2) | NO |  | Tổng tiền đơn |
| Status | nvarchar(max) | NO |  | Trạng thái (Pending/Completed/...) |
| LastModified | datetime2 | NO |  | Dùng cho delta sync |

## 6) OrderDetails

| Column | Type | Null | Key | Description |
|---|---|---:|---|---|
| OrderDetailID | int (IDENTITY) | NO | PK | Khoá chính chi tiết đơn |
| OrderID | int | NO | FK | FK tới Orders.OrderID (Cascade) |
| ProductID | int | NO | FK | FK tới Products.ProductID (Restrict) |
| Quantity | int | NO |  | Số lượng (khuyến nghị `>= 0`) |
| UnitPrice | decimal(18,2) | NO |  | Đơn giá tại thời điểm đặt |
| TotalPrice | decimal(18,2) | NO |  | Thành tiền (được tính và lưu bởi backend) |

## 7) InventoryTransactions

| Column | Type | Null | Key | Description |
|---|---|---:|---|---|
| TransactionID | int (IDENTITY) | NO | PK | Khoá chính giao dịch tồn kho |
| ProductID | int | NO | FK | FK tới Products.ProductID (Restrict) |
| TransactionType | nvarchar(max) | NO |  | Loại giao dịch (Stock In/Stock Out/Issue/Sales Order/...) |
| Quantity | int | NO |  | Số lượng (khuyến nghị `>= 0`) |
| TransactionDate | datetime2 | NO |  | Thời gian giao dịch |

## Relationships (tóm tắt)

- FK trong DB:
	- `Orders.CustomerID` → `Customers.CustomerID`
	- `OrderDetails.OrderID` → `Orders.OrderID`
	- `OrderDetails.ProductID` → `Products.ProductID`
	- `InventoryTransactions.ProductID` → `Products.ProductID`