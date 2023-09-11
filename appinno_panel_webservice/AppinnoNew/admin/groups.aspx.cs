using OfficeOpenXml;
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
    public partial class groups : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
            var db = new DataAccessDataContext();
            if (ClassCollection.Methods.isLogin(Context))
            {
                var user = ClassCollection.Methods.getUserInSession(HttpContext.Current);
                var permission = ClassCollection.Accessing.Deserialize(db.accessTbls.Single(c => c.ID == user.accessID).per);

                if (!permission.grouping.access.showlist)
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

            if (!permission.grouping.access.showlist)
            {
                result.result.code = 2;
                result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }

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
        public static string insertGroup(string title)
        {
            ClassCollection.Model.Result result = new ClassCollection.Model.Result();
            title = title.Trim();

            if (!ClassCollection.Methods.isLogin(HttpContext.Current))
            {
                result.code = 1;
                result.message = ClassCollection.Message.LOGIN_FIRST;
                return new JavaScriptSerializer().Serialize(result);
            }

            var db = new DataAccessDataContext();
            var user = ClassCollection.Methods.getUserInSession(HttpContext.Current);
            var permission = ClassCollection.Accessing.Deserialize(db.accessTbls.Single(c => c.ID == user.accessID).per);

            if (!permission.grouping.access.insert)
            {
                result.code = 2;
                result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }

            if (title.Length == 0 || title.Length >= 250)
            {
                result.code = 2;
                result.message = ClassCollection.Message.GROUP_TITLE_INCORRECT;
                return new JavaScriptSerializer().Serialize(result);
            }

            DateTime dt = new DateTime();
            dt = DateTime.Now;
            db.GroupTbls.InsertOnSubmit(new GroupTbl() { title = title, isBlock = false, regDate = dt });
            db.SubmitChanges();

            result.code = 0;
            result.message = ClassCollection.Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
        }


        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string editGroup(int GroupID, string title)
        {
            ClassCollection.Model.Result result = new ClassCollection.Model.Result();
            title = title.Trim();


            if (!ClassCollection.Methods.isLogin(HttpContext.Current))
            {
                result.code = 1;
                result.message = ClassCollection.Message.LOGIN_FIRST;
                return new JavaScriptSerializer().Serialize(result);
            }

            var db = new DataAccessDataContext();
            var user = ClassCollection.Methods.getUserInSession(HttpContext.Current);
            var permission = ClassCollection.Accessing.Deserialize(db.accessTbls.Single(c => c.ID == user.accessID).per);

            if (!permission.grouping.access.edit)
            {
                result.code = 2;
                result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }

            if (title.Length == 0 || title.Length >= 250)
            {
                result.code = 2;
                result.message = ClassCollection.Message.GROUP_TITLE_INCORRECT;
                return new JavaScriptSerializer().Serialize(result);
            }

            if (db.GroupTbls.Any(c => c.ID == GroupID) == false)
            {
                result.code = 3;
                result.message = ClassCollection.Message.GROUP_NOT_EXIST;
                return new JavaScriptSerializer().Serialize(result);
            }

            var group = db.GroupTbls.Single(c => c.ID == GroupID);
            group.title = title;
            db.SubmitChanges();

            result.code = 0;
            result.message = ClassCollection.Message.OPERATION_SUCCESS;

            return new JavaScriptSerializer().Serialize(result);
        }


        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string getGroup(int GroupID)
        {
            ClassCollection.Model.GroupResult result = new ClassCollection.Model.GroupResult();
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

            if (!permission.grouping.access.edit)
            {
                result.result.code = 2;
                result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }

            if (db.GroupTbls.Any(c => c.ID == GroupID) == false)
            {
                result.result.code = 3;
                result.result.message = ClassCollection.Message.GROUP_NOT_EXIST;
                return new JavaScriptSerializer().Serialize(result);
            }

            var group = db.GroupTbls.Single(c => c.ID == GroupID);
            result.group = new ClassCollection.Model.Group();
            result.group.title = group.title;
            result.group.ID = group.ID;
            db.SubmitChanges();

            result.result.code = 0;
            result.result.message = ClassCollection.Message.OPERATION_SUCCESS;

            return new JavaScriptSerializer().Serialize(result);
        }


        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string deleteGroup(int GroupID)
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

            if (!permission.grouping.access.delete || GroupID == 8)
            {
                result.code = 2;
                result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }

            if (db.GroupTbls.Any(c => c.ID == GroupID) == false)
            {
                result.code = 3;
                result.message = ClassCollection.Message.GROUP_NOT_EXIST;
                return new JavaScriptSerializer().Serialize(result);
            }
            if (db.SubGroupTbls.Any())
            {
                result.code = 4;
                result.message = ClassCollection.Message.DELETE_SUBCATEGORY_FIRST;
                return new JavaScriptSerializer().Serialize(result);
            }

            var group = db.GroupTbls.Single(c => c.ID == GroupID);
            db.GroupTbls.DeleteOnSubmit(group);
            db.SubmitChanges();

            result.code = 0;
            result.message = ClassCollection.Message.OPERATION_SUCCESS;
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

            if (!permission.grouping.access.showlist)
            {
                result.result.code = 2;
                result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }

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
        public static string insertSubGroup(string title, int groupID, Boolean canPush, Boolean toAll, Boolean toPartner, List<int> pushTo)
        {
            ClassCollection.Model.Result result = new ClassCollection.Model.Result();
            title = title.Trim();

            if (!ClassCollection.Methods.isLogin(HttpContext.Current))
            {
                result.code = 1;
                result.message = ClassCollection.Message.LOGIN_FIRST;
                return new JavaScriptSerializer().Serialize(result);
            }

            var db = new DataAccessDataContext();
            var user = ClassCollection.Methods.getUserInSession(HttpContext.Current);
            var permission = ClassCollection.Accessing.Deserialize(db.accessTbls.Single(c => c.ID == user.accessID).per);

            if (!permission.grouping.access.insert)
            {
                result.code = 2;
                result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }
            if (title.Length == 0 || title.Length >= 250)
            {
                result.code = 3;
                result.message = ClassCollection.Message.SUBGROUP_TITLE_INCORRECT;
                return new JavaScriptSerializer().Serialize(result);
            }
            if (db.GroupTbls.Any(c => c.ID == groupID) == false)
            {
                result.code = 4;
                result.message = ClassCollection.Message.GROUP_NOT_EXIST;
                return new JavaScriptSerializer().Serialize(result);
            }

            DateTime dt = new DateTime();
            dt = DateTime.Now;

            var subgroup = new SubGroupTbl();
            subgroup.canPush = canPush;
            subgroup.groupID = groupID;
            subgroup.isBlock = false;
            subgroup.regDate = dt;
            subgroup.toAll = toAll && canPush;
            subgroup.title = title;
            subgroup.toPartner = toPartner && canPush;
            db.SubGroupTbls.InsertOnSubmit(subgroup);
            db.SubmitChanges();

            if (canPush && !toAll)
                foreach (var to in pushTo)
                {
                    if (db.SubGroupTbls.Any(c => c.ID == to))
                    {
                        var tmp = new subGroupPushTbl();
                        tmp.subGroupID = subgroup.ID;
                        tmp.toSubGroupID = to;
                        db.subGroupPushTbls.InsertOnSubmit(tmp);
                        db.SubmitChanges();
                    }
                }

            result.code = 0;
            result.message = ClassCollection.Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
        }


        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string getSubGroup(int SubGroupID)
        {
            ClassCollection.Model.SubGroupResult result = new ClassCollection.Model.SubGroupResult();
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

            if (!permission.grouping.access.edit)
            {
                result.result.code = 2;
                result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }

            if (db.SubGroupTbls.Any(c => c.ID == SubGroupID) == false)
            {
                result.result.code = 3;
                result.result.message = ClassCollection.Message.SUBGROUP_NOT_EXIST;
                return new JavaScriptSerializer().Serialize(result);
            }

            var subgroup = db.SubGroupTbls.Single(c => c.ID == SubGroupID);
            result.subGroup = new ClassCollection.Model.SubGroup();
            result.subGroup.canPush = subgroup.canPush;
            result.subGroup.ID = subgroup.ID;
            result.subGroup.title = subgroup.title;
            result.subGroup.toPartner = subgroup.toPartner;
            result.subGroup.toAll = subgroup.toAll;
            result.subGroup.pushTo = new List<ClassCollection.Model.PushTo>();
            foreach (var item in subgroup.subGroupPushTbls)
            {
                var tmp = new ClassCollection.Model.PushTo();
                tmp.subGroupID = item.toSubGroupID;
                tmp.subGroupTitle = db.SubGroupTbls.Single(c => c.ID == item.toSubGroupID).title;
                tmp.groupTitle = db.SubGroupTbls.Single(c => c.ID == item.toSubGroupID).GroupTbl.title;
                result.subGroup.pushTo.Add(tmp);
            }


            result.result.code = 0;
            result.result.message = ClassCollection.Message.OPERATION_SUCCESS;

            return new JavaScriptSerializer().Serialize(result);
        }


        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string editSubGroup(long subgroupID, string title, Boolean canPush, Boolean toAll, Boolean toPartner, List<int> pushTo)
        {
            ClassCollection.Model.Result result = new ClassCollection.Model.Result();
            title = title.Trim();

            if (!ClassCollection.Methods.isLogin(HttpContext.Current))
            {
                result.code = 1;
                result.message = ClassCollection.Message.LOGIN_FIRST;
                return new JavaScriptSerializer().Serialize(result);
            }

            var db = new DataAccessDataContext();
            var user = ClassCollection.Methods.getUserInSession(HttpContext.Current);
            var permission = ClassCollection.Accessing.Deserialize(db.accessTbls.Single(c => c.ID == user.accessID).per);

            if (!permission.grouping.access.edit)
            {
                result.code = 2;
                result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }
            if (title.Length == 0 || title.Length >= 250)
            {
                result.code = 3;
                result.message = ClassCollection.Message.SUBGROUP_TITLE_INCORRECT;
                return new JavaScriptSerializer().Serialize(result);
            }
            if (db.SubGroupTbls.Any(c => c.ID == subgroupID) == false)
            {
                result.code = 4;
                result.message = ClassCollection.Message.SUBGROUP_NOT_EXIST;
                return new JavaScriptSerializer().Serialize(result);
            }

            DateTime dt = new DateTime();
            dt = DateTime.Now;

            var subgroup = db.SubGroupTbls.Single(c => c.ID == subgroupID);
            subgroup.canPush = canPush;
            subgroup.isBlock = false;
            subgroup.toAll = toAll && canPush;
            if (subgroup.ID != 2)
                subgroup.title = title;
            subgroup.toPartner = toPartner && canPush;
            db.SubmitChanges();
            db.subGroupPushTbls.DeleteAllOnSubmit(subgroup.subGroupPushTbls);
            db.SubmitChanges();
            if (canPush && !toAll && !toPartner)
            {

                foreach (var to in pushTo)
                {
                    if (db.SubGroupTbls.Any(c => c.ID == to))
                    {
                        var tmp = new subGroupPushTbl();
                        tmp.subGroupID = subgroup.ID;
                        tmp.toSubGroupID = to;
                        db.subGroupPushTbls.InsertOnSubmit(tmp);
                        db.SubmitChanges();
                    }
                }
            }
            result.code = 0;
            result.message = ClassCollection.Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
        }


        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string deleteSubGroup(int SubGroupID)
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

            if (!permission.grouping.access.delete || SubGroupID == 2)
            {
                result.code = 2;
                result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }

            if (db.SubGroupTbls.Any(c => c.ID == SubGroupID) == false)
            {
                result.code = 3;
                result.message = ClassCollection.Message.SUBGROUP_NOT_EXIST;
                return new JavaScriptSerializer().Serialize(result);
            }

            var subgroup = db.SubGroupTbls.Single(c => c.ID == SubGroupID);
            if (subgroup.userSubGroupTbls.Any())
            {
                result.code = 3;
                result.message = ClassCollection.Message.DELETE_USER_FIRST;
                return new JavaScriptSerializer().Serialize(result);
            }

            db.subGroupPushTbls.DeleteAllOnSubmit(subgroup.subGroupPushTbls);
            db.SubmitChanges();
            db.SubGroupTbls.DeleteOnSubmit(subgroup);
            db.SubmitChanges();
            result.code = 0;
            result.message = ClassCollection.Message.OPERATION_SUCCESS;

            return new JavaScriptSerializer().Serialize(result);
        }

    }
}