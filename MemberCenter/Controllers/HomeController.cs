using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MemberCenter.Models;
using VAPModel;

namespace MemberCenter.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {

            MyAssetViewModel model = new MyAssetViewModel()
            {
                AvailableCash = CurrentUser.Cash1,
                LockedCash = CurrentUser.Cash2,
                AvailablePoints = CurrentUser.Point1,
                LockedPoints = CurrentUser.Point2,
                AvailableChongXiao = CurrentUser.ChongXiao1,
                LockedChongXiao = CurrentUser.ChongXiao2,
                AvailableCoin = CurrentUser.Coin1,
                LockedCoin = CurrentUser.Coin2,
            };

            return View(model);
        }





    }
}