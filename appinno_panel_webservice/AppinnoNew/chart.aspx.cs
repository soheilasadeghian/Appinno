using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AppinnoNew
{
    public partial class chart : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
           // test 
        }
        /// <summary>
        /// این متد وظیفه بررسی وجود یا عدم وجود نمودار مورد نظر را بر عهده دارد
        /// </summary>
        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string isReportExist(int reportID)
        {
            AppinnoNew.ClassCollection.Model.ReportResult result = new ClassCollection.Model.ReportResult();
            result.result = new ClassCollection.Model.Result();

            DataAccessDataContext db = new DataAccessDataContext();

            if (db.reportTbls.Any(c => c.ID == reportID) == false)
            {
                result.result.code = 2;
                result.result.message = ClassCollection.Message.CHART_NOT_EXIST;
                result.report = null;

                return new JavaScriptSerializer().Serialize(result);
            }

            var report = db.reportTbls.Single(c => c.ID == reportID);

            var values = report.chartTbls;
           

            result.report = new ClassCollection.Model.fullReport();
            result.report.ID = report.ID;
            result.report.xTitle = report.xTitle;
            result.report.yTitle = report.yTitle;
            result.report.type = report.charttypes;

            result.report.values = new List<ClassCollection.Model.ReportValue>();
           
            foreach (var item in values)
            {
                var tmp = new ClassCollection.Model.ReportValue();

                tmp.ID = item.ID;
                tmp.xTitle = item.xTitle;
                tmp.yVlaue = item.yValue;

                result.report.values.Add(tmp);
            }

            result.result.code = 0;
            result.result.message = ClassCollection.Message.OPERATION_SUCCESS;

            return new JavaScriptSerializer().Serialize(result);
        }
    }
}