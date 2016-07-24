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

                    //Step 0. 报单交易记录、现金交易记录
                    BaoDanTransaction mBaoDan = new BaoDanTransaction
                    {
                        DateTime = DateTime.Now,
                        Amount = model.RequestQuantity,
                        Price = model.RequestPrice,
                        Fee = Constants.BaoDanBuyFee,
                        Status = 报单状态.已成交.ToString(),
                        Type = 报单类型.买入.ToString(),
                    };
                    CurrentUser.BaoDanTransaction.Add(mBaoDan);
                    CurrentUser.CashTransaction.Add(new CashTransaction
                    {
                        DateTime = DateTime.Now,
                        Status = 现金状态.可用.ToString(),
                        Type = 现金交易类型.购币.ToString(),
                        Amount = model.TotalCostCash,
                        Fee = 0,
                        BaoDanTransaction = mBaoDan, 
                    });

                    //Step 1. 增加虚拟币 并冻结
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

                    //Step 2. 为自己增加积分
                    decimal points = (model.TotalCostCash / Constants.MinCashBalance) * Constants.PointsRate;
                    CurrentUser.Point2 += points;
                    CurrentUser.PointTransaction.Add(new PointTransaction
                    {
                        DateTime = DateTime.Now,
                        Amount = points,
                        Type = 积分记录类型.购币所得积分.ToString(),
                        Status = 积分状态.冻结.ToString(),
                        BaoDanTransaction = mBaoDan,
                    });

                    //Step 2.5 为自己增加成就
                    CurrentUser.Achievement += model.TotalCostCash;

                    //Step 3.0 设置更新自己等级

                    //Step 3.1 为自己所有上线增加返利

                    //Step 3.2 为上线增加重消记录

                    //Step 3.5 为上线增加总成就

                    //Step 3.6 设置更新上线等级

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
        // GET: /BaoDan/Buy
        public ActionResult Sell()
        {
            return View();
        }

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
                                                          RequestAmount = row.CashTransaction.Amount - row.Fee,
                                                          FinalQuantity = row.Amount,
                                                          FinalAmount = row.CashTransaction.Amount,
                                                          FinalPrice = row.CashTransaction.Amount / row.Amount,
                                                      };
            return View(model);
        }



        #region private methods

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

        #endregion

    }
}