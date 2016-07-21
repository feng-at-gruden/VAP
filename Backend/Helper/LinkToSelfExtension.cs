using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using Microsoft.Ajax.Utilities;

namespace Backend.Helper
{
    public static class LinkToSelfExtension
    {
        public static IHtmlString LinkToSelf(this HtmlHelper htmlHelper, string linkText,
            object routeValues = null, object htmlAttrs = null)
        {
            var htmlAttrsDict = new Dictionary<string, object>();
            if (htmlAttrs != null)
            {
                foreach (var fld in htmlAttrs.GetType().GetFields())
                {
                    htmlAttrsDict.Add(fld.Name, fld.GetValue(htmlAttrs));
                }
            }
            return htmlHelper.RouteLink(linkText, htmlHelper.ViewData.GetRouteValuesEx(routeValues), htmlAttrsDict);
        }

        public static IHtmlString LinkToSelf(this HtmlHelper htmlHelper, string linkText,
            RouteValueDictionary routeValues, IDictionary<string, object> htmlAttrs)
        {
            return htmlHelper.RouteLink(linkText, htmlHelper.ViewData.GetRouteValuesEx(routeValues), htmlAttrs);
        }

        public static string LinkToSelf(this UrlHelper urlHelper, ViewDataDictionary viewData,
            object routeValues = null)
        {
            return urlHelper.RouteUrl(viewData.GetRouteValuesEx(routeValues));
        }
        public static string LinkToSelf(this UrlHelper urlHelper, ViewDataDictionary viewData,
            RouteValueDictionary routeValues)
        {
            return urlHelper.RouteUrl(viewData.GetRouteValuesEx(routeValues));
        }

        public static RouteValueDictionary GetRouteValuesEx(this ViewDataDictionary viewData, object routeValues = null)
        {
            return viewData.GetRouteValuesEx(new RouteValueDictionary(routeValues));
        }

        public static RouteValueDictionary GetRouteValuesEx(this ViewDataDictionary viewData, RouteValueDictionary routeValues)
        {
            var modelValues = (from state in viewData.ModelState
                               where !string.IsNullOrEmpty(state.Key) && state.Value.Value != null
                               select new KeyValuePair<string, object>(state.Key, state.Value.Value.AttemptedValue));

            return new RouteValueDictionary(
                routeValues
                    .Union(modelValues, new KeyEqualityComparer())
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
                );
        }
    }

    public class KeyEqualityComparer : IEqualityComparer<KeyValuePair<string, object>>
    {
        public bool Equals(KeyValuePair<string, object> kvp1, KeyValuePair<string, object> kvp2)
        {
            return kvp1.Key.ToLower() == kvp2.Key.ToLower();
        }

        public int GetHashCode(KeyValuePair<string, object> kvp)
        {
            return kvp.Key.ToLower().GetHashCode();
        }
    }
}