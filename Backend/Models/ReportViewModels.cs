using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Backend.Models
{
   
    public class GeneralReportViewModel
    {

        //会员总数
        public int MemberCount { get; set; }
        //待审核会员总数
        public int PendingMemberCount { get; set; }
        //今日注册会员总数
        public int TodayMemberCount { get; set; }
        //累计充值金额
        public decimal TotalTopup { get; set; }
        //累计待审核充值金额
        public decimal TotalPendingTopup { get; set; }

        //累计待审核售币数量
        public decimal TotalPendingSells { get; set; }
        //今日充值金额
        public decimal TodayTotalTopup { get; set; }
        //累计提现金额
        public decimal TotalWithdraw { get; set; }
        //累计待提现金额
        public decimal TotalPendingWithdraw { get; set; }
        //今日提现金额
        public decimal TodayTotalWithdraw { get; set; }
        //累计提现手续费
        public decimal TotalWithdrawFee { get; set; }
        //今日提现手续费
        public decimal TodayTotalWithdrawFee { get; set; }

        //当前会员现金总额
        public decimal TotalCash { get; set; }
        //当前会员冻结现金总额
        public decimal TotalLockCash { get; set; }

        //当前会员虚拟币总额
        public decimal TotalCoin{ get; set; }
        //当前会员冻结虚拟币总额
        public decimal TotalLockCoin { get; set; }

        //当前会员积分总额
        public decimal TotalPoint { get; set; }
        //当前会员冻结积分总额
        public decimal TotalLockPoint { get; set; }

        //当前会员冲销总额
        public decimal TotalCx { get; set; }
        //当前会员冻结冲销总额
        public decimal TotalLockCx { get; set; }

        public GeneralReportViewModel(vapEntities1 db)
        {
            var members = db.Members.Where(c => c.Id > 0);
            MemberCount = members.Count();
            TodayMemberCount = members.Count(c => c.RegisterTime >= DateTime.Today);
            TotalCx = members.Sum(c => c.ChongXiao1 + c.ChongXiao2);
            TotalLockCx = members.Sum(c => c.ChongXiao2);
            TotalLockCash = members.Sum(c => c.Cash2);
            TotalCash = members.Sum(c => c.Cash2+c.Cash1);
            TotalLockCoin = members.Sum(c => c.Coin2);
            TotalCoin = members.Sum(c => c.Coin1 + c.Coin2);
            TotalLockPoint = members.Sum(c => c.Point2);
            TotalPoint = members.Sum(c => c.Point2 + c.Point1);

            var cashRecords = db.CashTransactions;
            var chongzhi = VapLib.现金交易类型.充值.ToString();
            var tixian = VapLib.现金交易类型.提现.ToString();
            var daishenhe = VapLib.现金状态.待审核.ToString();
            var topups = cashRecords.Where(c => c.Type == chongzhi).ToList();
            if (topups.Any())
            {
                TotalTopup = topups.Sum(c => c.Amount);
                TotalPendingTopup = topups.Where(c => c.Status == daishenhe).Sum(c => c.Amount);
                TodayTotalTopup = topups.Where(c => c.DateTime >= DateTime.Today).Sum(c => c.Amount);
            }
            

            var withdraws = cashRecords.Where(c => c.Type == tixian).ToList();
            if (withdraws.Any())
            {
                TotalWithdraw = withdraws.Sum(c => c.Amount);
                TotalPendingWithdraw = withdraws.Where(c => c.Status == daishenhe).Sum(c => c.Amount);
                TodayTotalWithdraw = withdraws.Where(c => c.DateTime >= DateTime.Today).Sum(c => c.Amount);
                TotalWithdrawFee = withdraws.Sum(c => c.Fee);
                TodayTotalWithdrawFee = withdraws.Where(c => c.DateTime >= DateTime.Today).Sum(c => c.Fee);

            }
            
            var tempType = VapLib.报单类型.卖出.ToString();
            var tempStatus = VapLib.报单状态.未成交.ToString();
            var sells = db.BaoDanTransactions.Where(c => c.Type == tempType && c.Status == tempStatus).ToList();
            if (sells.Any())
            {
                TotalPendingSells = sells.Sum(c => c.Amount);
            }

        }

     
    }
}