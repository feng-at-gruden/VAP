using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MemberCenter.Controllers
{
    public class MessageController : BaseController
    {
        //
        // GET: /Message/
        public ActionResult Success()
        {
            SetMyAccountViewModel();
            return View();
        }
	}
}