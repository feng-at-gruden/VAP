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
        public static readonly string[] MemberLevels = Enum.GetNames(typeof(会员等级));
      
        //public const decimal PointsRate = 1500m;            //购币每消费10000现金 增长点数1500
        //public const decimal PV = 0.6m;                     //返利PV值 后需定义在数据库中     上线返利 = 下线消费金额 x 上线等级返利比例 × PV × 90%
        //public const decimal ChongXiaoRate = 0.1m;          //重消所占比例 后需定义在数据库中  上线重消 = 下线消费金额 x 上线等级返利比例 × PV × 10%
        //public const decimal MinBaoDanCashBalance = 10000m; //最小报单金额 买入
        //public const decimal MinBaoDanSell = 10m;           //最小报单数量 卖出
        //public const decimal BaoDanBuyFee = 0m;             //报单 买入 手续费  后需定义在数据库中  
        //public const decimal BaoDanSellFee = 0m;            //报单 售出 手续费  后需定义在数据库中  比率还是固定金额 待定
        //public const decimal CashTopupFee = 0m;             //资金充值手续费
        //public const decimal CashWithdrawFee = 30m;         //资金提现手续费
        //public const decimal CashWithdrawMax = 5000m;       //资金提现每笔最大额度
        //public const decimal CashWithdrawMin = 100m;        //资金提现每笔最小额度
        //public const decimal CashTopupMin = 10000m;         //资金充值每笔最小额度
        //public const decimal CoinPriceRate = 0.05m;         //电子币解冻比例
        //public const bool EnableRefundOnlyForActivateUser = true;       //Ture 只有上线报过单才能有返利和业绩提升 否则没有

        //public const String MemberUploadTopupFilePath = "Upload/Topup";       //用户上传汇款凭证存储路径
        public const String MemberUploadIdentityFilePath = "Upload/Identity";       //用户上传身份证存储路径

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
        已审核 = 1,        //审核通过后为可用状态，  可表示为 提现审核通过， 充值审核通过， 返利及售币冻结资金记录解冻 
        冻结 = 2,          //只给售币情况使用
        解冻 = 3,          //只给售币情况使用
    }

    public enum 现金交易类型
    {
        充值 = 1,     //可用现金增加  审核通过后 Member.Cash1增加
        提现 = 2,     //可用现金减少  审核通过后 Member.Cash1减少
        下线返利 = 3, //冻结现金增加   Member.Cash2增加
        购币消费 = 4,     //可用现金减少  审核通过后 Member.Cash1减少
        售币所得 = 5,     //冻结现金增加  审核通过后 Member.Cash2增加
        会员间转入 = 6,   //其他会员转入   资金增加
        会员间转出 = 7,   //转出给其他会员  资金减少
    }


    public enum 报单类型            //通用联合积分
    {
        买入 = 0,
        卖出 = 1,
        会员间转入 = 2,   //其他会员转入
        会员间转出 = 3,   //转出给其他会员
    }

    public enum 报单状态
    {
        未成交 = 0,
        已成交 = 1,        //只有出售电子币需要审核
        用户已取消 = 2,
    }


    //To confirm
    public enum 积分记录类型
    {
        购币所得 = 1,
        商城消费 = 2,
    }

    public enum 积分状态
    {
        冻结 = 0,
        可用 = 1,
    }


    //To confirm
    public enum 重消记录类型
    {
        下线返利 = 1,
        商城消费 = 2,
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


    public enum 银行账户信息类型
    {
        系统账户 = 0,
        会员账户 = 1,
    }


}
