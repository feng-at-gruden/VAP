using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MemberCenter.Models;

namespace MemberCenter.Controllers
{
    public class BaseController : Controller
    {

        protected Model1Container db = new Model1Container();

        protected Member CurrentUser {
            get
            {
                if(HttpContext.User.Identity.Name!=null)
                {
                    Member me = db.Members.SingleOrDefault(m => m.Email.Equals(HttpContext.User.Identity.Name, StringComparison.InvariantCultureIgnoreCase));
                    if (me == null)
                    {
                        FormsAuthentication.SignOut();
                        Response.Redirect("~/Account/Login");
                    }
                    return me;
                }
                return null;
            }
        }

        protected CoinPrice CurrentCoinPrice
        {
            get
            {
                var price = db.CoinPrices.OrderByDescending(m => m.DateTime).Take(1);
                if (price == null || price.Count() == 0)
                {
                    throw new HttpException(500, "当前没有足够虚拟币");
                }
                return price.ToArray()[0];
            }
        }


        protected String GetSystemSettingString(string key)
        {
            var s = db.SystemSettings.SingleOrDefault(m => m.Key.Equals(key));
            if (s == null)
                return null;
            else
                return s.Value;
        }

        protected decimal GetSystemSettingDecimal(string key)
        {
            string value = GetSystemSettingString(key);
            return decimal.Parse(value);
        }

        protected bool GetSystemSettingBoolean(string key)
        {
            string value = GetSystemSettingString(key);
            return bool.Parse(value);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing && db != null)
            {
                db.Dispose();
                db = null;
            }
            base.Dispose(disposing);
        }

        protected void SetMyAccountViewModel()
        {
            Member user = CurrentUser;
            TempData["MyAccount"] = GetMyAccountViewModel(user);
        }

        protected MyAccountViewModel GetMyAccountViewModel(Member user)
        {
            return new MyAccountViewModel
            {
                RealName = user.RealName,
                Level = user.MemberLevel.Level + "会员",
                Achievement = user.Achievement,
                AvailableCash = user.Cash1,
                LockedCash = user.Cash2,
                AvailablePoints = user.Point1,
                LockedPoints = user.Point2,
                AvailableChongXiao = user.ChongXiao1,
                LockedChongXiao = user.ChongXiao2,
                AvailableCoin = user.Coin1,
                LockedCoin = user.Coin2,
            };
        }


	}
}