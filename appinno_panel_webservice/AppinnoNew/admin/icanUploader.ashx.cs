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
    public class icanUploader : IHttpHandler, IRequiresSessionState
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
            
            #region edit

            if (context.Request.QueryString["mode"] == "e")
            {
                try
                {
                    long s = long.Parse(context.Request.Form["icanID"].ToString());
                }
                catch
                {

                }
                if (permission.ican.access.edit == false)
                {
                    result.code = 3;
                    result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                    context.Response.Write(new JavaScriptSerializer().Serialize(result));
                    return;
                }
                long icanID = long.Parse(context.Request.Form["icanID"].ToString());
                if (!db.icanTbls.Any(c => c.ID == icanID))
                {
                    result.code = 4;
                    result.message = ClassCollection.Message.ICAN_NOT_EXIST;
                    context.Response.Write(new JavaScriptSerializer().Serialize(result));
                    return;
                }

                var ican = db.icanTbls.Single(c => c.ID == icanID);

                var title = context.Request.Form["title"];
                var block = bool.Parse(context.Request.Form["block"]);
                var allp = context.Request.Form.AllKeys.Where(c => c.Contains("par_") || c.Contains("lpar_") || c.Contains("limg_") || c.Contains("lvid_") || c.Contains("lfile_"));
                var allvi = context.Request.Files.AllKeys.Where(c => (c.Contains("img_") && !c.Contains("limg_")) || (c.Contains("vid_") && !c.Contains("lvid_")) || (c.Contains("file_") && !c.Contains("lfile_")));
                var list = new List<string>();
                list.AddRange(allp);
                list.AddRange(allvi);
                list.Sort(delegate(string p1, string p2)
                {
                    try
                    {
                        var p1Index = long.Parse(p1.Replace("img_", "").Replace("vid_", "").Replace("par_", "").Replace("file_", "")
                            .Replace("lvid_", "").Replace("limg_", "").Replace("lpar_", "").Replace("lfile_", ""));

                        var p2Index = long.Parse(p2.Replace("img_", "").Replace("vid_", "").Replace("par_", "").Replace("file_", "")
                            .Replace("lvid_", "").Replace("limg_", "").Replace("lpar_", "").Replace("lfile_", ""));

                        return p1Index.CompareTo(p2Index);
                    }
                    catch
                    {
                        return 1;
                    }

                });

                var dt = new DateTime();
                dt = DateTime.Now;
                ican.title = title;
                ican.isBlock = block;
                //ican.creatorID = (HttpContext.Current.Session["user"] as userTbl).ID;
                ican.regDate = dt;

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

                List<long> old_file_Item = new List<long>();
                foreach (var item in list)
                {
                    long id = -1;
                    if (item.Contains("lfile_"))
                    {
                        id = long.Parse(context.Request.Form[item]);
                        old_file_Item.Add(id);
                    }

                }

                var deletingPar = db.icanContentTbls.Where(c => c.icanID == icanID && c.contentType == 0);
                foreach (var item in deletingPar)
                {
                    if (!old_Par_Item.Contains(item.ID))
                    {
                        db.icanContentTbls.DeleteOnSubmit(item);
                    }
                }
                var deletingImg = db.icanContentTbls.Where(c => c.icanID == icanID && c.contentType == 1);
                foreach (var item in deletingImg)
                {
                    if (!old_image_Item.Contains(item.ID))
                    {

                        var img = item.value;

                        if (System.IO.File.Exists(context.Server.MapPath(ClassCollection.Methods.geticanImagesPath() + img)))
                        {
                            System.IO.File.Delete(context.Server.MapPath(ClassCollection.Methods.geticanImagesPath() + img));
                        }
                        db.icanContentTbls.DeleteOnSubmit(item);
                    }
                }
                var deletingVideo = db.icanContentTbls.Where(c => c.icanID == icanID && c.contentType == 2);
                foreach (var item in deletingVideo)
                {
                    if (!old_video_Item.Contains(item.ID))
                    {
                        db.icanContentTbls.DeleteOnSubmit(item);
                        var vid = item.value;

                        if (System.IO.File.Exists(context.Server.MapPath(ClassCollection.Methods.geticanVideosPath() + vid)))
                        {
                            System.IO.File.Delete(context.Server.MapPath(ClassCollection.Methods.geticanVideosPath() + vid));
                        }
                        db.icanContentTbls.DeleteOnSubmit(item);
                    }
                }
                var deletingFile = db.icanContentTbls.Where(c => c.icanID == icanID && c.contentType == 3);
                foreach (var item in deletingFile)
                {
                    if (!old_file_Item.Contains(item.ID))
                    {
                        db.icanContentTbls.DeleteOnSubmit(item);
                        var fid = item.value;

                        if (System.IO.File.Exists(context.Server.MapPath(ClassCollection.Methods.geticanFilePath() + fid)))
                        {
                            System.IO.File.Delete(context.Server.MapPath(ClassCollection.Methods.geticanFilePath() + fid));
                        }
                        db.icanContentTbls.DeleteOnSubmit(item);
                    }
                }
                db.SubmitChanges();
                
                foreach (var item in list)
                {
                    if (item.Contains("par_") && !item.Contains("lpar_"))
                    {
                        var tbl = new icanContentTbl();
                        tbl.value = context.Request.Form[item];
                        tbl.regDate = dt;
                        tbl.icanID = ican.ID;
                        tbl.contentType = 0;
                        tbl.pri = int.Parse(item.Replace("par_", "").Replace("lpar_", ""));//contentedit
                        db.icanContentTbls.InsertOnSubmit(tbl);
                        db.SubmitChanges();
                    }
                    else if (item.Contains("img_") && !item.Contains("limg_"))
                    {

                        HttpPostedFile file = context.Request.Files[item];
                        var ext = "." + file.ContentType.Split('/')[1];
                        var imagename = "appinno" + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N") + ext;
                        string fname = context.Server.MapPath(ClassCollection.Methods.geticanImagesPath() + imagename);
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

                        var tbl = new icanContentTbl();
                        tbl.value = imagename;
                        tbl.regDate = dt;
                        tbl.icanID = ican.ID;
                        tbl.pri = int.Parse(item.Replace("img_", "").Replace("limg_", ""));
                        tbl.contentType = 1;
                        db.icanContentTbls.InsertOnSubmit(tbl);
                        db.SubmitChanges();

                    }
                    else if (item.Contains("vid_") && !item.Contains("lvid_"))
                    {
                        HttpPostedFile file = context.Request.Files[item];
                        var ext = "." + file.ContentType.Split('/')[1];
                        var videoname = "appinno" + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N") + ext;
                        string fname = context.Server.MapPath(ClassCollection.Methods.geticanVideosPath() + videoname);
                        try
                        {
                            file.SaveAs(fname);
                        }
                        catch (Exception e)
                        {
                            context.Response.ContentType = "text/plain";
                            context.Response.Write(e.Message + "save problem");
                        }

                        var tbl = new icanContentTbl();
                        tbl.value = videoname;
                        tbl.regDate = dt;
                        tbl.icanID= ican.ID;
                        tbl.pri = int.Parse(item.Replace("vid_", "").Replace("lvid_", ""));
                        tbl.contentType = 2;
                        db.icanContentTbls.InsertOnSubmit(tbl);
                        db.SubmitChanges();

                    }
                    else if (item.Contains("file_") && !item.Contains("lfile_"))
                    {
                        HttpPostedFile file = context.Request.Files[item];
                        var ext = "." + file.ContentType.Split('/')[1];
                        var filename = "appinno" + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N") + ext;
                        string fname = context.Server.MapPath(ClassCollection.Methods.geticanFilePath() + filename);
                        try
                        {
                            file.SaveAs(fname);
                        }
                        catch (Exception e)
                        {
                            context.Response.ContentType = "text/plain";
                            context.Response.Write(e.Message + "save problem");
                        }

                        var tbl = new icanContentTbl();
                        tbl.value = filename;
                        tbl.regDate = dt;
                        tbl.icanID = ican.ID;
                        tbl.pri = int.Parse(item.Replace("file_", "").Replace("lfile_", ""));
                        tbl.contentType = 3;
                        tbl.fileType = ext;
                        tbl.fileSize = file.ContentLength / 1024;
                        tbl.downloadCount = 0;
                        db.icanContentTbls.InsertOnSubmit(tbl);
                        db.SubmitChanges();

                    }
                }

                foreach (var item in list)
                {
                    if (item.Contains("lpar_"))
                    {
                        var tbl = db.icanContentTbls.Single(c => c.ID == long.Parse(context.Request.Form[item]));
                        tbl.pri = int.Parse(item.Replace("lpar_", "").Replace("par_", ""));
                        db.SubmitChanges();
                    }
                    else if (item.Contains("limg_"))
                    {
                        var tbl = db.icanContentTbls.Single(c => c.ID == long.Parse(context.Request.Form[item]));
                        tbl.pri = int.Parse(item.Replace("limg_", "").Replace("img_", ""));
                        db.SubmitChanges();
                    }
                    else if (item.Contains("lvid_"))
                    {
                        var tbl = db.icanContentTbls.Single(c => c.ID == long.Parse(context.Request.Form[item]));
                        tbl.pri = int.Parse(item.Replace("lvid_", "").Replace("vid_", ""));
                        db.SubmitChanges();

                    }
                    else if (item.Contains("lfile_"))
                    {
                        var tbl = db.icanContentTbls.Single(c => c.ID == long.Parse(context.Request.Form[item]));
                        tbl.pri = int.Parse(item.Replace("lfile_", "").Replace("file_", ""));
                        db.SubmitChanges();

                    }
                }

               
                if (db.icanContentTbls.Any(c => c.icanID == icanID  && (c.contentType == 3) ) == true)
                {
                    ican.haveAttachment = true;
                }
                else
                {
                    ican.haveAttachment = false;
                }

                ican.userID = user.ID;
                db.SubmitChanges();
                
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