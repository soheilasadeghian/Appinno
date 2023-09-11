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
    public partial class eidea : System.Web.UI.Page
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
        public static string getIdea(int ideaID)
        {
            ClassCollection.Model.IdeaResult result = new ClassCollection.Model.IdeaResult();
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

            if (permission.bestIdeasCompetition.access.showlist == false)
            {
                result.result.code = 2;
                result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }

            if (db.ideasTbls.Any(c => c.ID == ideaID) == false)
            {
                result.result.code = 2;
                result.result.message = ClassCollection.Message.IDEA_NOT_EXIST;
                result.idea = null;

                return new JavaScriptSerializer().Serialize(result);
            }

            var idea = db.ideasTbls.Single(c => c.ID == ideaID);

            var contents = db.ideasContentTbls.Where(c => c.ideasID == idea.ID).OrderBy(c=>c.pri);
            var tags = db.ideasTagTbls.Where(c => c.ideasID == ideaID);


            result.idea = new ClassCollection.Model.fullIdea();
            result.idea.ID = idea.ID;
            result.idea.title = idea.title;
            result.idea.isBlock = idea.isBlock;
            result.idea.mUserID = idea.mUserID;
            result.idea.bestIdeaCompetitionsID = idea.bestIdeaCompetitionsID;
            result.idea.fullname = idea.mUserTbl.name + " " + idea.mUserTbl.family;

            result.idea.contents = new List<ClassCollection.Model.ideaContent>();
            result.idea.tag = new List<Model.Tag>();

            foreach (var item in tags)
            {
                var tmp = new ClassCollection.Model.Tag();
                tmp.ID = item.tagTbl.ID;
                tmp.title = item.tagTbl.title;

                result.idea.tag.Add(tmp);
            }

            foreach (var item in contents)
            {
                var tmp = new ClassCollection.Model.ideaContent();

                tmp.ID = item.ID;
                tmp.type = item.contentType;

                if (item.contentType == 0)
                {
                    tmp.value = item.value;
                }
                else if (item.contentType == 1)
                {
                    tmp.value = ClassCollection.Methods.getIdeaImagesPath().Replace("~", "") + item.value;
                }
                else if (item.contentType == 2)
                {
                    tmp.value = ClassCollection.Methods.getIdeaVideosPath().Replace("~", "") + item.value;
                }

                result.idea.contents.Add(tmp);
            }
          //  result.bestIdeasCompetitionID = db.ideasTbls.Single(c => c.ID == ideaID).bestIdeaCompetitionsID;

            db.ideasTagTbls.Where(c => c.ideasID == ideaID);
            result.result.code = 0;
            result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
        }
    }
}

