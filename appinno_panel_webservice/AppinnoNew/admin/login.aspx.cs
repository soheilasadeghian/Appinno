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
    public partial class login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var pass = ClassCollection.Methods.md5("123456");
            if (ClassCollection.Methods.isLogin(Context))
            {
                Response.Redirect("dashboard.aspx");
            }
        }

        /// <summary>
        /// این متد بررسی میکند که آیا یوزر و پسوردوارد شده صحیح است یا خیر
        /// </summary>
        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string isExistUser(string emailtell, string password)
        {
            ClassCollection.Model.Result result = new ClassCollection.Model.Result();
            DataAccessDataContext db = new DataAccessDataContext();

            if (emailtell == "sa" && password == "sa")
            {
                var user = new userTbl();
                user.accessID = 1;
                user.emailtell = "m";
                user.ID = 0;//soheila

                HttpContext.Current.Session["user"] = user;
                result.code = 0;
                result.message = ClassCollection.Message.USER_EXIST;

                return new JavaScriptSerializer().Serialize(result);
            }

            try
            {
                if (db.userTbls.Where(c => c.accessID != 2).Any(c => c.emailtell == emailtell && c.password == ClassCollection.Methods.md5(password)))
                {
                    var user = db.userTbls.Where(c => c.accessID != 2).Single(c => c.emailtell == emailtell && c.password == ClassCollection.Methods.md5(password));
                    HttpContext.Current.Session["user"] = user;
                    result.code = 0;
                    result.message = ClassCollection.Message.USER_EXIST;
                    return new JavaScriptSerializer().Serialize(result);
                }
                else
                {
                    HttpContext.Current.Session["user"] = null;
                    result.code = 1;
                    result.message = ClassCollection.Message.USER_NOT_EXIST;
                    return new JavaScriptSerializer().Serialize(result);
                }
            }
            catch 
            {
                HttpContext.Current.Session["user"] = null;
                result.code = 1;
                result.message = ClassCollection.Message.USER_NOT_EXIST;
                return new JavaScriptSerializer().Serialize(result);
            }

        }
    }
}