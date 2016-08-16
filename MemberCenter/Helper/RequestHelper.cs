using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using VapLib;

namespace MemberCenter.Helper
{
    public class RequestHelper
    {

        private HttpRequestBase request;

        public RequestHelper(HttpRequestBase request)
        {
            this.request = request;
        }

        /// <summary>
        /// Save image to S3 Server
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="folder"></param>
        /// <returns></returns>
        public string SaveImageToServer(string targetFolder, bool isRequire = true, string errorMessage = "请上传汇款凭证截图文件")
        {
            if (isRequire && (request.Files == null || request.Files.Count == 0 || string.IsNullOrWhiteSpace(request.Files[0].FileName)))
            {
                throw new Exception(errorMessage);
            }
            if (request.Files.Count > 0 && request.Files[0].FileName!="" && request.Files[0].FileName!=null)
            { 
                var fileName = (new FileInfo(request.Files[0].FileName)).Name;
                if (!IsImageFile(fileName))
                {
                    throw new Exception("请选择正确文件格式!");
                }

                string fileSavedName = DateTime.Now.Ticks + "" + fileName.Substring(fileName.LastIndexOf("."));
                var path = Path.Combine(HttpContext.Current.Server.MapPath("~/" + targetFolder), fileSavedName);
                request.Files[0].SaveAs(path);
                return "/" + targetFolder + "/" + fileSavedName;
            }
            else
            {
                return "";
            }
        }

        public static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        private bool IsImageFile(string fileName)
        {
            var ext = Path.GetExtension(fileName);
            var imageExtensions = new string[] { "jpg", "bmp", "gif", "png", "jpeg" };
            if (string.IsNullOrWhiteSpace(ext))
            {
                return false;
            }
            ext = ext.ToLower().Trim('.');
            foreach (var e in imageExtensions)
            {
                if (ext == e)
                    return true;
            }
            return false;
        }
        
    }

}