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
                        });
                        db.SaveChanges();
                        ViewBag.ActionMessage = "充值信息已提交，请等待审核！";
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
                if (model.RequestAmount > CurrentUser.Cash1 || model.RequestAmount < GetSystemSettingDecimal("CashWithdrawMin") || model.RequestAmount > GetSystemSettingDecimal("CashWithdrawMax"))
                {
                    ModelState.AddModelError("", "提现金额有误(" + model.RequestAmount + ")");
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
                        Fee = GetSystemSettingDecimal("CashWithdrawFee"),
                        Bank = bankInfo.Bank,
                        BankAccount = bankInfo.Account,
                        BankName = bankInfo.Name,
                    });
                    //修改自己Cash1 数值
                    CurrentUser.Cash1 -= model.RequestAmount;
                    db.SaveChanges();
                    ViewBag.ActionMessage = "提现申请已提交，等待审核！";
                }
            }
            return View(GetCashWithdrawViewModel());
        }

        //
        // GET: /Transaction/Transfer
        public ActionResult Transfer()
        {
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
                        Type = 现金交易类型.会员间转出.ToString(),
                        Status = 现金状态.已审核.ToString(),
                        Fee = 0,
                        Comment = "转出至会员(UID:" + mUser.Id + ")",
                    });

                    mUser.CashTransaction.Add(new CashTransaction
                    {
                        DateTime = DateTime.Now,
                        Amount = model.Amount,
                        Type = 现金交易类型.会员间转入.ToString(),
                        Status = 现金状态.已审核.ToString(),
                        Fee = 0,
                        Comment = "会员(UID:" + CurrentUser.Id + ")转入",
                    });

                    //Calculate and update balance
                    CurrentUser.Cash1 -= model.Amount;
                    mUser.Cash1 += model.Amount;

                    db.SaveChanges();
                    ViewBag.ActionMessage = "资金转账成功！";
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
            DateTime stTime = new DateTime(DateTime.Now.Year,  DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            DateTime edTime = new DateTime(DateTime.Now.Year,  DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
            string transType = 现金交易类型.充值.ToString();
            int odd = db.CashTransactions.Count(m => m.DateTime >= stTime && m.DateTime <= edTime && m.Type.Equals(transType)) + 1;
            decimal odds = (odd % 100) * 0.01m;
            return odds;
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
                Fee = GetSystemSettingDecimal("CashWithdrawFee"),
            };

            String type = 现金交易类型.提现.ToString();
            String statusDone = 现金状态.已审核.ToString();
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
            SetMyAccountViewModel();
            return model;
        }

        #endregion
    }
}
