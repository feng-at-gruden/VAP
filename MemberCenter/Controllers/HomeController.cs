using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MemberCenter.Models;
using VapLib;

namespace MemberCenter.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            DateTime stTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            DateTime edTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
            String typeSell = 报单类型.卖出.ToString();
            String typeBuy = 报单类型.买入.ToString();
            String statusStr = 报单状态.用户已取消.ToString();

            var todayBodan = db.BaoDanTransactions.Where(m => m.DateTime >= stTime && m.DateTime <= edTime && !m.Status.Equals(statusStr));
            var totalBaoDan = db.BaoDanTransactions.Where(m => !m.Status.Equals(statusStr));
            
            var p = db.CoinPrices.Take(2).OrderByDescending(m=>m.DateTime);
            decimal currrentPrice = 0, yesterdayPrice = 0;
            if(p.Count()>=2)
            {
                currrentPrice = p.ToArray()[0].Price;
                yesterdayPrice = p.ToArray()[1].Price;
            }
            else if (p.Count()==1)
            {
                currrentPrice = p.ToArray()[0].Price;
                yesterdayPrice = p.ToArray()[0].Price;
            }

            DateTime chartEdTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            DateTime chartStTime = chartEdTime.AddMonths(-1);
            string newsType = 新闻类型.公告.ToString();
            HomeViewModel model = new HomeViewModel
            {
                CurrentCoinPrice = currrentPrice,
                YesterdayCoinPrice = yesterdayPrice,
                MaxCoinPrice = db.CoinPrices.Max(m=>m.Price),
                MinCoinPrice = db.CoinPrices.Min(m=>m.Price),
                MemberAmount = db.Members.Count(),
                TodayBaoDanAmount = todayBodan.Count() == 0 ? 0 : todayBodan.Sum(m => m.Amount),
                TodayBaoDanCash = (int)Math.Ceiling(todayBodan.Count() == 0 ? 0 : todayBodan.Sum(m => m.Amount * m.Price)),
                TotalTransactionCash = (int)Math.Ceiling(totalBaoDan.Count() == 0 ? 0 : totalBaoDan.Sum(m => m.Amount * m.Price)),
                RecentBaoDanBuy = (from row in db.BaoDanTransactions
                                   where row.Type.Equals(typeBuy) && !row.Status.Equals(statusStr)
                                    orderby row.DateTime descending
                                    select new BaoDanHistoryViewModel
                                    {
                                        RequestQuantity = row.Amount,
                                        RequestPrice = row.Price,
                                        RequestCash = row.Amount * row.Price,
                                    }).Take(6),
                RecentBaoDanSell = (from row in db.BaoDanTransactions
                                    where row.Type.Equals(typeSell) && !row.Status.Equals(statusStr)
                                    orderby row.DateTime descending
                                    select new BaoDanHistoryViewModel
                                    {
                                        RequestQuantity = row.Amount,
                                        RequestPrice = row.Price,
                                        RequestCash = row.Amount * row.Price,
                                    }).Take(6),
                CoinPriceHistory = (from row in db.CoinPrices
                                   //where row.DateTime >= chartStTime && row.DateTime <= chartEdTime
                                   select new CoinPriceHistoryViewModel
                                   {
                                        Price = row.Price,
                                        DateTime = row.DateTime
                                   }).Take(30),
                BuyHistory = (from row in db.SysStatistics
                              where row.Date >= chartStTime && row.Date <= chartEdTime
                                   select new DailyAmountViewModel
                                   {
                                       Amount = row.BaoDanBuyAmount.Value,
                                       DateTime = row.Date
                                   }).Take(30),
                SellHistory = (from row in db.SysStatistics
                               where row.Date >= chartStTime && row.Date <= chartEdTime
                             select new DailyAmountViewModel
                             {
                                 Amount = row.BaoDanSellAmount.Value,
                                 DateTime = row.Date
                             }).Take(30),

                News = (from row in db.News
                       where row.Type == newsType
                       orderby row.DateTime descending
                       select new NewsViewModel
                       {
                           Title = row.Title,
                           Content = row.Content,
                       }).Take(5),
            };

            if (User.Identity.IsAuthenticated)
            {
                SetMyAccountViewModel();
            }
            return View(model);
        }


        public ActionResult DashBoard()
        {
            return RedirectToAction("MyAssets", "Account");
        }


    }
}