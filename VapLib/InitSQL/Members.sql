USE [vap];
GO

INSERT INTO [Members] VALUES ('root@qq.com', 'root', '顶级用户(测试)', '123456', '654321', .00, .00, .00, .00, .00, .00, .000000, .000000, '2016-7-23 23:22:14', .00, '正常', '1', '1', '1', '1', '1', NULL, 15);
GO

INSERT INTO [Members] VALUES ('a@qq.com', 'A', '用户A', '123456', '654321', .00, .00, .00, .00, .00, .00, .000000, .000000, '2016-7-23 23:23:37', .00, '正常', NULL, NULL, NULL, NULL, NULL, 1, 6);
GO

INSERT INTO [Members] VALUES ('a-1@qq.com', NULL, NULL, '123456', '654321', .00, .00, .00, .00, .00, .00, .000000, .000000, '2016-7-24 15:26:49', .00, '正常', NULL, NULL, NULL, NULL, NULL, 2, 4);
GO

INSERT INTO [Members] VALUES ('a-2@qq.com', NULL, NULL, '123456', '654321', 10005.00, .00, .00, .00, .00, .00, 100.000000, .000000, '2016-7-24 15:26:59', .00, '正常', NULL, NULL, NULL, NULL, NULL, 3, 1);
GO