using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;


namespace MemberCenter
{
    public static class Extensions
    {
        const string UserKey = "CurrentUser";

        /// <summary>
        /// HttpRequestMessage Extension
        /// </summary>
        public static void SetUser(this HttpRequestMessage request, Object customer)
        {
            request.Properties.Add(UserKey, customer);
        }

        /// <summary>
        /// HttpRequestMessage Extension
        /// </summary>
        public static Object GetUser(this HttpRequestMessage request)
        {
            var customer = request.Properties[UserKey] as Object;
            if (customer == null)
            {
                throw new UnauthorizedAccessException();
            }
            return customer;
        }
    }
}