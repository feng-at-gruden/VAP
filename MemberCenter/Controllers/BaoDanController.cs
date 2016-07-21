using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MemberCenter.Controllers
{
    public class BaoDanController : Controller
    {
        //
        // GET: /BaoDan/
        public ActionResult Buy()
        {
            return View();
        }


        public ActionResult Sell()
        {
            return View();
        }

        public ActionResult LockedCoins()
        {
            return View();
        }

        public ActionResult MyRecords()
        {
            return View();
        }

	}
}