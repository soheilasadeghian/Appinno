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
    public class ideaUploader : IHttpHandler, IRequiresSessionState
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

            #region edit

            if (context.Request.QueryString["mode"] == "e")
            {
                try
                {
                    long s = long.Parse(context.Request.Form["ideaID"].ToString());
                }
                catch
                {

                }
                if (permission.bestIdeasCompetition.access.edit == false)
                {
                    result.code = 2;
                    result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                    context.Response.Write(new JavaScriptSerializer().Serialize(result));
                    return;
                }
                long ideaID = long.Parse(context.Request.Form["ideaID"].ToString());
                if (!db.ideasTbls.Any(c => c.ID == ideaID))
                {
                    result.code = 3;
                    result.message = ClassCollection.Message.IDEA_NOT_EXIST;
                    context.Response.Write(new JavaScriptSerializer().Serialize(result));
                    return;
                }
                try
                {
                    var dymmy = context.Request.Form["tags"];
                }
                catch
                {
                }
                var idea = db.ideasTbls.Single(c => c.ID == ideaID);
                var block = bool.Parse(context.Request.Form["block"]);
                var dt = new DateTime();
                dt = DateTime.Now;
                idea.isBlock = block;
                idea.regDate = dt;
                idea.userID = user.ID;
                db.SubmitChanges();

                result.code = 0;
                result.message = ClassCollection.Message.OPERATION_SUCCESS;
                context.Response.Write(new JavaScriptSerializer().Serialize(result));
                return;
            }
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