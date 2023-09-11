using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace AppinnoNew
{
    /// <summary>
    /// Summary description for FileDownloader1
    /// </summary>
    public class FileDownloader : IHttpHandler
    {
        //sample url:http://appinno.tana.ir/filedownloader.ashx?file=appinno_20161123162712626_5eddcdd19a0542e29522d8176d933ce6.mov&section=ican
        public void ProcessRequest(HttpContext context)
        {
            //Retrieve file name from URL and append my downloads path
            string section = context.Request.QueryString["section"];
            string fileName = context.Request.QueryString["file"];
            if (string.IsNullOrEmpty(fileName) == true || string.IsNullOrEmpty(section) == true)
            {
                context.Response.StatusCode = 500;
                context.Response.End();
                return;
            }

            #region dowload
            if (section == "download")
            {

                string filePath = context.Server.MapPath(ClassCollection.Methods.getdownloadFilePath() + fileName);
                //Be sure the file exists
                var db = new DataAccessDataContext();

                FileInfo file = new System.IO.FileInfo(filePath);
                if (!file.Exists)
                {
                    context.Response.StatusCode = 500;
                    context.Response.End();
                    return;
                }

                if (db.downloadContentTbls.Any(c => c.contentType == 2 && c.value.Contains(fileName)) == false)
                {
                    context.Response.StatusCode = 500;
                    context.Response.End();
                    return;
                }

                var F = db.downloadContentTbls.Single(c => c.contentType == 2 && c.value.Contains(fileName));
                F.downloadCount = F.downloadCount == null ? 0 : F.downloadCount + 1;
                db.SubmitChanges();

                //Don't allow response to be cached
                context.Response.Clear();
                context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                context.Response.Cache.SetNoStore();
                context.Response.Cache.SetExpires(DateTime.MinValue);
                //The next few lines is what actually starts downloads the file.
                context.Response.AddHeader("Content-Disposition", "attachment; filename=" + file.Name);
                context.Response.AddHeader("Content-Length", file.Length.ToString());
                context.Response.ContentType = "application/octet-stream";
                context.Response.WriteFile(file.FullName);
                context.ApplicationInstance.CompleteRequest();
                context.Response.End();
            }
            #endregion

            #region ican
            if (section == "ican")
            {
                string filePath = context.Server.MapPath(ClassCollection.Methods.geticanFilePath() + fileName);
                //Be sure the file exists
                var db = new DataAccessDataContext();

                FileInfo file = new System.IO.FileInfo(filePath);
                if (!file.Exists)
                {
                    context.Response.StatusCode = 500;
                    context.Response.End();
                    return;
                }

                if (db.icanContentTbls.Any(c => c.contentType == 3 && c.value.Contains(fileName)) == false)
                {
                    context.Response.StatusCode = 500;
                    context.Response.End();
                    return;
                }

                var F = db.icanContentTbls.Single(c => c.contentType == 3 && c.value.Contains(fileName));
                F.downloadCount = F.downloadCount == null ? 0 : F.downloadCount + 1;
                db.SubmitChanges();

                //Don't allow response to be cached
                context.Response.Clear();
                context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                context.Response.Cache.SetNoStore();
                context.Response.Cache.SetExpires(DateTime.MinValue);
                //The next few lines is what actually starts downloads the file.
                context.Response.AddHeader("Content-Disposition", "attachment; filename=" + file.Name);
                context.Response.AddHeader("Content-Length", file.Length.ToString());
                context.Response.ContentType = "application/octet-stream";
                context.Response.WriteFile(file.FullName);
                context.ApplicationInstance.CompleteRequest();
                context.Response.End();
            }
            #endregion

            #region message
            if (section == "message")
            {

                string filePath = context.Server.MapPath(ClassCollection.Methods.getmessagePath() + fileName);
                //Be sure the file exists
                var db = new DataAccessDataContext();

                FileInfo file = new System.IO.FileInfo(filePath);
                if (!file.Exists)
                {
                    context.Response.StatusCode = 500;
                    context.Response.End();
                    return;
                }

                if (db.messageTbls.Any(c => c.attachment.Contains(fileName)) == false)
                {
                    context.Response.StatusCode = 500;
                    context.Response.End();
                    return;
                }

                var F = db.messageTbls.Single(c => c.attachment.Contains(fileName));
                F.downloadCount = F.downloadCount == null ? 0 : F.downloadCount + 1;
                db.SubmitChanges();

                //Don't allow response to be cached
                context.Response.Clear();
                context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                context.Response.Cache.SetNoStore();
                context.Response.Cache.SetExpires(DateTime.MinValue);
                //The next few lines is what actually starts downloads the file.
                context.Response.AddHeader("Content-Disposition", "attachment; filename=" + file.Name);
                context.Response.AddHeader("Content-Length", file.Length.ToString());
                context.Response.ContentType = "application/octet-stream";
                context.Response.WriteFile(file.FullName);
                context.ApplicationInstance.CompleteRequest();
                context.Response.End();
            }
            #endregion
        }
        public bool ValidateParameters(HttpContext context)
        {
            return true;
        }
        public bool IsReusable
        {
            get
            {
                return true;
            }
        }
    }
}