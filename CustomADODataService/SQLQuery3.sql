
IF OBJECT_ID('dbo.Orders', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Orders
    (
        OrderId INT IDENTITY(1,1) PRIMARY KEY,
        CustomerName NVARCHAR(100) NOT NULL,
        OrderDate DATETIME2 NOT NULL
            DEFAULT SYSUTCDATETIME(),
        TotalAmount DECIMAL(18,2) NOT NULL
    );
END;
