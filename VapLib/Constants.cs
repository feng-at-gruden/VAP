using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VapLib
{
    public static class Constants
    {
        public const string AdminAccount = "admin";
        public const string AdminPass = "Admin123!";
        public const int PageSize = 10;
        /// <summary>
        /// 用户状态
        /// </summary>
        public enum UserStatusEnum
        {
            InActive,
            Active
        }
    }
}
