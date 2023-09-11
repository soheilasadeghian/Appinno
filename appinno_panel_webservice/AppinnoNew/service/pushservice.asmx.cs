using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;

namespace AppinnoNew.service
{
    /// <summary>
    /// این سرویس وظیفه مدیریت عملیات های پوش را بر عهده دارد
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class pushservice : System.Web.Services.WebService
    { 
        //soheila-start-poll
        /// <summary>
        /// این متد وظیفه ارسال پوش به کاربر مورد نظر را برعهده دارد
        /// </summary>
        /// <param name="key">کلمه عبور برنامه ای که در حال انجام عملیات است</param>
        /// <param name="ticker">عنوان اولیه پیام</param>
        /// <param name="title">عنوان اصلی پیام</param>
        /// <param name="text">متن پیام</param>
        /// <param name="token">کاربر مورد نظر جهت ارسال پوش</param>
        /// <returns>
        /// خروجی ساختار جی سون از کلاس زیر را دارد.
        /// <para><see cref="ClassCollection.ServiceModels.Result"/></para>
        /// <remarks>
        /// <para>code : -1 , message : <see cref="ClassCollection.Messages.OPERATION_NO_ACCESS"/></para>
        /// <para>Result.code : 0 , Result.message : <see cref="ClassCollection.Messages.OPERATION_SUCCESS"/> </para>
        /// <para>Result.code : 1 , Result.message : <see cref="ClassCollection.Messages.PUSH_FAILED"/> </para>
        /// </remarks>
        /// </returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void sendPushTo(string key, string ticker, string title, string text, string url, string token)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.Result Result = new ClassCollection.Model.Result();

            var userkey = ConfigurationManager.AppSettings["UserKey"];
            if (key != userkey)
            {
                Result.code = -1;
                Result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }
            if (url == "")
                url = "-";
            string postData = "{ \"registration_ids\": [ \"" + token + "\" ], " +
              "\"data\": {\"tickerText\":\"" + ticker + "\", " +
                         "\"contentTitle\":\"" + title + "\", " +
                         "\"url\":\"" + url + "\", " +
                         "\"manual\":\"" + "true" + "\", " +
                         "\"message\": \"" + text + "\"}}";

            string response = SendGCMNotification(ConfigurationManager.AppSettings["GCMAPIKEY"], postData);
            if (response != "0")
            {
                Result.code = 1;
                Result.message = ClassCollection.Message.PUSH_FAILED;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            Result.code = 0;
            Result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }
        
        public void XsendPushTo(string key, string ticker, string title, string text, string url, string token)
        {
            if (url == "")
                url = "-";
            string postData = "{ \"registration_ids\": [ \"" + token + "\" ], " +
              "\"data\": {\"tickerText\":\"" + ticker + "\", " +
                         "\"contentTitle\":\"" + title + "\", " +
                         "\"url\":\"" + url + "\", " +
                         "\"manual\":\"" + "true" + "\", " +
                         "\"message\": \"" + text + "\"}}";

            string response = SendGCMNotification(ConfigurationManager.AppSettings["GCMAPIKEY"], postData);
        }

        //soheila-end-poll
        private void sendPushIOS(string key, string ticker, string title, string text, string to)
        {
            //config

            new Thread(() =>
            {
                try
                {
                    String notification = "{\"sound\":\"default\",\"title\":\"" + title + "\",\"body\":\"" + text + "\"}"; // put the message you want to send here
                    String messageToSend = "{\"to\":\"" + to + "\",\"notification\":" + notification + " ,\"priority\": \"high\" }"; // Construct the message.

                    SendGCMNotification(ClassCollection.Methods.getGCMAPIKEY(), messageToSend);
                }
                catch
                {

                }
            }).Start();
        }


        /// <summary>
        /// این متد  وظیفه بروز رسانی یا درج مشخصات دستگاه مورد نظر برای ارسال پوش نوتیفیکیشن را بر عهده دارد
        /// </summary>

        /// <param name="key">کلمه عبور برنامه ای که درحال استفاده از متد جاری می باشد</param>
        /// <param name="latitude">پارامتر اول مختصات دستگاهی که درحال درج یا بروز رسانی آن هستیم</param>
        /// <param name="longitude">پارامتر اول مختصات دستگاهی که درحال درج یا بروز رسانی آن هستیم</param>
        /// <param name="token">توکن سرویس گوگل جهت پوش</param> 
        /// <param name="deviceID">کد دستگاهی که درحال درج یا بروز رسانی آن هستیم</param>
        /// <param name="userID">کد کاربری که درحال درج یا بروز رسانی مشخصات پوش آن هستیم، این مقدار میتواند نال باشد</param>
        /// <returns>
        /// خروجی ساختار جی سون از کلاس زیر را دارد.
        /// <para><see cref="ClassCollection.ServiceModels.Result"/></para>
        /// <remarks>
        /// <para>code : -1 , message : <see cref="ClassCollection.Messages.OPERATION_NO_ACCESS"/></para>
        /// <para>Result.code : 0 , Result.message : <see cref="ClassCollection.Messages.OPERATION_SUCCESS"/> </para>
        /// <para>Result.code : 1 , Result.message : <see cref="ClassCollection.Messages.INPUT_ERROR_CUSTOMER_LATITUD_LONGITUD_EMPTY"/> </para>
        /// <para>Result.code : 2 , Result.message : <see cref="ClassCollection.Messages.TOKEN_ERROR_NOT_EXIST"/> </para>
        /// <para>Result.code : 3 , Result.message : <see cref="ClassCollection.Messages.DEVICE_ID_INCORRECT"/> </para>
        /// </remarks>
        /// </returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void setPushInfo(string key, string token, string deviceID, long userID, string latitude, string longitude)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.Result Result = new ClassCollection.Model.Result();
            var dt = new DateTime();
            dt = DateTime.Now;

            var userkey = ConfigurationManager.AppSettings["UserKey"];
            if (key != userkey)
            {
                Result.code = -1;
                Result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            var db = new DataAccessDataContext();
            double lng = 0, lat = 0;

            if (!double.TryParse(longitude, out lng) || !double.TryParse(latitude, out lat))
            {
                Result.code = 1;
                Result.message = ClassCollection.Message.INPUT_ERROR_CUSTOMER_LATITUD_LONGITUD_EMPTY;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            deviceID = deviceID.Trim();

            if (token.Length == 0)
            {
                Result.code = 2;
                Result.message = ClassCollection.Message.TOKEN_ERROR_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }
            if (deviceID.Length == 0)
            {
                Result.code = 3;
                Result.message = ClassCollection.Message.DEVICE_ID_INCORRECT;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }
            if (userID != -1)
            {
                if (db.mUserTbls.Any(c => c.ID == userID) == false)
                {
                    Result.code = 4;
                    Result.message = ClassCollection.Message.USER_NOT_EXIST;
                    Context.Response.Write(js.Serialize(Result).ToLower());
                    return;
                }
            }
            if (db.pushUserTbls.Any(c => c.deviceID == deviceID))
            {
                var rec = db.pushUserTbls.Single(c => c.deviceID == deviceID);
                rec.lastUpdateDate = dt;
                rec.latitude = lat;
                rec.longitude = lng;
                rec.token = token;
                rec.mUsreID = userID;
                db.SubmitChanges();
            }
            else
            {
                var rec = new pushUserTbl();
                rec.lastUpdateDate = dt;
                rec.latitude = lat;
                rec.longitude = lng;
                rec.registerDate = dt;
                rec.token = token;
                rec.mUsreID = userID;
                rec.deviceID = deviceID;
                db.pushUserTbls.InsertOnSubmit(rec);
                db.SubmitChanges();
            }

            Result.code = 0;
            Result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }

        /// <summary>
        /// این متد وظیفه ارسال پوش به کاربران مورد نظر را برعهده دارد
        /// </summary>
        /// <param name="key">کلمه عبور برنامه ای که در حال انجام عملیات است</param>
        /// <param name="ticker">عنوان اولیه پیام</param>
        /// <param name="title">عنوان اصلی پیام</param>
        /// <param name="text">متن پیام</param>
        /// <param name="to">گیرندگان پیام
        /// <remarks>
        /// نحوه ارسال این پارامتر به شکل های زیر می باشد
        /// <para><c>all</c> : به تمام کاربرانی که برنامه را نصب کرده اند ارسال میشود</para>
        /// <para><c>closeTo>r:#,lat:#####,lng:######</c>  به کاربرانی که مکان آن ها در شعاع مختصات وارد شده باشد ارسال می شود</para>
        /// </remarks>
        /// </param>
        /// <returns>
        /// خروجی ساختار جی سون از کلاس زیر را دارد.
        /// <para><see cref="ClassCollection.ServiceModels.Result"/></para>
        /// <remarks>
        /// <para>code : -1 , message : <see cref="ClassCollection.Messages.OPERATION_NO_ACCESS"/></para>
        /// <para>Result.code : 0 , Result.message : <see cref="ClassCollection.Messages.OPERATION_SUCCESS"/> </para>
        /// <para>Result.code : 1 , Result.message : <see cref="ClassCollection.Messages.PUSH_COMMAND_INCORRECT"/> </para>
        /// <para>Result.code : 2 , Result.message : <see cref="ClassCollection.Messages.PUSH_FAILED"/> </para>
        /// </remarks>
        /// </returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void sendPush(string key, string ticker, string title, string text, string to, string manual, string subgroups = null)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.Result Result = new ClassCollection.Model.Result();

            var userkey = ConfigurationManager.AppSettings["UserKey"];
            if (key != userkey)
            {
                Result.code = -1;
                Result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            string ids = "";
            var db = new DataAccessDataContext();

            int radius = -1;
            double lat = -1, lng = -1;
            to = to.Trim();

            var users = db.pushUserTbls;
            IQueryable<string> tokens = users.Where(c => c.deviceID.Contains("-") == false).Select(c => c.token);
            IQueryable<string> iosTokens = users.Where(c => c.deviceID.Contains("-") == true).Select(c => c.token);

            if (to.Contains("closeTo>"))
            {
                to = to.Replace("closeTo>", "");
                var arr = to.Split(',');
                if (arr.Count() != 3)
                {
                    Result.code = 1;
                    Result.message = ClassCollection.Message.PUSH_COMMAND_INCORRECT;
                    Context.Response.Write(js.Serialize(Result).ToLower());
                    return;
                }

                if (!double.TryParse(arr[1].Replace("lat:", ""), out lat) || !double.TryParse(arr[2].Replace("lng:", ""), out lng) || !int.TryParse(arr[0].Replace("r:", ""), out radius))
                {
                    Result.code = 1;
                    Result.message = ClassCollection.Message.PUSH_COMMAND_INCORRECT;
                    Context.Response.Write(js.Serialize(Result).ToLower());
                    return;
                }

                lat = (Math.PI / 180) * lat;
                lng = (Math.PI / 180) * lng;

                tokens = users.Where(c =>
                (6371
                * Math.Acos(Math.Cos(lat)
                * Math.Cos((Math.PI / 180) * (double)c.latitude)
                * Math.Cos(((Math.PI / 180) * (double)(c.longitude)) - lng) + Math.Sin(lat)
                * Math.Sin((Math.PI / 180) * (double)c.latitude))) <= radius && c.deviceID.Contains("-") == false).Select(c => c.token);

                iosTokens = users.Where(c =>
                (6371
                * Math.Acos(Math.Cos(lat)
                * Math.Cos((Math.PI / 180) * (double)c.latitude)
                * Math.Cos(((Math.PI / 180) * (double)(c.longitude)) - lng) + Math.Sin(lat)
                * Math.Sin((Math.PI / 180) * (double)c.latitude))) <= radius && c.deviceID.Contains("-") == true).Select(c => c.token);
            }
            else if (!to.Contains("all") && !to.Contains("partners") && !to.Contains("subgroups"))
            {
                Result.code = 1;
                Result.message = ClassCollection.Message.PUSH_COMMAND_INCORRECT;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (to.Equals("all"))
            {
                tokens = users.Where(c => c.deviceID.Contains("-") == false).Select(c => c.token);
                iosTokens = users.Where(c => c.deviceID.Contains("-") == true).Select(c => c.token);
            }
            else if (to.Equals("partners"))
            {
                tokens = users.Where(c => c.mUsreID != null).Where(c => c.deviceID.Contains("-") == false).Select(c => c.token);
                iosTokens = users.Where(c => c.mUsreID != null).Where(c => c.deviceID.Contains("-") == true).Select(c => c.token);

            }
            else if (to.Equals("subgroups"))
            {
                if (subgroups == null)
                {
                    Result.code = 3;
                    Result.message = ClassCollection.Message.PUSH_COMMAND_INCORRECT;
                    Context.Response.Write(js.Serialize(Result).ToLower());
                    return;
                }

                var subs = subgroups.Split(',').Select(c => long.Parse(c));
                var uids = new List<long>();
                foreach (var item in subs)
                {
                    uids.AddRange(db.mUserTbls.Where(c => c.userSubGroupTbls.Any(p => p.subGroupID == item)).Select(c => c.ID).ToList());
                }
                uids = uids.Distinct().ToList();

                var tks = new List<string>();
                var iostks = new List<string>();
                foreach (var item in uids)
                {
                    try
                    {
                        var rec = db.pushUserTbls.Single(c => c.mUsreID.Value == item);
                        if (rec.deviceID.Contains("-") == false)
                            tks.Add(rec.token);
                        else iostks.Add(rec.token);
                    }
                    catch { }
                }

                tokens = tks.AsQueryable();
                iosTokens = iosTokens.AsQueryable();
            }
            foreach (var item in iosTokens)
            {
                sendPushIOS(key, ticker, title, text, item);
            }
            int pageIndex = 1;
            while (true)
            {
                bool Continue = false;
                var ttoken = tokens.Skip((pageIndex - 1) * 900).Take(900);
                ids = "";
                foreach (var item in ttoken)
                {
                    Continue = true;
                    ids += "\"" + item + "\",";

                }

                if (Continue)
                {
                    var lastIndex = ids.LastIndexOf(",");
                    ids = ids.Remove(lastIndex, 1);

                    string postData = "{ \"registration_ids\": [ " + ids + " ], " +
                      "\"data\": {\"tickerText\":\"" + ticker + "\", " +
                                 "\"contentTitle\":\"" + title + "\", " +
                                 "\"manual\":\"" + manual + "\", " +
                                 "\"message\": \"" + text + "\"}}";

                    string response = SendGCMNotification(ClassCollection.Methods.getGCMAPIKEY(), postData);
                    if (response != "0")
                    {
                        Result.code = 2;
                        Result.message = ClassCollection.Message.PUSH_FAILED;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                        return;
                    }
                    pageIndex++;
                }
                else
                    break;

            }
            Result.code = 0;
            Result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public bool XsendPush(string key, string ticker, string title, string text, string to, string subgroups = null)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.Result Result = new ClassCollection.Model.Result();

            var userkey = ConfigurationManager.AppSettings["UserKey"];
            if (key != userkey)
            {
                Result.code = -1;
                Result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                return false;
            }

            string ids = "";
            var db = new DataAccessDataContext();

            int radius = -1;
            double lat = -1, lng = -1;
            to = to.Trim();

            var users = db.pushUserTbls;
            IQueryable<string> tokens = users.Select(c => c.token);

            if (to.Contains("closeTo>"))
            {
                to = to.Replace("closeTo>", "");
                var arr = to.Split(',');
                if (arr.Count() != 3)
                {
                    Result.code = 1;
                    Result.message = ClassCollection.Message.PUSH_COMMAND_INCORRECT;
                    return false;
                }

                if (!double.TryParse(arr[1].Replace("lat:", ""), out lat) || !double.TryParse(arr[2].Replace("lng:", ""), out lng) || !int.TryParse(arr[0].Replace("r:", ""), out radius))
                {
                    Result.code = 1;
                    Result.message = ClassCollection.Message.PUSH_COMMAND_INCORRECT;
                    return false;
                }

                lat = (Math.PI / 180) * lat;
                lng = (Math.PI / 180) * lng;

                tokens = users.Where(c =>
                (6371
                * Math.Acos(Math.Cos(lat)
                * Math.Cos((Math.PI / 180) * (double)c.latitude)
                * Math.Cos(((Math.PI / 180) * (double)(c.longitude)) - lng) + Math.Sin(lat)
                * Math.Sin((Math.PI / 180) * (double)c.latitude))) <= radius).Select(c => c.token);
            }
            else if (!to.Contains("toall") && !to.Contains("topartners") && !to.Contains("subgroups"))
            {
                Result.code = 1;
                Result.message = ClassCollection.Message.PUSH_COMMAND_INCORRECT;
                return false;
            }

            if (to.Equals("toall"))
            {
                tokens = users.Select(c => c.token);
            }
            else if (to.Equals("topartners"))
            {
                tokens = users.Where(c => c.mUsreID != null).Select(c => c.token);
            }
            else if (to.Equals("subgroups"))
            {
                if (subgroups == null)
                {
                    Result.code = 3;
                    Result.message = ClassCollection.Message.PUSH_COMMAND_INCORRECT;
                    return false; ;
                }

                var subs = subgroups.Split(',').Select(c => long.Parse(c));
                var uids = new List<long>();
                foreach (var item in subs)
                {
                    uids.AddRange(db.mUserTbls.Where(c => c.userSubGroupTbls.Any(p => p.subGroupID == item)).Select(c => c.ID).ToList());
                }
                uids = uids.Distinct().ToList();

                var tks = new List<string>();

                foreach (var item in uids)
                {
                    try
                    {
                        tks.AddRange(db.pushUserTbls.Where(c => c.mUsreID != null).Where(c => c.mUsreID.Value == item).Select(c => c.token));
                    }
                    catch { }
                }

                tokens = tks.AsQueryable();
            }
            int pageIndex = 1;
            while (true)
            {
                bool Continue = false;
                var ttoken = tokens.Skip((pageIndex - 1) * 900).Take(900);
                ids = "";
                foreach (var item in ttoken)
                {
                    Continue = true;
                    ids += "\"" + item + "\",";
                }

                if (Continue)
                {
                    var lastIndex = ids.LastIndexOf(",");
                    ids = ids.Remove(lastIndex, 1);

                    string postData = "{ \"registration_ids\": [ " + ids + " ], " +
                      "\"data\": {\"tickerText\":\"" + ticker + "\", " +
                                 "\"contentTitle\":\"" + title + "\", " +
                                 "\"manual\":\"" + "true" + "\", " +
                                 "\"message\": \"" + text + "\"}}";

                    string response = SendGCMNotification(ClassCollection.Methods.getGCMAPIKEY(), postData);
                    if (response != "0")
                    {
                        Result.code = 2;
                        Result.message = ClassCollection.Message.PUSH_FAILED;
                        return false;
                    }
                    pageIndex++;
                }
                else
                    break;

            }
            Result.code = 0;
            Result.message = ClassCollection.Message.OPERATION_SUCCESS;
            return true;
        }
        private string SendGCMNotification(string apiKey, string postData)
        {
            string postDataContentType = "application/json";
            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateServerCertificate);

            //
            //  MESSAGE CONTENT
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            //
            //  CREATE REQUEST
            HttpWebRequest Request = (HttpWebRequest)WebRequest.Create("https://android.googleapis.com/gcm/send");
            Request.Method = "POST";
            Request.KeepAlive = false;
            Request.ContentType = postDataContentType;
            Request.Headers.Add(string.Format("Authorization: key={0}", apiKey));
            Request.ContentLength = byteArray.Length;

            Stream dataStream = Request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            //
            //  SEND MESSAGE
            try
            {
                WebResponse Response = Request.GetResponse();
                HttpStatusCode ResponseCode = ((HttpWebResponse)Response).StatusCode;
                if (ResponseCode.Equals(HttpStatusCode.Unauthorized) || ResponseCode.Equals(HttpStatusCode.Forbidden))
                {
                   // var text = "Unauthorized - need new token";
                }
                else if (!ResponseCode.Equals(HttpStatusCode.OK))
                {
                   // var text = "Response from web service isn't OK";
                }

                StreamReader Reader = new StreamReader(Response.GetResponseStream());
                string responseLine = Reader.ReadToEnd();
                Reader.Close();

                return "0";
            }
            catch
            {
            }
            return "error";
        }

        private static bool ValidateServerCertificate(
                                                    object sender,
                                                    X509Certificate certificate,
                                                    X509Chain chain,
                                                    SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

    }
}

