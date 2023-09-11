using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;

namespace AppinnoNew.admin
{
    /// <summary>
    /// Summary description for partnersUploader
    /// </summary>
    public class partnersUploader : IHttpHandler, IRequiresSessionState
    {

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
            var relation =
               //مدیر کل سامانه
               (user.accessID == 1) ||
               //مدیر کل سامانه(همراه اول
               (user.accessID == 52);

            if (!relation)
            {
                result.code = 2;
                result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                context.Response.Write(new JavaScriptSerializer().Serialize(result));
                return;
            }

            var fte = context.Request.Files[0];
            List<ClassCollection.Excel.Import.PartnerLoader.Partner> info = new List<ClassCollection.Excel.Import.PartnerLoader.Partner>();
            if (fte != null)
            {
                try
                {
                    info = ClassCollection.Excel.Import.PartnerLoader.LoadPartnerInfo(fte.InputStream);
                    foreach (var item in info)
                    {
                        if (item.registrationmobile != "")
                        {
                            item.registrationmobile = item.registrationmobile.StartsWith("0") ? "98" + item.registrationmobile.Remove(0, 1) : item.registrationmobile;
                        }

                        if (item.optionalmobile != "")
                        {
                            item.optionalmobile = item.optionalmobile.StartsWith("0") ? "98" + item.optionalmobile.Remove(0, 1) : item.optionalmobile;
                        }

                        if (db.partnersTbls.Any(c => c.registrationmobile == item.registrationmobile) == false)
                        {

                            var partner = new partnersTbl();

                            partner.email = item.email;
                            partner.family = item.family;
                            partner.innerTell = item.innerTell;
                            partner.level = item.level;
                            partner.name = item.name;
                            partner.optionalmobile = item.optionalmobile;
                            partner.registrationmobile = item.registrationmobile;

                            db.partnersTbls.InsertOnSubmit(partner);
                            db.SubmitChanges();
                        }
                        else
                        {
                            var tmp = db.partnersTbls.Single(c => c.registrationmobile == item.registrationmobile);

                            tmp.name = item.name;
                            tmp.family = item.family;
                            tmp.email = item.email;
                            tmp.innerTell = item.innerTell;
                            tmp.level = item.level;
                            tmp.optionalmobile = item.optionalmobile;

                            db.SubmitChanges();
                        }
                    }

                }
                catch
                {
                    result.code = 2;
                    result.message = ClassCollection.Message.FILE_INVALID;
                    context.Response.Write(new JavaScriptSerializer().Serialize(result));
                    return;
                }
            }

            result.code = 0;
            result.message = ClassCollection.Message.OPERATION_SUCCESS;
            context.Response.Write(new JavaScriptSerializer().Serialize(result));
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