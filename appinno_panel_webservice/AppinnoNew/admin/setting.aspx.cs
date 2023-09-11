using AppinnoNew.ClassCollection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AppinnoNew.admin
{
    public partial class setting : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var db = new DataAccessDataContext();
            if (ClassCollection.Methods.isLogin(Context))
            {
                var user = ClassCollection.Methods.getUserInSession(HttpContext.Current);
                var permission = ClassCollection.Accessing.Deserialize(db.accessTbls.Single(c => c.ID == user.accessID).per);

                var relation =
               //مدیر کل سامانه
               (user.accessID == 1) ||
               //مدیر کل سامانه(همراه اول
               (user.accessID == 52);

                if (!relation)
                {
                    Response.Redirect("dashboard.aspx");
                }
            }
            else
            {
                Response.Redirect("login.aspx");
            }
        }
        
        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string getDailyText()
        {
            ClassCollection.Model.Result result = new ClassCollection.Model.Result();

            if (!ClassCollection.Methods.isLogin(HttpContext.Current))
            {
                result.code = 1;
                result.message = ClassCollection.Message.LOGIN_FIRST;
                return new JavaScriptSerializer().Serialize(result);
            }

            var db = new DataAccessDataContext();
            var user = ClassCollection.Methods.getUserInSession(HttpContext.Current);
            var permission = ClassCollection.Accessing.Deserialize(db.accessTbls.Single(c => c.ID == user.accessID).per);

            var relation =
           //مدیر کل سامانه
           (user.accessID == 1) ||
           //مدیر کل سامانه(همراه اول
           (user.accessID == 52);

            if (!relation)
            {
                result.code = 2;
                result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }
            result.code = 0;
            result.message = db.settingTbls.Single(c => c.title == "adminmessage").value;
            return new JavaScriptSerializer().Serialize(result);
        }

        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string gePolicyText()
        {
            ClassCollection.Model.Result result = new ClassCollection.Model.Result();

            if (!ClassCollection.Methods.isLogin(HttpContext.Current))
            {
                result.code = 1;
                result.message = ClassCollection.Message.LOGIN_FIRST;
                return new JavaScriptSerializer().Serialize(result);
            }

            var db = new DataAccessDataContext();
            var user = ClassCollection.Methods.getUserInSession(HttpContext.Current);
            var permission = ClassCollection.Accessing.Deserialize(db.accessTbls.Single(c => c.ID == user.accessID).per);

            var relation =
           //مدیر کل سامانه
           (user.accessID == 1) ||
           //مدیر کل سامانه(همراه اول
           (user.accessID == 52);

            if (!relation)
            {
                result.code = 2;
                result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }
            result.code = 0;
            result.message = db.settingTbls.Single(c => c.title == "policy").value.Replace("<p>", "").Replace("</p>", "").Replace("<br />", "");
            return new JavaScriptSerializer().Serialize(result);
        }
        


        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string getAdminImage()
        {
            ClassCollection.Model.Result result = new ClassCollection.Model.Result();

            if (!ClassCollection.Methods.isLogin(HttpContext.Current))
            {
                result.code = 1;
                result.message = ClassCollection.Message.LOGIN_FIRST;
                return new JavaScriptSerializer().Serialize(result);
            }

            var db = new DataAccessDataContext();
            var user = ClassCollection.Methods.getUserInSession(HttpContext.Current);
            var permission = ClassCollection.Accessing.Deserialize(db.accessTbls.Single(c => c.ID == user.accessID).per);

            var relation =
           //مدیر کل سامانه
           (user.accessID == 1) ||
           //مدیر کل سامانه(همراه اول
           (user.accessID == 52);

            if (!relation)
            {
                result.code = 2;
                result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }

            var filename = db.settingTbls.Single(c => c.title == "adminimage").value;
            if (filename == null)
            {
                result.code = 3;
                result.message = ClassCollection.Message.ERROR_EXIST_FILE;
                return new JavaScriptSerializer().Serialize(result);
            }

            if (!System.IO.File.Exists(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getAdminImagePath() + filename)))
            {
                result.code = 4;
                result.message = ClassCollection.Message.ERROR_EXIST_FILE;
                return new JavaScriptSerializer().Serialize(result);
            }

            result.code = 0;
            result.message = ClassCollection.Methods.getAdminImagePath() + filename;
            return new JavaScriptSerializer().Serialize(result);
        }

    }
}

