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
    public partial class message : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var db = new DataAccessDataContext();
            if (ClassCollection.Methods.isLogin(Context))
            {
                var user = ClassCollection.Methods.getUserInSession(HttpContext.Current);
                var permission = ClassCollection.Accessing.Deserialize(db.accessTbls.Single(c => c.ID == user.accessID).per);
                
                var relation =
                //مدیر کل سامانه
                (user.accessID == 1) ||
                //مدیر کل سامانه(همراه اول
                (user.accessID == 52);

                if (!relation)
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
        public static string getAllMessage(string fillterString, bool toAll, bool toPartner, long subgroupID, string orderType, int pageIndex)
        {
            const int pageSize = 30;

            ClassCollection.Model.MessageListResult result = new ClassCollection.Model.MessageListResult();
            result.result = new ClassCollection.Model.Result();

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

            var relation =
               //مدیر کل سامانه
               (user.accessID == 1) ||
               //مدیر کل سامانه(همراه اول
               (user.accessID == 52);

            if (!relation)
            {
                result.result.code = 2;
                result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }
            if (subgroupID != -1)
            {
                if (db.SubGroupTbls.Any(c => c.ID == subgroupID) == false)
                {
                    result.result.code = 3;
                    result.result.message = ClassCollection.Message.SUBGROUP_NOT_EXIST;
                    return new JavaScriptSerializer().Serialize(result);
                }
            }

            var ac = db.messageTbls.Where(c => (c.messageSubGroupTbls.Any(p => p.subGroupID == subgroupID) || subgroupID == -1)
                && (c.toAll == true || toAll == false)
                && (c.toPartner == true || toPartner == false)
                );

            #region search

            if (fillterString != "")
            {
                var search = new Search(fillterString);
                var ors = search.getOR;

                var predicate = PredicateBuilder.False<messageTbl>();
                foreach (string keyword in ors)
                {
                    string temp = keyword;
                    predicate = predicate.Or(p => p.title.Contains(keyword) || p.text.Contains(keyword));
                }

                ac = ac.Where(predicate);
            }


            if (orderType == "a")
            {
                ac = ac.OrderBy(c => c.regDate);
            }
            else
            {
                ac = ac.OrderByDescending(c => c.regDate);
            }

            var acList = ac.ToList();

            
            var count = acList.Count();
            result.totalCount = count;

            acList = acList.Skip(skipCount).Take(pageSize).ToList();

            #endregion

            result.message = new List<Model.Message>();
            foreach (var item in acList)
            {
                var tmp = new ClassCollection.Model.Message();
                tmp.regDate = Persia.Calendar.ConvertToPersian(item.regDate).ToString("m") + " - " + Persia.Calendar.ConvertToPersian(item.regDate).ToString("H");
                tmp.ID = item.ID;
                tmp.text = item.text;
                tmp.title = item.title;
                result.message.Add(tmp);

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
        public static string getMessage(long messageID)
        {
            ClassCollection.Model.MessageResult result = new ClassCollection.Model.MessageResult();
            result.result = new Model.Result();

            if (!ClassCollection.Methods.isLogin(HttpContext.Current))
            {
                result.result.code = 1;
                result.result.message = ClassCollection.Message.LOGIN_FIRST;
                return new JavaScriptSerializer().Serialize(result);
            }

            var db = new DataAccessDataContext();
            var user = ClassCollection.Methods.getUserInSession(HttpContext.Current);
            var permission = ClassCollection.Accessing.Deserialize(db.accessTbls.Single(c => c.ID == user.accessID).per);

            var relation =
               //مدیر کل سامانه
               (user.accessID == 1) ||
               //مدیر کل سامانه(همراه اول
               (user.accessID == 52);

            if (!relation)
            {
                result.result.code = 2;
                result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }
            if (db.messageTbls.Any(c => c.ID == messageID) == false)
            {
                result.result.code = 3;
                result.result.message = ClassCollection.Message.PUSH_MESSAGE_NOT_EXIST;
                return new JavaScriptSerializer().Serialize(result);
            }

            var tmp = db.messageTbls.Single(c => c.ID == messageID);
            result.Message = new Model.Message();

            result.Message.ID = tmp.ID;
            result.Message.text = tmp.text;
            result.Message.toAll = tmp.toAll;
            result.Message.toPartner = tmp.toPartner;
            result.Message.title = tmp.title;
            result.Message.regDate = Persia.Calendar.ConvertToPersian(tmp.regDate).ToString("m") + " - " + Persia.Calendar.ConvertToPersian(tmp.regDate).ToString("H");
            result.Message.attachment = tmp.attachment;
            result.Message.isBlock = tmp.isBlock;

            if (tmp.toPartner == false && tmp.toAll == false)
            {
                result.pushTo = new List<Model.PushTo>();
                var grs = db.SubGroupTbls.Where(c => c.messageSubGroupTbls.Any(p => p.messageID == tmp.ID));
                foreach (var item in grs)
                {
                    var t = new Model.PushTo();
                    t.groupTitle = item.GroupTbl.title;
                    t.subGroupTitle = item.title;
                    result.pushTo.Add(t);
                }
                
            }

            result.result.code = 0;
            result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
        }


        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string deleteMessage(long messageID)
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

            var relation =
               //مدیر کل سامانه
               (user.accessID == 1) ||
               //مدیر کل سامانه(همراه اول
               (user.accessID == 52);

            if (!relation)
            {
                result.code = 2;
                result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }

            if (db.messageTbls.Any(c => c.ID == messageID) == false)
            {
                result.code = 3;
                result.message = ClassCollection.Message.PUSH_MESSAGE_NOT_EXIST;
                return new JavaScriptSerializer().Serialize(result);
            }

            var tmp = db.messageTbls.Single(c => c.ID == messageID);

            db.messageSubGroupTbls.DeleteAllOnSubmit(db.messageSubGroupTbls.Where(c => c.messageID == messageID));
            db.SubmitChanges();

            db.messageTbls.DeleteOnSubmit(tmp);
            db.SubmitChanges();

            result.code = 0;
            result.message = ClassCollection.Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
        }


        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string deleteAllMessage()
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

            var relation =
               //مدیر کل سامانه
               (user.accessID == 1) ||
               //مدیر کل سامانه(همراه اول
               (user.accessID == 52);

            if (!relation)
            {
                result.code = 2;
                result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }

            db.messageSubGroupTbls.DeleteAllOnSubmit(db.messageSubGroupTbls);
            db.SubmitChanges();

            db.messageTbls.DeleteAllOnSubmit(db.messageTbls);
            db.SubmitChanges();

            result.code = 0;
            result.message = ClassCollection.Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
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
        public static string sendMessage(long messageID)
        {
            ClassCollection.Model.MessageResult result = new ClassCollection.Model.MessageResult();
            result.result = new Model.Result();

            if (!ClassCollection.Methods.isLogin(HttpContext.Current))
            {
                result.result.code = 1;
                result.result.message = ClassCollection.Message.LOGIN_FIRST;
                return new JavaScriptSerializer().Serialize(result);
            }

            var db = new DataAccessDataContext();
            var user = ClassCollection.Methods.getUserInSession(HttpContext.Current);
            var permission = ClassCollection.Accessing.Deserialize(db.accessTbls.Single(c => c.ID == user.accessID).per);

            var relation =
               //مدیر کل سامانه
               (user.accessID == 1) ||
               //مدیر کل سامانه(همراه اول
               (user.accessID == 52);

            if (!relation)
            {
                result.result.code = 2;
                result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }

            if (db.messageTbls.Any(c => c.ID == messageID) == false)
            {
                result.result.code = 3;
                result.result.message = ClassCollection.Message.PUSH_MESSAGE_NOT_EXIST;
                return new JavaScriptSerializer().Serialize(result);
            }

            var message = db.messageTbls.Single(c => c.ID == messageID);
            var sender = db.mUserTbls.Single(c => c.ID == message.mUserID);
            
            if (!message.toPartner && !message.toAll)
            {
                string pushtoString = "";
                
                var grs = db.SubGroupTbls.Where(c => c.messageSubGroupTbls.Any(p => p.messageID == message.ID));
                foreach (var item in grs)
                {
                    pushtoString += item.ID + ",";
                }

                pushtoString = pushtoString.Remove(pushtoString.LastIndexOf(","), 1);
                ClassCollection.Methods.unBlocker(message, sender, pushtoString, message.toAll, message.toPartner);
            }
            else if (message.toPartner == true)
            {
                ClassCollection.Methods.unBlocker(message, sender, null, message.toAll, message.toPartner);
            }
            else if (message.toAll == true)
            {
                ClassCollection.Methods.unBlocker(message, sender, null, message.toAll, message.toPartner);
            }
            result.result.code = 0;
            result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result.result);
        }
    }
}

