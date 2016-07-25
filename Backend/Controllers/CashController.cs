using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Backend.Models;
using VapLib;

namespace Backend.Controllers
{
    public class CashController : Controller
    {
        private vapEntities1 db = new vapEntities1();

        /// <summary>
        /// 待审批现金充值记录
        /// </summary>
        /// <returns></returns>
        public ActionResult PendingTopups()
        {
            var cashtransactions = db.CashTransactions.Where(c => c.Status == 现金状态.待审核.ToString()
                                                                  && c.Type == 现金交易类型.充值.ToString())
                                                                  .OrderBy(c=>c.DateTime); 
            return View(cashtransactions.ToList());
        }
        public ActionResult PendingWithdraws()
        {
            var cashtransactions = db.CashTransactions.Where(c => c.Status == 现金状态.待审核.ToString()
                                                                  && c.Type == 现金交易类型.提现.ToString())
                                                                  .OrderBy(c => c.DateTime);
            return View(cashtransactions.ToList());
        }
       


        // GET: /Cash/Edit/5
        public ActionResult ApproveCashTrans(int id)
        {
           
            CashTransaction cashtransaction = db.CashTransactions.Find(id);
            if (cashtransaction != null)
            {
                var member = db.Members.Find(cashtransaction.Member.Id); 
                if (cashtransaction.Type == 现金交易类型.充值.ToString())
                {
                    cashtransaction.Status = 现金状态.可用.ToString();
                    member.Cash1 = member.Cash1 + cashtransaction.Amount;
                    
                }
                else if (cashtransaction.Type == 现金交易类型.提现.ToString())
                {
                    cashtransaction.Status = 现金状态.可用.ToString();
                    member.Cash1 = member.Cash1 - cashtransaction.Amount;
                    
                }
                db.Entry(member).State = EntityState.Modified;
                db.Entry(cashtransaction).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("PendingTopups");
        }

        
      

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
