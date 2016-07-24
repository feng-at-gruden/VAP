using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MemberCenter.Models
{
    public class BaoDanBuyViewModel
    {
        [Display(Name = "联和通用积分实时价格")]
        public decimal CurrentCoinPrice { get; set; }

        [Display(Name = "可用现金")]
        public decimal AvailableCash { get; set; }

        [Display(Name = "报单价格")]
        public decimal RequestPrice { get; set; }

        [Display(Name = "报单个数")]
        public decimal RequestQuantity { get; set; }

        [Display(Name = "报单总额")]
        public decimal RequestCash { get; set; }

        [Display(Name = "手续费")]
        public decimal Fee { get; set; }

        [Display(Name = "消耗现金")]
        public decimal TotalCostCash { get; set; }

        [Display(Name = "余额")]
        public decimal CashLeft { get; set; }
    }

    public class LockedCoinsViewModel
    {
        public Int32 BaoDanId { get; set; }

        public DateTime BaoDanTime { get; set; }

        [DisplayFormat(DataFormatString = "{0:n8}")]
        public decimal BaoDanQuantity { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal BaoDanPrice { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal LastPrice { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal NextPrice { get; set; }

        [DisplayFormat(DataFormatString = "{0:n8}")]
        public decimal LockedAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:n8}")]
        public decimal AvailableAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:n8}")]
        public decimal AmountLeft { get; set; }

        public String Status { get; set; }
    }

    public class BaoDanHistoryViewModel
    {
        public DateTime BaoDanTime { get; set; }

        public String Status { get; set; }

        public String Type { get; set; }

        [DisplayFormat(DataFormatString = "{0:n8}")]
        public decimal RequestQuantity { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal RequestAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal Fee { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal RequestPrice { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal FinalPrice { get; set; }

        [DisplayFormat(DataFormatString = "{0:n8}")]
        public decimal FinalQuantity { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal FinalAmount { get; set; }
    }


}

