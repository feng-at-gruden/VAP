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
    [Authorize]
    public class BaoDanController : BaseController
    {
        
        //
        // GET: /BaoDan/Buy
        public ActionResult Buy()
        {
            if (GetSystemSettingBoolean("SystemIsLocked"))
            {
                TempData["ActionMessage"] = "系统维护中，暂停交易！";
                return RedirectToAction("Error", "Message");
            }
            if (CurrentUser.Cash1 < GetSystemSettingDecimal("MinBaoDanCashBalance") + GetSystemSettingDecimal("BaoDanBuyFee"))
            {
                ModelState.AddModelError("", "对不起，您的账户可用资金不足￥" + (GetSystemSettingDecimal("MinBaoDanCashBalance") + GetSystemSettingDecimal("BaoDanBuyFee")).ToString("0,0.00") + "，请先充值。");
            }

            return View(CalculateBaoDanBuyModel());
        }

        // POST: /BaoDan/Buy
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Buy(BaoDanBuyViewModel model)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    //Step 0. Prevent hack, do validation
                    //Force overwrite model to prevent hack
                    var realModel = CalculateBaoDanBuyModel();
                    if(!CurrentUser.Status.Equals(会员状态.正常.ToString()))
                    {
                        ModelState.AddModelError("", "会员状态异常");
                        return RedirectToAction("Login", "Account");
                    }
                    if (CurrentUser.Cash1 < model.TotalCostCash || CurrentUser.Cash1 < GetSystemSettingDecimal("MinBaoDanCashBalance"))
                    {
                        ModelState.AddModelError("", "账户可用资金不足");
                        return View(CalculateBaoDanBuyModel());
                    }
                    if (model.RequestQuantity <= 0)
                    {
                        ModelState.AddModelError("", "报单数量错误");
                        return View(CalculateBaoDanBuyModel());
                    }

                    // Do Validation
                    if (model.RequestPrice != realModel.RequestPrice ||
                        model.RequestQuantity > realModel.RequestQuantity ||
                        model.RequestCash > realModel.RequestCash )
                    {
                        ModelState.AddModelError("", "报单数据错误， 请返回重试");
                        return View(CalculateBaoDanBuyModel());
                    }

                    decimal requestQty = model.RequestCash / CurrentCoinPrice.Price;
                    decimal totalCash = model.RequestCash + GetSystemSettingDecimal("BaoDanBuyFee");

                    //Step 1. 报单交易记录、现金交易记录
                    BaoDanTransaction mBaoDan = new BaoDanTransaction
                    {
                        DateTime = DateTime.Now,
                        Amount = requestQty,
                        Price = CurrentCoinPrice.Price,
                        Fee = GetSystemSettingDecimal("BaoDanBuyFee"),
                        Status = 报单状态.已成交.ToString(),
                        Type = 报单类型.买入.ToString(),
                        Member = CurrentUser,
                    };
                    CurrentUser.BaoDanTransaction.Add(mBaoDan);
                    CurrentUser.CashTransaction.Add(new CashTransaction
                    {
                        DateTime = DateTime.Now,
                        Status = 现金状态.已审核.ToString(),
                        Type = 现金交易类型.购买积分.ToString(),
                        Amount = -totalCash,
                        Fee = 0,
                        BaoDanTransaction = mBaoDan,
                    });

                    //Step 1.5 增加虚拟币 并冻结
                    CurrentUser.Coin2 += requestQty;
                    CurrentUser.LockedCoin.Add(new LockedCoin
                    {
                        AvailabeAmount = 0,
                        LockedAmount = requestQty,
                        TotalAmount = requestQty,
                        Price = CurrentCoinPrice.Price,
                        LastPrice = CurrentCoinPrice.Price,
                        NextPrice = Math.Ceiling(CurrentCoinPrice.Price * 1.05m * 1000) / 1000,
                        BaoDanTransaction = mBaoDan,
                    });

                    //Step 2.0 为自己增加积分
                    decimal points = (model.RequestCash / GetSystemSettingDecimal("MinBaoDanCashBalance")) * GetSystemSettingDecimal("PointsRate");
                    CurrentUser.Point1 += points;
                    CurrentUser.PointTransaction.Add(new PointTransaction
                    {
                        DateTime = DateTime.Now,
                        Amount = points,
                        Type = 积分记录类型.购买积分.ToString(),
                        Status = 积分状态.可用.ToString(),
                        BaoDanTransaction = mBaoDan,
                    });

                    //Step 2.5 为自己增加总业绩
                    CurrentUser.Achievement += model.RequestCash;

                    //Step 3.0 设置更新自己等级
                    RefreshMemberLevel(CurrentUser);

                    //Step 3.1 为自己所有上线增加返利
                    //Step 3.2 为上线增加重消记录
                    RefundForReferral(CurrentUser, model.RequestCash, mBaoDan, 0);

                    //Step 3.3 为上线增加总业绩
                    //Step 3.4 设置更新上线等级
                    UpdateReferralAchievementAndLevel(CurrentUser, model.RequestCash, mBaoDan);


                    //Step 4. 扣除现金
                    CurrentUser.Cash1 -= totalCash;

                    //Step 5. 更新系统统计表
                    UpdateOrInsertSysStatistics(mBaoDan);

                    db.SaveChanges();
                    ViewBag.ActionMessage = "报单成功！";
                    TempData["ActionMessage"] = ViewBag.ActionMessage;
                    return RedirectToAction("Success", "Message");
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
            }
            return View(CalculateBaoDanBuyModel());
        }

        //
        // GET: /BaoDan/Sell
        public ActionResult Sell()
        {
            if (GetSystemSettingBoolean("SystemIsLocked"))
            {
                TempData["ActionMessage"] = "系统维护中，暂停交易！";
                return RedirectToAction("Error", "Message");
            }
            return View(GetBaoDanSellModel());
        }

        // POST: /BaoDan/Sell
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Sell(BaoDanSellViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.RequestAmount > CurrentUser.Coin1 || model.RequestAmount < GetSystemSettingDecimal("MinBaoDanSell"))
                {
                    ModelState.AddModelError("", "报单数量错误(" + model.RequestAmount + ")");
                }
                else if (!CurrentUser.Password2.Equals(model.Password))
                {
                    ModelState.AddModelError("", "交易密码错误");
                }
                else
                {
                    //Step 增加报单记录
                    CurrentUser.BaoDanTransaction.Add(new BaoDanTransaction
                    {
                        DateTime = DateTime.Now,
                        Amount = model.RequestAmount,
                        Price = CurrentCoinPrice.Price,
                        Fee = GetSystemSettingDecimal("BaoDanSellFee"),
                        Status = 报单状态.未成交.ToString(),
                        Type = 报单类型.卖出.ToString(),
                    });

                    //Step 减少Coin1 
                    CurrentUser.Coin1 -= model.RequestAmount;
                    db.SaveChanges();
                    ViewBag.ActionMessage = "报单已提交，等待审核！";
                    TempData["ActionMessage"] = ViewBag.ActionMessage;
                    return RedirectToAction("Success", "Message");
                    //NOTE: Admin后台操作， (1)审核通过BaoDanTransaction记录, (2)增加CashTransaction记录, (3)更新Member.Cash2 
                }
            }

            return View(GetBaoDanSellModel());
        }

        //
        // GET: /BaoDan/Cancel/123
        public ActionResult Cancel(int id)
        {
            BaoDanTransaction mBaoDan = CurrentUser.BaoDanTransaction.SingleOrDefault(m => m.Id == id);
            if (mBaoDan!=null)
            {
                mBaoDan.Status = 报单状态.用户已取消.ToString();
                CurrentUser.Coin1 += mBaoDan.Amount;
                db.SaveChanges();
            }
            else
            {
                ModelState.AddModelError("", "找不到该报单记录！");
            }
            
            return RedirectToAction("Sell", "BaoDan");
        }

        //
        // GET: /BaoDan/LockedCoins
        public ActionResult LockedCoins()
        {
            IEnumerable<LockedCoinsViewModel> model = from row in CurrentUser.LockedCoin
                                                     orderby row.BaoDanTransaction.DateTime
                                                     select new LockedCoinsViewModel
                                                     {
                                                         BaoDanId = row.BaoDanTransaction.Id,
                                                         BaoDanPrice = row.Price,
                                                         BaoDanQuantity = row.TotalAmount,
                                                         BaoDanTime = row.BaoDanTransaction.DateTime,
                                                         LastPrice = row.LastPrice,
                                                         NextPrice = row.NextPrice,
                                                         AvailableAmount = row.AvailabeAmount,
                                                         LockedAmount = row.LockedAmount,
                                                         AmountLeft = row.TotalAmount - row.AvailabeAmount,
                                                         Status = row.TotalAmount>row.AvailabeAmount? "解冻中" :"全部解冻",
                                                     };
            SetMyAccountViewModel();
            return View(model);
        }

        //
        // GET: /BaoDan/History
        public ActionResult History()
        {
            String status = 报单状态.已成交.ToString();
            IEnumerable<BaoDanHistoryViewModel> model = from row in CurrentUser.BaoDanTransaction
                                                        where row.Status.Equals(status)
                                                        orderby row.DateTime descending
                                                        select new BaoDanHistoryViewModel
                                                        {
                                                            Id = row.Id,
                                                            Type = row.Type,
                                                            Status = row.Status,
                                                            BaoDanTime = row.DateTime,
                                                            Fee = row.Fee,
                                                            RequestQuantity = row.Amount,
                                                            RequestPrice = row.Price,
                                                            RequestCash = row.Amount * row.Price,
                                                            FinalQuantity = row.Amount,
                                                            FinalCash = row.Amount * row.Price + row.Fee,
                                                            FinalPrice = (row.Amount * row.Price + row.Fee) / row.Amount,
                                                        };
            SetMyAccountViewModel();
            return View(model);
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
            return View(new BaoDanTransferViewModel { 
                AvailableAmount = CurrentUser.Coin1,
            });
        }

        //
        // POST: /Transaction/Transfer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Transfer(BaoDanTransferViewModel model)
        {
            if (ModelState.IsValid)
            {
                Member mUser = null;
                bool hasError = false;
                //Validation,
                //Check current balance is much than request amount
                if (model.Amount > CurrentUser.Coin1 || model.Amount <= 0)
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

                if (mUser != null && !hasError)
                {
                    //Add Transaction record to db
                    CurrentUser.BaoDanTransaction.Add(new BaoDanTransaction
                    {
                        DateTime = DateTime.Now,
                        Amount = -model.Amount,
                        Type = 报单类型.会员转出.ToString(),
                        Status = 报单状态.已成交.ToString(),
                        Fee = 0,
                        Comment = "转出至会员(UID:" + mUser.Id + ")",
                    });

                    mUser.BaoDanTransaction.Add(new BaoDanTransaction
                    {
                        DateTime = DateTime.Now,
                        Amount = model.Amount,
                        Type = 报单类型.会员转入.ToString(),
                        Status = 报单状态.已成交.ToString(),
                        Fee = 0,
                        Comment = "会员(UID:" + CurrentUser.Id + ")转入",
                    });

                    //Calculate and update balance
                    CurrentUser.Coin1 -= model.Amount;
                    mUser.Coin1 += model.Amount;

                    db.SaveChanges();
                    ViewBag.ActionMessage = "积分转账成功！";
                    TempData["ActionMessage"] = ViewBag.ActionMessage;
                    return RedirectToAction("Success", "Message");
                }
            }
            SetMyAccountViewModel();
            return View(new BaoDanTransferViewModel
            {
                AvailableAmount = CurrentUser.Coin1,
            });
        }


        #region private methods

        /// <summary>
        /// 计算可购币数及ViewModel值
        /// </summary>
        /// <returns></returns>
        private BaoDanBuyViewModel CalculateBaoDanBuyModel()
        {
            int maxRequestCash = (int)Math.Floor((CurrentUser.Cash1 - GetSystemSettingDecimal("BaoDanBuyFee")) / GetSystemSettingDecimal("MinBaoDanCashBalance"));
            decimal coinCash = maxRequestCash * GetSystemSettingDecimal("MinBaoDanCashBalance");
            decimal price = CurrentCoinPrice.Price;
            decimal qty = coinCash / price;

            BaoDanBuyViewModel model = new BaoDanBuyViewModel
            {
                CurrentCoinPrice = CurrentCoinPrice.Price,
                RequestPrice = CurrentCoinPrice.Price,
                AvailableCash = CurrentUser.Cash1,
                RequestQuantity = qty,
                RequestCash = coinCash,
                MaxRequestCash = maxRequestCash,
                Fee = GetSystemSettingDecimal("BaoDanBuyFee"),
                TotalCostCash = coinCash + GetSystemSettingDecimal("BaoDanBuyFee"),
                CashLeft = CurrentUser.Cash1 - coinCash - GetSystemSettingDecimal("BaoDanBuyFee"),
            };
            SetMyAccountViewModel();
            return model;
        }


        private BaoDanSellViewModel GetBaoDanSellModel()
        {
            BaoDanSellViewModel model = new BaoDanSellViewModel
            {
                CurrentPrice = CurrentCoinPrice.Price,
                AvailabeAmount = CurrentUser.Coin1,
                RequestAmount = 0,
                Fee = GetSystemSettingDecimal("BaoDanSellFee"),
            };

            String status = 报单状态.未成交.ToString();
            String type = 报单类型.卖出.ToString();
            model.RecentPendingRequests = from row in CurrentUser.BaoDanTransaction
                                                        where row.Status.Equals(status) && row.Type.Equals(type)
                                                        orderby row.DateTime descending
                                                        select new BaoDanHistoryViewModel
                                                        {
                                                            Id = row.Id,
                                                            Type = row.Type,
                                                            Status = row.Status,
                                                            BaoDanTime = row.DateTime,
                                                            Fee = row.Fee,
                                                            RequestQuantity = row.Amount,
                                                            RequestPrice = row.Price,
                                                            RequestCash = row.Amount * row.Price,
                                                            FinalQuantity = row.Amount,
                                                            FinalCash = row.Amount * row.Price + row.Fee,
                                                            FinalPrice = (row.Amount * row.Price + row.Fee) / row.Amount,
                                                        };
            SetMyAccountViewModel();
            return model;
        }

        /// <summary>
        /// 根据用户等级规则 计算更新用户等级
        /// </summary>
        /// <param name="member"></param>
        private void RefreshMemberLevel(Member member)
        {
            if (member.MemberLevel.Level.Equals(会员等级.七钻.ToString()))
                return;

            int nLevelId = member.MemberLevel.Id + 1;
            MemberLevel nextLevel = db.MemberLevel.SingleOrDefault(m => m.Id == nLevelId);
            if (nextLevel == null)
                return;

            if (member.Achievement < nextLevel.Achievement)
                return;

            if (nextLevel.MemberCount > 0)
            {
                int myValidMemberCount = 0;
                foreach(Member myMember in member.MyMembers)
                {
                    if (myMember.Achievement >= nextLevel.MemberAchievement)
                        myValidMemberCount++;
                }
                if (myValidMemberCount >= nextLevel.MemberCount)
                    member.MemberLevel = nextLevel;
                else
                    return;
            }
            else
            {
                member.MemberLevel = nextLevel;
            }

            RefreshMemberLevel(member);
        }

        /// <summary>
        /// Step 3.1 为自己所有上线增加返利
        /// Step 3.2 为上线增加重消记录
        /// </summary>
        /// <param name="member"></param>
        /// <param name="lastRefundRate">向上遍历 如果某上线的级别等于或低于其自己 则跳过其上线, lastRefundRate为上一次等级高于自己的上线的rate</param>
        private void RefundForReferral(Member member, decimal amount, BaoDanTransaction mBaoDan, decimal currentTotalRefundRate)
        {
            Member mRef = member.Referral;
            if (mRef == null)
                return;

            string mBaoDanBuyStatus = 报单类型.买入.ToString();
            decimal currentRefundRate = 0;
            
            //计算mRef返利比例
            if (GetSystemSettingBoolean("EnableRefundOnlyForActivateUser") && mRef.BaoDanTransaction.Count(m => m.Type == mBaoDanBuyStatus) <= 0)
            {
                //上线从未报过单 则跳过
            }
            else
            {
                if (member.Id == mBaoDan.Member.Id)
                {
                    //直接上线
                    currentRefundRate = mRef.MemberLevel.RefundRate;
                }
                else
                {
                    if (member.MemberLevel.RefundRate >= mRef.MemberLevel.RefundRate)
                    {
                        //如果某上线的级别等于或低于其自己 
                        //Nothing
                    }
                    else
                    {
                        currentRefundRate = mRef.MemberLevel.RefundRate - currentTotalRefundRate;
                    }
                }

                if (currentRefundRate > 0)
                {
                    //保存记录
                    decimal refTotalRefund = currentRefundRate * GetCorrectSettingPercentValue("PV") * amount;

                    // Step 3.1 为自己所有上线增加返利
                    decimal refRfund = refTotalRefund * (1 - GetCorrectSettingPercentValue("ChongXiaoRate"));
                    mRef.CashTransaction.Add(new CashTransaction
                    {
                        DateTime = DateTime.Now,
                        Status = 现金状态.冻结.ToString(),
                        Type = 现金交易类型.下线返利.ToString(),
                        Amount = refRfund,
                        Fee = 0,
                        BaoDanTransaction = mBaoDan,
                    });
                    mRef.Cash2 += refRfund;

                    // Step 3.2 为上线增加重消记录
                    decimal refChonXiao = refTotalRefund * GetCorrectSettingPercentValue("ChongXiaoRate");
                    mRef.ChongXiaoTransaction.Add(new ChongXiaoTransaction
                    {
                        DateTime = DateTime.Now,
                        Status = 重消状态.可用.ToString(),
                        Type = 重消记录类型.下线返利.ToString(),
                        Amount = refChonXiao,
                        BaoDanTransaction = mBaoDan,
                    });
                    mRef.ChongXiao1 += refChonXiao;
                }
                
            }

            // 直到最高价 七钻结束
            if (mRef.MemberLevel.Level.Equals(会员等级.七钻.ToString()))
            {
                return;
            }

            RefundForReferral(mRef, amount, mBaoDan, currentTotalRefundRate + currentRefundRate);
        }

        /// <summary>
        /// Step 3.3 为上线增加总业绩
        /// Step 3.4 设置更新上线等级
        /// </summary>
        private void UpdateReferralAchievementAndLevel(Member member, decimal amount, BaoDanTransaction mBaoDan)
        {
            Member mRef = member.Referral;
            if (mRef == null)
                return;

            // Step 3.3 为上线增加总业绩
            string mBaoDanBuyStatus = 报单类型.买入.ToString();
            if (!GetSystemSettingBoolean("EnableRefundOnlyForActivateUser") || mRef.BaoDanTransaction.Count(m => m.Type == mBaoDanBuyStatus) > 0)
                mRef.Achievement += amount;   // 各个上线总业绩 + 消费现金金额

            // Step 3.4 设置更新上线等级
            RefreshMemberLevel(mRef);

            UpdateReferralAchievementAndLevel(mRef, amount, mBaoDan);
        }

        /// <summary>
        /// 更新系统统计表 统计每天 买入 卖出量及交易金额
        /// </summary>
        /// <param name="mBaoDan"></param>
        private void UpdateOrInsertSysStatistics(BaoDanTransaction mBaoDan)
        {
            DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            SysStatistics mStatistics = db.SysStatistics.SingleOrDefault(m => m.Date == currentDate);
            if (mStatistics == null)
            {
                db.SysStatistics.Add(new SysStatistics
                {
                    Date = currentDate,
                    BaoDanBuyAmount = mBaoDan.Amount,
                    BaoDanSellAmount = 0,
                    TotalCashTransactionAmount = mBaoDan.Amount * mBaoDan.Price,
                    NewMemberAmount = 0,
                });
            }
            else
            {
                mStatistics.TotalCashTransactionAmount += mBaoDan.Amount * mBaoDan.Price;
                mStatistics.BaoDanBuyAmount += mBaoDan.Amount;
            }
        }

        #endregion

    }
}