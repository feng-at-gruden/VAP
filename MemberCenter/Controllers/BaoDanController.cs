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
            var cPrice = db.CoinPrices.OrderByDescending(m => m.DateTime).Take(1);
            if (cPrice == null || cPrice.Count()==0)
            {
                ModelState.AddModelError("", "当前系统没有足够虚拟币！");
                return View(new BaoDanBuyViewModel());
            }
            if (CurrentUser.Cash1 < Constants.MinCashBalance)
            {
                ModelState.AddModelError("", "对不起，您的账户可用资金不足￥" + Constants.MinCashBalance.ToString("0,0.00") + "，请先充值。");
                return View(new BaoDanBuyViewModel());
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
                    if(!CurrentUser.Status.Equals(会员状态.正常.ToString()))
                    {
                        ModelState.AddModelError("", "会员状态异常");
                        return RedirectToAction("Login", "Account");
                    }
                    if (CurrentUser.Cash1 < model.TotalCostCash)
                    {
                        ModelState.AddModelError("", "报单金额错误");
                        return View(CalculateBaoDanBuyModel());
                    }

                    //Step 1. 报单交易记录、现金交易记录
                    BaoDanTransaction mBaoDan = new BaoDanTransaction
                    {
                        DateTime = DateTime.Now,
                        Amount = model.RequestQuantity,
                        Price = model.RequestPrice,
                        Fee = Constants.BaoDanBuyFee,
                        Status = 报单状态.已成交.ToString(),
                        Type = 报单类型.买入.ToString(),
                        Member = CurrentUser,
                    };
                    CurrentUser.BaoDanTransaction.Add(mBaoDan);
                    CurrentUser.CashTransaction.Add(new CashTransaction
                    {
                        DateTime = DateTime.Now,
                        Status = 现金状态.可用.ToString(),
                        Type = 现金交易类型.购币消费.ToString(),
                        Amount = -model.TotalCostCash,
                        Fee = 0,
                        BaoDanTransaction = mBaoDan,
                    });

                    //Step 1.5 增加虚拟币 并冻结
                    CurrentUser.Coin2 += model.RequestQuantity;
                    CurrentUser.LockedCoin.Add(new LockedCoin
                    {
                        AvailabeAmount = 0,
                        LockedAmount = model.RequestQuantity,
                        TotalAmount = model.RequestQuantity,
                        Price = model.RequestPrice,
                        LastPrice = model.RequestPrice,
                        NextPrice = model.RequestPrice * 1.05m,
                        BaoDanTransaction = mBaoDan,
                    });

                    //Step 2.0 为自己增加积分
                    decimal points = (model.RequestCash / Constants.MinCashBalance) * Constants.PointsRate;
                    CurrentUser.Point2 += points;
                    CurrentUser.PointTransaction.Add(new PointTransaction
                    {
                        DateTime = DateTime.Now,
                        Amount = points,
                        Type = 积分记录类型.购币所得积分.ToString(),
                        Status = 积分状态.冻结.ToString(),
                        BaoDanTransaction = mBaoDan,
                    });

                    //Step 2.5 为自己增加总业绩
                    CurrentUser.Achievement += model.RequestCash;

                    //Step 3.0 设置更新自己等级
                    RefreshMemberLevel(CurrentUser);

                    //Step 3.1 为自己所有上线增加返利
                    //Step 3.2 为上线增加重消记录
                    //Step 3.3 为上线增加总业绩
                    //Step 3.4 设置更新上线等级
                    RefundForReferral(CurrentUser, model.RequestCash, mBaoDan, null);


                    //Step 4. 扣除现金
                    CurrentUser.Cash1 -= model.TotalCostCash;

                    //
                    db.SaveChanges();
                    ViewBag.ActionMessage = "报单成功！";
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
            return View();
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
            return View(model);
        }

        //
        // GET: /BaoDan/History
        public ActionResult History()
        {
            IEnumerable<BaoDanHistoryViewModel> model = from row in CurrentUser.BaoDanTransaction
                                                      orderby row.DateTime
                                                      select new BaoDanHistoryViewModel
                                                      {
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
            return View(model);
        }



        #region private methods

        /// <summary>
        /// 计算可购币数及ViewModel值
        /// </summary>
        /// <returns></returns>
        private BaoDanBuyViewModel CalculateBaoDanBuyModel()
        {
            CoinPrice cPrice = db.CoinPrices.OrderByDescending(m => m.DateTime).Take(1).ToArray()[0];
            decimal coinCash = Math.Floor(CurrentUser.Cash1 / Constants.MinCashBalance) * Constants.MinCashBalance;
            decimal price = cPrice.Price;
            decimal qty = coinCash / price;

            BaoDanBuyViewModel model = new BaoDanBuyViewModel
            {
                CurrentCoinPrice = cPrice.Price,
                RequestPrice = cPrice.Price,
                AvailableCash = CurrentUser.Cash1,
                RequestQuantity = qty,
                RequestCash = coinCash,
                Fee = Constants.BaoDanBuyFee,
                TotalCostCash = coinCash + Constants.BaoDanBuyFee,
                CashLeft = CurrentUser.Cash1 - coinCash - Constants.BaoDanBuyFee,
            };
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
        /// Step 3.3 为上线增加总业绩
        /// Step 3.4 设置更新上线等级
        /// </summary>
        /// <param name="member"></param>
        /// <param name="lastRefundRate">向上遍历 如果某上线的级别等于或低于其自己 则跳过其上线, lastRefundRate为上一次等级高于自己的上线的rate</param>
        private void RefundForReferral(Member member, decimal amount, BaoDanTransaction mBaoDan, decimal? lastRefundRate)
        {
            Member mRef = member.Referral;
            if (mRef == null)
                return;

            decimal? currentRefundRate = lastRefundRate.HasValue ? lastRefundRate : null;
            if (member.Id != mBaoDan.Member.Id && member.MemberLevel.RefundRate >= mRef.MemberLevel.RefundRate)
            {
                //向上遍历 如果某上线的级别等于或低于其自己 则跳过其上线
                //Nothing
            }
            else
            {
                //直接上线获利最多
                decimal finalRefundRate = (member.Id == mBaoDan.Member.Id) ? mRef.MemberLevel.RefundRate : (mRef.MemberLevel.RefundRate - Math.Max(member.MemberLevel.RefundRate, lastRefundRate.Value));
                decimal refTotalRefund = finalRefundRate * Constants.PV * amount;
                currentRefundRate = finalRefundRate;

                // Step 3.1 为自己所有上线增加返利
                decimal refRfund = refTotalRefund * (1 - Constants.ChongXiaoRate);
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
                decimal refChonXiao = refTotalRefund * Constants.ChongXiaoRate;
                mRef.ChongXiaoTransaction.Add(new ChongXiaoTransaction
                    {
                        DateTime = DateTime.Now,
                        Status = 重消状态.冻结.ToString(),
                        Type = 重消记录类型.下线返利重消.ToString(),
                        Amount = refChonXiao,
                        BaoDanTransaction = mBaoDan,
                    });
                mRef.ChongXiao2 = refChonXiao;

                // Step 3.3 为上线增加总业绩
                //mRef.Achievement += refTotalRefund;   去掉！ 下线返利不计算入总业绩

                // Step 3.4 设置更新上线等级
                RefreshMemberLevel(mRef);

                // 直到最高价 七钻结束
                if (mRef.MemberLevel.Level.Equals(会员等级.七钻.ToString()))
                {
                    return;
                }
            }

            RefundForReferral(mRef, amount, mBaoDan, currentRefundRate);
        }

        #endregion

    }
}