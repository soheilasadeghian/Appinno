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
    public partial class icanlist : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var db = new DataAccessDataContext();
            if (ClassCollection.Methods.isLogin(Context))
            {
                var user = ClassCollection.Methods.getUserInSession(HttpContext.Current);
                var permission = ClassCollection.Accessing.Deserialize(db.accessTbls.Single(c => c.ID == user.accessID).per);

                if (!permission.ican.access.showlist)
                {
                    Response.Redirect("dashboard.aspx");
                }
            }
            else
            {
                Response.Redirect("login.aspx");
            }
        }
        
        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string getAllIcan(string regDateFrom, string regDateTo, string icanstate, string fillterString, string orderType, 
            string sortby, int pageIndex)
        {
            ClassCollection.Model.IcanAdminResult result = new ClassCollection.Model.IcanAdminResult();
            result.result = new ClassCollection.Model.Result();
            const int pageSize = 30;

            int skipCount = (pageIndex - 1) * pageSize;

            if (!ClassCollection.Methods.isLogin(HttpContext.Current))
            {
                result.result.code = 1;
                result.result.message = ClassCollection.Message.LOGIN_FIRST;
                return new JavaScriptSerializer().Serialize(result);
            }

            var db = new DataAccessDataContext();
            var user = ClassCollection.Methods.getUserInSession(HttpContext.Current);
            var permission = ClassCollection.Accessing.Deserialize(db.accessTbls.Single(c => c.ID == user.accessID).per);

            if (!permission.ican.access.showlist)
            {
                result.result.code = 2;
                result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }

            #region publish regDate

            var regDatefromD = new DateTime();
            var regDatetoD = new DateTime();
            if (regDateFrom != "")
            {
                try
                {
                    var tt = regDateFrom.Split('/');
                    regDatefromD = Persia.Calendar.ConvertToGregorian(int.Parse(tt[0]), int.Parse(tt[1]), int.Parse(tt[2]), Persia.DateType.Persian);
                }
                catch
                {
                    result.result.code = 3;
                    result.result.message = ClassCollection.Message.DATE_INCORRECT;
                    return new JavaScriptSerializer().Serialize(result);
                }
            }
            if (regDateTo != "")
            {
                try
                {
                    var tt = regDateTo.Split('/');
                    regDatetoD = Persia.Calendar.ConvertToGregorian(int.Parse(tt[0]), int.Parse(tt[1]), int.Parse(tt[2]), Persia.DateType.Persian);
                }
                catch
                {
                    result.result.code = 3;
                    result.result.message = ClassCollection.Message.DATE_INCORRECT;
                    return new JavaScriptSerializer().Serialize(result);
                }
            }
            if (regDateTo != "" && regDateFrom != "")
            {
                if (regDatefromD.CompareTo(regDatetoD) > 0)
                {
                    result.result.code = 3;
                    result.result.message = ClassCollection.Message.DATE_INCORRECT;
                    return new JavaScriptSerializer().Serialize(result);
                }
            }
            #endregion

            var us = db.icanTbls.Where(c => c.icanContentTbls.Any()).AsQueryable();

            if (regDateTo != "")
            {
                us = us.Where(c => c.regDate <= regDatetoD);
            }
            if (regDateFrom != "")
            {
                us = us.Where(c => c.regDate >= regDatefromD);
            }
            if (icanstate == "block")
            {
                us = us.Where(c => c.isBlock == true);
            }
            else if (icanstate == "unblock")
            {
                us = us.Where(c => c.isBlock == false);
            }

            #region search

            if (fillterString != "")
            {
                var search = new Search(fillterString);
                var ors = search.getOR;

                var predicate = PredicateBuilder.False<icanTbl>();
                foreach (string keyword in ors)
                {
                    string temp = keyword;
                    predicate = predicate.Or(p => p.title.Contains(keyword) || p.icanContentTbls.Any(c=>c.contentType==0 && c.value.Contains(keyword)));
                }

                us = us.Where(predicate);
            }

            if (orderType == "a")
            {
                switch (sortby)
                {
                    case "reg":
                        us = us.OrderBy(c => c.regDate);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (sortby)
                {
                    case "reg":
                        us = us.OrderByDescending(c => c.regDate);
                        break;
                    default:
                        break;
                }
            }

            var acList = us.ToList();

            var count = acList.Count();
            result.totalCount = count;
            acList = acList.Skip(skipCount).Take(pageSize).ToList();

            #endregion

            result.Ican = new List<Model.IcanAdmin>();
            foreach (var item in acList)
            {
                var tmp = new ClassCollection.Model.IcanAdmin();
                tmp.ID = item.ID;
                tmp.title = item.title;
                tmp.isBlock = item.isBlock;
                tmp.userNameFamily = item.mUserTbl.name + " " + item.mUserTbl.family;
                tmp.regDate = Persia.Calendar.ConvertToPersian(item.regDate).ToString("m") + " - " + Persia.Calendar.ConvertToPersian(item.regDate).ToString("H");
                result.Ican.Add(tmp);
            }

            if (count % pageSize == 0)
                count = count / pageSize;
            else
                count = (count / pageSize) + 1;
            
            result.pageCount = count;
            result.result.code = 0;
            result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
        }

        
        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string deleteIcan(long icanID)
        {
            ClassCollection.Model.Result result = new ClassCollection.Model.Result();

            if (!ClassCollection.Methods.isLogin(HttpContext.Current))
            {
                result.code = 1;
                result.message = ClassCollection.Message.LOGIN_FIRST;
                return new JavaScriptSerializer().Serialize(result);
            }

            var db = new DataAccessDataContext();
            var user = ClassCollection.Methods.getUserInSession(HttpContext.Current);
            var permission = ClassCollection.Accessing.Deserialize(db.accessTbls.Single(c => c.ID == user.accessID).per);

            if (!permission.ican.access.delete)
            {
                result.code = 2;
                result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }
            if (db.icanTbls.Any(c => c.ID == icanID) == false)
            {
                result.code = 2;
                result.message = ClassCollection.Message.NEWS_NOT_EXIST;
                return new JavaScriptSerializer().Serialize(result);
            }

            var ican = db.icanTbls.Single(c => c.ID == icanID);

            var contents = db.icanContentTbls.Where(c => c.icanID == ican.ID);

            foreach (var item in contents)
            {
                if (item.contentType == 1)//image
                {
                    var image = item.value;
                    if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(ClassCollection.Methods.geticanImagesPath() + image)))
                    {
                        try
                        {
                            System.IO.File.Delete(HttpContext.Current.Server.MapPath(ClassCollection.Methods.geticanImagesPath() + image));
                        }
                        catch
                        {
                            result.code = 3;
                            result.message = ClassCollection.Message.ERROR_DELETE_IMAGE;
                            return new JavaScriptSerializer().Serialize(result);
                        }
                    }


                }
                else if (item.contentType == 2)//video
                {
                    var video = item.value;
                    if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(ClassCollection.Methods.geticanVideosPath() + video)))
                    {
                        try
                        {
                            System.IO.File.Delete(HttpContext.Current.Server.MapPath(ClassCollection.Methods.geticanVideosPath() + video));
                        }
                        catch
                        {
                            result.code = 4;
                            result.message = ClassCollection.Message.ERROR_DELETE_VIDEO;
                            return new JavaScriptSerializer().Serialize(result);
                        }

                    }
                }

                else if (item.contentType == 3)//file
                {
                    var file = item.value;
                    if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(ClassCollection.Methods.geticanFilePath() + file)))
                    {
                        try
                        {
                            System.IO.File.Delete(HttpContext.Current.Server.MapPath(ClassCollection.Methods.geticanFilePath() + file));
                        }
                        catch
                        {
                            result.code = 5;
                            result.message = ClassCollection.Message.ERROR_DELETE_FILE;
                            return new JavaScriptSerializer().Serialize(result);
                        }

                    }
                }
            }


            var comments = db.icanCommentTbls.Where(c => c.icanID == ican.ID);
            db.icanCommentTbls.DeleteAllOnSubmit(comments);
            db.SubmitChanges();

            db.icanContentTbls.DeleteAllOnSubmit(contents);
            db.SubmitChanges();

            db.icanTbls.DeleteOnSubmit(ican);
            db.SubmitChanges();

            result.code = 0;
            result.message = ClassCollection.Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
        }


        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string deleteAllIcans()
        {
            ClassCollection.Model.Result result = new ClassCollection.Model.Result();

            if (!ClassCollection.Methods.isLogin(HttpContext.Current))
            {
                result.code = 1;
                result.message = ClassCollection.Message.LOGIN_FIRST;
                return new JavaScriptSerializer().Serialize(result);
            }

            var db = new DataAccessDataContext();
            var user = ClassCollection.Methods.getUserInSession(HttpContext.Current);
            var permission = ClassCollection.Accessing.Deserialize(db.accessTbls.Single(c => c.ID == user.accessID).per);

            if (!permission.ican.access.delete || !permission.ican.access.showlist)
            {
                result.code = 2;
                result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }

            var icans = db.icanTbls;
            foreach (var item in icans)
            {
                try
                {
                    deleteIcan(item.ID);
                }
                catch { }
            }

            result.code = 0;
            result.message = ClassCollection.Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
        }

        #region getAllGroup comment
        //[System.Web.Services.WebMethod(EnableSession = true)]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public static string getAllGroup()
        //{
        //    ClassCollection.Model.GroupListResult result = new ClassCollection.Model.GroupListResult();
        //    result.result = new ClassCollection.Model.Result();


        //    if (!ClassCollection.Methods.isLogin(HttpContext.Current))
        //    {
        //        result.result.code = 1;
        //        result.result.message = ClassCollection.Message.LOGIN_FIRST;
        //        return new JavaScriptSerializer().Serialize(result);
        //    }

        //    var db = new DataAccessDataContext();
        //    var user = ClassCollection.Methods.getUserInSession(HttpContext.Current);
        //    var permission = ClassCollection.Accessing.Deserialize(db.accessTbls.Single(c => c.ID == user.accessID).per);

        //    var groups = db.GroupTbls.ToList();

        //    result.groups = new List<ClassCollection.Model.Group>();
        //    foreach (var item in groups)
        //    {
        //        var tmp = new ClassCollection.Model.Group();
        //        tmp.ID = item.ID;
        //        tmp.regDate = Persia.Calendar.ConvertToPersian(item.regDate).ToString();
        //        tmp.title = item.title;
        //        result.groups.Add(tmp);
        //    }

        //    result.result.code = 0;
        //    result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
        //    return new JavaScriptSerializer().Serialize(result);
        //}

        


        //[System.Web.Services.WebMethod(EnableSession = true)]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public static string getAllSubGroup(int GroupID)
        //{
        //    ClassCollection.Model.subGroupListResult result = new ClassCollection.Model.subGroupListResult();
        //    result.result = new ClassCollection.Model.Result();

        //    if (!ClassCollection.Methods.isLogin(HttpContext.Current))
        //    {
        //        result.result.code = 1;
        //        result.result.message = ClassCollection.Message.LOGIN_FIRST;
        //        return new JavaScriptSerializer().Serialize(result);
        //    }

        //    var db = new DataAccessDataContext();
        //    var user = ClassCollection.Methods.getUserInSession(HttpContext.Current);
        //    var permission = ClassCollection.Accessing.Deserialize(db.accessTbls.Single(c => c.ID == user.accessID).per);


        //    if (db.GroupTbls.Any(c => c.ID == GroupID) == false)
        //    {
        //        result.result.code = 3;
        //        result.result.message = ClassCollection.Message.GROUP_NOT_EXIST;
        //        return new JavaScriptSerializer().Serialize(result);
        //    }

        //    var subgroups = db.GroupTbls.Single(c => c.ID == GroupID).SubGroupTbls;

        //    result.subGroup = new List<ClassCollection.Model.SubGroup>();
        //    foreach (var item in subgroups)
        //    {
        //        var tmp = new ClassCollection.Model.SubGroup();
        //        tmp.canPush = item.canPush;
        //        tmp.ID = item.ID;
        //        tmp.title = item.title;
        //        tmp.toPartner = item.toPartner;
        //        result.subGroup.Add(tmp);
        //    }

        //    result.result.code = 0;
        //    result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
        //    return new JavaScriptSerializer().Serialize(result);
        //}
        #endregion

    }
}

