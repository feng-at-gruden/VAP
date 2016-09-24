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
    public class BaoDanController : Controller
    {
        private vapEntities1 db = new vapEntities1();
        /// <summary>
        /// 会员报单卖出记录
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public ActionResult MemberSells(int memberId)
        {
            //var status = 报单状态.未成交.ToString();
            var type = 报单类型.卖出.ToString();
            var records = db.BaoDanTransactions.Include(b => b.Member).Where(c => c.MemberId == memberId
                && c.Type == type).OrderBy(c => c.DateTime);
            return View(records.ToList());
        }
        /// <summary>
        /// 会员报单买入记录
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public ActionResult MemberBuys(int memberId)
        {
            //var status = 报单状态.未成交.ToString();
            var type = 报单类型.买入.ToString();
            var records = db.BaoDanTransactions.Include(b => b.Member).Where(c => c.MemberId == memberId
                && c.Type == type).OrderBy(c => c.DateTime);
            return View(records.ToList());
        }
        // GET: BaoDan
        public ActionResult PendingSells()
        {
            if (TempData.ContainsKey("ModelState"))
            {
                ModelState.Merge((ModelStateDictionary)TempData["ModelState"]);
            }
            var status =报单状态.未成交.ToString();
            var type =报单类型.卖出.ToString();
            var records = db.BaoDanTransactions.Include(b => b.Member).Where(c=>c.Status==status
                &&c.Type==type).OrderBy(c=>c.DateTime);
            return View(records.ToList());
        }
        public ActionResult GetBaodanTrans(string type,string status,string memberAccount)
        {


            var records = db.BaoDanTransactions.Include(b => b.Member);
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
            return View(records.OrderBy(c=>c.DateTime).ToList());
        }

        public ActionResult ApproveSell(int id)
        {

            var sell = db.BaoDanTransactions.Find(id);
            if (sell != null)
            {
                //检查卖出报单状态和类型
                if (sell.Status == 报单状态.未成交.ToString() && sell.Type == 报单类型.卖出.ToString())
                {
                    var member = db.Members.Find(sell.Member.Id);
                    //更改状态 为已成交
                    sell.Status = 报单状态.已成交.ToString();
                    
                    //增加会员现金冻结记录 周一解冻
                    //2016 08 13 卖出资金不用冻结，直接加到可用资金
                    var tempAmount = sell.Amount * sell.Price - sell.Fee;
                    member.CashTransactions.Add(new CashTransaction
                    {
                        DateTime = DateTime.Now,
                        Type = 现金交易类型.积分出售.ToString(),
                        Status = 现金状态.解冻.ToString(),
                        Amount = tempAmount.Value,
                        Fee = 0m,
                        BaoDanTransactionId = sell.Id
                    });

                    //增加会员可用现金
                    member.Cash1 += tempAmount.Value;
                    db.Entry(member).State = EntityState.Modified;
                    db.Entry(sell).State = EntityState.Modified;

                    //By Feng 更新日系统统计表
                    UpdateOrInsertDailySysStatistics(sell);

                    db.SaveChanges();
                    ModelState.AddModelError("", "该记录不存在。");

                }
                else
                {
                    ModelState.AddModelError("", "审批成功。");
                }

            }
            else
            {
                ModelState.AddModelError("", "该记录不存在。");
            }
            //无此记录，跳转到首页
            
            TempData["ModelState"] = ModelState;
            return RedirectToAction("PendingSells");
        }

        public ActionResult DeleteBaodan(int id)
        {
            var baodan = db.BaoDanTransactions.Find(id);
            db.BaoDanTransactions.Remove(baodan);
            db.SaveChanges();
            ModelState.AddModelError("", "报单删除成功。");
            TempData["ModelState"] = ModelState;
            return RedirectToAction("PendingSells");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private void UpdateOrInsertDailySysStatistics(BaoDanTransaction mBaoDan)
        {
            //Please updated model and uncomment below
            
            DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            var mStatistics = db.SysStatistics.SingleOrDefault(m => m.Date == currentDate);
            if (mStatistics == null)
            {
                db.SysStatistics.Add(new SysStatistic
                {
                    Date = currentDate,
                    BaoDanBuyAmount = 0,
                    BaoDanSellAmount = mBaoDan.Amount,
                    TotalCashTransactionAmount = mBaoDan.Amount * mBaoDan.Price,
                    NewMemberAmount = 0,
                });
            }
            else
            {
                mStatistics.TotalCashTransactionAmount += mBaoDan.Amount * mBaoDan.Price;
                mStatistics.BaoDanSellAmount += mBaoDan.Amount;
            }
            
        }

    }
}
