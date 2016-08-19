using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Backend.Models
{
   
    public class LockedCoinViewModel
    {

        
        public int Id { get; set; }
        
        public decimal? LastPrice { get; set; }

        public decimal? NextPrice { get; set; }
        public decimal? Price { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal LockedAmount { get; set; }
        public decimal AvailableAmount { get; set; }

        public DateTime BaoDanTime { get; set; }
       
        public LockedCoinViewModel(LockedCoin model)
        {

            Id = model.Id;
            LastPrice = model.LastPrice;
            NextPrice = model.NextPrice;
            Price = model.Price;
            TotalAmount = model.TotalAmount;
            LockedAmount = model.LockedAmount;
            AvailableAmount = model.AvailabeAmount;
            BaoDanTime = model.BaoDanTransaction.DateTime;
        }

     
    }
}