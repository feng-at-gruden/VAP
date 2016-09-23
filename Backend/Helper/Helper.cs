using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Backend.Models;

namespace Backend.Helper
{
    public class Helper
    {
        public static string GetUserRole(string userId, vapEntities1 dbEntities)
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
        public static bool IsUserInRole(string userId, string role, vapEntities1 dbEntities)
        {

            return GetUserRole(userId,dbEntities) == role;
            
        }
        public static bool IsUserInRole(string userRole, string roles)
        {

            return roles.Contains(userRole);

        }
    }
}