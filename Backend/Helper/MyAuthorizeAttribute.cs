using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Backend.Models;
using Microsoft.AspNet.Identity;

namespace Backend.Helper
{
    public class MyAuthorizeAttribute : AuthorizeAttribute
    {

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var userId = httpContext.User.Identity.GetUserId(); 
            var roleList = Roles.Split(',');
            foreach (var s in roleList)
            {
                if (Helper.IsUserInRole(userId, s, new vapEntities1()))
                {
                    return true;
                }
            }
            /*var aspNetUser =  as AspNetUser;
            if (aspNetUser != null)
            {
                var firstOrDefault = aspNetUser.AspNetRoles.FirstOrDefault();
                if (firstOrDefault != null)
                {
                    string currentRole = firstOrDefault.Name;
                    //从Session中获取User对象，然后得到其角色信息。如果用户重写了Identity, 则可以在httpContext.Current.User.Identity中获取
                    if (Roles.Contains(currentRole))
                        return true;
                }
            }*/
           
            return base.AuthorizeCore(httpContext);
        }

    }
}