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
    public partial class eican : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!ClassCollection.Methods.isLogin(Context))
            {
                Response.Redirect("login.aspx");
            }
        }
        

        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string getIcan(int icanID)
        {
            ClassCollection.Model.IcanResult result = new ClassCollection.Model.IcanResult();
            result.result = new ClassCollection.Model.Result();

            if (!ClassCollection.Methods.isLogin(HttpContext.Current))
            {
                result.result.code = 1;
                result.result.message = ClassCollection.Message.LOGIN_FIRST;
                return new JavaScriptSerializer().Serialize(result);
            }
            var db = new DataAccessDataContext();
            var user = ClassCollection.Methods.getUserInSession(HttpContext.Current);
            var permission = ClassCollection.Accessing.Deserialize(db.accessTbls.Single(c => c.ID == user.accessID).per);

            if (permission.ican.access.showlist == false)
            {
                result.result.code = 2;
                result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }

            if (db.icanTbls.Any(c => c.ID == icanID) == false)
            {
                result.result.code = 2;
                result.result.message = ClassCollection.Message.ICAN_NOT_EXIST;
                result.ican = null;

                return new JavaScriptSerializer().Serialize(result);
            }

            var ican = db.icanTbls.Single(c => c.ID == icanID);

            var contents = db.icanContentTbls.Where(c => c.icanID == ican.ID).OrderBy(c => c.pri);/*contentedit*/


            result.ican = new ClassCollection.Model.fullIcan();
            result.ican.ID = ican.ID;
            result.ican.title = ican.title;
            result.ican.isBlock = ican.isBlock;
            result.ican.creatorID = ican.creatorID;
            result.ican.viewCount = ican.viewCount;
            result.ican.haveattachment = ican.haveAttachment;

            result.ican.contents = new List<ClassCollection.Model.icanContent>();

            foreach (var item in contents)
            {
                var tmp = new ClassCollection.Model.icanContent();

                tmp.ID = item.ID;
                tmp.type = item.contentType;

                if (item.contentType == 0)
                {
                    tmp.value = item.value;
                }
                else if (item.contentType == 1)
                {
                    tmp.value = ClassCollection.Methods.geticanImagesPath().Replace("~", "") + item.value;
                }
                else if (item.contentType == 2)
                {
                    tmp.value = ClassCollection.Methods.geticanVideosPath().Replace("~", "") + item.value;
                }
                else if (item.contentType == 3)
                {
                    tmp.fileSize = item.fileSize;
                    tmp.fileType = item.fileType;
                    tmp.downloadCount = item.downloadCount;
                    tmp.value = ClassCollection.Methods.geticanFilePath().Replace("~", "") + item.value;
                }

                result.ican.contents.Add(tmp);
            }

            result.result.code = 0;
            result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
        }
    }
}

