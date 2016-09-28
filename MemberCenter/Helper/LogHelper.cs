using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace MemberCenter.Helper
{
    public class LogHelper
    {

        public static void WriteErrorLog(String content, String errorType)
        {
            String fileName = System.Web.HttpContext.Current.Server.MapPath("~/log/") + errorType + "-" + DateTime.Now.ToString("yyyyMMdd") + ".log";
            try
            {
                StreamWriter sw = new StreamWriter(fileName, true);
                sw.WriteLine(content);
                sw.Close();
            }
            catch(Exception e)
            {
                
            }
        }

    }


}