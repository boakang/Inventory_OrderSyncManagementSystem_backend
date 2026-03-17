# Inventory Order Sync Management System (Backend)

ASP.NET Core Web API quản lý Products/Customers/Orders/Inventory/Suppliers. Backend dùng Entity Framework Core với SQL Server, có các endpoint đồng bộ (delta sync) dựa trên `LastModified`, và có bộ tài liệu Database (ERD + Data Dictionary) kèm các script SQL (tables/views/procedures/triggers/sample data).

Mục tiêu tài liệu này là “HR-friendly”: mô tả ngắn gọn, rõ cấu trúc, công nghệ, kết nối DB, và các artefact liên quan thiết kế/tối ưu/kiểm thử DB.

## Công nghệ sử dụng

- .NET 7 (`net7.0`)
- ASP.NET Core Web API
- Entity Framework Core 7 + SQL Server
- Swagger/OpenAPI (bật khi `ASPNETCORE_ENVIRONMENT=Development`)

## Database & Kết nối

- Database: SQL Server (LocalDB/Express/Developer/Full)
- Kết nối: `appsettings.json` → `ConnectionStrings:DefaultConnection`
- Schema được tạo bởi EF Core Migrations trong thư mục `Migrations/`

## Cấu trúc thư mục (backend)

- `Controllers/`: Web API endpoints
- `Services/`: business logic (CRUD, inventory adjustments, reporting, sync)
- `Models/`: entities + DTO
- `Data/`: `AppDbContext`
- `Migrations/`: EF migrations
- `Database/`: tài liệu & script SQL
  - `ERD.md`: sơ đồ quan hệ dữ liệu
  - `DataDictionary.md`: từ điển dữ liệu
  - `sample_data.sql`: seed data qua SSMS
  - `db_constraints.sql`: chuẩn hóa dữ liệu âm + CHECK constraints không âm
  - `db_functions.sql`: views/functions/stored procedures (tham khảo/chạy tay)

## Chạy dự án

### Prerequisites

- .NET SDK 7.x
- SQL Server
- (Khuyến nghị) EF Core CLI: `dotnet-ef`

### 1) Apply migrations

```bash
dotnet tool install --global dotnet-ef
dotnet ef database update
```

### 2) Run

```bash
dotnet run
```

- HTTP: `http://localhost:5080`
- Swagger (Development): `http://localhost:5080/swagger`

## Seed dữ liệu mẫu

### Cách A (Dev): endpoint seed

Chỉ hoạt động khi environment là Development.

```bash
curl -X POST http://localhost:5080/api/dev/seed
```

### Cách B: chạy script trên SSMS

- `Database/sample_data.sql`
- (Khuyến nghị) chạy thêm `Database/db_constraints.sql` để chuẩn hóa dữ liệu và bật CHECK constraints.

## Kiểm tra kết nối DB (Health-check)

- `GET /api/health` → `{ status: "ok" }`
- `GET /api/health/db` → `canConnect`, `canQuery`, `dataSource`, `database`, `error`

## Data integrity (không âm)

Backend đã enforce các rule sau:

- `Products.Price >= 0`
- `Products.StockQuantity >= 0`
- `OrderDetails.Quantity >= 0`
- `InventoryTransactions.Quantity >= 0`
- Khi thao tác tồn kho theo transaction outbound (Stock Out / Issue / Sales Order), hệ thống sẽ trừ stock và không cho stock xuống dưới 0.

Lưu ý: lịch sử tồn kho (`InventoryTransactions`) hiện lưu `Quantity` không âm; hướng tăng/giảm được suy ra từ `TransactionType`.

## API Endpoints

Base path: `/api/*`

CRUD:

- `GET/POST/PUT/DELETE /api/products`
- `GET/POST/PUT/DELETE /api/customers`
- `GET/POST/PUT/DELETE /api/orders`
- `GET/POST/PUT/DELETE /api/inventory`
- `GET/POST/PUT/DELETE /api/categories`
- `GET/POST/PUT/DELETE /api/suppliers`

Reports:

- `GET /api/reports/top-selling?topN=5`
- `GET /api/reports/revenue?period=daily|monthly`
- `GET /api/reports/inventory`

Synchronization (delta sync theo `LastModified`):

- `GET /api/Synchronization/GetUpdatedData?lastModified=...`
- `POST /api/Synchronization/UploadChanges`

## Ghi chú liên quan yêu cầu HR

- Thiết kế & quản lý DB: tài liệu `Database/ERD.md`, `Database/DataDictionary.md`, các script tạo bảng/constraint
- Hỗ trợ Backend: EF Core (`Data/AppDbContext.cs`, migrations), Web API (Controllers/Services)
- Kiểm thử & bảo trì: `GET /api/health/db` để kiểm tra canConnect/canQuery; script CHECK constraints để đảm bảo toàn vẹn

## Dev settings

- CORS: policy `AllowAll` đã bật trong `Program.cs` để hỗ trợ frontend trong quá trình dev.

## Các link khác trong project
[Frontend](https://github.com/boakang/Inventory_OrderSyncManagementSystem_frontend)
[Database](https://github.com/boakang/Inventory_OrderSyncManagementSystem_sqlserver)