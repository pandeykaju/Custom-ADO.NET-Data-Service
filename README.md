# Custom ADO.NET Data Service with Caching and Web API Endpoints

## Project Overview

This project is an ASP.NET Core Web API application developed using C# and ADO.NET. It demonstrates database connectivity, CRUD operations, SQL transactions, disconnected data access, and in-memory caching.

## Technologies Used

- C#
- ASP.NET Core Web API (.NET 10)
- ADO.NET
- Microsoft.Data.SqlClient
- SQL Server LocalDB
- Visual Studio 2026
- REST API
- IMemoryCache

## Project Features

1. Create, retrieve, update, and delete products.
2. Retrieve products using the connected ADO.NET model with SqlDataReader.
3. Retrieve products using the disconnected ADO.NET model with SqlDataAdapter and DataSet.
4. Create customer orders using SQL transactions.
5. Commit successful transactions and roll back failed transactions.
6. Use parameterized SQL queries.
7. Cache product data using IMemoryCache.
8. Configure cache expiration through appsettings.json.
9. Invalidate the product cache after successful CRUD operations.

## Database Structure

Database name: CustomADODatabase

The database contains three tables:

- Products — Stores product information, including price and quantity.
- Orders — Stores customer orders and total amounts.
- OrderItems — Stores the products and quantities associated with each order.

The DatabaseSetup.sql file contains the database setup script.

## API Endpoints

| Method | Endpoint | Description |
|---|---|---|
| GET | /api/products | Retrieve all products |
| GET | /api/products/{id} | Retrieve a product by ID |
| POST | /api/products | Create a product |
| PUT | /api/products/{id} | Update a product |
| DELETE | /api/products/{id} | Delete a product |
| GET | /api/products/dataset | Retrieve products using DataSet |
| POST | /api/orders | Create an order using a SQL transaction |

## How to Run

1. Install Visual Studio with ASP.NET and web development support.
2. Install SQL Server LocalDB.
3. Open the CustomADODataService.slnx solution.
4. Run DatabaseSetup.sql against SQL Server LocalDB to create the database and tables.
5. Check the DefaultConnection connection string in appsettings.json.
6. Restore the NuGet packages.
7. Build the solution.
8. Run the application using HTTPS.
9. Test the endpoints using the included .http file or another HTTP client.

## Caching

The application uses IMemoryCache to cache the product list.

- X-Cache: MISS indicates that data was retrieved from the database and cached.
- X-Cache: HIT indicates that data was served from the cache.
- The cache expiration duration is configurable in appsettings.json.
- Successful product creation, updates, and deletion invalidate the cached product list.

## Transaction Management

Order creation uses SqlTransaction to ensure that changes to Orders and OrderItems are committed together.

If an operation fails, the transaction is rolled back to prevent partially saved orders.

## Conclusion

This project demonstrates core ADO.NET concepts, REST API development, transaction management, and caching using ASP.NET Core and SQL Server.
