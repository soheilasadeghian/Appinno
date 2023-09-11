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
    public partial class ecreativityCompetition : System.Web.UI.Page
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
        public static string getcreativityCompetition(int creativityCompetitionID)
        {
            ClassCollection.Model.CreativitysCompetitionResult result = new ClassCollection.Model.CreativitysCompetitionResult();
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

            if (permission.creativityCompetition.access.showlist == false)
            {
                result.result.code = 2;
                result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }

            if (db.creativityCompetitionTbls.Any(c => c.ID == creativityCompetitionID) == false)
            {
                result.result.code = 2;
                result.result.message = ClassCollection.Message.COMPETITIONS_NOT_EXIST;
                result.creativityCompetition = null;

                return new JavaScriptSerializer().Serialize(result);
            }

            var creativityCompetition = db.creativityCompetitionTbls.Single(c => c.ID == creativityCompetitionID);
            
            if (db.answerTbls.Any(c => c.creativityCompetitionID == creativityCompetitionID))
            {
                status = false;
            }

            var contents = db.creativityCompetitionContentTbls.Where(c => c.creativityCompetitionID == creativityCompetition.ID).OrderBy(c=>c.pri);
            var tags = db.creativityCompetitionTagTbls.Where(c => c.creativityCompetitionID == creativityCompetitionID);

           
            result.creativityCompetition = new ClassCollection.Model.FullCreativityCompetition();
            result.creativityCompetition.canEdit = status;
            result.creativityCompetition.ID = creativityCompetition.ID;
            result.creativityCompetition.userID = creativityCompetition.userID;
            result.creativityCompetition.title = creativityCompetition.title;
            result.creativityCompetition.isBlock = creativityCompetition.isBlock;
            result.creativityCompetition.startDate = Persia.Calendar.ConvertToPersian(creativityCompetition.startDate).ToString();
            result.creativityCompetition.endDate = Persia.Calendar.ConvertToPersian(creativityCompetition.endDate).ToString();
            result.creativityCompetition.isSendNotification = creativityCompetition.isSendNotification;
            result.creativityCompetition.notificationText = creativityCompetition.notificationText;

            result.creativityCompetition.contents = new List<ClassCollection.Model.CreativitysCompetitionContent>();
            result.creativityCompetition.tag = new List<Model.Tag>();

            foreach (var item in tags)
            {
                var tmp = new ClassCollection.Model.Tag();
                tmp.ID = item.tagTbl.ID;
                tmp.title = item.tagTbl.title;

                result.creativityCompetition.tag.Add(tmp);
            }
            
            foreach (var item in contents)
            {
                var tmp = new ClassCollection.Model.CreativitysCompetitionContent();

                tmp.ID = item.ID;
                tmp.type = item.contentType;

                if (item.contentType == 0)
                {
                    tmp.value = item.value;
                }
                else if (item.contentType == 1)
                {
                    tmp.value = ClassCollection.Methods.getCreativityCompetitionImagesPath().Replace("~", "") + item.value;
                }
                else if (item.contentType == 2)
                {
                    tmp.value = ClassCollection.Methods.getCreativityCompetitionVideosPath().Replace("~", "") + item.value;
                }


                result.creativityCompetition.contents.Add(tmp);
            }

            result.result.code = 0;
            result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
        }
    }
}

