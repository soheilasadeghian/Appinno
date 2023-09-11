using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using static appinno2.ClassCollection.WebServiceModels;

namespace appinno2.ClassCollection
{
    public class WebService
    {
        public static class Core
        {
            public static string CallMethod( string method, Dictionary<string, string> Parameters)
            {
                string webServiceURL = "http://localhost:21457/service/userservice.asmx/" + method;
                //string data = parameter; ;
                byte[] dataStream = CreateHttpRequestData(Parameters);

                WebRequest request = WebRequest.Create(webServiceURL);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                Stream stream = request.GetRequestStream();
                stream.Write(dataStream, 0, dataStream.Length);
                stream.Close();
                WebResponse response = request.GetResponse();
                Stream respStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(respStream);

                string json = reader.ReadToEnd();
                return json;

            }
            private static byte[] CreateHttpRequestData(Dictionary<string, string> dic)
            {
                StringBuilder sbParameters = new StringBuilder();
                foreach (string param in dic.Keys)
                {
                    sbParameters.Append(param);//key => parameter name
                    sbParameters.Append('=');
                    sbParameters.Append(dic[param]);//key value
                    sbParameters.Append('&');
                }
                sbParameters.Remove(sbParameters.Length - 1, 1);

                UTF8Encoding encoding = new UTF8Encoding();

                return encoding.GetBytes(sbParameters.ToString());

            }

        }

        public static LatestNewsForListResult getLatestNewsForList(string mobileOrEmail, string fillterString, long groupID, long subGroupID, int count, int pageIndex, long tagID)
        { 
            var dic = new Dictionary<string, string>();
            dic.Add("key", "1");
            dic.Add("mobileOrEmail", mobileOrEmail);
            dic.Add("fillterString", fillterString);
            dic.Add("groupID", groupID+"");
            dic.Add("subGroupID", subGroupID+"");
            dic.Add("count", count+"");
            dic.Add("pageIndex", pageIndex+"");
            dic.Add("tagID", tagID+"");

            var result = Core.CallMethod("getLatestNewsForList", dic);

            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Deserialize<LatestNewsForListResult>(result);
        }
        public static LatestDownloadForListResult getLatestDownloadForList(string mobileOrEmail, string fillterString, long groupID, long subGroupID, int count, int pageIndex, long tagID)
        {
            var dic = new Dictionary<string, string>();
            dic.Add("key", "1");
            dic.Add("mobileOrEmail", mobileOrEmail);
            dic.Add("fillterString", fillterString);
            dic.Add("groupID", groupID + "");
            dic.Add("subGroupID", subGroupID + "");
            dic.Add("count", count + "");
            dic.Add("pageIndex", pageIndex + "");
            dic.Add("tagID", tagID + "");

            var result = Core.CallMethod("getLatestDownloadForList", dic);

            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Deserialize<LatestDownloadForListResult>(result);
        }
        public static DownloadResult getDownload(string downloadID)
        {
            var dic = new Dictionary<string, string>();
            dic.Add("key", "1");
            dic.Add("downloadID", downloadID);

            var result = Core.CallMethod("getDownload", dic);

            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Deserialize<DownloadResult>(result);
        }
        public static LatestEventsForListResult getAllEvents(string mobileOrEmail, string fillterString, long groupID, long subGroupID, int count, int pageIndex, long tagID)
        {
            var dic = new Dictionary<string, string>();
            dic.Add("key", "1");
            dic.Add("mobileOrEmail", mobileOrEmail);
            dic.Add("fillterString", fillterString);
            dic.Add("groupID", groupID + "");
            dic.Add("subGroupID", subGroupID + "");
            dic.Add("count", count + "");
            dic.Add("pageIndex", pageIndex + "");
            dic.Add("tagID", tagID + "");

            var result = Core.CallMethod("getAllEvents", dic);

            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Deserialize<LatestEventsForListResult>(result);
        }
        public static LatestPublicationsForListResult getLatestPubForList(string mobileOrEmail, string fillterString, long groupID, long subGroupID, int count, int pageIndex, long tagID)
        {
                    var dic = new Dictionary<string, string>();
                    dic.Add("key", "1");
                    dic.Add("mobileOrEmail", mobileOrEmail);
                    dic.Add("fillterString", fillterString);
                    dic.Add("groupID", groupID + "");
                    dic.Add("subGroupID", subGroupID + "");
                    dic.Add("count", count + "");
                    dic.Add("pageIndex", pageIndex + "");
                    dic.Add("tagID", tagID + "");

                    var result = Core.CallMethod("getLatestPubForList", dic);

                    JavaScriptSerializer js = new JavaScriptSerializer();
                    return js.Deserialize<LatestPublicationsForListResult>(result);
                }
        public static LatestIoForListResult getLatestIoForList(string mobileOrEmail, string fillterString, long groupID, long subGroupID, int count, int pageIndex, long tagID)
        {
            var dic = new Dictionary<string, string>();
            dic.Add("key", "1");
            dic.Add("mobileOrEmail", mobileOrEmail);
            dic.Add("fillterString", fillterString);
            dic.Add("groupID", groupID + "");
            dic.Add("subGroupID", subGroupID + "");
            dic.Add("count", count + "");
            dic.Add("pageIndex", pageIndex + "");
            dic.Add("tagID", tagID + "");

            var result = Core.CallMethod("getLatestIoForList", dic);

            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Deserialize<LatestIoForListResult>(result);
        }
        public static LatestChartForListResult getLatestChartForList(string mobileOrEmail, string fillterString, long groupID, long subGroupID, int count, int pageIndex, long tagID)
        {
            var dic = new Dictionary<string, string>();
            dic.Add("key", "1");
            dic.Add("mobileOrEmail", mobileOrEmail);
            dic.Add("fillterString", fillterString);
            dic.Add("groupID", groupID + "");
            dic.Add("subGroupID", subGroupID + "");
            dic.Add("count", count + "");
            dic.Add("pageIndex", pageIndex + "");
            dic.Add("tagID", tagID + "");

            var result = Core.CallMethod("getLatestChartForList", dic);

            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Deserialize<LatestChartForListResult>(result);
        }

        public static SingleNewsResult getSingleNews(long newsID)
        {
            var dic = new Dictionary<string, string>();
            dic.Add("key", "1");
            dic.Add("newsID", newsID + "");

            var result = Core.CallMethod("getSingleNews", dic);

            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Deserialize<SingleNewsResult>(result);
        }
        public static SingleEventsResult getSingleEvents(long eventsID)
        {
            var dic = new Dictionary<string, string>();
            dic.Add("key", "1");
            dic.Add("eventsID", eventsID + "");

            var result = Core.CallMethod("getSingleEvents", dic);

            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Deserialize<SingleEventsResult>(result);
        }
        public static SinglePublicationsResult getSinglePub(long publicationsID)
        {
            var dic = new Dictionary<string, string>();
            dic.Add("key", "1");
            dic.Add("publicationsID", publicationsID + "");

            var result = Core.CallMethod("getSinglePub", dic);

            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Deserialize<SinglePublicationsResult>(result);
        }
        public static SingleDownloadResult getSingleDownload(long downloadID)
        {
            var dic = new Dictionary<string, string>();
            dic.Add("key", "1");
            dic.Add("downloadID", downloadID + "");

            var result = Core.CallMethod("getSingleDownload", dic);

            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Deserialize<SingleDownloadResult>(result);
        }
        public static SingleIoResult getSingleIo(long ioID)
        {
            var dic = new Dictionary<string, string>();
            dic.Add("key", "1");
            dic.Add("ioID", ioID + "");

            var result = Core.CallMethod("getSingleIo", dic);

            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Deserialize<SingleIoResult>(result);
        }
        public static LikeUnlike likeUnlikeNews(long userID, long newsID, bool isLike)
        {
            var dic = new Dictionary<string, string>();
            dic.Add("key", "1");
            dic.Add("userID", userID + "");
            dic.Add("newsID", newsID + "");
            dic.Add("isLike", isLike + "");

            var result = Core.CallMethod("likeUnlikeNews", dic);

            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Deserialize<LikeUnlike>(result);
        }
    }
}