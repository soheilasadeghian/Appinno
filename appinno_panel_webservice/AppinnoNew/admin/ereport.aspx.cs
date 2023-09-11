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
    public partial class ereport : System.Web.UI.Page
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


        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string getAllTag()
        {
            ClassCollection.Model.TagListResult result = new ClassCollection.Model.TagListResult();
            result.result = new ClassCollection.Model.Result();


            if (!ClassCollection.Methods.isLogin(HttpContext.Current))
            {
                result.result.code = 1;
                result.result.message = ClassCollection.Message.LOGIN_FIRST;
                return new JavaScriptSerializer().Serialize(result);
            }

            var db = new DataAccessDataContext();
            var user = ClassCollection.Methods.getUserInSession(HttpContext.Current);

            var tags = db.tagTbls.OrderByDescending(c => c.ID).ToList();

            result.tag = new List<ClassCollection.Model.Tag>();
            foreach (var item in tags)
            {
                var tmp = new ClassCollection.Model.Tag();
                tmp.ID = item.ID;
                tmp.title = item.title;
                result.tag.Add(tmp);
            }

            result.result.code = 0;
            result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
        }

        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string getreport(int reportID)
        {
            ClassCollection.Model.reportResult result = new ClassCollection.Model.reportResult();
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

            if (permission.chart.access.showlist == false)
            {
                result.result.code = 2;
                result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }

            if (db.masterReportTbls.Any(c => c.ID == reportID) == false)
            {
                result.result.code = 2;
                result.result.message = ClassCollection.Message.CHART_NOT_EXIST;
                result.report = null;

                return new JavaScriptSerializer().Serialize(result);
            }

            var master = db.masterReportTbls.Single(c => c.ID == reportID);

            var contents = db.masterReportContentTbls.Where(c => c.masterReportID == reportID);
            var groups = db.masterRreportSubGroupTbls.Where(c => c.reportID == reportID);
            var tags = db.masterReportTagTbls.Where(c => c.masterReportID == reportID);

            result.report = new ClassCollection.Model.fullreport();
            result.report.ID = master.ID;
            result.report.title = master.title;
            result.report.isBlock = master.isBlock;
            result.report.publishDate = Persia.Calendar.ConvertToPersian(master.publishDate).ToString();
            result.report.userID = master.userID;
            result.report.mUserID = master.mUserID;
            result.report.viewCount = master.viewCount;

            result.report.contents = new List<ClassCollection.Model.reportContent>();
            result.report.groups = new List<ClassCollection.Model.reportGroupSubGroup>();
            result.report.tag = new List<Model.Tag>();
            foreach (var item in tags)
            {
                var tmp = new ClassCollection.Model.Tag();
                tmp.ID = item.tagTbl.ID;
                tmp.title = item.tagTbl.title;

                result.report.tag.Add(tmp);
            }
            foreach (var item in groups)
            {
                var tmp = new ClassCollection.Model.reportGroupSubGroup();
                tmp.ID = item.ID;
                tmp.reportID = item.reportID;
                tmp.subGroupID = item.subGroupID;
                tmp.subGroupTitle = item.SubGroupTbl.title;
                tmp.groupID = item.SubGroupTbl.groupID;
                tmp.groupTitle = item.SubGroupTbl.GroupTbl.title;

                result.report.groups.Add(tmp);
            }

            foreach (var item in contents)
            {
                var tmp = new ClassCollection.Model.reportContent();

                tmp.ID = item.ID;
                tmp.type = item.type;
                
                if (item.type == 0)
                {
                    tmp.value = item.value;
                }
                else if (item.type == 1)
                {
                    tmp.value = ClassCollection.Methods.getreportImagesPath().Replace("~", "") + item.value;
                }
                else if (item.type == 2)
                {
                    tmp.minReport = new Model.minReport();
                    var id = long.Parse(item.value);
                    var t = db.reportTbls.Single(c => c.ID == id);
                    tmp.minReport.ID = t.ID;
                    tmp.minReport.type = t.charttypes;
                    tmp.minReport.xTitle = t.xTitle;
                    tmp.minReport.yTitle = t.yTitle;
                    var charts = t.chartTbls;
                    tmp.minReport.chart = new List<Model.chart>();
                    foreach (var chh in charts)
                    {
                        var tp = new Model.chart();
                        tp.ID = chh.ID;
                        tp.reportID = chh.reportID;
                        tp.xTitle = chh.xTitle;
                        tp.yValue = chh.yValue;
                        tmp.minReport.chart.Add(tp);
                    }

                }


                result.report.contents.Add(tmp);
            }

            result.result.code = 0;
            result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
        }
    }
}

