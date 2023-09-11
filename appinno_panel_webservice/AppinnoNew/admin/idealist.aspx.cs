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
    public partial class idealist : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var db = new DataAccessDataContext();
            if (ClassCollection.Methods.isLogin(Context))
            {
                var user = ClassCollection.Methods.getUserInSession(HttpContext.Current);
                var permission = ClassCollection.Accessing.Deserialize(db.accessTbls.Single(c => c.ID == user.accessID).per);

                if (!permission.bestIdeasCompetition.access.showlist)
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
        public static string getAllIdeas(string regDateFrom, string regDateTo
            , string ideastate
            , string fillterString, string orderType,
            string sortby, int pageIndex, int bestIdeaCompetitionId, bool justWinner=false)
        {
            ClassCollection.Model.IdeaAdminResult result = new ClassCollection.Model.IdeaAdminResult();
            result.result = new ClassCollection.Model.Result();
            const int pageSize = 30;
            var db = new DataAccessDataContext();

            int skipCount = (pageIndex - 1) * pageSize;

            if (!ClassCollection.Methods.isLogin(HttpContext.Current))
            {
                result.result.code = 1;
                result.result.message = ClassCollection.Message.LOGIN_FIRST;
                return new JavaScriptSerializer().Serialize(result);
            }

            if (!db.bestIdeaCompetitionsTbls.Any(c => c.ID == bestIdeaCompetitionId))
            {
                result.result.code = 3;
                result.result.message = ClassCollection.Message.COMPETITIONS_NOT_EXIST;
                return new JavaScriptSerializer().Serialize(result);
            }
            var user = ClassCollection.Methods.getUserInSession(HttpContext.Current);
            var permission = ClassCollection.Accessing.Deserialize(db.accessTbls.Single(c => c.ID == user.accessID).per);

            if (!permission.bestIdeasCompetition.access.showlist)
            {
                result.result.code = 2;
                result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }

            if (justWinner)
            {
                var ideas = db.ideasTbls.Where(c => c.bestIdeaCompetitionsID == bestIdeaCompetitionId && c.isBlock == false && c.ideasContentTbls.Any()).OrderByDescending(c => c.ideasLikeTbls.Count(p => p.isLike == true)).AsQueryable().Take(3);
                result.pageTitle = db.bestIdeaCompetitionsTbls.Single(c => c.ID == bestIdeaCompetitionId).title;
                result.idea = new List<Model.IdeaAdmin>();
                foreach (var item in ideas)
                {
                    var tmp = new ClassCollection.Model.IdeaAdmin();
                    tmp.ID = item.ID;
                    tmp.title = item.title;
                    tmp.isBlock = item.isBlock;
                    tmp.mUserName = item.mUserTbl.name + " " + item.mUserTbl.family;
                    tmp.regDate = Persia.Calendar.ConvertToPersian(item.regDate).ToString("m") + " - " + Persia.Calendar.ConvertToPersian(item.regDate).ToString("H");
                    result.idea.Add(tmp);
                }


                result.result.code = 0;
                result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
                return new JavaScriptSerializer().Serialize(result);
            }

            #region  regDate

            var regDatefromD = new DateTime();
            var regDatetoD = new DateTime();
            if (regDateFrom != "")
            {
                try
                {
                    var tt = regDateFrom.Split('/');
                    regDatefromD = Persia.Calendar.ConvertToGregorian(int.Parse(tt[0]), int.Parse(tt[1]), int.Parse(tt[2]), Persia.DateType.Persian);
                }
                catch
                {
                    result.result.code = 3;
                    result.result.message = ClassCollection.Message.DATE_INCORRECT;
                    return new JavaScriptSerializer().Serialize(result);
                }
            }
            if (regDateTo != "")
            {
                try
                {
                    var tt = regDateTo.Split('/');
                    regDatetoD = Persia.Calendar.ConvertToGregorian(int.Parse(tt[0]), int.Parse(tt[1]), int.Parse(tt[2]), Persia.DateType.Persian);
                }
                catch
                {
                    result.result.code = 3;
                    result.result.message = ClassCollection.Message.DATE_INCORRECT;
                    return new JavaScriptSerializer().Serialize(result);
                }
            }
            if (regDateTo != "" && regDateFrom != "")
            {
                if (regDatefromD.CompareTo(regDatetoD) > 0)
                {
                    result.result.code = 3;
                    result.result.message = ClassCollection.Message.DATE_INCORRECT;
                    return new JavaScriptSerializer().Serialize(result);
                }
            }
            #endregion

            var us = db.ideasTbls.Where(c => c.bestIdeaCompetitionsID == bestIdeaCompetitionId && c.ideasContentTbls.Any()).AsQueryable();


            if (regDateTo != "")
            {
                us = us.Where(c => c.regDate <= regDatetoD);
            }
            if (regDateFrom != "")
            {
                us = us.Where(c => c.regDate >= regDatefromD);
            }


            if (ideastate == "block")
            {
                us = us.Where(c => c.isBlock == true);
            }
            else if (ideastate == "unblock")
            {
                us = us.Where(c => c.isBlock == false);
            }

            #region search

            if (fillterString != "")
            {
                var search = new Search(fillterString);
                var ors = search.getOR;

                var predicate = PredicateBuilder.False<ideasTbl>();
                foreach (string keyword in ors)
                {
                    string temp = keyword;
                    predicate = predicate.Or(p => p.title.Contains(keyword) || p.ideasContentTbls.Any(c => c.contentType == 0 && c.value.Contains(keyword)));
                }

                us = us.Where(predicate);
            }

            if (orderType == "a")
            {
                switch (sortby)
                {
                    case "reg":
                        us = us.OrderBy(c => c.regDate);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (sortby)
                {
                    case "reg":
                        us = us.OrderByDescending(c => c.regDate);
                        break;
                    default:
                        break;
                }
            }

            var acList = us.ToList();

            var count = acList.Count();
            acList = acList.Skip(skipCount).Take(pageSize).ToList();

            #endregion

            result.idea = new List<Model.IdeaAdmin>();
            foreach (var item in acList)
            {
                var tmp = new ClassCollection.Model.IdeaAdmin();
                tmp.ID = item.ID;
                tmp.title = item.title;
                tmp.isBlock = item.isBlock;
                tmp.mUserName = item.mUserTbl.name + " " + item.mUserTbl.family;
                tmp.regDate = Persia.Calendar.ConvertToPersian(item.regDate).ToString("m") + " - " + Persia.Calendar.ConvertToPersian(item.regDate).ToString("H");
                result.idea.Add(tmp);
            }

            if (count % pageSize == 0)
                count = count / pageSize;
            else
                count = (count / pageSize) + 1;

            result.pageTitle = db.bestIdeaCompetitionsTbls.Single(c => c.ID == bestIdeaCompetitionId).title;
            result.pageCount = count;
            result.result.code = 0;
            result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
        }

        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string deleteIdea(long IdeaID)
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

            if (!permission.bestIdeasCompetition.access.delete)
            {
                result.code = 2;
                result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }
            if (db.ideasTbls.Any(c => c.ID == IdeaID) == false)
            {
                result.code = 2;
                result.message = ClassCollection.Message.IDEA_NOT_EXIST;
                return new JavaScriptSerializer().Serialize(result);
            }

            var idea = db.ideasTbls.Single(c => c.ID == IdeaID);
            db.ideasTagTbls.DeleteAllOnSubmit(idea.ideasTagTbls);
            db.SubmitChanges();

            var contents = db.ideasContentTbls.Where(c => c.ideasID == idea.ID);

            foreach (var item in contents)
            {

                if (item.contentType == 1)//image
                {
                    var image = item.value;
                    if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getIdeaImagesPath() + image)))
                    {
                        try
                        {
                            System.IO.File.Delete(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getIdeaImagesPath() + image));
                        }
                        catch
                        {
                            result.code = 3;
                            result.message = ClassCollection.Message.ERROR_DELETE_IMAGE;
                            return new JavaScriptSerializer().Serialize(result);

                        }
                    }


                }
                else if (item.contentType == 2)//video
                {
                    var video = item.value;
                    if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getIdeaVideosPath() + video)))
                    {
                        try
                        {
                            System.IO.File.Delete(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getIdeaVideosPath() + video));
                        }
                        catch
                        {
                            result.code = 4;
                            result.message = ClassCollection.Message.ERROR_DELETE_VIDEO;
                            return new JavaScriptSerializer().Serialize(result);
                        }

                    }
                }
            }

            db.ideasContentTbls.DeleteAllOnSubmit(contents);
            db.SubmitChanges();

            db.ideasLikeTbls.DeleteAllOnSubmit(db.ideasLikeTbls.Where(c => c.ideasID == idea.ID));
            db.SubmitChanges();

            db.ideasTbls.DeleteOnSubmit(idea);
            db.SubmitChanges();

            result.code = 0;
            result.message = ClassCollection.Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
        }


        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string deleteAllIdeas()
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

            if (!permission.bestIdeasCompetition.access.delete || !permission.news.access.showlist)
            {
                result.code = 2;
                result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }

            var ideas = db.ideasTbls;
            foreach (var item in ideas)
            {
                try
                {
                    deleteIdea(item.ID);
                }
                catch { }
            }

            result.code = 0;
            result.message = ClassCollection.Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
        }

    }
}

