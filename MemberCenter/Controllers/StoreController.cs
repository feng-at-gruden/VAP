using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MemberCenter.Models;

namespace MemberCenter.Controllers
{
    public class StoreController : BaseController
    {
        //
        // GET: /Store/PointHistory
        public ActionResult PointHistory()
        {
            IEnumerable<ConsumptionViewModel> model = from row in CurrentUser.PointTransaction
                                                    orderby row.DateTime descending
                                                    select new ConsumptionViewModel
                                                    {
                                                        DateTime = row.DateTime,
                                                        Amount = row.Amount,
                                                        Comment = row.Comment,
                                                        Type = row.Type,
                                                    };
            SetMyAccountViewModel();
            return View(model);
        }

        //
        // GET: /Store/ChongXiaoHistory
        public ActionResult ChongXiaoHistory()
        {
            IEnumerable<ConsumptionViewModel> model = from row in CurrentUser.ChongXiaoTransaction
                                                    orderby row.DateTime descending
                                                    select new ConsumptionViewModel
                                                    {
                                                        DateTime = row.DateTime,
                                                        Amount = row.Amount,
                                                        Comment = row.Comment,
                                                        Type = row.Type,
                                                    };
            SetMyAccountViewModel();
            return View(model);
        }


	}
}