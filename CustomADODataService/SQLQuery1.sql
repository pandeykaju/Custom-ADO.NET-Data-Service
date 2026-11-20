CREATE TABLE dbo.Products
(
    ProductId INT IDENTITY(1,1) PRIMARY KEY,
    ProductName NVARCHAR(100) NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    Quantity INT NOT NULL
);
INSERT INTO dbo.Products
    (ProductName, Price, Quantity)
VALUES
    ('Laptop', 55000, 10),
    ('Mouse', 500, 30),
    ('Keyboard', 1200, 20),
    ('Monitor', 12000, 15),
    ('Printer', 8500, 8);
   