
using Microsoft.Data.SqlClient;
using System.Data;
using CustomADODataService.Models;

namespace CustomADODataService.Repository
{
    public class OrderRepository
    {
        private readonly string _connectionString;

        public OrderRepository(IConfiguration configuration)
        {
            _connectionString = configuration
                .GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Connection string not found.");
        }

        public int CreateOrder(Order order)
        {
            using var connection =
                new SqlConnection(_connectionString);

            connection.Open();

            using var transaction =
                connection.BeginTransaction();

            try
            {
                decimal totalAmount = 0;

                // Insert order first
                using var orderCommand = new SqlCommand(
                    @"INSERT INTO dbo.Orders
                      (CustomerName, TotalAmount)
                      OUTPUT INSERTED.OrderId
                      VALUES (@CustomerName, 0)",
                    connection, transaction);

                orderCommand.Parameters.Add(
                    "@CustomerName", SqlDbType.NVarChar, 100)
                    .Value = order.CustomerName;

                int orderId =
                    (int)orderCommand.ExecuteScalar()!;

                foreach (var item in order.Items)
                {
                    // Retrieve the trusted product price
                    using var priceCommand = new SqlCommand(
                        @"SELECT Price FROM dbo.Products
                          WHERE ProductId = @ProductId",
                        connection, transaction);

                    priceCommand.Parameters.Add(
                        "@ProductId", SqlDbType.Int)
                        .Value = item.ProductId;

                    var priceResult =
                        priceCommand.ExecuteScalar();

                    if (priceResult == null)
                        throw new InvalidOperationException(
                            "Product not found.");

                    decimal unitPrice =
                        Convert.ToDecimal(priceResult);

                    totalAmount += unitPrice * item.Quantity;

                    // Insert order item
                    using var itemCommand = new SqlCommand(
                        @"INSERT INTO dbo.OrderItems
                          (OrderId, ProductId,
                           Quantity, UnitPrice)
                          VALUES
                          (@OrderId, @ProductId,
                           @Quantity, @UnitPrice)",
                        connection, transaction);

                    itemCommand.Parameters.Add(
                        "@OrderId", SqlDbType.Int)
                        .Value = orderId;

                    itemCommand.Parameters.Add(
                        "@ProductId", SqlDbType.Int)
                        .Value = item.ProductId;

                    itemCommand.Parameters.Add(
                        "@Quantity", SqlDbType.Int)
                        .Value = item.Quantity;

                    var priceParameter =
                        itemCommand.Parameters.Add(
                            "@UnitPrice", SqlDbType.Decimal);

                    priceParameter.Precision = 18;
                    priceParameter.Scale = 2;
                    priceParameter.Value = unitPrice;

                    itemCommand.ExecuteNonQuery();
                }

                // Update the final order total
                using var updateCommand = new SqlCommand(
                    @"UPDATE dbo.Orders
                      SET TotalAmount = @TotalAmount
                      WHERE OrderId = @OrderId",
                    connection, transaction);

                var totalParameter =
                    updateCommand.Parameters.Add(
                        "@TotalAmount", SqlDbType.Decimal);

                totalParameter.Precision = 18;
                totalParameter.Scale = 2;
                totalParameter.Value = totalAmount;

                updateCommand.Parameters.Add(
                    "@OrderId", SqlDbType.Int)
                    .Value = orderId;

                updateCommand.ExecuteNonQuery();

                // Save all changes atomically
                transaction.Commit();

                return orderId;
            }
            catch
            {
                // Undo all changes on failure
                transaction.Rollback();
                throw;
            }
        }
    }
}