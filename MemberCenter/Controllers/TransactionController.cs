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
using MemberCenter.Helper;
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
            if (GetSystemSettingBoolean("SystemIsLocked"))
            {
                TempData["ActionMessage"] = "系统维护中，暂停交易！";
                return RedirectToAction("Error", "Message");
            }
            return View(GetCashTopupViewModel());
        }

        //
        // POST: /Transaction/CashTopup
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CashTopup(CashTopupViewModel model)
        {
            if (ModelState.IsValid)
            {
                String bankInfoType = 银行账户信息类型.系统账户.ToString();
                BankInfo bankInfo= db.BankInfo.SingleOrDefault(m => m.Id == model.BankInfoId && m.Type.Equals(bankInfoType));
                if(bankInfo == null)
                {
                    ModelState.AddModelError("", "银行汇款信息错误");
                }
                else if (model.Amount < GetSystemSettingDecimal("CashTopupMin"))
                {
                    ModelState.AddModelError("", "充值金额错误，每次最少充值￥" + GetSystemSettingDecimal("CashTopupMin"));
                }
                else
                {
                    var requestHelper = new RequestHelper(this.Request);
                    try{
                        var filePath = requestHelper.SaveImageToServer(GetSystemSettingString("MemberUploadTopupFilePath"));
                        CurrentUser.CashTransaction.Add(new CashTransaction
                        {
                            DateTime = DateTime.Now,
                            Amount = model.Amount + model.Odd,
                            Fee = GetSystemSettingDecimal("CashTopupFee"),
                            Bank = bankInfo.Bank,
                            BankName = bankInfo.Name,
                            BankAccount = bankInfo.Account,
                            Type = 现金交易类型.充值.ToString(),
                            Status = 现金状态.待审核.ToString(),
                            Comment = model.Comment,
                            FileUrl = filePath,
                            RemitUserName = model.RemitUserName,
                            RemitNumber = model.RemitNumber
                        });
                        db.SaveChanges();
                        ViewBag.ActionMessage = "充值信息已提交，请等待审核！";
                        TempData["ActionMessage"] = ViewBag.ActionMessage;
                        return RedirectToAction("Success", "Message");
                    }catch(Exception e){
                        ModelState.AddModelError("", e.Message);
                    }
                    
                }
            }

            return View(GetCashTopupViewModel());
        }

        //
        // GET: /Transaction/CashWithdraw
        public ActionResult CashWithdraw()
        {
            if (GetSystemSettingBoolean("SystemIsLocked"))
            {
                TempData["ActionMessage"] = "系统维护中，暂停交易！";
                return RedirectToAction("Error", "Message");
            }
            return View(GetCashWithdrawViewModel());
        }

        //
        // POST: /Transaction/CashWithdraw
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CashWithdraw(CashWithdrawViewModel model)
        {
            if (ModelState.IsValid)
            {
                //计算当日已体现量
                String type = 现金交易类型.提现.ToString();
                DateTime stDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                DateTime edDate = stDate.AddDays(1);
                
                var transactionAmount = CurrentUser.CashTransaction.Where(m => type.Equals(m.Type) && m.DateTime >= stDate && m.DateTime <= edDate);
                decimal todayTransactionAmount = transactionAmount.Count() == 0 ? 0 : transactionAmount.Sum(m => m.Amount);

                if (Math.Abs(todayTransactionAmount) + model.RequestAmount > GetSystemSettingDecimal("DailyMaxWithdrawAmount"))
                {
                    ModelState.AddModelError("", "已超过单日最大提现限额");
                }
                else if (model.RequestAmount > CurrentUser.Cash1 || model.RequestAmount < GetSystemSettingDecimal("CashWithdrawMin") || model.RequestAmount > GetSystemSettingDecimal("CashWithdrawMax"))
                {
                    ModelState.AddModelError("", "提现金额有误(" + model.RequestAmount + ")，每笔最少￥" + GetSystemSettingDecimal("CashWithdrawMin") + "，最多￥" + GetSystemSettingDecimal("CashWithdrawMax"));
                }
                else if (CurrentUser.BankInfo.Count<=0)       
                {
                    ModelState.AddModelError("", "还没有设置提现银行帐号信息");
                }
                else if (!CurrentUser.Password2.Equals(model.Password))
                {
                    ModelState.AddModelError("", "交易密码错误");
                }
                else
                { 
                    //增加CashTransaction记录
                    BankInfo bankInfo = CurrentUser.BankInfo.SingleOrDefault();
                    CurrentUser.CashTransaction.Add(new CashTransaction
                    {
                        DateTime = DateTime.Now,
                        Type = 现金交易类型.提现.ToString(),
                        Status = 现金状态.待审核.ToString(),
                        Amount = -model.RequestAmount,
                        Fee = CalculateFee(model.RequestAmount, "CashWithdrawFee"),
                        Bank = bankInfo.Bank,
                        BankAccount = bankInfo.Account,
                        BankName = bankInfo.Name,
                    });
                    //修改自己Cash1 数值
                    CurrentUser.Cash1 -= model.RequestAmount;
                    db.SaveChanges();
                    ViewBag.ActionMessage = "提现申请已提交，等待审核！";
                    TempData["ActionMessage"] = ViewBag.ActionMessage;
                    return RedirectToAction("Success", "Message");
                }
            }
            return View(GetCashWithdrawViewModel());
        }

        //
        // GET: /Transaction/Transfer
        public ActionResult Transfer()
        {
            if (GetSystemSettingBoolean("SystemIsLocked"))
            {
                TempData["ActionMessage"] = "系统维护中，暂停交易！";
                return RedirectToAction("Error", "Message");
            }
            SetMyAccountViewModel();
            return View(new CashTransferViewModel { 
                AvailableAmount = CurrentUser.Cash1,
            });
        }

        //
        // POST: /Transaction/Transfer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Transfer(CashTransferViewModel model)
        {
            if (ModelState.IsValid)
            {
                Member mUser = null;
                bool hasError = false;
                //Validation,
                //Check current balance is much than request amount
                if(model.Amount > CurrentUser.Cash1 || model.Amount<=0)
                {
                    ModelState.AddModelError("", "转账金额有误，请重试！");
                    hasError = true;
                }

                if (!CurrentUser.Password2.Equals(model.Password))
                {
                    ModelState.AddModelError("", "交易密码错误");
                    hasError = true;
                }

                //Check target member exists
                if(model.User.IndexOf("@")>0)
                {
                    mUser = db.Members.SingleOrDefault(m => m.Email.Equals(model.User, StringComparison.InvariantCultureIgnoreCase));
                    if(mUser == null)
                    {
                        ModelState.AddModelError("", "找不到接受会员，请重试！");
                        hasError = true;
                    }
                }
                else
                {
                    try
                    {
                        int id = int.Parse(model.User);
                        mUser = db.Members.SingleOrDefault(m => m.Id == id);
                        if (mUser == null)
                        {
                            ModelState.AddModelError("", "找不到接受会员，请重试！");
                            hasError = true;
                        }
                    }
                    catch(Exception e)
                    {
                        ModelState.AddModelError("", "找不到接受会员，请重试！");
                        hasError = true;
                    }
                }

                if (mUser != null && !hasError)
                {
                    //Add Transaction record to db
                    CurrentUser.CashTransaction.Add(new CashTransaction
                    {
                        DateTime = DateTime.Now,
                        Amount = -model.Amount,
                        Type = 现金交易类型.会员转出.ToString(),
                        Status = 现金状态.已审核.ToString(),
                        Fee = 0,
                        Comment = "转出至会员(UID:" + mUser.Id + ")",
                    });

                    mUser.CashTransaction.Add(new CashTransaction
                    {
                        DateTime = DateTime.Now,
                        Amount = model.Amount,
                        Type = 现金交易类型.会员转入.ToString(),
                        Status = 现金状态.已审核.ToString(),
                        Fee = 0,
                        Comment = "会员(UID:" + CurrentUser.Id + ")转入",
                    });

                    //Calculate and update balance
                    CurrentUser.Cash1 -= model.Amount;
                    mUser.Cash1 += model.Amount;

                    db.SaveChanges();
                    ViewBag.ActionMessage = "资金转账成功！";
                    TempData["ActionMessage"] = ViewBag.ActionMessage;
                    return RedirectToAction("Success", "Message");
                }
            }
            SetMyAccountViewModel();
            return View(new CashTransferViewModel
            {
                AvailableAmount = CurrentUser.Cash1,
            });
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
            SetMyAccountViewModel();
            return View(model);
        }
       

        #region Private Methods

        private CashTopupViewModel GetCashTopupViewModel()
        {
            String bankInfoType = 银行账户信息类型.系统账户.ToString();
            CashTopupViewModel model = new CashTopupViewModel
            {
                Odd = GetDailyTopupNextOdd(),
                Amount = 0,
                SysBankInfos = from row in db.BankInfo
                               where row.Type.Equals(bankInfoType)
                               select new BankInfoViewModel
                               {
                                   Id = row.Id,
                                   Account = row.Account,
                                   Bank = row.Bank,
                                   Name = row.Name,
                                   Description = row.Description,
                                   URL = row.URL
                               },
            };
            SetMyAccountViewModel();
            return model;
        }


        private decimal GetDailyTopupNextOdd()
        {
            return 0.00m;
            /*
            DateTime stTime = new DateTime(DateTime.Now.Year,  DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            DateTime edTime = new DateTime(DateTime.Now.Year,  DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
            string transType = 现金交易类型.充值.ToString();
            int odd = db.CashTransactions.Count(m => m.DateTime >= stTime && m.DateTime <= edTime && m.Type.Equals(transType)) + 1;
            decimal odds = (odd % 100) * 0.01m;
            return odds;*/
        }


        private CashWithdrawViewModel GetCashWithdrawViewModel()
        {
            BankInfo mBankInfo = CurrentUser.BankInfo.OrderByDescending(m => m.Id).SingleOrDefault();
            int bankInfoId = mBankInfo == null ? 0 : mBankInfo.Id;
            CashWithdrawViewModel model = new CashWithdrawViewModel
            {
                RequestAmount = 0,
                AvailableAmount = CurrentUser.Cash1,
                BankInfoId = bankInfoId,
                MaxWithdrawAmount = GetSystemSettingDecimal("CashWithdrawMax"),
                MinWithdrawAmount = GetSystemSettingDecimal("CashWithdrawMin"),
                DailyMaxWithdrawAmount = GetSystemSettingDecimal("DailyMaxWithdrawAmount"),
                FeeSetting = GetSystemSettingString("CashWithdrawFee"),
            };

            String type = 现金交易类型.提现.ToString();
            String statusDone = 现金状态.已审核.ToString();
            String statusPending = 现金状态.待审核.ToString();
            model.WithdrawHistory = from row in CurrentUser.CashTransaction
                                    where row.Type.Equals(type) && row.Status.Equals(statusPending)
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
            SetMyAccountViewModel();
            return model;
        }

        #endregion
    }
}
