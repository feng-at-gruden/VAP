/*
Navicat SQL Server Data Transfer

Source Server         : Feng-SQLServer
Source Server Version : 105000
Source Host           : GC-LAPTOP-1\FENGSQLSERVER:1433
Source Database       : vap
Source Schema         : dbo

Target Server Type    : SQL Server
Target Server Version : 105000
File Encoding         : 65001

Date: 2016-07-12 22:43:25
*/


-- ----------------------------
-- Table structure for Members
-- ----------------------------
DROP TABLE [dbo].[Members]
GO
CREATE TABLE [dbo].[Members] (
[Id] int NOT NULL ,
[Email] nvarchar(50) NOT NULL ,
[UserName] nvarchar(50) NOT NULL ,
[RealName] nvarchar(MAX) NOT NULL ,
[Password1] nvarchar(50) NOT NULL ,
[Password2] nvarchar(50) NOT NULL ,
[Password3] nvarchar(50) NOT NULL ,
[Cash1] decimal(18) NOT NULL ,
[Cash2] decimal(18) NOT NULL ,
[Point1] decimal(18) NOT NULL ,
[Point2] decimal(18) NOT NULL ,
[ChongXiao1] decimal(18) NOT NULL ,
[ChongXiao2] decimal(18) NOT NULL ,
[Coin1] decimal(18) NOT NULL ,
[Coin2] decimal(18) NOT NULL ,
[RegisterTime] datetime NOT NULL ,
[Level] nvarchar(MAX) NOT NULL ,
[Status] nvarchar(MAX) NOT NULL ,
[TiXianStatus] nvarchar(MAX) NOT NULL ,
[TiBiStatus] nvarchar(MAX) NOT NULL ,
[IdSubmitted] bit NOT NULL ,
[IdApproved] bit NOT NULL ,
[Referral_Id] int NOT NULL 
)


GO

-- ----------------------------
-- Records of Members
-- ----------------------------
INSERT INTO [dbo].[Members] ([Id], [Email], [UserName], [RealName], [Password1], [Password2], [Password3], [Cash1], [Cash2], [Point1], [Point2], [ChongXiao1], [ChongXiao2], [Coin1], [Coin2], [RegisterTime], [Level], [Status], [TiXianStatus], [TiBiStatus], [IdSubmitted], [IdApproved], [Referral_Id]) VALUES (N'1', N'root@qq.com', N'root', N'顶级用户', N'123456', N'123456', N'123456', N'20000', N'500', N'0', N'200', N'0', N'100', N'50', N'80', N'2016-07-12 22:32:32.000', N'七钻', N'正常', N'正常', N'正常', N'1', N'1', N'1')
GO
GO

-- ----------------------------
-- Indexes structure for table Members
-- ----------------------------
CREATE INDEX [IX_FK_MemberMember] ON [dbo].[Members]
([Referral_Id] ASC) 
GO

-- ----------------------------
-- Primary Key structure for table Members
-- ----------------------------
ALTER TABLE [dbo].[Members] ADD PRIMARY KEY ([Id])
GO

-- ----------------------------
-- Foreign Key structure for table [dbo].[Members]
-- ----------------------------
ALTER TABLE [dbo].[Members] ADD FOREIGN KEY ([Referral_Id]) REFERENCES [dbo].[Members] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO
