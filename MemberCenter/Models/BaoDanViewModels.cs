using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MemberCenter.Models
{
    public class BaoDanBuyViewModels
    {
        [Display(Name = "联和通用积分实时价格")]
        public decimal CurrentCoinPrice { get; set; }

        [Display(Name = "可用现金")]
        public decimal AvailableCash { get; set; }

        [Display(Name = "报单价格")]
        public decimal RequestPrice { get; set; }

        [Display(Name = "报单个数")]
        public decimal RequestQuantity { get; set; }

        [Display(Name = "消耗现金")]
        public decimal CostCash { get; set; }

        [Display(Name = "报单总额")]
        public decimal TotalCash { get; set; }
    }


}