﻿using System;
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
        /// 
        /// </summary>

        public enum 会员状态
        {
            待审核 = 0,
            正常 = 1,
            锁定 = 2,
        }

        public enum 现金状态
        {
            待审核 = 0,
            可用 = 1,
            冻结 = 2,
        }

        public enum 现金交易类型
        {
            充值 = 1,
            货币提现 = 2,
            下线返利 = 3,
        }


        public enum 货币状态
        {
            冻结 = 0,
            可用 = 1,
        }

        //To confirm
        public enum 积分记录类型
        {
            购币所得积分 = 1,
            商场积分消费 = 2,
        }

        public enum 积分状态
        {
            冻结 = 0,
            可用 = 1,
        }

        public enum 重消状态
        {
            冻结 = 0,
            可用 = 1,
        }


        public enum 用户等级
        {
            一星 = 1,
            二星 = 2,
            三星 = 3,
            四星 = 4,
            五星 = 5,
            六星 = 6,
            七星 = 7,
            一钻 = 8,
            二钻 = 9,
            三钻 = 10,
            四钻 = 11,
            五钻 = 12,
            六钻 = 13,
            七钻 = 14,
        }


    }
}
