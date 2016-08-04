using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MemberCenter.Models;

namespace MemberCenter.Models
{
    public class HomeViewModel
    {
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "当前价格")]
        public decimal CurrentCoinPrice { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "最高价格")]
        public decimal MaxCoinPrice { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "最低价格")]
        public decimal MinCoinPrice { get; set; }

        [DisplayFormat(DataFormatString = "{0:n6}")]
        [Display(Name = "今日成交量")]
        public decimal TodayBaoDanAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:n0}")]
        [Display(Name = "今日成交额")]
        public int TodayBaoDanCash { get; set; }

        [DisplayFormat(DataFormatString = "{0:n0}")]
        [Display(Name = "历史成交额")]
        public int TotalTransactionCash { get; set; }

        [DisplayFormat(DataFormatString = "{0}")]
        [Display(Name = "注册用户数")]
        public int MemberAmount { get; set; }

        [Display(Name = "最近卖盘")]
        public IEnumerable<BaoDanHistoryViewModel> RecentBaoDanSell { get; set; }

        [Display(Name = "最近买盘")]
        public IEnumerable<BaoDanHistoryViewModel> RecentBaoDanBuy { get; set; }

        [Display(Name = "历史价格数据")]
        public IEnumerable<CoinPriceHistoryViewModel> CoinPriceHistory { get; set; }

        [Display(Name = "历史买入数据")]
        public IEnumerable<DailyAmountViewModel> BuyHistory { get; set; }

        [Display(Name = "历史卖出数据")]
        public IEnumerable<DailyAmountViewModel> SellHistory { get; set; }
    }


    public class CoinPriceHistoryViewModel
    {
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "价格")]
        public decimal Price { get; set; }

        [Display(Name = "日期")]
        public DateTime DateTime { get; set; }
    }

    public class DailyAmountViewModel
    {
        [DisplayFormat(DataFormatString = "{0:n6}")]
        [Display(Name = "数量")]
        public decimal Amount { get; set; }

        [Display(Name = "日期")]
        public DateTime DateTime { get; set; }
    }

}