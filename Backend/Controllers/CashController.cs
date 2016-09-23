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
    [MyAuthorize(Roles = "Admin")]
    public class CashController : Controller
    {
        private vapEntities1 db = new vapEntities1();
        /// <summary>
        /// 所有cash 操作记录
        /// </summary>
        /// <returns></returns>
        public ActionResult CashTrans(string type,string status,string memberAccount)
        {

            var records = db.CashTransactions.Include(c=>c.Member);
            if (!string.IsNullOrEmpty(type))
            {
                records = records.Where(c => c.Type == type);
            }
            if (!string.IsNullOrEmpty(status))
            {
                records = records.Where(c => c.Status == status);
            }
            if (!string.IsNullOrEmpty(memberAccount))
            {
                records = records.Where(c => c.Member.Email.Contains(memberAccount));
            }
            ViewBag.memberAccount = memberAccount;
            ViewBag.status = status;
            ViewBag.type = type;

            IEnumerable<CashTransactionViewModel> modle = from row in records
                                                          select new CashTransactionViewModel
                                                          {
                                                              Type = row.Type,
                                                              Status = row.Status,
                                                              DateTime = row.DateTime,
                                                              Fee = row.Fee,
                                                              Amount = row.Amount,
                                                              ID = row.Id,
                                                              FileUrl = row.FileUrl,
                                                              Bank = row.Bank,
                                                              RemitNumber = row.RemitNumber,
                                                              RemitAccount = row.RemitUserName,
                                                              MemberEmail = row.Member.Email,
                                                              MemberId = row.Member.Id,
                                                              Comment = row.Comment
                                                          };
            return View(modle);
        }
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
        /// <summary>
        /// 会员冻结积分记录
        /// </summary>
        /// <returns></returns>
        public ActionResult MemberLocks(int memberId)
        {
            if (TempData.ContainsKey("ModelState"))
            {
                ModelState.Merge((ModelStateDictionary)TempData["ModelState"]);
            }
            //var status = 现金状态.冻结.ToString();
            //var type = 现金交易类型.充值.ToString();
            var lockedCv = db.LockedCoins.Where(c => c.MemberId == memberId
                                                && c.LockedAmount >0).ToList()
                                                .Select(c=>new LockedCoinViewModel(c));

            return View(lockedCv);
        }
        public ActionResult UnlockCashTrans(int id)
        {

            var record = db.LockedCoins.Find(id);
            var member = db.Members.Find(record.MemberId);
            if(record.LockedAmount>0)
            {
                var amount = record.LockedAmount;
                record.AvailabeAmount = record.TotalAmount;
                record.LockedAmount = 0;
                
               
                member.Coin1 += amount;
                member.Coin2 -= amount;

                db.Entry(member).State = EntityState.Modified;
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();
                ModelState.AddModelError("", "解冻操作执行成功。");
            }
            else
            {
                ModelState.AddModelError("", "没有符合的记录。");
            }
            

            TempData["ModelState"] = ModelState;
            return RedirectToAction("MemberLocks",new{memberId=member.Id});
           
        }
        /// <summary>
        /// 会员现金充值记录
        /// </summary>
        /// <returns></returns>
        public ActionResult MemberTopups(int memberId)
        {
            
            //var status = 现金状态.待审核.ToString();
            var type = 现金交易类型.充值.ToString();
            var cashtransactions = db.CashTransactions.Where(c => c.MemberId == memberId
                                                                  && c.Type == type)
                                                                  .OrderBy(c => c.DateTime);

            IEnumerable<CashTransactionViewModel> model = from row in cashtransactions
                                                          select new CashTransactionViewModel
                                                          {
                                                              Type = row.Type,
                                                              Status = row.Status,
                                                              DateTime = row.DateTime,
                                                              Fee = row.Fee,
                                                              Amount = row.Amount,
                                                              ID = row.Id,
                                                              FileUrl = row.FileUrl,
                                                              Bank = row.Bank,
                                                              RemitNumber = row.RemitNumber,
                                                              RemitAccount = row.RemitUserName,
                                                              Comment = row.Comment
                                                          };
            return View(model);
        }
        /// <summary>
        /// 会员提现记录
        /// </summary>
        /// <returns></returns>
        public ActionResult MemberWithdraws(int memberId)
        {

            //var status = 现金状态.待审核.ToString();
            var type = 现金交易类型.提现.ToString();
            var cashtransactions = db.CashTransactions.Where(c => c.MemberId == memberId
                                                                  && c.Type == type)
                                                                  .OrderBy(c => c.DateTime);
            return View(cashtransactions.ToList());
        }

        [MyAuthorize(Roles = "Admin,Finance")]
        public ActionResult PendingWithdraws()
        {
            if (TempData.ContainsKey("ModelState"))
            {
                ModelState.Merge((ModelStateDictionary)TempData["ModelState"]);
            }
            var status = 现金状态.待审核.ToString();
            var type = 现金交易类型.提现.ToString();
            /*
            var cashtransactions = db.CashTransactions.Where(c => c.Status == status
                                                                  && c.Type == type)
                                                                  .OrderBy(c => c.DateTime);
            return View(cashtransactions.ToList());*/
            IEnumerable<CashWithdrawHistoryViewModel> model = from row in db.CashTransactions
                                                              where row.Status==status && row.Type==type
                                                              orderby row.DateTime descending
                                                              select new CashWithdrawHistoryViewModel
                                                              {
                                                                  Status = status.Equals(row.Status) ? "待审核" : "已审核",
                                                                  WithdrawTime = row.DateTime,
                                                                  Fee = row.Fee,
                                                                  Amount = Math.Abs(row.Amount),
                                                                  Bank = row.Bank,
                                                                  BankAccount = row.BankAccount,
                                                                  Id = row.Id,
                                                                  MemberId = row.MemberId,
                                                                  MemberEmail = row.Member.Email,
                                                              };

            return View(model);
        }
        [MyAuthorize(Roles = "Admin,Finance")]
        public ActionResult DeleteCashTrans(int id,string type="T")
        {
            var recored = db.CashTransactions.Find(id);
            db.CashTransactions.Remove(recored);
            db.SaveChanges();
            ModelState.AddModelError("", "记录删除成功。");
            TempData["ModelState"] = ModelState;
            if(type=="T")
                return RedirectToAction("PendingTopups");
            else
                return RedirectToAction("PendingWithdraws");
        }
        [MyAuthorize(Roles = "Admin,Finance")]
        // GET: /Cash/Edit/5
        public ActionResult ApproveCashTrans(int id)
        {

            CashTransaction cashtransaction = db.CashTransactions.Find(id);
            return View(cashtransaction);
        }
       [MyAuthorize(Roles = "Admin,Finance")]
        [HttpPost]
        public ActionResult ApproveCashTrans(CashTransaction model)
        {

            CashTransaction cashtransaction = db.CashTransactions.Find(model.Id);
            if (cashtransaction != null)
            {
                //验证现金状态为 待审核
                if (cashtransaction.Status == 现金状态.待审核.ToString())
                {
                    //充值，会员可用现金增加
                    if (cashtransaction.Type == 现金交易类型.充值.ToString())
                    {
                        var member = db.Members.Find(cashtransaction.Member.Id);
                        cashtransaction.Status = 现金状态.已审核.ToString();
                        member.Cash1 = member.Cash1 + cashtransaction.Amount;
                        db.Entry(member).State = EntityState.Modified;
                        db.Entry(cashtransaction).State = EntityState.Modified;
                        db.SaveChanges();
                        ModelState.AddModelError("", "审批成功。");
                        return RedirectToAction("PendingTopups");

                    } //提现 会员可用现金再提交申请时已经扣除
                    else if (cashtransaction.Type == 现金交易类型.提现.ToString())
                    {
                        cashtransaction.Status = 现金状态.已审核.ToString();
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
