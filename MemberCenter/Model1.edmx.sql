
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 07/20/2016 13:22:08
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
IF OBJECT_ID(N'[dbo].[FK_PaymentMethodCashTransaction]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CashTransactions] DROP CONSTRAINT [FK_PaymentMethodCashTransaction];
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
    [PaymentMethod_Id] int  NOT NULL,
    [BankInfo_Id] int  NOT NULL
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
    [Price] decimal(18,0)  NOT NULL,
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

-- Creating table 'CoinPrices'
CREATE TABLE [dbo].[CoinPrices] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Price] decimal(18,0)  NOT NULL,
    [DateTime] datetime  NOT NULL
);
GO

-- Creating table 'SystemSettings'
CREATE TABLE [dbo].[SystemSettings] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Key] nvarchar(max)  NOT NULL,
    [Value] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'RefoundRates'
CREATE TABLE [dbo].[RefoundRates] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [MemberLevel] nvarchar(max)  NOT NULL,
    [Rate] decimal(18,0)  NOT NULL
);
GO

-- Creating table 'LockedCoins'
CREATE TABLE [dbo].[LockedCoins] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [LastPrice] decimal(18,0)  NOT NULL,
    [NexPrice] decimal(18,0)  NOT NULL,
    [Price] decimal(18,0)  NOT NULL,
    [TotalAmount] decimal(18,0)  NOT NULL,
    [LockedAmount] decimal(18,0)  NOT NULL,
    [AvailabeAmount] decimal(18,0)  NOT NULL,
    [MemberId] int  NOT NULL
);
GO

-- Creating table 'BankInfos'
CREATE TABLE [dbo].[BankInfos] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Bank] nvarchar(max)  NOT NULL,
    [Account] nvarchar(max)  NOT NULL,
    [Description] nvarchar(max)  NOT NULL,
    [MemberId] int  NULL
);
GO

-- Creating table 'News'
CREATE TABLE [dbo].[News] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Type] nvarchar(max)  NOT NULL,
    [Title] nvarchar(max)  NOT NULL,
    [Content] nvarchar(max)  NOT NULL,
    [DateTime] datetime  NOT NULL,
    [Status] nvarchar(max)  NOT NULL,
    [CreatedBy] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'IPLogs'
CREATE TABLE [dbo].[IPLogs] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [IP] nvarchar(max)  NOT NULL,
    [DateTime] nvarchar(max)  NOT NULL,
    [MemberId] int  NOT NULL
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

-- Creating primary key on [Id] in table 'CoinPrices'
ALTER TABLE [dbo].[CoinPrices]
ADD CONSTRAINT [PK_CoinPrices]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SystemSettings'
ALTER TABLE [dbo].[SystemSettings]
ADD CONSTRAINT [PK_SystemSettings]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'RefoundRates'
ALTER TABLE [dbo].[RefoundRates]
ADD CONSTRAINT [PK_RefoundRates]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'LockedCoins'
ALTER TABLE [dbo].[LockedCoins]
ADD CONSTRAINT [PK_LockedCoins]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'BankInfos'
ALTER TABLE [dbo].[BankInfos]
ADD CONSTRAINT [PK_BankInfos]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'News'
ALTER TABLE [dbo].[News]
ADD CONSTRAINT [PK_News]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'IPLogs'
ALTER TABLE [dbo].[IPLogs]
ADD CONSTRAINT [PK_IPLogs]
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

-- Creating foreign key on [MemberId] in table 'LockedCoins'
ALTER TABLE [dbo].[LockedCoins]
ADD CONSTRAINT [FK_MemberLockedCoin]
    FOREIGN KEY ([MemberId])
    REFERENCES [dbo].[Members]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_MemberLockedCoin'
CREATE INDEX [IX_FK_MemberLockedCoin]
ON [dbo].[LockedCoins]
    ([MemberId]);
GO

-- Creating foreign key on [MemberId] in table 'BankInfos'
ALTER TABLE [dbo].[BankInfos]
ADD CONSTRAINT [FK_MemberBankInfo]
    FOREIGN KEY ([MemberId])
    REFERENCES [dbo].[Members]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_MemberBankInfo'
CREATE INDEX [IX_FK_MemberBankInfo]
ON [dbo].[BankInfos]
    ([MemberId]);
GO

-- Creating foreign key on [BankInfo_Id] in table 'CashTransactions'
ALTER TABLE [dbo].[CashTransactions]
ADD CONSTRAINT [FK_CashTransactionBankInfo]
    FOREIGN KEY ([BankInfo_Id])
    REFERENCES [dbo].[BankInfos]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_CashTransactionBankInfo'
CREATE INDEX [IX_FK_CashTransactionBankInfo]
ON [dbo].[CashTransactions]
    ([BankInfo_Id]);
GO

-- Creating foreign key on [MemberId] in table 'IPLogs'
ALTER TABLE [dbo].[IPLogs]
ADD CONSTRAINT [FK_MemberIPLog]
    FOREIGN KEY ([MemberId])
    REFERENCES [dbo].[Members]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_MemberIPLog'
CREATE INDEX [IX_FK_MemberIPLog]
ON [dbo].[IPLogs]
    ([MemberId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------