
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 07/17/2016 15:37:44
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
IF OBJECT_ID(N'[dbo].[FK_MemberBaoDanTransaction]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CoinTransactions] DROP CONSTRAINT [FK_MemberBaoDanTransaction];
GO
IF OBJECT_ID(N'[dbo].[FK_MemberChongXiaoTransaction]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ChongXiaoTransactions] DROP CONSTRAINT [FK_MemberChongXiaoTransaction];
GO
IF OBJECT_ID(N'[dbo].[FK_PointTransactionBaoDanTransaction]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CoinTransactions] DROP CONSTRAINT [FK_PointTransactionBaoDanTransaction];
GO
IF OBJECT_ID(N'[dbo].[FK_ChongXiaoTransactionBaoDanTransaction]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CoinTransactions] DROP CONSTRAINT [FK_ChongXiaoTransactionBaoDanTransaction];
GO
IF OBJECT_ID(N'[dbo].[FK_CashTransactionBaoDanTransaction]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CoinTransactions] DROP CONSTRAINT [FK_CashTransactionBaoDanTransaction];
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
IF OBJECT_ID(N'[dbo].[CoinTransactions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CoinTransactions];
GO
IF OBJECT_ID(N'[dbo].[ChongXiaoTransactions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ChongXiaoTransactions];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Members'
CREATE TABLE [dbo].[Members] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Email] nvarchar(50)  NOT NULL,
    [UserName] nvarchar(50)  NULL,
    [RealName] nvarchar(max)  NULL,
    [Password1] nvarchar(50)  NOT NULL,
    [Password2] nvarchar(50)  NULL,
    [Password3] nvarchar(50)  NULL,
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
    [Achievement] decimal(18,0)  NOT NULL,
    [Status] nvarchar(max)  NOT NULL,
    [TiXianStatus] nvarchar(max)  NULL,
    [TiBiStatus] nvarchar(max)  NULL,
    [IdSubmitted] bit  NULL,
    [IdApproved] bit  NULL,
    [ApprovedBy] nvarchar(max)  NULL,
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
    [Status] smallint  NOT NULL,
    [PaymentMethod_Id] int  NOT NULL
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
    [Description] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'CoinTransactions'
CREATE TABLE [dbo].[CoinTransactions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [DateTime] time  NOT NULL,
    [Amount] decimal(18,0)  NOT NULL,
    [LockedAmount] decimal(18,0)  NOT NULL,
    [FreeAmount] decimal(18,0)  NOT NULL,
    [Price] decimal(18,0)  NOT NULL,
    [LastPrice] decimal(18,0)  NOT NULL,
    [NextPrice] decimal(18,0)  NOT NULL,
    [Type] smallint  NOT NULL,
    [Status] smallint  NOT NULL,
    [MemberId] int  NOT NULL,
    [Fee] decimal(18,0)  NOT NULL,
    [PointTransaction_Id] int  NOT NULL,
    [ChongXiaoTransaction_Id] int  NOT NULL,
    [CashTransaction_Id] int  NOT NULL
);
GO

-- Creating table 'ChongXiaoTransactions'
CREATE TABLE [dbo].[ChongXiaoTransactions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [DateTime] time  NOT NULL,
    [Amount] decimal(18,0)  NOT NULL,
    [MemberId] int  NOT NULL,
    [Type] smallint  NOT NULL,
    [Status] smallint  NOT NULL
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

-- Creating primary key on [Id] in table 'CoinTransactions'
ALTER TABLE [dbo].[CoinTransactions]
ADD CONSTRAINT [PK_CoinTransactions]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ChongXiaoTransactions'
ALTER TABLE [dbo].[ChongXiaoTransactions]
ADD CONSTRAINT [PK_ChongXiaoTransactions]
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

-- Creating foreign key on [MemberId] in table 'CoinTransactions'
ALTER TABLE [dbo].[CoinTransactions]
ADD CONSTRAINT [FK_MemberBaoDanTransaction]
    FOREIGN KEY ([MemberId])
    REFERENCES [dbo].[Members]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_MemberBaoDanTransaction'
CREATE INDEX [IX_FK_MemberBaoDanTransaction]
ON [dbo].[CoinTransactions]
    ([MemberId]);
GO

-- Creating foreign key on [MemberId] in table 'ChongXiaoTransactions'
ALTER TABLE [dbo].[ChongXiaoTransactions]
ADD CONSTRAINT [FK_MemberChongXiaoTransaction]
    FOREIGN KEY ([MemberId])
    REFERENCES [dbo].[Members]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_MemberChongXiaoTransaction'
CREATE INDEX [IX_FK_MemberChongXiaoTransaction]
ON [dbo].[ChongXiaoTransactions]
    ([MemberId]);
GO

-- Creating foreign key on [PointTransaction_Id] in table 'CoinTransactions'
ALTER TABLE [dbo].[CoinTransactions]
ADD CONSTRAINT [FK_PointTransactionBaoDanTransaction]
    FOREIGN KEY ([PointTransaction_Id])
    REFERENCES [dbo].[PointTransactions]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_PointTransactionBaoDanTransaction'
CREATE INDEX [IX_FK_PointTransactionBaoDanTransaction]
ON [dbo].[CoinTransactions]
    ([PointTransaction_Id]);
GO

-- Creating foreign key on [ChongXiaoTransaction_Id] in table 'CoinTransactions'
ALTER TABLE [dbo].[CoinTransactions]
ADD CONSTRAINT [FK_ChongXiaoTransactionBaoDanTransaction]
    FOREIGN KEY ([ChongXiaoTransaction_Id])
    REFERENCES [dbo].[ChongXiaoTransactions]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ChongXiaoTransactionBaoDanTransaction'
CREATE INDEX [IX_FK_ChongXiaoTransactionBaoDanTransaction]
ON [dbo].[CoinTransactions]
    ([ChongXiaoTransaction_Id]);
GO

-- Creating foreign key on [CashTransaction_Id] in table 'CoinTransactions'
ALTER TABLE [dbo].[CoinTransactions]
ADD CONSTRAINT [FK_CashTransactionBaoDanTransaction]
    FOREIGN KEY ([CashTransaction_Id])
    REFERENCES [dbo].[CashTransactions]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_CashTransactionBaoDanTransaction'
CREATE INDEX [IX_FK_CashTransactionBaoDanTransaction]
ON [dbo].[CoinTransactions]
    ([CashTransaction_Id]);
GO

-- Creating foreign key on [PaymentMethod_Id] in table 'CashTransactions'
ALTER TABLE [dbo].[CashTransactions]
ADD CONSTRAINT [FK_PaymentMethodCashTransaction]
    FOREIGN KEY ([PaymentMethod_Id])
    REFERENCES [dbo].[PaymentMethods]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_PaymentMethodCashTransaction'
CREATE INDEX [IX_FK_PaymentMethodCashTransaction]
ON [dbo].[CashTransactions]
    ([PaymentMethod_Id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------