
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 07/12/2016 20:53:55
-- Generated from EDMX file: D:\Projects\VS2013\VAP\VAP\MemberCenter\Model1.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [vap];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_MemberCashTransaction]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CashTransactions] DROP CONSTRAINT [FK_MemberCashTransaction];
GO
IF OBJECT_ID(N'[dbo].[FK_MemberPointTransaction]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PointTransactions] DROP CONSTRAINT [FK_MemberPointTransaction];
GO
IF OBJECT_ID(N'[dbo].[FK_MemberMember]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Members] DROP CONSTRAINT [FK_MemberMember];
GO
IF OBJECT_ID(N'[dbo].[FK_CashTransactionPaymentMethod]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PaymentMethods] DROP CONSTRAINT [FK_CashTransactionPaymentMethod];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Members]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Members];
GO
IF OBJECT_ID(N'[dbo].[CashTransactions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CashTransactions];
GO
IF OBJECT_ID(N'[dbo].[PointTransactions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PointTransactions];
GO
IF OBJECT_ID(N'[dbo].[PaymentMethods]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PaymentMethods];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Members'
CREATE TABLE [dbo].[Members] (
    [Id] int  NOT NULL,
    [Email] nvarchar(50)  NOT NULL,
    [UserName] nvarchar(50)  NOT NULL,
    [RealName] nvarchar(max)  NOT NULL,
    [Password1] nvarchar(50)  NOT NULL,
    [Password2] nvarchar(50)  NOT NULL,
    [Password3] nvarchar(50)  NOT NULL,
    [Cash1] decimal(18,0)  NOT NULL,
    [Cash2] decimal(18,0)  NOT NULL,
    [Point1] decimal(18,0)  NOT NULL,
    [Point2] decimal(18,0)  NOT NULL,
    [ChongXiao1] decimal(18,0)  NOT NULL,
    [ChongXiao2] decimal(18,0)  NOT NULL,
    [Coin1] decimal(18,0)  NOT NULL,
    [Coin2] decimal(18,0)  NOT NULL,
    [RegisterTime] datetime  NOT NULL,
    [Level] nvarchar(max)  NOT NULL,
    [Status] nvarchar(max)  NOT NULL,
    [TiXianStatus] nvarchar(max)  NOT NULL,
    [TiBiStatus] nvarchar(max)  NOT NULL,
    [IdSubmitted] bit  NOT NULL,
    [IdApproved] bit  NOT NULL,
    [Referral_Id] int  NOT NULL
);
GO

-- Creating table 'CashTransactions'
CREATE TABLE [dbo].[CashTransactions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [MemberId] int  NOT NULL,
    [Amount] decimal(18,0)  NOT NULL,
    [DateTime] time  NOT NULL,
    [Type] smallint  NOT NULL,
    [Status] smallint  NOT NULL
);
GO

-- Creating table 'PointTransactions'
CREATE TABLE [dbo].[PointTransactions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [MemberId] int  NOT NULL,
    [Amount] decimal(18,0)  NOT NULL,
    [DateTime] time  NOT NULL,
    [Type] smallint  NOT NULL,
    [Status] smallint  NOT NULL
);
GO

-- Creating table 'PaymentMethods'
CREATE TABLE [dbo].[PaymentMethods] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Bank] nvarchar(max)  NOT NULL,
    [Account] nvarchar(max)  NOT NULL,
    [Description] nvarchar(max)  NOT NULL,
    [CashTransaction_Id] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Members'
ALTER TABLE [dbo].[Members]
ADD CONSTRAINT [PK_Members]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CashTransactions'
ALTER TABLE [dbo].[CashTransactions]
ADD CONSTRAINT [PK_CashTransactions]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'PointTransactions'
ALTER TABLE [dbo].[PointTransactions]
ADD CONSTRAINT [PK_PointTransactions]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'PaymentMethods'
ALTER TABLE [dbo].[PaymentMethods]
ADD CONSTRAINT [PK_PaymentMethods]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [MemberId] in table 'CashTransactions'
ALTER TABLE [dbo].[CashTransactions]
ADD CONSTRAINT [FK_MemberCashTransaction]
    FOREIGN KEY ([MemberId])
    REFERENCES [dbo].[Members]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_MemberCashTransaction'
CREATE INDEX [IX_FK_MemberCashTransaction]
ON [dbo].[CashTransactions]
    ([MemberId]);
GO

-- Creating foreign key on [MemberId] in table 'PointTransactions'
ALTER TABLE [dbo].[PointTransactions]
ADD CONSTRAINT [FK_MemberPointTransaction]
    FOREIGN KEY ([MemberId])
    REFERENCES [dbo].[Members]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_MemberPointTransaction'
CREATE INDEX [IX_FK_MemberPointTransaction]
ON [dbo].[PointTransactions]
    ([MemberId]);
GO

-- Creating foreign key on [Referral_Id] in table 'Members'
ALTER TABLE [dbo].[Members]
ADD CONSTRAINT [FK_MemberMember]
    FOREIGN KEY ([Referral_Id])
    REFERENCES [dbo].[Members]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_MemberMember'
CREATE INDEX [IX_FK_MemberMember]
ON [dbo].[Members]
    ([Referral_Id]);
GO

-- Creating foreign key on [CashTransaction_Id] in table 'PaymentMethods'
ALTER TABLE [dbo].[PaymentMethods]
ADD CONSTRAINT [FK_CashTransactionPaymentMethod]
    FOREIGN KEY ([CashTransaction_Id])
    REFERENCES [dbo].[CashTransactions]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_CashTransactionPaymentMethod'
CREATE INDEX [IX_FK_CashTransactionPaymentMethod]
ON [dbo].[PaymentMethods]
    ([CashTransaction_Id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------