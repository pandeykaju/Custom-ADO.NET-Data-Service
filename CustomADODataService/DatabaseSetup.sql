
IF DB_ID(N'CustomADODatabase') IS NULL
BEGIN
    CREATE DATABASE CustomADODatabase;
END
GO

USE CustomADODatabase;
GO

-- Products table
IF OBJECT_ID(N'dbo.Products', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Products
    (
        ProductId INT IDENTITY(1,1) NOT NULL,
        ProductName NVARCHAR(100) NOT NULL,
        Price DECIMAL(18,2) NOT NULL,
        Quantity INT NOT NULL,
        PRIMARY KEY CLUSTERED (ProductId ASC)
    );
END
GO

-- Orders table
IF OBJECT_ID(N'dbo.Orders', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Orders
    (
        OrderId INT IDENTITY(1,1) NOT NULL,
        CustomerName NVARCHAR(100) NOT NULL,
        OrderDate DATETIME2(7)
            DEFAULT (SYSUTCDATETIME()) NOT NULL,
        TotalAmount DECIMAL(18,2) NOT NULL,
        PRIMARY KEY CLUSTERED (OrderId ASC)
    );
END
GO

-- OrderItems table
IF OBJECT_ID(N'dbo.OrderItems', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.OrderItems
    (
        OrderItemId INT IDENTITY(1,1) NOT NULL,
        OrderId INT NOT NULL,
        ProductId INT NOT NULL,
        Quantity INT NOT NULL,
        UnitPrice DECIMAL(18,2) NOT NULL,

        PRIMARY KEY CLUSTERED (OrderItemId ASC),

        CONSTRAINT FK_OrderItems_Orders
            FOREIGN KEY (OrderId)
            REFERENCES dbo.Orders(OrderId),

        CONSTRAINT FK_OrderItems_Products
            FOREIGN KEY (ProductId)
            REFERENCES dbo.Products(ProductId),

        CONSTRAINT CK_OrderItems_Quantity
            CHECK (Quantity > 0)
    );
END
GO