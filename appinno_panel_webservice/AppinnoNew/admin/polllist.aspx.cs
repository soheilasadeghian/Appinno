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
    public partial class polllist : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var db = new DataAccessDataContext();
            if (ClassCollection.Methods.isLogin(Context))
            {
                var user = ClassCollection.Methods.getUserInSession(HttpContext.Current);
                var permission = ClassCollection.Accessing.Deserialize(db.accessTbls.Single(c => c.ID == user.accessID).per);

                if (!permission.news.access.showlist)
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
        public static string getAllPoll(string regDateFrom, string regDateTo
            , string pubDateFrom, string pubDateTo, string pollstate
            , string fillterString, long subgroupID, string orderType,
            string sortby, int pageIndex)
        {
            ClassCollection.Model.PollListPanelResult result = new ClassCollection.Model.PollListPanelResult();
            result.result = new ClassCollection.Model.Result();
            const int pageSize = 30;

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

            if (!permission.poll.access.showlist)
            {
                result.result.code = 2;
                result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }

            #region publish regDate

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

            #region publish Date
            var dt = new DateTime();
            dt = DateTime.Now;
            var pubDatefromD = new DateTime();
            var pubDatetoD = new DateTime();
            if (pubDateFrom != "")
            {
                try
                {
                    var tt = pubDateFrom.Split('/');
                    pubDatefromD = Persia.Calendar.ConvertToGregorian(int.Parse(tt[0]), int.Parse(tt[1]), int.Parse(tt[2]), Persia.DateType.Persian);
                }
                catch
                {
                    result.result.code = 3;
                    result.result.message = ClassCollection.Message.DATE_INCORRECT;
                    return new JavaScriptSerializer().Serialize(result);
                }
            }
            if (pubDateTo != "")
            {
                try
                {
                    var tt = pubDateTo.Split('/');
                    pubDatetoD = Persia.Calendar.ConvertToGregorian(int.Parse(tt[0]), int.Parse(tt[1]), int.Parse(tt[2]), Persia.DateType.Persian);
                }
                catch
                {
                    result.result.code = 3;
                    result.result.message = ClassCollection.Message.DATE_INCORRECT;
                    return new JavaScriptSerializer().Serialize(result);
                }
            }
            if (pubDateTo != "" && pubDateFrom != "")
            {
                if (pubDatefromD.CompareTo(pubDatetoD) > 0)
                {
                    result.result.code = 3;
                    result.result.message = ClassCollection.Message.DATE_INCORRECT;
                    return new JavaScriptSerializer().Serialize(result);
                }
            }
            #endregion

            var us = db.pollTbls.Where(c => c.pollContentTbls.Any()).AsQueryable();

            if (regDateTo != "")
            {
                us = us.Where(c => c.regDate <= regDatetoD);
            }
            if (regDateFrom != "")
            {
                us = us.Where(c => c.regDate >= regDatefromD);
            }
            if (pubDateTo != "")
            {
                us = us.Where(c => c.startDate <= pubDatetoD);
            }
            if (pubDateFrom != "")
            {
                us = us.Where(c => c.startDate >= pubDatefromD);
            }
            if (subgroupID == -1)
            {
                us = us.Where(c => c.pollSubGroupTbls.Any() == false);
            }
            else if (subgroupID == -2)
            {
                us = us.Where(c => c.pollSubGroupTbls.Any());
            }
            else if (subgroupID > 0)
            {
                if (db.SubGroupTbls.Any(c => c.ID == subgroupID) == false)
                {
                    result.result.code = 3;
                    result.result.message = ClassCollection.Message.SUBGROUP_NOT_EXIST;
                    return new JavaScriptSerializer().Serialize(result);
                }
                us = us.Where(c => c.pollSubGroupTbls.Any(p => p.subGroupID == subgroupID));
            }
            if (pollstate == "block")
            {
                us = us.Where(c => c.isBlock == true);
            }
            else if (pollstate == "unblock")
            {
                us = us.Where(c => c.isBlock == false);
            }
            else if (pollstate == "finished")
            {
                us = us.Where(c => c.finishedDate.Date < dt.Date);
            }
            else if (pollstate == "voting")
            {
                us = us.Where(c => c.finishedDate.Date >= dt.Date);
            }
            else if (pollstate == "hasimg")
            {
                us = us.Where(c => c.pollContentTbls.Any(x => x.contentType == 1));
            }

            #region search

            if (fillterString != "")
            {
                var search = new Search(fillterString);
                var ors = search.getOR;

                var predicate = PredicateBuilder.False<pollTbl>();
                foreach (string keyword in ors)
                {
                    string temp = keyword;
                    predicate = predicate.Or(p => p.pollContentTbls.Any(x => x.contentType == 0 && x.value.Contains(keyword)));
                }

                us = us.Where(predicate);
            }

            if (orderType == "a")
            {
                switch (sortby)
                {
                    case "pub":
                        us = us.OrderBy(c => c.startDate);
                        break;
                    case "reg":
                        us = us.OrderBy(c => c.regDate);
                        break;
                    case "viewcount":
                        us = us.OrderBy(c => c.viewCount);
                        break;
                    case "usercount":
                        us = us.OrderBy(c => c.optionTbls.Sum(x => x.mUserOptionTbls.Count));
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (sortby)
                {
                    case "pub":
                        us = us.OrderByDescending(c => c.startDate);
                        break;
                    case "reg":
                        us = us.OrderByDescending(c => c.regDate);
                        break;
                    case "viewcount":
                        us = us.OrderByDescending(c => c.viewCount);
                        break;
                    case "usercount":
                        us = us.OrderByDescending(c => c.optionTbls.Sum(x => x.mUserOptionTbls.Count));
                        break;
                    default:
                        break;
                }
            }

            var acList = us.ToList();

            var count = acList.Count();
            result.totalCount = count;
            acList = acList.Skip(skipCount).Take(pageSize).ToList();

            #endregion

            result.poll = new List<Model.PollPanel>();
            foreach (var item in acList)
            {
                var tmp = new ClassCollection.Model.PollPanel();
                tmp.startDate = Persia.Calendar.ConvertToPersian(item.startDate).ToString("m");
                tmp.ID = item.ID;
                tmp.text = item.pollContentTbls.Where(c => c.contentType == 0).Take(1).Single().value;
                tmp.text = tmp.text.Replace("<p>", "").Replace("</p>", "").Replace("<br />", "");
                tmp.text = tmp.text.Substring(0, Math.Min(40, tmp.text.Length));
                tmp.isBlock = item.isBlock;
                tmp.regDate = Persia.Calendar.ConvertToPersian(item.regDate).ToString("m") + " - " + Persia.Calendar.ConvertToPersian(item.regDate).ToString("H");
                result.poll.Add(tmp);
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
        public static string deletePoll(long pollID)
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

            if (!permission.poll.access.delete)
            {
                result.code = 2;
                result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }
            if (db.pollTbls.Any(c => c.ID == pollID) == false)
            {
                result.code = 2;
                result.message = ClassCollection.Message.POLL_NOT_EXIST;
                return new JavaScriptSerializer().Serialize(result);
            }

            var polli = db.pollTbls.Single(c => c.ID == pollID);
            db.pollTagTbls.DeleteAllOnSubmit(polli.pollTagTbls);
            db.SubmitChanges();

            var contents = db.pollContentTbls.Where(c => c.pollID == polli.ID);

            foreach (var item in contents)
            {

                if (item.contentType == 1)//image
                {
                    var image = item.value;
                    if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getPollImagesPath() + image)))
                    {
                        try
                        {
                            System.IO.File.Delete(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getPollImagesPath() + image));
                        }
                        catch
                        {
                            result.code = 3;
                            result.message = ClassCollection.Message.ERROR_DELETE_IMAGE;
                            return new JavaScriptSerializer().Serialize(result);

                        }
                    }


                }
            }

            db.pollSubGroupTbls.DeleteAllOnSubmit(db.pollSubGroupTbls.Where(c => c.pollID == pollID));
            db.pollContentTbls.DeleteAllOnSubmit(contents);
            db.SubmitChanges();

            var options = polli.optionTbls;
            foreach (var op in options)
            {
                var muserOptions = op.mUserOptionTbls;
                db.mUserOptionTbls.DeleteAllOnSubmit(muserOptions);
                db.SubmitChanges();
            }

            db.optionTbls.DeleteAllOnSubmit(options);
            db.SubmitChanges();

            db.pollTbls.DeleteOnSubmit(polli);
            db.SubmitChanges();

            result.code = 0;
            result.message = ClassCollection.Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
        }


        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string deleteAllPoll()
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

            if (!permission.poll.access.delete || !permission.poll.access.showlist)
            {
                result.code = 2;
                result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }

            var polls = db.pollTbls;
            foreach (var item in polls)
            {
                try
                {
                    deletePoll(item.ID);
                }
                catch { }
            }

            result.code = 0;
            result.message = ClassCollection.Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
        }


        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string getAllGroup()
        {
            ClassCollection.Model.GroupListResult result = new ClassCollection.Model.GroupListResult();
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

            var groups = db.GroupTbls.ToList();

            result.groups = new List<ClassCollection.Model.Group>();
            foreach (var item in groups)
            {
                var tmp = new ClassCollection.Model.Group();
                tmp.ID = item.ID;
                tmp.regDate = Persia.Calendar.ConvertToPersian(item.regDate).ToString();
                tmp.title = item.title;
                result.groups.Add(tmp);
            }

            result.result.code = 0;
            result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
        }


        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string getAllSubGroup(int GroupID)
        {
            ClassCollection.Model.subGroupListResult result = new ClassCollection.Model.subGroupListResult();
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


            if (db.GroupTbls.Any(c => c.ID == GroupID) == false)
            {
                result.result.code = 3;
                result.result.message = ClassCollection.Message.GROUP_NOT_EXIST;
                return new JavaScriptSerializer().Serialize(result);
            }

            var subgroups = db.GroupTbls.Single(c => c.ID == GroupID).SubGroupTbls;

            result.subGroup = new List<ClassCollection.Model.SubGroup>();
            foreach (var item in subgroups)
            {
                var tmp = new ClassCollection.Model.SubGroup();
                tmp.canPush = item.canPush;
                tmp.ID = item.ID;
                tmp.title = item.title;
                tmp.toPartner = item.toPartner;
                result.subGroup.Add(tmp);
            }

            result.result.code = 0;
            result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
        }
    }
}

