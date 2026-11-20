
using Microsoft.Data.SqlClient;
using System.Data;
using CustomADODataService.Models;

namespace CustomADODataService.Repository
{
    public class ProductRepository
    {
        private readonly string _connectionString;

        public ProductRepository(IConfiguration configuration)
        {
            _connectionString = configuration
                .GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Database connection string not found.");
        }

        // READ: Get all products
        public List<Product> GetAllProducts()
        {
            var products = new List<Product>();

            using var connection =
                new SqlConnection(_connectionString);

            using var command = new SqlCommand(
                "SELECT ProductId, ProductName, Price, Quantity FROM dbo.Products",
                connection);

            connection.Open();

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                products.Add(new Product
                {
                    ProductId = reader.GetInt32(0),
                    ProductName = reader.GetString(1),
                    Price = reader.GetDecimal(2),
                    Quantity = reader.GetInt32(3)
                });
            }

            return products;
        }

        // READ: Get a product by ID
        public Product? GetProductById(int id)
        {
            using var connection =
                new SqlConnection(_connectionString);

            using var command = new SqlCommand(
                @"SELECT ProductId, ProductName, Price, Quantity
                  FROM dbo.Products
                  WHERE ProductId = @ProductId",
                connection);

            command.Parameters.Add("@ProductId", SqlDbType.Int)
                .Value = id;

            connection.Open();

            using var reader = command.ExecuteReader();

            if (!reader.Read())
                return null;

            return new Product
            {
                ProductId = reader.GetInt32(0),
                ProductName = reader.GetString(1),
                Price = reader.GetDecimal(2),
                Quantity = reader.GetInt32(3)
            };
        }

        // CREATE: Add a new product
        public int AddProduct(Product product)
        {
            using var connection =
                new SqlConnection(_connectionString);

            using var command = new SqlCommand(
                @"INSERT INTO dbo.Products
                    (ProductName, Price, Quantity)
                  OUTPUT INSERTED.ProductId
                  VALUES (@ProductName, @Price, @Quantity)",
                connection);

            AddProductParameters(command, product);

            connection.Open();

            return (int)command.ExecuteScalar()!;
        }

        // UPDATE: Modify an existing product
        public bool UpdateProduct(Product product)
        {
            using var connection =
                new SqlConnection(_connectionString);

            using var command = new SqlCommand(
                @"UPDATE dbo.Products
                  SET ProductName = @ProductName,
                      Price = @Price,
                      Quantity = @Quantity
                  WHERE ProductId = @ProductId",
                connection);

            command.Parameters.Add("@ProductId", SqlDbType.Int)
                .Value = product.ProductId;

            AddProductParameters(command, product);

            connection.Open();

            return command.ExecuteNonQuery() > 0;
        }

        // DELETE: Remove a product
        public bool DeleteProduct(int id)
        {
            using var connection =
                new SqlConnection(_connectionString);

            using var command = new SqlCommand(
                "DELETE FROM dbo.Products WHERE ProductId = @ProductId",
                connection);

            command.Parameters.Add("@ProductId", SqlDbType.Int)
                .Value = id;

            connection.Open();

            return command.ExecuteNonQuery() > 0;
        }

        // Shared parameter configuration
        private static void AddProductParameters(
            SqlCommand command, Product product)
        {
            command.Parameters.Add(
                "@ProductName", SqlDbType.NVarChar, 100)
                .Value = product.ProductName;

            var price = command.Parameters.Add(
                "@Price", SqlDbType.Decimal);

            price.Precision = 18;
            price.Scale = 2;
            price.Value = product.Price;

            command.Parameters.Add(
                "@Quantity", SqlDbType.Int)
                .Value = product.Quantity;
        }

        // DISCONNECTED MODEL: DataSet + SqlDataAdapter
        public DataSet GetProductsDataSet()
        {
            var dataSet = new DataSet();

            using var connection =
                new SqlConnection(_connectionString);

            using var command = new SqlCommand(
                @"SELECT ProductId, ProductName, Price, Quantity
                  FROM dbo.Products",
                connection);

            using var adapter = new SqlDataAdapter(command);

            adapter.Fill(dataSet, "Products");

            return dataSet;
        }
    }
}