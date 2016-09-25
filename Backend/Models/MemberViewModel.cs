using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace Backend.Models
{
   
    public class MemberViewModel
    {

        
        public int Id { get; set; }
        public int? ReferId { get; set; }
        public decimal Achievement { get; set; }
        public decimal BuyAmount { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public string UserName { get; set; }
        public string RealName { get; set; }
        public string MemberLevel { get; set; }
        public DateTime RegisterTime { get; set; }



        public MemberViewModel(Member model)
        {

            Id = model.Id;
            ReferId = model.Referral_Id;
            Achievement = model.Achievement;
            Email = model.Email;
            Status = model.Status;
            UserName = model.UserName;
            RealName = model.RealName;
            MemberLevel = model.MemberLevel.Level;
            RegisterTime = model.RegisterTime;
            //BuyAmount = model.BaoDanTransactions.Where(c => c.Type == VapLib.报单类型.买入.ToString() && c.Status==VapLib.报单状态.已成交.ToString()).Sum(c=>c.Amount);
        }

     
    }
}