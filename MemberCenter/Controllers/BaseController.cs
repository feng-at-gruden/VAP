using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

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

        protected override void Dispose(bool disposing)
        {
            if (disposing && db != null)
            {
                db.Dispose();
                db = null;
            }
            base.Dispose(disposing);
        }

	}
}