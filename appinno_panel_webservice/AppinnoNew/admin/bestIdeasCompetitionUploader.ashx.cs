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
    public class bestIdeasCompetitionUploader : IHttpHandler, IRequiresSessionState
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
            DateTime dp = new DateTime();
            dp = DateTime.Now;


            #region insert

            if (context.Request.QueryString["mode"] == "n")
            {
                if (permission.bestIdeasCompetition.access.insert == false)
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
                var startDate = context.Request.Form["startDate"];
                var endDate = context.Request.Form["endDate"];
                var resultVoteDate = context.Request.Form["resultVoteDate"];
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

                var dt = new DateTime();
                dt = DateTime.Now;
                var startDateArr = startDate.Split('/');
                var sD = Persia.Calendar.ConvertToGregorian(int.Parse(startDateArr[0]), int.Parse(startDateArr[1]), int.Parse(startDateArr[2]), Persia.DateType.Persian);
                var endDateArr = endDate.Split('/');
                var eD = Persia.Calendar.ConvertToGregorian(int.Parse(endDateArr[0]), int.Parse(endDateArr[1]), int.Parse(endDateArr[2]), Persia.DateType.Persian);
                var resultVoteDateArr = resultVoteDate.Split('/');
                var vD = Persia.Calendar.ConvertToGregorian(int.Parse(resultVoteDateArr[0]), int.Parse(resultVoteDateArr[1]), int.Parse(resultVoteDateArr[2]), Persia.DateType.Persian);
                if (!(sD.Date <= eD.Date) || !(sD.Date <= vD.Date) || !(eD.Date <= vD.Date))
                {
                    result.code = 3;
                    result.message = ClassCollection.Message.DATE_INCORRECT_PRIORITY;
                    context.Response.Write(new JavaScriptSerializer().Serialize(result));
                    return;
                }
                if(!(dt.Date <= sD.Date))
                {
                    result.code = 4;
                    result.message = ClassCollection.Message.DATE_INCORRECT_NOW;
                    context.Response.Write(new JavaScriptSerializer().Serialize(result));
                    return;
                }
                
                var bestIdeasCompetition = new bestIdeaCompetitionsTbl();
                bestIdeasCompetition.title = title;
                bestIdeasCompetition.isBlock = true;
                
                bestIdeasCompetition.creatorID = (HttpContext.Current.Session["user"] as userTbl).ID;
                bestIdeasCompetition.startDate = sD;
                bestIdeasCompetition.endDate = eD;
                bestIdeasCompetition.resultVoteDate = vD;
                bestIdeasCompetition.regDate = dt;
                db.bestIdeaCompetitionsTbls.InsertOnSubmit(bestIdeasCompetition);
                db.SubmitChanges();

                
                foreach (var item in tags)
                {
                    var tag = new bestIdeaCompetitionsTagTbl();
                    tag.bestIdeaCompetitionsID = bestIdeasCompetition.ID;
                    tag.tagID = item;

                    db.bestIdeaCompetitionsTagTbls.InsertOnSubmit(tag);
                    db.SubmitChanges();
                }

                foreach (var item in list)
                {
                    if (item.Contains("par_"))
                    {
                        var tbl = new bestIdeaCompetitionsContentTbl();
                        tbl.value = context.Request.Form[item];
                        tbl.regDate = dt;
                        tbl.bestIdeaCompetitionsID = bestIdeasCompetition.ID;
                        tbl.contentType = 0;
                        db.bestIdeaCompetitionsContentTbls.InsertOnSubmit(tbl);
                        db.SubmitChanges();
                    }
                    else if (item.Contains("img_"))
                    {

                        HttpPostedFile file = context.Request.Files[item];
                        var ext = "." + file.ContentType.Split('/')[1];
                        var imagename = "appinno" + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N") + ext;
                        string fname = context.Server.MapPath(ClassCollection.Methods.getBestIdeaCompetitionsImagesPath() + imagename);
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

                        var tbl = new bestIdeaCompetitionsContentTbl();
                        tbl.value = imagename;
                        tbl.regDate = dt;
                        tbl.bestIdeaCompetitionsID = bestIdeasCompetition.ID;
                        tbl.contentType = 1;
                        db.bestIdeaCompetitionsContentTbls.InsertOnSubmit(tbl);
                        db.SubmitChanges();

                    }
                    else if (item.Contains("vid_"))
                    {
                        HttpPostedFile file = context.Request.Files[item];
                        var ext = "." + file.ContentType.Split('/')[1];
                        var imagename = "appinno" + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N") + ext;
                        string fname = context.Server.MapPath(ClassCollection.Methods.getBestIdeaCompetitionsVideosPath() + imagename);
                        try
                        {
                            file.SaveAs(fname);
                        }
                        catch (Exception e)
                        {
                            context.Response.ContentType = "text/plain";
                            context.Response.Write(e.Message + "save problem");
                        }

                        var tbl = new bestIdeaCompetitionsContentTbl();
                        tbl.value = imagename;
                        tbl.regDate = dt;
                        tbl.bestIdeaCompetitionsID = bestIdeasCompetition.ID;
                        tbl.contentType = 2;
                        db.bestIdeaCompetitionsContentTbls.InsertOnSubmit(tbl);
                        db.SubmitChanges();
                    }
                }

                //new service.pushservice().XsendPush("1", "news", news.ID + "", news.title, "all", "false");
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
                    long s = long.Parse(context.Request.Form["bestIdeasCompetitionID"].ToString());
                }
                catch
                {

                }
                if (permission.bestIdeasCompetition.access.edit == false)
                {
                    result.code = 2;
                    result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                    context.Response.Write(new JavaScriptSerializer().Serialize(result));
                    return;
                }
                long bestIdeasCompetitionID = long.Parse(context.Request.Form["bestIdeasCompetitionID"].ToString());
                if (!db.bestIdeaCompetitionsTbls.Any(c => c.ID == bestIdeasCompetitionID))
                {
                    result.code = 3;
                    result.message = ClassCollection.Message.COMPETITIONS_NOT_EXIST;
                    context.Response.Write(new JavaScriptSerializer().Serialize(result));
                    return;
                }

                var bestIdeaCompetitions = db.bestIdeaCompetitionsTbls.Single(c => c.ID == bestIdeasCompetitionID);

                //if (!(dp < bestIdeaCompetitions.startDate))
                //{
                //    result.code = 3;
                //    result.message = ClassCollection.Message.EDIT_INCORRECT;
                //    context.Response.Write(new JavaScriptSerializer().Serialize(result));
                //    return;
                //}

                if (db.ideasTbls.Any(c => c.bestIdeaCompetitionsID == bestIdeasCompetitionID))
                {
                    result.code = 3;
                    result.message = ClassCollection.Message.EDIT_INCORRECT;
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
                var startDate = context.Request.Form["startDate"];
                var endDate = context.Request.Form["endDate"];
                var resultVoteDate = context.Request.Form["resultVoteDate"];
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
                var startDateArr = startDate.Split('/');
                var sD = Persia.Calendar.ConvertToGregorian(int.Parse(startDateArr[0]), int.Parse(startDateArr[1]), int.Parse(startDateArr[2]), Persia.DateType.Persian);
                var endDateArr = endDate.Split('/');
                var eD = Persia.Calendar.ConvertToGregorian(int.Parse(endDateArr[0]), int.Parse(endDateArr[1]), int.Parse(endDateArr[2]), Persia.DateType.Persian);
                var resultVoteDateArr = resultVoteDate.Split('/');
                var vD = Persia.Calendar.ConvertToGregorian(int.Parse(resultVoteDateArr[0]), int.Parse(resultVoteDateArr[1]), int.Parse(resultVoteDateArr[2]), Persia.DateType.Persian);
                if (!(sD.Date <= eD.Date) || !(sD.Date <= vD.Date) || !(eD.Date <= vD.Date))
                {
                    result.code = 3;
                    result.message = ClassCollection.Message.DATE_INCORRECT_PRIORITY;
                    context.Response.Write(new JavaScriptSerializer().Serialize(result));
                    return;
                }
                
                
                bestIdeaCompetitions.title = title;
                bestIdeaCompetitions.isBlock = block;
                bestIdeaCompetitions.creatorID = (HttpContext.Current.Session["user"] as userTbl).ID;
                bestIdeaCompetitions.startDate = sD;
                bestIdeaCompetitions.endDate = eD;
                bestIdeaCompetitions.resultVoteDate = vD;
                bestIdeaCompetitions.regDate = dt;

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
                var deletingPar = db.bestIdeaCompetitionsContentTbls.Where(c => c.bestIdeaCompetitionsID == bestIdeasCompetitionID && c.contentType == 0);
                foreach (var item in deletingPar)
                {
                    if (!old_Par_Item.Contains(item.ID))
                    {
                        db.bestIdeaCompetitionsContentTbls.DeleteOnSubmit(item);
                    }
                }
                var deletingImg = db.bestIdeaCompetitionsContentTbls.Where(c => c.bestIdeaCompetitionsID == bestIdeasCompetitionID && c.contentType == 1);
                foreach (var item in deletingImg)
                {
                    if (!old_image_Item.Contains(item.ID))
                    {

                        var img = item.value;

                        if (System.IO.File.Exists(context.Server.MapPath(ClassCollection.Methods.getBestIdeaCompetitionsImagesPath() + img)))
                        {
                            System.IO.File.Delete(context.Server.MapPath(ClassCollection.Methods.getBestIdeaCompetitionsImagesPath() + img));
                        }
                        db.bestIdeaCompetitionsContentTbls.DeleteOnSubmit(item);
                    }
                }
                var deletingVideo = db.bestIdeaCompetitionsContentTbls.Where(c => c.bestIdeaCompetitionsID == bestIdeasCompetitionID && c.contentType == 2);
                foreach (var item in deletingVideo)
                {
                    if (!old_video_Item.Contains(item.ID))
                    {
                        db.bestIdeaCompetitionsContentTbls.DeleteOnSubmit(item);
                        var vid = item.value;

                        if (System.IO.File.Exists(context.Server.MapPath(ClassCollection.Methods.getBestIdeaCompetitionsVideosPath() + vid)))
                        {
                            System.IO.File.Delete(context.Server.MapPath(ClassCollection.Methods.getBestIdeaCompetitionsVideosPath() + vid));
                        }
                        db.bestIdeaCompetitionsContentTbls.DeleteOnSubmit(item);
                    }
                }
                db.SubmitChanges();

                db.bestIdeaCompetitionsTagTbls.DeleteAllOnSubmit(db.bestIdeaCompetitionsTagTbls.Where(c => c.bestIdeaCompetitionsID == bestIdeasCompetitionID));
                db.SubmitChanges();

                foreach (var item in tags)
                {
                    var tag = new bestIdeaCompetitionsTagTbl();
                    tag.bestIdeaCompetitionsID = bestIdeaCompetitions.ID;
                    tag.tagID = item;

                    db.bestIdeaCompetitionsTagTbls.InsertOnSubmit(tag);
                    db.SubmitChanges();
                }

                foreach (var item in list)
                {
                    if (item.Contains("par_") && !item.Contains("lpar_"))
                    {
                        var tbl = new bestIdeaCompetitionsContentTbl();
                        tbl.value = context.Request.Form[item];
                        tbl.regDate = dt;
                        tbl.bestIdeaCompetitionsID = bestIdeaCompetitions.ID;
                        tbl.pri = int.Parse(item.Replace("par_", "").Replace("lpar_", ""));//contentedit
                        tbl.contentType = 0;
                        db.bestIdeaCompetitionsContentTbls.InsertOnSubmit(tbl);
                        db.SubmitChanges();
                    }
                    else if (item.Contains("img_") && !item.Contains("limg_"))
                    {
                        HttpPostedFile file = context.Request.Files[item];
                        var ext = "." + file.ContentType.Split('/')[1];
                        var imagename = "appinno" + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N") + ext;
                        string fname = context.Server.MapPath(ClassCollection.Methods.getBestIdeaCompetitionsImagesPath() + imagename);
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

                        var tbl = new bestIdeaCompetitionsContentTbl();
                        tbl.value = imagename;
                        tbl.regDate = dt;
                        tbl.bestIdeaCompetitionsID = bestIdeaCompetitions.ID;
                        tbl.pri = int.Parse(item.Replace("img_", "").Replace("limg_", ""));//contentedit
                        tbl.contentType = 1;
                        db.bestIdeaCompetitionsContentTbls.InsertOnSubmit(tbl);
                        db.SubmitChanges();

                    }
                    else if (item.Contains("vid_") && !item.Contains("lvid_"))
                    {
                        HttpPostedFile file = context.Request.Files[item];
                        var ext = "." + file.ContentType.Split('/')[1];
                        var imagename = "appinno" + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N") + ext;
                        string fname = context.Server.MapPath(ClassCollection.Methods.getBestIdeaCompetitionsVideosPath() + imagename);
                        try
                        {
                            file.SaveAs(fname);
                        }
                        catch (Exception e)
                        {
                            context.Response.ContentType = "text/plain";
                            context.Response.Write(e.Message + "save problem");
                        }

                        var tbl = new bestIdeaCompetitionsContentTbl();
                        tbl.value = imagename;
                        tbl.regDate = dt;
                        tbl.bestIdeaCompetitionsID = bestIdeaCompetitions.ID;
                        tbl.pri = int.Parse(item.Replace("vid_", "").Replace("lvid_", ""));//contentedit
                        tbl.contentType = 2;
                        db.bestIdeaCompetitionsContentTbls.InsertOnSubmit(tbl);
                        db.SubmitChanges();

                    }
                }

                foreach (var item in list)
                {
                    if (item.Contains("lpar_") )
                    {
                        var tbl = db.bestIdeaCompetitionsContentTbls.Single(c => c.ID == long.Parse(context.Request.Form[item]));
                        tbl.pri = int.Parse(item.Replace("lpar_", "").Replace("par_", ""));
                        db.SubmitChanges();
                    }
                    else if (item.Contains("limg_") )
                    {
                        var tbl = db.bestIdeaCompetitionsContentTbls.Single(c => c.ID == long.Parse(context.Request.Form[item]));
                        tbl.pri = int.Parse(item.Replace("limg_", "").Replace("img_", ""));
                        db.SubmitChanges();
                    }
                    else if (item.Contains("lvid_") )
                    {
                        var tbl = db.bestIdeaCompetitionsContentTbls.Single(c => c.ID == long.Parse(context.Request.Form[item]));
                        tbl.pri = int.Parse(item.Replace("lvid_", "").Replace("vid_", ""));
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