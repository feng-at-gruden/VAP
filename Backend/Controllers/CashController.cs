using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Backend.Helper;
using Backend.Models;
using VapLib;

namespace Backend.Controllers
{
    [MyAuthorize(Roles = "Admin,Finance,CustomerService")]
    public class CashController : Controller
    {
        private vapEntities1 db = new vapEntities1();

        /// <summary>
        /// 待审批现金充值记录
        /// </summary>
        /// <returns></returns>
        public ActionResult PendingTopups()
        {
            if (TempData.ContainsKey("ModelState"))
            {
                ModelState.Merge((ModelStateDictionary)TempData["ModelState"]);
            }
            var status =现金状态.待审核.ToString();
            var type = 现金交易类型.充值.ToString();
            var cashtransactions = db.CashTransactions.Where(c => c.Status == status
                                                                  && c.Type == type)
                                                                  .OrderBy(c=>c.DateTime); 
            return View(cashtransactions.ToList());
        }
        public ActionResult PendingWithdraws()
        {
            if (TempData.ContainsKey("ModelState"))
            {
                ModelState.Merge((ModelStateDictionary)TempData["ModelState"]);
            }
            var status = 现金状态.待审核.ToString();
            var type = 现金交易类型.提现.ToString();
            var cashtransactions = db.CashTransactions.Where(c => c.Status == status
                                                                  && c.Type == type)
                                                                  .OrderBy(c => c.DateTime);
            return View(cashtransactions.ToList());
        }
       


        // GET: /Cash/Edit/5
        public ActionResult ApproveCashTrans(int id)
        {
           
            CashTransaction cashtransaction = db.CashTransactions.Find(id);
            if (cashtransaction != null)
            {
                //验证现金状态为 待审核
                if (cashtransaction.Status == 现金状态.待审核.ToString())
                {
                    //充值，会员可用现金增加
                    if (cashtransaction.Type == 现金交易类型.充值.ToString())
                    {
                        var member = db.Members.Find(cashtransaction.Member.Id);
                        cashtransaction.Status = 现金状态.可用.ToString();
                        member.Cash1 = member.Cash1 + cashtransaction.Amount;
                        db.Entry(member).State = EntityState.Modified;
                        db.Entry(cashtransaction).State = EntityState.Modified;
                        db.SaveChanges();
                        ModelState.AddModelError("", "审批成功。");
                        return RedirectToAction("PendingTopups");

                    } //提现 会员可用现金再提交申请时已经扣除
                    else if (cashtransaction.Type == 现金交易类型.提现.ToString())
                    {
                        cashtransaction.Status = 现金状态.可用.ToString();
                        db.Entry(cashtransaction).State = EntityState.Modified;
                        db.SaveChanges();
                        ModelState.AddModelError("", "审批成功。");
                        return RedirectToAction("PendingWithdraws");

                    }
                }
                else
                {
                    ModelState.AddModelError("", "该记录状态不是待审核状态。");
                }

            }//无此记录，跳转到首页
            else
            {
                ModelState.AddModelError("", "该记录不存在。");
            }
            TempData["ModelState"] = ModelState;
            
            
            return RedirectToAction("Index","Home");
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
