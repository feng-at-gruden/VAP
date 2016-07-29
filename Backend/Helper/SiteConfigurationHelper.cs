using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace Backend.Helper
{
    public static class SiteConfigurationHelper
    {
        public static string SiteRootPath
        {
            get { return ConfigurationManager.AppSettings["WebRootFolder"]; }
        }
    }
}