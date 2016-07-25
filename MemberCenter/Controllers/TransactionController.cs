using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MemberCenter;
using MemberCenter.Models;
using VapLib;

namespace MemberCenter.Controllers
{
    [Authorize]
    public class TransactionController : BaseController
    {

        //
        // GET: /Transaction/CashTopup
        public ActionResult CashTopup()
        {
            
            CashTopupViewModel model = new CashTopupViewModel {
            };
            return View(model);
        }

        //
        // POST: /Transaction/CashTopup
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CashTopup(CashTopupViewModel model)
        {
            if (ModelState.IsValid)
            {
                //TODO
                //db.Entry(cashtransaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            return View();
        }

        //
        // GET: /Transaction/CashWithdraw
        public ActionResult CashWithdraw()
        {
            return View(GetCashWithdrawViewMode());
        }



        //
        // GET: /Transaction/History
        public ActionResult History()
        {
            IEnumerable<CashTransactionViewModel> model = from row in CurrentUser.CashTransaction
                                                            orderby row.DateTime descending
                                                          select new CashTransactionViewModel
                                                        {
                                                            Type = row.Type,
                                                            Status = row.Status,
                                                            DateTime = row.DateTime,
                                                            Fee = row.Fee,
                                                            Amount = row.Amount,
                                                            ID = row.Id,
                                                        };
            return View(model);
        }
       

        #region Private Methods

        private CashWithdrawViewModel GetCashWithdrawViewMode()
        {
            BankInfo mBankInfo = CurrentUser.BankInfo.OrderByDescending(m => m.Id).SingleOrDefault();
            int bankInfoId = mBankInfo == null ? 0 : mBankInfo.Id;
            CashWithdrawViewModel model = new CashWithdrawViewModel
            {
                RequestAmount = 0,      //TODO
                AvailableAmount = CurrentUser.Cash1,
                BankInfoId = bankInfoId,
                MaxWithdrawAmount = Constants.CashWithdrawMax,
                MinWithdrawAmount = Constants.CashWithdrawMin,
                Fee = Constants.CashWithdrawFee,
            };

            String type = 现金交易类型.提现.ToString();
            String statusDone = 现金状态.可用.ToString();
            model.WithdrawHistory = from row in CurrentUser.CashTransaction
                                          where row.Type.Equals(type)
                                          orderby row.DateTime descending
                                          select new CashWithdrawHistoryViewModel
                                          {
                                              Status = statusDone.Equals(row.Status)? "已处理": "未处理",
                                              WithdrawTime = row.DateTime,
                                              Fee = row.Fee,
                                              Amount = row.Amount,
                                              Bank = row.Bank,
                                              BankAccount = row.BankAccount,
                                          };
            return model;
        }

        #endregion
    }
}
