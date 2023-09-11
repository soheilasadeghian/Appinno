using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;

namespace AppinnoNew.admin
{
    /// <summary>
    /// Summary description for partnersUploader
    /// </summary>
    public class reportUploader : IHttpHandler, IRequiresSessionState
    {
        public class record
        {
            public string title { get; set; }
            public float value { get; set; }
        }
        public class itemclass
        {
            public string xtitle { get; set; }
            public string ytitle { get; set; }
            public string type { get; set; }
            public string id { get; set; }
            public List<record> data { get; set; }
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Request.ContentEncoding = Encoding.UTF8;
            ClassCollection.Model.Result result = new ClassCollection.Model.Result();
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

            #region insert

            if (context.Request.QueryString["mode"] == "n")
            {
                if (permission.chart.access.insert == false)
                {
                    result.code = 2;
                    result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                    context.Response.Write(new JavaScriptSerializer().Serialize(result));
                    return;
                }
                try
                {
                    var dymmy = context.Request.Form["tags"];
                }
                catch
                {
                }
                var subgroups = new JavaScriptSerializer().Deserialize<List<long>>(context.Request.Form["subgroups"]);
                var tags = new JavaScriptSerializer().Deserialize<List<long>>(context.Request.Form["tags"]);
                var tagstring = context.Request.Form["tagstring"].Split('#').Where(c => string.IsNullOrEmpty(c) == false).Select(c => c.TrimEnd().TrimStart()).ToList<string>();

                foreach (var tc in tagstring)
                {
                    if (db.tagTbls.Any(c => c.title == tc))
                    {
                        var t = db.tagTbls.Single(c => c.title == tc);
                        tags.Add(t.ID);
                    }
                    else
                    {
                        var nt = new tagTbl();
                        nt.title = tc;
                        db.tagTbls.InsertOnSubmit(nt);
                        db.SubmitChanges();

                        tags.Add(nt.ID);
                    }
                }
                var title = context.Request.Form["title"];
                var pubdate = context.Request.Form["date"];
                var allp = context.Request.Form.AllKeys.Where(c => c.Contains("par_") || c.Contains("chart_"));
                var allvi = context.Request.Files.AllKeys.Where(c => c.Contains("img_"));
                var list = new List<string>();
                list.AddRange(allp);
                list.AddRange(allvi);
                list.Sort(delegate(string p1, string p2)
                {
                    try
                    {
                        var p1Index = int.Parse(p1.Replace("img_", "").Replace("chart_", "").Replace("par_", ""));
                        var p2Index = int.Parse(p2.Replace("img_", "").Replace("chart_", "").Replace("par_", ""));
                        return p1Index.CompareTo(p2Index);
                    }
                    catch
                    {
                        return 0;
                    }

                });

                var report = new masterReportTbl();
                var dt = new DateTime();
                dt = DateTime.Now;
                report.viewCount = 0;
                report.title = title;
                report.isBlock = true;
                report.userID = (HttpContext.Current.Session["user"] as userTbl).ID;
                var dateArr = pubdate.Split('/');
                report.publishDate = Persia.Calendar.ConvertToGregorian(int.Parse(dateArr[0]), int.Parse(dateArr[1]), int.Parse(dateArr[2]), Persia.DateType.Persian);
                report.regDate = dt;
                db.masterReportTbls.InsertOnSubmit(report);
                db.SubmitChanges();

                foreach (var item in subgroups)
                {
                    var subreport = new masterRreportSubGroupTbl();
                    subreport.reportID = report.ID;
                    subreport.subGroupID = item;

                    db.masterRreportSubGroupTbls.InsertOnSubmit(subreport);
                    db.SubmitChanges();
                }
                foreach (var item in tags)
                {
                    var tag = new masterReportTagTbl();
                    tag.masterReportID = report.ID;
                    tag.tagID = item;

                    db.masterReportTagTbls.InsertOnSubmit(tag);
                    db.SubmitChanges();
                }


                foreach (var item in list)
                {
                    if (item.Contains("par_"))
                    {
                        var tbl = new masterReportContentTbl();
                        tbl.value = context.Request.Form[item];
                        tbl.masterReportID = report.ID;
                        tbl.type = 0;
                        db.masterReportContentTbls.InsertOnSubmit(tbl);
                        db.SubmitChanges();
                    }
                    else if (item.Contains("img_"))
                    {

                        HttpPostedFile file = context.Request.Files[item];
                        var ext = "." + file.ContentType.Split('/')[1];
                        var imagename = "appinno" + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N") + ext;
                        string fname = context.Server.MapPath(ClassCollection.Methods.getreportImagesPath() + imagename);
                        try
                        {
                            file.SaveAs(fname);
                        }
                        catch (Exception e)
                        {
                            context.Response.ContentType = "text/plain";
                            context.Response.Write(e.Message + "save problem");
                            return;
                        }

                        var tbl = new masterReportContentTbl();
                        tbl.value = imagename;

                        tbl.masterReportID = report.ID;
                        tbl.type = 1;
                        db.masterReportContentTbls.InsertOnSubmit(tbl);
                        db.SubmitChanges();

                    }
                    else if (item.Contains("chart_"))
                    {
                        var tbl = new masterReportContentTbl();

                        var data = new JavaScriptSerializer().Deserialize<itemclass>(context.Request.Form[item]);
                        var tmp = new reportTbl();

                        tmp.xTitle = data.xtitle;
                        tmp.yTitle = data.ytitle;
                        tmp.charttypes = data.type;
                        db.reportTbls.InsertOnSubmit(tmp);
                        db.SubmitChanges();


                        foreach (var rec in data.data)
                        {
                            var t = new chartTbl();
                            t.reportID = tmp.ID;
                            t.xTitle = rec.title;
                            t.yValue = rec.value;
                            db.chartTbls.InsertOnSubmit(t);
                            db.SubmitChanges();
                        }
                        tbl.value = tmp.ID.ToString();
                        tbl.masterReportID = report.ID;
                        tbl.type = 2;
                        db.masterReportContentTbls.InsertOnSubmit(tbl);
                        db.SubmitChanges();

                    }
                }
                new service.pushservice().XsendPush("1", "report", report.ID + "", report.title, "toall", "false");
                result.code = 0;
                result.message = ClassCollection.Message.OPERATION_SUCCESS;
                context.Response.Write(new JavaScriptSerializer().Serialize(result));
                return;
            }
            #endregion

            #region edit

            if (context.Request.QueryString["mode"] == "e")
            {
                try
                {
                    long s = long.Parse(context.Request.Form["reportID"].ToString());
                }
                catch
                {

                }
                if (permission.chart.access.edit == false)
                {
                    result.code = 2;
                    result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                    context.Response.Write(new JavaScriptSerializer().Serialize(result));
                    return;
                }
                long reportID = long.Parse(context.Request.Form["reportID"].ToString());
                if (!db.masterReportTbls.Any(c => c.ID == reportID))
                {
                    result.code = 3;
                    result.message = ClassCollection.Message.CHART_NOT_EXIST;
                    context.Response.Write(new JavaScriptSerializer().Serialize(result));
                    return;
                }

                var master = db.masterReportTbls.Single(c => c.ID == reportID);
                try
                {
                    var dymmy = context.Request.Form["tags"];
                }
                catch
                {
                }
                var subgroups = new JavaScriptSerializer().Deserialize<List<long>>(context.Request.Form["subgroups"]);
                var tags = new JavaScriptSerializer().Deserialize<List<long>>(context.Request.Form["tags"]);
                var tagstring = context.Request.Form["tagstring"].Split('#').Where(c => string.IsNullOrEmpty(c) == false).Select(c => c.TrimEnd().TrimStart()).ToList<string>();

                foreach (var tc in tagstring)
                {
                    if (db.tagTbls.Any(c => c.title == tc))
                    {
                        var t = db.tagTbls.Single(c => c.title == tc);
                        tags.Add(t.ID);
                    }
                    else
                    {
                        var nt = new tagTbl();
                        nt.title = tc;
                        db.tagTbls.InsertOnSubmit(nt);
                        db.SubmitChanges();

                        tags.Add(nt.ID);
                    }
                }
                var title = context.Request.Form["title"];
                var block = bool.Parse(context.Request.Form["block"]);
                var pubdate = context.Request.Form["date"];
                var allp = context.Request.Form.AllKeys.Where(c => c.Contains("par_") || c.Contains("lpar_") || c.Contains("lchart_") || c.Contains("limg_") || c.Contains("chart_"));
                var allvi = context.Request.Files.AllKeys.Where(c => (c.Contains("img_") && !c.Contains("limg_")) );
                var list = new List<string>();
                list.AddRange(allp);
                list.AddRange(allvi);
                list.Sort(delegate(string p1, string p2)
                {
                    try
                    {
                        var p1Index = long.Parse(p1.Replace("img_", "").Replace("chart_", "").Replace("par_", "")
                            .Replace("lchart_", "").Replace("limg_", "").Replace("lpar_", ""));

                        var p2Index = long.Parse(p2.Replace("img_", "").Replace("chart_", "").Replace("par_", "")
                            .Replace("lchart_", "").Replace("limg_", "").Replace("lpar_", ""));

                        return p1Index.CompareTo(p2Index);
                    }
                    catch
                    {
                        return 1;
                    }

                });

                var dt = new DateTime();
                dt = DateTime.Now;
                master.title = title;
                master.isBlock = block;
                if (!master.mUserID.HasValue)
                    master.userID = (HttpContext.Current.Session["user"] as userTbl).ID;
                var dateArr = pubdate.Split('/');
                master.publishDate = Persia.Calendar.ConvertToGregorian(int.Parse(dateArr[0]), int.Parse(dateArr[1]), int.Parse(dateArr[2]), Persia.DateType.Persian);
                master.regDate = dt;

                List<long> old_Par_Item = new List<long>();
                foreach (var item in list)
                {
                    long id = -1;
                    if (item.Contains("lpar_"))
                    {
                        id = long.Parse(context.Request.Form[item]);
                        old_Par_Item.Add(id);
                    }

                }

                List<long> old_image_Item = new List<long>();
                foreach (var item in list)
                {
                    long id = -1;
                    if (item.Contains("limg_"))
                    {
                        id = long.Parse(context.Request.Form[item]);
                        old_image_Item.Add(id);
                    }

                }

                List<long> old_chart_Item = new List<long>();
                foreach (var item in list)
                {
                    long id = -1;
                    if (item.Contains("lchart_"))
                    {
                        id = long.Parse(context.Request.Form[item].Split('_')[1]);
                        old_chart_Item.Add(id);
                    }


                }
                var deletingPar = db.masterReportContentTbls.Where(c => c.masterReportID == reportID && c.type == 0);
                foreach (var item in deletingPar)
                {
                    if (!old_Par_Item.Contains(item.ID))
                    {
                        db.masterReportContentTbls.DeleteOnSubmit(item);
                    }
                }
                var deletingImg = db.masterReportContentTbls.Where(c => c.masterReportID == reportID && c.type == 1);
                foreach (var item in deletingImg)
                {
                    if (!old_image_Item.Contains(item.ID))
                    {

                        var img = item.value;

                        if (System.IO.File.Exists(context.Server.MapPath(ClassCollection.Methods.getreportImagesPath() + img)))
                        {
                            System.IO.File.Delete(context.Server.MapPath(ClassCollection.Methods.getreportImagesPath() + img));
                        }
                        db.masterReportContentTbls.DeleteOnSubmit(item);
                    }
                }
                var deletingchart = db.masterReportContentTbls.Where(c => c.masterReportID == reportID && c.type == 2);
                foreach (var item in deletingchart)
                {
                    if (!old_chart_Item.Contains(long.Parse(item.value)))
                    {
                        var rptID = long.Parse(item.value);
                        var rpt = db.reportTbls.Single(c => c.ID == rptID);
                        db.chartTbls.DeleteAllOnSubmit(rpt.chartTbls);
                        db.SubmitChanges();
                        db.reportTbls.DeleteOnSubmit(rpt);
                        db.SubmitChanges();
                        db.masterReportContentTbls.DeleteOnSubmit(item);
                        db.SubmitChanges();
                    }
                }

                master.userID = user.ID;
                db.SubmitChanges();

                db.masterReportTagTbls.DeleteAllOnSubmit(db.masterReportTagTbls.Where(c => c.masterReportID == reportID));
                db.SubmitChanges();

                foreach (var item in tags)
                {
                    var tag = new masterReportTagTbl();
                    tag.masterReportID = master.ID;
                    tag.tagID = item;

                    db.masterReportTagTbls.InsertOnSubmit(tag);
                    db.SubmitChanges();
                }

                db.masterRreportSubGroupTbls.DeleteAllOnSubmit(db.masterRreportSubGroupTbls.Where(c => c.reportID == reportID));
                db.SubmitChanges();

                foreach (var item in subgroups)
                {
                    var subreport = new masterRreportSubGroupTbl();
                    subreport.reportID = master.ID;
                    subreport.subGroupID = item;

                    db.masterRreportSubGroupTbls.InsertOnSubmit(subreport);
                    db.SubmitChanges();
                }

                foreach (var item in list)
                {
                    if (item.Contains("par_") && !item.Contains("lpar_"))
                    {
                        var tbl = new masterReportContentTbl();
                        tbl.value = context.Request.Form[item];
                        tbl.masterReportID = master.ID;
                        tbl.type = 0;
                        db.masterReportContentTbls.InsertOnSubmit(tbl);
                        db.SubmitChanges();
                    }
                    else if (item.Contains("img_") && !item.Contains("limg_"))
                    {

                        HttpPostedFile file = context.Request.Files[item];
                        var ext = "." + file.ContentType.Split('/')[1];
                        var imagename = "appinno" + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N") + ext;
                        string fname = context.Server.MapPath(ClassCollection.Methods.getreportImagesPath() + imagename);
                        try
                        {
                            file.SaveAs(fname);
                        }
                        catch (Exception e)
                        {
                            context.Response.ContentType = "text/plain";
                            context.Response.Write(e.Message + "save problem");
                            return;
                        }

                        var tbl = new masterReportContentTbl();
                        tbl.value = imagename;
                        tbl.masterReportID = master.ID;
                        tbl.type = 1;
                        db.masterReportContentTbls.InsertOnSubmit(tbl);
                        db.SubmitChanges();

                    }
                    else if (item.Contains("chart_") && !item.Contains("lchart_"))
                    {
                        var tbl = new masterReportContentTbl();

                        var data = new JavaScriptSerializer().Deserialize<itemclass>(context.Request.Form[item]);
                        var tmp = new reportTbl();

                        tmp.xTitle = data.xtitle;
                        tmp.yTitle = data.ytitle;
                        tmp.charttypes = data.type;
                        db.reportTbls.InsertOnSubmit(tmp);
                        db.SubmitChanges();


                        foreach (var rec in data.data)
                        {
                            var t = new chartTbl();
                            t.reportID = tmp.ID;
                            t.xTitle = rec.title;
                            t.yValue = rec.value;
                            db.chartTbls.InsertOnSubmit(t);
                            db.SubmitChanges();
                        }
                        tbl.value = tmp.ID.ToString();
                        tbl.masterReportID = master.ID;
                        tbl.type = 2;
                        db.masterReportContentTbls.InsertOnSubmit(tbl);
                        db.SubmitChanges();
                    }
                }

                result.code = 0;
                result.message = ClassCollection.Message.OPERATION_SUCCESS;
                context.Response.Write(new JavaScriptSerializer().Serialize(result));
                return;
            }
            #endregion
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