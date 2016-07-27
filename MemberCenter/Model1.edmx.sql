
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 07/27/2016 15:51:30
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
    ALTER TABLE [dbo].[BaoDanTransactions] DROP CONSTRAINT [FK_MemberBaoDanTransaction];
GO
IF OBJECT_ID(N'[dbo].[FK_MemberChongXiaoTransaction]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ChongXiaoTransactions] DROP CONSTRAINT [FK_MemberChongXiaoTransaction];
GO
IF OBJECT_ID(N'[dbo].[FK_MemberLockedCoin]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[LockedCoins] DROP CONSTRAINT [FK_MemberLockedCoin];
GO
IF OBJECT_ID(N'[dbo].[FK_MemberIPLog]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[IPLogs] DROP CONSTRAINT [FK_MemberIPLog];
GO
IF OBJECT_ID(N'[dbo].[FK_MemberMemberLevel]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Members] DROP CONSTRAINT [FK_MemberMemberLevel];
GO
IF OBJECT_ID(N'[dbo].[FK_BaoDanTransactionChongXiaoTransaction]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ChongXiaoTransactions] DROP CONSTRAINT [FK_BaoDanTransactionChongXiaoTransaction];
GO
IF OBJECT_ID(N'[dbo].[FK_BaoDanTransactionCashTransaction]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CashTransactions] DROP CONSTRAINT [FK_BaoDanTransactionCashTransaction];
GO
IF OBJECT_ID(N'[dbo].[FK_BaoDanTransactionPointTransaction]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PointTransactions] DROP CONSTRAINT [FK_BaoDanTransactionPointTransaction];
GO
IF OBJECT_ID(N'[dbo].[FK_BaoDanTransactionLockedCoin]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[LockedCoins] DROP CONSTRAINT [FK_BaoDanTransactionLockedCoin];
GO
IF OBJECT_ID(N'[dbo].[FK_MemberBankInfo]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[BankInfo] DROP CONSTRAINT [FK_MemberBankInfo];
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
IF OBJECT_ID(N'[dbo].[BaoDanTransactions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BaoDanTransactions];
GO
IF OBJECT_ID(N'[dbo].[ChongXiaoTransactions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ChongXiaoTransactions];
GO
IF OBJECT_ID(N'[dbo].[CoinPrices]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CoinPrices];
GO
IF OBJECT_ID(N'[dbo].[SystemSettings]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SystemSettings];
GO
IF OBJECT_ID(N'[dbo].[MemberLevel]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MemberLevel];
GO
IF OBJECT_ID(N'[dbo].[LockedCoins]', 'U') IS NOT NULL
    DROP TABLE [dbo].[LockedCoins];
GO
IF OBJECT_ID(N'[dbo].[BankInfo]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BankInfo];
GO
IF OBJECT_ID(N'[dbo].[News]', 'U') IS NOT NULL
    DROP TABLE [dbo].[News];
GO
IF OBJECT_ID(N'[dbo].[IPLogs]', 'U') IS NOT NULL
    DROP TABLE [dbo].[IPLogs];
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
    [Cash1] decimal(18,2)  NOT NULL,
    [Cash2] decimal(18,2)  NOT NULL,
    [Point1] decimal(18,2)  NOT NULL,
    [Point2] decimal(18,2)  NOT NULL,
    [ChongXiao1] decimal(18,2)  NOT NULL,
    [ChongXiao2] decimal(18,2)  NOT NULL,
    [Coin1] decimal(18,6)  NOT NULL,
    [Coin2] decimal(18,6)  NOT NULL,
    [RegisterTime] datetime  NOT NULL,
    [Achievement] decimal(18,2)  NOT NULL,
    [Status] nvarchar(max)  NOT NULL,
    [TiXianStatus] nvarchar(max)  NULL,
    [TiBiStatus] nvarchar(max)  NULL,
    [IdSubmitted] bit  NULL,
    [IdApproved] bit  NULL,
    [ApprovedBy] nvarchar(max)  NULL,
    [Referral_Id] int  NULL,
    [MemberLevel_Id] int  NOT NULL
);
GO

-- Creating table 'CashTransactions'
CREATE TABLE [dbo].[CashTransactions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [MemberId] int  NOT NULL,
    [Fee] decimal(18,2)  NOT NULL,
    [Amount] decimal(18,2)  NOT NULL,
    [DateTime] datetime  NOT NULL,
    [Type] nvarchar(max)  NOT NULL,
    [Status] nvarchar(max)  NOT NULL,
    [BaoDanTransactionId] int  NULL,
    [Bank] nvarchar(max)  NULL,
    [BankName] nvarchar(max)  NULL,
    [BankAccount] nvarchar(max)  NULL,
    [Comment] nvarchar(max)  NULL,
    [FileUrl] nvarchar(max)  NULL
);
GO

-- Creating table 'PointTransactions'
CREATE TABLE [dbo].[PointTransactions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [MemberId] int  NOT NULL,
    [Amount] decimal(18,2)  NOT NULL,
    [DateTime] datetime  NOT NULL,
    [Type] nvarchar(max)  NOT NULL,
    [Status] nvarchar(max)  NOT NULL,
    [BaoDanTransactionId] int  NULL
);
GO

-- Creating table 'BaoDanTransactions'
CREATE TABLE [dbo].[BaoDanTransactions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [DateTime] datetime  NOT NULL,
    [Amount] decimal(18,6)  NOT NULL,
    [Price] decimal(18,2)  NOT NULL,
    [Type] nvarchar(max)  NOT NULL,
    [Status] nvarchar(max)  NOT NULL,
    [MemberId] int  NOT NULL,
    [Fee] decimal(18,2)  NOT NULL
);
GO

-- Creating table 'ChongXiaoTransactions'
CREATE TABLE [dbo].[ChongXiaoTransactions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [DateTime] datetime  NOT NULL,
    [Amount] decimal(18,2)  NOT NULL,
    [MemberId] int  NOT NULL,
    [Type] nvarchar(max)  NOT NULL,
    [Status] nvarchar(max)  NOT NULL,
    [BaoDanTransactionId] int  NULL
);
GO

-- Creating table 'CoinPrices'
CREATE TABLE [dbo].[CoinPrices] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Price] decimal(18,2)  NOT NULL,
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

-- Creating table 'MemberLevel'
CREATE TABLE [dbo].[MemberLevel] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Level] nvarchar(max)  NOT NULL,
    [RefundRate] decimal(18,2)  NOT NULL,
    [MemberCount] int  NOT NULL,
    [MemberAchievement] decimal(18,2)  NOT NULL,
    [Achievement] decimal(18,2)  NOT NULL
);
GO

-- Creating table 'LockedCoins'
CREATE TABLE [dbo].[LockedCoins] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [LastPrice] decimal(18,2)  NOT NULL,
    [NextPrice] decimal(18,2)  NOT NULL,
    [Price] decimal(18,2)  NOT NULL,
    [TotalAmount] decimal(18,6)  NOT NULL,
    [LockedAmount] decimal(18,6)  NOT NULL,
    [AvailabeAmount] decimal(18,6)  NOT NULL,
    [MemberId] int  NOT NULL,
    [BaoDanTransaction_Id] int  NOT NULL
);
GO

-- Creating table 'BankInfo'
CREATE TABLE [dbo].[BankInfo] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Bank] nvarchar(max)  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Account] nvarchar(max)  NOT NULL,
    [Description] nvarchar(max)  NOT NULL,
    [MemberId] int  NULL,
    [URL] nvarchar(max)  NOT NULL,
    [Type] nvarchar(max)  NOT NULL,
    [Member_Id] int  NULL
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
    [Client] nvarchar(max)  NOT NULL,
    [DateTime] datetime  NOT NULL,
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

-- Creating primary key on [Id] in table 'BaoDanTransactions'
ALTER TABLE [dbo].[BaoDanTransactions]
ADD CONSTRAINT [PK_BaoDanTransactions]
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

-- Creating primary key on [Id] in table 'MemberLevel'
ALTER TABLE [dbo].[MemberLevel]
ADD CONSTRAINT [PK_MemberLevel]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'LockedCoins'
ALTER TABLE [dbo].[LockedCoins]
ADD CONSTRAINT [PK_LockedCoins]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'BankInfo'
ALTER TABLE [dbo].[BankInfo]
ADD CONSTRAINT [PK_BankInfo]
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

-- Creating foreign key on [MemberId] in table 'BaoDanTransactions'
ALTER TABLE [dbo].[BaoDanTransactions]
ADD CONSTRAINT [FK_MemberBaoDanTransaction]
    FOREIGN KEY ([MemberId])
    REFERENCES [dbo].[Members]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_MemberBaoDanTransaction'
CREATE INDEX [IX_FK_MemberBaoDanTransaction]
ON [dbo].[BaoDanTransactions]
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

-- Creating foreign key on [MemberLevel_Id] in table 'Members'
ALTER TABLE [dbo].[Members]
ADD CONSTRAINT [FK_MemberMemberLevel]
    FOREIGN KEY ([MemberLevel_Id])
    REFERENCES [dbo].[MemberLevel]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_MemberMemberLevel'
CREATE INDEX [IX_FK_MemberMemberLevel]
ON [dbo].[Members]
    ([MemberLevel_Id]);
GO

-- Creating foreign key on [BaoDanTransactionId] in table 'ChongXiaoTransactions'
ALTER TABLE [dbo].[ChongXiaoTransactions]
ADD CONSTRAINT [FK_BaoDanTransactionChongXiaoTransaction]
    FOREIGN KEY ([BaoDanTransactionId])
    REFERENCES [dbo].[BaoDanTransactions]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_BaoDanTransactionChongXiaoTransaction'
CREATE INDEX [IX_FK_BaoDanTransactionChongXiaoTransaction]
ON [dbo].[ChongXiaoTransactions]
    ([BaoDanTransactionId]);
GO

-- Creating foreign key on [BaoDanTransactionId] in table 'CashTransactions'
ALTER TABLE [dbo].[CashTransactions]
ADD CONSTRAINT [FK_BaoDanTransactionCashTransaction]
    FOREIGN KEY ([BaoDanTransactionId])
    REFERENCES [dbo].[BaoDanTransactions]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_BaoDanTransactionCashTransaction'
CREATE INDEX [IX_FK_BaoDanTransactionCashTransaction]
ON [dbo].[CashTransactions]
    ([BaoDanTransactionId]);
GO

-- Creating foreign key on [BaoDanTransactionId] in table 'PointTransactions'
ALTER TABLE [dbo].[PointTransactions]
ADD CONSTRAINT [FK_BaoDanTransactionPointTransaction]
    FOREIGN KEY ([BaoDanTransactionId])
    REFERENCES [dbo].[BaoDanTransactions]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_BaoDanTransactionPointTransaction'
CREATE INDEX [IX_FK_BaoDanTransactionPointTransaction]
ON [dbo].[PointTransactions]
    ([BaoDanTransactionId]);
GO

-- Creating foreign key on [BaoDanTransaction_Id] in table 'LockedCoins'
ALTER TABLE [dbo].[LockedCoins]
ADD CONSTRAINT [FK_BaoDanTransactionLockedCoin]
    FOREIGN KEY ([BaoDanTransaction_Id])
    REFERENCES [dbo].[BaoDanTransactions]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_BaoDanTransactionLockedCoin'
CREATE INDEX [IX_FK_BaoDanTransactionLockedCoin]
ON [dbo].[LockedCoins]
    ([BaoDanTransaction_Id]);
GO

-- Creating foreign key on [Member_Id] in table 'BankInfo'
ALTER TABLE [dbo].[BankInfo]
ADD CONSTRAINT [FK_MemberBankInfo]
    FOREIGN KEY ([Member_Id])
    REFERENCES [dbo].[Members]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_MemberBankInfo'
CREATE INDEX [IX_FK_MemberBankInfo]
ON [dbo].[BankInfo]
    ([Member_Id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------