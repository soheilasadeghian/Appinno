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
    public partial class tag : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var db = new DataAccessDataContext();
            if (ClassCollection.Methods.isLogin(Context))
            {
                var user = ClassCollection.Methods.getUserInSession(HttpContext.Current);
                var permission = ClassCollection.Accessing.Deserialize(db.accessTbls.Single(c => c.ID == user.accessID).per);

                if (!permission.news.access.showlist && !permission.events.access.showlist && !permission.io.access.showlist && !permission.publication.access.showlist
                      && !permission.chart.access.showlist && !permission.download.access.showlist && !permission.poll.access.showlist && !permission.ican.access.showlist
                      && !permission.creativityCompetition.access.showlist && !permission.myIranCompetition.access.showlist && !permission.bestIdeasCompetition.access.showlist)
                {
                    Response.Redirect("dashboard.aspx");
                }

                //if (user.ID != ClassCollection.Methods.getAdminID())
                //{
                //    Response.Redirect("dashboard.aspx");
                //}
            }
            else
            {
                Response.Redirect("login.aspx");
            }
        }

        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string getAllTag(string fillterString, string orderType, int pageIndex)
        {
            const int pageSize = 30;

            ClassCollection.Model.TagListResult result = new ClassCollection.Model.TagListResult();
            result.result = new ClassCollection.Model.Result();

            int skipCount = (pageIndex - 1) * pageSize;

            if (!ClassCollection.Methods.isLogin(HttpContext.Current))
            {
                result.result.code = 1;
                result.result.message = ClassCollection.Message.LOGIN_FIRST;
                return new JavaScriptSerializer().Serialize(result);
            }

            var db = new DataAccessDataContext();
            var user = ClassCollection.Methods.getUserInSession(HttpContext.Current);
            var permission = ClassCollection.Accessing.Deserialize(db.accessTbls.Single(c => c.ID == user.accessID).per);

            if (!permission.news.access.showlist && !permission.events.access.showlist && !permission.io.access.showlist && !permission.publication.access.showlist
                      && !permission.chart.access.showlist && !permission.download.access.showlist && !permission.poll.access.showlist && !permission.ican.access.showlist
                      && !permission.creativityCompetition.access.showlist && !permission.myIranCompetition.access.showlist && !permission.bestIdeasCompetition.access.showlist)
            {
                result.result.code = 2;
                result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }

            var ac = db.tagTbls.AsQueryable<tagTbl>();

            #region search

            if (fillterString != "")
            {
                var search = new Search(fillterString);
                var ors = search.getOR;

                var predicate = PredicateBuilder.False<tagTbl>();
                foreach (string keyword in ors)
                {
                    string temp = keyword;
                    predicate = predicate.Or(p => p.title.Contains(keyword));
                }

                ac = ac.Where(predicate);
            }

            if (orderType == "a")
                ac = ac.OrderBy(c => c.ID);
            else
                ac = ac.OrderByDescending(c => c.ID);



            var acList = ac.ToList();

            var count = acList.Count();
            result.totalCount = count;
            acList = acList.Skip(skipCount).Take(pageSize).ToList();

            #endregion

            result.tag = new List<Model.Tag>();
            foreach (var item in acList)
            {
                var tmp = new Model.Tag();
                tmp.title = item.title;
                tmp.ID = item.ID;
                result.tag.Add(tmp);
            }
            if (count % pageSize == 0)
                count = count / pageSize;
            else
                count = (count / pageSize) + 1;

            result.pageCount = count;
            result.result.code = 0;
            result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
        }

        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string getTag(long tagID)
        {

            ClassCollection.Model.TagResult result = new ClassCollection.Model.TagResult();
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

            if (!permission.news.access.showlist && !permission.events.access.showlist && !permission.io.access.showlist && !permission.publication.access.showlist
                      && !permission.chart.access.showlist && !permission.download.access.showlist && !permission.poll.access.showlist && !permission.ican.access.showlist
                      && !permission.creativityCompetition.access.showlist && !permission.myIranCompetition.access.showlist && !permission.bestIdeasCompetition.access.showlist)
            {
                result.result.code = 2;
                result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }
            if (db.tagTbls.Any(c => c.ID == tagID) == false)
            {
                result.result.code = 3;
                result.result.message = "تگ مورد نظر یافت نشد";
                return new JavaScriptSerializer().Serialize(result);

            }
            var tag = db.tagTbls.Single(c => c.ID == tagID);

            result.tag = new Model.Tag();
            result.tag.ID = tag.ID;
            result.tag.title = tag.title;

            result.result.code = 0;
            result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
        }

        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string insertTag(string title)
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

            if (!permission.news.access.showlist && !permission.events.access.showlist && !permission.io.access.showlist && !permission.publication.access.showlist
                     && !permission.chart.access.showlist && !permission.download.access.showlist && !permission.poll.access.showlist && !permission.ican.access.showlist
                     && !permission.creativityCompetition.access.showlist && !permission.myIranCompetition.access.showlist && !permission.bestIdeasCompetition.access.showlist)
            {
                result.code = 2;
                result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }

            if (title.Length == 0)
            {
                result.code = 3;
                result.message = "تگ وارد شده صحیح نمی باشد ";
                return new JavaScriptSerializer().Serialize(result);
            }

            if (db.tagTbls.Any(c => c.title == title))
            {
                result.code = 4;
                result.message = "تگ وارد شده تکراری می باشد ";
                return new JavaScriptSerializer().Serialize(result);
            }

            if (title != "")
            {
                var tmp = new tagTbl();
                tmp.title = title;

                db.tagTbls.InsertOnSubmit(tmp);
                db.SubmitChanges();
            }

            result.code = 0;
            result.message = ClassCollection.Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
        }


        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string editTag(long tagID, string title)
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

            if (!permission.news.access.showlist && !permission.events.access.showlist && !permission.io.access.showlist && !permission.publication.access.showlist
                      && !permission.chart.access.showlist && !permission.download.access.showlist && !permission.poll.access.showlist && !permission.ican.access.showlist
                      && !permission.creativityCompetition.access.showlist && !permission.myIranCompetition.access.showlist && !permission.bestIdeasCompetition.access.showlist)
            {
                result.code = 2;
                result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }

            if (db.tagTbls.Any(c => c.ID == tagID) == false)
            {
                result.code = 3;
                result.message = "تگ مورد نظر یافت نشد";
                return new JavaScriptSerializer().Serialize(result);
            }

            if (title.Length == 0)
            {
                result.code = 3;
                result.message = "تگ وارد شده صحیح نمی باشد ";
                return new JavaScriptSerializer().Serialize(result);
            }

            if (db.tagTbls.Any(c => c.title == title && c.ID != tagID))
            {
                result.code = 4;
                result.message = "تگ وارد شده تکراری می باشد ";
                return new JavaScriptSerializer().Serialize(result);
            }

            if (title != "")
            {
                var tmp = db.tagTbls.Single(c => c.ID == tagID);
                tmp.title = title;
                db.SubmitChanges();
            }

            result.code = 0;
            result.message = ClassCollection.Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
        }

        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string deleteTag(long tagID)
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

            if (!permission.news.access.showlist && !permission.events.access.showlist && !permission.io.access.showlist && !permission.publication.access.showlist
                     && !permission.chart.access.showlist && !permission.download.access.showlist && !permission.poll.access.showlist && !permission.ican.access.showlist
                     && !permission.creativityCompetition.access.showlist && !permission.myIranCompetition.access.showlist && !permission.bestIdeasCompetition.access.showlist)

            {
                result.code = 2;
                result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }

            if (db.tagTbls.Any(c => c.ID == tagID) == false)
            {
                result.code = 3;
                result.message = "تگ مورد نظر یافت نشد";
                return new JavaScriptSerializer().Serialize(result);
            }

            var tmp = db.tagTbls.Single(c => c.ID == tagID);
            if (tmp.newsTagTbls.Any() || tmp.bestIdeaCompetitionsTagTbls.Any() || tmp.creativityCompetitionTagTbls.Any() || tmp.downloadTagTbls.Any() || tmp.eventTagTbls.Any() || tmp.ideasTagTbls.Any() || tmp.ioTagTbls.Any() || tmp.masterReportTagTbls.Any() || tmp.myIranTagTbls.Any() || tmp .newsTagTbls.Any() || tmp.pollTagTbls.Any() || tmp.publicationTagTbls.Any())
            {
                result.code = 4;
                result.message = "تگ مورد نظر درحال استفاده می باشد";
                return new JavaScriptSerializer().Serialize(result);
            }


            db.tagTbls.DeleteOnSubmit(tmp);
            db.SubmitChanges();

            result.code = 0;
            result.message = ClassCollection.Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
        }


        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string deleteAllTag()
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

            if (!permission.news.access.showlist && !permission.events.access.showlist && !permission.io.access.showlist && !permission.publication.access.showlist
                     && !permission.chart.access.showlist && !permission.download.access.showlist && !permission.poll.access.showlist && !permission.ican.access.showlist
                     && !permission.creativityCompetition.access.showlist && !permission.myIranCompetition.access.showlist && !permission.bestIdeasCompetition.access.showlist)

            {
                result.code = 2;
                result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }

            var tags = db.tagTbls.Where(c => c.newsTagTbls.Any() == false);

            db.tagTbls.DeleteAllOnSubmit(tags);
            db.SubmitChanges();

            result.code = 0;
            result.message = ClassCollection.Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
        }
    }
}

