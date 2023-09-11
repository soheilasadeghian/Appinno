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
    public class pollUploader : IHttpHandler, IRequiresSessionState
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
                if (permission.poll.access.insert == false)
                {
                    result.code = 2;
                    result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                    context.Response.Write(new JavaScriptSerializer().Serialize(result));
                    return;
                }
                try
                {
                    var dymmy = context.Request.Form["subgroups"];
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
                var options = new JavaScriptSerializer().Deserialize<List<string>>(context.Request.Form["options"]);
                var title = context.Request.Form["title"];
                var startDate = context.Request.Form["date"];
                var endDate = context.Request.Form["date2"];
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

                var polli = new pollTbl();
                var dt = new DateTime();
                dt = DateTime.Now;
                polli.isBlock = true;
                polli.userID = (HttpContext.Current.Session["user"] as userTbl).ID;
                try
                {
                    var dateArr = startDate.Split('/');
                    polli.startDate = Persia.Calendar.ConvertToGregorian(int.Parse(dateArr[0]), int.Parse(dateArr[1]), int.Parse(dateArr[2]), Persia.DateType.Persian);

                    dateArr = endDate.Split('/');
                    polli.finishedDate = Persia.Calendar.ConvertToGregorian(int.Parse(dateArr[0]), int.Parse(dateArr[1]), int.Parse(dateArr[2]), Persia.DateType.Persian);

                    if (polli.startDate.CompareTo(polli.finishedDate) > 0)
                    {
                        result.code = 2;
                        result.message = ClassCollection.Message.DATE_INCORRECT;
                        context.Response.Write(new JavaScriptSerializer().Serialize(result));
                        return;
                    }
                }
                catch
                {
                    result.code = 2;
                    result.message = ClassCollection.Message.DATE_INCORRECT;
                    context.Response.Write(new JavaScriptSerializer().Serialize(result));
                    return;
                }

                polli.regDate = dt;
                db.pollTbls.InsertOnSubmit(polli);
                db.SubmitChanges();

                foreach (var item in subgroups)
                {
                    var subpoll = new pollSubGroupTbl();
                    subpoll.pollID = polli.ID;
                    subpoll.subGroupID = item;

                    db.pollSubGroupTbls.InsertOnSubmit(subpoll);
                    db.SubmitChanges();
                }
                foreach (var item in tags)
                {
                    var tag = new pollTagTbl();
                    tag.pollID = polli.ID;
                    tag.tagID = item;

                    db.pollTagTbls.InsertOnSubmit(tag);
                    db.SubmitChanges();
                }

                foreach (var item in list)
                {
                    if (item.Contains("par_"))
                    {
                        var tbl = new pollContentTbl();
                        tbl.value = context.Request.Form[item];
                        tbl.regDate = dt;
                        tbl.pollID = polli.ID;
                        tbl.contentType = 0;
                        db.pollContentTbls.InsertOnSubmit(tbl);
                        db.SubmitChanges();
                    }
                    else if (item.Contains("img_"))
                    {

                        HttpPostedFile file = context.Request.Files[item];
                        var ext = "." + file.ContentType.Split('/')[1];
                        var imagename = "appinno" + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N") + ext;
                        string fname = context.Server.MapPath(ClassCollection.Methods.getPollImagesPath() + imagename);
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

                        var tbl = new pollContentTbl();
                        tbl.value = imagename;
                        tbl.regDate = dt;
                        tbl.pollID = polli.ID;
                        tbl.contentType = 1;
                        db.pollContentTbls.InsertOnSubmit(tbl);
                        db.SubmitChanges();

                    }
                }

                foreach (var item in options)
                {
                    var op = new optionTbl();
                    op.pollID = polli.ID;
                    op.regDate = dt;
                    op.text = item;

                    db.optionTbls.InsertOnSubmit(op);
                    db.SubmitChanges();
                }

                //
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
                    long s = long.Parse(context.Request.Form["pollID"].ToString());
                }
                catch
                {

                }
                if (permission.poll.access.edit == false)
                {
                    result.code = 2;
                    result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                    context.Response.Write(new JavaScriptSerializer().Serialize(result));
                    return;
                }
                long pollID = long.Parse(context.Request.Form["pollID"].ToString());
                if (!db.pollTbls.Any(c => c.ID == pollID))
                {
                    result.code = 3;
                    result.message = ClassCollection.Message.POLL_NOT_EXIST;
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
                var poll = db.pollTbls.Single(c => c.ID == pollID);
                var block = bool.Parse(context.Request.Form["block"]);

                var canEdit = !poll.optionTbls.Any(x => x.mUserOptionTbls.Any());
                if (!canEdit)
                {
                    poll.isBlock = block;
                    db.SubmitChanges();
                    result.code = 0;
                    result.message = ClassCollection.Message.OPERATION_SUCCESS;
                    context.Response.Write(new JavaScriptSerializer().Serialize(result));
                    return;
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
                var startDate = context.Request.Form["date"];
                var endDate = context.Request.Form["date2"];
                var alloptions= context.Request.Form.AllKeys.Where(c => c.Contains("lo_") || c.Contains("no_") ).ToList();
                var allp = context.Request.Form.AllKeys.Where(c => c.Contains("par_") || c.Contains("lpar_") || c.Contains("limg_"));
                var allvi = context.Request.Files.AllKeys.Where(c => (c.Contains("img_") && !c.Contains("limg_")) || (c.Contains("vid_")&&!c.Contains("lvid_")));
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
                alloptions.Sort(delegate(string p1, string p2)
                {
                    try
                    {
                        var p1Index = long.Parse(p1.Replace("lo_", "").Replace("no_", ""));

                        var p2Index = long.Parse(p2.Replace("lo_", "").Replace("no_", ""));

                        return p1Index.CompareTo(p2Index);
                    }
                    catch
                    {
                        return 1;
                    }

                });

                var dt = new DateTime();
                dt = DateTime.Now;
                poll.isBlock = block;
               
                poll.userID = (HttpContext.Current.Session["user"] as userTbl).ID;
                
                try
                {
                    var dateArr = startDate.Split('/');
                    poll.startDate = Persia.Calendar.ConvertToGregorian(int.Parse(dateArr[0]), int.Parse(dateArr[1]), int.Parse(dateArr[2]), Persia.DateType.Persian);

                    dateArr = endDate.Split('/');
                    poll.finishedDate = Persia.Calendar.ConvertToGregorian(int.Parse(dateArr[0]), int.Parse(dateArr[1]), int.Parse(dateArr[2]), Persia.DateType.Persian);

                    if (poll.startDate.CompareTo(poll.finishedDate) > 0)
                    {
                        result.code = 2;
                        result.message = ClassCollection.Message.DATE_INCORRECT;
                        context.Response.Write(new JavaScriptSerializer().Serialize(result));
                        return;
                    }
                }
                catch
                {
                    result.code = 2;
                    result.message = ClassCollection.Message.DATE_INCORRECT;
                    context.Response.Write(new JavaScriptSerializer().Serialize(result));
                    return;
                }

                poll.regDate = dt;

                List<long> old_OP_Item = new List<long>();
                foreach (var item in alloptions)
                {
                    long id = -1;
                    if (item.Contains("lo_"))
                    {
                        var t = context.Request.Form[item];
                        id = long.Parse(item.Replace("lo_", ""));
                        old_OP_Item.Add(id);
                    }

                }

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
                var deletingOption = db.optionTbls.Where(c => c.pollID == pollID );
                foreach (var item in deletingOption)
                {
                    if (!old_OP_Item.Contains(item.ID))
                    {
                        db.optionTbls.DeleteOnSubmit(item);
                    }
                }
                

                var deletingPar = db.pollContentTbls.Where(c => c.pollID == pollID && c.contentType == 0);
                foreach (var item in deletingPar)
                {
                    if (!old_Par_Item.Contains(item.ID))
                    {
                        db.pollContentTbls.DeleteOnSubmit(item);
                    }
                }
                var deletingImg = db.pollContentTbls.Where(c => c.pollID == pollID && c.contentType == 1);
                foreach (var item in deletingImg)
                {
                    if (!old_image_Item.Contains(item.ID))
                    {

                        var img = item.value;

                        if (System.IO.File.Exists(context.Server.MapPath(ClassCollection.Methods.getPollImagesPath() + img)))
                        {
                            System.IO.File.Delete(context.Server.MapPath(ClassCollection.Methods.getPollImagesPath() + img));
                        }
                        db.pollContentTbls.DeleteOnSubmit(item);
                    }
                }
                var deletingVideo = db.pollContentTbls.Where(c => c.pollID == pollID && c.contentType == 2);
               
                db.SubmitChanges();

                db.pollTagTbls.DeleteAllOnSubmit(db.pollTagTbls.Where(c => c.pollID == pollID));
                db.SubmitChanges();

                foreach (var item in tags)
                {
                    var tag = new pollTagTbl();
                    tag.pollID = poll.ID;
                    tag.tagID = item;

                    db.pollTagTbls.InsertOnSubmit(tag);
                    db.SubmitChanges();
                }


                db.pollSubGroupTbls.DeleteAllOnSubmit(db.pollSubGroupTbls.Where(c => c.pollID == pollID));
                db.SubmitChanges();

                foreach (var item in subgroups)
                {
                    var subpoll = new pollSubGroupTbl();
                    subpoll.pollID = poll.ID;
                    subpoll.subGroupID = item;

                    db.pollSubGroupTbls.InsertOnSubmit(subpoll);
                    db.SubmitChanges();
                }

                foreach (var item in alloptions)
                {
                    if(item.Contains("no_"))
                    {
                        var optio = new optionTbl();
                        optio.pollID = pollID;
                        optio.regDate = dt;
                        optio.text = context.Request.Form[item];
                        db.optionTbls.InsertOnSubmit(optio);
                        db.SubmitChanges();
                    }
                }
                foreach (var item in alloptions)
                {
                    if (item.Contains("lo_"))
                    {
                        var tbl = db.optionTbls.Single(c => c.ID == long.Parse(item.Replace("lo_", "")));
                        tbl.text = context.Request.Form[item];
                        db.SubmitChanges();
                    }
                }
                foreach (var item in list)
                {
                    if (item.Contains("par_") && !item.Contains("lpar_"))
                    {
                        var tbl = new pollContentTbl();
                        tbl.value = context.Request.Form[item];
                        tbl.regDate = dt;
                        tbl.pollID = poll.ID;
                        tbl.pri = int.Parse(item.Replace("par_", "").Replace("lpar_", ""));//contentedit
                        tbl.contentType = 0;
                        db.pollContentTbls.InsertOnSubmit(tbl);
                        db.SubmitChanges();
                    }
                    else if (item.Contains("img_") && !item.Contains("limg_"))
                    {

                        HttpPostedFile file = context.Request.Files[item];
                        var ext = "." + file.ContentType.Split('/')[1];
                        var imagename = "appinno" + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N") + ext;
                        string fname = context.Server.MapPath(ClassCollection.Methods.getPollImagesPath() + imagename);
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

                        var tbl = new pollContentTbl();
                        tbl.value = imagename;
                        tbl.regDate = dt;
                        tbl.pollID = poll.ID;
                        tbl.pri = int.Parse(item.Replace("img_", "").Replace("limg_", ""));//contentedit
                        tbl.contentType = 1;
                        db.pollContentTbls.InsertOnSubmit(tbl);
                        db.SubmitChanges();

                    }
                   
                }

                foreach (var item in list)
                {
                    if (item.Contains("lpar_") )
                    {
                        var tbl = db.pollContentTbls.Single(c => c.ID == long.Parse(context.Request.Form[item]));
                        tbl.pri = int.Parse(item.Replace("lpar_", "").Replace("par_", ""));
                        db.SubmitChanges();
                    }
                    else if (item.Contains("limg_") )
                    {
                        var tbl = db.pollContentTbls.Single(c => c.ID == long.Parse(context.Request.Form[item]));
                        tbl.pri = int.Parse(item.Replace("limg_", "").Replace("img_", ""));
                        db.SubmitChanges();
                    }
                    else if (item.Contains("lvid_") )
                    {
                        var tbl = db.pollContentTbls.Single(c => c.ID == long.Parse(context.Request.Form[item]));
                        tbl.pri = int.Parse(item.Replace("lvid_", "").Replace("vid_", ""));
                        //contentedit-end
                        db.SubmitChanges();
                    }
                }
                if(poll.isBlock==false && poll.isFinished==false)
                {
                    string pv = "";
                    if(poll.pollContentTbls.Any(c => c.contentType == 0))
                    {
                        var pc = poll.pollContentTbls.Where(c => c.contentType == 0).Take(1).Single();
                        pc.value = pc.value.Replace("<p>", "").Replace("</p>", "").Replace("<h2>", "").Replace("<h2/>", "").Replace("<br>", "").Replace("<br />", "").Replace("<br/>", "");
                        pv = pc.value.Substring(0, Math.Min(pc.value.Length, 40));
                       
                    }
                    else
                    {
                        pv = "نظرسنجی جدید";
                    }
                    new service.pushservice().XsendPush("1", "poll", poll.ID + "", pv, "toall", "false");
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