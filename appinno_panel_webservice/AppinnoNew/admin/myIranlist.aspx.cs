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
    public partial class myIranlist : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var db = new DataAccessDataContext();
            if (ClassCollection.Methods.isLogin(Context))
            {
                var user = ClassCollection.Methods.getUserInSession(HttpContext.Current);
                var permission = ClassCollection.Accessing.Deserialize(db.accessTbls.Single(c => c.ID == user.accessID).per);

                if (!permission.myIranCompetition.access.showlist)
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
        public static string getAllmyIran(string regDateFrom, string regDateTo
            , string startDateFrom, string startDateTo
            , string endDateFrom, string endDateTo
            , string Competitionstate
            , string Competitionstatus
            , string fillterString, string orderType
            , string sortby, int pageIndex)
        {
            ClassCollection.Model.MyIranAdminResult result = new ClassCollection.Model.MyIranAdminResult();
            result.result = new ClassCollection.Model.Result();
            const int pageSize = 30;

            var dt = new DateTime();
            dt = DateTime.Now;

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

            if (!permission.myIranCompetition.access.showlist)
            {
                result.result.code = 2;
                result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }

            #region check regDate

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

            #region check startDate

            var startDatefromD = new DateTime();
            var startDatetoD = new DateTime();
            if (startDateFrom != "")
            {
                try
                {
                    var tt = startDateFrom.Split('/');
                    startDatefromD = Persia.Calendar.ConvertToGregorian(int.Parse(tt[0]), int.Parse(tt[1]), int.Parse(tt[2]), Persia.DateType.Persian);
                }
                catch
                {
                    result.result.code = 3;
                    result.result.message = ClassCollection.Message.DATE_INCORRECT;
                    return new JavaScriptSerializer().Serialize(result);
                }
            }
            if (startDateTo != "")
            {
                try
                {
                    var tt = startDateTo.Split('/');
                    startDatetoD = Persia.Calendar.ConvertToGregorian(int.Parse(tt[0]), int.Parse(tt[1]), int.Parse(tt[2]), Persia.DateType.Persian);
                }
                catch
                {
                    result.result.code = 3;
                    result.result.message = ClassCollection.Message.DATE_INCORRECT;
                    return new JavaScriptSerializer().Serialize(result);
                }
            }
            if (startDateTo != "" && startDateFrom != "")
            {
                if (startDatefromD.CompareTo(startDatetoD) > 0)
                {
                    result.result.code = 3;
                    result.result.message = ClassCollection.Message.DATE_INCORRECT;
                    return new JavaScriptSerializer().Serialize(result);
                }
            }
            #endregion

            #region check endDate

            var endDatefromD = new DateTime();
            var endDatetoD = new DateTime();
            if (endDateFrom != "")
            {
                try
                {
                    var tt = endDateFrom.Split('/');
                    endDatefromD = Persia.Calendar.ConvertToGregorian(int.Parse(tt[0]), int.Parse(tt[1]), int.Parse(tt[2]), Persia.DateType.Persian);
                }
                catch
                {
                    result.result.code = 3;
                    result.result.message = ClassCollection.Message.DATE_INCORRECT;
                    return new JavaScriptSerializer().Serialize(result);
                }
            }
            if (endDateTo != "")
            {
                try
                {
                    var tt = endDateTo.Split('/');
                    endDatetoD = Persia.Calendar.ConvertToGregorian(int.Parse(tt[0]), int.Parse(tt[1]), int.Parse(tt[2]), Persia.DateType.Persian);
                }
                catch
                {
                    result.result.code = 3;
                    result.result.message = ClassCollection.Message.DATE_INCORRECT;
                    return new JavaScriptSerializer().Serialize(result);
                }
            }
            if (endDateTo != "" && endDateFrom != "")
            {
                if (endDatefromD.CompareTo(endDatetoD) > 0)
                {
                    result.result.code = 3;
                    result.result.message = ClassCollection.Message.DATE_INCORRECT;
                    return new JavaScriptSerializer().Serialize(result);
                }
            }
            #endregion
            
            var us = db.myIranTbls.Where(c => c.myIranContentTbls.Any()).AsQueryable();

            if (Competitionstatus == "ثبت شده")
            {
                us = us.Where(c => c.startDate > dt);
            }
            if (Competitionstatus == "درحال برگزاری")
            {
                us = us.Where(c => c.startDate < dt && c.endDate > dt);
            }
            if (Competitionstatus == "درحال بررسی نتایج")
            {
                us = us.Where(c => c.endDate < dt && c.isDone == false);
            }
            if (Competitionstatus == "پایان یافته")
            {
                us = us.Where(c => c.endDate < dt && c.isDone == true);
            }


            if (regDateTo != "")
            {
                us = us.Where(c => c.regDate <= regDatetoD);
            }
            if (regDateFrom != "")
            {
                us = us.Where(c => c.regDate >= regDatefromD);
            }
            if (startDateTo != "")
            {
                us = us.Where(c => c.startDate <= startDatetoD);
            }
            if (startDateFrom != "")
            {
                us = us.Where(c => c.startDate >= startDatefromD);
            }
            if (endDateTo != "")
            {
                us = us.Where(c => c.endDate <= endDatetoD);
            }
            if (endDateFrom != "")
            {
                us = us.Where(c => c.endDate >= endDatefromD);
            }
           

            if (Competitionstate == "block")
            {
                us = us.Where(c => c.isBlock == true);
            }
            else if (Competitionstate == "unblock")
            {
                us = us.Where(c => c.isBlock == false);
            }


            if (Competitionstate == "block")
            {
                us = us.Where(c => c.isBlock == true);
            }
            else if (Competitionstate == "unblock")
            {
                us = us.Where(c => c.isBlock == false);
            }

            #region search

            if (fillterString != "")
            {
                var search = new Search(fillterString);
                var ors = search.getOR;

                var predicate = PredicateBuilder.False<myIranTbl>();
                foreach (string keyword in ors)
                {
                    string temp = keyword;
                    predicate = predicate.Or(p => p.title.Contains(keyword) || p.myIranContentTbls.Any(c=>c.contentType==0 && c.value.Contains(keyword)));
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
                    case "startDate":
                        us = us.OrderBy(c => c.startDate);
                        break;
                    case "endDate":
                        us = us.OrderBy(c => c.endDate);
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
                    case "startDate":
                        us = us.OrderByDescending(c => c.startDate);
                        break;
                    case "endDate":
                        us = us.OrderByDescending(c => c.endDate);
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

            result.myIran = new List<Model.MyIranAdmin>();
            foreach (var item in acList)
            {
                var status = "";
                if (dt < item.startDate) status = "ثبت شده";
                if (item.startDate < dt && dt < item.endDate) status = "درحال برگزاری";
                if (item.endDate < dt && item.isDone == false) status = "درحال بررسی نتایج";
                if (item.endDate < dt && item.isDone == true) status = "پایان مسابقه";

                var tmp = new ClassCollection.Model.MyIranAdmin();
                tmp.ID = item.ID;
                tmp.title = item.title;
                tmp.status = status;
                tmp.isBlock = item.isBlock;
                tmp.regDate = Persia.Calendar.ConvertToPersian(item.regDate).ToString("m") + " - " + Persia.Calendar.ConvertToPersian(item.regDate).ToString("H");
                tmp.startDate = Persia.Calendar.ConvertToPersian(item.startDate).ToString("m") + " - " + Persia.Calendar.ConvertToPersian(item.startDate).ToString("H");
                tmp.endDate = Persia.Calendar.ConvertToPersian(item.endDate).ToString("m") + " - " + Persia.Calendar.ConvertToPersian(item.endDate).ToString("H");
                tmp.isSendNotification = item.isSendNotification;
                result.myIran.Add(tmp);
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
        public static string SendMyIranNotification(long myIranID, string text)
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

            if (!permission.myIranCompetition.access.edit)
            {
                result.code = 2;
                result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }
            if (db.myIranTbls.Any(c => c.ID == myIranID) == false)
            {
                result.code = 2;
                result.message = ClassCollection.Message.COMPETITIONS_NOT_EXIST;
                return new JavaScriptSerializer().Serialize(result);
            }
            
            var myIran = db.myIranTbls.Single(c => c.ID == myIranID);
            myIran.isSendNotification = true;
            myIran.notificationText = text;
            db.SubmitChanges();
            new service.pushservice().XsendPush("1", "MyIranStartNot", myIran.ID + "", myIran.notificationText, "toall", "false");
            result.code = 0;
            result.message = ClassCollection.Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);

        }


        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string BlockUnblockMyIran(long myIranID, bool isBlock)
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

            if (!permission.myIranCompetition.access.delete)
            {
                result.code = 2;
                result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }
            if (db.myIranTbls.Any(c => c.ID == myIranID) == false)
            {
                result.code = 2;
                result.message = ClassCollection.Message.COMPETITIONS_NOT_EXIST;
                return new JavaScriptSerializer().Serialize(result);
            }

            var myIran = db.myIranTbls.Single(c => c.ID == myIranID);
            if (isBlock == true)
            {
                myIran.isBlock = true;
            }
            else
            {
                myIran.isBlock = false;
            }

            db.SubmitChanges();

            result.code = 0;
            result.message = ClassCollection.Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);

        }
    }
}

