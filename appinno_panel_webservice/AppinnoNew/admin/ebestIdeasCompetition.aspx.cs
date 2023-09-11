﻿using AppinnoNew.ClassCollection;
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
    public partial class ebestIdeasCompetition : System.Web.UI.Page
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
        public static string getbestIdeasCompetition(int bestIdeasCompetitionID)
        {
            ClassCollection.Model.BestIdeasCompetitionResult result = new ClassCollection.Model.BestIdeasCompetitionResult();
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

            if (permission.bestIdeasCompetition.access.showlist == false)
            {
                result.result.code = 2;
                result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }

            if (db.bestIdeaCompetitionsTbls.Any(c => c.ID == bestIdeasCompetitionID) == false)
            {
                result.result.code = 2;
                result.result.message = ClassCollection.Message.COMPETITIONS_NOT_EXIST;
                result.bestIdeasCompetition = null;

                return new JavaScriptSerializer().Serialize(result);
            }

            var bestIdeaCompetition = db.bestIdeaCompetitionsTbls.Single(c => c.ID == bestIdeasCompetitionID);

            if (db.ideasTbls.Any(c => c.bestIdeaCompetitionsID == bestIdeasCompetitionID))
            {
                status = false;
            }

            var contents = db.bestIdeaCompetitionsContentTbls.Where(c => c.bestIdeaCompetitionsID == bestIdeaCompetition.ID).OrderBy(c=>c.pri);
            var tags = db.bestIdeaCompetitionsTagTbls.Where(c => c.bestIdeaCompetitionsID == bestIdeasCompetitionID);

           
            result.bestIdeasCompetition = new ClassCollection.Model.FullBestIdeasCompetition();
            result.bestIdeasCompetition.canEdit = status;
            result.bestIdeasCompetition.ID = bestIdeaCompetition.ID;
            result.bestIdeasCompetition.creatorID = bestIdeaCompetition.creatorID;
            result.bestIdeasCompetition.title = bestIdeaCompetition.title;
            result.bestIdeasCompetition.isBlock = bestIdeaCompetition.isBlock;
            result.bestIdeasCompetition.startDate = Persia.Calendar.ConvertToPersian(bestIdeaCompetition.startDate).ToString();
            result.bestIdeasCompetition.endDate = Persia.Calendar.ConvertToPersian(bestIdeaCompetition.endDate).ToString();
            result.bestIdeasCompetition.resultVoteDate = Persia.Calendar.ConvertToPersian(bestIdeaCompetition.resultVoteDate).ToString();
            result.bestIdeasCompetition.isSendNotification = bestIdeaCompetition.isSendNotification;
            result.bestIdeasCompetition.notificationText = bestIdeaCompetition.notificationText;

            result.bestIdeasCompetition.contents = new List<ClassCollection.Model.BestIdeasCompetitionContent>();
            result.bestIdeasCompetition.tag = new List<Model.Tag>();

            foreach (var item in tags)
            {
                var tmp = new ClassCollection.Model.Tag();
                tmp.ID = item.tagTbl.ID;
                tmp.title = item.tagTbl.title;

                result.bestIdeasCompetition.tag.Add(tmp);
            }
            
            foreach (var item in contents)
            {
                var tmp = new ClassCollection.Model.BestIdeasCompetitionContent();

                tmp.ID = item.ID;
                tmp.type = item.contentType;

                if (item.contentType == 0)
                {
                    tmp.value = item.value;
                }
                else if (item.contentType == 1)
                {
                    tmp.value = ClassCollection.Methods.getBestIdeaCompetitionsImagesPath().Replace("~", "") + item.value;
                }
                else if (item.contentType == 2)
                {
                    tmp.value = ClassCollection.Methods.getBestIdeaCompetitionsVideosPath().Replace("~", "") + item.value;
                }


                result.bestIdeasCompetition.contents.Add(tmp);
            }

            result.result.code = 0;
            result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
        }
    }
}

