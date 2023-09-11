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
    public class responseUploader : IHttpHandler, IRequiresSessionState
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
                    long s = long.Parse(context.Request.Form["responseID"].ToString());
                }
                catch
                {

                }
                if (permission.myIranCompetition.access.edit == false)
                {
                    result.code = 2;
                    result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                    context.Response.Write(new JavaScriptSerializer().Serialize(result));
                    return;
                }
                long responseID = long.Parse(context.Request.Form["responseID"].ToString());
                if (!db.responseTbls.Any(c => c.ID == responseID))
                {
                    result.code = 3;
                    result.message = ClassCollection.Message.ANSWER_NOT_EXIST;
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


                var dt = new DateTime();
                dt = DateTime.Now;
                var response = db.responseTbls.Single(c => c.ID == responseID);
                var isBlock = bool.Parse(context.Request.Form["block"]);
                var isCorrect = bool.Parse(context.Request.Form["iscorrect"]);
                var isWinner = bool.Parse(context.Request.Form["iswinner"]);
                var status = db.getMyIranStatus(response.myIranID);

                if (isWinner==true && (status == DataAccessDataContext.MyIranStatus.idle || status == DataAccessDataContext.MyIranStatus.sending))
                {
                    result.code = 5;
                    result.message = ClassCollection.Message.ANSWER_WINNER_STATUS;
                    context.Response.Write(new JavaScriptSerializer().Serialize(result));
                    return;
                }

                if (response.isWinner == false)
                {
                    response.isCorrect = isCorrect;
                    response.isBlock = isBlock;
                }

                if (response.isWinner == false)
                {
                    if (isCorrect)
                    {
                        response.isWinner = isWinner;
                    }
                    else if (isWinner == true)
                    {
                        result.code = 5;
                        result.message = ClassCollection.Message.WINNER_ISCORRECT_INCORRECT;
                        context.Response.Write(new JavaScriptSerializer().Serialize(result));
                        return;
                    }
                }
                else
                {
                    result.code = 5;
                    result.message = ClassCollection.Message.WINNER_EDIT_INCORRECT;
                    context.Response.Write(new JavaScriptSerializer().Serialize(result));
                    return;
                }

                response.userID = user.ID;
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