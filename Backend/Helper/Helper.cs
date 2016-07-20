using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Backend.Models;

namespace Backend.Helper
{
    public class Helper
    {
        public static string GetUserRole(string userId, VapEntities dbEntities)
        {
            var user = dbEntities.AspNetUsers.Find(userId);
            if (user != null)
            {
                var firstOrDefault = user.AspNetRoles.FirstOrDefault();
                if (firstOrDefault != null)
                    return firstOrDefault.Name;
            }
            return "";
        }
        public static bool IsUserInRole(string userId,string role, VapEntities dbEntities)
        {

            return GetUserRole(userId,dbEntities) == role;
            
        }
    }
}