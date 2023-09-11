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
    public partial class emyIran : System.Web.UI.Page
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
        public static string getAllTag()
        {
            ClassCollection.Model.TagListResult result = new ClassCollection.Model.TagListResult();
            result.result = new ClassCollection.Model.Result();


            if (!ClassCollection.Methods.isLogin(HttpContext.Current))
            {
                result.result.code = 1;
                result.result.message = ClassCollection.Message.LOGIN_FIRST;
                return new JavaScriptSerializer().Serialize(result);
            }

            var db = new DataAccessDataContext();
            var user = ClassCollection.Methods.getUserInSession(HttpContext.Current);

            var tags = db.tagTbls.OrderByDescending(c => c.ID).ToList();

            result.tag = new List<ClassCollection.Model.Tag>();
            foreach (var item in tags)
            {
                var tmp = new ClassCollection.Model.Tag();
                tmp.ID = item.ID;
                tmp.title = item.title;
                result.tag.Add(tmp);
            }

            result.result.code = 0;
            result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
        }

        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string getmyIran(int myIranID)
        {
            ClassCollection.Model.myIransResult result = new ClassCollection.Model.myIransResult();
            result.result = new ClassCollection.Model.Result();
                      bool status= true;

            if (!ClassCollection.Methods.isLogin(HttpContext.Current))
            {
                result.result.code = 1;
                result.result.message = ClassCollection.Message.LOGIN_FIRST;
                return new JavaScriptSerializer().Serialize(result);
            }
            var db = new DataAccessDataContext();
            var user = ClassCollection.Methods.getUserInSession(HttpContext.Current);
            var permission = ClassCollection.Accessing.Deserialize(db.accessTbls.Single(c => c.ID == user.accessID).per);
            var dt = new DateTime();
            dt = DateTime.Now;

            if (permission.myIranCompetition.access.showlist == false)
            {
                result.result.code = 2;
                result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }

            if (db.myIranTbls.Any(c => c.ID == myIranID) == false)
            {
                result.result.code = 2;
                result.result.message = ClassCollection.Message.COMPETITIONS_NOT_EXIST;
                result.myIran = null;

                return new JavaScriptSerializer().Serialize(result);
            }

            var myIran = db.myIranTbls.Single(c => c.ID == myIranID);
            
            if (db.responseTbls.Any(c => c.myIranID == myIranID))
            {
                status = false;
            }

            var contents = db.myIranContentTbls.Where(c => c.myIranID == myIran.ID).OrderBy(c=>c.pri);
            var tags = db.myIranTagTbls.Where(c => c.myIranID == myIranID);

           
            result.myIran = new ClassCollection.Model.FullMyIran();
            result.myIran.canEdit = status;
            result.myIran.ID = myIran.ID;
            result.myIran.userID = myIran.userID;
            result.myIran.title = myIran.title;
            result.myIran.isBlock = myIran.isBlock;
            result.myIran.startDate = Persia.Calendar.ConvertToPersian(myIran.startDate).ToString();
            result.myIran.endDate = Persia.Calendar.ConvertToPersian(myIran.endDate).ToString();
            result.myIran.isSendNotification = myIran.isSendNotification;
            result.myIran.notificationText = myIran.notificationText;

            result.myIran.contents = new List<ClassCollection.Model.myIransContent>();
            result.myIran.tag = new List<Model.Tag>();

            foreach (var item in tags)
            {
                var tmp = new ClassCollection.Model.Tag();
                tmp.ID = item.tagTbl.ID;
                tmp.title = item.tagTbl.title;

                result.myIran.tag.Add(tmp);
            }
            
            foreach (var item in contents)
            {
                var tmp = new ClassCollection.Model.myIransContent();

                tmp.ID = item.ID;
                tmp.type = item.contentType;

                if (item.contentType == 0)
                {
                    tmp.value = item.value;
                }
                else if (item.contentType == 1)
                {
                    tmp.value = ClassCollection.Methods.getMyIranImagesPath().Replace("~", "") + item.value;
                }
                else if (item.contentType == 2)
                {
                    tmp.value = ClassCollection.Methods.getMyIranVideosPath().Replace("~", "") + item.value;
                }


                result.myIran.contents.Add(tmp);
            }

            result.result.code = 0;
            result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
        }
    }
}

