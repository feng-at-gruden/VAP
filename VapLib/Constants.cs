using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VapLib
{
    public static class Constants
    {
        public const string AdminAccount = "admin";
        public const string AdminPass = "Admin123!";

        public const string DefaultPass = "Abc123456";
        public const int PageSize = 10;
       

        public static readonly string[] MetaType = Enum.GetNames(typeof(现金交易类型));
        public static readonly string[] NewsType = Enum.GetNames(typeof(新闻类型));

        public const int MinCashBalance = 10000;         //最小报单金额
        public const int PointsRate = 1500;              //购币每消费10000现金 增长点数
        public const decimal ChongXiaoRate = 0.15m;      //重消所占返利比例
        public const decimal BaoDanBuyFee = 5;           //报单 买入 手续费  后需定义在数据库中  
        public const decimal BaoDanSellFee = 100;        //报单 售出 手续费  后需定义在数据库中  比率还是固定金额 待定


    }
    
    /// <summary>
    /// 
    /// </summary>
    public enum 新闻类型
    {
        站内新闻,
        站外新闻,
        公告,
        其他
    }
    public enum 会员状态
    {
        待审核 = 0,
        正常 = 1,
        锁定 = 2,
    }

    public enum 现金状态
    {
        待审核 = 0,
        可用 = 1,
        冻结 = 2,     //只给售币情况使用
    }

    public enum 现金交易类型
    {
        充值 = 1,     //可用现金增加  审核通过后 Member.Cash1增加
        提现 = 2,     //可用现金减少  审核通过后 Member.Cash1减少
        下线返利 = 3, //冻结现金增加   Member.Cash2增加
        购币 = 4,     //可用现金减少  审核通过后 Member.Cash1减少
        售币 = 5,     //冻结现金增加  审核通过后 Member.Cash2增加
    }


    public enum 货币状态
    {
        冻结 = 0,
        可用 = 1,
    }

    public enum 报单类型
    {
        买入 = 0,
        卖出 = 1,
    }

    public enum 报单状态
    {
        未成交 = 0,
        已成交 = 1,        //只有出售电子币需要审核
    }


    //To confirm
    public enum 积分记录类型
    {
        购币所得积分 = 1,
        商场积分消费 = 2,
    }

    public enum 积分状态
    {
        冻结 = 0,
        可用 = 1,
    }


    //To confirm
    public enum 重消记录类型
    {
        下线返利重消 = 1,
        消费 = 2,
    }
    public enum 重消状态
    {
        冻结 = 0,
        可用 = 1,
    }


    public enum 会员等级
    {
        无等级 = 0,
        一星 = 1,
        二星 = 2,
        三星 = 3,
        四星 = 4,
        五星 = 5,
        六星 = 6,
        七星 = 7,
        一钻 = 8,
        二钻 = 9,
        三钻 = 10,
        四钻 = 11,
        五钻 = 12,
        六钻 = 13,
        七钻 = 14,
    }



}
