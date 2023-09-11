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
   
    public partial class dashboard : System.Web.UI.Page
    {
        public class dates
        {
            public DateTime fromdatemiladi { get; set; }
            public DateTime todatemiladi { get; set; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string getaccess()
        {
            ClassCollection.Model.AccessingResult result = new ClassCollection.Model.AccessingResult();
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

            result.access = new ClassCollection.Accessing();

            result.access = permission;
            
            result.access.relation=
            //مدیر کل سامانه
            (user.accessID == 1) ||
            //مدیر کل سامانه(همراه اول
            (user.accessID == 52);

            result.result.code = 0;
            result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
        }

        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string getUserCount(string date)
        {
            ClassCollection.Model.NumberResult result = new ClassCollection.Model.NumberResult();
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

            #region  Date

            var dateD = new DateTime();
            if (date == "")
            {

                date = Persia.Calendar.ConvertToPersian(DateTime.Now).ToString();
            }
            if (date != "")
            {
                try
                {
                    var tt = date.Split('/');
                    dateD = Persia.Calendar.ConvertToGregorian(int.Parse(tt[0]), int.Parse(tt[1]), int.Parse(tt[2]), Persia.DateType.Persian);
                }
                catch
                {
                    result.result.code = 3;
                    result.result.message = ClassCollection.Message.DATE_INCORRECT;
                    return new JavaScriptSerializer().Serialize(result);
                }
            }

            #endregion
            result.number = db.mUserTbls.Where(c => c.regDate.Date.Equals(dateD.Date)).Count();
            result.date = date;
            result.result.code = 0;
            result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
        }


        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string getUngroupUserCount()
        {
            ClassCollection.Model.NumberResult result = new ClassCollection.Model.NumberResult();
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

            #region  Date

            var date = Persia.Calendar.ConvertToPersian(DateTime.Now).ToString();


            #endregion
            result.number = db.mUserTbls.Where(c => c.userSubGroupTbls.Any() == false).Count();
            result.date = date;
            result.result.code = 0;
            result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
        }
        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string getPartnerCount()
        {
            ClassCollection.Model.NumberResult result = new ClassCollection.Model.NumberResult();
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

            #region  Date

            var date = Persia.Calendar.ConvertToPersian(DateTime.Now).ToString();


            #endregion

            result.number = db.mUserTbls.Where(c => c.userSubGroupTbls.Any() == true).Count();
            result.date = date;
            result.result.code = 0;
            result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
        }

        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string getUserList(string date)
        {
            ClassCollection.Model.dashboardUserResult result = new ClassCollection.Model.dashboardUserResult();
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


            if (permission.user.access.showlist == false)
            {
                result.result.code = 2;
                result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }
            #region  Date

            var dateD = new DateTime();
            if (date == "")
            {

                date = Persia.Calendar.ConvertToPersian(DateTime.Now).ToString();
            }
            if (date != "")
            {
                try
                {
                    var tt = date.Split('/');
                    dateD = Persia.Calendar.ConvertToGregorian(int.Parse(tt[0]), int.Parse(tt[1]), int.Parse(tt[2]), Persia.DateType.Persian);
                }
                catch
                {
                    result.result.code = 3;
                    result.result.message = ClassCollection.Message.DATE_INCORRECT;
                    return new JavaScriptSerializer().Serialize(result);
                }
            }

            #endregion
            result.user = new List<ClassCollection.Model.dashboardUser>();

            var users = db.mUserTbls.Where(c => c.regDate.Date.Equals(dateD.Date)).OrderByDescending(p => p.ID).GroupBy(c => c.userSubGroupTbls.Any());
            foreach (var item in users)
            {
                foreach (var ini in item)
                {
                    var tmp = new ClassCollection.Model.dashboardUser();
                    tmp.info = ini.name + " " + ini.family;
                    tmp.tell = ini.emailtell;
                    tmp.isPartner = ini.userSubGroupTbls.Any();
                    result.user.Add(tmp);
                }

            }

            result.date = date;
            result.result.code = 0;
            result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
        }
        
        public static dates getdates(string date)
        {
            string[] words = date.Split(' ');
            string month = words[0];
            string year = words[1];

            string monthnumber;
            switch (month)
            {
                case "فروردین":
                    monthnumber = "01";
                    break;
                case "اردیبهشت":
                    monthnumber = "02";
                    break;
                case "خرداد":
                    monthnumber = "03";
                    break;
                case "تیر":
                    monthnumber = "04";
                    break;
                case "مرداد":
                    monthnumber = "05";
                    break;
                case "شهریور":
                    monthnumber = "06";
                    break;
                case "مهر":
                    monthnumber = "07";
                    break;
                case "آبان":
                    monthnumber = "08";
                    break;
                case "آذر":
                    monthnumber = "09";
                    break;
                case "دی":
                    monthnumber = "10";
                    break;
                case "بهمن":
                    monthnumber = "11";
                    break;
                case "اسفند":
                    monthnumber = "12";
                    break;
                default:
                    monthnumber = "0";
                    break;
            }

            var fromdatemiladi = Persia.Calendar.ConvertToGregorian(int.Parse(year), int.Parse(monthnumber), int.Parse("01"), Persia.DateType.Persian);
            var todatemiladi = Persia.Calendar.ConvertToGregorian(int.Parse(year), int.Parse((int.Parse(monthnumber) + 1).ToString()), int.Parse("01"), Persia.DateType.Persian);

            var da = new dates();
            da.fromdatemiladi = fromdatemiladi;
            da.todatemiladi = todatemiladi;
            return da;
        }

        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string getCount(string date,string content)
        {
            ClassCollection.Model.NumberResult result = new ClassCollection.Model.NumberResult();
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

            var fromdatemiladi = getdates(date).fromdatemiladi;
            var todatemiladi = getdates(date).todatemiladi;

            int count;
            switch (content)
            {
                case "news":
                    count = db.newsTbls.Where(x => x.regDate >= fromdatemiladi && x.regDate <= todatemiladi).Count();
                    break;
                case "event":
                    count = db.eventsTbls.Where(x => x.regDate >= fromdatemiladi && x.regDate <= todatemiladi).Count();
                    break;
                case "io":
                    count = db.ioTbls.Where(x => x.regDate >= fromdatemiladi && x.regDate <= todatemiladi).Count();
                    break;
                case "pub":
                    count = db.publicationTbls.Where(x => x.regDate >= fromdatemiladi && x.regDate <= todatemiladi).Count();
                    break;
                case "bestidea":
                    count = db.bestIdeaCompetitionsTbls.Where(x => x.regDate >= fromdatemiladi && x.regDate <= todatemiladi).Count();
                    break;
                case "creativity":
                    count = db.creativityCompetitionTbls.Where(x => x.regDate >= fromdatemiladi && x.regDate <= todatemiladi).Count();
                    break;
                case "myiran":
                    count = db.myIranTbls.Where(x => x.regDate >= fromdatemiladi && x.regDate <= todatemiladi).Count();
                    break;
                default:
                    count = 0;
                    break;
            }
            
            result.number = count;
            result.result.code = 0;
            result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
        }
        
       
    }
}