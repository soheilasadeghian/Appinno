using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;

namespace AppinnoNew.admin
{
    /// <summary>
    /// Summary description for partnersUploader
    /// </summary>
    public class settingUploader : IHttpHandler, IRequiresSessionState
    {
        public class item
        {
            public string type { get; set; }
            public HttpPostedFile value { get; set; }
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Request.ContentEncoding = Encoding.UTF8;
            ClassCollection.Model.Result result = new ClassCollection.Model.Result();
            if (!ClassCollection.Methods.isLogin(context))
            {
                result.code = 1;
                result.message = ClassCollection.Message.LOGIN_FIRST;
                context.Response.Write(new JavaScriptSerializer().Serialize(result));
                return;
            }
            var db = new DataAccessDataContext();
            var user = ClassCollection.Methods.getUserInSession(HttpContext.Current);
            var permission = ClassCollection.Accessing.Deserialize(db.accessTbls.Single(c => c.ID == user.accessID).per);

            
            var relation =
           //مدیر کل سامانه
           (user.accessID == 1) ||
           //مدیر کل سامانه(همراه اول
           (user.accessID == 52);
            
            #region edit
            
            if (!relation)
            {
                result.code = 2;
                result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                context.Response.Write(new JavaScriptSerializer().Serialize(result));
                return;
            }

            var dailytitle = context.Request.Form["daily-title"];
            var daily = db.settingTbls.Single(c => c.title == "adminmessage");
            daily.value = dailytitle;

            var policy = context.Request.Form["policy"];
            var po = db.settingTbls.Single(c => c.title == "policy");
            po.value = policy;

            
            HttpPostedFile file = context.Request.Files["img"];
            if (!(file == null))
            {
                var ext = "." + file.ContentType.Split('/')[1];
                var imagename = "appinno" + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N") + ext;
                string fname = context.Server.MapPath(ClassCollection.Methods.getAdminImagePath() + imagename);
                try
                {
                    file.SaveAs(fname);
                }
                catch (Exception e)
                {
                    context.Response.ContentType = "text/plain";
                    context.Response.Write(e.Message + "save problem");
                    return;
                }

                var adminimage = db.settingTbls.Single(c => c.title == "adminimage");
                adminimage.value = imagename;
            }
            db.SubmitChanges();
              
            
            result.code = 0;
            result.message = ClassCollection.Message.OPERATION_SUCCESS;
            context.Response.Write(new JavaScriptSerializer().Serialize(result));
            return;
            #endregion
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}