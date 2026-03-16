# Inventory Order Sync Management System

This project is a backend system for managing inventory and orders, built using:

- ASP.NET Core Web API
- SQL Server
- Entity Framework Core

## Features

- **Product Management**: Add, update, delete, and view products.
- **Customer Management**: Manage customer information.
- **Inventory Tracking**: Track stock levels and updates.
- **Sales Order Processing**: Handle order creation and inventory updates.

## Project Structure

The project is organized into the following main directories and files:

### Root Directory
- **appsettings.Development.json**: Configuration file for development environment.
- **appsettings.json**: General configuration file.
- **Inventory_OrderSyncManagementSystem.csproj**: Project file for the .NET application.
- **Inventory_OrderSyncManagementSystem.sln**: Solution file for the project.
- **Program.cs**: Entry point of the application.
- **README.md**: Documentation file (this file).
- **WeatherForecast.cs**: Example model for weather forecasting.

### Controllers
Contains API controllers for handling HTTP requests:
- `CategoriesController.cs`
- `CustomersController.cs`
- `InventoryController.cs`
- `OrdersController.cs`
- `ProductsController.cs`
- `SynchronizationController.cs`
- `WeatherForecastController.cs`

### Data
- **AppDbContext.cs**: Entity Framework database context for managing database operations.

### Database
Contains SQL scripts for database setup and management:
- `add_constraint.sql`
- `add_table.sql`
- `procedure.sql`
- `sample_data.sql`
- `trigger.sql`
- `view.sql`
- **DataDictionary.md**: Documentation for database fields.
- **ERD.md**: Entity Relationship Diagram documentation.

### Generated
Contains auto-generated files and code.

### InventoryOrderSystem
Organized into subdirectories for backend and database management:
- **Backend**: Contains controllers, data, DTOs, migrations, models, repositories, and services.
- **Database**: Contains functions, seed data, stored procedures, tables, triggers, and views.
- **Docs**: Documentation files such as API and database design.

### Models
Contains data models and DTOs:
- `Category.cs`, `CategoryDto.cs`
- `Customer.cs`, `CustomerDto.cs`
- `InventoryDto.cs`
- `InventoryTransaction.cs`
- `Order.cs`, `OrderDetail.cs`, `OrderDto.cs`
- `Product.cs`, `ProductDto.cs`

### Properties
- **launchSettings.json**: Configuration for application launch settings.

### Services
Contains service classes for business logic:
- `CategoryService.cs`, `ICategoryService.cs`
- `CustomerService.cs`, `ICustomerService.cs`
- `InventoryService.cs`, `IInventoryService.cs`
- `OrderService.cs`, `IOrderService.cs`
- `ProductService.cs`, `IProductService.cs`
- `ReportingService.cs`
- `SynchronizationService.cs`
- `TransactionService.cs`

### Other Directories
- **bin/**: Contains compiled binaries.
- **obj/**: Contains temporary object files.
- **publish/**: Contains files for deployment.

## Getting Started

1. Clone the repository.
2. Open the solution in Visual Studio.
3. Update the `appsettings.json` file with your SQL Server connection string.
4. Run the application.

## API Endpoints

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