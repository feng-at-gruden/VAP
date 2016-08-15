using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Web;
using System.Web.Http;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using System.Web.Http.Controllers;
using MemberCenter.Models;

namespace MemberCenter
{
    public class APIAuthorize : AuthorizationFilterAttribute, IDisposable
    {
        protected Model1Container DbContext;

        public APIAuthorize()
        {
            DbContext = new Model1Container();
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var user = GetUserViaBasicAuth(HttpContext.Current);
            actionContext.Request.SetUser(user);
            base.OnAuthorization(actionContext);
        }

        public void Dispose()
        {
            if (DbContext != null)
            {
                DbContext.Dispose();
                DbContext = null;
            }
        }


        private Member GetUserViaBasicAuth(HttpContext httpContextCurrent)
        {
            string username;
            string password;
            GetUserFromCurrentHttpContext(httpContextCurrent, out username, out password);

            Member member = DbContext.Members.SingleOrDefault(m => m.Email.Equals(username, StringComparison.InvariantCultureIgnoreCase) && m.Password1.Equals(password));
            if (member == null)
                ThrowApiError("身份验证错误！", HttpStatusCode.Unauthorized);

            return member;
        }

        private void GetUserFromCurrentHttpContext(HttpContext currentHttpContext, out string username, out string password)
        {
            var authorizationHeaderString = currentHttpContext.Request.Headers["Authorization"];
            if (!ParseAuthHeader(authorizationHeaderString, out username, out password))
            {
                currentHttpContext.Response.AddHeader("WWW-Authenticate", "Basic");
                ThrowApiError("请输入正确用户名和密码.", HttpStatusCode.Unauthorized);
            }
        }

        private bool ParseAuthHeader(string authHeader, out string username, out string password)
        {
            username = null;
            password = null;

            // Check this is a Basic Auth header 
            if (String.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Basic"))
                return false;

            // Pull out the Credentials with are seperated by ':' and Base64 encoded 
            var base64Credentials = Convert.FromBase64String(authHeader.Substring(6));
            var credentials = Encoding.ASCII.GetString(base64Credentials).Split(new[] { ':' });

            if (credentials.Length != 2 || String.IsNullOrEmpty(credentials[0]) || String.IsNullOrEmpty(credentials[1]))
                return false;

            username = credentials[0];
            password = credentials[1];
            return true;
        }

        private dynamic ThrowApiError(string message, HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest)
        {
            var resp = new HttpResponseMessage(httpStatusCode) { Content = new StringContent(message) };
            throw new HttpResponseException(resp);
        }

    }


}