using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Data.Entity.Validation;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using MemberCenter.Models;
using MemberCenter.Helper;
using VapLib;

namespace MemberCenter.Controllers
{
    public class PointsController : BaseController
    {

        //
        // GET: /Points/History
        public ActionResult History()
        {
            IEnumerable<ConsumptionViewModel> model = from row in CurrentUser.PointTransaction
                                                      orderby row.DateTime descending
                                                      select new ConsumptionViewModel
                                                      {
                                                          DateTime = row.DateTime,
                                                          Amount = row.Amount,
                                                          Comment = row.Comment,
                                                          Type = row.Type,
                                                      };
            SetMyAccountViewModel();
            return View(model);
        }

        //
        // GET: /Points/Transfer
        public ActionResult Transfer()
        {
            if (GetSystemSettingBoolean("SystemIsLocked"))
            {
                TempData["ActionMessage"] = "系统维护中，暂停交易！";
                return RedirectToAction("Error", "Message");
            }
            SetMyAccountViewModel();
            return View(new PointTransferViewModel
            {
                AvailableAmount = CurrentUser.Point1,
                MinRequestAmount = (int)GetSystemSettingDecimal("PointsRate"),
                MaxRequestAmount = (Int32)(Math.Floor(CurrentUser.Point1 / GetSystemSettingDecimal("PointsRate")) * GetSystemSettingDecimal("PointsRate"))
            });
        }

        //
        // POST: /Points/Transfer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Transfer(PointTransferViewModel model)
        {
            if (ModelState.IsValid)
            {
                Member mUser = null;
                bool hasError = false;
                //Validation,
                //Check current balance is much than request amount
                if (model.Amount > CurrentUser.Point1 || model.Amount < GetSystemSettingDecimal("PointsRate"))
                {
                    ModelState.AddModelError("", "转账数量有误，请重试！");
                    hasError = true;
                }

                if (!CurrentUser.Password2.Equals(model.Password))
                {
                    ModelState.AddModelError("", "交易密码错误");
                    hasError = true;
                }

                //Check target member exists
                if (model.User.IndexOf("@") > 0)
                {
                    mUser = db.Members.SingleOrDefault(m => m.Email.Equals(model.User, StringComparison.InvariantCultureIgnoreCase));
                    if (mUser == null)
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
                    catch (Exception e)
                    {
                        ModelState.AddModelError("", "找不到接受会员，请重试！");
                        hasError = true;
                    }
                }

                if (mUser != null && CurrentUser.Id == mUser.Id)
                {
                    ModelState.AddModelError("", "接受会员不能是自己，请重试！");
                    hasError = true;
                }

                if (mUser != null && !hasError)
                {
                    //Add Transaction record to db
                    CurrentUser.PointTransaction.Add(new PointTransaction
                    {
                        DateTime = DateTime.Now,
                        Amount = -model.Amount,
                        Type = 兑换券记录类型.会员转出.ToString(),
                        Status = 兑换券状态.可用.ToString(),
                        Comment = "转出至会员(UID:" + mUser.Id + ")",
                    });

                    mUser.PointTransaction.Add(new PointTransaction
                    {
                        DateTime = DateTime.Now,
                        Amount = model.Amount,
                        Type = 兑换券记录类型.会员转入.ToString(),
                        Status = 兑换券状态.可用.ToString(),
                        Comment = "会员(UID:" + CurrentUser.Id + ")转入",
                    });

                    //Calculate and update balance
                    CurrentUser.Point1 -= model.Amount;
                    mUser.Point1 += model.Amount;

                    db.SaveChanges();
                    ViewBag.ActionMessage = "购物券转账成功！";
                    TempData["ActionMessage"] = ViewBag.ActionMessage;
                    return RedirectToAction("Success", "Message");
                }
            }
            SetMyAccountViewModel();
            return View(new PointTransferViewModel
            {
                AvailableAmount = CurrentUser.Point1,
                MinRequestAmount = (int)GetSystemSettingDecimal("PointsRate"),
                MaxRequestAmount = (Int32)(Math.Floor(CurrentUser.Point1 / GetSystemSettingDecimal("PointsRate")) * GetSystemSettingDecimal("PointsRate"))
            });
        }

        //
        // GET: /Points/Buy
        public ActionResult Buy()
        {
            if (GetSystemSettingBoolean("SystemIsLocked"))
            {
                TempData["ActionMessage"] = "系统维护中，暂停交易！";
                return RedirectToAction("Error", "Message");
            }
            if (CurrentUser.Cash1 < GetSystemSettingDecimal("PointsRate"))
            {
                ModelState.AddModelError("", "对不起，您的账户可用资金不足￥" + GetSystemSettingDecimal("PointsRate").ToString("0,0.00") + "，请先充值。");
            }

            SetMyAccountViewModel();
            return View(CalculatePointBuyModel());
        }


        // POST: /Points/Buy
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Buy(PointBuyViewModel model)
        {
            if (GetSystemSettingBoolean("SystemIsLocked"))
            {
                TempData["ActionMessage"] = "系统维护中，暂停交易！";
                return RedirectToAction("Error", "Message");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var mMinRequestAmount = (int)GetSystemSettingDecimal("PointsRate");
                    var mMaxRequestAmount = (Int32)(Math.Floor(CurrentUser.Cash1 / GetSystemSettingDecimal("PointsRate")) * GetSystemSettingDecimal("PointsRate"));
                    if(model.RequestAmount > mMaxRequestAmount || model.RequestAmount < mMinRequestAmount)
                    {
                        ModelState.AddModelError("", "购买数量错误");
                        return View(CalculatePointBuyModel());
                    }
                    CurrentUser.CashTransaction.Add(new CashTransaction
                    {
                        DateTime = DateTime.Now,
                        Status = 现金状态.已审核.ToString(),
                        Type = 现金交易类型.购买兑换券.ToString(),
                        Amount = -model.RequestAmount,
                        Fee = 0,
                    });
                    CurrentUser.PointTransaction.Add(new PointTransaction
                    {
                        DateTime = DateTime.Now,
                        Amount = model.RequestAmount,
                        Type = 兑换券记录类型.现金买入.ToString(),
                        Status = 兑换券状态.可用.ToString(),
                        Comment = "",
                    });
                    CurrentUser.Cash1 -= model.RequestAmount;
                    CurrentUser.Point1 += model.RequestAmount;
                    db.SaveChanges();
                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            ModelState.AddModelError("", validationError.ToString());
                        }
                    }
                }
                ViewBag.ActionMessage = "购买成功！";
                TempData["ActionMessage"] = ViewBag.ActionMessage;
                return RedirectToAction("Success", "Message");
            }
            return View(CalculatePointBuyModel());
        }


        private PointBuyViewModel CalculatePointBuyModel()
        {
            return new PointBuyViewModel
            {
                AvailableCash = CurrentUser.Cash1,
                MinRequestAmount = (int)GetSystemSettingDecimal("PointsRate"),
                MaxRequestAmount = (Int32)(Math.Floor(CurrentUser.Cash1 / GetSystemSettingDecimal("PointsRate")) * GetSystemSettingDecimal("PointsRate"))
            };
        }

	}
}