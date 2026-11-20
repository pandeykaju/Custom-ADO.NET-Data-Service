
IF OBJECT_ID('dbo.OrderItems', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.OrderItems
    (
        OrderItemId INT IDENTITY(1,1) PRIMARY KEY,
        OrderId INT NOT NULL,
        ProductId INT NOT NULL,
        Quantity INT NOT NULL,
        UnitPrice DECIMAL(18,2) NOT NULL,

        CONSTRAINT FK_OrderItems_Orders
            FOREIGN KEY (OrderId)
            REFERENCES dbo.Orders(OrderId),

        CONSTRAINT FK_OrderItems_Products
            FOREIGN KEY (ProductId)
            REFERENCES dbo.Products(ProductId),

        CONSTRAINT CK_OrderItems_Quantity
            CHECK (Quantity > 0)
    );
END;
