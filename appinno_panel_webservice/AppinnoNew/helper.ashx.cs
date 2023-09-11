using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;

namespace AppinnoNew
{
    public class helper : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Request.ContentEncoding = Encoding.UTF8;
            ClassCollection.Model.Result result = new ClassCollection.Model.Result();

            if (context.Request.QueryString["comm"] == "excel")
            {
                var excelfile = context.Request.Files["excel"];
                var entries = ClassCollection.Excel.Import.ChartLoader.LoadChart(excelfile.InputStream);

                context.Response.Write(new JavaScriptSerializer().Serialize(entries));
                return;                
            }
            if (!ClassCollection.Methods.isLogin(context))
            {
                result.code = 1;
                result.message = ClassCollection.Message.LOGIN_FIRST;
                context.Response.Write(new JavaScriptSerializer().Serialize(result));
                return;
            }
            var db = new DataAccessDataContext();
            var user = ClassCollection.Methods.getUserInSession(HttpContext.Current);
            var permission = ClassCollection.Accessing.Deserialize(db.accessTbls.Single(c => c.ID == user.accessID).per);

        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}