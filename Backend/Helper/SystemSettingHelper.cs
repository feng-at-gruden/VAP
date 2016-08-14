using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Backend.Models;


namespace Backend.Helper
{
    public static class SystemSettingHelper
    {
        public static String GetSystemSettingString(vapEntities1 db, string key)
        {
            var s = db.SystemSettings.SingleOrDefault(m => m.Key.Equals(key));
            if (s == null)
                return null;
            else
                return s.Value;
        }

        public static decimal GetSystemSettingDecimal(vapEntities1 db, string key)
        {
            string value = GetSystemSettingString(db, key);
            return decimal.Parse(value);
        }

        public static bool GetSystemSettingBoolean(vapEntities1 db, string key)
        {
            string value = GetSystemSettingString(db, key);
            return bool.Parse(value);
        }

    }
}