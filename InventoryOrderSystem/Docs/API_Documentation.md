# API Documentation

This document provides details about the API endpoints for the Inventory and Order Management System.

## Endpoints

### Products
- **GET** `/api/products` - Get all products.
- **GET** `/api/products/{id}` - Get a product by ID.
- **POST** `/api/products` - Add a new product.
- **PUT** `/api/products/{id}` - Update an existing product.
- **DELETE** `/api/products/{id}` - Delete a product.

### Customers
- **GET** `/api/customers` - Get all customers.
- **GET** `/api/customers/{id}` - Get a customer by ID.
- **POST** `/api/customers` - Add a new customer.
- **PUT** `/api/customers/{id}` - Update an existing customer.
- **DELETE** `/api/customers/{id}` - Delete a customer.

### Orders
- **GET** `/api/orders` - Get all orders.
- **GET** `/api/orders/{id}` - Get an order by ID.
- **POST** `/api/orders` - Create a new order.
- **PUT** `/api/orders/{id}` - Update an existing order.
- **DELETE** `/api/orders/{id}` - Delete an order.

### Inventory
- **GET** `/api/inventory` - Get all inventory items.
- **GET** `/api/inventory/{productId}` - Get inventory by product ID.
- **POST** `/api/inventory` - Add inventory for a product.
- **PUT** `/api/inventory/{productId}` - Update inventory for a product.
- **DELETE** `/api/inventory/{productId}` - Delete inventory for a product.