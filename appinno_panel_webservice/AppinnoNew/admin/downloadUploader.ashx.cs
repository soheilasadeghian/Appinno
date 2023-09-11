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
    public class downloadUploader : IHttpHandler, IRequiresSessionState
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
                if (permission.download.access.insert == false)
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

                var download = new downloadTbl();
                var dt = new DateTime();
                dt = DateTime.Now;
                download.title = title;
                download.isBlock = true;
                download.isPublished = false;
                download.userID = (HttpContext.Current.Session["user"] as userTbl).ID;
                var dateArr = pubdate.Split('/');
                download.publishDate = Persia.Calendar.ConvertToGregorian(int.Parse(dateArr[0]), int.Parse(dateArr[1]), int.Parse(dateArr[2]), Persia.DateType.Persian);
                download.regDate = dt;
                db.downloadTbls.InsertOnSubmit(download);
                db.SubmitChanges();

                foreach (var item in subgroups)
                {
                    var subdownload = new downloadSubGroupTbl();
                    subdownload.downloadID = download.ID;
                    subdownload.subGroupID = item;

                    db.downloadSubGroupTbls.InsertOnSubmit(subdownload);
                    db.SubmitChanges();
                }
                foreach (var item in tags)
                {
                    var tag = new downloadTagTbl();
                    tag.downloadID = download.ID;
                    tag.tagID = item;

                    db.downloadTagTbls.InsertOnSubmit(tag);
                    db.SubmitChanges();
                }

                foreach (var item in list)
                {
                    if (item.Contains("par_"))
                    {
                        var tbl = new downloadContentTbl();
                        tbl.value = context.Request.Form[item];
                        tbl.regDate = dt;
                        tbl.downloadID = download.ID;
                        tbl.contentType = 0;
                        db.downloadContentTbls.InsertOnSubmit(tbl);
                        db.SubmitChanges();
                    }
                    else if (item.Contains("img_"))
                    {

                        HttpPostedFile file = context.Request.Files[item];
                        var ext = "." + file.ContentType.Split('/')[1];
                        var imagename = "appinno" + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N") + ext;
                        string fname = context.Server.MapPath(ClassCollection.Methods.getdownloadImagesPath() + imagename);
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

                        var tbl = new downloadContentTbl();
                        tbl.value = imagename;
                        tbl.regDate = dt;
                        tbl.downloadID = download.ID;
                        tbl.contentType = 1;
                        db.downloadContentTbls.InsertOnSubmit(tbl);
                        db.SubmitChanges();

                    }
                    else if (item.Contains("vid_"))
                    {
                        HttpPostedFile file = context.Request.Files[item];
                        var ext = "." + file.ContentType.Split('/')[1];
                        var imagename = "appinno" + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N") + ext;
                        string fname = context.Server.MapPath(ClassCollection.Methods.getdownloadFilePath() + imagename);
                        try
                        {
                            file.SaveAs(fname);
                        }
                        catch (Exception e)
                        {
                            context.Response.ContentType = "text/plain";
                            context.Response.Write(e.Message + "save problem");

                        }

                        var tbl = new downloadContentTbl();
                        tbl.value = imagename;
                        tbl.regDate = dt;
                        tbl.downloadID = download.ID;
                       
                        tbl.fileSize = file.ContentLength / 1024;
                        tbl.fileType = ext;
                        tbl.contentType = 2;
                        tbl.downloadCount = 0;
                        db.downloadContentTbls.InsertOnSubmit(tbl);
                        db.SubmitChanges();

                    }
                }
               if(download.isBlock==false && download.isPublished==false)
                {
                    download.isPublished = true;
                    db.SubmitChanges();
                    new service.pushservice().XsendPush("1", "download", download.ID + "", download.title, "toall", "false");
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
                    long s = long.Parse(context.Request.Form["downloadID"].ToString());
                }
                catch
                {

                }
                if (permission.download.access.edit == false)
                {
                    result.code = 2;
                    result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                    context.Response.Write(new JavaScriptSerializer().Serialize(result));
                    return;
                }
                long downloadID = long.Parse(context.Request.Form["downloadID"].ToString());
                if (!db.downloadTbls.Any(c => c.ID == downloadID))
                {
                    result.code = 3;
                    result.message = ClassCollection.Message.DOWNLOAD_NOT_EXIST;
                    context.Response.Write(new JavaScriptSerializer().Serialize(result));
                    return;
                }

                var download = db.downloadTbls.Single(c => c.ID == downloadID);
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
                var allp = context.Request.Form.AllKeys.Where(c => c.Contains("par_") || c.Contains("lpar_") || c.Contains("limg_") || c.Contains("lvid_"));
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

                var dt = new DateTime();
                dt = DateTime.Now;
                download.title = title;
                download.isBlock = block;
                if (!download.mUserID.HasValue)
                    download.userID = (HttpContext.Current.Session["user"] as userTbl).ID;
                var dateArr = pubdate.Split('/');
                download.publishDate = Persia.Calendar.ConvertToGregorian(int.Parse(dateArr[0]), int.Parse(dateArr[1]), int.Parse(dateArr[2]), Persia.DateType.Persian);
                download.regDate = dt;

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
                var deletingPar = db.downloadContentTbls.Where(c => c.downloadID == downloadID && c.contentType == 0);
                foreach (var item in deletingPar)
                {
                    if (!old_Par_Item.Contains(item.ID))
                    {
                        db.downloadContentTbls.DeleteOnSubmit(item);
                    }
                }
                var deletingImg = db.downloadContentTbls.Where(c => c.downloadID == downloadID && c.contentType == 1);
                foreach (var item in deletingImg)
                {
                    if (!old_image_Item.Contains(item.ID))
                    {

                        var img = item.value;

                        if (System.IO.File.Exists(context.Server.MapPath(ClassCollection.Methods.getdownloadImagesPath() + img)))
                        {
                            System.IO.File.Delete(context.Server.MapPath(ClassCollection.Methods.getdownloadImagesPath() + img));
                        }
                        db.downloadContentTbls.DeleteOnSubmit(item);
                    }
                }
                var deletingVideo = db.downloadContentTbls.Where(c => c.downloadID == downloadID && c.contentType == 2);
                foreach (var item in deletingVideo)
                {
                    if (!old_video_Item.Contains(item.ID))
                    {
                        db.downloadContentTbls.DeleteOnSubmit(item);
                        var vid = item.value;

                        if (System.IO.File.Exists(context.Server.MapPath(ClassCollection.Methods.getdownloadFilePath() + vid)))
                        {
                            System.IO.File.Delete(context.Server.MapPath(ClassCollection.Methods.getdownloadFilePath() + vid));
                        }
                        db.downloadContentTbls.DeleteOnSubmit(item);
                    }
                }
                download.userID = user.ID;
                db.SubmitChanges();

                db.downloadTagTbls.DeleteAllOnSubmit(db.downloadTagTbls.Where(c => c.downloadID == downloadID));
                db.SubmitChanges();

                foreach (var item in tags)
                {
                    var tag = new downloadTagTbl();
                    tag.downloadID = download.ID;
                    tag.tagID = item;

                    db.downloadTagTbls.InsertOnSubmit(tag);
                    db.SubmitChanges();
                }


                db.downloadSubGroupTbls.DeleteAllOnSubmit(db.downloadSubGroupTbls.Where(c => c.downloadID == downloadID));
                db.SubmitChanges();

                foreach (var item in subgroups)
                {
                    var subdownload = new downloadSubGroupTbl();
                    subdownload.downloadID = download.ID;
                    subdownload.subGroupID = item;

                    db.downloadSubGroupTbls.InsertOnSubmit(subdownload);
                    db.SubmitChanges();
                }

                foreach (var item in list)
                {
                    if (item.Contains("par_") && !item.Contains("lpar_"))
                    {
                        var tbl = new downloadContentTbl();
                        tbl.value = context.Request.Form[item];
                        tbl.regDate = dt;
                        tbl.downloadID = download.ID;
                        tbl.contentType = 0;
                        tbl.pri = int.Parse(item.Replace("par_", "").Replace("lpar_", ""));//contentedit
                        db.downloadContentTbls.InsertOnSubmit(tbl);
                        db.SubmitChanges();
                    }
                    else if (item.Contains("img_") && !item.Contains("limg_"))
                    {

                        HttpPostedFile file = context.Request.Files[item];
                        var ext = "." + file.ContentType.Split('/')[1];
                        var imagename = "appinno" + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N") + ext;
                        string fname = context.Server.MapPath(ClassCollection.Methods.getdownloadImagesPath() + imagename);
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

                        var tbl = new downloadContentTbl();
                        tbl.value = imagename;
                        tbl.regDate = dt;
                        tbl.downloadID = download.ID;
                        tbl.pri = int.Parse(item.Replace("img_", "").Replace("limg_", ""));//contentedit
                        tbl.contentType = 1;
                        db.downloadContentTbls.InsertOnSubmit(tbl);
                        db.SubmitChanges();

                    }
                    else if (item.Contains("vid_") && !item.Contains("lvid_"))
                    {
                        HttpPostedFile file = context.Request.Files[item];
                        var ext = "." + file.ContentType.Split('/')[1];
                        var imagename = "appinno" + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N") + ext;
                        string fname = context.Server.MapPath(ClassCollection.Methods.getdownloadFilePath() + imagename);
                        try
                        {
                            file.SaveAs(fname);
                        }
                        catch (Exception e)
                        {
                            context.Response.ContentType = "text/plain";
                            context.Response.Write(e.Message + "save problem");
                        }

                        var tbl = new downloadContentTbl();
                        tbl.value = imagename;
                        tbl.regDate = dt;
                        tbl.downloadID = download.ID;
                        tbl.pri = int.Parse(item.Replace("vid_", "").Replace("lvid_", ""));//contentedit
                        tbl.fileType = ext;
                        tbl.fileSize = file.ContentLength / 1024;
                        tbl.downloadCount = 0;
                        tbl.contentType = 2;
                        db.downloadContentTbls.InsertOnSubmit(tbl);
                        //contentedit-start
                        db.SubmitChanges();

                    }
                }

                foreach (var item in list)
                {
                    if (item.Contains("lpar_"))
                    {
                        var tbl = db.downloadContentTbls.Single(c => c.ID == long.Parse(context.Request.Form[item]));
                        tbl.pri = int.Parse(item.Replace("lpar_", "").Replace("par_", ""));
                        db.SubmitChanges();
                    }
                    else if (item.Contains("limg_"))
                    {
                        var tbl = db.downloadContentTbls.Single(c => c.ID == long.Parse(context.Request.Form[item]));
                        tbl.pri = int.Parse(item.Replace("limg_", "").Replace("img_", ""));
                        db.SubmitChanges();
                    }
                    else if (item.Contains("lvid_"))
                    {
                        var tbl = db.downloadContentTbls.Single(c => c.ID == long.Parse(context.Request.Form[item]));
                        tbl.pri = int.Parse(item.Replace("lvid_", "").Replace("vid_", ""));
                        //contentedit-end
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