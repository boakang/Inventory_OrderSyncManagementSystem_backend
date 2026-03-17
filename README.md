# Inventory Order Sync Management System

A comprehensive enterprise-grade solution for managing products, customers, inventory, and orders. This system is designed for small to medium-sized businesses and supports real-time synchronization between web and mobile platforms.

## Core Features

- **Product Management**: Full CRUD operations for products and categories.
- **Customer CRM**: Manage client data and track transaction history.
- **Order Processing**: Multi-step order creation with real-time stock availability validation.
- **Warehouse Management**: Track inbound and outbound inventory movements with automatic stock updates.
- **Data Synchronization**: Built-in support for `LastModified` timestamps to sync data across web and mobile.
- **Advanced Analytics**: Visual dashboards and reports for revenue trends, top products, and low-stock alerts.

## Project Structure

The project is split into a robust Backend and a modern Frontend.

### [Backend](file:///d:/Inventory_OrderSyncManagementSystem) (ASP.NET Core 6.0)
- **Controllers**: RESTful API endpoints for all modules.
- **Services**: Business logic layer including synchronization and reporting.
- **Models**: EF Core entities and Data Transfer Objects (DTOs).
- **Data**: `AppDbContext` for SQL Server integration.

### [Frontend](file:///D:/Inventory_OrderSyncManagementSystem_frontend) (React + Vite)
- **Modern UI**: Built with Tailwind CSS and Lucide icons.
- **Interactive Dashboards**: Data visualization using Recharts.
- **Responsive Design**: Premium look and feel optimized for efficiency.

## How to Run

### 1. Prerequisites
- .NET 6.0 SDK or later
- SQL Server (LocalDB or Express)
- Node.js 18+ and npm

### 2. Backend Setup
1. Open the solution in Visual Studio or VS Code.
2. Update the connection string in `appsettings.json`.
3. Apply migrations: `dotnet ef database update`.
4. Run the project: `dotnet run`.

### 3. Frontend Setup
1. Navigate to the `D:\Inventory_OrderSyncManagementSystem_frontend` directory.
2. Install dependencies: `npm install`.
3. Start the dev server: `npm run dev`.
4. Access the app at `http://localhost:3000`.

## API Endpoints
- `GET /api/products`: List all products
- `POST /api/orders`: Create a new order (triggers stock deduction)
- `GET /api/reports/revenue`: Fetch revenue analytics
- `GET /api/suppliers`: Manage suppliers

- **Products**: `/api/products`
- **Customers**: `/api/customers`
- **Orders**: `/api/orders`
- **Inventory**: `/api/inventory`

## Documentation

- **ERD**: Entity Relationship Diagram (located in the `Docs/` folder).
- **Data Dictionary**: Definitions of tables and fields (located in the `Docs/` folder).
- **Database Schema**: SQL scripts for creating the database (located in the `Database/` folder).
- **Swagger**: Interactive API documentation.

## Prerequisites

- .NET 6.0 SDK or later
- SQL Server 2019 or later

## License

This project is licensed under the MIT License.

## link frontend: https://github.com/boakang/Inventory_OrderSyncManagementSystem_frontend

## Notes
- Ensure the backend is running before starting the frontend to enable full data functionality.
- Synchronization logic uses `LastModified` flags for delta updates.

