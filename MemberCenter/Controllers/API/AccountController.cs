using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using System.Threading.Tasks;
using MemberCenter.Models;
using VapLib;

namespace MemberCenter.Controllers.API
{
    
    public class AccountController : ApiController
    {

        protected Model1Container db = new Model1Container();

        //
        // GET: /API/Status
        [HttpGet]
        public StatusModel Status()
        {
            return new StatusModel
            {
                Version = "1.0",
            };
        }

        //
        // POST: /API/ConsumePoints
        [APIAuthorize]
        [HttpPost]
        public APIResponseModel ConsumePoints(ConsumeRequestModel request)
        {
            Member member = GetCurrentUser();
            //Validation
            if(member.Point1 < request.Amount)
                ThrowApiError("购物券余额不足！", HttpStatusCode.BadRequest);

            member.PointTransaction.Add(new PointTransaction
            {
                DateTime = DateTime.Now,
                Amount = -request.Amount,
                Comment = request.Comment,
                Type = 积分记录类型.商城消费.ToString(),
                Status = 积分状态.可用.ToString(),
            });
            member.Point1 -= request.Amount;

            db.SaveChanges();
            return new APIResponseModel { Message = "Success"};
        }

        //
        // POST: /API/ConsumeChongXiao
        [APIAuthorize]
        [HttpPost]
        public APIResponseModel ConsumeChongXiao(ConsumeRequestModel request)
        {
            Member member = GetCurrentUser();
            //Validation
            if (member.ChongXiao1 < request.Amount)
                ThrowApiError("重消余额不足！", HttpStatusCode.BadRequest);

            member.ChongXiaoTransaction.Add(new ChongXiaoTransaction
            {
                DateTime = DateTime.Now,
                Amount = -request.Amount,
                Comment = request.Comment,
                Type = 重消记录类型.商城消费.ToString(),
                Status = 重消状态.可用.ToString(),
            });
            member.ChongXiao1 -= request.Amount;

            db.SaveChanges();
            return new APIResponseModel { Message = "Success" };
        }

        //
        // GET: /API/UnlockCash
        [APIAuthorize]
        [HttpGet]
        public APIResponseModel UnlockCash()
        {
            var date = DateTime.Today.Date;
            while (date.DayOfWeek != DayOfWeek.Monday)
            {
                date = date.AddDays(-1);
            }
            
            var status = VapLib.现金状态.冻结.ToString();
            var lockTrans = db.CashTransactions.Where(c => c.DateTime < date && c.Status == status).ToList();
            foreach (var cashTransaction in lockTrans)
            {
                var member = db.Members.Find(cashTransaction.MemberId);
                member.Cash1 += cashTransaction.Amount;
                member.Cash2 -= cashTransaction.Amount;
                cashTransaction.Status = VapLib.现金状态.解冻.ToString();

                db.Entry(member).State = EntityState.Modified;
                db.Entry(cashTransaction).State = EntityState.Modified;
                db.SaveChanges();

            }
            db.SaveChanges();
            return new APIResponseModel { Message = "Success" };
        }



        protected Member GetCurrentUser()
        {
            Member user = (Member)Request.GetUser();
            return db.Members.SingleOrDefault(m => m.Email.Equals(user.Email, StringComparison.InvariantCultureIgnoreCase) && m.Password1.Equals(user.Password1));;
        }

        private dynamic ThrowApiError(string message, HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest)
        {
            var resp = new HttpResponseMessage(httpStatusCode) { Content = new StringContent(message) };
            throw new HttpResponseException(resp);
        }

	}
}