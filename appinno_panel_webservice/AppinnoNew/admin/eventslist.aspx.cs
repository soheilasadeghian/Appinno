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
    public partial class eventslist : System.Web.UI.Page
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
        public static string getAllevents(string regDateFrom, string regDateTo
            , string toDateFrom, string toDateTo,string fromDateFrom,string fromDateTo, string eventsstate
            , string fillterString, long subgroupID, string orderType, 
            string sortby, int pageIndex)
        {
            ClassCollection.Model.eventsAdminResult result = new ClassCollection.Model.eventsAdminResult();
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

            if (!permission.events.access.showlist)
            {
                result.result.code = 2;
                result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }

            #region   regDate

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

            #region to Date
            var toDatefromD = new DateTime();
            var toDatetoD = new DateTime();
            if (toDateFrom != "")
            {
                try
                {
                    var tt = toDateFrom.Split('/');
                    toDatefromD = Persia.Calendar.ConvertToGregorian(int.Parse(tt[0]), int.Parse(tt[1]), int.Parse(tt[2]), Persia.DateType.Persian);
                }
                catch
                {
                    result.result.code = 3;
                    result.result.message = ClassCollection.Message.DATE_INCORRECT;
                    return new JavaScriptSerializer().Serialize(result);
                }
            }
            if (toDateTo != "")
            {
                try
                {
                    var tt = toDateTo.Split('/');
                    toDatetoD = Persia.Calendar.ConvertToGregorian(int.Parse(tt[0]), int.Parse(tt[1]), int.Parse(tt[2]), Persia.DateType.Persian);
                }
                catch
                {
                    result.result.code = 3;
                    result.result.message = ClassCollection.Message.DATE_INCORRECT;
                    return new JavaScriptSerializer().Serialize(result);
                }
            }
            if (toDateTo != "" && toDateFrom != "")
            {
                if (toDatefromD.CompareTo(toDatetoD) > 0)
                {
                    result.result.code = 3;
                    result.result.message = ClassCollection.Message.DATE_INCORRECT;
                    return new JavaScriptSerializer().Serialize(result);
                }
            }
            #endregion

            #region from Date
            var fromDatefromD = new DateTime();
            var fromDatetoD = new DateTime();
            if (fromDateFrom != "")
            {
                try
                {
                    var tt = fromDateFrom.Split('/');
                    fromDatefromD = Persia.Calendar.ConvertToGregorian(int.Parse(tt[0]), int.Parse(tt[1]), int.Parse(tt[2]), Persia.DateType.Persian);
                }
                catch
                {
                    result.result.code = 3;
                    result.result.message = ClassCollection.Message.DATE_INCORRECT;
                    return new JavaScriptSerializer().Serialize(result);
                }
            }
            if (fromDateTo != "")
            {
                try
                {
                    var tt = fromDateTo.Split('/');
                    fromDatetoD = Persia.Calendar.ConvertToGregorian(int.Parse(tt[0]), int.Parse(tt[1]), int.Parse(tt[2]), Persia.DateType.Persian);
                }
                catch
                {
                    result.result.code = 3;
                    result.result.message = ClassCollection.Message.DATE_INCORRECT;
                    return new JavaScriptSerializer().Serialize(result);
                }
            }
            if (fromDateTo != "" && fromDateFrom != "")
            {
                if (fromDatefromD.CompareTo(fromDatetoD) > 0)
                {
                    result.result.code = 3;
                    result.result.message = ClassCollection.Message.DATE_INCORRECT;
                    return new JavaScriptSerializer().Serialize(result);
                }
            }
            #endregion

            var us = db.eventsTbls.Where(c => c.eventsContentTbls.Any()).AsQueryable();

            if (regDateTo != "")
            {
                us = us.Where(c => c.regDate <= regDatetoD);
            }
            if (regDateFrom != "")
            {
                us = us.Where(c => c.regDate >= regDatefromD);
            }
            if (toDateTo != "")
            {
                us = us.Where(c => c.toDate <= toDatetoD);
            }
            if (toDateFrom != "")
            {
                us = us.Where(c => c.toDate >= toDatefromD);
            }
            if (fromDateTo != "")
            {
                us = us.Where(c => c.fromDate <= fromDatetoD);
            }
            if (fromDateFrom != "")
            {
                us = us.Where(c => c.fromDate >= fromDatefromD);
            }
            if (subgroupID == -1)
            {
                us = us.Where(c => c.eventsSubGroupTbls.Any() == false);
            }
            else if (subgroupID == -2)
            {
                us = us.Where(c => c.eventsSubGroupTbls.Any());
            }
            else if (subgroupID > 0)
            {
                if (db.SubGroupTbls.Any(c => c.ID == subgroupID) == false)
                {
                    result.result.code = 3;
                    result.result.message = ClassCollection.Message.SUBGROUP_NOT_EXIST;
                    return new JavaScriptSerializer().Serialize(result);
                }
                us = us.Where(c => c.eventsSubGroupTbls.Any(p => p.subGroupID == subgroupID));
            }

            if (eventsstate == "block")
            {
                us = us.Where(c => c.isBlock == true);
            }
            else if (eventsstate == "unblock")
            {
                us = us.Where(c => c.isBlock == false);
            }

            #region search

            if (fillterString != "")
            {
                var search = new Search(fillterString);
                var ors = search.getOR;

                var predicate = PredicateBuilder.False<eventsTbl>();
                foreach (string keyword in ors)
                {
                    string temp = keyword;
                    predicate = predicate.Or(p => p.title.Contains(keyword) || p.eventsContentTbls.Any(c=>c.contentType==0 && c.value.Contains(keyword)));
                }

                us = us.Where(predicate);
            }

            if (orderType == "a")
            {
                switch (sortby)
                {
                    case "to":
                        us = us.OrderBy(c => c.toDate);
                        break;
                    case "from":
                        us = us.OrderBy(c => c.fromDate);
                        break;
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
                    case "to":
                        us = us.OrderByDescending(c => c.toDate);
                        break;
                    case "from":
                        us = us.OrderByDescending(c => c.fromDate);
                        break;
                    case "reg":
                        us = us.OrderByDescending(c => c.regDate);
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

            result.events = new List<Model.eventsAdmin>();
            foreach (var item in acList)
            {
                var tmp = new ClassCollection.Model.eventsAdmin();
                tmp.toDate = Persia.Calendar.ConvertToPersian(item.toDate).ToString("m") ;
                tmp.fromDate = Persia.Calendar.ConvertToPersian(item.fromDate).ToString("m");
                tmp.ID = item.ID;
                tmp.title = item.title;
                tmp.isBlock = item.isBlock;
                tmp.regDate = Persia.Calendar.ConvertToPersian(item.regDate).ToString("m") + " - " + Persia.Calendar.ConvertToPersian(item.regDate).ToString("H");
                result.events.Add(tmp);

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
        public static string deleteevents(long eventsID)
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

            if (!permission.events.access.delete)
            {
                result.code = 2;
                result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }
            if (db.eventsTbls.Any(c => c.ID == eventsID) == false)
            {
                result.code = 2;
                result.message = ClassCollection.Message.EVENTS_NOT_EXIST;
                return new JavaScriptSerializer().Serialize(result);
            }

            var events = db.eventsTbls.Single(c => c.ID == eventsID);

            db.eventTagTbls.DeleteAllOnSubmit(events.eventTagTbls);
            db.SubmitChanges();

            var contents = db.eventsContentTbls.Where(c => c.eventsID == events.ID);

            foreach (var item in contents)
            {

                if (item.contentType == 1)//image
                {
                    var image = item.value;
                    if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(ClassCollection.Methods.geteventsImagesPath() + image)))
                    {
                        try
                        {
                            System.IO.File.Delete(HttpContext.Current.Server.MapPath(ClassCollection.Methods.geteventsImagesPath() + image));
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
                    if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(ClassCollection.Methods.geteventsVideosPath() + video)))
                    {
                        try
                        {
                            System.IO.File.Delete(HttpContext.Current.Server.MapPath(ClassCollection.Methods.geteventsVideosPath() + video));
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

            db.eventsSubGroupTbls.DeleteAllOnSubmit(db.eventsSubGroupTbls.Where(c => c.eventsID == eventsID));
            db.eventsContentTbls.DeleteAllOnSubmit(contents);
            db.SubmitChanges();

            db.eventsLikeTbls.DeleteAllOnSubmit(db.eventsLikeTbls.Where(c => c.eventsID == events.ID));
            db.SubmitChanges();

            db.eventsTbls.DeleteOnSubmit(events);
            db.SubmitChanges();

            result.code = 0;
            result.message = ClassCollection.Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
        }


        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string deleteAllevents()
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

            if (!permission.events.access.delete || !permission.events.access.showlist)
            {
                result.code = 2;
                result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }

            var events = db.eventsTbls;
            foreach (var item in events)
            {
                try
                {
                    deleteevents(item.ID);
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

