USE [vap];
GO

INSERT INTO [SystemSettings] VALUES ('PointsRate', '1500', '购币每消费10000现金 增长点数1500');
GO
INSERT INTO [SystemSettings] VALUES ('PV', '60%', '返利PV值(百分比)');
GO
INSERT INTO [SystemSettings] VALUES ('ChongXiaoRate', '10%', '重消所占返利比例(百分比)');
GO
INSERT INTO [SystemSettings] VALUES ('MinBaoDanCashBalance', '10000', '最小报单金额(单位：元)');
GO
INSERT INTO [SystemSettings] VALUES ('MinBaoDanSell', '10', '积分出售最小数量');
GO
INSERT INTO [SystemSettings] VALUES ('BaoDanBuyFee', '0', '报单 买入 手续费(单位：元)');
GO
INSERT INTO [SystemSettings] VALUES ('BaoDanSellFee', '0', '报单 售出 手续费(单位：元)');
GO
INSERT INTO [SystemSettings] VALUES ('CashTopupFee', '0', '资金充值手续费(单位：元)');
GO
INSERT INTO [SystemSettings] VALUES ('CashWithdrawFee', '3%', '资金提现手续费(单位：元 或百分比)');
GO
INSERT INTO [SystemSettings] VALUES ('CashWithdrawMax', '5000', '资金提现每笔最大额度(单位：元)');
GO
INSERT INTO [SystemSettings] VALUES ('CashWithdrawMin', '100', '资金提现每笔最小额度(单位：元)');
GO
INSERT INTO [SystemSettings] VALUES ('CashTopupMin', '10000', '资金充值每笔最小额度(单位：元)');
GO
INSERT INTO [SystemSettings] VALUES ('CoinPriceRate', '0.05', '积分解冻价格最小增幅及解冻比例(5%)');
GO
INSERT INTO [SystemSettings] VALUES ('EnableRefundOnlyForActivateUser', 'true', '上线只有报过单才会有返利和业绩提升开关(true 或 false)');
GO
INSERT INTO [SystemSettings] VALUES ('MemberUploadTopupFilePath', 'Upload/Topup', '用户上传汇款凭证存储路径');
GO
INSERT INTO [SystemSettings] VALUES ('MemberUploadIdentityFilePath', 'Upload/Identity', '用户上传身份证存储路径');
GO
