
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 11/17/2025 19:36:56
-- Generated from EDMX file: C:\Users\ASUS\source\repos\Web Banh bao 4 chang linh ngu lam\Web Banh bao 4 chang linh ngu lam\Models\DBBanhBaoTuanDatModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [DBBanhBaoTuanDat];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_Cart_Customer]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Cart] DROP CONSTRAINT [FK_Cart_Customer];
GO
IF OBJECT_ID(N'[dbo].[FK_Cart_Product]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Cart] DROP CONSTRAINT [FK_Cart_Product];
GO
IF OBJECT_ID(N'[dbo].[FK_OrderDetail_Order]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[OrderDetail] DROP CONSTRAINT [FK_OrderDetail_Order];
GO
IF OBJECT_ID(N'[dbo].[FK_OrderDetail_Product]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[OrderDetail] DROP CONSTRAINT [FK_OrderDetail_Product];
GO
IF OBJECT_ID(N'[dbo].[FK_OrderPro_Customer]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[OrderPro] DROP CONSTRAINT [FK_OrderPro_Customer];
GO
IF OBJECT_ID(N'[dbo].[FK_Product_Category]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Product] DROP CONSTRAINT [FK_Product_Category];
GO
IF OBJECT_ID(N'[dbo].[FK_Review_Customer]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Review] DROP CONSTRAINT [FK_Review_Customer];
GO
IF OBJECT_ID(N'[dbo].[FK_Review_Product]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Review] DROP CONSTRAINT [FK_Review_Product];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[AdminUser]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AdminUser];
GO
IF OBJECT_ID(N'[dbo].[Cart]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Cart];
GO
IF OBJECT_ID(N'[dbo].[Category]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Category];
GO
IF OBJECT_ID(N'[dbo].[Customer]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Customer];
GO
IF OBJECT_ID(N'[dbo].[OrderDetail]', 'U') IS NOT NULL
    DROP TABLE [dbo].[OrderDetail];
GO
IF OBJECT_ID(N'[dbo].[OrderPro]', 'U') IS NOT NULL
    DROP TABLE [dbo].[OrderPro];
GO
IF OBJECT_ID(N'[dbo].[Product]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Product];
GO
IF OBJECT_ID(N'[dbo].[Review]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Review];
GO
IF OBJECT_ID(N'[dbo].[sysdiagrams]', 'U') IS NOT NULL
    DROP TABLE [dbo].[sysdiagrams];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'AdminUsers'
CREATE TABLE [dbo].[AdminUsers] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [UserName] nvarchar(100)  NOT NULL,
    [PasswordHash] nvarchar(255)  NOT NULL,
    [Role] nvarchar(50)  NULL
);
GO

-- Creating table 'Carts'
CREATE TABLE [dbo].[Carts] (
    [CartID] int IDENTITY(1,1) NOT NULL,
    [CustomerID] int  NULL,
    [ProductID] int  NULL,
    [Quantity] int  NULL,
    [AddedDate] datetime  NULL
);
GO

-- Creating table 'Categories'
CREATE TABLE [dbo].[Categories] (
    [IDCate] int IDENTITY(1,1) NOT NULL,
    [CodeCate] nchar(20)  NOT NULL,
    [NameCate] nvarchar(100)  NOT NULL,
    [Description] nvarchar(max)  NULL
);
GO

-- Creating table 'Customers'
CREATE TABLE [dbo].[Customers] (
    [IDCus] int IDENTITY(1,1) NOT NULL,
    [NameCus] nvarchar(255)  NOT NULL,
    [PhoneCus] nvarchar(15)  NULL,
    [EmailCus] nvarchar(255)  NULL,
    [AddressCus] nvarchar(255)  NULL,
    [City] nvarchar(100)  NULL,
    [District] nvarchar(100)  NULL,
    [SavedPaymentMethodType] nvarchar(50)  NULL,
    [SavedCardLast4] nvarchar(10)  NULL,
    [SavePaymentMethod] bit  NULL
);
GO

-- Creating table 'OrderDetails'
CREATE TABLE [dbo].[OrderDetails] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [IDOrder] int  NULL,
    [IDProduct] int  NULL,
    [Quantity] int  NULL,
    [UnitPrice] decimal(18,2)  NULL
);
GO

-- Creating table 'OrderProes'
CREATE TABLE [dbo].[OrderProes] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [DateOrder] datetime  NULL,
    [IDCus] int  NULL,
    [DeliveryAddress] nvarchar(255)  NULL,
    [PaymentMethod] nvarchar(50)  NULL,
    [TotalAmount] decimal(18,2)  NULL,
    [Status] nvarchar(50)  NULL
);
GO

-- Creating table 'Products'
CREATE TABLE [dbo].[Products] (
    [ProductID] int IDENTITY(1,1) NOT NULL,
    [NamePro] nvarchar(255)  NOT NULL,
    [DescriptionPro] nvarchar(max)  NULL,
    [Category] int  NULL,
    [Price] decimal(18,2)  NOT NULL,
    [ImagePro] nvarchar(255)  NULL,
    [IsFeatured] bit  NULL
);
GO

-- Creating table 'sysdiagrams'
CREATE TABLE [dbo].[sysdiagrams] (
    [name] nvarchar(128)  NOT NULL,
    [principal_id] int  NOT NULL,
    [diagram_id] int IDENTITY(1,1) NOT NULL,
    [version] int  NULL,
    [definition] varbinary(max)  NULL
);
GO

-- Creating table 'Reviews'
CREATE TABLE [dbo].[Reviews] (
    [ReviewID] int IDENTITY(1,1) NOT NULL,
    [ProductID] int  NOT NULL,
    [CustomerID] int  NULL,
    [Rating] int  NOT NULL,
    [Comment] nvarchar(max)  NULL,
    [CreatedAt] datetime  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [ID] in table 'AdminUsers'
ALTER TABLE [dbo].[AdminUsers]
ADD CONSTRAINT [PK_AdminUsers]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [CartID] in table 'Carts'
ALTER TABLE [dbo].[Carts]
ADD CONSTRAINT [PK_Carts]
    PRIMARY KEY CLUSTERED ([CartID] ASC);
GO

-- Creating primary key on [IDCate] in table 'Categories'
ALTER TABLE [dbo].[Categories]
ADD CONSTRAINT [PK_Categories]
    PRIMARY KEY CLUSTERED ([IDCate] ASC);
GO

-- Creating primary key on [IDCus] in table 'Customers'
ALTER TABLE [dbo].[Customers]
ADD CONSTRAINT [PK_Customers]
    PRIMARY KEY CLUSTERED ([IDCus] ASC);
GO

-- Creating primary key on [ID] in table 'OrderDetails'
ALTER TABLE [dbo].[OrderDetails]
ADD CONSTRAINT [PK_OrderDetails]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'OrderProes'
ALTER TABLE [dbo].[OrderProes]
ADD CONSTRAINT [PK_OrderProes]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ProductID] in table 'Products'
ALTER TABLE [dbo].[Products]
ADD CONSTRAINT [PK_Products]
    PRIMARY KEY CLUSTERED ([ProductID] ASC);
GO

-- Creating primary key on [diagram_id] in table 'sysdiagrams'
ALTER TABLE [dbo].[sysdiagrams]
ADD CONSTRAINT [PK_sysdiagrams]
    PRIMARY KEY CLUSTERED ([diagram_id] ASC);
GO

-- Creating primary key on [ReviewID] in table 'Reviews'
ALTER TABLE [dbo].[Reviews]
ADD CONSTRAINT [PK_Reviews]
    PRIMARY KEY CLUSTERED ([ReviewID] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [CustomerID] in table 'Carts'
ALTER TABLE [dbo].[Carts]
ADD CONSTRAINT [FK_Cart_Customer]
    FOREIGN KEY ([CustomerID])
    REFERENCES [dbo].[Customers]
        ([IDCus])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Cart_Customer'
CREATE INDEX [IX_FK_Cart_Customer]
ON [dbo].[Carts]
    ([CustomerID]);
GO

-- Creating foreign key on [ProductID] in table 'Carts'
ALTER TABLE [dbo].[Carts]
ADD CONSTRAINT [FK_Cart_Product]
    FOREIGN KEY ([ProductID])
    REFERENCES [dbo].[Products]
        ([ProductID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Cart_Product'
CREATE INDEX [IX_FK_Cart_Product]
ON [dbo].[Carts]
    ([ProductID]);
GO

-- Creating foreign key on [Category] in table 'Products'
ALTER TABLE [dbo].[Products]
ADD CONSTRAINT [FK_Product_Category]
    FOREIGN KEY ([Category])
    REFERENCES [dbo].[Categories]
        ([IDCate])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Product_Category'
CREATE INDEX [IX_FK_Product_Category]
ON [dbo].[Products]
    ([Category]);
GO

-- Creating foreign key on [IDCus] in table 'OrderProes'
ALTER TABLE [dbo].[OrderProes]
ADD CONSTRAINT [FK_OrderPro_Customer]
    FOREIGN KEY ([IDCus])
    REFERENCES [dbo].[Customers]
        ([IDCus])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_OrderPro_Customer'
CREATE INDEX [IX_FK_OrderPro_Customer]
ON [dbo].[OrderProes]
    ([IDCus]);
GO

-- Creating foreign key on [IDOrder] in table 'OrderDetails'
ALTER TABLE [dbo].[OrderDetails]
ADD CONSTRAINT [FK_OrderDetail_Order]
    FOREIGN KEY ([IDOrder])
    REFERENCES [dbo].[OrderProes]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_OrderDetail_Order'
CREATE INDEX [IX_FK_OrderDetail_Order]
ON [dbo].[OrderDetails]
    ([IDOrder]);
GO

-- Creating foreign key on [IDProduct] in table 'OrderDetails'
ALTER TABLE [dbo].[OrderDetails]
ADD CONSTRAINT [FK_OrderDetail_Product]
    FOREIGN KEY ([IDProduct])
    REFERENCES [dbo].[Products]
        ([ProductID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_OrderDetail_Product'
CREATE INDEX [IX_FK_OrderDetail_Product]
ON [dbo].[OrderDetails]
    ([IDProduct]);
GO

-- Creating foreign key on [CustomerID] in table 'Reviews'
ALTER TABLE [dbo].[Reviews]
ADD CONSTRAINT [FK_Review_Customer]
    FOREIGN KEY ([CustomerID])
    REFERENCES [dbo].[Customers]
        ([IDCus])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Review_Customer'
CREATE INDEX [IX_FK_Review_Customer]
ON [dbo].[Reviews]
    ([CustomerID]);
GO

-- Creating foreign key on [ProductID] in table 'Reviews'
ALTER TABLE [dbo].[Reviews]
ADD CONSTRAINT [FK_Review_Product]
    FOREIGN KEY ([ProductID])
    REFERENCES [dbo].[Products]
        ([ProductID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Review_Product'
CREATE INDEX [IX_FK_Review_Product]
ON [dbo].[Reviews]
    ([ProductID]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------