using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
                    return db.Members.SingleOrDefault(m => m.Email.Equals(HttpContext.User.Identity.Name, StringComparison.InvariantCultureIgnoreCase));
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