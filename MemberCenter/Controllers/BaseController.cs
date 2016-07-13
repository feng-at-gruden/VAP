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