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
    public partial class userlist : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var db = new DataAccessDataContext();
            if (ClassCollection.Methods.isLogin(Context))
            {
                var user = ClassCollection.Methods.getUserInSession(HttpContext.Current);
                var permission = ClassCollection.Accessing.Deserialize(db.accessTbls.Single(c => c.ID == user.accessID).per);

                if (!permission.user.access.showlist)
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
        public static string getAllUser(string from, string to, string userstate, string fillterString, long subgroupID, string orderType, string sortby, int pageIndex)
        {
            const int pageSize = 30;

            ClassCollection.Model.mUserListResult result = new ClassCollection.Model.mUserListResult();
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

            if (!permission.user.access.showlist)
            {
                result.result.code = 2;
                result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }
            var fromD = new DateTime();
            var toD = new DateTime();
            if (from != "")
            {
                try
                {
                    var tt = from.Split('/');
                    fromD = Persia.Calendar.ConvertToGregorian(int.Parse(tt[0]), int.Parse(tt[1]), int.Parse(tt[2]), Persia.DateType.Persian);
                }
                catch
                {
                    result.result.code = 3;
                    result.result.message = ClassCollection.Message.DATE_INCORRECT;
                    return new JavaScriptSerializer().Serialize(result);
                }
            }
            if (to != "")
            {
                try
                {
                    var tt = to.Split('/');
                    toD = Persia.Calendar.ConvertToGregorian(int.Parse(tt[0]), int.Parse(tt[1]), int.Parse(tt[2]), Persia.DateType.Persian);
                }
                catch
                {
                    result.result.code = 3;
                    result.result.message = ClassCollection.Message.DATE_INCORRECT;
                    return new JavaScriptSerializer().Serialize(result);
                }
            }
            if (to != "" && from != "")
            {
                if (fromD.CompareTo(toD) > 0)
                {
                    result.result.code = 3;
                    result.result.message = ClassCollection.Message.DATE_INCORRECT;
                    return new JavaScriptSerializer().Serialize(result);
                }
            }

            var us = db.mUserTbls.AsQueryable();

            if (to != "")
            {
                us = us.Where(c => c.regDate <= toD);
            }
            if (from != "")
            {
                us = us.Where(c => c.regDate >= fromD);
            }
            if (subgroupID == -1)
            {
                us = us.Where(c => c.userSubGroupTbls.Any() == false);
            }
            else if (subgroupID == -2)
            {
                us = us.Where(c => c.userSubGroupTbls.Any());
            }
            else if (subgroupID > 0)
            {
                if (db.SubGroupTbls.Any(c => c.ID == subgroupID) == false)
                {
                    result.result.code = 3;
                    result.result.message = ClassCollection.Message.SUBGROUP_NOT_EXIST;
                    return new JavaScriptSerializer().Serialize(result);
                }

                us = us.Where(c => c.userSubGroupTbls.Any(p => p.subGroupID == subgroupID));
            }

            if (userstate == "block")
            {
                us = us.Where(c => c.isBlock == true);
            }
            else if (userstate == "unblock")
            {
                us = us.Where(c => c.isBlock == false);
            }




            #region search

            if (fillterString != "")
            {
                var search = new Search(fillterString);
                var ors = search.getOR;

                var predicate = PredicateBuilder.False<mUserTbl>();
                foreach (string keyword in ors)
                {
                    string temp = keyword;
                    predicate = predicate.Or(p => p.name.Contains(keyword) || p.family.Contains(keyword) || p.emailtell.Contains(keyword));
                }

                us = us.Where(predicate);
            }

            if (orderType == "a")
            {
                switch (sortby)
                {
                    case "name":
                        us = us.OrderBy(c => c.name);
                        break;
                    case "family":
                        us = us.OrderBy(c => c.family);
                        break;
                    case "date":
                        us = us.OrderBy(c => c.regDate);
                        break;
                    case "emailtell":
                        us = us.OrderBy(c => c.emailtell);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (sortby)
                {
                    case "name":
                        us = us.OrderByDescending(c => c.name);
                        break;
                    case "family":
                        us = us.OrderByDescending(c => c.family);
                        break;
                    case "date":
                        us = us.OrderByDescending(c => c.regDate);
                        break;
                    case "emailtell":
                        us = us.OrderByDescending(c => c.emailtell);
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

            result.user = new List<Model.mUser>();
            
            foreach (var item in acList)
            {
                var tmp = new ClassCollection.Model.mUser();
                tmp.regDate = Persia.Calendar.ConvertToPersian(item.regDate).ToString("m") + " - " + Persia.Calendar.ConvertToPersian(item.regDate).ToString("H");
                tmp.ID = item.ID;
                tmp.name = item.name;
                tmp.family = item.family;
                tmp.emailormobile = item.emailtell;
                tmp.isBlock = item.isBlock;

                if(db.partnersTbls.Any(c=>c.registrationmobile==item.emailtell))
                {
                    var p = db.partnersTbls.Where(c => c.registrationmobile == item.emailtell).Single();
                    tmp.innerTell = p.innerTell;
                    tmp.level = p.level.Substring(0,Math.Min(25,p.level.Length));
                }
                else
                {
                    tmp.level = "-";
                    tmp.innerTell = "-";
                }
                result.user.Add(tmp);

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
        public static string editUser(long userID, bool block, List<long> subgroups)
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

            if (!permission.user.access.edit)
            {
                result.code = 2;
                result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }

            if (db.mUserTbls.Any(c => c.ID == userID) == false)
            {
                result.code = 3;
                result.message = ClassCollection.Message.USER_NOT_EXIST;
                return new JavaScriptSerializer().Serialize(result);
            }

            db.userSubGroupTbls.DeleteAllOnSubmit(db.userSubGroupTbls.Where(c => c.userID == userID));
            db.SubmitChanges();

            var muser = db.mUserTbls.Single(c => c.ID == userID);
            muser.isBlock = block;
            db.SubmitChanges();
            foreach (var item in subgroups)
            {
                if (db.SubGroupTbls.Any(c => c.ID == item))
                {
                    var subg = new userSubGroupTbl();
                    subg.userID = muser.ID;
                    subg.subGroupID = item;
                    db.userSubGroupTbls.InsertOnSubmit(subg);
                    db.SubmitChanges();
                }
            }

            result.code = 0;
            result.message = ClassCollection.Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);

        }

        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string getUser(long userID)
        {
            ClassCollection.Model.mUserResult result = new ClassCollection.Model.mUserResult();
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

            if (!permission.user.access.edit)
            {
                result.result.code = 2;
                result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }

            if (db.mUserTbls.Any(c => c.ID == userID) == false)
            {
                result.result.code = 3;
                result.result.message = ClassCollection.Message.USER_NOT_EXIST;
                return new JavaScriptSerializer().Serialize(result);
            }
            var muser = db.mUserTbls.Single(c => c.ID == userID);
            result.user = new Model.mUser();
            result.user.emailormobile = muser.emailtell;
            result.user.family = muser.family;
            result.user.ID = muser.ID;
            result.user.isBlock = muser.isBlock;
            result.user.name = muser.name;
            result.user.regDate = Persia.Calendar.ConvertToPersian(muser.regDate).ToString("m") + " - " + Persia.Calendar.ConvertToPersian(muser.regDate).ToString("H");

            result.user.groups = new List<Model.PushTo>();
            foreach (var item in muser.userSubGroupTbls)
            {
                var t = new Model.PushTo();
                t.groupTitle = item.SubGroupTbl.GroupTbl.title;
                t.subGroupID = item.subGroupID;
                t.subGroupTitle = item.SubGroupTbl.title;

                result.user.groups.Add(t);
            }

            result.result.code = 0;
            result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);

        }


        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string deleteUser(long userID)
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

            if (!permission.user.access.delete)
            {
                result.code = 2;
                result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }
            if (db.mUserTbls.Any(c => c.ID == userID) == false)
            {
                result.code = 3;
                result.message = ClassCollection.Message.USER_NOT_EXIST;
                return new JavaScriptSerializer().Serialize(result);
            }

            var tmp = db.mUserTbls.Single(c => c.ID == userID);

            db.userSubGroupTbls.DeleteAllOnSubmit(db.userSubGroupTbls.Where(c => c.userID == userID));
            db.SubmitChanges();



            db.mUserTbls.DeleteOnSubmit(tmp);
            db.SubmitChanges();

            result.code = 0;
            result.message = ClassCollection.Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
        }


        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string deleteAllUser()
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

            if (!permission.user.access.delete || !permission.user.access.showlist)
            {
                result.code = 2;
                result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }

            db.userSubGroupTbls.DeleteAllOnSubmit(db.userSubGroupTbls);
            db.SubmitChanges();

            db.mUserTbls.DeleteAllOnSubmit(db.mUserTbls);
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
    }
}

