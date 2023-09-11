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
    public partial class cdownload : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var db = new DataAccessDataContext();
            if (ClassCollection.Methods.isLogin(Context))
            {
                var user = ClassCollection.Methods.getUserInSession(HttpContext.Current);
                var permission = ClassCollection.Accessing.Deserialize(db.accessTbls.Single(c => c.ID == user.accessID).per);

                if (!permission.download.access.showlist)
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
        public static string getAllComment(string from, string to, string commentstate, string fillterString,string orderType, string sortby, int pageIndex)
        {
            const int pageSize = 30;

            ClassCollection.Model.adminCommentListResult result = new ClassCollection.Model.adminCommentListResult();
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

            if (!permission.download.access.showlist)
            {
                result.result.code = 2;
                result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }
            var fromD = new DateTime();
            var toD = new DateTime();
            if (from != "")
            {
                try
                {
                    var tt = from.Split('/');
                    fromD = Persia.Calendar.ConvertToGregorian(int.Parse(tt[0]), int.Parse(tt[1]), int.Parse(tt[2]), Persia.DateType.Persian);
                }
                catch
                {
                    result.result.code = 3;
                    result.result.message = ClassCollection.Message.DATE_INCORRECT;
                    return new JavaScriptSerializer().Serialize(result);
                }
            }
            if (to != "")
            {
                try
                {
                    var tt = to.Split('/');
                    toD = Persia.Calendar.ConvertToGregorian(int.Parse(tt[0]), int.Parse(tt[1]), int.Parse(tt[2]), Persia.DateType.Persian);
                }
                catch
                {
                    result.result.code = 3;
                    result.result.message = ClassCollection.Message.DATE_INCORRECT;
                    return new JavaScriptSerializer().Serialize(result);
                }
            }
            if (to != "" && from != "")
            {
                if (fromD.CompareTo(toD) > 0)
                {
                    result.result.code = 3;
                    result.result.message = ClassCollection.Message.DATE_INCORRECT;
                    return new JavaScriptSerializer().Serialize(result);
                }
            }

            var com = db.downloadCommentTbls.AsQueryable();

            if (to != "")
            {
                com = com.Where(c => c.regDate <= toD);
            }
            if (from != "")
            {
                com = com.Where(c => c.regDate >= fromD);
            }

            if (commentstate == "block")
            {
                com = com.Where(c => c.isBlock == true);
            }
            else if (commentstate == "unblock")
            {
                com = com.Where(c => c.isBlock == false);
            }

            #region search

            if (fillterString != "")
            {
                var search = new Search(fillterString);
                var ors = search.getOR;

                var predicate = PredicateBuilder.False<downloadCommentTbl>();
                foreach (string keyword in ors)
                {
                    string temp = keyword;
                    predicate = predicate.Or(p => p.fullName.Contains(keyword)  || p.email.Contains(keyword) || p.mobile.Contains(keyword) || p.text.Contains(keyword));
                }

                com = com.Where(predicate);
            }

            if (orderType == "a")
            {
                switch (sortby)
                {
                    case "name":
                        com = com.OrderBy(c => c.fullName);
                        break;
                   
                    case "regdate":
                        com = com.OrderBy(c => c.regDate);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (sortby)
                {
                    case "name":
                        com = com.OrderByDescending(c => c.fullName);
                        break;
                    case "regdate":
                        com = com.OrderByDescending(c => c.regDate);
                        break;
                    default:
                        break;
                }
            }

            var acList = com.ToList();

            var count = acList.Count();
            acList = acList.Skip(skipCount).Take(pageSize).ToList();

            #endregion

            result.comment = new List<Model.adminComment>();
            foreach (var item in acList)
            {
                var tmp = new ClassCollection.Model.adminComment();
                tmp.regDate = Persia.Calendar.ConvertToPersian(item.regDate).ToString("m") + " - " + Persia.Calendar.ConvertToPersian(item.regDate).ToString("H");
                tmp.ID = item.ID;
                tmp.fullName = item.fullName;
                tmp.newsID = item.downloadID;
                tmp.text = item.text;
                tmp.newsTitle = item.downloadTbl.title;
                tmp.mobileOrTell = !string.IsNullOrEmpty(item.mobile) ? item.mobile : item.email;
                tmp.isBlock = item.isBlock;
                result.comment.Add(tmp);
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
        public static string blockComment(long commentID,bool isBlock)
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


            if (!permission.download.access.showlist)
            {
                result.code = 2;
                result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }
            if (db.downloadCommentTbls.Any(c => c.ID == commentID) == false)
            {
                result.code = 3;
                result.message = ClassCollection.Message.COMMENT_NOT_EXIST;
                return new JavaScriptSerializer().Serialize(result);
            }

            var tmp = db.downloadCommentTbls.Single(c => c.ID == commentID);

            tmp.userID = user.ID;
            tmp.isBlock = isBlock;
            db.SubmitChanges();

            result.code = 0;
            result.message = ClassCollection.Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);

        }


        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string deleteComment(long commentID)
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

            if (!permission.download.access.showlist)
            {
                result.code = 2;
                result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }
            if (db.downloadCommentTbls.Any(c => c.ID == commentID) == false)
            {
                result.code = 3;
                result.message = ClassCollection.Message.COMMENT_NOT_EXIST;
                return new JavaScriptSerializer().Serialize(result);
            }

            var tmp = db.downloadCommentTbls.Single(c => c.ID == commentID);


            db.downloadCommentTbls.DeleteOnSubmit(tmp);
            db.SubmitChanges();

            result.code = 0;
            result.message = ClassCollection.Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
        }


        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string deleteAllComment()
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

            if (!permission.download.access.showlist)
            {
                result.code = 2;
                result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }

            db.downloadCommentTbls.DeleteAllOnSubmit(db.downloadCommentTbls);
            db.SubmitChanges();

            result.code = 0;
            result.message = ClassCollection.Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
        }

    }
}

