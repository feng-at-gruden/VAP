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
   
    public class CashController : Controller
    {
        private vapEntities1 db = new vapEntities1();
        private static object dbLock = new object();

        /// <summary>
        /// 所有cash 操作记录
        /// </summary>
        /// <returns></returns>
        [MyAuthorize(Roles = "Admin,Finance")]
        public ActionResult CashTrans(string type,string status,string memberAccount,string startDate,string endDate)
        {
            var start = DateTime.Now;
            var end = DateTime.Now;
            if (!DateTime.TryParse(startDate, out start) &&!string.IsNullOrEmpty(startDate))
            {
                ModelState.AddModelError("","起始日期格式输入不正确");
                return View();
            }
            if (!DateTime.TryParse(endDate, out end) && !string.IsNullOrEmpty(endDate))
            {
                ModelState.AddModelError("", "截止日期格式输入不正确");
                return View();
            }
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
            if (!string.IsNullOrEmpty(startDate))
            {
                records = records.Where(c => c.DateTime>=start);
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                end = end.AddDays(1);
                records = records.Where(c => c.DateTime <end);
            }
            ViewBag.memberAccount = memberAccount;
            ViewBag.status = status;
            ViewBag.type = type;
            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;

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
        [MyAuthorize(Roles = "Admin")]
        public ActionResult PendingTopups(string startDate, string endDate)
        {
            if (TempData.ContainsKey("ModelState"))
            {
                ModelState.Merge((ModelStateDictionary)TempData["ModelState"]);
            }
            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;
            var status =现金状态.待审核.ToString();
            var type = 现金交易类型.充值.ToString();
            var cashtransactions = db.CashTransactions.Where(c => c.Status == status
                                                                  && c.Type == type)
                                                                  ;
            if (!string.IsNullOrEmpty(startDate))
            {
                DateTime start;
                DateTime.TryParse(startDate, out start);
                cashtransactions = cashtransactions.Where(c => c.DateTime >= start);
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                DateTime end;
                DateTime.TryParse(endDate, out end);
                end = end.AddDays(1);
                cashtransactions = cashtransactions.Where(c => c.DateTime < end);
            }
            return View(cashtransactions.OrderBy(c => c.DateTime).ToList());
        }
        /// <summary>
        /// 会员冻结积分记录
        /// </summary>
        /// <returns></returns>
        [MyAuthorize(Roles = "Admin")]
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
        [MyAuthorize(Roles = "Admin")]
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
        [MyAuthorize(Roles = "Admin")]
        
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
        [MyAuthorize(Roles = "Admin")]
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
        public ActionResult PendingWithdraws(string startDate,string endDate)
        {
            if (TempData.ContainsKey("ModelState"))
            {
                ModelState.Merge((ModelStateDictionary)TempData["ModelState"]);
            }
            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;
            var status = 现金状态.待审核.ToString();
            var type = 现金交易类型.提现.ToString();
            
            var cashtransactions = db.CashTransactions.Where(c => c.Status == status
                                                                  && c.Type == type);
            if (!string.IsNullOrEmpty(startDate))
            {
                DateTime start;
                DateTime.TryParse(startDate, out start);
                cashtransactions = cashtransactions.Where(c => c.DateTime >= start);
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                DateTime end;
                DateTime.TryParse(endDate, out end);
                end = end.AddDays(1);
                cashtransactions = cashtransactions.Where(c => c.DateTime < end);
            }
            /*IEnumerable<CashWithdrawHistoryViewModel> model = from row in db.CashTransactions
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
                                                              };*/
            IEnumerable<CashWithdrawHistoryViewModel> model = cashtransactions.ToList()
                .OrderByDescending(c => c.DateTime)
                .Select(c => new CashWithdrawHistoryViewModel
                {
                    Status = status.Equals(c.Status) ? "待审核" : "已审核",
                    WithdrawTime = c.DateTime,
                    Fee = c.Fee,
                    Amount = Math.Abs(c.Amount),
                    Bank = c.Bank,
                    BankAccount = c.BankAccount,
                    Id = c.Id,
                    MemberId = c.MemberId,
                    MemberEmail = c.Member.Email,
                });

            return View(model);
        }
        [MyAuthorize(Roles = "Admin,Finance")]
        public ActionResult DeleteCashTrans(int id,string type="T")
        {
            var recored = db.CashTransactions.Find(id);
            
            db.CashTransactions.Remove(recored);
            db.SaveChanges();
            
            TempData["ModelState"] = ModelState;
            if (type == "T")
            {
                ModelState.AddModelError("", "充值记录删除成功！");
                return RedirectToAction("PendingTopups");
            }
            else
            {
                //recored.Member.Cash1 += Math.Abs(recored.Amount);
                Member member = db.Members.Find(recored.MemberId);
                member.Cash1 += Math.Abs(recored.Amount);
                db.SaveChanges();
                ModelState.AddModelError("", "提现申请记录删除成功！ ￥" + Math.Abs(recored.Amount) + "元已返还至该用户可用资金账户。");
                return RedirectToAction("PendingWithdraws");
            }
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
                lock (dbLock)
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
