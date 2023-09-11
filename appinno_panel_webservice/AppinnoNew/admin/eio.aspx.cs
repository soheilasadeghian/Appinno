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
    public partial class eio : System.Web.UI.Page
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

        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string getIo(int ioID)
        {
            ClassCollection.Model.IoResult result = new ClassCollection.Model.IoResult();
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

            if (permission.io.access.showlist == false)
            {
                result.result.code = 2;
                result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }

            if (db.ioTbls.Any(c => c.ID == ioID) == false)
            {
                result.result.code = 2;
                result.result.message = ClassCollection.Message.IO_NOT_EXIST;
                result.io = null;

                return new JavaScriptSerializer().Serialize(result);
            }

            var io = db.ioTbls.Single(c => c.ID == ioID);
            var tags = db.ioTagTbls.Where(c => c.ioID == ioID);

            var contents = db.ioContentTbls.Where(c => c.ioID == io.ID).OrderBy(c => c.pri);/*contentedit*/
            var groups = db.ioSubGroupTbls.Where(c => c.ioID == ioID);
         

            result.io = new ClassCollection.Model.fullIo();
            result.io.ID = io.ID;
            result.io.title = io.title;
            result.io.isBlock = io.isBlock;
            result.io.publishDate = Persia.Calendar.ConvertToPersian(io.publishDate).ToString();
            result.io.like = io.ioLikeTbls.Count(c => c.isLike == true);
            result.io.unlike = io.ioLikeTbls.Count(c => c.isLike == false);
            result.io.userID = io.userID;
            result.io.mUserID = io.mUserID;
            result.io.viewCount = io.viewCount;

            result.io.contents = new List<ClassCollection.Model.ioContent>();
            result.io.tag = new List<Model.Tag>();
            foreach (var item in tags)
            {
                var tmp = new ClassCollection.Model.Tag();
                tmp.ID = item.tagTbl.ID;
                tmp.title = item.tagTbl.title;

                result.io.tag.Add(tmp);
            }


            result.io.groups = new List<ClassCollection.Model.ioGroupSubGroup>();

            foreach (var item in groups)
            {
                var tmp = new ClassCollection.Model.ioGroupSubGroup();
                tmp.ID = item.ID;
                tmp.ioID = item.ioID;
                tmp.subGroupID = item.subGroupID;
                tmp.subGroupTitle = item.SubGroupTbl.title;
                tmp.groupID = item.SubGroupTbl.groupID;
                tmp.groupTitle = item.SubGroupTbl.GroupTbl.title;

                result.io.groups.Add(tmp);
            }

            foreach (var item in contents)
            {
                var tmp = new ClassCollection.Model.ioContent();

                tmp.ID = item.ID;
                tmp.type = item.contentType;

                if (item.contentType == 0)
                {
                    tmp.value = item.value;
                }
                else if (item.contentType == 1)
                {
                    tmp.value = ClassCollection.Methods.getioImagesPath().Replace("~", "") + item.value;
                }
                else if (item.contentType == 2)
                {
                    tmp.value = ClassCollection.Methods.getioVideosPath().Replace("~", "") + item.value;
                }


                result.io.contents.Add(tmp);
            }

            result.result.code = 0;
            result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
        }
    }
}

