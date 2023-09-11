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
    public partial class oprators : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var db = new DataAccessDataContext();
            if (ClassCollection.Methods.isLogin(Context))
            {
                var user = ClassCollection.Methods.getUserInSession(HttpContext.Current);
                var permission = ClassCollection.Accessing.Deserialize(db.accessTbls.Single(c => c.ID == user.accessID).per);

                if (!permission.operators.access.showlist)
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
        public static string getAllAccess()
        {
            ClassCollection.Model.AccessListResult result = new ClassCollection.Model.AccessListResult();
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

            if (!permission.operators.access.showlist)
            {
                result.result.code = 2;
                result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }

            var ac = db.accessTbls.Where(c => c.accessID == user.accessID || user.accessID == 1).ToList();

            result.access = new List<ClassCollection.Model.Access>();

            foreach (var item in ac)
            {
                var tmp = new ClassCollection.Model.Access();
                tmp.ID = item.ID;
                tmp.title = item.title;
                result.access.Add(tmp);
            }
            result.result.code = 0;
            result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
        }

        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string getAllOprators(
            string fillterString,
            int accessID,
            int pageIndex)
        {
            const int pageSize = 30;

            ClassCollection.Model.OpratorListResult result = new ClassCollection.Model.OpratorListResult();
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

            if (!permission.operators.access.showlist)
            {
                result.result.code = 2;
                result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }
            if (db.accessTbls.Any(c => c.ID == accessID) == false && accessID != -1)
            {
                result.result.code = 3;
                result.result.message = ClassCollection.Message.ACCESS_NOT_EXIST;
                return new JavaScriptSerializer().Serialize(result);
            }
            var us = db.userTbls.Where(c => c.accessTbl.accessID == user.accessID || user.ID == 1).AsQueryable<userTbl>();

            #region search

            if (fillterString != "")
            {
                var search = new Search(fillterString);

                var ors = search.getOR;

                var predicate = PredicateBuilder.False<userTbl>();
                foreach (string keyword in ors)
                {
                    string temp = keyword;
                    predicate = predicate.Or(p => p.family.Contains(keyword) || p.name.Contains(keyword) || p.emailtell.Contains(keyword));
                }

                us = us.Where(predicate);
            }
            us = us.Where(c => c.accessID == accessID || accessID == -1);

            var usList = us.ToList();

            var count = usList.Count();
            result.totalCount = count;
            usList = usList.Skip(skipCount).Take(pageSize).ToList();

            #endregion

            result.oprator = new List<Model.Oprator>();
            foreach (var item in usList)
            {
                var tmp = new ClassCollection.Model.Oprator();
                tmp.ID = item.ID;
                tmp.accessTitle = item.accessTbl.title;
                tmp.accessID = item.accessID;
                tmp.emailtell = item.emailtell;
                tmp.family = item.family;
                tmp.name = item.name;

                result.oprator.Add(tmp);
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
        public static string insertOprators(
            string name,
            string family,
            string password,
            string emailtell,
            int accessID
            )
        {
            ClassCollection.Model.Result result = new Model.Result();

            if (!ClassCollection.Methods.isLogin(HttpContext.Current))
            {
                result.code = 1;
                result.message = ClassCollection.Message.LOGIN_FIRST;
                return new JavaScriptSerializer().Serialize(result);
            }

            var dt = new DateTime();
            dt = DateTime.Now;
            var db = new DataAccessDataContext();
            var user = ClassCollection.Methods.getUserInSession(HttpContext.Current);
            var permission = ClassCollection.Accessing.Deserialize(db.accessTbls.Single(c => c.ID == user.accessID).per);

            if (!permission.operators.access.insert)
            {
                result.code = 2;
                result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }

            name = name.TrimEnd().TrimStart();
            family = family.TrimStart().TrimEnd();
            emailtell = emailtell.TrimEnd().TrimStart();

            if (name == "")
            {
                result.code = 3;
                result.message = ClassCollection.Message.NAME_INCORRECT;
                return new JavaScriptSerializer().Serialize(result);
            }
            if (family == "")
            {
                result.code = 4;
                result.message = ClassCollection.Message.FAMILY_INCORRECT;
                return new JavaScriptSerializer().Serialize(result);
            }
            if (emailtell == "")
            {
                result.code = 5;
                result.message = ClassCollection.Message.EMAILTELL_INCORRECT;
                return new JavaScriptSerializer().Serialize(result);
            }
            if (password == "")
            {
                result.code = 7;
                result.message = ClassCollection.Message.PASSWORD_INCORRECT;
                return new JavaScriptSerializer().Serialize(result);
            }
            if (db.accessTbls.Any(c => c.ID == accessID) == false)
            {
                result.code = 6;
                result.message = ClassCollection.Message.ACCESS_NOT_EXIST;
                return new JavaScriptSerializer().Serialize(result);
            }
            if (db.userTbls.Any(c => c.emailtell == emailtell))
            {
                result.code = 8;
                result.message = ClassCollection.Message.USER_EXIST;
                return new JavaScriptSerializer().Serialize(result);
            }
            var tmp = new userTbl();
            tmp.lastedit = user.name + " " + user.family + " // " + user.emailtell;
            tmp.emailtell = emailtell;
            tmp.family = family;
            tmp.isBlock = false;
            tmp.name = name;
            tmp.password = ClassCollection.Methods.md5(password);
            tmp.regDate = dt;
            tmp.accessID = accessID;
            db.userTbls.InsertOnSubmit(tmp);
            db.SubmitChanges();

            result.code = 0;
            result.message = Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
        }
        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string editOprators(
             long opratorID,
             string name,
             string family,
             string password,
             string emailtell,
             int accessID
             )
        {
            ClassCollection.Model.Result result = new Model.Result();

            if (!ClassCollection.Methods.isLogin(HttpContext.Current))
            {
                result.code = 1;
                result.message = ClassCollection.Message.LOGIN_FIRST;
                return new JavaScriptSerializer().Serialize(result);
            }

            var dt = new DateTime();
            dt = DateTime.Now;
            var db = new DataAccessDataContext();
            var user = ClassCollection.Methods.getUserInSession(HttpContext.Current);
            var permission = ClassCollection.Accessing.Deserialize(db.accessTbls.Single(c => c.ID == user.accessID).per);

            if (!permission.operators.access.insert || opratorID == 1)
            {
                result.code = 2;
                result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }

            name = name.TrimEnd().TrimStart();
            family = family.TrimStart().TrimEnd();
            emailtell = emailtell.TrimEnd().TrimStart();

            if (name == "")
            {
                result.code = 3;
                result.message = ClassCollection.Message.NAME_INCORRECT;
                return new JavaScriptSerializer().Serialize(result);
            }
            if (family == "")
            {
                result.code = 4;
                result.message = ClassCollection.Message.FAMILY_INCORRECT;
                return new JavaScriptSerializer().Serialize(result);
            }
            if (emailtell == "")
            {
                result.code = 5;
                result.message = ClassCollection.Message.EMAILTELL_INCORRECT;
                return new JavaScriptSerializer().Serialize(result);
            }
            if (db.accessTbls.Any(c => c.ID == accessID) == false)
            {
                result.code = 6;
                result.message = ClassCollection.Message.ACCESS_NOT_EXIST;
                return new JavaScriptSerializer().Serialize(result);
            }
            if (db.userTbls.Any(c => c.ID == opratorID) == false)
            {
                result.code = 7;
                result.message = ClassCollection.Message.USER_NOT_EXIST;
                return new JavaScriptSerializer().Serialize(result);
            }
            var tmp = db.userTbls.Single(c => c.ID == opratorID);
            tmp.lastedit = user.name + " " + user.family + " // " + user.emailtell;
            tmp.emailtell = emailtell;
            tmp.family = family;
            tmp.isBlock = false;
            tmp.name = name;
            if (password != "")
                tmp.password = ClassCollection.Methods.md5(password);
            tmp.regDate = dt;
            tmp.accessID = accessID;

            db.SubmitChanges();

            result.code = 0;
            result.message = Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
        }
        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string getOprator(long opratorID)
        {
            ClassCollection.Model.OpratorResult result = new Model.OpratorResult();
            result.result = new Model.Result();

            if (!ClassCollection.Methods.isLogin(HttpContext.Current))
            {
                result.result.code = 1;
                result.result.message = ClassCollection.Message.LOGIN_FIRST;
                return new JavaScriptSerializer().Serialize(result);
            }

            var db = new DataAccessDataContext();
            var user = ClassCollection.Methods.getUserInSession(HttpContext.Current);
            var permission = ClassCollection.Accessing.Deserialize(db.accessTbls.Single(c => c.ID == user.accessID).per);

            if (!permission.operators.access.edit)
            {
                result.result.code = 2;
                result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }

            if (db.userTbls.Any(c => c.ID == opratorID) == false)
            {
                result.result.code = 3;
                result.result.message = ClassCollection.Message.USER_NOT_EXIST;
                return new JavaScriptSerializer().Serialize(result);
            }
            var obj = db.userTbls.Single(c => c.ID == opratorID);

            result.oprator = new ClassCollection.Model.Oprator();
            result.oprator.ID = obj.ID;
            result.oprator.name = obj.name;
            result.oprator.family = obj.family;
            result.oprator.emailtell = obj.emailtell;
            result.oprator.accessID = obj.accessID;
            result.oprator.accessTitle = obj.accessTbl.title;

            result.result.code = 0;
            result.result.message = Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
        }

        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string deleteOprator(long opratorID)
        {
            ClassCollection.Model.Result result = new Model.Result();

            if (!ClassCollection.Methods.isLogin(HttpContext.Current))
            {
                result.code = 1;
                result.message = ClassCollection.Message.LOGIN_FIRST;
                return new JavaScriptSerializer().Serialize(result);
            }

            var db = new DataAccessDataContext();
            var user = ClassCollection.Methods.getUserInSession(HttpContext.Current);
            var permission = ClassCollection.Accessing.Deserialize(db.accessTbls.Single(c => c.ID == user.accessID).per);

            if (!permission.operators.access.delete || opratorID == 1)
            {
                result.code = 2;
                result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }

            if (db.userTbls.Any(c => c.ID == opratorID) == false)
            {
                result.code = 3;
                result.message = ClassCollection.Message.USER_NOT_EXIST;
                return new JavaScriptSerializer().Serialize(result);
            }

            var obj = db.userTbls.Single(c => c.ID == opratorID);

            var inuse = obj.bestIdeaCompetitionsTbls.Any() || obj.creativityCompetitionTbls.Any() || obj.myIranTbls.Any() || obj.pollTbls.Any() || obj.pushMessageTbls.Any();
            inuse = inuse || db.newsTbls.Any(c => c.userID == obj.ID) || db.eventsTbls.Any(c => c.userID == obj.ID) || db.ioTbls.Any(c => c.userID == obj.ID)
                    || db.publicationTbls.Any(c => c.userID == obj.ID) || db.icanTbls.Any(c => c.userID == obj.ID) || db.masterReportTbls.Any(c => c.userID == obj.ID)
                    || db.downloadTbls.Any(c => c.userID == obj.ID);
            inuse = inuse || db.newsCommentTbls.Any(c => c.userID == obj.ID) || db.eventCommentTbls.Any(c => c.userID == obj.ID) || db.ioCommentTbls.Any(c => c.userID == obj.ID)
                    || db.publicationCommentTbls.Any(c => c.userID == obj.ID) || db.icanCommentTbls.Any(c => c.userID == obj.ID) || db.reportCommentTbls.Any(c => c.userID == obj.ID)
                    || db.downloadCommentTbls.Any(c => c.userID == obj.ID);
            if (inuse)
            {
                result.code = 3;
                result.message = ClassCollection.Message.DELETE_OPERATOR_INCORRECT;
                return new JavaScriptSerializer().Serialize(result);
            }

            db.userTbls.DeleteOnSubmit(obj);
            db.SubmitChanges();

            result.code = 0;
            result.message = Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
        }
    }
}

