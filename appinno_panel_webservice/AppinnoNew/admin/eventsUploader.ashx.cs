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
    public class eventsUploader : IHttpHandler, IRequiresSessionState
    {
        public class item
        {
            public string type { get; set; }
            public HttpPostedFile value { get; set; }
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
                if (permission.events.access.insert == false)
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
                var todate = context.Request.Form["todate"];
                var fromdate = context.Request.Form["fromdate"];
                var allp = context.Request.Form.AllKeys.Where(c => c.Contains("par_"));
                var allvi = context.Request.Files.AllKeys.Where(c => c.Contains("img_") || c.Contains("vid_"));
                var list = new List<string>();
                list.AddRange(allp);
                list.AddRange(allvi);
                list.Sort(delegate(string p1, string p2)
                {
                    try
                    {
                        var p1Index = int.Parse(p1.Replace("img_", "").Replace("vid_", "").Replace("par_", ""));
                        var p2Index = int.Parse(p2.Replace("img_", "").Replace("vid_", "").Replace("par_", ""));
                        return p1Index.CompareTo(p2Index);
                    }
                    catch
                    {
                        return 0;
                    }

                });

                var events = new eventsTbl();
                var dt = new DateTime();
                dt = DateTime.Now;
                events.title = title;
                events.isBlock = true;
                events.isPublished = false;
                events.userID = (HttpContext.Current.Session["user"] as userTbl).ID;
                var dateArr = todate.Split('/');
                events.toDate = Persia.Calendar.ConvertToGregorian(int.Parse(dateArr[0]), int.Parse(dateArr[1]), int.Parse(dateArr[2]), Persia.DateType.Persian);

                dateArr = fromdate.Split('/');
                events.fromDate = Persia.Calendar.ConvertToGregorian(int.Parse(dateArr[0]), int.Parse(dateArr[1]), int.Parse(dateArr[2]), Persia.DateType.Persian);

                if (events.fromDate.CompareTo(events.toDate) > 0)
                {
                    result.code = 2;
                    result.message = ClassCollection.Message.DATE_INCORRECT;
                    context.Response.Write(new JavaScriptSerializer().Serialize(result));
                    return;
                }

                events.regDate = dt;
                db.eventsTbls.InsertOnSubmit(events);
                db.SubmitChanges();

                foreach (var item in subgroups)
                {
                    var subevents = new eventsSubGroupTbl();
                    subevents.eventsID = events.ID;
                    subevents.subGroupID = item;

                    db.eventsSubGroupTbls.InsertOnSubmit(subevents);
                    db.SubmitChanges();
                }
                foreach (var item in tags)
                {
                    var tag = new eventTagTbl();
                    tag.eventID = events.ID;
                    tag.tagID = item;

                    db.eventTagTbls.InsertOnSubmit(tag);
                    db.SubmitChanges();
                }
                foreach (var item in list)
                {
                    if (item.Contains("par_"))
                    {
                        var tbl = new eventsContentTbl();
                        tbl.value = context.Request.Form[item];
                        tbl.regDate = dt;
                        tbl.eventsID = events.ID;
                        tbl.contentType = 0;
                        db.eventsContentTbls.InsertOnSubmit(tbl);
                        db.SubmitChanges();
                    }
                    else if (item.Contains("img_"))
                    {

                        HttpPostedFile file = context.Request.Files[item];
                        var ext = "." + file.ContentType.Split('/')[1];
                        var imagename = "appinno" + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N") + ext;
                        string fname = context.Server.MapPath(ClassCollection.Methods.geteventsImagesPath() + imagename);
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

                        var tbl = new eventsContentTbl();
                        tbl.value = imagename;
                        tbl.regDate = dt;
                        tbl.eventsID = events.ID;
                        tbl.contentType = 1;
                        db.eventsContentTbls.InsertOnSubmit(tbl);
                        db.SubmitChanges();

                    }
                    else if (item.Contains("vid_"))
                    {
                        HttpPostedFile file = context.Request.Files[item];
                        var ext = "." + file.ContentType.Split('/')[1];
                        var imagename = "appinno" + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N") + ext;
                        string fname = context.Server.MapPath(ClassCollection.Methods.geteventsVideosPath() + imagename);
                        try
                        {
                            file.SaveAs(fname);
                        }
                        catch (Exception e)
                        {
                            context.Response.ContentType = "text/plain";
                            context.Response.Write(e.Message + "save problem");

                        }

                        var tbl = new eventsContentTbl();
                        tbl.value = imagename;
                        tbl.regDate = dt;
                        tbl.eventsID = events.ID;
                        tbl.contentType = 2;
                        db.eventsContentTbls.InsertOnSubmit(tbl);
                        db.SubmitChanges();

                    }
                }
               
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
                    long s = long.Parse(context.Request.Form["eventsID"].ToString());
                }
                catch
                {

                }
                if (permission.events.access.edit == false)
                {
                    result.code = 2;
                    result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                    context.Response.Write(new JavaScriptSerializer().Serialize(result));
                    return;
                }
                long eventsID = long.Parse(context.Request.Form["eventsID"].ToString());
                if (!db.eventsTbls.Any(c => c.ID == eventsID))
                {
                    result.code = 3;
                    result.message = ClassCollection.Message.EVENTS_NOT_EXIST;
                    context.Response.Write(new JavaScriptSerializer().Serialize(result));
                    return;
                }

                var events = db.eventsTbls.Single(c => c.ID == eventsID);
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
                var todate = context.Request.Form["todate"];
                var fromdate = context.Request.Form["fromdate"];
                var allp = context.Request.Form.AllKeys.Where(c => c.Contains("par_") || c.Contains("lpar_") || c.Contains("limg_") || c.Contains("lvid_"));
                var allvi = context.Request.Files.AllKeys.Where(c => (c.Contains("img_") && !c.Contains("limg_")) || (c.Contains("vid_") && !c.Contains("lvid_")));
                var list = new List<string>();
                list.AddRange(allp);
                list.AddRange(allvi);
                list.Sort(delegate(string p1, string p2)
                {
                    try
                    {
                        var p1Index = long.Parse(p1.Replace("img_", "").Replace("vid_", "").Replace("par_", "")
                            .Replace("lvid_", "").Replace("limg_", "").Replace("lpar_", ""));

                        var p2Index = long.Parse(p2.Replace("img_", "").Replace("vid_", "").Replace("par_", "")
                            .Replace("lvid_", "").Replace("limg_", "").Replace("lpar_", ""));

                        return p1Index.CompareTo(p2Index);
                    }
                    catch
                    {
                        return 1;
                    }

                });

                var dt = new DateTime();
                dt = DateTime.Now;
                events.title = title;
                events.isBlock = block;
                if (!events.mUserID.HasValue)
                    events.userID = (HttpContext.Current.Session["user"] as userTbl).ID;

                var dateArr = todate.Split('/');
                events.toDate = Persia.Calendar.ConvertToGregorian(int.Parse(dateArr[0]), int.Parse(dateArr[1]), int.Parse(dateArr[2]), Persia.DateType.Persian);

                dateArr = fromdate.Split('/');
                events.fromDate = Persia.Calendar.ConvertToGregorian(int.Parse(dateArr[0]), int.Parse(dateArr[1]), int.Parse(dateArr[2]), Persia.DateType.Persian);

                events.regDate = dt;

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

                List<long> old_video_Item = new List<long>();
                foreach (var item in list)
                {
                    long id = -1;
                    if (item.Contains("lvid_"))
                    {
                        id = long.Parse(context.Request.Form[item]);
                        old_video_Item.Add(id);
                    }


                }
                var deletingPar = db.eventsContentTbls.Where(c => c.eventsID == eventsID && c.contentType == 0);
                foreach (var item in deletingPar)
                {
                    if (!old_Par_Item.Contains(item.ID))
                    {
                        db.eventsContentTbls.DeleteOnSubmit(item);
                    }
                }
                var deletingImg = db.eventsContentTbls.Where(c => c.eventsID == eventsID && c.contentType == 1);
                foreach (var item in deletingImg)
                {
                    if (!old_image_Item.Contains(item.ID))
                    {

                        var img = item.value;

                        if (System.IO.File.Exists(context.Server.MapPath(ClassCollection.Methods.geteventsImagesPath() + img)))
                        {
                            System.IO.File.Delete(context.Server.MapPath(ClassCollection.Methods.geteventsImagesPath() + img));
                        }
                        db.eventsContentTbls.DeleteOnSubmit(item);
                    }
                }
                var deletingVideo = db.eventsContentTbls.Where(c => c.eventsID == eventsID && c.contentType == 2);
                foreach (var item in deletingVideo)
                {
                    if (!old_video_Item.Contains(item.ID))
                    {
                        db.eventsContentTbls.DeleteOnSubmit(item);
                        var vid = item.value;

                        if (System.IO.File.Exists(context.Server.MapPath(ClassCollection.Methods.geteventsVideosPath() + vid)))
                        {
                            System.IO.File.Delete(context.Server.MapPath(ClassCollection.Methods.geteventsVideosPath() + vid));
                        }
                        db.eventsContentTbls.DeleteOnSubmit(item);
                    }
                }

                events.userID = user.ID;
                db.SubmitChanges();

                db.eventTagTbls.DeleteAllOnSubmit(db.eventTagTbls.Where(c => c.eventID == eventsID));
                db.SubmitChanges();

                foreach (var item in tags)
                {
                    var tag = new eventTagTbl();
                    tag.eventID = events.ID;
                    tag.tagID = item;

                    db.eventTagTbls.InsertOnSubmit(tag);
                    db.SubmitChanges();
                }


                db.eventsSubGroupTbls.DeleteAllOnSubmit(db.eventsSubGroupTbls.Where(c => c.eventsID == eventsID));
                db.SubmitChanges();

                foreach (var item in subgroups)
                {
                    var subevents = new eventsSubGroupTbl();
                    subevents.eventsID = events.ID;
                    subevents.subGroupID = item;

                    db.eventsSubGroupTbls.InsertOnSubmit(subevents);
                    db.SubmitChanges();
                }

                foreach (var item in list)
                {
                    if (item.Contains("par_") && !item.Contains("lpar_"))
                    {
                        var tbl = new eventsContentTbl();
                        tbl.value = context.Request.Form[item];
                        tbl.regDate = dt;
                        tbl.eventsID = events.ID;
                        tbl.contentType = 0;
                        tbl.pri = int.Parse(item.Replace("par_", "").Replace("lpar_", ""));//contentedit
                        db.eventsContentTbls.InsertOnSubmit(tbl);
                        db.SubmitChanges();
                    }
                    else if (item.Contains("img_") && !item.Contains("limg_"))
                    {

                        HttpPostedFile file = context.Request.Files[item];
                        var ext = "." + file.ContentType.Split('/')[1];
                        var imagename = "appinno" + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N") + ext;
                        string fname = context.Server.MapPath(ClassCollection.Methods.geteventsImagesPath() + imagename);
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

                        var tbl = new eventsContentTbl();
                        tbl.value = imagename;
                        tbl.regDate = dt;
                        tbl.eventsID = events.ID;
                        tbl.pri = int.Parse(item.Replace("img_", "").Replace("limg_", ""));//contentedit
                        tbl.contentType = 1;
                        db.eventsContentTbls.InsertOnSubmit(tbl);
                        db.SubmitChanges();

                    }
                    else if (item.Contains("vid_") && !item.Contains("lvid_"))
                    {
                        HttpPostedFile file = context.Request.Files[item];
                        var ext = "." + file.ContentType.Split('/')[1];
                        var imagename = "appinno" + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N") + ext;
                        string fname = context.Server.MapPath(ClassCollection.Methods.geteventsVideosPath() + imagename);
                        try
                        {
                            file.SaveAs(fname);
                        }
                        catch (Exception e)
                        {
                            context.Response.ContentType = "text/plain";
                            context.Response.Write(e.Message + "save problem");
                        }

                        var tbl = new eventsContentTbl();
                        tbl.value = imagename;
                        tbl.regDate = dt;
                        tbl.eventsID = events.ID;
                        tbl.pri = int.Parse(item.Replace("vid_", "").Replace("lvid_", ""));//contentedit
                        tbl.contentType = 2;
                        db.eventsContentTbls.InsertOnSubmit(tbl);
                        //contentedit-start
                        db.SubmitChanges();
                    }
                }

                foreach (var item in list)
                {
                    if (item.Contains("lpar_"))
                    {
                        var tbl = db.eventsContentTbls.Single(c => c.ID == long.Parse(context.Request.Form[item]));
                        tbl.pri = int.Parse(item.Replace("lpar_", "").Replace("par_", ""));
                        db.SubmitChanges();
                    }
                    else if (item.Contains("limg_"))
                    {
                        var tbl = db.eventsContentTbls.Single(c => c.ID == long.Parse(context.Request.Form[item]));
                        tbl.pri = int.Parse(item.Replace("limg_", "").Replace("img_", ""));
                        db.SubmitChanges();
                    }
                    else if (item.Contains("lvid_"))
                    {
                        var tbl = db.eventsContentTbls.Single(c => c.ID == long.Parse(context.Request.Form[item]));
                        tbl.pri = int.Parse(item.Replace("lvid_", "").Replace("vid_", ""));
                        //contentedit-end
                        db.SubmitChanges();
                    }
                }
                if(events.isBlock==false && events.isPublished==false)
                {
                    events.isPublished = true;
                    db.SubmitChanges();
                    new service.pushservice().XsendPush("1", "events", events.ID + "", events.title, "toall", "false");

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