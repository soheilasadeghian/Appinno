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
    public partial class edownload : System.Web.UI.Page
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
        public static string getdownload(int downloadID)
        {
            ClassCollection.Model.downloadResult result = new ClassCollection.Model.downloadResult();
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

            if (permission.download.access.showlist == false)
            {
                result.result.code = 2;
                result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }

            if (db.downloadTbls.Any(c => c.ID == downloadID) == false)
            {
                result.result.code = 2;
                result.result.message = ClassCollection.Message.DOWNLOAD_NOT_EXIST;
                result.download = null;

                return new JavaScriptSerializer().Serialize(result);
            }

            var download = db.downloadTbls.Single(c => c.ID == downloadID);

            var contents = db.downloadContentTbls.Where(c => c.downloadID == download.ID).OrderBy(c => c.pri);/*contentedit*/
            var groups = db.downloadSubGroupTbls.Where(c => c.downloadID == downloadID);
            var tags = db.downloadTagTbls.Where(c => c.downloadID == downloadID);

            result.download = new ClassCollection.Model.fulldownload();
            result.download.ID = download.ID;
            result.download.title = download.title;
            result.download.isBlock = download.isBlock;
            result.download.publishDate = Persia.Calendar.ConvertToPersian(download.publishDate).ToString();
            result.download.like = download.downloadLikeTbls.Count(c => c.isLike == true);
            result.download.unlike = download.downloadLikeTbls.Count(c => c.isLike == false);
            result.download.userID = download.userID;
            result.download.mUserID = download.mUserID;
            result.download.viewCount = download.viewCount;

            result.download.contents = new List<ClassCollection.Model.downloadContent>();
            result.download.groups = new List<ClassCollection.Model.downloadGroupSubGroup>();
            result.download.tag = new List<Model.Tag>();

            foreach (var item in tags)
            {
                var tmp = new ClassCollection.Model.Tag();
                tmp.ID = item.tagTbl.ID;
                tmp.title = item.tagTbl.title;

                result.download.tag.Add(tmp);
            }

            foreach (var item in groups)
            {
                var tmp = new ClassCollection.Model.downloadGroupSubGroup();
                tmp.ID = item.ID;
                tmp.downloadID = item.downloadID;
                tmp.subGroupID = item.subGroupID;
                tmp.subGroupTitle = item.SubGroupTbl.title;
                tmp.groupID = item.SubGroupTbl.groupID;
                tmp.groupTitle = item.SubGroupTbl.GroupTbl.title;

                result.download.groups.Add(tmp);
            }

            foreach (var item in contents)
            {
                var tmp = new ClassCollection.Model.downloadContent();

                tmp.ID = item.ID;
                tmp.type = item.contentType;

                if (item.contentType == 0)
                {
                    tmp.value = item.value;
                }
                else if (item.contentType == 1)
                {
                    tmp.value = ClassCollection.Methods.getdownloadImagesPath().Replace("~", "") + item.value;
                }
                else if (item.contentType == 2)
                {
                    tmp.fileSize = item.fileSize;
                    tmp.fileType = item.fileType;
                    tmp.downloadCount = item.downloadCount;
                    tmp.value = ClassCollection.Methods.getdownloadFilePath().Replace("~", "") + item.value;
                }


                result.download.contents.Add(tmp);
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
    }
}

