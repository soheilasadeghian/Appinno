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
    public partial class partners : System.Web.UI.Page
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
        public static string getAllPartners(string fillterString, string sortBy, string orderType, int pageIndex)
        {
            const int pageSize = 30;

            ClassCollection.Model.PartnerListResult result = new ClassCollection.Model.PartnerListResult();
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

            var ac = db.partnersTbls.AsQueryable<partnersTbl>();

            #region search

            if (fillterString != "")
            {
                var search = new Search(fillterString);
                var ors = search.getOR;

                var predicate = PredicateBuilder.False<partnersTbl>();
                foreach (string keyword in ors)
                {
                    string temp = keyword;
                    predicate = predicate.Or(p => p.email.Contains(keyword) || p.family.Contains(keyword) || p.innerTell.Contains(keyword) || p.level.Contains(keyword) || p.name.Contains(keyword) || p.registrationmobile.Contains(keyword) || p.optionalmobile.Contains(keyword));
                }

                ac = ac.Where(predicate);
            }
            if (sortBy == "name")
            {
                if (orderType == "a")
                    ac = ac.OrderBy(c => c.name);
                else
                    ac = ac.OrderByDescending(c => c.name);
            }
            else if (sortBy == "family")
            {
                if (orderType == "a")
                    ac = ac.OrderBy(c => c.family);
                else
                    ac = ac.OrderByDescending(c => c.family);
            }
            else if (sortBy == "innerTell")
            {
                if (orderType == "a")
                    ac = ac.OrderBy(c => c.innerTell);
                else
                    ac = ac.OrderByDescending(c => c.innerTell);
            }
            else if (sortBy == "level")
            {
                if (orderType == "a")
                    ac = ac.OrderBy(c => c.level);
                else
                    ac = ac.OrderByDescending(c => c.level);
            }
            else if (sortBy == "email")
            {
                if (orderType == "a")
                    ac = ac.OrderBy(c => c.email);
                else
                    ac = ac.OrderByDescending(c => c.email);
            }
            else if (sortBy == "registrationmobile")
            {
                if (orderType == "a")
                    ac = ac.OrderBy(c => c.registrationmobile);
                else
                    ac = ac.OrderByDescending(c => c.registrationmobile);
            }
            else if (sortBy == "optionalmobile")
            {
                if (orderType == "a")
                    ac = ac.OrderBy(c => c.optionalmobile);
                else
                    ac = ac.OrderByDescending(c => c.optionalmobile);
            }


            var acList = ac.ToList();
            
            var count = acList.Count();
            result.totalCount = count;
            acList = acList.Skip(skipCount).Take(pageSize).ToList();

            #endregion

            result.partner = new List<Excel.Import.PartnerLoader.Partner>();
            foreach (var item in acList)
            {
                var tmp = new Excel.Import.PartnerLoader.Partner();
                tmp.email = item.email;
                tmp.family = item.family;
                tmp.innerTell = item.innerTell;
                tmp.level = item.level;
                tmp.name = item.name;
                tmp.ID = item.ID;
                tmp.registrationmobile = item.registrationmobile;
                tmp.optionalmobile = item.optionalmobile;
                result.partner.Add(tmp);
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
        public static string insertPartner(string name, string family, string email, string innerTell, string level, string registrationmobile, string  optionalmobile)
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

            if (db.partnersTbls.Any(c => c.registrationmobile == registrationmobile) == true)
            {
                result.code = 3;
                result.message = ClassCollection.Message.PARTNAER_MOBILE_EXIST;
                return new JavaScriptSerializer().Serialize(result);
            }

            if (name != "")
            {
                if (name.Length == 0 || name.Length >= 250)
                {
                    result.code = 4;
                    result.message = ClassCollection.Message.NAME_INCORRECT;
                    return new JavaScriptSerializer().Serialize(result);
                }
            }
            if (family != "")
            {
                if (family.Length == 0 || family.Length >= 250)
                {
                    result.code = 5;
                    result.message = ClassCollection.Message.FAMILY_INCORRECT;
                    return new JavaScriptSerializer().Serialize(result);
                }
            }
            if (email != "")
            {
                if (email.Contains("@") && email.Length >= 250 || !email.Contains("@"))
                {
                    result.code = 6;
                    result.message = ClassCollection.Message.EMAIL_INCORRECT;
                    return new JavaScriptSerializer().Serialize(result);
                }
            }


            if (level != "")
            {
                if (level.Length >= 250)
                {
                    result.code = 7;
                    result.message = ClassCollection.Message.LEVEL_INCORRECT;
                    return new JavaScriptSerializer().Serialize(result);
                }
            }

            if (registrationmobile == "")
            {
                    result.code = 8;
                    result.message = ClassCollection.Message.MOBILE_INCORRECT;
                    return new JavaScriptSerializer().Serialize(result);
            }

            if (registrationmobile != "")
            {
                registrationmobile = registrationmobile.StartsWith("0") ? "98" + registrationmobile.Remove(0, 1) : registrationmobile;
                if (registrationmobile.Length != 12)
                {
                    result.code = 9;
                    result.message = ClassCollection.Message.MOBILE_INCORRECT;
                    return new JavaScriptSerializer().Serialize(result);
                }
            }
            if (optionalmobile != "")
            {
                optionalmobile = optionalmobile.StartsWith("0") ? "98" + optionalmobile.Remove(0, 1) : optionalmobile;
                if (optionalmobile.Length != 12)
                {
                    result.code = 10;
                    result.message = ClassCollection.Message.OPTIONALMOBILE_EXIST;
                    return new JavaScriptSerializer().Serialize(result);
                }
            }
            if (innerTell != "")
            {
                if (level.Length >= 250)
                {
                    result.code = 11;
                    result.message = ClassCollection.Message.INNERTELL_EXIST;
                    return new JavaScriptSerializer().Serialize(result);
                }
            }

            
            
            
            var tmp = new partnersTbl();
            tmp.email = email;
            tmp.family = family;
            tmp.innerTell = innerTell;
            tmp.level = level;
            tmp.name = name;
            tmp.optionalmobile = optionalmobile;
            tmp.registrationmobile = registrationmobile;
            
            db.partnersTbls.InsertOnSubmit(tmp);
            db.SubmitChanges();

            result.code = 0;
            result.message = ClassCollection.Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
        }


        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string editPartner(long partnerID, string name, string family, string email, string innerTell, string level, string registrationmobile, string optionalmobile)
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

            if (db.partnersTbls.Any(c => c.ID == partnerID) == false)
            {
                result.code = 3;
                result.message = ClassCollection.Message.PARTNAER_NOT_EXIST;
                return new JavaScriptSerializer().Serialize(result);
            }

            if (registrationmobile != "")
            {
                registrationmobile = registrationmobile.StartsWith("0") ? "98" + registrationmobile.Remove(0, 1) : registrationmobile;
                if (registrationmobile.Length != 12)
                {
                    result.code = 4;
                    result.message = ClassCollection.Message.MOBILE_INCORRECT;
                    return new JavaScriptSerializer().Serialize(result);
                }
            }

            if (db.partnersTbls.Any(c => c.registrationmobile == registrationmobile && c.ID != partnerID) == true)
            {
                result.code = 5;
                result.message = ClassCollection.Message.PARTNAER_MOBILE_EXIST;
                return new JavaScriptSerializer().Serialize(result);
            }

            if (name != "")
            {
                if (name.Length == 0 || name.Length >= 250)
                {
                    result.code = 6;
                    result.message = ClassCollection.Message.NAME_INCORRECT;
                    return new JavaScriptSerializer().Serialize(result);
                }
            }
            if (family != "")
            {
                if (family.Length == 0 || family.Length >= 250)
                {
                    result.code = 7;
                    result.message = ClassCollection.Message.FAMILY_INCORRECT;
                    return new JavaScriptSerializer().Serialize(result);
                }
            }
            if (email != "")
            {
                if (email.Contains("@") && email.Length >= 250 || !email.Contains("@"))
                {
                    result.code = 8;
                    result.message = ClassCollection.Message.EMAIL_INCORRECT;
                    return new JavaScriptSerializer().Serialize(result);
                }
            }

            if (level != "")
            {
                if (level.Length >= 250)
                {
                    result.code = 9;
                    result.message = ClassCollection.Message.LEVEL_INCORRECT;
                    return new JavaScriptSerializer().Serialize(result);
                }
            }

            if (optionalmobile != "")
            {
                optionalmobile = optionalmobile.StartsWith("0") ? "98" + optionalmobile.Remove(0, 1) : optionalmobile;
                if (optionalmobile.Length != 12)
                {
                    result.code = 10;
                    result.message = ClassCollection.Message.OPTIONALMOBILE_EXIST;
                    return new JavaScriptSerializer().Serialize(result);
                }
            }
            if (innerTell != "")
            {
                if (level.Length >= 250)
                {
                    result.code = 11;
                    result.message = ClassCollection.Message.INNERTELL_EXIST;
                    return new JavaScriptSerializer().Serialize(result);
                }
            }
            
           
            
            var tmp = db.partnersTbls.Single(c => c.ID == partnerID);
            tmp.email = email;
            tmp.family = family;
            tmp.innerTell = innerTell;
            tmp.level = level;
            tmp.name = name;
            tmp.optionalmobile = optionalmobile;
            db.SubmitChanges();


            result.code = 0;
            result.message = ClassCollection.Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
        }

        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string getPartner(long partnerID)
        {
            ClassCollection.Model.PartnerResult result = new ClassCollection.Model.PartnerResult();
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

            if (db.partnersTbls.Any(c => c.ID == partnerID) == false)
            {
                result.result.code = 3;
                result.result.message = ClassCollection.Message.PARTNAER_NOT_EXIST;
                return new JavaScriptSerializer().Serialize(result);
            }

            var tmp = db.partnersTbls.Single(c => c.ID == partnerID);
            result.partner = new Excel.Import.PartnerLoader.Partner();

            result.partner.email = tmp.email;
            result.partner.family = tmp.family;
            result.partner.innerTell = tmp.innerTell;
            result.partner.level = tmp.level;
            result.partner.name = tmp.name;
            result.partner.registrationmobile = tmp.registrationmobile.StartsWith("98") ? "0" + tmp.registrationmobile.Remove(0, 2) : tmp.registrationmobile;
            result.partner.optionalmobile = tmp.optionalmobile.StartsWith("98") ? "0" + tmp.optionalmobile.Remove(0, 2) : tmp.optionalmobile;

            result.result.code = 0;
            result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
        }


        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string deletePartner(long partnerID)
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

            if (db.partnersTbls.Any(c => c.ID == partnerID) == false)
            {
                result.code = 3;
                result.message = ClassCollection.Message.PARTNAER_NOT_EXIST;
                return new JavaScriptSerializer().Serialize(result);
            }

            var tmp = db.partnersTbls.Single(c => c.ID == partnerID);
            db.partnersTbls.DeleteOnSubmit(tmp);
            db.SubmitChanges();

            result.code = 0;
            result.message = ClassCollection.Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
        }


        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string deleteAllPartner()
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

            db.partnersTbls.DeleteAllOnSubmit(db.partnersTbls);
            db.SubmitChanges();

            result.code = 0;
            result.message = ClassCollection.Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
        }
    }
}

