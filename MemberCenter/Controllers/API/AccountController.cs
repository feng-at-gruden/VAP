using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using MemberCenter.Models;
using VapLib;

namespace MemberCenter.Controllers.API
{
    [APIAuthorize]
    public class AccountController : ApiController
    {

        protected Model1Container db = new Model1Container();

        //
        // GET: /API/
        [HttpGet]
        public StatusModel Status()
        {
            return new StatusModel
            {
                Version = "1.0",
            };
        }


        [HttpPost]
        public APIResponseModel ConsumePoints(ConsumeRequestModel request)
        {
            Member member = GetCurrentUser();
            //TODO, validation.
            // 
            member.PointTransaction.Add(new PointTransaction
            {
                DateTime = DateTime.Now,
                Amount = request.Amount,
                Type = 积分记录类型.商场积分消费.ToString(),
                Status = 积分状态.可用.ToString(),
            });
            member.Point1 -= request.Amount;

            db.SaveChanges();
            return new APIResponseModel { Message = "Success"};
        }



        protected Member GetCurrentUser()
        {
            Member user = (Member)Request.GetUser();
            return db.Members.SingleOrDefault(m => m.Email.Equals(user.Email, StringComparison.InvariantCultureIgnoreCase) && m.Password1.Equals(user.Password1));;
        }

	}
}