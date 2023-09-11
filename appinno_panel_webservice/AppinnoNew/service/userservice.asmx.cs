using AppinnoNew.ClassCollection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;

namespace AppinnoNew.service
{
    /// <summary>
    /// این سرویس جهت استفاده اپراتورهای سامانه مورد استفاده قرار میگیرد
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]

    public class userservice : System.Web.Services.WebService
    {
        #region user

        /// <summary>
        /// این تابع وظیفه تولید و ارسال کد فعال سازی به شماره یا ایمیل ورودی را بر عهده دارد.
        /// </summary>
        /// <param name="key">کلمه عبور برنامه ای که درحال استفاده از متد جاری می باشد</param>
        /// <param name="mobileOrEmail">شماره موبایل یا ایمیل کاربر
        /// <remarks>
        ///  <para>در صورت قرار دادن شماره موبایل در این پارامتر طول آن باید برابر با 12 کاراکتر باشد</para>
        ///  <para>در صورت قرار دادن ایمیل در این پارامتر طول آن باید کمتر از 250 کاراکتر باید</para> 
        /// </remarks>
        /// </param>

        /// <returns>خروجی ساختار جی سون از کلاس زیر را دارد.
        /// <para><see cref="ClassCollection.Model.Result" /></para>
        /// <remarks>
        /// <para>Code : -1 , Message : <see cref="ClassCollection.Message.OPERATION_NO_ACCESS"/></para>
        /// <para>Code : 0 , Message : <see cref="ClassCollection.Message.VALIDATION_CODE_SENT" /></para>
        /// <para>Code : 1 , Message : <see cref="ClassCollection.Message.ERROR_INPUT_PARAMETER" /></para>

        /// </remarks>
        /// </returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void sendValidationCode(string key, string mobileOrEmail)
        {
            var ip = HttpContext.Current.Request.UserHostAddress;

            var db = new DataAccessDataContext();
            DateTime dt = new DateTime();
            dt = DateTime.Now;

            //db.ipTbls.InsertOnSubmit(new ipTbl() { ip = ip, regDate = dt });
            //db.SubmitChanges();

            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.Result Result = new ClassCollection.Model.Result();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.code = -1;
                Result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            mobileOrEmail = mobileOrEmail.Trim();

            if (mobileOrEmail == "")
            {
                Result.code = 1;
                Result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            if (mobileOrEmail.Contains("@") && mobileOrEmail.Length >= 250)
            {
                Result.code = 1;
                Result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                Context.Response.Write(js.Serialize(Result));
                return;
            }
            if (!mobileOrEmail.Contains("@") && mobileOrEmail.Length != 12)
            {
                Result.code = 1;
                Result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            string code = ClassCollection.Methods.randomValidationCode();

            var tmp = new temporaryRegistrationTbl();

            if (db.temporaryRegistrationTbls.Count(c => c.emailMobile == mobileOrEmail) > 0)
            {
                db.temporaryRegistrationTbls.DeleteAllOnSubmit(db.temporaryRegistrationTbls.Where(c => c.emailMobile == mobileOrEmail));
                db.SubmitChanges();
            }

            tmp.code = code;

            Result.code = 0;
            Result.message = ClassCollection.Message.VALIDATION_CODE_SENT;

            if (mobileOrEmail.Contains("@"))
            {
                tmp.emailMobile = mobileOrEmail;
                ClassCollection.Methods.sendMail(mobileOrEmail, "کدفعال سازی اپینو", "کد فعال سازی : " + code);
            }
            else
            {
                tmp.emailMobile = mobileOrEmail;
                ClassCollection.Methods.sendSms(mobileOrEmail, "کدفعال سازی اپینو:" + code);
            }

            tmp.regDate = dt;
            db.temporaryRegistrationTbls.InsertOnSubmit(tmp);
            db.SubmitChanges();

            Context.Response.Write(js.Serialize(Result));
            return;

        }

        /// <summary>
        /// این تابع وظیفه بررسی صحت کدفعال سازی وارد شده را بر عهده دارد.
        /// </summary>
        /// <param name="key">کلمه عبور برنامه ای که درحال استفاده از متد جاری می باشد</param>
        /// <param name="mobileOrEmail">شماره موبایل یا ایمیل کاربر
        /// <remarks>
        ///  <para>در صورت قرار دادن شماره موبایل در این پارامتر طول آن باید برابر با 12 کاراکتر باشد</para>
        ///  <para>در صورت قرار دادن ایمیل در این پارامتر طول آن باید کمتر از 250 کاراکتر باشد</para> 
        /// </remarks>
        /// </param>
        /// <param name="code">کد فعال سازی ارسال شده توسط کاربر</param>
        /// <returns>خروجی ساختار جی سون از کلاس زیر را دارد.
        /// <para><see cref="ClassCollection.Model.Result" /></para>
        /// <remarks>
        /// <para>Code : -1 , Message : <see cref="ClassCollection.Message.OPERATION_NO_ACCESS"/></para>
        /// <para>Code : 0 , Message : <see cref="ClassCollection.Message.VALIDATION_CODE_CORRECT" /></para>
        /// <para>Code : 1 , Message : <see cref="ClassCollection.Message.VALIDATION_CODE_INCORRECT" /></para>
        /// <para>Code : 2 , Message : <see cref="ClassCollection.Message.ERROR_INPUT_PARAMETER" /></para>

        /// </remarks>
        /// </returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void checkValidationCode(string key, string mobileOrEmail, string code)
        {

            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.Result Result = new ClassCollection.Model.Result();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.code = -1;
                Result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            mobileOrEmail = mobileOrEmail.Trim();
            code = code.Trim();

            if (code == "")
            {
                Result.code = 1;
                Result.message = ClassCollection.Message.VALIDATION_CODE_INCORRECT;
                Context.Response.Write(js.Serialize(Result));
                return;
            }
            if (mobileOrEmail == "")
            {
                Result.code = 2;
                Result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                Context.Response.Write(js.Serialize(Result));
                return;
            }
            if (mobileOrEmail.Contains("@") && mobileOrEmail.Length >= 250)
            {
                Result.code = 2;
                Result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                Context.Response.Write(js.Serialize(Result));
                return;
            }
            if (!mobileOrEmail.Contains("@") && mobileOrEmail.Length != 12)
            {
                Result.code = 2;
                Result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            var db = new DataAccessDataContext();
            var exist = db.temporaryRegistrationTbls.Any(c => c.emailMobile == mobileOrEmail && c.code == code);

            if (exist)
            {
                Result.code = 0;
                Result.message = ClassCollection.Message.VALIDATION_CODE_CORRECT;
                Context.Response.Write(js.Serialize(Result));
                return;
            }
            else
            {
                Result.code = 1;
                Result.message = ClassCollection.Message.VALIDATION_CODE_INCORRECT;
                Context.Response.Write(js.Serialize(Result));
                return;
            }

        }

        /// <summary>
        /// این تابع وظیفه بررسی صحت پارامترهای وارد شده را بر عهده دارد.
        /// </summary>
        /// <param name="key">کلمه عبور برنامه ای که درحال استفاده از متد جاری می باشد</param>
        /// <param name="mobileOrEmail">شماره موبایل یا ایمیل کاربر
        /// <remarks>
        ///  <para>در صورت قرار دادن شماره موبایل در این پارامتر طول آن باید برابر با 12 کاراکتر باشد</para>
        ///  <para>در صورت قرار دادن ایمیل در این پارامتر طول آن باید کمتر از 250 کاراکتر باشد</para> 
        /// </remarks>
        /// </param>
        /// <returns>خروجی ساختار جی سون از کلاس زیر را دارد.
        /// <para><see cref="ClassCollection.Model.Result" /></para>
        /// <remarks>
        /// <para>Code : -1 , Message : <see cref="ClassCollection.Message.OPERATION_NO_ACCESS"/></para>
        /// <para>Code : 0 , Message : <see cref="ClassCollection.Message.OPERATION_SUCCESS" /></para>
        /// <para>Code : 1 , Message : <see cref="ClassCollection.Message.ERROR_INPUT_PARAMETER" /></para>
        /// <para>Code : 2 , Message : <see cref="ClassCollection.Message.EMAIL_EXIST" /></para>
        /// <para>Code : 3 , Message : <see cref="ClassCollection.Message.MOBILE_EXIST" /></para>
        /// </remarks>
        /// </returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void checkInfo(string key, string mobileOrEmail)
        {

            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.mUserInfoResult Result = new ClassCollection.Model.mUserInfoResult();

            Result.result = new Model.Result();
            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            mobileOrEmail = mobileOrEmail.Trim();

            if (mobileOrEmail == "")
            {
                Result.result.code = 1;
                Result.result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                Context.Response.Write(js.Serialize(Result));
                return;
            }
            if (mobileOrEmail.Contains("@") && mobileOrEmail.Length >= 250)
            {
                Result.result.code = 1;
                Result.result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                Context.Response.Write(js.Serialize(Result));
                return;
            }
            if (!mobileOrEmail.Contains("@") && mobileOrEmail.Length != 12)
            {
                Result.result.code = 1;
                Result.result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            var db = new DataAccessDataContext();

            if (mobileOrEmail.Contains("@"))
            {

                if ((db.mUserTbls.Any(c => c.emailtell == mobileOrEmail)))
                {
                    Result.result.code = 3;
                    Result.result.message = ClassCollection.Message.MOBILE_EXIST;
                    var u = db.mUserTbls.Single(c => c.emailtell == mobileOrEmail);

                    var up = db.partnersTbls.Single(c => c.registrationmobile == mobileOrEmail);

                    Result.userInfo = new Model.mUserInfo();
                    Result.userInfo.email = up.email;
                    Result.userInfo.emailormobile = mobileOrEmail;
                    Result.userInfo.family = u.family;
                    Result.userInfo.ID = u.ID;
                    Result.userInfo.innerTell = up.innerTell;
                    Result.userInfo.level = up.level;
                    Result.userInfo.name = u.name;
                    Result.userInfo.optionalMobile = up.optionalmobile;
                }
                else
                {
                    Result.result.code = 0;
                    Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
                }
            }
            else
            {
                if (db.mUserTbls.Any(c => c.emailtell == mobileOrEmail))
                {
                    Result.result.code = 3;
                    Result.result.message = ClassCollection.Message.MOBILE_EXIST;
                    var u = db.mUserTbls.Single(c => c.emailtell == mobileOrEmail);
                    var up = db.partnersTbls.Single(c => c.registrationmobile == mobileOrEmail);

                    Result.userInfo = new Model.mUserInfo();
                    Result.userInfo.email = up.email;
                    Result.userInfo.emailormobile = mobileOrEmail;
                    Result.userInfo.family = u.family;
                    Result.userInfo.ID = u.ID;
                    Result.userInfo.innerTell = up.innerTell;
                    Result.userInfo.level = up.level;
                    Result.userInfo.name = u.name;
                    Result.userInfo.optionalMobile = up.optionalmobile;
                }
                else
                {
                    Result.result.code = 0;
                    Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
                }
            }




            Context.Response.Write(js.Serialize(Result));
            return;
        }

        /// <summary>
        /// این متد وظیفه ارسال کلمه عبور جدید برای کاربر را برعهده دارد
        /// </summary>
        /// <param name="key">کلمه عبور برنامه ای که درحال استفاده از متد جاری می باشد</param>
        /// <param name="mobileOrEmail">شماره موبایل یا ایمیل کاربر
        /// <remarks>
        ///  <para>در صورت قرار دادن شماره موبایل در این پارامتر طول آن باید برابر با 12 کاراکتر باشد</para>
        ///  <para>در صورت قرار دادن ایمیل در این پارامتر طول آن باید کمتر از 250 کاراکتر باشد</para> 
        /// </remarks>
        /// </param>
        /// <returns>
        /// خروجی ساختار جی سون از کلاس زیر را دارد.
        /// <para><see cref="ClassCollection.Model.Result"/></para>
        /// <remarks>
        /// <para>Code : -1 , Message : <see cref="ClassCollection.Message.OPERATION_NO_ACCESS"/></para>
        /// <para>Code : 0 , Message : <see cref="ClassCollection.Message.NEW_PASSWORD_SENT"/></para>
        /// <para>Code : 1 , Message : <see cref="ClassCollection.Message.ERROR_INPUT_PARAMETER"/> </para>
        /// <para>Code : 2 , Message : <see cref="ClassCollection.Message.USER_NOT_EXIST"/> </para>
        /// </remarks>
        /// </returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void resetPassword(string key, string mobileOrEmail)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.Result Result = new ClassCollection.Model.Result();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.code = -1;
                Result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            mobileOrEmail = mobileOrEmail.Trim();

            if (mobileOrEmail == "")
            {
                Result.code = 1;
                Result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                Context.Response.Write(js.Serialize(Result));
                return;
            }
            if (mobileOrEmail.Contains("@") && mobileOrEmail.Length >= 250)
            {
                Result.code = 1;
                Result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                Context.Response.Write(js.Serialize(Result));
                return;
            }
            if (!mobileOrEmail.Contains("@") && mobileOrEmail.Length != 12)
            {
                Result.code = 1;
                Result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            var db = new DataAccessDataContext();
            if (mobileOrEmail.Contains("@"))
            {
                if (!db.mUserTbls.Any(c => c.emailtell == mobileOrEmail))
                {
                    Result.code = 2;
                    Result.message = ClassCollection.Message.USER_NOT_EXIST;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                Result.code = 0;
                Result.message = ClassCollection.Message.NEW_PASSWORD_SENT;

                var user = db.mUserTbls.Single(c => c.emailtell == mobileOrEmail);
                string pass = ClassCollection.Methods.randomValidationCode();
                user.password = ClassCollection.Methods.md5(pass);
                db.SubmitChanges();
                ClassCollection.Methods.sendMail(mobileOrEmail, "کلمه عبور جدید در اپینو", "کلمه عبور جدید : " + pass);
                Context.Response.Write(js.Serialize(Result));
                return;
            }
            else
            {
                if (!db.mUserTbls.Any(c => c.emailtell == mobileOrEmail))
                {
                    Result.code = 2;
                    Result.message = ClassCollection.Message.USER_NOT_EXIST;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }

                Result.code = 0;
                Result.message = ClassCollection.Message.NEW_PASSWORD_SENT;

                var user = db.mUserTbls.Single(c => c.emailtell == mobileOrEmail);
                string pass = ClassCollection.Methods.randomValidationCode();
                user.password = ClassCollection.Methods.md5(pass);
                db.SubmitChanges();

                ClassCollection.Methods.sendSms(mobileOrEmail, "کلمه عبور جدید در اپینو:" + pass);
                Context.Response.Write(js.Serialize(Result));
                return;

            }
        }

        /// <summary>
        /// این تابع وظیفه ثبت کاربر جدید را بر عهده دارد.
        /// </summary>
        /// <param name="key">کلمه عبور برنامه ای که درحال استفاده از متد جاری می باشد</param>
        /// <param name="mobileOrEmail">شماره موبایل یا ایمیل کاربر
        /// <remarks>
        ///  <para>در صورت قرار دادن شماره موبایل در این پارامتر طول آن باید برابر با 12 کاراکتر باشد</para>
        ///  <para>در صورت قرار دادن ایمیل در این پارامتر طول آن باید کمتر از 250 کاراکتر باید</para> 
        /// </remarks>
        /// </param>
        /// <param name="password" >کلمه عبور کاربر را مشخص می کند
        /// <remark>
        /// طول این پارامتر باید کمتر از 250 کاراکتر و بیشتر از 5 کاراکتر باشد
        /// </remark>
        /// </param>
        /// <param name="name" >نام کاربر را مشخص می کند
        /// <remark>
        /// طول این پارامتر باید کمتر از 250 کاراکتر باشد
        /// </remark>
        /// </param>
        /// <param name="family" >نام خانوادگی کاربر را مشخص می کند
        /// <remark>
        /// طول این پارامتر باید کمتر از 250 کاراکتر باشد
        /// </remark>
        /// </param>
        /// <returns>خروجی ساختار جی سون از کلاس زیر را دارد.
        /// <para><see cref="ClassCollection.Model.Result" /></para>
        /// <remarks>
        /// <para>Code : -1 , Message : <see cref="ClassCollection.Message.OPERATION_NO_ACCESS"/></para>
        /// <para>Code : 0 , Message : <see cref="ClassCollection.Message.OPERATION_SUCCESS" /></para>
        /// <para>Code : 1 , Message : <see cref="ClassCollection.Message.ERROR_INPUT_PARAMETER" /></para>
        /// <para>Code : 2 , Message : <see cref="ClassCollection.Message.ERROR_INPUT_PARAMETER" /></para>
        /// <para>Code : 3 , Message : <see cref="ClassCollection.Message.ERROR_INPUT_PARAMETER" /></para>
        /// <para>Code : 4 , Message : <see cref="ClassCollection.Message.ERROR_INPUT_PARAMETER" /></para>
        /// <para>Code : 5 , Message : <see cref="ClassCollection.Message.EMAIL_EXIST" /></para>
        /// <para>Code : 6 , Message : <see cref="ClassCollection.Message.MOBILE_EXIST" /></para>
        /// </remarks>
        /// </returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void registerUser(string key, string mobileOrEmail, string password, string name, string family, string innerTell, string email, string level, string optionalmobile)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.Result Result = new ClassCollection.Model.Result();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.code = -1;
                Result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            mobileOrEmail = mobileOrEmail.Trim();
            password = password.Trim();
            name = name.TrimEnd().TrimStart();
            family = family.TrimEnd().TrimStart();

            if (mobileOrEmail == "")
            {
                Result.code = 1;
                Result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            //if (mobileOrEmail.Contains("@") && mobileOrEmail.Length >= 250)
            //{
            //    Result.code = 1;
            //    Result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
            //    Context.Response.Write(js.Serialize(Result) );
            //    return;
            //}
            //if (!mobileOrEmail.Contains("@") && mobileOrEmail.Length != 12) 

            if (mobileOrEmail.Length != 12)
            {
                Result.code = 2;
                Result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                Context.Response.Write(js.Serialize(Result));
                return;
            }
            if (password.Length < 5 || password.Length >= 250)
            {
                Result.code = 3;
                Result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "3");
                Context.Response.Write(js.Serialize(Result));
                return;
            }
            if (name != "")
            {
                if (name.Length == 0 || name.Length >= 250)
                {
                    Result.code = 4;
                    Result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "4");
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
            }
            if (family != "")
            {
                if (family.Length == 0 || family.Length >= 250)
                {
                    Result.code = 5;
                    Result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "5");
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
            }

            DateTime dt = new DateTime();
            dt = DateTime.Now;
            var db = new DataAccessDataContext();

            //if (mobileOrEmail.Contains("@"))
            //{
            //    if (db.mUserTbls.Any(c => c.emailtell == mobileOrEmail))
            //    {
            //        Result.code = 5; 
            //        Result.message = ClassCollection.Message.EMAIL_EXIST;
            //        Context.Response.Write(js.Serialize(Result) );
            //        return;
            //    }
            //}
            //else
            //{
            if (db.mUserTbls.Any(c => c.emailtell == mobileOrEmail))
            {
                Result.code = 6;
                Result.message = ClassCollection.Message.MOBILE_EXIST;
                Context.Response.Write(js.Serialize(Result));
                return;
            }
            //}

            email = email.Trim();
            level = level.TrimEnd().TrimStart();
            optionalmobile = optionalmobile.Trim();
            innerTell = innerTell.Trim();

            if (email != "")
            {
                if (email.Contains("@") && email.Length >= 250)
                {
                    Result.code = 7;
                    Result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "7");
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
            }

            if (level != "")
            {
                if (level.Length >= 250)
                {
                    Result.code = 8;
                    Result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "8");
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
            }

            if (optionalmobile != "")
            {
                if (optionalmobile.Length != 12)
                {
                    Result.code = 9;
                    Result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "9");
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
            }



            var user = new mUserTbl();
            user.emailtell = mobileOrEmail;
            user.family = family;
            user.name = name;
            user.password = ClassCollection.Methods.md5(password);
            user.regDate = dt;
            user.lastedit = " ";
            db.mUserTbls.InsertOnSubmit(user);
            db.SubmitChanges();

            // اگر اطلاعات کاربر در دفتر تلفن قبلا توسط مدیر وارد نشده باشد اینجا اطلاعات آن در دفتر تلفن ثبت می شود.ولی اگر قبلا مدیر اطلاعات کاربر را وارد کرده باشد دیگر اطلاعات آن اینجا از کاربر گرفته نمی شودو همان قبلی ها می ماند.چون اطلاعاتی که اپراتور وارد می کند به اطلاعاتی که کاربر وارد می کند اولویت دارد
            if (!db.partnersTbls.Any(c => c.registrationmobile == mobileOrEmail))
            {
                var tmp = new partnersTbl();
                tmp.registrationmobile = mobileOrEmail;
                tmp.name = name;
                tmp.family = family;
                tmp.innerTell = innerTell;
                tmp.email = email;
                tmp.level = level;
                tmp.optionalmobile = optionalmobile;

                db.partnersTbls.InsertOnSubmit(tmp);
                db.SubmitChanges();
            }
            //else
            //{
            //    var tmp = db.partnersTbls.Single(c => c.registrationmobile == mobileOrEmail);
            //    if (name != "")
            //        tmp.name = name;
            //    if (family != "")
            //        tmp.family = family;
            //    if (innerTell != "")
            //        tmp.innerTell = innerTell;
            //    if (email != "")
            //        tmp.email = email;
            //    if (level != "")
            //        tmp.level = level;
            //    if (optionalmobile != "")
            //        tmp.optionalmobile = optionalmobile;

            //    db.SubmitChanges();
            //}

            Result.code = 0;
            Result.message = ClassCollection.Message.OPERATION_SUCCESS;

            #region حذف رکورد کد فعال سازی از جدول temporaryRegistrationTbls


            if (db.temporaryRegistrationTbls.Any(c => c.emailMobile == mobileOrEmail))
            {
                db.temporaryRegistrationTbls.DeleteAllOnSubmit(db.temporaryRegistrationTbls.Where(c => c.emailMobile == mobileOrEmail));
                db.SubmitChanges();
            }

            #endregion

            Context.Response.Write(js.Serialize(Result));
            return;

        }

        /// <summary>
        /// این تابع وظیفه ویرایش کاربر مورد نظر را بر عهده دارد.
        /// </summary>
        /// <param name="key">کلمه عبور برنامه ای که درحال استفاده از متد جاری می باشد</param>
        /// <param name="mobileOrEmail">شماره موبایل یا ایمیل کاربری که در حال ویرایش آن هستیم
        /// <remarks>
        ///  <para>در صورت قرار دادن شماره موبایل در این پارامتر طول آن باید برابر با 12 کاراکتر باشد</para>
        ///  <para>در صورت قرار دادن ایمیل در این پارامتر طول آن باید کمتر از 250 کاراکتر باید</para> 
        /// </remarks>
        /// </param>
        /// <param name="password" >کلمه عبورجدید  کاربر را مشخص می کند
        /// <remark>
        /// طول این پارامتر باید کمتر از 250 کاراکتر و بیشتر از 5 کاراکتر باشد
        /// </remark>
        /// </param>
        /// <param name="name" >نام جدید کاربر را مشخص می کند
        /// <remark>
        /// طول این پارامتر باید کمتر از 250 کاراکتر باشد
        /// </remark>
        /// </param>
        /// <param name="family" >نام خانوادگی جدید کاربر را مشخص می کند
        /// <remark>
        /// طول این پارامتر باید کمتر از 250 کاراکتر باشد
        /// </remark>
        /// </param>
        /// <returns>خروجی ساختار جی سون از کلاس زیر را دارد.
        /// <para><see cref="ClassCollection.Model.Result" /></para>
        /// <remarks>
        /// <para>Code : -1 , Message : <see cref="ClassCollection.Message.OPERATION_NO_ACCESS"/></para>
        /// <para>Code : 0 , Message : <see cref="ClassCollection.Message.OPERATION_SUCCESS" /></para>
        /// <para>Code : 1 , Message : <see cref="ClassCollection.Message.ERROR_INPUT_PARAMETER" /></para>
        /// <para>Code : 2 , Message : <see cref="ClassCollection.Message.ERROR_INPUT_PARAMETER" /></para>
        /// <para>Code : 3 , Message : <see cref="ClassCollection.Message.ERROR_INPUT_PARAMETER" /></para>
        /// <para>Code : 4 , Message : <see cref="ClassCollection.Message.ERROR_INPUT_PARAMETER" /></para>
        /// <para>Code : 5 , Message : <see cref="ClassCollection.Message.USER_NOT_EXIST" /></para>
        /// </remarks>
        /// </returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void editUser(string key, string oldPassword, string mobileOrEmail, string password, string name, string family, string innerTell, string email, string level, string optionalmobile)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.Result Result = new ClassCollection.Model.Result();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.code = -1;
                Result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            mobileOrEmail = mobileOrEmail.Trim();
            password = password.Trim();
            name = name.TrimStart().TrimEnd();
            family = family.TrimStart().TrimEnd();
            optionalmobile = optionalmobile.TrimEnd().TrimStart();
            innerTell = innerTell.TrimStart().TrimEnd();
            email = email.TrimEnd().TrimStart();

            if (mobileOrEmail == "")
            {
                Result.code = 1;
                Result.message = ClassCollection.Message.MOBILE_INCORRECT;
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            //if (mobileOrEmail.Contains("@") && mobileOrEmail.Length >= 250)
            //{
            //    Result.code = 2;
            //    Result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
            //    Context.Response.Write(js.Serialize(Result));
            //    return;
            //}
            if (!mobileOrEmail.Contains("@") && mobileOrEmail.Length != 12)
            {
                Result.code = 3;
                Result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                Context.Response.Write(js.Serialize(Result));
                return;
            }
            if (password != "" && (password.Length < 5 || password.Length >= 250))
            {
                Result.code = 4;
                Result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "3");
                Context.Response.Write(js.Serialize(Result));
                return;
            }
            if (name != "" && (name.Length >= 250))
            {
                Result.code = 5;
                Result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "3");
                Context.Response.Write(js.Serialize(Result));
                return;
            }
            if (family != "" && (family.Length >= 250))
            {
                Result.code = 6;
                Result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "4");
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            if (email != "")
            {
                if (email.Contains("@") && email.Length >= 250)
                {
                    Result.code = 7;
                    Result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "7");
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
            }

            if (level != "")
            {
                if (level.Length >= 250)
                {
                    Result.code = 8;
                    Result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "8");
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
            }

            if (optionalmobile != "")
            {
                if (optionalmobile.Length != 12)
                {
                    Result.code = 9;
                    Result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "9");
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
            }


            var db = new DataAccessDataContext();
            oldPassword = ClassCollection.Methods.md5(oldPassword);
            if (!db.mUserTbls.Any(c => c.emailtell == mobileOrEmail && c.password == oldPassword))
            {
                Result.code = 7;
                Result.message = ClassCollection.Message.PASSWORD_INCORRECT;
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            var user = db.mUserTbls.Single(c => c.emailtell == mobileOrEmail);

            if (family != "")
                user.family = family;
            if (name != "")
                user.name = name;
            if (password != "")
                user.password = ClassCollection.Methods.md5(password);

            db.SubmitChanges();


            var tmp = db.partnersTbls.Single(c => c.registrationmobile == mobileOrEmail);
            if (name != "")
                tmp.name = name;
            if (family != "")
                tmp.family = family;
            if (innerTell != "")
                tmp.innerTell = innerTell;
            if (email != "")
                tmp.email = email;
            if (level != "")
                tmp.level = level;

            if (optionalmobile != "")
                tmp.optionalmobile = optionalmobile;

            db.SubmitChanges();

            Result.code = 0;
            Result.message = ClassCollection.Message.OPERATION_SUCCESS;


            Context.Response.Write(js.Serialize(Result));
            return;

        }

        /// <summary>
        /// این تابع وظیفه بررسی وجود یا عدم کاربر مورد نظر را بر عهده دارد.
        /// </summary>
        /// <param name="key">کلمه عبور برنامه ای که درحال استفاده از متد جاری می باشد</param>
        /// <param name="mobileOrEmail">شماره موبایل یا ایمیل کاربر
        /// <remarks>
        ///  <para>در صورت قرار دادن شماره موبایل در این پارامتر طول آن باید برابر با 12 کاراکتر باشد</para>
        ///  <para>در صورت قرار دادن ایمیل در این پارامتر طول آن باید کمتر از 250 کاراکتر باید</para> 
        /// </remarks>
        /// </param>
        /// <param name="password" >کلمه عبور کاربر را مشخص می کند
        /// <remark>
        /// طول این پارامتر باید کمتر از 250 کاراکتر و بیشتر از 5 کاراکتر باشد
        /// </remark>
        /// </param>
        /// <returns>خروجی ساختار جی سون از کلاس زیر را دارد.
        /// <para><see cref="ClassCollection.Model.UserRsult" /></para>
        /// <remarks>
        /// <para>Code : -1 , Message : <see cref="ClassCollection.Message.OPERATION_NO_ACCESS"/></para>
        /// <para>Code : 0 , Message : <see cref="ClassCollection.Message.USER_EXIST" /></para>
        /// <para>Code : 1 , Message : <see cref="ClassCollection.Message.ERROR_INPUT_PARAMETER" /></para>
        /// <para>Code : 2 , Message : <see cref="ClassCollection.Message.ERROR_INPUT_PARAMETER" /></para>
        /// <para>Code : 3 , Message : <see cref="ClassCollection.Message.USER_NOT_EXIST" /></para>
        /// <para>Code : 4 , Message : <see cref="ClassCollection.Message.USER_BLOCKED" /></para>
        /// </remarks>
        /// </returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void isExistUser(string key, string mobileOrEmail, string password)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.mUserResult Result = new ClassCollection.Model.mUserResult();
            Result.result = new ClassCollection.Model.Result();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            mobileOrEmail = mobileOrEmail.Trim();
            password = password.Trim();

            if (mobileOrEmail == "")
            {
                Result.result.code = 1;
                Result.result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            if (mobileOrEmail.Contains("@") && mobileOrEmail.Length >= 250)
            {
                Result.result.code = 2;
                Result.result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                Context.Response.Write(js.Serialize(Result));
                return;
            }
            if (!mobileOrEmail.Contains("@") && mobileOrEmail.Length != 12)
            {
                Result.result.code = 3;
                Result.result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                Context.Response.Write(js.Serialize(Result));
                return;
            }
            if (password.Length < 5 || password.Length >= 250)
            {
                Result.result.code = 4;
                Result.result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "3");
                Context.Response.Write(js.Serialize(Result));
                return;
            }
            
            var db = new DataAccessDataContext();

            if (!db.mUserTbls.Any(c => c.emailtell == mobileOrEmail && c.password == ClassCollection.Methods.md5(password)))
            {
                Result.result.code = 5;
                Result.result.message = ClassCollection.Message.USER_NOT_EXIST;
                Result.user = null;
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            var user = db.mUserTbls.Single(c => c.emailtell == mobileOrEmail && c.password == ClassCollection.Methods.md5(password));
            if (user.isBlock)
            {
                Result.result.code = 6;
                Result.result.message = ClassCollection.Message.USER_BLOCKED;
                Result.user = null;
                Context.Response.Write(js.Serialize(Result));
                return;
            }


            Result.user = new ClassCollection.Model.mUser();
            Result.user.groups = new List<ClassCollection.Model.PushTo>();
            var subGroups = user.userSubGroupTbls;
            foreach (var item in subGroups)
            {
                var tmp = new ClassCollection.Model.PushTo();
                tmp.groupID = item.SubGroupTbl.groupID;
                tmp.groupTitle = item.SubGroupTbl.GroupTbl.title;
                tmp.subGroupID = item.subGroupID;
                tmp.subGroupTitle = item.SubGroupTbl.title;

                Result.user.groups.Add(tmp);
            }

            Result.user.emailormobile = user.emailtell;
            Result.user.family = user.family;
            Result.user.ID = user.ID;
            Result.user.name = user.name;
            Result.user.regDate = Persia.Calendar.ConvertToPersian(user.regDate).ToString();

            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.USER_EXIST;

            Context.Response.Write(js.Serialize(Result));
            return;

        }

        /// <summary>
        /// این تابع وظیفه ارسال لیست گروه هایی ک این کابر قادر به ارسال پوش برای آنها است را بر عهده دارد.
        /// </summary>
        /// <param name="key">کلمه عبور برنامه ای که درحال استفاده از متد جاری می باشد</param>
        /// <param name="mobileOrEmail">شماره موبایل یا ایمیل کاربر
        /// <remarks>
        ///  <para>در صورت قرار دادن شماره موبایل در این پارامتر طول آن باید برابر با 12 کاراکتر باشد</para>
        ///  <para>در صورت قرار دادن ایمیل در این پارامتر طول آن باید کمتر از 250 کاراکتر باید</para> 
        /// </remarks>
        /// </param>
        /// <returns>خروجی ساختار جی سون از کلاس زیر را دارد.
        /// <para><see cref="ClassCollection.Model.userCanPushResult" /></para>
        /// <remarks>
        /// <para>Code : -1 , Message : <see cref="ClassCollection.Message.OPERATION_NO_ACCESS"/></para>
        /// <para>Code : 0 , Message : <see cref="ClassCollection.Message.OPERATION_SUCCESS" /></para>
        /// <para>Code : 1 , Message : <see cref="ClassCollection.Message.ERROR_INPUT_PARAMETER" /></para>
        /// <para>Code : 2 , Message : <see cref="ClassCollection.Message.ERROR_INPUT_PARAMETER" /></para>
        /// <para>Code : 3 , Message : <see cref="ClassCollection.Message.USER_NOT_EXIST" /></para>
        /// <para>Code : 4 , Message : <see cref="ClassCollection.Message.USER_BLOCKED" /></para>
        /// </remarks>
        /// </returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void userCanPush(string key, string mobileOrEmail)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.userCanPushResult Result = new ClassCollection.Model.userCanPushResult();
            Result.result = new ClassCollection.Model.Result();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            mobileOrEmail = mobileOrEmail.Trim();

            if (mobileOrEmail == "")
            {
                Result.result.code = 10;
                Result.result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            if (mobileOrEmail.Contains("@") && mobileOrEmail.Length >= 250)
            {
                Result.result.code = 20;
                Result.result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                Context.Response.Write(js.Serialize(Result));
                return;
            }
            if (!mobileOrEmail.Contains("@") && mobileOrEmail.Length != 12)
            {
                Result.result.code = 30;
                Result.result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                Context.Response.Write(js.Serialize(Result));
                return;
            }
            
            var db = new DataAccessDataContext();

            if (!db.mUserTbls.Any(c => c.emailtell == mobileOrEmail))
            {
                Result.result.code = 40;
                Result.result.message = ClassCollection.Message.USER_NOT_EXIST;
                Result.groups = null;
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            var user = db.mUserTbls.Single(c => c.emailtell == mobileOrEmail);
            if (user.isBlock)
            {
                Result.result.code = 50;
                Result.result.message = ClassCollection.Message.USER_BLOCKED;
                Result.groups = null;
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            if(user.userSubGroupTbls.Any(c=>c.SubGroupTbl.toAll==true))
            {
                Result.result.code = 1;
                Result.result.message = "امکان ارسال به همه";
                Result.groups = null;
                Context.Response.Write(js.Serialize(Result));
                return;
            }
            if (user.userSubGroupTbls.Any(c => c.SubGroupTbl.toPartner == true))
            {
                Result.result.code = 2;
                Result.result.message = "امکان ارسال به همکاران";
                Result.groups = null;
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            Result.groups = new List<ClassCollection.Model.PushTo>();
            var subGroups = user.userSubGroupTbls;
            foreach (var item in subGroups)
            {
                if (db.subGroupPushTbls.Any(c => c.subGroupID == item.subGroupID))
                {
                    var subs = db.subGroupPushTbls.Where(c => c.subGroupID == item.subGroupID);
                    foreach (var it in subs)
                    {
                        if (Result.groups.Any(c => c.subGroupID == it.toSubGroupID) == false)
                        {
                            var tmp = new ClassCollection.Model.PushTo();
                            tmp.groupID = db.SubGroupTbls.Single(c => c.ID == it.toSubGroupID).groupID;
                            tmp.groupTitle = db.SubGroupTbls.Single(c => c.ID == it.toSubGroupID).GroupTbl.title;
                            tmp.subGroupID = it.toSubGroupID;
                            tmp.subGroupTitle = db.SubGroupTbls.Single(c => c.ID == it.toSubGroupID).title;

                            Result.groups.Add(tmp);
                        }
                    }
                }

            }

            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result));
            return;

        }

        /// <summary>
        /// این تابع وظیفه ارسال پوش به گروه هایی ک کاربر انتخاب کرده است را بر عهده دارد.
        /// </summary>
        /// <param name="key">کلمه عبور برنامه ای که درحال استفاده از متد جاری می باشد</param>
        /// <param name="mobileOrEmail">شماره موبایل یا ایمیل کاربر
        /// <remarks>
        ///  <para>در صورت قرار دادن شماره موبایل در این پارامتر طول آن باید برابر با 12 کاراکتر باشد</para>
        ///  <para>در صورت قرار دادن ایمیل در این پارامتر طول آن باید کمتر از 250 کاراکتر باید</para> 
        /// </remarks>
        /// </param>
        /// <param name="toSubgroups" >کد زیرگوه هایی که کاربر انتخاب کرده است
        /// <remark>
        ///کد زیرگروه ها با کاما از یکدیگر جدا میشوند
        /// </remark>
        /// </param>
        /// <returns>خروجی ساختار جی سون از کلاس زیر را دارد.
        /// <para><see cref="ClassCollection.Model.Result" /></para>
        /// <remarks>
        /// <para>Code : -1 , Message : <see cref="ClassCollection.Message.OPERATION_NO_ACCESS"/></para>
        /// <para>Code : 0 , Message : <see cref="ClassCollection.Message.OPERATION_SUCCESS" /></para>
        /// <para>Code : 1 , Message : <see cref="ClassCollection.Message.ERROR_INPUT_PARAMETER" /></para>
        /// <para>Code : 2 , Message : <see cref="ClassCollection.Message.ERROR_INPUT_PARAMETER" /></para>
        /// <para>Code : 3 , Message : <see cref="ClassCollection.Message.USER_NOT_EXIST" /></para>
        /// <para>Code : 4 , Message : <see cref="ClassCollection.Message.USER_BLOCKED" /></para>
        /// </remarks>
        /// </returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void userPush(string key, string mobileOrEmail, string text, string toSubgroups)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.Result Result = new ClassCollection.Model.Result();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.code = -1;
                Result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }
            mobileOrEmail = mobileOrEmail.Trim();

            if (mobileOrEmail == "")
            {
                Result.code = 1;
                Result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            if (mobileOrEmail.Contains("@") && mobileOrEmail.Length >= 250)
            {
                Result.code = 2;
                Result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                Context.Response.Write(js.Serialize(Result));
                return;
            }
            if (!mobileOrEmail.Contains("@") && mobileOrEmail.Length != 12)
            {
                Result.code = 3;
                Result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                Context.Response.Write(js.Serialize(Result));
                return;
            }
            
            var db = new DataAccessDataContext();

            if (!db.mUserTbls.Any(c => c.emailtell == mobileOrEmail))
            {
                Result.code = 4;
                Result.message = ClassCollection.Message.USER_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            var user = db.mUserTbls.Single(c => c.emailtell == mobileOrEmail);

            if (user.isBlock == true)
            {
                Result.code = 5;
                Result.message = ClassCollection.Message.USER_BLOCKED;
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            var groups = toSubgroups.Split(',');
            var pushto = new List<ClassCollection.Model.PushTo>();
            var subGroups = user.userSubGroupTbls;
            foreach (var item in subGroups)
            {
                if (db.subGroupPushTbls.Any(c => c.subGroupID == item.subGroupID))
                {
                    var subs = db.subGroupPushTbls.Where(c => c.subGroupID == item.subGroupID);
                    foreach (var it in subs)
                    {
                        if (pushto.Any(c => c.subGroupID == it.toSubGroupID) == false)
                        {
                            var tmp = new ClassCollection.Model.PushTo();
                            tmp.groupID = db.SubGroupTbls.Single(c => c.ID == it.toSubGroupID).groupID;
                            tmp.groupTitle = db.SubGroupTbls.Single(c => c.ID == it.toSubGroupID).GroupTbl.title;
                            tmp.subGroupID = it.toSubGroupID;
                            tmp.subGroupTitle = db.SubGroupTbls.Single(c => c.ID == it.toSubGroupID).title;

                            pushto.Add(tmp);
                        }
                    }
                }

            }


            bool breaked = false;
            var psg = "";
            foreach (var item in groups)
            {
                long sgid = 0;
                if (long.TryParse(item, out sgid))
                {
                    if (db.SubGroupTbls.Any(c => c.ID == sgid))
                    {
                        if (pushto.Any(c => c.subGroupID == sgid))
                        {
                            var sub = db.SubGroupTbls.Single(c => c.ID == sgid);
                            if (sub.canPush == true)
                            {
                                if (sub.toAll)
                                {
                                    breaked = true;
                                    //new pushservice().XsendPush("1", "userpush", user.name + " " + user.family, text, "all", null);
                                    break;
                                }
                                else if (sub.toPartner == true)
                                {
                                    breaked = true;
                                    //new pushservice().XsendPush("1", "userpush", user.name + " " + user.family, text, "partners", null);
                                    break;
                                }
                                else
                                {
                                    psg += sub.ID + ",";
                                }

                            }
                        }
                    }
                }
            }
            if (!breaked)
            {
                try
                {
                    psg = psg.Substring(0, psg.LastIndexOf(","));
                    //new pushservice().XsendPush("1", "userpush", user.name + " " + user.family, text, "subgroups", psg);
                }
                catch
                {


                }
            }

            Result.code = 0;
            Result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result));
            return;

        }

        #endregion

        #region tag

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getTagList(string key, bool isNews, bool isEvent, bool isIo, bool isPublication, bool isDownload, bool isReport)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.TagListResult Result = new ClassCollection.Model.TagListResult();
            Result.result = new ClassCollection.Model.Result();
            Result.tag = new List<Model.Tag>();


            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            if (isNews)
                Result.tag.AddRange(db.tagTbls.Where(c => c.newsTagTbls.Any()).Select(c => new Model.Tag() { ID = c.ID, title = c.title }));
            if (isEvent)
                Result.tag.AddRange(db.tagTbls.Where(c => c.eventTagTbls.Any()).Select(c => new Model.Tag() { ID = c.ID, title = c.title }));
            if (isIo)
                Result.tag.AddRange(db.tagTbls.Where(c => c.ioTagTbls.Any()).Select(c => new Model.Tag() { ID = c.ID, title = c.title }));
            if (isPublication)
                Result.tag.AddRange(db.tagTbls.Where(c => c.publicationTagTbls.Any()).Select(c => new Model.Tag() { ID = c.ID, title = c.title }));
            if (isDownload)
                Result.tag.AddRange(db.tagTbls.Where(c => c.downloadTagTbls.Any()).Select(c => new Model.Tag() { ID = c.ID, title = c.title }));
            if (isReport)
                Result.tag.AddRange(db.tagTbls.Where(c => c.masterReportTagTbls.Any()).Select(c => new Model.Tag() { ID = c.ID, title = c.title }));


            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result));
            return;
        }


        #endregion

        #region chart
        /// <summary>
        /// این تابع وظیفه ارسال نمودارها برای کاربر مورد نظر را بر عهده دارد.
        /// </summary>
        /// <param name="key">کلمه عبور برنامه ای که درحال استفاده از متد جاری می باشد</param>
        /// <param name="mobileOrEmail">شماره موبایل یا ایمیل کاربر
        /// <remarks>
        ///  <para>در صورت قرار دادن شماره موبایل در این پارامتر طول آن باید برابر با 12 کاراکتر باشد</para>
        ///  <para>در صورت قرار دادن ایمیل در این پارامتر طول آن باید کمتر از 250 کاراکتر باید</para> 
        ///  <para>در صورتی که فقط نیاز به دریافت نمودارهای عمومی می باشد مقدار این پارامتر را مقدار رشته خالی قرار دهید</para>
        /// </remarks>
        /// </param>
        /// <param name="fillterString" >رشته ی مورد نظر کاربر جهت جستجو در بین نمودارها
        /// <remarks>
        ///- اجزای جستجو با فاصله از یکدیگر جدا میشوند
        ///- درصورتی که کلمه مورد نظر در بین دو علامت دابل کوتیشن قرار بگیرد نمودارهای یافت شده حتما شامل آن کلمه خواهد بود
        /// </remarks>
        /// </param>
        /// <param name="groupID">کد گروهی که جهت استفاده در فیلتر کردن مد نظر میباشد
        /// <remarks>در صورت عدم نیاز به لحاظ این پارامتر در جستجو مقدار -1 را قرار دهید</remarks>
        /// </param>
        /// <param name="subGroupID">کد زیرگروهی که جهت استفاده در فیلتر کردن مد نظر میباشد
        /// <remarks>در صورت عدم نیاز به لحاظ این پارامتر در جستجو مقدار -1 را قرار دهید</remarks>
        /// </param>
        /// <param name="count">تعداد نتایج درخواستی در هر صفحه</param>
        /// <param name="pageIndex">شماره صفحه مورد نظر از جستجو</param>
        /// 
        /// <returns>خروجی ساختار جی سون از کلاس زیر را دارد.
        /// <para><see cref="ClassCollection.Model.LatestChartForListResult" /></para>
        /// <remarks>
        /// <para>Code : -1 , Message : <see cref="ClassCollection.Message.OPERATION_NO_ACCESS"/>, Result.charts : <c>null</c></para>
        /// <para>Code : 0 , Message : <see cref="ClassCollection.Message.OPERATION_SUCCESS" />, Result.charts : List of <see cref=" ClassCollection.Model.LatestChartForList"/></para>
        /// <para>Code : 1 , Message : <see cref="ClassCollection.Message.ERROR_INPUT_PARAMETER" />, Result.charts : <c>null</c></para>
        /// <para>Code : 2 , Message : <see cref="ClassCollection.Message.USER_NOT_EXIST" />, Result.charts : <c>null</c></para>
        /// <para>Code : 3 , Message : <see cref="ClassCollection.Message.USER_BLOCKED" />, Result.charts : <c>null</c></para>
        /// <para>Code : 4 , Message : <see cref="ClassCollection.Message.GROUP_NOT_EXIST" />, Result.charts : <c>null</c></para> 
        /// <para>Code : 5 , Message : <see cref="ClassCollection.Message.SUBGROUP_NOT_EXIST" />, Result.charts : <c>null</c></para>
        /// </remarks>
        /// </returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getLatestChartForList(string key, string mobileOrEmail, string fillterString, long groupID, long subGroupID, int count, int pageIndex, long tagID)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.LatestChartForListResult Result = new ClassCollection.Model.LatestChartForListResult();
            Result.result = new ClassCollection.Model.Result();

            var skipCount = count * (pageIndex - 1);
            var now = new DateTime();
            now = DateTime.Now.Date;
            var db = new DataAccessDataContext();
            var user = new mUserTbl();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            mobileOrEmail = mobileOrEmail.Trim();

            if (mobileOrEmail == "")
            {
                mobileOrEmail = "-1";
                user.ID = -1;
            }
            else
            {

                if (mobileOrEmail.Contains("@") && mobileOrEmail.Length >= 250)
                {
                    Result.result.code = 1;
                    Result.result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                if (!mobileOrEmail.Contains("@") && mobileOrEmail.Length != 12)
                {
                    Result.result.code = 2;
                    Result.result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }

                if (!db.mUserTbls.Any(c => c.emailtell == mobileOrEmail))
                {
                    Result.result.code = 3;
                    Result.result.message = ClassCollection.Message.USER_NOT_EXIST;
                    Result.charts = null;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                else
                {
                    user = db.mUserTbls.Single(c => c.emailtell == mobileOrEmail);
                    if (user.isBlock)
                    {
                        Result.result.code = 4;
                        Result.result.message = ClassCollection.Message.USER_BLOCKED;
                        Result.charts = null;
                        Context.Response.Write(js.Serialize(Result));
                        return;
                    }
                }

            }

            IQueryable<masterReportTbl> charts = db.masterReportTbls;

            if (tagID != -1 && db.tagTbls.Any(c => c.ID == tagID))
                charts = charts.Where(c => c.masterReportTagTbls.Any(p => p.tagID == tagID));
            #region search
            if (fillterString != "")
            {
                var search = new Search(fillterString);
                var ands = search.getAND;
                var ors = search.getOR;


                var predicate = PredicateBuilder.False<masterReportTbl>();
                foreach (string keyword in ors)
                {
                    string temp = keyword;
                    predicate = predicate.Or(p => ((p.masterRreportSubGroupTbls.Any(c => c.SubGroupTbl.userSubGroupTbls.Any(z => z.userID == user.ID) && user.ID != -1)) || (p.masterRreportSubGroupTbls.Count() == 0 && user.ID == -1))
                        &&
                        (p.title.Contains(temp) || p.masterReportContentTbls.Any(z => z.type == 0 && z.value.Contains(temp)) || (p.masterReportTagTbls.Any(x => x.tagTbl.title.Contains(temp))))

                        &&
                        (p.publishDate.Date <= now)
                        && p.isBlock == false
                        );
                }


                foreach (var keyword in ands)
                {
                    charts = charts.Where(p => ((p.masterRreportSubGroupTbls.Any(c => c.SubGroupTbl.userSubGroupTbls.Any(z => z.userID == user.ID) && user.ID != -1)) || (p.masterRreportSubGroupTbls.Count() == 0 && user.ID == -1))
                        &&
                        (p.title.Contains(keyword) || p.masterReportContentTbls.Any(z => z.type == 0 && z.value.Contains(keyword)))
                        &&
                        (p.publishDate.Date <= now)
                        && p.isBlock == false
                        );
                }
                charts = charts.Where(predicate);
            }
            else
            {
                charts = charts.Where(p => ((p.masterRreportSubGroupTbls.Any(c => c.SubGroupTbl.userSubGroupTbls.Any(z => z.userID == user.ID) && user.ID != -1)) || (p.masterRreportSubGroupTbls.Count() == 0 && user.ID == -1))
                        &&
                        (p.publishDate.Date <= now)
                        && p.isBlock == false
                        );
            }
            if (groupID != -1 && user.ID != -1)
            {
                if (db.GroupTbls.Any(c => c.ID == groupID) == false)
                {
                    Result.result.code = 5;
                    Result.result.message = ClassCollection.Message.GROUP_NOT_EXIST;
                    Result.charts = null;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                charts = charts.Where(c => c.masterRreportSubGroupTbls.Any(p => p.SubGroupTbl.groupID == groupID));
            }
            if (subGroupID != -1 && user.ID != -1)
            {
                if (db.SubGroupTbls.Any(c => c.ID == subGroupID) == false)
                {
                    Result.result.code = 6;
                    Result.result.message = ClassCollection.Message.SUBGROUP_NOT_EXIST;
                    Result.charts = null;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                charts = charts.Where(x => x.masterRreportSubGroupTbls.Any(c => c.subGroupID == subGroupID));
            }

            #endregion

            charts = charts.Where(c => c.masterReportContentTbls.Any()).OrderByDescending(c => c.publishDate);
            charts = charts.Skip(skipCount).Take(count);

            Result.charts = new List<ClassCollection.Model.LatestChartForList>();
            foreach (var item in charts)
            {
                var tmp = new ClassCollection.Model.LatestChartForList();
                tmp.ID = item.ID;
                tmp.tag = new List<Model.Tag>();
                foreach (var t in item.masterReportTagTbls)
                {
                    tmp.tag.Add(new Model.Tag() { title = t.tagTbl.title, ID = t.tagTbl.ID });
                }
                tmp.publishDate = Persia.Calendar.ConvertToPersian(item.publishDate).ToString("n");
                tmp.title = item.title;


                Result.charts.Add(tmp);
            }
            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result));
            return;

        }


        /// <summary>
        /// این تابع وظیفه ارسال نمودار برای کاربر مورد نظر را بر عهده دارد.
        /// </summary>
        /// <param name="key">کلمه عبور برنامه ای که درحال استفاده از متد جاری می باشد</param>
        /// <param name="chartID">کد نمودار مورد نظر جهت نمایش جزییات آن
        ///</param>
        /// <returns>خروجی ساختار جی سون از کلاس زیر را دارد.
        /// <para><see cref="ClassCollection.Model.ChartResult" /></para>
        /// <remarks>
        /// <para>Code : -1 , Message : <see cref="ClassCollection.Message.OPERATION_NO_ACCESS"/>, Result.fullChart : <c>null</c></para>
        /// <para>Code : 0 , Message : <see cref="ClassCollection.Message.OPERATION_SUCCESS" />, Result.fullChart : <see cref=" ClassCollection.Model.fullReport"/> </para>
        /// <para>Code : 1 , Message : <see cref="ClassCollection.Message.CHART_NOT_EXIST" />, Result.fullChart : <c>null</c></para>
        /// </remarks>
        /// </returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getChart(string key, long chartID)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.singleReportResult Result = new ClassCollection.Model.singleReportResult();
            Result.result = new ClassCollection.Model.Result();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            
            var db = new DataAccessDataContext();

            if (db.masterReportTbls.Any(c => c.ID == chartID && c.isBlock == false) == false)
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.CHART_NOT_EXIST;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            var master = db.masterReportTbls.Single(c => c.ID == chartID);

            Result.report = new Model.singleReport();
            Result.report.ID = master.ID;
            Result.report.publishDate = Persia.Calendar.ConvertToPersian(master.publishDate).ToString("n");
            Result.report.title = master.title;
            Result.report.tag = new List<Model.Tag>();
            foreach (var item in master.masterReportTagTbls)
            {
                Result.report.tag.Add(new Model.Tag() { ID = item.tagID, title = item.tagTbl.title });
            }

            Result.report.group = new List<ClassCollection.Model.ReportGroupSubGroup>();
            var subgroups = master.masterRreportSubGroupTbls;
            foreach (var item in subgroups)
            {
                var tmp = new ClassCollection.Model.ReportGroupSubGroup();
                tmp.groupID = item.SubGroupTbl.groupID;
                tmp.groupTitle = item.SubGroupTbl.GroupTbl.title;
                tmp.ID = item.ID;
                tmp.reportID = item.reportID;
                tmp.subGroupID = item.subGroupID;
                tmp.subGroupTitle = item.SubGroupTbl.title;
                Result.report.group.Add(tmp);

            }
            Result.report.content = new List<ClassCollection.Model.sreportContent>();
            var contents = master.masterReportContentTbls.OrderByDescending(c => c.ID);

            foreach (var item in contents)
            {
                var tmp = new ClassCollection.Model.sreportContent();
                tmp.ID = item.ID;
                tmp.type = item.type;
                if (tmp.type == 2)
                    tmp.value = "/chart.aspx?id=" + item.value;
                else
                    tmp.value = item.value;
                Result.report.content.Add(tmp);
            }
            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result));
            return;

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void sendChartComment(string key, long chartID, long userID, string mobileOrEmail, string fullName, string text)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.Result Result = new ClassCollection.Model.Result();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.code = -1;
                Result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            fullName = fullName.TrimEnd().TrimStart();
            text = text.TrimStart().TrimEnd();

            if (fullName.Length == 0)
            {
                Result.code = 1;
                Result.message = ClassCollection.Message.NAME_INCORRECT;

                Context.Response.Write(js.Serialize(Result));
                return;
            }
            if (text.Length == 0 || text.Length > 250)
            {
                Result.code = 2;
                Result.message = ClassCollection.Message.COMMENT_TEXT_INCORRECT;

                Context.Response.Write(js.Serialize(Result));
                return;
            }
            //if (Methods.isEmail(mobileOrEmail) == false)
            //{
            //    if (mobileOrEmail.Length != 11)
            //    {
            //        Result.code = 3;
            //        Result.message = ClassCollection.Message.MOBILE_INCORRECT;

            //        Context.Response.Write(js.Serialize(Result));
            //        return;
            //    }
            //}

            DateTime dt = new DateTime();
            dt = DateTime.Now;
            var db = new DataAccessDataContext();

            if (db.masterReportTbls.Any(c => c.ID == chartID && c.isBlock == false) == false)
            {
                Result.code = 4;
                Result.message = ClassCollection.Message.CHART_NOT_EXIST;

                Context.Response.Write(js.Serialize(Result));
                return;
            }
            if (db.mUserTbls.Any(c => c.ID == userID) == false && userID != -1)
            {
                Result.code = 5;
                Result.message = ClassCollection.Message.USER_NOT_EXIST;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            var chartComment = new reportCommentTbl();
            chartComment.fullName = fullName;
            chartComment.text = text;
            chartComment.regDate = dt;
            chartComment.reportID = chartID;
            chartComment.mUserID = userID;
            chartComment.mobile = Methods.isEmail(mobileOrEmail) ? null : mobileOrEmail;
            chartComment.email = Methods.isEmail(mobileOrEmail) ? mobileOrEmail : null;
            chartComment.isBlock = true;
            db.reportCommentTbls.InsertOnSubmit(chartComment);
            db.SubmitChanges();


            Result.code = 0;
            Result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result));
            return;

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getChartCommentList(string key, long chartID, int count, int pageIndex)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.CommentListResult Result = new ClassCollection.Model.CommentListResult();
            Result.result = new ClassCollection.Model.Result();


            var skipCount = count * (pageIndex - 1);
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }
            if (db.chartTbls.Any(c => c.ID == chartID) == false)
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.CHART_NOT_EXIST;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            Result.comment = new List<Model.Comment>();
            var comments = db.reportCommentTbls.Where(c => c.isBlock == false && c.reportID == chartID).OrderByDescending(c => c.regDate);
            Result.comment = comments.Skip(skipCount).Take(count).Select(c => new Model.Comment() { fullName = c.fullName, ID = c.ID, text = c.text }).ToList<Model.Comment>();


            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result));
            return;

        }

        #endregion

        #region news
        /// <summary>
        /// این تابع وظیفه ارسال خبر برای کاربر مورد نظر را بر عهده دارد.
        /// </summary>
        /// <param name="key">کلمه عبور برنامه ای که درحال استفاده از متد جاری می باشد</param>
        /// <param name="mobileOrEmail">شماره موبایل یا ایمیل کاربر
        /// <remarks>
        ///  <para>در صورت قرار دادن شماره موبایل در این پارامتر طول آن باید برابر با 12 کاراکتر باشد</para>
        ///  <para>در صورت قرار دادن ایمیل در این پارامتر طول آن باید کمتر از 250 کاراکتر باید</para> 
        ///  <para>در صورتی که فقط نیاز به دریافت اخبار عمومی می باشد مقدار این پارامتر را مقدار رشته خالی قرار دهید</para>
        /// </remarks>
        /// </param>
        /// <param name="fillterString" >رشته ی مورد نظر کاربر جهت جستجو در بین اخبار
        /// <remarks>
        ///- اجزای جستجو با فاصله از یکدیگر جدا میشوند
        ///- درصورتی که کلمه مورد نظر در بین دو علامت دابل کوتیشن قرار بگیرد اخبار یافت شده حتما شامل آن کلمه خواهد بود
        /// </remarks>
        /// </param>
        /// <param name="groupID">کد گروهی که جهت استفاده در فیلتر کردن مد نظر میباشد
        /// <remarks>در صورت عدم نیاز به لحاظ این پارامتر در جستجو مقدار -1 را قرار دهید</remarks>
        /// </param>
        /// <param name="subGroupID">کد زیرگروهی که جهت استفاده در فیلتر کردن مد نظر میباشد
        /// <remarks>در صورت عدم نیاز به لحاظ این پارامتر در جستجو مقدار -1 را قرار دهید</remarks>
        /// </param>
        /// <param name="count">تعداد نتایج درخواستی در هر صفحه</param>
        /// <param name="pageIndex">شماره صفحه مورد نظر از جستجو</param>
        /// 
        /// <returns>خروجی ساختار جی سون از کلاس زیر را دارد.
        /// <para><see cref="ClassCollection.Model.LatestNewsForListResult" /></para>
        /// <remarks>
        /// <para>Code : -1 , Message : <see cref="ClassCollection.Message.OPERATION_NO_ACCESS"/>, Result.news : <c>null</c></para>
        /// <para>Code : 0 , Message : <see cref="ClassCollection.Message.OPERATION_SUCCESS" />, Result.news : List of <see cref=" ClassCollection.Model.LatestNewsForListResult"/></para>
        /// <para>Code : 1 , Message : <see cref="ClassCollection.Message.ERROR_INPUT_PARAMETER" />, Result.news : <c>null</c></para>
        /// <para>Code : 2 , Message : <see cref="ClassCollection.Message.USER_NOT_EXIST" />, Result.news : <c>null</c></para>
        /// <para>Code : 3 , Message : <see cref="ClassCollection.Message.USER_BLOCKED" />, Result.news : <c>null</c></para>
        /// <para>Code : 4 , Message : <see cref="ClassCollection.Message.GROUP_NOT_EXIST" />, Result.news : <c>null</c></para> 
        /// <para>Code : 5 , Message : <see cref="ClassCollection.Message.SUBGROUP_NOT_EXIST" />, Result.news : <c>null</c></para>
        /// </remarks>
        /// </returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getLatestNewsForList(string key, string mobileOrEmail, string fillterString, long groupID, long subGroupID, int count, int pageIndex, long tagID)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.LatestNewsForListResult Result = new ClassCollection.Model.LatestNewsForListResult();
            Result.result = new ClassCollection.Model.Result();

            var skipCount = count * (pageIndex - 1);
            var now = new DateTime();
            now = DateTime.Now.Date;
            var db = new DataAccessDataContext();
            var user = new mUserTbl();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            mobileOrEmail = mobileOrEmail.Trim();

            if (mobileOrEmail == "")
            {
                mobileOrEmail = "-1";
                user.ID = -1;
            }
            else
            {

                if (mobileOrEmail.Contains("@") && mobileOrEmail.Length >= 250)
                {
                    Result.result.code = 1;
                    Result.result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                if (!mobileOrEmail.Contains("@") && mobileOrEmail.Length != 12)
                {
                    Result.result.code = 2;
                    Result.result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }

                if (!db.mUserTbls.Any(c => c.emailtell == mobileOrEmail))
                {
                    Result.result.code = 3;
                    Result.result.message = ClassCollection.Message.USER_NOT_EXIST;
                    Result.news = null;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                else
                {
                    user = db.mUserTbls.Single(c => c.emailtell == mobileOrEmail);
                    if (user.isBlock)
                    {
                        Result.result.code = 4;
                        Result.result.message = ClassCollection.Message.USER_BLOCKED;
                        Result.news = null;
                        Context.Response.Write(js.Serialize(Result));
                        return;
                    }
                }

            }

            IQueryable<newsTbl> news = db.newsTbls;

            if (tagID != -1 && db.tagTbls.Any(c => c.ID == tagID))
            {
                news = news.Where(c => c.newsTagTbls.Any(p => p.tagID == tagID));
            }

            #region search
            if (fillterString != "")
            {
                var search = new Search(fillterString);
                var ands = search.getAND;
                var ors = search.getOR;


                var predicate = PredicateBuilder.False<newsTbl>();
                foreach (string keyword in ors)
                {
                    string temp = keyword;
                    predicate = predicate.Or(p => ((p.newsSubGroupTbls.Any(c => c.SubGroupTbl.userSubGroupTbls.Any(z => z.userID == user.ID) && user.ID != -1)) || (p.newsSubGroupTbls.Count() == 0 && user.ID == -1))
                        &&
                        (p.title.Contains(temp) || p.newsContentTbls.Any(v => v.contentType == 0 && v.value.Contains(temp)) || p.newsTagTbls.Any(x => x.tagTbl.title.Contains(keyword)))
                        &&
                        (p.publishDate.Date <= now)
                        && p.isBlock == false
                        );
                }


                foreach (var keyword in ands)
                {
                    news = news.Where(p => ((p.newsSubGroupTbls.Any(c => c.SubGroupTbl.userSubGroupTbls.Any(z => z.userID == user.ID) && user.ID != -1)) || (p.newsSubGroupTbls.Count() == 0 && user.ID == -1))
                        &&
                        (p.title.Contains(keyword) || p.newsContentTbls.Any(v => v.contentType == 0 && v.value.Contains(keyword)))
                        &&
                        (p.publishDate.Date <= now)
                        && p.isBlock == false
                        );
                }
                news = news.Where(predicate);
            }
            else
            {
                news = news.Where(p => ((p.newsSubGroupTbls.Any(c => c.SubGroupTbl.userSubGroupTbls.Any(z => z.userID == user.ID) && user.ID != -1)) || (p.newsSubGroupTbls.Count() == 0 && user.ID == -1))
                        &&
                        (p.publishDate.Date <= now)
                        && p.isBlock == false
                        );
            }
            if (groupID != -1 && user.ID != -1)
            {
                if (db.GroupTbls.Any(c => c.ID == groupID) == false)
                {
                    Result.result.code = 5;
                    Result.result.message = ClassCollection.Message.GROUP_NOT_EXIST;
                    Result.news = null;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                news = news.Where(c => c.newsSubGroupTbls.Any(p => p.SubGroupTbl.groupID == groupID));
            }
            if (subGroupID != -1 && user.ID != -1)
            {
                if (db.SubGroupTbls.Any(c => c.ID == subGroupID) == false)
                {
                    Result.result.code = 6;
                    Result.result.message = ClassCollection.Message.SUBGROUP_NOT_EXIST;
                    Result.news = null;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                news = news.Where(x => x.newsSubGroupTbls.Any(c => c.subGroupID == subGroupID));
            }

            #endregion

            news = news.Where(c => c.newsContentTbls.Any()).OrderByDescending(c => c.publishDate);
            news = news.Skip(skipCount).Take(count);
            Result.news = new List<ClassCollection.Model.LatestNewsForList>();
            foreach (var item in news)
            {
                var tmp = new ClassCollection.Model.LatestNewsForList();
                tmp.ID = item.ID;
                try
                {
                    tmp.image = item.newsContentTbls.Where(c => c.contentType == 1).Take(1).Single().value;
                }
                catch
                {
                    tmp.image = "";
                }
                tmp.like = item.newsLikeTbls.Where(c => c.isLike).Count();
                tmp.unlike = item.newsLikeTbls.Where(c => !c.isLike).Count();
                tmp.viewCount = item.viewCount;
                tmp.publishDate = Persia.Calendar.ConvertToPersian(item.publishDate).ToString("n");
                tmp.tag = new List<Model.Tag>();

                tmp.title = item.title;
                try
                {
                    tmp.summary = item.newsContentTbls.Where(c => c.contentType == 0).Take(1).Single().value;
                }
                catch
                {
                    tmp.summary = item.title;
                }
                foreach (var t in item.newsTagTbls)
                {
                    tmp.tag.Add(new Model.Tag() { ID = t.tagTbl.ID, title = t.tagTbl.title });
                }
                Result.news.Add(tmp);
            }
            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result));
            return;

        }

        /// <summary>
        /// این تابع وظیفه ارسال خبر برای کاربر مورد نظر را بر عهده دارد.
        /// </summary>
        /// <param name="key">کلمه عبور برنامه ای که درحال استفاده از متد جاری می باشد</param>
        /// <param name="newsID">کد خبر مورد نظر جهت نمایش جزییات آن
        ///</param>
        /// <returns>خروجی ساختار جی سون از کلاس زیر را دارد.
        /// <para><see cref="ClassCollection.Model.NewsResult" /></para>
        /// <remarks>
        /// <para>Code : -1 , Message : <see cref="ClassCollection.Message.OPERATION_NO_ACCESS"/>, Result.news : <c>null</c></para>
        /// <para>Code : 0 , Message : <see cref="ClassCollection.Message.OPERATION_SUCCESS" />, Result.news : List of <see cref=" ClassCollection.Model.NewsResult"/> </para>
        /// <para>Code : 1 , Message : <see cref="ClassCollection.Message.NEWS_NOT_EXIST" />, Result.news : <c>null</c></para>
        /// </remarks>
        /// </returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getNews(string key, long newsID)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.NewsResult Result = new ClassCollection.Model.NewsResult();
            Result.result = new ClassCollection.Model.Result();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            
            var db = new DataAccessDataContext();

            if (db.newsTbls.Any(c => c.ID == newsID && c.isBlock == false) == false)
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.NEWS_NOT_EXIST;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            var news = db.newsTbls.Single(c => c.ID == newsID);

            Result.news = new ClassCollection.Model.fullNews();
            if (news.mUserID != null)
                Result.news.mUserID = news.mUserID;
            else
                Result.news.mUserID = -1;
            Result.news.ID = news.ID;
            Result.news.like = news.newsLikeTbls.Count(c => c.isLike == true);
            Result.news.unlike = news.newsLikeTbls.Count(c => c.isLike == false);
            Result.news.publishDate = Persia.Calendar.ConvertToPersian(news.publishDate).ToString("n");
            Result.news.title = news.title;
            Result.news.viewCount = news.viewCount + 1;

            news.viewCount += 1;
            db.SubmitChanges();

            Result.news.contents = new List<ClassCollection.Model.newsContent>();
            var contents = news.newsContentTbls.OrderBy(c => c.pri);

            foreach (var item in contents)
            {
                var tmp = new ClassCollection.Model.newsContent();
                tmp.ID = item.ID;
                tmp.type = item.contentType;
                tmp.value = item.value;
                Result.news.contents.Add(tmp);
            }

            Result.news.groups = new List<ClassCollection.Model.newsGroupSubGroup>();
            var subgroups = news.newsSubGroupTbls;
            foreach (var item in subgroups)
            {
                var tmp = new ClassCollection.Model.newsGroupSubGroup();
                tmp.groupID = item.SubGroupTbl.groupID;
                tmp.groupTitle = item.SubGroupTbl.GroupTbl.title;
                tmp.ID = item.ID;
                tmp.newsID = item.newsID;
                tmp.subGroupID = item.subGroupID;
                tmp.subGroupTitle = item.SubGroupTbl.title;
                Result.news.groups.Add(tmp);

            }

            Result.news.tag = new List<Model.Tag>();
            foreach (var t in news.newsTagTbls)
            {
                Result.news.tag.Add(new Model.Tag() { ID = t.tagTbl.ID, title = t.tagTbl.title });
            }

            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Context.Response.Write(js.Serialize(Result));
            return;
        }

        /// <summary>
        /// این تابع وظیفه ارسال خبر برای کاربر مورد نظر را بر عهده دارد.
        /// </summary>
        /// <param name="key">کلمه عبور برنامه ای که درحال استفاده از متد جاری می باشد</param>
        /// <param name="newsID">کد خبر مورد نظر جهت نمایش جزییات آن
        ///</param>
        /// <returns>خروجی ساختار جی سون از کلاس زیر را دارد.
        /// <para><see cref="ClassCollection.Model.SingleNewsResult" /></para>
        /// <remarks>
        /// <para>Code : -1 , Message : <see cref="ClassCollection.Message.OPERATION_NO_ACCESS"/>, Result.news : <c>null</c></para>
        /// <para>Code : 0 , Message : <see cref="ClassCollection.Message.OPERATION_SUCCESS" />, Result.news : <see cref=" ClassCollection.Model.singleFullNews"/> </para>
        /// <para>Code : 1 , Message : <see cref="ClassCollection.Message.NEWS_NOT_EXIST" />, Result.news : <c>null</c></para>
        /// </remarks>
        /// </returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getSingleNews(string key, long newsID)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.SingleNewsResult Result = new ClassCollection.Model.SingleNewsResult();
            Result.result = new ClassCollection.Model.Result();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }
            
            var db = new DataAccessDataContext();

            if (db.newsTbls.Any(c => c.ID == newsID && c.isBlock == false) == false)
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.NEWS_NOT_EXIST;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            var news = db.newsTbls.Single(c => c.ID == newsID);

            Result.singlenews = new ClassCollection.Model.singleFullNews();
            Result.singlenews.userID = news.userID;
            Result.singlenews.ID = news.ID;
            Result.singlenews.like = news.newsLikeTbls.Count(c => c.isLike == true);
            Result.singlenews.unlike = news.newsLikeTbls.Count(c => c.isLike == false);
            Result.singlenews.publishDate = Persia.Calendar.ConvertToPersian(news.publishDate).ToString("n");
            Result.singlenews.title = news.title;
            Result.singlenews.viewCount = news.viewCount + 1;

            news.viewCount += 1;
            db.SubmitChanges();

            var contents = news.newsContentTbls.OrderBy(c => c.pri);

            try
            {
                Result.singlenews.image = contents.First(c => c.contentType == 1).value;
            }
            catch
            {
                Result.singlenews.image = "";
            }

            try
            {
                Result.singlenews.video = contents.First(c => c.contentType == 2).value;
            }
            catch
            {
                Result.singlenews.video = "";
            }
            try
            {
                Result.singlenews.text = contents.First(c => c.contentType == 0).value;
            }
            catch
            {
                Result.singlenews.text = "";
            }


            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result));
            return;

        }

        /// <summary>
        /// این تابع وظیفه ثبت لایک و آنلایک برای خبر توسط کاربر مورد نظر را بر عهده دارد.
        /// </summary>
        /// <param name="key">کلمه عبور برنامه ای که درحال استفاده از متد جاری می باشد</param>
        /// <param name="newsID">کد خبر مورد نظر جهت ثبت نظر برای آن
        ///</param>
        /// <param name="userID">کد کاربری که در حال ثبت نظر برای خبر می باشد
        /// <remarks>
        /// در صورتی که کاربر مهمان می باشد مقدار -1 را قرار دهید
        /// </remarks>
        ///</param>
        /// <param name="isLike">نوع نظر درحال ثبت کاربر برای خبر
        /// <remarks>
        /// <para>True : لایک برای خبر درج میشود</para>
        /// <para>False : آنلایک برای خبر درج میشود</para>
        /// </remarks>
        ///</param>
        /// <returns>خروجی ساختار جی سون از کلاس زیر را دارد.
        /// <para><see cref="ClassCollection.Model.Result" /></para>
        /// <remarks>
        /// <para>Code : -1 , Message : <see cref="ClassCollection.Message.OPERATION_NO_ACCESS"/></para>
        /// <para>Code : 0 , Message : <see cref="ClassCollection.Message.OPERATION_SUCCESS" /></para>
        /// <para>Code : 1 , Message : <see cref="ClassCollection.Message.NEWS_NOT_EXIST" /></para>
        /// <para>Code : 2 , Message : <see cref="ClassCollection.Message.USER_NOT_EXIST" /></para>
        /// </remarks>
        /// </returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void likeUnlikeNews(string key, long userID, long newsID, bool isLike)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.Result Result = new ClassCollection.Model.Result();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.code = -1;
                Result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            DateTime dt = new DateTime();
            dt = DateTime.Now;
            var db = new DataAccessDataContext();

            if (db.newsTbls.Any(c => c.ID == newsID && c.isBlock == false) == false)
            {
                Result.code = 1;
                Result.message = ClassCollection.Message.NEWS_NOT_EXIST;

                Context.Response.Write(js.Serialize(Result));
                return;
            }
            if (userID != -1)
            {
                if (db.mUserTbls.Any(c => c.ID == userID && c.isBlock == false) == false)
                {
                    Result.code = 2;
                    Result.message = ClassCollection.Message.USER_NOT_EXIST;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }

            }

            if (db.newsLikeTbls.Any(c => c.userID == userID && c.newsID == newsID) == false)
            {
                var newslike = new newsLikeTbl();
                newslike.isLike = isLike;
                newslike.newsID = newsID;
                newslike.userID = userID == -1 ? 3 : userID;
                newslike.regDate = dt;
                db.newsLikeTbls.InsertOnSubmit(newslike);
                db.SubmitChanges();
            }

            Result.code = 0;
            Result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result));
            return;

        }

        /// <summary>
        /// این تابع وظیفه ارسال اخبار دریاقت نشده برای کاربر مورد نظر را بر عهده دارد.
        /// </summary>
        /// <param name="key">کلمه عبور برنامه ای که درحال استفاده از متد جاری می باشد</param>
        /// <param name="mobileOrEmail">شماره موبایل یا ایمیل کاربر
        /// <remarks>
        ///  <para>در صورت قرار دادن شماره موبایل در این پارامتر طول آن باید برابر با 12 کاراکتر باشد</para>
        ///  <para>در صورت قرار دادن ایمیل در این پارامتر طول آن باید کمتر از 250 کاراکتر باید</para> 
        ///  <para>در صورتی که فقط نیاز به دریافت اخبار عمومی می باشد مقدار این پارامتر را مقدار رشته خالی قرار دهید</para>
        /// </remarks>
        /// </param>
        /// <param name="lastNewsID" >کد آخرین خبر جهت ارسال اخبار درج شده ی بعد از آن
        /// </param>
        /// <returns>خروجی ساختار جی سون از کلاس زیر را دارد.
        /// <para><see cref="ClassCollection.Model.LatestNewsForNotificationResult" /></para>
        /// <remarks>
        /// <para>Code : -1, Message : <see cref="ClassCollection.Message.OPERATION_NO_ACCESS"/>, Result.news : <c>null</c></para>
        /// <para>Code : 0 , Message : <see cref="ClassCollection.Message.OPERATION_SUCCESS" />, Result.news : List of <see cref=" ClassCollection.Model.LatestNewsForNotification"/></para>
        /// <para>Code : 1 , Message : <see cref="ClassCollection.Message.ERROR_INPUT_PARAMETER" />, Result.news : <c>null</c></para>
        /// <para>Code : 2 , Message : <see cref="ClassCollection.Message.USER_NOT_EXIST" />, Result.news : <c>null</c></para>
        /// <para>Code : 3 , Message : <see cref="ClassCollection.Message.USER_BLOCKED" />, Result.news : <c>null</c></para>
        /// </remarks>
        /// </returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getLatestNewsForNotification(string key, string mobileOrEmail, long lastNewsID)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.LatestNewsForNotificationResult Result = new ClassCollection.Model.LatestNewsForNotificationResult();
            Result.result = new ClassCollection.Model.Result();

            DateTime dt = new DateTime();
            dt = DateTime.Now.Date;

            var db = new DataAccessDataContext();
            var user = new mUserTbl();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            mobileOrEmail = mobileOrEmail.Trim();

            if (mobileOrEmail == "")
            {
                mobileOrEmail = "-1";
                user.ID = -1;
            }
            else
            {

                if (mobileOrEmail.Contains("@") && mobileOrEmail.Length >= 250)
                {
                    Result.result.code = 1;
                    Result.result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                if (!mobileOrEmail.Contains("@") && mobileOrEmail.Length != 12)
                {
                    Result.result.code = 2;
                    Result.result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }

                if (!db.mUserTbls.Any(c => c.emailtell == mobileOrEmail))
                {
                    Result.result.code = 3;
                    Result.result.message = ClassCollection.Message.USER_NOT_EXIST;
                    Result.news = null;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                else
                {
                    user = db.mUserTbls.Single(c => c.emailtell == mobileOrEmail);
                    if (user.isBlock)
                    {
                        Result.result.code = 4;
                        Result.result.message = ClassCollection.Message.USER_BLOCKED;
                        Result.news = null;
                        Context.Response.Write(js.Serialize(Result));
                        return;
                    }
                }

            }

            IQueryable<newsTbl> news = db.newsTbls;

            news = news.Where(p => (((p.newsSubGroupTbls.Any(c => c.SubGroupTbl.userSubGroupTbls.Any(z => z.userID == user.ID) && user.ID != -1)) || (p.newsSubGroupTbls.Count() == 0 && user.ID == -1)) && p.ID > lastNewsID)
                    &&
                    (p.publishDate.Date <= dt)
                    && p.isBlock == false
                    );

            news = news.OrderByDescending(c => c.publishDate);

            Result.news = new List<ClassCollection.Model.LatestNewsForNotification>();
            foreach (var item in news)
            {
                var tmp = new ClassCollection.Model.LatestNewsForNotification();
                tmp.ID = item.ID;
                tmp.title = item.title;

                Result.news.Add(tmp);
            }
            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result));
            return;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void sendNewsComment(string key, long newsID, long userID, string mobileOrEmail, string fullName, string text)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.Result Result = new ClassCollection.Model.Result();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.code = -1;
                Result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            fullName = fullName.TrimEnd().TrimStart();
            text = text.TrimStart().TrimEnd();

            if (fullName.Length == 0)
            {
                Result.code = 1;
                Result.message = ClassCollection.Message.NAME_INCORRECT;

                Context.Response.Write(js.Serialize(Result));
                return;
            }
            if (text.Length == 0 || text.Length > 250)
            {
                Result.code = 2;
                Result.message = ClassCollection.Message.COMMENT_TEXT_INCORRECT;

                Context.Response.Write(js.Serialize(Result));
                return;
            }
            //if(Methods.isEmail(mobileOrEmail)==false)
            //{
            //    if(mobileOrEmail.Length!=11)
            //    {
            //        Result.code =3;
            //        Result.message = ClassCollection.Message.MOBILE_INCORRECT;

            //        Context.Response.Write(js.Serialize(Result));
            //        return;
            //    }
            //}

            DateTime dt = new DateTime();
            dt = DateTime.Now;
            var db = new DataAccessDataContext();

            if (db.newsTbls.Any(c => c.ID == newsID && c.isBlock == false) == false)
            {
                Result.code = 4;
                Result.message = ClassCollection.Message.NEWS_NOT_EXIST;

                Context.Response.Write(js.Serialize(Result));
                return;
            }
            if (db.mUserTbls.Any(c => c.ID == userID) == false && userID != -1)
            {
                Result.code = 5;
                Result.message = ClassCollection.Message.USER_NOT_EXIST;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            var newsComment = new newsCommentTbl();
            newsComment.fullName = fullName;
            newsComment.text = text;
            newsComment.regDate = dt;
            newsComment.newsID = newsID;
            newsComment.mUserID = userID;
            newsComment.mobile = Methods.isEmail(mobileOrEmail) ? null : mobileOrEmail;
            newsComment.email = Methods.isEmail(mobileOrEmail) ? mobileOrEmail : null;
            newsComment.isBlock = true;
            db.newsCommentTbls.InsertOnSubmit(newsComment);
            db.SubmitChanges();


            Result.code = 0;
            Result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result));
            return;

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getNewsCommentList(string key, long newsID, int count, int pageIndex)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.CommentListResult Result = new ClassCollection.Model.CommentListResult();
            Result.result = new ClassCollection.Model.Result();


            var skipCount = count * (pageIndex - 1);
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }
            if (db.newsTbls.Any(c => c.ID == newsID) == false)
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.NEWS_NOT_EXIST;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            Result.comment = new List<Model.Comment>();
            var comments = db.newsCommentTbls.Where(c => c.isBlock == false && c.newsID == newsID).OrderByDescending(c => c.regDate);
            Result.comment = comments.Skip(skipCount).Take(count).Select(c => new Model.Comment() { fullName = c.fullName, ID = c.ID, text = c.text }).ToList<Model.Comment>();


            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result));
            return;

        }

        #endregion

        #region events
        /// <summary>
        /// این تابع وظیفه ارسال رویدادهایی که در روز جاری بوده یا شروع آن از تاریخ امروز به بعد می باشد برای کاربر مورد نظر را بر عهده دارد.
        /// </summary>
        /// <param name="key">کلمه عبور برنامه ای که درحال استفاده از متد جاری می باشد</param>
        /// <param name="mobileOrEmail">شماره موبایل یا ایمیل کاربر
        /// <remarks>
        ///  <para>در صورت قرار دادن شماره موبایل در این پارامتر طول آن باید برابر با 12 کاراکتر باشد</para>
        ///  <para>در صورت قرار دادن ایمیل در این پارامتر طول آن باید کمتر از 250 کاراکتر باید</para> 
        ///  <para>در صورتی که فقط نیاز به دریافت رویدادهای عمومی می باشد مقدار این پارامتر را مقدار رشته خالی قرار دهید</para>
        /// </remarks>
        /// </param>
        /// <param name="fillterString" >رشته ی مورد نظر کاربر جهت جستجو در بین رویدادها
        /// <remarks>
        ///- اجزای جستجو با فاصله از یکدیگر جدا میشوند
        ///- درصورتی که کلمه مورد نظر در بین دو علامت دابل کوتیشن قرار بگیرد رویداد یافت شده حتما شامل آن کلمه خواهد بود
        /// </remarks>
        /// </param>
        /// <param name="groupID">کد گروهی که جهت استفاده در فیلتر کردن مد نظر میباشد
        /// <remarks>در صورت عدم نیاز به لحاظ این پارامتر در جستجو مقدار -1 را قرار دهید</remarks>
        /// </param>
        /// <param name="subGroupID">کد زیرگروهی که جهت استفاده در فیلتر کردن مد نظر میباشد
        /// <remarks>در صورت عدم نیاز به لحاظ این پارامتر در جستجو مقدار -1 را قرار دهید</remarks>
        /// </param>
        /// <param name="count">تعداد نتایج درخواستی در هر صفحه</param>
        /// <param name="pageIndex">شماره صفحه مورد نظر از جستجو</param>
        /// 
        /// <returns>خروجی ساختار جی سون از کلاس زیر را دارد.
        /// <para><see cref="ClassCollection.Model.LatestEventsForListResult" /></para>
        /// <remarks>
        /// <para>Code : -1 , Message : <see cref="ClassCollection.Message.OPERATION_NO_ACCESS"/>, Result.events : <c>null</c></para>
        /// <para>Code : 0 , Message : <see cref="ClassCollection.Message.OPERATION_SUCCESS" />, Result.events : List of <see cref=" ClassCollection.Model.LatestEventsForList"/></para>
        /// <para>Code : 1 , Message : <see cref="ClassCollection.Message.ERROR_INPUT_PARAMETER" />, Result.events : <c>null</c></para>
        /// <para>Code : 2 , Message : <see cref="ClassCollection.Message.USER_NOT_EXIST" />, Result.events : <c>null</c></para>
        /// <para>Code : 3 , Message : <see cref="ClassCollection.Message.USER_BLOCKED" />, Result.events : <c>null</c></para> 
        /// <para>Code : 4 , Message : <see cref="ClassCollection.Message.GROUP_NOT_EXIST" />, Result.events : <c>null</c></para> 
        /// <para>Code : 5 , Message : <see cref="ClassCollection.Message.SUBGROUP_NOT_EXIST" />, Result.events : <c>null</c></para>
        /// </remarks>
        /// </returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getAllEvents(string key, string mobileOrEmail, string fillterString, long groupID, long subGroupID, int count, int pageIndex, long tagID)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.LatestEventsForListResult Result = new ClassCollection.Model.LatestEventsForListResult();
            Result.result = new ClassCollection.Model.Result();

            var skipCount = count * (pageIndex - 1);
            DateTime dt = new DateTime();
            dt = DateTime.Now.Date;

            var db = new DataAccessDataContext();
            var user = new mUserTbl();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            mobileOrEmail = mobileOrEmail.Trim();

            if (mobileOrEmail == "")
            {
                mobileOrEmail = "-1";
                user.ID = -1;
            }
            else
            {

                if (mobileOrEmail.Contains("@") && mobileOrEmail.Length >= 250)
                {
                    Result.result.code = 1;
                    Result.result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                if (!mobileOrEmail.Contains("@") && mobileOrEmail.Length != 12)
                {
                    Result.result.code = 2;
                    Result.result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }

                if (!db.mUserTbls.Any(c => c.emailtell == mobileOrEmail))
                {
                    Result.result.code = 3;
                    Result.result.message = ClassCollection.Message.USER_NOT_EXIST;
                    Result.events = null;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                else
                {
                    user = db.mUserTbls.Single(c => c.emailtell == mobileOrEmail);
                    if (user.isBlock)
                    {
                        Result.result.code = 4;
                        Result.result.message = ClassCollection.Message.USER_BLOCKED;
                        Result.events = null;
                        Context.Response.Write(js.Serialize(Result));
                        return;
                    }
                }

            }

            IQueryable<eventsTbl> events = db.eventsTbls;

            if (tagID != -1 && db.tagTbls.Any(c => c.ID == tagID))
                events = events.Where(c => c.eventTagTbls.Any(p => p.tagID == tagID));

            #region search
            if (fillterString != "")
            {
                var search = new Search(fillterString);
                var ands = search.getAND;
                var ors = search.getOR;


                var predicate = PredicateBuilder.False<eventsTbl>();
                foreach (string keyword in ors)
                {
                    string temp = keyword;
                    predicate = predicate.Or(p => ((p.eventsSubGroupTbls.Any(c => c.SubGroupTbl.userSubGroupTbls.Any(z => z.userID == user.ID) && user.ID != -1)) || (p.eventsSubGroupTbls.Count() == 0 && user.ID == -1))
                        &&
                        (p.title.Contains(temp) || p.eventsContentTbls.Any(v => v.contentType == 0 && v.value.Contains(temp)) || p.eventTagTbls.Any(x => x.tagTbl.title.Contains(keyword)))
                        //&& آقای نادری گفت فعلا برداریم این شرط رو
                        //((p.toDate.Date >= dt.Date && p.fromDate <= dt.Date) || (p.fromDate.Date >= dt.Date))
                        && p.isBlock == false
                        );
                }

                foreach (var keyword in ands)
                {
                    events = events.Where(p => ((p.eventsSubGroupTbls.Any(c => c.SubGroupTbl.userSubGroupTbls.Any(z => z.userID == user.ID) && user.ID != -1)) || (p.eventsSubGroupTbls.Count() == 0 && user.ID == -1))
                        &&
                        (p.title.Contains(keyword) || p.eventsContentTbls.Any(v => v.contentType == 0 && v.value.Contains(keyword)))
                        //&& آقای نادری گفت فعلا برداریم این شرط رو
                        //((p.toDate.Date >= dt.Date && p.fromDate <= dt.Date) || (p.fromDate.Date >= dt.Date))
                        && p.isBlock == false
                        );
                }
                events = events.Where(predicate);
            }
            else
            {
                events = events.Where(p => ((p.eventsSubGroupTbls.Any(c => c.SubGroupTbl.userSubGroupTbls.Any(z => z.userID == user.ID) && user.ID != -1)) || (p.eventsSubGroupTbls.Count() == 0 && user.ID==-1))
                        //&& آقای نادری گفت فعلا برداریم این شرط رو
                        //((p.toDate.Date >= dt.Date && p.fromDate <= dt.Date) || (p.fromDate.Date >= dt.Date))
                        && p.isBlock == false
                        );
            }


            if (groupID != -1 && user.ID != -1)
            {
                if (db.GroupTbls.Any(c => c.ID == groupID) == false)
                {
                    Result.result.code = 5;
                    Result.result.message = ClassCollection.Message.GROUP_NOT_EXIST;
                    Result.events = null;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                events = events.Where(c => c.eventsSubGroupTbls.Any(p => p.SubGroupTbl.groupID == groupID));
            }
            if (subGroupID != -1 && user.ID != -1)
            {
                if (db.SubGroupTbls.Any(c => c.ID == subGroupID) == false)
                {
                    Result.result.code = 6;
                    Result.result.message = ClassCollection.Message.SUBGROUP_NOT_EXIST;
                    Result.events = null;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                events = events.Where(x => x.eventsSubGroupTbls.Any(c => c.subGroupID == subGroupID));
            }

            #endregion

            events = events.Where(c => c.eventsContentTbls.Any()).OrderByDescending(c => c.fromDate);
            events = events.Skip(skipCount).Take(count);
            Result.events = new List<ClassCollection.Model.LatestEventsForList>();
            foreach (var item in events)
            {
                var tmp = new ClassCollection.Model.LatestEventsForList();
                tmp.ID = item.ID;
                try
                {
                    tmp.image = item.eventsContentTbls.Where(c => c.contentType == 1).Take(1).Single().value;
                }
                catch
                {
                    tmp.image = "";
                }
                tmp.like = item.eventsLikeTbls.Where(c => c.isLike).Count();
                tmp.unlike = item.eventsLikeTbls.Where(c => !c.isLike).Count();
                tmp.viewCount = item.viewCount;
                tmp.toDate = Persia.Calendar.ConvertToPersian(item.toDate).ToString("n");
                tmp.fromDate = Persia.Calendar.ConvertToPersian(item.fromDate).ToString("n");
                tmp.tag = new List<Model.Tag>();
                tmp.title = item.title;
                try
                {
                    tmp.summary = item.eventsContentTbls.Where(c => c.contentType == 0).Take(1).Single().value;
                }
                catch
                {
                    tmp.summary = item.title;
                }
                foreach (var t in item.eventTagTbls)
                {
                    tmp.tag.Add(new Model.Tag() { ID = t.tagTbl.ID, title = t.tagTbl.title });
                }
                Result.events.Add(tmp);
            }
            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result));
            return;

        }

        /// <summary>
        /// این تابع وظیفه ارسال رویداد برای کاربر مورد نظر را بر عهده دارد.
        /// </summary>
        /// <param name="key">کلمه عبور برنامه ای که درحال استفاده از متد جاری می باشد</param>
        /// <param name="mobileOrEmail">شماره موبایل یا ایمیل کاربر
        /// <remarks>
        ///  <para>در صورت قرار دادن شماره موبایل در این پارامتر طول آن باید برابر با 12 کاراکتر باشد</para>
        ///  <para>در صورت قرار دادن ایمیل در این پارامتر طول آن باید کمتر از 250 کاراکتر باید</para> 
        ///  <para>در صورتی که فقط نیاز به دریافت رویدادهای عمومی می باشد مقدار این پارامتر را مقدار رشته خالی قرار دهید</para>
        /// </remarks>
        /// </param>
        /// <param name="date">
        /// تاریخ مورد نظر جهت دریافت رویدادهای موجود در آن
        /// <remarks>
        /// فرمت ارسال تاریخ باید به شکل روبرو باشد : YYYY-MM-DD
        /// </remarks>
        /// </param>
        /// <param name="fillterString" >رشته ی مورد نظر کاربر جهت جستجو در بین رویدادها
        /// <remarks>
        ///- اجزای جستجو با فاصله از یکدیگر جدا میشوند
        ///- درصورتی که کلمه مورد نظر در بین دو علامت دابل کوتیشن قرار بگیرد رویداد یافت شده حتما شامل آن کلمه خواهد بود
        /// </remarks>
        /// </param>
        /// <param name="groupID">کد گروهی که جهت استفاده در فیلتر کردن مد نظر میباشد
        /// <remarks>در صورت عدم نیاز به لحاظ این پارامتر در جستجو مقدار -1 را قرار دهید</remarks>
        /// </param>
        /// <param name="subGroupID">کد زیرگروهی که جهت استفاده در فیلتر کردن مد نظر میباشد
        /// <remarks>در صورت عدم نیاز به لحاظ این پارامتر در جستجو مقدار -1 را قرار دهید</remarks>
        /// </param>
        /// <param name="count">تعداد نتایج درخواستی در هر صفحه</param>
        /// <param name="pageIndex">شماره صفحه مورد نظر از جستجو</param>
        /// 
        /// <returns>خروجی ساختار جی سون از کلاس زیر را دارد.
        /// <para><see cref="ClassCollection.Model.LatestEventsForListResult" /></para>
        /// <remarks>
        /// <para>Code : -1 , Message : <see cref="ClassCollection.Message.OPERATION_NO_ACCESS"/>, Result.events : <c>null</c></para>
        /// <para>Code : 0 , Message : <see cref="ClassCollection.Message.OPERATION_SUCCESS" />, Result.events : List of <see cref=" ClassCollection.Model.LatestEventsForList"/></para>
        /// <para>Code : 1 , Message : <see cref="ClassCollection.Message.DATE_INCORRECT" />, Result.events : <c>null</c></para>
        /// <para>Code : 2 , Message : <see cref="ClassCollection.Message.ERROR_INPUT_PARAMETER" />, Result.events : <c>null</c></para>
        /// <para>Code : 3 , Message : <see cref="ClassCollection.Message.USER_NOT_EXIST" />, Result.events : <c>null</c></para>
        /// <para>Code : 4 , Message : <see cref="ClassCollection.Message.USER_BLOCKED" />, Result.events : <c>null</c></para> 
        /// <para>Code : 5 , Message : <see cref="ClassCollection.Message.GROUP_NOT_EXIST" />, Result.events : <c>null</c></para> 
        /// <para>Code : 6 , Message : <see cref="ClassCollection.Message.SUBGROUP_NOT_EXIST" />, Result.events : <c>null</c></para>
        /// </remarks>
        /// </returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getLatestEventsForList(string key, string mobileOrEmail, string date, string fillterString, long groupID, long subGroupID, int count, int pageIndex)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.LatestEventsForListResult Result = new ClassCollection.Model.LatestEventsForListResult();
            Result.result = new ClassCollection.Model.Result();

            var skipCount = count * (pageIndex - 1);
            var Date = new DateTime();

            var db = new DataAccessDataContext();
            var user = new mUserTbl();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            mobileOrEmail = mobileOrEmail.Trim();
            date = date.Trim();

            int pyear = 0, pmonth = 0, pday = 0;
            var DateArray = date.Split('-');
            if (DateArray.Count() != 3
               || !int.TryParse(DateArray[0], out pyear)
               || !int.TryParse(DateArray[1], out pmonth)
               || !int.TryParse(DateArray[2], out pday))
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.DATE_INCORRECT;
                Context.Response.Write(js.Serialize(Result));
                return;
            }
            try
            {
                Date = new DateTime(pyear, pmonth, pday, 0, 0, 0);
            }
            catch
            {
                Result.result.code = 2;
                Result.result.message = ClassCollection.Message.DATE_INCORRECT;
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            if (mobileOrEmail == "")
            {
                mobileOrEmail = "-1";
                user.ID = -1;
            }
            else
            {

                if (mobileOrEmail.Contains("@") && mobileOrEmail.Length >= 250)
                {
                    Result.result.code = 3;
                    Result.result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                if (!mobileOrEmail.Contains("@") && mobileOrEmail.Length != 12)
                {
                    Result.result.code = 4;
                    Result.result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }

                if (!db.mUserTbls.Any(c => c.emailtell == mobileOrEmail))
                {
                    Result.result.code = 5;
                    Result.result.message = ClassCollection.Message.USER_NOT_EXIST;
                    Result.events = null;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                else
                {
                    user = db.mUserTbls.Single(c => c.emailtell == mobileOrEmail);
                    if (user.isBlock)
                    {
                        Result.result.code = 6;
                        Result.result.message = ClassCollection.Message.USER_BLOCKED;
                        Result.events = null;
                        Context.Response.Write(js.Serialize(Result));
                        return;
                    }
                }

            }

            IQueryable<eventsTbl> events = db.eventsTbls;

            #region search
            if (fillterString != "")
            {
                var search = new Search(fillterString);
                var ands = search.getAND;
                var ors = search.getOR;


                var predicate = PredicateBuilder.False<eventsTbl>();
                foreach (string keyword in ors)
                {
                    string temp = keyword;
                    predicate = predicate.Or(p => ((p.eventsSubGroupTbls.Any(c => c.SubGroupTbl.userSubGroupTbls.Any(z => z.userID == user.ID) && user.ID != -1)) || (p.eventsSubGroupTbls.Count() == 0 && user.ID == -1))
                        &&
                        (p.title.Contains(temp) || p.eventsContentTbls.Any(v => v.contentType == 0 && v.value.Contains(temp)) || p.eventTagTbls.Any(x => x.tagTbl.title.Contains(temp)))
                        &&
                        (p.toDate.Date >= Date && p.fromDate <= Date)
                        && p.isBlock == false
                        );
                }


                foreach (var keyword in ands)
                {
                    events = events.Where(p => ((p.eventsSubGroupTbls.Any(c => c.SubGroupTbl.userSubGroupTbls.Any(z => z.userID == user.ID) && user.ID != -1)) || (p.eventsSubGroupTbls.Count() == 0 && user.ID == -1))
                        &&
                        (p.title.Contains(keyword) || p.eventsContentTbls.Any(v => v.contentType == 0 && v.value.Contains(keyword)))
                        &&
                        (p.toDate.Date >= Date && p.fromDate <= Date)
                        && p.isBlock == false
                        );
                }
                events = events.Where(predicate);
            }
            else
            {
                events = events.Where(p => ((p.eventsSubGroupTbls.Any(c => c.SubGroupTbl.userSubGroupTbls.Any(z => z.userID == user.ID) && user.ID != -1)) || (p.eventsSubGroupTbls.Count() == 0 && user.ID == -1))
                        &&
                        (p.toDate.Date >= Date && p.fromDate <= Date)
                        && p.isBlock == false
                        );
            }
            if (groupID != -1 && user.ID != -1)
            {
                if (db.GroupTbls.Any(c => c.ID == groupID) == false)
                {
                    Result.result.code = 7;
                    Result.result.message = ClassCollection.Message.GROUP_NOT_EXIST;
                    Result.events = null;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                events = events.Where(c => c.eventsSubGroupTbls.Any(p => p.SubGroupTbl.groupID == groupID));
            }
            if (subGroupID != -1 && user.ID != -1)
            {
                if (db.SubGroupTbls.Any(c => c.ID == subGroupID) == false)
                {
                    Result.result.code = 8;
                    Result.result.message = ClassCollection.Message.SUBGROUP_NOT_EXIST;
                    Result.events = null;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                events = events.Where(x => x.eventsSubGroupTbls.Any(c => c.subGroupID == subGroupID));
            }

            #endregion

            events = events.Where(c => c.eventsContentTbls.Any()).OrderByDescending(c => c.fromDate);
            events = events.Skip(skipCount).Take(count);
            Result.events = new List<ClassCollection.Model.LatestEventsForList>();
            foreach (var item in events)
            {
                var tmp = new ClassCollection.Model.LatestEventsForList();
                tmp.ID = item.ID;
                try
                {
                    tmp.image = item.eventsContentTbls.Where(c => c.contentType == 1).Take(1).Single().value;
                }
                catch
                {
                    tmp.image = "";
                }
                tmp.like = item.eventsLikeTbls.Where(c => c.isLike).Count();
                tmp.unlike = item.eventsLikeTbls.Where(c => !c.isLike).Count();
                tmp.viewCount = item.viewCount;
                tmp.toDate = Persia.Calendar.ConvertToPersian(item.toDate).ToString("n");
                tmp.fromDate = Persia.Calendar.ConvertToPersian(item.fromDate).ToString("n");
                tmp.title = item.title;
                try
                {
                    tmp.summary = item.eventsContentTbls.Where(c => c.contentType == 0).Take(1).Single().value;
                }
                catch
                {
                    tmp.summary = item.title;
                }

                Result.events.Add(tmp);
            }
            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result));
            return;

        }

        /// <summary>
        /// این تابع وظیفه ارسال رویداد برای کاربر مورد نظر را بر عهده دارد.
        /// </summary>
        /// <param name="key">کلمه عبور برنامه ای که درحال استفاده از متد جاری می باشد</param>
        /// <param name="eventsID">کد رویداد مورد نظر جهت نمایش جزییات آن
        ///</param>
        /// <returns>خروجی ساختار جی سون از کلاس زیر را دارد.
        /// <para><see cref="ClassCollection.Model.EventsResult" /></para>
        /// <remarks>
        /// <para>Code : -1 , Message : <see cref="ClassCollection.Message.OPERATION_NO_ACCESS"/>, Result.events : <c>null</c></para>
        /// <para>Code : 0 , Message : <see cref="ClassCollection.Message.OPERATION_SUCCESS" />, Result.events : List of <see cref=" ClassCollection.Model.EventsResult"/> </para>
        /// <para>Code : 1 , Message : <see cref="ClassCollection.Message.EVENTS_NOT_EXIST" />, Result.events : <c>null</c></para>
        /// </remarks>
        /// </returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getEvents(string key, long eventsID)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.EventsResult Result = new ClassCollection.Model.EventsResult();
            Result.result = new ClassCollection.Model.Result();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }
            
            var db = new DataAccessDataContext();

            if (db.eventsTbls.Any(c => c.ID == eventsID && c.isBlock == false) == false)
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.EVENTS_NOT_EXIST;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            var events = db.eventsTbls.Single(c => c.ID == eventsID);

            Result.events = new ClassCollection.Model.fullEvents();
            Result.events.userID = events.userID;
            Result.events.ID = events.ID;
            Result.events.like = events.eventsLikeTbls.Count(c => c.isLike == true);
            Result.events.unlike = events.eventsLikeTbls.Count(c => c.isLike == false);
            Result.events.toDate = Persia.Calendar.ConvertToPersian(events.toDate).ToString("n");
            Result.events.fromDate = Persia.Calendar.ConvertToPersian(events.fromDate).ToString("n");
            Result.events.title = events.title;
            Result.events.viewCount = events.viewCount + 1;
            Result.events.tag = new List<Model.Tag>();

            foreach (var t in events.eventTagTbls)
            {
                Result.events.tag.Add(new Model.Tag() { title = t.tagTbl.title, ID = t.tagTbl.ID });
            }

            events.viewCount += 1;
            db.SubmitChanges();

            Result.events.contents = new List<ClassCollection.Model.eventsContent>();
            var contents = events.eventsContentTbls.OrderBy(c => c.pri);

            foreach (var item in contents)
            {
                var tmp = new ClassCollection.Model.eventsContent();
                tmp.ID = item.ID;
                tmp.type = item.contentType;
                tmp.value = item.value;
                Result.events.contents.Add(tmp);
            }

            Result.events.groups = new List<ClassCollection.Model.eventsGroupSubGroup>();
            var subgroups = events.eventsSubGroupTbls;
            foreach (var item in subgroups)
            {
                var tmp = new ClassCollection.Model.eventsGroupSubGroup();
                tmp.groupID = item.SubGroupTbl.groupID;
                tmp.groupTitle = item.SubGroupTbl.GroupTbl.title;
                tmp.ID = item.ID;
                tmp.eventsID = item.eventsID;
                tmp.subGroupID = item.subGroupID;
                tmp.subGroupTitle = item.SubGroupTbl.title;
                Result.events.groups.Add(tmp);

            }

            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result));
            return;

        }

        /// <summary>
        /// این تابع وظیفه ارسال رویداد برای کاربر مورد نظر را بر عهده دارد.
        /// </summary>
        /// <param name="key">کلمه عبور برنامه ای که درحال استفاده از متد جاری می باشد</param>
        /// <param name="eventsID">کد رویداد مورد نظر جهت نمایش جزییات آن
        ///</param>
        /// <returns>خروجی ساختار جی سون از کلاس زیر را دارد.
        /// <para><see cref="ClassCollection.Model.SingleEventsResult" /></para>
        /// <remarks>
        /// <para>Code : -1 , Message : <see cref="ClassCollection.Message.OPERATION_NO_ACCESS"/>, Result.events : <c>null</c></para>
        /// <para>Code : 0 , Message : <see cref="ClassCollection.Message.OPERATION_SUCCESS" />, Result.events : <see cref=" ClassCollection.Model.singleFullEvents"/> </para>
        /// <para>Code : 1 , Message : <see cref="ClassCollection.Message.EVENTS_NOT_EXIST" />, Result.events : <c>null</c></para>
        /// </remarks>
        /// </returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getSingleEvents(string key, long eventsID)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.SingleEventsResult Result = new ClassCollection.Model.SingleEventsResult();
            Result.result = new ClassCollection.Model.Result();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            
            var db = new DataAccessDataContext();

            if (db.eventsTbls.Any(c => c.ID == eventsID && c.isBlock == false) == false)
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.EVENTS_NOT_EXIST;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            var events = db.eventsTbls.Single(c => c.ID == eventsID);

            Result.singleevents = new ClassCollection.Model.singleFullEvents();
            if (events.mUserID != null)
                Result.singleevents.mUserID = events.mUserID;
            else
                Result.singleevents.mUserID = -1;
            Result.singleevents.ID = events.ID;
            Result.singleevents.like = events.eventsLikeTbls.Count(c => c.isLike == true);
            Result.singleevents.unlike = events.eventsLikeTbls.Count(c => c.isLike == false);
            Result.singleevents.toDate = Persia.Calendar.ConvertToPersian(events.toDate).ToString("n");
            Result.singleevents.fromDate = Persia.Calendar.ConvertToPersian(events.fromDate).ToString("n");
            Result.singleevents.title = events.title;
            Result.singleevents.viewCount = events.viewCount + 1;

            events.viewCount += 1;
            db.SubmitChanges();


            var contents = events.eventsContentTbls.OrderBy(c => c.pri);

            try
            {
                Result.singleevents.image = contents.First(c => c.contentType == 1).value;
            }
            catch
            {
                Result.singleevents.image = "";
            }

            try
            {
                Result.singleevents.video = contents.First(c => c.contentType == 2).value;
            }
            catch
            {
                Result.singleevents.video = "";
            }
            try
            {
                Result.singleevents.text = contents.First(c => c.contentType == 0).value;
            }
            catch
            {
                Result.singleevents.text = "";
            }

            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result));
            return;

        }

        /// <summary>
        /// این تابع وظیفه ثبت لایک و آنلایک برای رویداد توسط کاربر مورد نظر را بر عهده دارد.
        /// </summary>
        /// <param name="key">کلمه عبور برنامه ای که درحال استفاده از متد جاری می باشد</param>
        /// <param name="eventsID">کد v,dnhn مورد نظر جهت ثبت نظر برای آن
        ///</param>
        /// <param name="userID">کد کاربری که در حال ثبت نظر برای رویداد می باشد
        /// <remarks>
        /// در صورتی که کاربر مهمان می باشد مقدار -1 را قرار دهید
        /// </remarks>
        ///</param>
        /// <param name="isLike">نوع نظر درحال ثبت کاربر برای رویداد
        /// <remarks>
        /// <para>True : لایک برای رویداد درج میشود</para>
        /// <para>False : آنلایک برای رویداد درج میشود</para>
        /// </remarks>
        ///</param>
        /// <returns>خروجی ساختار جی سون از کلاس زیر را دارد.
        /// <para><see cref="ClassCollection.Model.Result" /></para>
        /// <remarks>
        /// <para>Code : -1 , Message : <see cref="ClassCollection.Message.OPERATION_NO_ACCESS"/></para>
        /// <para>Code : 0 , Message : <see cref="ClassCollection.Message.OPERATION_SUCCESS" /></para>
        /// <para>Code : 1 , Message : <see cref="ClassCollection.Message.EVENTS_NOT_EXIST" /></para>
        /// <para>Code : 2 , Message : <see cref="ClassCollection.Message.USER_NOT_EXIST" /></para>
        /// </remarks>
        /// </returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void likeUnlikeEvents(string key, long userID, long eventsID, bool isLike)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.Result Result = new ClassCollection.Model.Result();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.code = -1;
                Result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            DateTime dt = new DateTime();
            dt = DateTime.Now;
            var db = new DataAccessDataContext();

            if (db.eventsTbls.Any(c => c.ID == eventsID && c.isBlock == false) == false)
            {
                Result.code = 1;
                Result.message = ClassCollection.Message.EVENTS_NOT_EXIST;

                Context.Response.Write(js.Serialize(Result));
                return;
            }
            if (userID != -1)
            {
                if (db.mUserTbls.Any(c => c.ID == userID && c.isBlock == false) == false)
                {
                    Result.code = 2;
                    Result.message = ClassCollection.Message.USER_NOT_EXIST;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }

            }

            if (db.eventsLikeTbls.Any(c => c.userID == userID && c.eventsID == eventsID) == false)
            {
                var eventslike = new eventsLikeTbl();
                eventslike.isLike = isLike;
                eventslike.eventsID = eventsID;
                eventslike.userID = userID == -1 ? 3 : userID;
                eventslike.regDate = dt;
                db.eventsLikeTbls.InsertOnSubmit(eventslike);
                db.SubmitChanges();
            }

            Result.code = 0;
            Result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result));
            return;

        }

        /// <summary>
        /// این تابع وظیفه ارسال رویدادها دریاقت نشده برای کاربر مورد نظر را بر عهده دارد.
        /// </summary>
        /// <param name="key">کلمه عبور برنامه ای که درحال استفاده از متد جاری می باشد</param>
        /// <param name="mobileOrEmail">شماره موبایل یا ایمیل کاربر
        /// <remarks>
        ///  <para>در صورت قرار دادن شماره موبایل در این پارامتر طول آن باید برابر با 12 کاراکتر باشد</para>
        ///  <para>در صورت قرار دادن ایمیل در این پارامتر طول آن باید کمتر از 250 کاراکتر باید</para> 
        ///  <para>در صورتی که فقط نیاز به دریافت رویدادهای عمومی می باشد مقدار این پارامتر را مقدار رشته خالی قرار دهید</para>
        /// </remarks>
        /// </param>
        /// <param name="lastEventsID" >کد آخرین رویداد جهت ارسال رویدادهای درج شده ی بعد از آن
        /// </param>
        /// <returns>خروجی ساختار جی سون از کلاس زیر را دارد.
        /// <para><see cref="ClassCollection.Model.LatestEventsForNotificationResult" /></para>
        /// <remarks>
        /// <para>Code : -1, Message : <see cref="ClassCollection.Message.OPERATION_NO_ACCESS"/>, Result.events : <c>null</c></para>
        /// <para>Code : 0 , Message : <see cref="ClassCollection.Message.OPERATION_SUCCESS" />, Result.events : List of <see cref=" ClassCollection.Model.LatestEventsForNotification"/></para>
        /// <para>Code : 1 , Message : <see cref="ClassCollection.Message.ERROR_INPUT_PARAMETER" />, Result.events : <c>null</c></para>
        /// <para>Code : 2 , Message : <see cref="ClassCollection.Message.USER_NOT_EXIST" />, Result.events : <c>null</c></para>
        /// <para>Code : 3 , Message : <see cref="ClassCollection.Message.USER_BLOCKED" />, Result.events : <c>null</c></para>
        /// </remarks>
        /// </returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getLatestEventsForNotification(string key, string mobileOrEmail, long lastEventsID)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.LatestEventsForNotificationResult Result = new ClassCollection.Model.LatestEventsForNotificationResult();
            Result.result = new ClassCollection.Model.Result();

            DateTime dt = new DateTime();
            dt = DateTime.Now.Date;

            var db = new DataAccessDataContext();
            var user = new mUserTbl();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            mobileOrEmail = mobileOrEmail.Trim();

            if (mobileOrEmail == "")
            {
                mobileOrEmail = "-1";
                user.ID = -1;
            }
            else
            {

                if (mobileOrEmail.Contains("@") && mobileOrEmail.Length >= 250)
                {
                    Result.result.code = 1;
                    Result.result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                if (!mobileOrEmail.Contains("@") && mobileOrEmail.Length != 12)
                {
                    Result.result.code = 2;
                    Result.result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }

                if (!db.mUserTbls.Any(c => c.emailtell == mobileOrEmail))
                {
                    Result.result.code = 3;
                    Result.result.message = ClassCollection.Message.USER_NOT_EXIST;
                    Result.events = null;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                else
                {
                    user = db.mUserTbls.Single(c => c.emailtell == mobileOrEmail);
                    if (user.isBlock)
                    {
                        Result.result.code = 4;
                        Result.result.message = ClassCollection.Message.USER_BLOCKED;
                        Result.events = null;
                        Context.Response.Write(js.Serialize(Result));
                        return;
                    }
                }

            }

            IQueryable<eventsTbl> events = db.eventsTbls;

            events = events.Where(p => (((p.eventsSubGroupTbls.Any(c => c.SubGroupTbl.userSubGroupTbls.Any(z => z.userID == user.ID) && user.ID != -1)) || (p.eventsSubGroupTbls.Count() == 0 && user.ID == -1)) && p.ID > lastEventsID)
                    &&
                    (p.toDate.Date >= dt && p.fromDate <= dt)
                    && p.isBlock == false
                    );

            events = events.OrderByDescending(c => c.fromDate);

            Result.events = new List<ClassCollection.Model.LatestEventsForNotification>();
            foreach (var item in events)
            {
                var tmp = new ClassCollection.Model.LatestEventsForNotification();
                tmp.ID = item.ID;
                tmp.title = item.title;

                Result.events.Add(tmp);
            }
            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result));
            return;
        }

        /// <summary>
        /// این تابع وظیفه ارسال رویداد در ماه مورد نظر برای کاربر مورد نظر را بر عهده دارد.
        /// </summary>
        /// <param name="key">کلمه عبور برنامه ای که درحال استفاده از متد جاری می باشد</param>
        /// <param name="mobileOrEmail">شماره موبایل یا ایمیل کاربر
        /// <remarks>
        ///  <para>در صورت قرار دادن شماره موبایل در این پارامتر طول آن باید برابر با 12 کاراکتر باشد</para>
        ///  <para>در صورت قرار دادن ایمیل در این پارامتر طول آن باید کمتر از 250 کاراکتر باید</para> 
        ///  <para>در صورتی که فقط نیاز به دریافت رویدادهای عمومی می باشد مقدار این پارامتر را مقدار رشته خالی قرار دهید</para>
        /// </remarks>
        /// </param>
        /// <param name="date">
        /// تاریخ مورد نظر جهت دریافت رویدادهای موجود در آن
        /// <remarks>
        /// فرمت ارسال تاریخ باید به شکل روبرو باشد : YYYY-MM
        /// </remarks>
        /// </param>
        /// <param name="fillterString" >رشته ی مورد نظر کاربر جهت جستجو در بین رویدادها
        /// <remarks>
        ///- اجزای جستجو با فاصله از یکدیگر جدا میشوند
        ///- درصورتی که کلمه مورد نظر در بین دو علامت دابل کوتیشن قرار بگیرد رویداد یافت شده حتما شامل آن کلمه خواهد بود
        /// </remarks>
        /// </param>
        /// <param name="groupID">کد گروهی که جهت استفاده در فیلتر کردن مد نظر میباشد
        /// <remarks>در صورت عدم نیاز به لحاظ این پارامتر در جستجو مقدار -1 را قرار دهید</remarks>
        /// </param>
        /// <param name="subGroupID">کد زیرگروهی که جهت استفاده در فیلتر کردن مد نظر میباشد
        /// <remarks>در صورت عدم نیاز به لحاظ این پارامتر در جستجو مقدار -1 را قرار دهید</remarks>
        /// </param>
        /// <param name="count">تعداد نتایج درخواستی در هر صفحه</param>
        /// <param name="pageIndex">شماره صفحه مورد نظر از جستجو</param>
        /// 
        /// <returns>خروجی ساختار جی سون از کلاس زیر را دارد.
        /// <para><see cref="ClassCollection.Model.LatestEventsForListResult" /></para>
        /// <remarks>
        /// <para>Code : -1 , Message : <see cref="ClassCollection.Message.OPERATION_NO_ACCESS"/>, Result.events : <c>null</c></para>
        /// <para>Code : 0 , Message : <see cref="ClassCollection.Message.OPERATION_SUCCESS" />, Result.events : List of <see cref=" ClassCollection.Model.LatestEventsForList"/></para>
        /// <para>Code : 1 , Message : <see cref="ClassCollection.Message.DATE_INCORRECT" />, Result.events : <c>null</c></para>
        /// <para>Code : 2 , Message : <see cref="ClassCollection.Message.EMAIL_INCORRECT" />, Result.events : <c>null</c></para>
        /// <para>Code : 3 , Message : <see cref="ClassCollection.Message.MOBILE_INCORRECT" />, Result.events : <c>null</c></para>
        /// <para>Code : 4 , Message : <see cref="ClassCollection.Message.USER_NOT_EXIST" />, Result.events : <c>null</c></para> 
        /// <para>Code : 5 , Message : <see cref="ClassCollection.Message.USER_BLOCKED" />, Result.events : <c>null</c></para> 
        /// <para>Code : 6 , Message : <see cref="ClassCollection.Message.GROUP_NOT_EXIST" />, Result.events : <c>null</c></para>
        /// <para>Code : 7 , Message : <see cref="ClassCollection.Message.SUBGROUP_NOT_EXIST" />, Result.events : <c>null</c></para>
        /// </remarks>
        /// </returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getEventsInMonthForList(string key, string mobileOrEmail, string date, string fillterString, long groupID, long subGroupID, int count, int pageIndex)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.LatestEventsForListResult Result = new ClassCollection.Model.LatestEventsForListResult();
            Result.result = new ClassCollection.Model.Result();

            var skipCount = count * (pageIndex - 1);
            var Date = new DateTime();
            var toDate = new DateTime();

            var db = new DataAccessDataContext();
            var user = new mUserTbl();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            mobileOrEmail = mobileOrEmail.Trim();
            date = date.Trim();

            int pyear = 0, pmonth = 0;
            var DateArray = date.Split('-');

            if (DateArray.Count() != 2
               || !int.TryParse(DateArray[0], out pyear)
               || !int.TryParse(DateArray[1], out pmonth))
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.DATE_INCORRECT;
                Context.Response.Write(js.Serialize(Result));
                return;
            }
            try
            {
                Date = Persia.Calendar.ConvertToGregorian(pyear, pmonth, 1, Persia.DateType.Persian);
                toDate = Date.AddMonths(1).AddDays(-1);
            }
            catch
            {
                Result.result.code = 2;
                Result.result.message = ClassCollection.Message.DATE_INCORRECT;
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            if (mobileOrEmail == "")
            {
                mobileOrEmail = "-1";
                user.ID = -1;
            }
            else
            {

                if (mobileOrEmail.Contains("@") && mobileOrEmail.Length >= 250)
                {
                    Result.result.code = 3;
                    Result.result.message = ClassCollection.Message.EMAIL_INCORRECT;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                if (!mobileOrEmail.Contains("@") && mobileOrEmail.Length != 12)
                {
                    Result.result.code = 4;
                    Result.result.message = ClassCollection.Message.MOBILE_INCORRECT;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }

                if (!db.mUserTbls.Any(c => c.emailtell == mobileOrEmail))
                {
                    Result.result.code = 5;
                    Result.result.message = ClassCollection.Message.USER_NOT_EXIST;
                    Result.events = null;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                else
                {
                    user = db.mUserTbls.Single(c => c.emailtell == mobileOrEmail);
                    if (user.isBlock)
                    {
                        Result.result.code = 6;
                        Result.result.message = ClassCollection.Message.USER_BLOCKED;
                        Result.events = null;
                        Context.Response.Write(js.Serialize(Result));
                        return;
                    }
                }

            }

            IQueryable<eventsTbl> events = db.eventsTbls;

            #region search

            if (fillterString != "")
            {
                var search = new Search(fillterString);
                var ands = search.getAND;
                var ors = search.getOR;

                var predicate = PredicateBuilder.False<eventsTbl>();
                foreach (string keyword in ors)
                {
                    string temp = keyword;
                    predicate = predicate.Or(p => ((p.eventsSubGroupTbls.Any(c => c.SubGroupTbl.userSubGroupTbls.Any(z => z.userID == user.ID) && user.ID != -1)) || (p.eventsSubGroupTbls.Count() == 0 && user.ID == -1))
                        &&
                        (p.title.Contains(temp) || p.eventsContentTbls.Any(v => v.contentType == 0 && v.value.Contains(temp)) || p.eventTagTbls.Any(x => x.tagTbl.title.Contains(keyword)))
                        &&
                       ((p.fromDate.Date <= Date.Date && p.toDate.Date >= Date.Date) || (p.fromDate.Date >= Date.Date && p.toDate.Date <= toDate.Date))
                        && p.isBlock == false
                        );
                }

                foreach (var keyword in ands)
                {
                    events = events.Where(p => ((p.eventsSubGroupTbls.Any(c => c.SubGroupTbl.userSubGroupTbls.Any(z => z.userID == user.ID) && user.ID != -1)) || (p.eventsSubGroupTbls.Count() == 0 && user.ID == -1))
                        &&
                        ((p.toDate.Year >= Date.Year && p.toDate.Month >= Date.Month) && (p.fromDate.Year <= Date.Year && p.fromDate.Month <= Date.Month))
                        &&
                      ((p.fromDate.Date <= Date.Date && p.toDate.Date >= Date.Date) || (p.fromDate.Date >= Date.Date && p.toDate.Date <= toDate.Date))
                        && p.isBlock == false
                        );
                }
                events = events.Where(predicate);
            }
            else
            {
                events = events.Where(p => ((p.eventsSubGroupTbls.Any(c => c.SubGroupTbl.userSubGroupTbls.Any(z => z.userID == user.ID) && user.ID != -1)) || (p.eventsSubGroupTbls.Count() == 0 && user.ID == -1))
                        &&
                       ((p.fromDate.Date <= Date.Date && p.toDate.Date >= Date.Date) || (p.fromDate.Date >= Date.Date && p.toDate.Date <= toDate.Date))
                        && p.isBlock == false
                        );
            }
            if (groupID != -1 && user.ID != -1)
            {
                if (db.GroupTbls.Any(c => c.ID == groupID) == false)
                {
                    Result.result.code = 7;
                    Result.result.message = ClassCollection.Message.GROUP_NOT_EXIST;
                    Result.events = null;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                events = events.Where(c => c.eventsSubGroupTbls.Any(p => p.SubGroupTbl.groupID == groupID));
            }
            if (subGroupID != -1 && user.ID != -1)
            {
                if (db.SubGroupTbls.Any(c => c.ID == subGroupID) == false)
                {
                    Result.result.code = 8;
                    Result.result.message = ClassCollection.Message.SUBGROUP_NOT_EXIST;
                    Result.events = null;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                events = events.Where(x => x.eventsSubGroupTbls.Any(c => c.subGroupID == subGroupID));
            }

            #endregion

            events = events.Where(c => c.eventsContentTbls.Any()).OrderBy(c => c.fromDate);
            events = events.Skip(skipCount).Take(count);
            Result.events = new List<ClassCollection.Model.LatestEventsForList>();
            foreach (var item in events)
            {
                var tmp = new ClassCollection.Model.LatestEventsForList();
                tmp.ID = item.ID;
                try
                {
                    tmp.image = item.eventsContentTbls.Where(c => c.contentType == 1).Take(1).Single().value;
                }
                catch
                {
                    tmp.image = "";
                }
                tmp.like = item.eventsLikeTbls.Where(c => c.isLike).Count();
                tmp.unlike = item.eventsLikeTbls.Where(c => !c.isLike).Count();
                tmp.viewCount = item.viewCount;
                tmp.toDate = Persia.Calendar.ConvertToPersian(item.toDate).ToString("n");
                tmp.fromDate = Persia.Calendar.ConvertToPersian(item.fromDate).ToString("n");
                tmp.title = item.title;
                try
                {
                    tmp.summary = item.eventsContentTbls.Where(c => c.contentType == 0).Take(1).Single().value;
                }
                catch
                {
                    tmp.summary = item.title;
                }

                Result.events.Add(tmp);
            }
            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result));
            return;

        }

        /// <summary>
        /// این تابع وظیفه ارسال روزهایی که در ماه مورد نظر دارای رویداد می باشد را بر عهده دارد.
        /// </summary>
        /// <param name="key">کلمه عبور برنامه ای که درحال استفاده از متد جاری می باشد</param>
        /// <param name="mobileOrEmail">شماره موبایل یا ایمیل کاربر
        /// <remarks>
        ///  <para>در صورت قرار دادن شماره موبایل در این پارامتر طول آن باید برابر با 12 کاراکتر باشد</para>
        ///  <para>در صورت قرار دادن ایمیل در این پارامتر طول آن باید کمتر از 250 کاراکتر باید</para> 
        ///  <para>در صورتی که فقط نیاز به دریافت رویدادهای عمومی می باشد مقدار این پارامتر را مقدار رشته خالی قرار دهید</para>
        /// </remarks>
        /// </param>
        /// <param name="date">
        /// تاریخ مورد نظر جهت دریافت رویدادهای موجود در آن
        /// <remarks>
        /// فرمت ارسال تاریخ باید به شکل روبرو باشد : YYYY-MM
        /// </remarks>
        /// </param>
        /// <param name="fillterString" >رشته ی مورد نظر کاربر جهت جستجو در بین رویدادها
        /// <remarks>
        ///- اجزای جستجو با فاصله از یکدیگر جدا میشوند
        ///- درصورتی که کلمه مورد نظر در بین دو علامت دابل کوتیشن قرار بگیرد رویداد یافت شده حتما شامل آن کلمه خواهد بود
        /// </remarks>
        /// </param>
        /// <param name="groupID">کد گروهی که جهت استفاده در فیلتر کردن مد نظر میباشد
        /// <remarks>در صورت عدم نیاز به لحاظ این پارامتر در جستجو مقدار -1 را قرار دهید</remarks>
        /// </param>
        /// <param name="subGroupID">کد زیرگروهی که جهت استفاده در فیلتر کردن مد نظر میباشد
        /// <remarks>در صورت عدم نیاز به لحاظ این پارامتر در جستجو مقدار -1 را قرار دهید</remarks>
        /// </param>
        /// <returns>خروجی ساختار جی سون از کلاس زیر را دارد.
        /// <para><see cref="ClassCollection.Model.DaysWithEventForListResult" /></para>
        /// <remarks>
        /// <para>Code : -1 , Message : <see cref="ClassCollection.Message.OPERATION_NO_ACCESS"/>, Result.date : <c>null</c></para>
        /// <para>Code : 0 , Message : <see cref="ClassCollection.Message.OPERATION_SUCCESS" />, Result.date : List of <see cref=" ClassCollection.Model.DaysWithEventForList"/></para>
        /// <para>Code : 1 , Message : <see cref="ClassCollection.Message.DATE_INCORRECT" />, Result.date : <c>null</c></para>
        /// <para>Code : 2 , Message : <see cref="ClassCollection.Message.EMAIL_INCORRECT" />, Result.date : <c>null</c></para>
        /// <para>Code : 3 , Message : <see cref="ClassCollection.Message.MOBILE_INCORRECT" />, Result.date : <c>null</c></para>
        /// <para>Code : 4 , Message : <see cref="ClassCollection.Message.USER_NOT_EXIST" />, Result.date : <c>null</c></para> 
        /// <para>Code : 5 , Message : <see cref="ClassCollection.Message.USER_BLOCKED" />, Result.date : <c>null</c></para> 
        /// <para>Code : 6 , Message : <see cref="ClassCollection.Message.GROUP_NOT_EXIST" />, Result.date : <c>null</c></para>
        /// <para>Code : 7 , Message : <see cref="ClassCollection.Message.SUBGROUP_NOT_EXIST" />, Result.date : <c>null</c></para>
        /// </remarks>
        /// </returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getDaysWithEvent(string key, string mobileOrEmail, string date, string fillterString, long groupID, long subGroupID)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.DaysWithEventForListResult Result = new ClassCollection.Model.DaysWithEventForListResult();
            Result.result = new ClassCollection.Model.Result();
            
            var Date = new DateTime();
            var toDate = new DateTime();

            var db = new DataAccessDataContext();
            var user = new mUserTbl();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            mobileOrEmail = mobileOrEmail.Trim();
            date = date.Trim();

            int pyear = 0, pmonth = 0;
            var DateArray = date.Split('-');

            if (DateArray.Count() != 2
               || !int.TryParse(DateArray[0], out pyear)
               || !int.TryParse(DateArray[1], out pmonth))
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.DATE_INCORRECT;
                Context.Response.Write(js.Serialize(Result));
                return;
            }
            try
            {
                Date = Persia.Calendar.ConvertToGregorian(pyear, pmonth, 1, Persia.DateType.Persian);
                toDate = Date.AddMonths(1).AddDays(-1);
            }
            catch
            {
                Result.result.code = 2;
                Result.result.message = ClassCollection.Message.DATE_INCORRECT;
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            if (mobileOrEmail == "")
            {
                mobileOrEmail = "-1";
                user.ID = -1;
            }
            else
            {

                if (mobileOrEmail.Contains("@") && mobileOrEmail.Length >= 250)
                {
                    Result.result.code = 3;
                    Result.result.message = ClassCollection.Message.EMAIL_INCORRECT;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                if (!mobileOrEmail.Contains("@") && mobileOrEmail.Length != 12)
                {
                    Result.result.code = 4;
                    Result.result.message = ClassCollection.Message.MOBILE_INCORRECT;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }

                if (!db.mUserTbls.Any(c => c.emailtell == mobileOrEmail))
                {
                    Result.result.code = 5;
                    Result.result.message = ClassCollection.Message.USER_NOT_EXIST;
                    Result.date = null;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                else
                {
                    user = db.mUserTbls.Single(c => c.emailtell == mobileOrEmail);
                    if (user.isBlock)
                    {
                        Result.result.code = 6;
                        Result.result.message = ClassCollection.Message.USER_BLOCKED;
                        Result.date = null;
                        Context.Response.Write(js.Serialize(Result));
                        return;
                    }
                }

            }

            IQueryable<eventsTbl> events = db.eventsTbls;

            #region search

            if (fillterString != "")
            {
                var search = new Search(fillterString);
                var ands = search.getAND;
                var ors = search.getOR;

                var predicate = PredicateBuilder.False<eventsTbl>();
                foreach (string keyword in ors)
                {
                    string temp = keyword;
                    predicate = predicate.Or(p => ((p.eventsSubGroupTbls.Any(c => c.SubGroupTbl.userSubGroupTbls.Any(z => z.userID == user.ID) && user.ID != -1)) || (p.eventsSubGroupTbls.Count() == 0 && user.ID == -1))
                        &&
                        (p.title.Contains(temp) || p.eventsContentTbls.Any(v => v.contentType == 0 && v.value.Contains(temp)) || p.eventTagTbls.Any(x => x.tagTbl.title.Contains(keyword)))
                        &&
                        ((p.fromDate.Date <= Date.Date && p.toDate.Date >= Date.Date) || (p.fromDate.Date >= Date.Date && p.toDate.Date <= toDate.Date))
                        && p.isBlock == false
                        );
                }

                foreach (var keyword in ands)
                {
                    events = events.Where(p => ((p.eventsSubGroupTbls.Any(c => c.SubGroupTbl.userSubGroupTbls.Any(z => z.userID == user.ID) && user.ID != -1)) || (p.eventsSubGroupTbls.Count() == 0 && user.ID == -1))
                        &&
                        (p.title.Contains(keyword) || p.eventsContentTbls.Any(v => v.contentType == 0 && v.value.Contains(keyword)))
                        &&
                       ((p.fromDate.Date <= Date.Date && p.toDate.Date >= Date.Date) || (p.fromDate.Date >= Date.Date && p.toDate.Date <= toDate.Date))
                        && p.isBlock == false
                        );
                }
                events = events.Where(predicate);
            }
            else
            {
                events = events.Where(p => ((p.eventsSubGroupTbls.Any(c => c.SubGroupTbl.userSubGroupTbls.Any(z => z.userID == user.ID) && user.ID != -1)) || (p.eventsSubGroupTbls.Count() == 0 && user.ID == -1))
                        &&
                        ((p.fromDate.Date <= Date.Date && p.toDate.Date >= Date.Date) || (p.fromDate.Date >= Date.Date && p.toDate.Date <= toDate.Date))
                        && p.isBlock == false
                        );
            }
            if (groupID != -1 && user.ID != -1)
            {
                if (db.GroupTbls.Any(c => c.ID == groupID) == false)
                {
                    Result.result.code = 7;
                    Result.result.message = ClassCollection.Message.GROUP_NOT_EXIST;
                    Result.date = null;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                events = events.Where(c => c.eventsSubGroupTbls.Any(p => p.SubGroupTbl.groupID == groupID));
            }
            if (subGroupID != -1 && user.ID != -1)
            {
                if (db.SubGroupTbls.Any(c => c.ID == subGroupID) == false)
                {
                    Result.result.code = 8;
                    Result.result.message = ClassCollection.Message.SUBGROUP_NOT_EXIST;
                    Result.date = null;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                events = events.Where(x => x.eventsSubGroupTbls.Any(c => c.subGroupID == subGroupID));
            }

            #endregion

            Result.date = new List<ClassCollection.Model.DaysWithEventForList>();
            while (true)
            {
                if (Date.Date.CompareTo(toDate.Date) == 0)
                {
                    break;
                }
                else
                {
                    foreach (var item in events)
                    {
                        if (item.toDate.Date >= Date.Date && item.fromDate.Date <= Date)
                        {
                            var td = Persia.Calendar.ConvertToPersian(Date);
                            Result.date.Add(new ClassCollection.Model.DaysWithEventForList() { day = td.ArrayType[2], month = td.ArrayType[1], year = td.ArrayType[0] });
                            break;
                        }

                    }
                    Date = Date.AddDays(1);
                }
            }


            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result));
            return;

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void sendEventComment(string key, long eventID, long userID, string mobileOrEmail, string fullName, string text)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.Result Result = new ClassCollection.Model.Result();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.code = -1;
                Result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            fullName = fullName.TrimEnd().TrimStart();
            text = text.TrimStart().TrimEnd();

            if (fullName.Length == 0)
            {
                Result.code = 1;
                Result.message = ClassCollection.Message.NAME_INCORRECT;

                Context.Response.Write(js.Serialize(Result));
                return;
            }
            if (text.Length == 0 || text.Length > 250)
            {
                Result.code = 2;
                Result.message = ClassCollection.Message.COMMENT_TEXT_INCORRECT;

                Context.Response.Write(js.Serialize(Result));
                return;
            }
            //if (Methods.isEmail(mobileOrEmail) == false)
            //{
            //    if (mobileOrEmail.Length != 11)
            //    {
            //        Result.code = 3;
            //        Result.message = ClassCollection.Message.MOBILE_INCORRECT;

            //        Context.Response.Write(js.Serialize(Result));
            //        return;
            //    }
            //}

            DateTime dt = new DateTime();
            dt = DateTime.Now;
            var db = new DataAccessDataContext();

            if (db.eventsTbls.Any(c => c.ID == eventID && c.isBlock == false) == false)
            {
                Result.code = 4;
                Result.message = ClassCollection.Message.NEWS_NOT_EXIST;

                Context.Response.Write(js.Serialize(Result));
                return;
            }
            if (db.mUserTbls.Any(c => c.ID == userID) == false && userID != -1)
            {
                Result.code = 5;
                Result.message = ClassCollection.Message.USER_NOT_EXIST;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            var eventComment = new eventCommentTbl();
            eventComment.fullName = fullName;
            eventComment.text = text;
            eventComment.regDate = dt;
            eventComment.eventID = eventID;
            eventComment.mUserID = userID;
            eventComment.mobile = Methods.isEmail(mobileOrEmail) ? null : mobileOrEmail;
            eventComment.email = Methods.isEmail(mobileOrEmail) ? mobileOrEmail : null;
            eventComment.isBlock = true;
            db.eventCommentTbls.InsertOnSubmit(eventComment);
            db.SubmitChanges();


            Result.code = 0;
            Result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result));
            return;

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getEventCommentList(string key, long eventID, int count, int pageIndex)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.CommentListResult Result = new ClassCollection.Model.CommentListResult();
            Result.result = new ClassCollection.Model.Result();


            var skipCount = count * (pageIndex - 1);
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }
            if (db.eventsTbls.Any(c => c.ID == eventID) == false)
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.EVENTS_NOT_EXIST;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            Result.comment = new List<Model.Comment>();
            var comments = db.eventCommentTbls.Where(c => c.isBlock == false && c.eventID == eventID).OrderByDescending(c => c.regDate);
            Result.comment = comments.Skip(skipCount).Take(count).Select(c => new Model.Comment() { fullName = c.fullName, ID = c.ID, text = c.text }).ToList<Model.Comment>();


            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result));
            return;

        }
        #endregion

        #region organization
        /// <summary>
        /// این تابع وظیفه ارسال سازمان ها برای کاربر مورد نظر را بر عهده دارد.
        /// </summary>
        /// <param name="key">کلمه عبور برنامه ای که درحال استفاده از متد جاری می باشد</param>
        /// <param name="mobileOrEmail">شماره موبایل یا ایمیل کاربر
        /// <remarks>
        ///  <para>در صورت قرار دادن شماره موبایل در این پارامتر طول آن باید برابر با 12 کاراکتر باشد</para>
        ///  <para>در صورت قرار دادن ایمیل در این پارامتر طول آن باید کمتر از 250 کاراکتر باید</para> 
        ///  <para>در صورتی که فقط نیاز به دریافت سازمان های عمومی می باشد مقدار این پارامتر را مقدار رشته خالی قرار دهید</para>
        /// </remarks>
        /// </param>
        /// <param name="fillterString" >رشته ی مورد نظر کاربر جهت جستجو در بین سازمان ها
        /// <remarks>
        ///- اجزای جستجو با فاصله از یکدیگر جدا میشوند
        ///- درصورتی که کلمه مورد نظر در بین دو علامت دابل کوتیشن قرار بگیرد سازمان های یافت شده حتما شامل آن کلمه خواهد بود
        /// </remarks>
        /// </param>
        /// <param name="groupID">کد گروهی که جهت استفاده در فیلتر کردن مد نظر میباشد
        /// <remarks>در صورت عدم نیاز به لحاظ این پارامتر در جستجو مقدار -1 را قرار دهید</remarks>
        /// </param>
        /// <param name="subGroupID">کد زیرگروهی که جهت استفاده در فیلتر کردن مد نظر میباشد
        /// <remarks>در صورت عدم نیاز به لحاظ این پارامتر در جستجو مقدار -1 را قرار دهید</remarks>
        /// </param>
        /// <param name="count">تعداد نتایج درخواستی در هر صفحه</param>
        /// <param name="pageIndex">شماره صفحه مورد نظر از جستجو</param>
        /// 
        /// <returns>خروجی ساختار جی سون از کلاس زیر را دارد.
        /// <para><see cref="ClassCollection.Model.LatestIoForListResult" /></para>
        /// <remarks>
        /// <para>Code : -1 , Message : <see cref="ClassCollection.Message.OPERATION_NO_ACCESS"/>, Result.io : <c>null</c></para>
        /// <para>Code : 0 , Message : <see cref="ClassCollection.Message.OPERATION_SUCCESS" />, Result.io : List of <see cref=" ClassCollection.Model.LatestIoForList"/></para>
        /// <para>Code : 1 , Message : <see cref="ClassCollection.Message.ERROR_INPUT_PARAMETER" />, Result.io : <c>null</c></para>
        /// <para>Code : 2 , Message : <see cref="ClassCollection.Message.USER_NOT_EXIST" />, Result.io : <c>null</c></para>
        /// <para>Code : 3 , Message : <see cref="ClassCollection.Message.USER_BLOCKED" />, Result.io : <c>null</c></para>
        /// <para>Code : 4 , Message : <see cref="ClassCollection.Message.GROUP_NOT_EXIST" />, Result.io : <c>null</c></para> 
        /// <para>Code : 5 , Message : <see cref="ClassCollection.Message.SUBGROUP_NOT_EXIST" />, Result.io : <c>null</c></para>
        /// </remarks>
        /// </returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getLatestIoForList(string key, string mobileOrEmail, string fillterString, long groupID, long subGroupID, int count, int pageIndex, long tagID)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.LatestIoForListResult Result = new ClassCollection.Model.LatestIoForListResult();
            Result.result = new ClassCollection.Model.Result();

            var skipCount = count * (pageIndex - 1);
            var now = new DateTime();
            now = DateTime.Now.Date;
            var db = new DataAccessDataContext();
            var user = new mUserTbl();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            mobileOrEmail = mobileOrEmail.Trim();

            if (mobileOrEmail == "")
            {
                mobileOrEmail = "-1";
                user.ID = -1;
            }
            else
            {

                if (mobileOrEmail.Contains("@") && mobileOrEmail.Length >= 250)
                {
                    Result.result.code = 1;
                    Result.result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                if (!mobileOrEmail.Contains("@") && mobileOrEmail.Length != 12)
                {
                    Result.result.code = 2;
                    Result.result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }

                if (!db.mUserTbls.Any(c => c.emailtell == mobileOrEmail))
                {
                    Result.result.code = 3;
                    Result.result.message = ClassCollection.Message.USER_NOT_EXIST;
                    Result.io = null;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                else
                {
                    user = db.mUserTbls.Single(c => c.emailtell == mobileOrEmail);
                    if (user.isBlock)
                    {
                        Result.result.code = 4;
                        Result.result.message = ClassCollection.Message.USER_BLOCKED;
                        Result.io = null;
                        Context.Response.Write(js.Serialize(Result));
                        return;
                    }
                }

            }

            IQueryable<ioTbl> io = db.ioTbls;
            if (tagID != -1 && db.tagTbls.Any(c => c.ID == tagID))
                io = io.Where(c => c.ioTagTbls.Any(p => p.tagID == tagID));
            #region search
            if (fillterString != "")
            {
                var search = new Search(fillterString);
                var ands = search.getAND;
                var ors = search.getOR;


                var predicate = PredicateBuilder.False<ioTbl>();
                foreach (string keyword in ors)
                {
                    string temp = keyword;
                    predicate = predicate.Or(p => ((p.ioSubGroupTbls.Any(c => c.SubGroupTbl.userSubGroupTbls.Any(z => z.userID == user.ID) && user.ID != -1)) || (p.ioSubGroupTbls.Count() == 0 && user.ID == -1))
                        &&
                        (p.title.Contains(temp) || p.ioContentTbls.Any(v => v.contentType == 0 && v.value.Contains(temp)) || p.ioTagTbls.Any(x => x.tagTbl.title.Contains(keyword)))
                        &&
                        (p.publishDate.Date <= now)
                        && p.isBlock == false
                        );
                }


                foreach (var keyword in ands)
                {
                    io = io.Where(p => ((p.ioSubGroupTbls.Any(c => c.SubGroupTbl.userSubGroupTbls.Any(z => z.userID == user.ID) && user.ID != -1)) || (p.ioSubGroupTbls.Count() == 0 && user.ID == -1))
                        &&
                        (p.title.Contains(keyword) || p.ioContentTbls.Any(v => v.contentType == 0 && v.value.Contains(keyword)))
                        &&
                        (p.publishDate.Date <= now)
                        && p.isBlock == false
                        );
                }
                io = io.Where(predicate);
            }
            else
            {
                io = io.Where(p => ((p.ioSubGroupTbls.Any(c => c.SubGroupTbl.userSubGroupTbls.Any(z => z.userID == user.ID) && user.ID != -1)) || (p.ioSubGroupTbls.Count() == 0 && user.ID == -1))
                        &&
                        (p.publishDate.Date <= now)
                        && p.isBlock == false
                        );
            }
            if (groupID != -1 && user.ID != -1)
            {
                if (db.GroupTbls.Any(c => c.ID == groupID) == false)
                {
                    Result.result.code = 5;
                    Result.result.message = ClassCollection.Message.GROUP_NOT_EXIST;
                    Result.io = null;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                io = io.Where(c => c.ioSubGroupTbls.Any(p => p.SubGroupTbl.groupID == groupID));
            }
            if (subGroupID != -1 && user.ID != -1)
            {
                if (db.SubGroupTbls.Any(c => c.ID == subGroupID) == false)
                {
                    Result.result.code = 6;
                    Result.result.message = ClassCollection.Message.SUBGROUP_NOT_EXIST;
                    Result.io = null;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                io = io.Where(x => x.ioSubGroupTbls.Any(c => c.subGroupID == subGroupID));
            }

            #endregion

            io = io.Where(c => c.ioContentTbls.Any()).OrderByDescending(c => c.publishDate);
            io = io.Skip(skipCount).Take(count);
            Result.io = new List<ClassCollection.Model.LatestIoForList>();
            foreach (var item in io)
            {
                var tmp = new ClassCollection.Model.LatestIoForList();
                tmp.ID = item.ID;
                try
                {
                    tmp.image = item.ioContentTbls.Where(c => c.contentType == 1).Take(1).Single().value;
                }
                catch
                {
                    tmp.image = "";
                }
                tmp.like = item.ioLikeTbls.Where(c => c.isLike).Count();
                tmp.unlike = item.ioLikeTbls.Where(c => !c.isLike).Count();
                tmp.viewCount = item.viewCount;
                tmp.publishDate = Persia.Calendar.ConvertToPersian(item.publishDate).ToString("n");
                tmp.title = item.title;
                tmp.tag = new List<Model.Tag>();
                foreach (var t in item.ioTagTbls)
                {
                    tmp.tag.Add(new Model.Tag() { ID = t.tagTbl.ID, title = t.tagTbl.title });
                }
                try
                {
                    tmp.summary = item.ioContentTbls.Where(c => c.contentType == 0).Take(1).Single().value;
                }
                catch
                {
                    tmp.summary = item.title;
                }

                Result.io.Add(tmp);
            }
            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result));
            return;

        }

        /// <summary>
        /// این تابع وظیفه ارسال سازمان برای کاربر مورد نظر را بر عهده دارد.
        /// </summary>
        /// <param name="key">کلمه عبور برنامه ای که درحال استفاده از متد جاری می باشد</param>
        /// <param name="ioID">کد سازمان مورد نظر جهت نمایش جزییات آن
        ///</param>
        /// <returns>خروجی ساختار جی سون از کلاس زیر را دارد.
        /// <para><see cref="ClassCollection.Model.IoResult" /></para>
        /// <remarks>
        /// <para>Code : -1 , Message : <see cref="ClassCollection.Message.OPERATION_NO_ACCESS"/>, Result.io : <c>null</c></para>
        /// <para>Code : 0 , Message : <see cref="ClassCollection.Message.OPERATION_SUCCESS" />, Result.io : <see cref=" ClassCollection.Model.fullIo"/> </para>
        /// <para>Code : 1 , Message : <see cref="ClassCollection.Message.IO_NOT_EXIST" />, Result.io : <c>null</c></para>
        /// </remarks>
        /// </returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getIo(string key, long ioID)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.IoResult Result = new ClassCollection.Model.IoResult();
            Result.result = new ClassCollection.Model.Result();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            
            var db = new DataAccessDataContext();

            if (db.ioTbls.Any(c => c.ID == ioID && c.isBlock == false) == false)
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.IO_NOT_EXIST;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            var io = db.ioTbls.Single(c => c.ID == ioID);

            Result.io = new ClassCollection.Model.fullIo();
            if (io.mUserID != null)
                Result.io.mUserID = io.mUserID;
            else
                Result.io.mUserID = -1;
            Result.io.ID = io.ID;
            Result.io.like = io.ioLikeTbls.Count(c => c.isLike == true);
            Result.io.unlike = io.ioLikeTbls.Count(c => c.isLike == false);
            Result.io.publishDate = Persia.Calendar.ConvertToPersian(io.publishDate).ToString("n");
            Result.io.title = io.title;
            Result.io.viewCount = io.viewCount + 1;
            Result.io.tag = new List<Model.Tag>();
            foreach (var t in io.ioTagTbls)
            {
                Result.io.tag.Add(new Model.Tag() { title = t.tagTbl.title, ID = t.tagTbl.ID });
            }

            io.viewCount += 1;
            db.SubmitChanges();

            Result.io.contents = new List<ClassCollection.Model.ioContent>();
            var contents = io.ioContentTbls.OrderBy(c => c.pri);

            foreach (var item in contents)
            {
                var tmp = new ClassCollection.Model.ioContent();
                tmp.ID = item.ID;
                tmp.type = item.contentType;
                tmp.value = item.value;
                Result.io.contents.Add(tmp);
            }

            Result.io.groups = new List<ClassCollection.Model.ioGroupSubGroup>();
            var subgroups = io.ioSubGroupTbls;
            foreach (var item in subgroups)
            {
                var tmp = new ClassCollection.Model.ioGroupSubGroup();
                tmp.groupID = item.SubGroupTbl.groupID;
                tmp.groupTitle = item.SubGroupTbl.GroupTbl.title;
                tmp.ID = item.ID;
                tmp.ioID = item.ioID;
                tmp.subGroupID = item.subGroupID;
                tmp.subGroupTitle = item.SubGroupTbl.title;
                Result.io.groups.Add(tmp);

            }

            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result));
            return;

        }

        /// <summary>
        /// این تابع وظیفه ارسال سازمان برای کاربر مورد نظر را بر عهده دارد.
        /// </summary>
        /// <param name="key">کلمه عبور برنامه ای که درحال استفاده از متد جاری می باشد</param>
        /// <param name="ioID">کد سازمان مورد نظر جهت نمایش جزییات آن
        ///</param>
        /// <returns>خروجی ساختار جی سون از کلاس زیر را دارد.
        /// <para><see cref="ClassCollection.Model.SingleIoResult" /></para>
        /// <remarks>
        /// <para>Code : -1 , Message : <see cref="ClassCollection.Message.OPERATION_NO_ACCESS"/>, Result.io : <c>null</c></para>
        /// <para>Code : 0 , Message : <see cref="ClassCollection.Message.OPERATION_SUCCESS" />, Result.io : <see cref=" ClassCollection.Model.singleFullIo"/> </para>
        /// <para>Code : 1 , Message : <see cref="ClassCollection.Message.IO_NOT_EXIST" />, Result.io : <c>null</c></para>
        /// </remarks>
        /// </returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getSingleIo(string key, long ioID)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.SingleIoResult Result = new ClassCollection.Model.SingleIoResult();
            Result.result = new ClassCollection.Model.Result();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            
            var db = new DataAccessDataContext();

            if (db.ioTbls.Any(c => c.ID == ioID && c.isBlock == false) == false)
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.IO_NOT_EXIST;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            var io = db.ioTbls.Single(c => c.ID == ioID);

            Result.singleio = new ClassCollection.Model.singleFullIo();
            Result.singleio.userID = io.userID;
            Result.singleio.ID = io.ID;
            Result.singleio.like = io.ioLikeTbls.Count(c => c.isLike == true);
            Result.singleio.unlike = io.ioLikeTbls.Count(c => c.isLike == false);
            Result.singleio.publishDate = Persia.Calendar.ConvertToPersian(io.publishDate).ToString("n");
            Result.singleio.title = io.title;
            Result.singleio.viewCount = io.viewCount + 1;

            io.viewCount += 1;
            db.SubmitChanges();

            var contents = io.ioContentTbls.OrderBy(c => c.pri);

            try
            {
                Result.singleio.image = contents.First(c => c.contentType == 1).value;
            }
            catch
            {
                Result.singleio.image = "";
            }

            try
            {
                Result.singleio.video = contents.First(c => c.contentType == 2).value;
            }
            catch
            {
                Result.singleio.video = "";
            }
            try
            {
                Result.singleio.text = contents.First(c => c.contentType == 0).value;
            }
            catch
            {
                Result.singleio.text = "";
            }

            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result));
            return;

        }

        /// <summary>
        /// این تابع وظیفه ثبت لایک و آنلایک برای سازمان توسط کاربر مورد نظر را بر عهده دارد.
        /// </summary>
        /// <param name="key">کلمه عبور برنامه ای که درحال استفاده از متد جاری می باشد</param>
        /// <param name="ioID">کد سازمان مورد نظر جهت ثبت نظر برای آن
        ///</param>
        /// <param name="userID">کد کاربری که در حال ثبت نظر برای سازمان می باشد
        /// <remarks>
        /// در صورتی که کاربر مهمان می باشد مقدار -1 را قرار دهید
        /// </remarks>
        ///</param>
        /// <param name="isLike">نوع نظر درحال ثبت کاربر برای خبر
        /// <remarks>
        /// <para>True : لایک برای سازمان درج میشود</para>
        /// <para>False : آنلایک برای سازمان درج میشود</para>
        /// </remarks>
        ///</param>
        /// <returns>خروجی ساختار جی سون از کلاس زیر را دارد.
        /// <para><see cref="ClassCollection.Model.Result" /></para>
        /// <remarks>
        /// <para>Code : -1 , Message : <see cref="ClassCollection.Message.OPERATION_NO_ACCESS"/></para>
        /// <para>Code : 0 , Message : <see cref="ClassCollection.Message.OPERATION_SUCCESS" /></para>
        /// <para>Code : 1 , Message : <see cref="ClassCollection.Message.IO_NOT_EXIST" /></para>
        /// <para>Code : 2 , Message : <see cref="ClassCollection.Message.USER_NOT_EXIST" /></para>
        /// </remarks>
        /// </returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void likeUnlikeIo(string key, long userID, long ioID, bool isLike)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.Result Result = new ClassCollection.Model.Result();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.code = -1;
                Result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            DateTime dt = new DateTime();
            dt = DateTime.Now;
            var db = new DataAccessDataContext();

            if (db.ioTbls.Any(c => c.ID == ioID && c.isBlock == false) == false)
            {
                Result.code = 1;
                Result.message = ClassCollection.Message.IO_NOT_EXIST;

                Context.Response.Write(js.Serialize(Result));
                return;
            }
            if (userID != -1)
            {
                if (db.mUserTbls.Any(c => c.ID == userID && c.isBlock == false) == false)
                {
                    Result.code = 2;
                    Result.message = ClassCollection.Message.USER_NOT_EXIST;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }

            }

            if (db.ioLikeTbls.Any(c => c.userID == userID && c.ioID == ioID) == false)
            {
                var iolike = new ioLikeTbl();
                iolike.isLike = isLike;
                iolike.ioID = ioID;
                iolike.userID = userID == -1 ? 3 : userID;
                iolike.regDate = dt;
                db.ioLikeTbls.InsertOnSubmit(iolike);
                db.SubmitChanges();
            }

            Result.code = 0;
            Result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result));
            return;

        }

        /// <summary>
        /// این تابع وظیفه ارسال سازمان ها دریاقت نشده برای کاربر مورد نظر را بر عهده دارد.
        /// </summary>
        /// <param name="key">کلمه عبور برنامه ای که درحال استفاده از متد جاری می باشد</param>
        /// <param name="mobileOrEmail">شماره موبایل یا ایمیل کاربر
        /// <remarks>
        ///  <para>در صورت قرار دادن شماره موبایل در این پارامتر طول آن باید برابر با 12 کاراکتر باشد</para>
        ///  <para>در صورت قرار دادن ایمیل در این پارامتر طول آن باید کمتر از 250 کاراکتر باید</para> 
        ///  <para>در صورتی که فقط نیاز به دریافت سازمان های عمومی می باشد مقدار این پارامتر را مقدار رشته خالی قرار دهید</para>
        /// </remarks>
        /// </param>
        /// <param name="lastIoID" >کد آخرین سازمان جهت ارسال سازمان های درج شده ی بعد از آن
        /// </param>
        /// <returns>خروجی ساختار جی سون از کلاس زیر را دارد.
        /// <para><see cref="ClassCollection.Model.LatestIoForNotificationResult" /></para>
        /// <remarks>
        /// <para>Code : -1, Message : <see cref="ClassCollection.Message.OPERATION_NO_ACCESS"/>, Result.io : <c>null</c></para>
        /// <para>Code : 0 , Message : <see cref="ClassCollection.Message.OPERATION_SUCCESS" />, Result.io : List of <see cref=" ClassCollection.Model.LatestIoForNotification"/></para>
        /// <para>Code : 1 , Message : <see cref="ClassCollection.Message.ERROR_INPUT_PARAMETER" />, Result.io : <c>null</c></para>
        /// <para>Code : 2 , Message : <see cref="ClassCollection.Message.USER_NOT_EXIST" />, Result.io : <c>null</c></para>
        /// <para>Code : 3 , Message : <see cref="ClassCollection.Message.USER_BLOCKED" />, Result.io : <c>null</c></para>
        /// </remarks>
        /// </returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getLatestIoForNotification(string key, string mobileOrEmail, long lastIoID)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.LatestIoForNotificationResult Result = new ClassCollection.Model.LatestIoForNotificationResult();
            Result.result = new ClassCollection.Model.Result();

            DateTime dt = new DateTime();
            dt = DateTime.Now.Date;

            var db = new DataAccessDataContext();
            var user = new mUserTbl();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            mobileOrEmail = mobileOrEmail.Trim();

            if (mobileOrEmail == "")
            {
                mobileOrEmail = "-1";
                user.ID = -1;
            }
            else
            {

                if (mobileOrEmail.Contains("@") && mobileOrEmail.Length >= 250)
                {
                    Result.result.code = 1;
                    Result.result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                if (!mobileOrEmail.Contains("@") && mobileOrEmail.Length != 12)
                {
                    Result.result.code = 1;
                    Result.result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }

                if (!db.mUserTbls.Any(c => c.emailtell == mobileOrEmail))
                {
                    Result.result.code = 2;
                    Result.result.message = ClassCollection.Message.USER_NOT_EXIST;
                    Result.io = null;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                else
                {
                    user = db.mUserTbls.Single(c => c.emailtell == mobileOrEmail);
                    if (user.isBlock)
                    {
                        Result.result.code = 3;
                        Result.result.message = ClassCollection.Message.USER_BLOCKED;
                        Result.io = null;
                        Context.Response.Write(js.Serialize(Result));
                        return;
                    }
                }

            }

            IQueryable<ioTbl> io = db.ioTbls;

            io = io.Where(p => (((p.ioSubGroupTbls.Any(c => c.SubGroupTbl.userSubGroupTbls.Any(z => z.userID == user.ID) && user.ID != -1)) || (p.ioSubGroupTbls.Count() == 0 && user.ID == -1)) && p.ID > lastIoID)
                    &&
                    (p.publishDate.Date <= dt)
                    && p.isBlock == false
                    );

            io = io.OrderByDescending(c => c.publishDate);

            Result.io = new List<ClassCollection.Model.LatestIoForNotification>();
            foreach (var item in io)
            {
                var tmp = new ClassCollection.Model.LatestIoForNotification();
                tmp.ID = item.ID;
                tmp.title = item.title;

                Result.io.Add(tmp);
            }
            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result));
            return;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void sendIoComment(string key, long ioID, long userID, string mobileOrEmail, string fullName, string text)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.Result Result = new ClassCollection.Model.Result();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.code = -1;
                Result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            fullName = fullName.TrimEnd().TrimStart();
            text = text.TrimStart().TrimEnd();

            if (fullName.Length == 0)
            {
                Result.code = 1;
                Result.message = ClassCollection.Message.NAME_INCORRECT;

                Context.Response.Write(js.Serialize(Result));
                return;
            }
            if (text.Length == 0 || text.Length > 250)
            {
                Result.code = 2;
                Result.message = ClassCollection.Message.COMMENT_TEXT_INCORRECT;

                Context.Response.Write(js.Serialize(Result));
                return;
            }
            //if (Methods.isEmail(mobileOrEmail) == false)
            //{
            //    if (mobileOrEmail.Length != 11)
            //    {
            //        Result.code = 3;
            //        Result.message = ClassCollection.Message.MOBILE_INCORRECT;

            //        Context.Response.Write(js.Serialize(Result));
            //        return;
            //    }
            //}

            DateTime dt = new DateTime();
            dt = DateTime.Now;
            var db = new DataAccessDataContext();

            if (db.ioTbls.Any(c => c.ID == ioID && c.isBlock == false) == false)
            {
                Result.code = 4;
                Result.message = ClassCollection.Message.IO_NOT_EXIST;

                Context.Response.Write(js.Serialize(Result));
                return;
            }
            if (db.mUserTbls.Any(c => c.ID == userID) == false && userID != -1)
            {
                Result.code = 5;
                Result.message = ClassCollection.Message.USER_NOT_EXIST;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            var ioComment = new ioCommentTbl();
            ioComment.fullName = fullName;
            ioComment.text = text;
            ioComment.regDate = dt;
            ioComment.ioID = ioID;
            ioComment.mUserID = userID;
            ioComment.mobile = Methods.isEmail(mobileOrEmail) ? null : mobileOrEmail;
            ioComment.email = Methods.isEmail(mobileOrEmail) ? mobileOrEmail : null;
            ioComment.isBlock = true;
            db.ioCommentTbls.InsertOnSubmit(ioComment);
            db.SubmitChanges();


            Result.code = 0;
            Result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result));
            return;

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getIoCommentList(string key, long ioID, int count, int pageIndex)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.CommentListResult Result = new ClassCollection.Model.CommentListResult();
            Result.result = new ClassCollection.Model.Result();


            var skipCount = count * (pageIndex - 1);
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }
            if (db.ioTbls.Any(c => c.ID == ioID) == false)
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.IO_NOT_EXIST;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            Result.comment = new List<Model.Comment>();
            var comments = db.ioCommentTbls.Where(c => c.isBlock == false && c.ioID == ioID).OrderByDescending(c => c.regDate);
            Result.comment = comments.Skip(skipCount).Take(count).Select(c => new Model.Comment() { fullName = c.fullName, ID = c.ID, text = c.text }).ToList<Model.Comment>();


            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result));
            return;

        }

        #endregion

        #region download
        /// <summary>
        /// این تابع وظیفه ارسال دانلود برای کاربر مورد نظر را بر عهده دارد.
        /// </summary>
        /// <param name="key">کلمه عبور برنامه ای که درحال استفاده از متد جاری می باشد</param>
        /// <param name="mobileOrEmail">شماره موبایل یا ایمیل کاربر
        /// <remarks>
        ///  <para>در صورت قرار دادن شماره موبایل در این پارامتر طول آن باید برابر با 12 کاراکتر باشد</para>
        ///  <para>در صورت قرار دادن ایمیل در این پارامتر طول آن باید کمتر از 250 کاراکتر باید</para> 
        ///  <para>در صورتی که فقط نیاز به دریافت اخبار عمومی می باشد مقدار این پارامتر را مقدار رشته خالی قرار دهید</para>
        /// </remarks>
        /// </param>
        /// <param name="fillterString" >رشته ی مورد نظر کاربر جهت جستجو در بین اخبار
        /// <remarks>
        ///- اجزای جستجو با فاصله از یکدیگر جدا میشوند
        ///- درصورتی که کلمه مورد نظر در بین دو علامت دابل کوتیشن قرار بگیرد اخبار یافت شده حتما شامل آن کلمه خواهد بود
        /// </remarks>
        /// </param>
        /// <param name="groupID">کد گروهی که جهت استفاده در فیلتر کردن مد نظر میباشد
        /// <remarks>در صورت عدم نیاز به لحاظ این پارامتر در جستجو مقدار -1 را قرار دهید</remarks>
        /// </param>
        /// <param name="subGroupID">کد زیرگروهی که جهت استفاده در فیلتر کردن مد نظر میباشد
        /// <remarks>در صورت عدم نیاز به لحاظ این پارامتر در جستجو مقدار -1 را قرار دهید</remarks>
        /// </param>
        /// <param name="count">تعداد نتایج درخواستی در هر صفحه</param>
        /// <param name="pageIndex">شماره صفحه مورد نظر از جستجو</param>
        /// 
        /// <returns>خروجی ساختار جی سون از کلاس زیر را دارد.
        /// <para><see cref="ClassCollection.Model.LatestDownloadForListResult" /></para>
        /// <remarks>
        /// <para>Code : -1 , Message : <see cref="ClassCollection.Message.OPERATION_NO_ACCESS"/>, Result.download : <c>null</c></para>
        /// <para>Code : 0 , Message : <see cref="ClassCollection.Message.OPERATION_SUCCESS" />, Result.download : List of <see cref=" ClassCollection.Model.LatestDownloadForList"/></para>
        /// <para>Code : 1 , Message : <see cref="ClassCollection.Message.ERROR_INPUT_PARAMETER" />, Result.download : <c>null</c></para>
        /// <para>Code : 2 , Message : <see cref="ClassCollection.Message.USER_NOT_EXIST" />, Result.download : <c>null</c></para>
        /// <para>Code : 3 , Message : <see cref="ClassCollection.Message.USER_BLOCKED" />, Result.download : <c>null</c></para>
        /// <para>Code : 4 , Message : <see cref="ClassCollection.Message.GROUP_NOT_EXIST" />, Result.download : <c>null</c></para> 
        /// <para>Code : 5 , Message : <see cref="ClassCollection.Message.SUBGROUP_NOT_EXIST" />, Result.download : <c>null</c></para>
        /// </remarks>
        /// </returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getLatestDownloadForList(string key, string mobileOrEmail, string fillterString, long groupID, long subGroupID, int count, int pageIndex, long tagID)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.LatestDownloadForListResult Result = new ClassCollection.Model.LatestDownloadForListResult();
            Result.result = new ClassCollection.Model.Result();

            var skipCount = count * (pageIndex - 1);
            var now = new DateTime();
            now = DateTime.Now.Date;
            var db = new DataAccessDataContext();
            var user = new mUserTbl();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            mobileOrEmail = mobileOrEmail.Trim();

            if (mobileOrEmail == "")
            {
                mobileOrEmail = "-1";
                user.ID = -1;
            }
            else
            {

                if (mobileOrEmail.Contains("@") && mobileOrEmail.Length >= 250)
                {
                    Result.result.code = 1;
                    Result.result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                if (!mobileOrEmail.Contains("@") && mobileOrEmail.Length != 12)
                {
                    Result.result.code = 1;
                    Result.result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }

                if (!db.mUserTbls.Any(c => c.emailtell == mobileOrEmail))
                {
                    Result.result.code = 2;
                    Result.result.message = ClassCollection.Message.USER_NOT_EXIST;
                    Result.download = null;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                else
                {
                    user = db.mUserTbls.Single(c => c.emailtell == mobileOrEmail);
                    if (user.isBlock)
                    {
                        Result.result.code = 3;
                        Result.result.message = ClassCollection.Message.USER_BLOCKED;
                        Result.download = null;
                        Context.Response.Write(js.Serialize(Result));
                        return;
                    }
                }

            }

            IQueryable<downloadTbl> download = db.downloadTbls;
            if (tagID != -1 && db.tagTbls.Any(c => c.ID == tagID))
                download = download.Where(c => c.downloadTagTbls.Any(p => p.tagID == tagID));
            #region search
            if (fillterString != "")
            {
                var search = new Search(fillterString);
                var ands = search.getAND;
                var ors = search.getOR;


                var predicate = PredicateBuilder.False<downloadTbl>();
                foreach (string keyword in ors)
                {
                    string temp = keyword;
                    predicate = predicate.Or(p => ((p.downloadSubGroupTbls.Any(c => c.SubGroupTbl.userSubGroupTbls.Any(z => z.userID == user.ID) && user.ID != -1)) || (p.downloadSubGroupTbls.Count() == 0 && user.ID == -1))
                        &&
                        (p.title.Contains(temp) || p.downloadContentTbls.Any(v => v.contentType == 0 && v.value.Contains(temp)) || p.downloadTagTbls.Any(x => x.tagTbl.title.Contains(keyword)))
                        &&
                        (p.publishDate.Date <= now)
                        && p.isBlock == false
                        );
                }


                foreach (var keyword in ands)
                {
                    download = download.Where(p => ((p.downloadSubGroupTbls.Any(c => c.SubGroupTbl.userSubGroupTbls.Any(z => z.userID == user.ID) && user.ID != -1)) || (p.downloadSubGroupTbls.Count() == 0 && user.ID == -1))
                        &&
                        (p.title.Contains(keyword) || p.downloadContentTbls.Any(v => v.contentType == 0 && v.value.Contains(keyword)))
                        &&
                        (p.publishDate.Date <= now)
                        && p.isBlock == false
                        );
                }
                download = download.Where(predicate);
            }
            else
            {
                download = download.Where(p => ((p.downloadSubGroupTbls.Any(c => c.SubGroupTbl.userSubGroupTbls.Any(z => z.userID == user.ID) && user.ID != -1)) || (p.downloadSubGroupTbls.Count() == 0 && user.ID == -1))
                        &&
                        (p.publishDate.Date <= now)
                        && p.isBlock == false
                        );
            }
            if (groupID != -1 && user.ID != -1)
            {
                if (db.GroupTbls.Any(c => c.ID == groupID) == false)
                {
                    Result.result.code = 4;
                    Result.result.message = ClassCollection.Message.GROUP_NOT_EXIST;
                    Result.download = null;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                download = download.Where(c => c.downloadSubGroupTbls.Any(p => p.SubGroupTbl.groupID == groupID));
            }
            if (subGroupID != -1 && user.ID != -1)
            {
                if (db.SubGroupTbls.Any(c => c.ID == subGroupID) == false)
                {
                    Result.result.code = 5;
                    Result.result.message = ClassCollection.Message.SUBGROUP_NOT_EXIST;
                    Result.download = null;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                download = download.Where(x => x.downloadSubGroupTbls.Any(c => c.subGroupID == subGroupID));
            }

            #endregion

            download = download.Where(c => c.downloadContentTbls.Any()).OrderByDescending(c => c.publishDate);
            download = download.Skip(skipCount).Take(count);
            Result.download = new List<ClassCollection.Model.LatestDownloadForList>();
            foreach (var item in download)
            {
                var tmp = new ClassCollection.Model.LatestDownloadForList();
                tmp.ID = item.ID;
                try
                {
                    tmp.image = item.downloadContentTbls.Where(c => c.contentType == 1).Take(1).Single().value;
                }
                catch
                {
                    tmp.image = "";
                }
                tmp.like = item.downloadLikeTbls.Where(c => c.isLike).Count();
                tmp.unlike = item.downloadLikeTbls.Where(c => !c.isLike).Count();
                tmp.viewCount = item.viewCount;
                tmp.publishDate = Persia.Calendar.ConvertToPersian(item.publishDate).ToString("n");
                tmp.title = item.title;
                tmp.tag = new List<Model.Tag>();
                foreach (var t in item.downloadTagTbls)
                {
                    tmp.tag.Add(new Model.Tag() { ID = t.tagID, title = t.tagTbl.title });
                }
                try
                {
                    tmp.summary = item.downloadContentTbls.Where(c => c.contentType == 0).Take(1).Single().value;
                }
                catch
                {
                    tmp.summary = item.title;
                }

                Result.download.Add(tmp);
            }
            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result));
            return;

        }

        /// <summary>
        /// این تابع وظیفه ارسال پست دانلود برای کاربر مورد نظر را بر عهده دارد.
        /// </summary>
        /// <param name="key">کلمه عبور برنامه ای که درحال استفاده از متد جاری می باشد</param>
        /// <param name="downloadID">کد دانلود مورد نظر جهت نمایش جزییات آن
        ///</param>
        /// <returns>خروجی ساختار جی سون از کلاس زیر را دارد.
        /// <para><see cref="ClassCollection.Model.DownloadResult" /></para>
        /// <remarks>
        /// <para>Code : -1 , Message : <see cref="ClassCollection.Message.OPERATION_NO_ACCESS"/>, Result.download : <c>null</c></para>
        /// <para>Code : 0 , Message : <see cref="ClassCollection.Message.OPERATION_SUCCESS" />, Result.download :  <see cref=" ClassCollection.Model.fullDownload"/> </para>
        /// <para>Code : 1 , Message : <see cref="ClassCollection.Message.DOWNLOAD_NOT_EXIST" />, Result.download : <c>null</c></para>
        /// </remarks>
        /// </returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getDownload(string key, long downloadID)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.DownloadResult Result = new ClassCollection.Model.DownloadResult();
            Result.result = new ClassCollection.Model.Result();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }
            
            var db = new DataAccessDataContext();

            if (db.downloadTbls.Any(c => c.ID == downloadID && c.isBlock == false) == false)
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.DOWNLOAD_NOT_EXIST;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            var download = db.downloadTbls.Single(c => c.ID == downloadID);

            Result.download = new ClassCollection.Model.fullDownload();
            if (download.mUserID != null)
                Result.download.mUserID = download.mUserID;
            else
                Result.download.mUserID = -1;
            Result.download.ID = download.ID;
            Result.download.like = download.downloadLikeTbls.Count(c => c.isLike == true);
            Result.download.unlike = download.downloadLikeTbls.Count(c => c.isLike == false);
            Result.download.publishDate = Persia.Calendar.ConvertToPersian(download.publishDate).ToString("n");
            Result.download.title = download.title;
            Result.download.viewCount = download.viewCount + 1;
            Result.download.tag = new List<Model.Tag>();

            download.viewCount += 1;
            db.SubmitChanges();

            foreach (var t in download.downloadTagTbls)
            {
                Result.download.tag.Add(new Model.Tag() { title = t.tagTbl.title, ID = t.tagID });
            }

            Result.download.contents = new List<ClassCollection.Model.downloadContent>();
            var contents = download.downloadContentTbls.OrderBy(c => c.pri);

            foreach (var item in contents)
            {
                var tmp = new ClassCollection.Model.downloadContent();
                tmp.ID = item.ID;
                tmp.type = item.contentType;
                tmp.value = item.value;
                if (item.contentType == 2)
                {
                    tmp.fileType = item.fileType;
                    tmp.fileSize = (int)item.fileSize;
                    tmp.downloadCount = item.downloadCount == null ? 0 : (int)item.downloadCount;
                }
                Result.download.contents.Add(tmp);
            }

            Result.download.groups = new List<ClassCollection.Model.downloadGroupSubGroup>();
            var subgroups = download.downloadSubGroupTbls;
            foreach (var item in subgroups)
            {
                var tmp = new ClassCollection.Model.downloadGroupSubGroup();
                tmp.groupID = item.SubGroupTbl.groupID;
                tmp.groupTitle = item.SubGroupTbl.GroupTbl.title;
                tmp.ID = item.ID;
                tmp.downloadID = item.downloadID;
                tmp.subGroupID = item.subGroupID;
                tmp.subGroupTitle = item.SubGroupTbl.title;
                Result.download.groups.Add(tmp);

            }

            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result));
            return;

        }

        /// <summary>
        /// این تابع وظیفه ارسال پست دانلود برای کاربر مورد نظر را بر عهده دارد.
        /// </summary>
        /// <param name="key">کلمه عبور برنامه ای که درحال استفاده از متد جاری می باشد</param>
        /// <param name="downloadID">کد دانلود مورد نظر جهت نمایش جزییات آن
        ///</param>
        /// <returns>خروجی ساختار جی سون از کلاس زیر را دارد.
        /// <para><see cref="ClassCollection.Model.SingleDownloadResult" /></para>
        /// <remarks>
        /// <para>Code : -1 , Message : <see cref="ClassCollection.Message.OPERATION_NO_ACCESS"/>, Result.download : <c>null</c></para>
        /// <para>Code : 0 , Message : <see cref="ClassCollection.Message.OPERATION_SUCCESS" />, Result.download :  <see cref=" ClassCollection.Model.SingleFullDownload"/> </para>
        /// <para>Code : 1 , Message : <see cref="ClassCollection.Message.DOWNLOAD_NOT_EXIST" />, Result.download : <c>null</c></para>
        /// </remarks>
        /// </returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getSingleDownload(string key, long downloadID)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.SingleDownloadResult Result = new ClassCollection.Model.SingleDownloadResult();
            Result.result = new ClassCollection.Model.Result();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }


            var db = new DataAccessDataContext();

            if (db.downloadTbls.Any(c => c.ID == downloadID && c.isBlock == false) == false)
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.DOWNLOAD_NOT_EXIST;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            var download = db.downloadTbls.Single(c => c.ID == downloadID);

            Result.singledownload = new ClassCollection.Model.SingleFullDownload();
            Result.singledownload.userID = download.userID;
            Result.singledownload.ID = download.ID;
            Result.singledownload.like = download.downloadLikeTbls.Count(c => c.isLike == true);
            Result.singledownload.unlike = download.downloadLikeTbls.Count(c => c.isLike == false);
            Result.singledownload.publishDate = Persia.Calendar.ConvertToPersian(download.publishDate).ToString("n");
            Result.singledownload.title = download.title;
            Result.singledownload.viewCount = download.viewCount + 1;

            download.viewCount += 1;
            db.SubmitChanges();

            var contents = download.downloadContentTbls.OrderBy(c => c.pri);

            try
            {
                Result.singledownload.image = contents.First(c => c.contentType == 1).value;
            }
            catch
            {
                Result.singledownload.image = "";
            }

            try
            {
                Result.singledownload.file = contents.First(c => c.contentType == 2).value;
            }
            catch
            {
                Result.singledownload.file = "";
            }
            try
            {
                Result.singledownload.text = contents.First(c => c.contentType == 0).value;
            }
            catch
            {
                Result.singledownload.text = "";
            }

            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result));
            return;

        }

        /// <summary>
        /// این تابع وظیفه ثبت لایک و آنلایک برای دانلود توسط کاربر مورد نظر را بر عهده دارد.
        /// </summary>
        /// <param name="key">کلمه عبور برنامه ای که درحال استفاده از متد جاری می باشد</param>
        /// <param name="downloadID">کد دانلود مورد نظر جهت ثبت نظر برای آن
        ///</param>
        /// <param name="userID">کد کاربری که در حال ثبت نظر برای دانلود می باشد
        /// <remarks>
        /// در صورتی که کاربر مهمان می باشد مقدار -1 را قرار دهید
        /// </remarks>
        ///</param>
        /// <param name="isLike">نوع نظر درحال ثبت کاربر برای خبر
        /// <remarks>
        /// <para>True : لایک برای دانلود درج میشود</para>
        /// <para>False : آنلایک برای دانلود درج میشود</para>
        /// </remarks>
        ///</param>
        /// <returns>خروجی ساختار جی سون از کلاس زیر را دارد.
        /// <para><see cref="ClassCollection.Model.Result" /></para>
        /// <remarks>
        /// <para>Code : -1 , Message : <see cref="ClassCollection.Message.OPERATION_NO_ACCESS"/></para>
        /// <para>Code : 0 , Message : <see cref="ClassCollection.Message.OPERATION_SUCCESS" /></para>
        /// <para>Code : 1 , Message : <see cref="ClassCollection.Message.NEWS_NOT_EXIST" /></para>
        /// <para>Code : 2 , Message : <see cref="ClassCollection.Message.USER_NOT_EXIST" /></para>
        /// </remarks>
        /// </returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void likeUnlikeDownload(string key, long userID, long downloadID, bool isLike)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.Result Result = new ClassCollection.Model.Result();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.code = -1;
                Result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            DateTime dt = new DateTime();
            dt = DateTime.Now;
            var db = new DataAccessDataContext();

            if (db.downloadTbls.Any(c => c.ID == downloadID && c.isBlock == false) == false)
            {
                Result.code = 1;
                Result.message = ClassCollection.Message.DOWNLOAD_NOT_EXIST;

                Context.Response.Write(js.Serialize(Result));
                return;
            }
            if (userID != -1)
            {
                if (db.mUserTbls.Any(c => c.ID == userID && c.isBlock == false) == false)
                {
                    Result.code = 2;
                    Result.message = ClassCollection.Message.USER_NOT_EXIST;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }

            }

            if (db.downloadLikeTbls.Any(c => c.userID == userID && c.downloadID == downloadID) == false)
            {
                var downloadlike = new downloadLikeTbl();
                downloadlike.isLike = isLike;
                downloadlike.downloadID = downloadID;
                downloadlike.userID = userID == -1 ? 3 : userID;
                downloadlike.regDate = dt;
                db.downloadLikeTbls.InsertOnSubmit(downloadlike);
                db.SubmitChanges();
            }

            Result.code = 0;
            Result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result));
            return;

        }

        /// <summary>
        /// این تابع وظیفه ارسال دانلودها دریاقت نشده برای کاربر مورد نظر را بر عهده دارد.
        /// </summary>
        /// <param name="key">کلمه عبور برنامه ای که درحال استفاده از متد جاری می باشد</param>
        /// <param name="mobileOrEmail">شماره موبایل یا ایمیل کاربر
        /// <remarks>
        ///  <para>در صورت قرار دادن شماره موبایل در این پارامتر طول آن باید برابر با 12 کاراکتر باشد</para>
        ///  <para>در صورت قرار دادن ایمیل در این پارامتر طول آن باید کمتر از 250 کاراکتر باید</para> 
        ///  <para>در صورتی که فقط نیاز به دریافت دانلودها عمومی می باشد مقدار این پارامتر را مقدار رشته خالی قرار دهید</para>
        /// </remarks>
        /// </param>
        /// <param name="lastDownloadID" >کد آخرین دانلود جهت ارسال دانلودها درج شده ی بعد از آن
        /// </param>
        /// <returns>خروجی ساختار جی سون از کلاس زیر را دارد.
        /// <para><see cref="ClassCollection.Model.LatestDownloadForNotificationResult" /></para>
        /// <remarks>
        /// <para>Code : -1, Message : <see cref="ClassCollection.Message.OPERATION_NO_ACCESS"/>, Result.download : <c>null</c></para>
        /// <para>Code : 0 , Message : <see cref="ClassCollection.Message.OPERATION_SUCCESS" />, Result.download : List of <see cref=" ClassCollection.Model.LatestDownloadForNotification"/></para>
        /// <para>Code : 1 , Message : <see cref="ClassCollection.Message.ERROR_INPUT_PARAMETER" />, Result.download : <c>null</c></para>
        /// <para>Code : 2 , Message : <see cref="ClassCollection.Message.USER_NOT_EXIST" />, Result.download : <c>null</c></para>
        /// <para>Code : 3 , Message : <see cref="ClassCollection.Message.USER_BLOCKED" />, Result.download : <c>null</c></para>
        /// </remarks>
        /// </returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getLatestDownloadForNotification(string key, string mobileOrEmail, long lastDownloadID)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.LatestDownloadForNotificationResult Result = new ClassCollection.Model.LatestDownloadForNotificationResult();
            Result.result = new ClassCollection.Model.Result();

            DateTime dt = new DateTime();
            dt = DateTime.Now.Date;

            var db = new DataAccessDataContext();
            var user = new mUserTbl();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            mobileOrEmail = mobileOrEmail.Trim();

            if (mobileOrEmail == "")
            {
                mobileOrEmail = "-1";
                user.ID = -1;
            }
            else
            {

                if (mobileOrEmail.Contains("@") && mobileOrEmail.Length >= 250)
                {
                    Result.result.code = 1;
                    Result.result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                if (!mobileOrEmail.Contains("@") && mobileOrEmail.Length != 12)
                {
                    Result.result.code = 1;
                    Result.result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }

                if (!db.mUserTbls.Any(c => c.emailtell == mobileOrEmail))
                {
                    Result.result.code = 2;
                    Result.result.message = ClassCollection.Message.USER_NOT_EXIST;
                    Result.download = null;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                else
                {
                    user = db.mUserTbls.Single(c => c.emailtell == mobileOrEmail);
                    if (user.isBlock)
                    {
                        Result.result.code = 3;
                        Result.result.message = ClassCollection.Message.USER_BLOCKED;
                        Result.download = null;
                        Context.Response.Write(js.Serialize(Result));
                        return;
                    }
                }

            }

            IQueryable<downloadTbl> download = db.downloadTbls;

            download = download.Where(p => (((p.downloadSubGroupTbls.Any(c => c.SubGroupTbl.userSubGroupTbls.Any(z => z.userID == user.ID) && user.ID != -1)) || (p.downloadSubGroupTbls.Count() == 0 && user.ID == -1)) && p.ID > lastDownloadID)
                    &&
                    (p.publishDate.Date <= dt)
                    && p.isBlock == false
                    );

            download = download.OrderByDescending(c => c.publishDate);

            Result.download = new List<ClassCollection.Model.LatestDownloadForNotification>();
            foreach (var item in download)
            {
                var tmp = new ClassCollection.Model.LatestDownloadForNotification();
                tmp.ID = item.ID;
                tmp.title = item.title;

                Result.download.Add(tmp);
            }
            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result));
            return;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void sendDownloadComment(string key, long downloadID, long userID, string mobileOrEmail, string fullName, string text)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.Result Result = new ClassCollection.Model.Result();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.code = -1;
                Result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            fullName = fullName.TrimEnd().TrimStart();
            text = text.TrimStart().TrimEnd();

            if (fullName.Length == 0)
            {
                Result.code = 1;
                Result.message = ClassCollection.Message.NAME_INCORRECT;

                Context.Response.Write(js.Serialize(Result));
                return;
            }
            if (text.Length == 0 || text.Length > 250)
            {
                Result.code = 2;
                Result.message = ClassCollection.Message.COMMENT_TEXT_INCORRECT;

                Context.Response.Write(js.Serialize(Result));
                return;
            }
            //if (Methods.isEmail(mobileOrEmail) == false)
            //{
            //    if (mobileOrEmail.Length != 11)
            //    {
            //        Result.code = 3;
            //        Result.message = ClassCollection.Message.MOBILE_INCORRECT;

            //        Context.Response.Write(js.Serialize(Result));
            //        return;
            //    }
            //}

            DateTime dt = new DateTime();
            dt = DateTime.Now;
            var db = new DataAccessDataContext();

            if (db.downloadTbls.Any(c => c.ID == downloadID && c.isBlock == false) == false)
            {
                Result.code = 4;
                Result.message = ClassCollection.Message.IO_NOT_EXIST;

                Context.Response.Write(js.Serialize(Result));
                return;
            }
            if (db.mUserTbls.Any(c => c.ID == userID) == false && userID != -1)
            {
                Result.code = 5;
                Result.message = ClassCollection.Message.USER_NOT_EXIST;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            var downloadComment = new downloadCommentTbl();
            downloadComment.fullName = fullName;
            downloadComment.text = text;
            downloadComment.regDate = dt;
            downloadComment.downloadID = downloadID;
            downloadComment.mUserID = userID;
            downloadComment.mobile = Methods.isEmail(mobileOrEmail) ? null : mobileOrEmail;
            downloadComment.email = Methods.isEmail(mobileOrEmail) ? mobileOrEmail : null;
            downloadComment.isBlock = true;
            db.downloadCommentTbls.InsertOnSubmit(downloadComment);
            db.SubmitChanges();


            Result.code = 0;
            Result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result));
            return;

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getDownloadCommentList(string key, long downloadID, int count, int pageIndex)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.CommentListResult Result = new ClassCollection.Model.CommentListResult();
            Result.result = new ClassCollection.Model.Result();


            var skipCount = count * (pageIndex - 1);
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }
            if (db.downloadTbls.Any(c => c.ID == downloadID) == false)
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.IO_NOT_EXIST;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            Result.comment = new List<Model.Comment>();
            var comments = db.downloadCommentTbls.Where(c => c.isBlock == false && c.downloadID == downloadID).OrderByDescending(c => c.regDate);
            Result.comment = comments.Skip(skipCount).Take(count).Select(c => new Model.Comment() { fullName = c.fullName, ID = c.ID, text = c.text }).ToList<Model.Comment>();


            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result));
            return;

        }

        #endregion

        #region publication
        /// <summary>
        /// این تابع وظیفه ارسال انتشارات برای کاربر مورد نظر را بر عهده دارد.
        /// </summary>
        /// <param name="key">کلمه عبور برنامه ای که درحال استفاده از متد جاری می باشد</param>
        /// <param name="mobileOrEmail">شماره موبایل یا ایمیل کاربر
        /// <remarks>
        ///  <para>در صورت قرار دادن شماره موبایل در این پارامتر طول آن باید برابر با 12 کاراکتر باشد</para>
        ///  <para>در صورت قرار دادن ایمیل در این پارامتر طول آن باید کمتر از 250 کاراکتر باید</para> 
        ///  <para>در صورتی که فقط نیاز به دریافت انتشارات عمومی می باشد مقدار این پارامتر را مقدار رشته خالی قرار دهید</para>
        /// </remarks>
        /// </param>
        /// <param name="fillterString" >رشته ی مورد نظر کاربر جهت جستجو در بین انتشارات
        /// <remarks>
        ///- اجزای جستجو با فاصله از یکدیگر جدا میشوند
        ///- درصورتی که کلمه مورد نظر در بین دو علامت دابل کوتیشن قرار بگیرد انتشارات یافت شده حتما شامل آن کلمه خواهد بود
        /// </remarks>
        /// </param>
        /// <param name="groupID">کد گروهی که جهت استفاده در فیلتر کردن مد نظر میباشد
        /// <remarks>در صورت عدم نیاز به لحاظ این پارامتر در جستجو مقدار -1 را قرار دهید</remarks>
        /// </param>
        /// <param name="subGroupID">کد زیرگروهی که جهت استفاده در فیلتر کردن مد نظر میباشد
        /// <remarks>در صورت عدم نیاز به لحاظ این پارامتر در جستجو مقدار -1 را قرار دهید</remarks>
        /// </param>
        /// <param name="count">تعداد نتایج درخواستی در هر صفحه</param>
        /// <param name="pageIndex">شماره صفحه مورد نظر از جستجو</param>
        /// 
        /// <returns>خروجی ساختار جی سون از کلاس زیر را دارد.
        /// <para><see cref="ClassCollection.Model.LatestPublicationsForListResult" /></para>
        /// <remarks>
        /// <para>Code : -1 , Message : <see cref="ClassCollection.Message.OPERATION_NO_ACCESS"/>, Result.publication : <c>null</c></para>
        /// <para>Code : 0 , Message : <see cref="ClassCollection.Message.OPERATION_SUCCESS" />, Result.publication : List of <see cref=" ClassCollection.Model.LatestPublicationsForList"/></para>
        /// <para>Code : 1 , Message : <see cref="ClassCollection.Message.ERROR_INPUT_PARAMETER" />, Result.publication : <c>null</c></para>
        /// <para>Code : 2 , Message : <see cref="ClassCollection.Message.USER_NOT_EXIST" />, Result.publication : <c>null</c></para>
        /// <para>Code : 3 , Message : <see cref="ClassCollection.Message.USER_BLOCKED" />, Result.publication : <c>null</c></para>
        /// <para>Code : 4 , Message : <see cref="ClassCollection.Message.GROUP_NOT_EXIST" />, Result.publication : <c>null</c></para> 
        /// <para>Code : 5 , Message : <see cref="ClassCollection.Message.SUBGROUP_NOT_EXIST" />, Result.publication : <c>null</c></para>
        /// </remarks>
        /// </returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getLatestPubForList(string key, string mobileOrEmail, string fillterString, long groupID, long subGroupID, int count, int pageIndex, long tagID)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.LatestPublicationsForListResult Result = new ClassCollection.Model.LatestPublicationsForListResult();
            Result.result = new ClassCollection.Model.Result();

            var skipCount = count * (pageIndex - 1);
            var now = new DateTime();
            now = DateTime.Now.Date;
            var db = new DataAccessDataContext();
            var user = new mUserTbl();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            mobileOrEmail = mobileOrEmail.Trim();

            if (mobileOrEmail == "")
            {
                mobileOrEmail = "-1";
                user.ID = -1;
            }
            else
            {

                if (mobileOrEmail.Contains("@") && mobileOrEmail.Length >= 250)
                {
                    Result.result.code = 1;
                    Result.result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                if (!mobileOrEmail.Contains("@") && mobileOrEmail.Length != 12)
                {
                    Result.result.code = 1;
                    Result.result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }

                if (!db.mUserTbls.Any(c => c.emailtell == mobileOrEmail))
                {
                    Result.result.code = 2;
                    Result.result.message = ClassCollection.Message.USER_NOT_EXIST;
                    Result.publications = null;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                else
                {
                    user = db.mUserTbls.Single(c => c.emailtell == mobileOrEmail);
                    if (user.isBlock)
                    {
                        Result.result.code = 3;
                        Result.result.message = ClassCollection.Message.USER_BLOCKED;
                        Result.publications = null;
                        Context.Response.Write(js.Serialize(Result));
                        return;
                    }
                }

            }

            IQueryable<publicationTbl> publication = db.publicationTbls;

            if (tagID != -1 && db.tagTbls.Any(c => c.ID == tagID))
                publication = publication.Where(c => c.publicationTagTbls.Any(p => p.tagID == tagID));

            #region search
            if (fillterString != "")
            {
                var search = new Search(fillterString);
                var ands = search.getAND;
                var ors = search.getOR;


                var predicate = PredicateBuilder.False<publicationTbl>();
                foreach (string keyword in ors)
                {
                    string temp = keyword;
                    predicate = predicate.Or(p => ((p.publicationSubGroupTbls.Any(c => c.SubGroupTbl.userSubGroupTbls.Any(z => z.userID == user.ID) && user.ID != -1)) || (p.publicationSubGroupTbls.Count() == 0 && user.ID == -1))
                        &&
                        (p.title.Contains(temp) || p.publicationContentTbls.Any(v => v.contentType == 0 && v.value.Contains(temp)) || p.publicationTagTbls.Any(c => c.tagTbl.title.Contains(keyword)))
                        &&
                        (p.publishDate.Date <= now)
                        && p.isBlock == false
                        );
                }


                foreach (var keyword in ands)
                {
                    publication = publication.Where(p => ((p.publicationSubGroupTbls.Any(c => c.SubGroupTbl.userSubGroupTbls.Any(z => z.userID == user.ID) && user.ID != -1)) || (p.publicationSubGroupTbls.Count() == 0 && user.ID == -1))
                        &&
                        (p.title.Contains(keyword) || p.publicationContentTbls.Any(v => v.contentType == 0 && v.value.Contains(keyword)))
                        &&
                        (p.publishDate.Date <= now)
                        && p.isBlock == false
                        );
                }
                publication = publication.Where(predicate);
            }
            else
            {
                publication = publication.Where(p => ((p.publicationSubGroupTbls.Any(c => c.SubGroupTbl.userSubGroupTbls.Any(z => z.userID == user.ID) && user.ID != -1)) || (p.publicationSubGroupTbls.Count() == 0 && user.ID == -1))
                        &&
                        (p.publishDate.Date <= now)
                        && p.isBlock == false
                        );
            }
            if (groupID != -1 && user.ID != -1)
            {
                if (db.GroupTbls.Any(c => c.ID == groupID) == false)
                {
                    Result.result.code = 4;
                    Result.result.message = ClassCollection.Message.GROUP_NOT_EXIST;
                    Result.publications = null;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                publication = publication.Where(c => c.publicationSubGroupTbls.Any(p => p.SubGroupTbl.groupID == groupID));
            }
            if (subGroupID != -1 && user.ID != -1)
            {
                if (db.SubGroupTbls.Any(c => c.ID == subGroupID) == false)
                {
                    Result.result.code = 5;
                    Result.result.message = ClassCollection.Message.SUBGROUP_NOT_EXIST;
                    Result.publications = null;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                publication = publication.Where(x => x.publicationSubGroupTbls.Any(c => c.subGroupID == subGroupID));
            }

            #endregion

            publication = publication.Where(c => c.publicationContentTbls.Any()).OrderByDescending(c => c.publishDate);
            publication = publication.Skip(skipCount).Take(count);
            Result.publications = new List<ClassCollection.Model.LatestPublicationsForList>();
            foreach (var item in publication)
            {
                var tmp = new ClassCollection.Model.LatestPublicationsForList();
                tmp.ID = item.ID;
                try
                {
                    tmp.image = item.publicationContentTbls.Where(c => c.contentType == 1).Take(1).Single().value;
                }
                catch
                {
                    tmp.image = "";
                }
                tmp.like = item.publicationLikeTbls.Where(c => c.isLike).Count();
                tmp.unlike = item.publicationLikeTbls.Where(c => !c.isLike).Count();
                tmp.viewCount = item.viewCount;
                tmp.publishDate = Persia.Calendar.ConvertToPersian(item.publishDate).ToString("n");
                tmp.title = item.title;
                tmp.tag = new List<Model.Tag>();
                foreach (var t in item.publicationTagTbls)
                {
                    tmp.tag.Add(new Model.Tag() { ID = t.tagTbl.ID, title = t.tagTbl.title });
                }
                try
                {
                    tmp.summary = item.publicationContentTbls.Where(c => c.contentType == 0).Take(1).Single().value;
                }
                catch
                {
                    tmp.summary = item.title;
                }

                Result.publications.Add(tmp);
            }
            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result));
            return;

        }


        /// <summary>
        /// این تابع وظیفه ارسال انتشارات برای کاربر مورد نظر را بر عهده دارد.
        /// </summary>
        /// <param name="key">کلمه عبور برنامه ای که درحال استفاده از متد جاری می باشد</param>
        /// <param name="publicationsID">کد انتشارات مورد نظر جهت نمایش جزییات آن
        ///</param>
        /// <returns>خروجی ساختار جی سون از کلاس زیر را دارد.
        /// <para><see cref="ClassCollection.Model.PublicationsResult" /></para>
        /// <remarks>
        /// <para>Code : -1 , Message : <see cref="ClassCollection.Message.OPERATION_NO_ACCESS"/>, Result.publication : <c>null</c></para>
        /// <para>Code : 0 , Message : <see cref="ClassCollection.Message.OPERATION_SUCCESS" />, Result.publication :  <see cref=" ClassCollection.Model.fullPublications"/> </para>
        /// <para>Code : 1 , Message : <see cref="ClassCollection.Message.PUBLICATIONS_NOT_EXIST" />, Result.publication : <c>null</c></para>
        /// </remarks>
        /// </returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getPub(string key, long publicationsID)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.publicationResult Result = new ClassCollection.Model.publicationResult();
            Result.result = new ClassCollection.Model.Result();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }
            
            var db = new DataAccessDataContext();

            if (db.publicationTbls.Any(c => c.ID == publicationsID && c.isBlock == false) == false)
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.PUBLICATIONS_NOT_EXIST;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            var publication = db.publicationTbls.Single(c => c.ID == publicationsID);

            Result.publication = new ClassCollection.Model.fullpublication();
            if (publication.mUserID != null)
                Result.publication.mUserID = publication.mUserID;
            else
                Result.publication.mUserID = -1;
            Result.publication.ID = publication.ID;
            Result.publication.like = publication.publicationLikeTbls.Count(c => c.isLike == true);
            Result.publication.unlike = publication.publicationLikeTbls.Count(c => c.isLike == false);
            Result.publication.publishDate = Persia.Calendar.ConvertToPersian(publication.publishDate).ToString("n");
            Result.publication.title = publication.title;
            Result.publication.viewCount = publication.viewCount + 1;
            Result.publication.tag = new List<Model.Tag>();

            publication.viewCount += 1;
            db.SubmitChanges();

            foreach (var item in publication.publicationTagTbls)
            {
                Result.publication.tag.Add(new Model.Tag() { ID = item.tagID, title = item.tagTbl.title });
            }

            Result.publication.contents = new List<ClassCollection.Model.publicationContent>();
            var contents = publication.publicationContentTbls.OrderBy(c => c.pri);

            foreach (var item in contents)
            {
                var tmp = new ClassCollection.Model.publicationContent();
                tmp.ID = item.ID;
                tmp.type = item.contentType;
                tmp.value = item.value;
                Result.publication.contents.Add(tmp);
            }

            Result.publication.groups = new List<ClassCollection.Model.publicationGroupSubGroup>();
            var subgroups = publication.publicationSubGroupTbls;
            foreach (var item in subgroups)
            {
                var tmp = new ClassCollection.Model.publicationGroupSubGroup();
                tmp.groupID = item.SubGroupTbl.groupID;
                tmp.groupTitle = item.SubGroupTbl.GroupTbl.title;
                tmp.ID = item.ID;
                tmp.publicationID = item.publicationsID;
                tmp.subGroupID = item.subGroupID;
                tmp.subGroupTitle = item.SubGroupTbl.title;
                Result.publication.groups.Add(tmp);

            }

            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result));
            return;

        }

        /// <summary>
        /// این تابع وظیفه ارسال انتشارات برای کاربر مورد نظر را بر عهده دارد.
        /// </summary>
        /// <param name="key">کلمه عبور برنامه ای که درحال استفاده از متد جاری می باشد</param>
        /// <param name="publicationsID">کد انتشارات مورد نظر جهت نمایش جزییات آن
        ///</param>
        /// <returns>خروجی ساختار جی سون از کلاس زیر را دارد.
        /// <para><see cref="ClassCollection.Model.SinglePublicationsResult" /></para>
        /// <remarks>
        /// <para>Code : -1 , Message : <see cref="ClassCollection.Message.OPERATION_NO_ACCESS"/>, Result.publication : <c>null</c></para>
        /// <para>Code : 0 , Message : <see cref="ClassCollection.Message.OPERATION_SUCCESS" />, Result.publication :  <see cref=" ClassCollection.Model.singleFullPublications"/> </para>
        /// <para>Code : 1 , Message : <see cref="ClassCollection.Message.PUBLICATIONS_NOT_EXIST" />, Result.publication : <c>null</c></para>
        /// </remarks>
        /// </returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getSinglePub(string key, long publicationsID)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.SinglePublicationsResult Result = new ClassCollection.Model.SinglePublicationsResult();
            Result.result = new ClassCollection.Model.Result();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            
            var db = new DataAccessDataContext();

            if (db.publicationTbls.Any(c => c.ID == publicationsID && c.isBlock == false) == false)
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.PUBLICATIONS_NOT_EXIST;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            var publication = db.publicationTbls.Single(c => c.ID == publicationsID);

            Result.singlepublications = new ClassCollection.Model.singleFullPublications();
            Result.singlepublications.userID = publication.userID;
            Result.singlepublications.ID = publication.ID;
            Result.singlepublications.like = publication.publicationLikeTbls.Count(c => c.isLike == true);
            Result.singlepublications.unlike = publication.publicationLikeTbls.Count(c => c.isLike == false);
            Result.singlepublications.publishDate = Persia.Calendar.ConvertToPersian(publication.publishDate).ToString("n");
            Result.singlepublications.title = publication.title;
            Result.singlepublications.viewCount = publication.viewCount + 1;

            publication.viewCount += 1;
            db.SubmitChanges();

            var contents = publication.publicationContentTbls.OrderBy(c => c.pri);


            try
            {
                Result.singlepublications.image = contents.First(c => c.contentType == 1).value;
            }
            catch
            {
                Result.singlepublications.image = "";
            }

            try
            {
                Result.singlepublications.video = contents.First(c => c.contentType == 2).value;
            }
            catch
            {
                Result.singlepublications.video = "";
            }
            try
            {
                Result.singlepublications.text = contents.First(c => c.contentType == 0).value;
            }
            catch
            {
                Result.singlepublications.text = "";
            }

            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result));
            return;

        }

        /// <summary>
        /// این تابع وظیفه ثبت لایک و آنلایک برای انتشارات توسط کاربر مورد نظر را بر عهده دارد.
        /// </summary>
        /// <param name="key">کلمه عبور برنامه ای که درحال استفاده از متد جاری می باشد</param>
        /// <param name="publicationsID">کد انتشارات مورد نظر جهت ثبت نظر برای آن
        ///</param>
        /// <param name="userID">کد کاربری که در حال ثبت نظر برای انتشارات می باشد
        /// <remarks>
        /// در صورتی که کاربر مهمان می باشد مقدار -1 را قرار دهید
        /// </remarks>
        ///</param>
        /// <param name="isLike">نوع نظر درحال ثبت کاربر برای خبر
        /// <remarks>
        /// <para>True : لایک برای انتشارات درج میشود</para>
        /// <para>False : آنلایک برای انتشارات درج میشود</para>
        /// </remarks>
        ///</param>
        /// <returns>خروجی ساختار جی سون از کلاس زیر را دارد.
        /// <para><see cref="ClassCollection.Model.Result" /></para>
        /// <remarks>
        /// <para>Code : -1 , Message : <see cref="ClassCollection.Message.OPERATION_NO_ACCESS"/></para>
        /// <para>Code : 0 , Message : <see cref="ClassCollection.Message.OPERATION_SUCCESS" /></para>
        /// <para>Code : 1 , Message : <see cref="ClassCollection.Message.PUBLICATIONS_NOT_EXIST" /></para>
        /// <para>Code : 2 , Message : <see cref="ClassCollection.Message.USER_NOT_EXIST" /></para>
        /// </remarks>
        /// </returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void likeUnlikePub(string key, long userID, long publicationsID, bool isLike)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.Result Result = new ClassCollection.Model.Result();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.code = -1;
                Result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            DateTime dt = new DateTime();
            dt = DateTime.Now;
            var db = new DataAccessDataContext();

            if (db.publicationTbls.Any(c => c.ID == publicationsID && c.isBlock == false) == false)
            {
                Result.code = 1;
                Result.message = ClassCollection.Message.PUBLICATIONS_NOT_EXIST;

                Context.Response.Write(js.Serialize(Result));
                return;
            }
            if (userID != -1)
            {
                if (db.mUserTbls.Any(c => c.ID == userID && c.isBlock == false) == false)
                {
                    Result.code = 2;
                    Result.message = ClassCollection.Message.USER_NOT_EXIST;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }

            }

            if (db.publicationLikeTbls.Any(c => c.userID == userID && c.publicationsID == publicationsID) == false)
            {
                var publicationslike = new publicationLikeTbl();
                publicationslike.isLike = isLike;
                publicationslike.publicationsID = publicationsID;
                publicationslike.userID = userID == -1 ? 3 : userID;
                publicationslike.regDate = dt;
                db.publicationLikeTbls.InsertOnSubmit(publicationslike);
                db.SubmitChanges();
            }

            Result.code = 0;
            Result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result));
            return;

        }

        /// <summary>
        /// این تابع وظیفه ارسال انتشارات دریاقت نشده برای کاربر مورد نظر را بر عهده دارد.
        /// </summary>
        /// <param name="key">کلمه عبور برنامه ای که درحال استفاده از متد جاری می باشد</param>
        /// <param name="mobileOrEmail">شماره موبایل یا ایمیل کاربر
        /// <remarks>
        ///  <para>در صورت قرار دادن شماره موبایل در این پارامتر طول آن باید برابر با 12 کاراکتر باشد</para>
        ///  <para>در صورت قرار دادن ایمیل در این پارامتر طول آن باید کمتر از 250 کاراکتر باید</para> 
        ///  <para>در صورتی که فقط نیاز به دریافت انتشارات عمومی می باشد مقدار این پارامتر را مقدار رشته خالی قرار دهید</para>
        /// </remarks>
        /// </param>
        /// <param name="lastPublicationsID" >کد آخرین انتشارات جهت ارسال انتشارات درج شده ی بعد از آن
        /// </param>
        /// <returns>خروجی ساختار جی سون از کلاس زیر را دارد.
        /// <para><see cref="ClassCollection.Model.LatestPublicationsForNotificationResult" /></para>
        /// <remarks>
        /// <para>Code : -1, Message : <see cref="ClassCollection.Message.OPERATION_NO_ACCESS"/>, Result.publication : <c>null</c></para>
        /// <para>Code : 0 , Message : <see cref="ClassCollection.Message.OPERATION_SUCCESS" />, Result.publication : List of <see cref=" ClassCollection.Model.LatestPublicationsForNotification"/></para>
        /// <para>Code : 1 , Message : <see cref="ClassCollection.Message.ERROR_INPUT_PARAMETER" />, Result.publication : <c>null</c></para>
        /// <para>Code : 2 , Message : <see cref="ClassCollection.Message.USER_NOT_EXIST" />, Result.publication : <c>null</c></para>
        /// <para>Code : 3 , Message : <see cref="ClassCollection.Message.USER_BLOCKED" />, Result.publication : <c>null</c></para>
        /// </remarks>
        /// </returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getLatestPubForNotification(string key, string mobileOrEmail, long lastPublicationsID)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.LatestPublicationsForNotificationResult Result = new ClassCollection.Model.LatestPublicationsForNotificationResult();
            Result.result = new ClassCollection.Model.Result();

            DateTime dt = new DateTime();
            dt = DateTime.Now.Date;

            var db = new DataAccessDataContext();
            var user = new mUserTbl();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            mobileOrEmail = mobileOrEmail.Trim();

            if (mobileOrEmail == "")
            {
                mobileOrEmail = "-1";
                user.ID = -1;
            }
            else
            {

                if (mobileOrEmail.Contains("@") && mobileOrEmail.Length >= 250)
                {
                    Result.result.code = 1;
                    Result.result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                if (!mobileOrEmail.Contains("@") && mobileOrEmail.Length != 12)
                {
                    Result.result.code = 1;
                    Result.result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }

                if (!db.mUserTbls.Any(c => c.emailtell == mobileOrEmail))
                {
                    Result.result.code = 2;
                    Result.result.message = ClassCollection.Message.USER_NOT_EXIST;
                    Result.publications = null;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                else
                {
                    user = db.mUserTbls.Single(c => c.emailtell == mobileOrEmail);
                    if (user.isBlock)
                    {
                        Result.result.code = 3;
                        Result.result.message = ClassCollection.Message.USER_BLOCKED;
                        Result.publications = null;
                        Context.Response.Write(js.Serialize(Result));
                        return;
                    }
                }

            }

            IQueryable<publicationTbl> publication = db.publicationTbls;

            publication = publication.Where(p => (((p.publicationSubGroupTbls.Any(c => c.SubGroupTbl.userSubGroupTbls.Any(z => z.userID == user.ID) && user.ID != -1)) || (p.publicationSubGroupTbls.Count() == 0 && user.ID == -1)) && p.ID > lastPublicationsID)
                    &&
                    (p.publishDate.Date <= dt)
                    && p.isBlock == false
                    );

            publication = publication.OrderByDescending(c => c.publishDate);

            Result.publications = new List<ClassCollection.Model.LatestPublicationsForNotification>();
            foreach (var item in publication)
            {
                var tmp = new ClassCollection.Model.LatestPublicationsForNotification();
                tmp.ID = item.ID;
                tmp.title = item.title;

                Result.publications.Add(tmp);
            }
            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result));
            return;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void sendPubComment(string key, long pubID, long userID, string mobileOrEmail, string fullName, string text)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.Result Result = new ClassCollection.Model.Result();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.code = -1;
                Result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            fullName = fullName.TrimEnd().TrimStart();
            text = text.TrimStart().TrimEnd();

            if (fullName.Length == 0)
            {
                Result.code = 1;
                Result.message = ClassCollection.Message.NAME_INCORRECT;

                Context.Response.Write(js.Serialize(Result));
                return;
            }
            if (text.Length == 0 || text.Length > 250)
            {
                Result.code = 2;
                Result.message = ClassCollection.Message.COMMENT_TEXT_INCORRECT;

                Context.Response.Write(js.Serialize(Result));
                return;
            }
            //if (Methods.isEmail(mobileOrEmail) == false)
            //{
            //    if (mobileOrEmail.Length != 11)
            //    {
            //        Result.code = 3;
            //        Result.message = ClassCollection.Message.MOBILE_INCORRECT;

            //        Context.Response.Write(js.Serialize(Result));
            //        return;
            //    }
            //}

            DateTime dt = new DateTime();
            dt = DateTime.Now;
            var db = new DataAccessDataContext();

            if (db.publicationTbls.Any(c => c.ID == pubID && c.isBlock == false) == false)
            {
                Result.code = 4;
                Result.message = ClassCollection.Message.IO_NOT_EXIST;

                Context.Response.Write(js.Serialize(Result));
                return;
            }
            if (db.mUserTbls.Any(c => c.ID == userID) == false && userID != -1)
            {
                Result.code = 5;
                Result.message = ClassCollection.Message.USER_NOT_EXIST;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            var publicationComment = new publicationCommentTbl();
            publicationComment.fullName = fullName;
            publicationComment.text = text;
            publicationComment.regDate = dt;
            publicationComment.publicationID = pubID;
            publicationComment.mUserID = userID;
            publicationComment.mobile = Methods.isEmail(mobileOrEmail) ? null : mobileOrEmail;
            publicationComment.email = Methods.isEmail(mobileOrEmail) ? mobileOrEmail : null;
            publicationComment.isBlock = true;
            db.publicationCommentTbls.InsertOnSubmit(publicationComment);
            db.SubmitChanges();


            Result.code = 0;
            Result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result));
            return;

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getPublicationCommentList(string key, long pubID, int count, int pageIndex)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.CommentListResult Result = new ClassCollection.Model.CommentListResult();
            Result.result = new ClassCollection.Model.Result();


            var skipCount = count * (pageIndex - 1);
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }
            if (db.publicationTbls.Any(c => c.ID == pubID) == false)
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.PUBLICATIONS_NOT_EXIST;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            Result.comment = new List<Model.Comment>();
            var comments = db.publicationCommentTbls.Where(c => c.isBlock == false && c.publicationID == pubID).OrderByDescending(c => c.regDate);
            Result.comment = comments.Skip(skipCount).Take(count).Select(c => new Model.Comment() { fullName = c.fullName, ID = c.ID, text = c.text }).ToList<Model.Comment>();


            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result));
            return;

        }
        #endregion

        #region partner

        /// <summary>
        /// این تابع وظیفه ارسال لیست همکاران برای کاربر مورد نظر را بر عهده دارد.
        /// </summary>
        /// <param name="key">کلمه عبور برنامه ای که درحال استفاده از متد جاری می باشد</param>
        /// <param name="mobileOrEmail">شماره موبایل یا ایمیل کاربر
        /// <remarks>
        ///  <para>در صورت قرار دادن شماره موبایل در این پارامتر طول آن باید برابر با 12 کاراکتر باشد</para>
        ///  <para>در صورت قرار دادن ایمیل در این پارامتر طول آن باید کمتر از 250 کاراکتر باید</para> 
        /// </remarks>
        /// </param>
        /// <param name="fillterString" >رشته ی مورد نظر کاربر جهت جستجو در بین اخبار
        /// <remarks>
        ///- اجزای جستجو با فاصله از یکدیگر جدا میشوند
        ///- درصورتی که کلمه مورد نظر در بین دو علامت دابل کوتیشن قرار بگیرد مشخصات یافت شده حتما شامل آن کلمه خواهد بود
        /// </remarks>
        /// </param>
        /// <param name="count">تعداد نتایج درخواستی در هر صفحه</param>
        /// <param name="pageIndex">شماره صفحه مورد نظر از جستجو</param>
        /// 
        /// <returns>خروجی ساختار جی سون از کلاس زیر را دارد.
        /// <para><see cref="ClassCollection.Model.LatestNewsForListResult" /></para>
        /// <remarks>
        /// <para>Code : -1 , Message : <see cref="ClassCollection.Message.OPERATION_NO_ACCESS"/>, Result.partner : <c>null</c></para>
        /// <para>Code : 0 , Message : <see cref="ClassCollection.Message.OPERATION_SUCCESS" />, Result.partner : List of <see cref=" ClassCollection.Model.PartnerListResult"/></para>
        /// <para>Code : 1 , Message : <see cref="ClassCollection.Message.ERROR_INPUT_PARAMETER" />, Result.partner : <c>null</c></para>
        /// <para>Code : 2 , Message : <see cref="ClassCollection.Message.USER_NOT_EXIST" />, Result.partner : <c>null</c></para>
        /// <para>Code : 3 , Message : <see cref="ClassCollection.Message.USER_BLOCKED" />, Result.partner : <c>null</c></para>
        /// </remarks>
        /// </returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getPartnerForList(string key, string mobileOrEmail, string fillterString, int count, int pageIndex)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.PartnerListResult Result = new ClassCollection.Model.PartnerListResult();
            Result.result = new ClassCollection.Model.Result();

            var skipCount = count * (pageIndex - 1);
            var db = new DataAccessDataContext();
            var user = new mUserTbl();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            mobileOrEmail = mobileOrEmail.Trim();

            if (mobileOrEmail == "")
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.USER_NOT_EXIST;
                Result.partner = null;
                Context.Response.Write(js.Serialize(Result));
                return;
            }
            else
            {
                if (mobileOrEmail.Length != 12)
                {
                    Result.result.code = 2;
                    Result.result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }

                if (!db.mUserTbls.Any(c => c.emailtell == mobileOrEmail))
                {
                    Result.result.code = 3;
                    Result.result.message = ClassCollection.Message.USER_NOT_EXIST;
                    Result.partner = null;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                else
                {
                    user = db.mUserTbls.Single(c => c.emailtell == mobileOrEmail);
                    if (user.isBlock)
                    {
                        Result.result.code = 4;
                        Result.result.message = ClassCollection.Message.USER_BLOCKED;
                        Result.partner = null;
                        Context.Response.Write(js.Serialize(Result));
                        return;
                    }
                }

            }

            IQueryable<partnersTbl> partners = db.partnersTbls;

            #region search
            if (fillterString != "")
            {
                var search = new Search(fillterString);
                var ands = search.getAND;
                var ors = search.getOR;

                var predicate = PredicateBuilder.False<partnersTbl>();
                foreach (string keyword in ors)
                {
                    string temp = keyword;
                    predicate = predicate.Or(p => p.name.Contains(keyword) || p.email.Contains(keyword) || p.family.Contains(key) || p.innerTell.Contains(keyword) || p.level.Contains(keyword) || p.registrationmobile.Contains(keyword) || p.optionalmobile.Contains(keyword));

                }
                if (ors.Count > 0)
                    partners = partners.Where(predicate);

                foreach (var item in ands)
                {
                    var keyword = item.Replace("\"", "");
                    partners = partners.Where(p => p.name.Contains(keyword) || p.email.Contains(keyword) || p.family.Contains(key) || p.innerTell.Contains(keyword) || p.level.Contains(keyword) || p.registrationmobile.Contains(keyword) || p.optionalmobile.Contains(keyword));
                }

            }

            #endregion

            partners = partners.OrderByDescending(c => c.ID);
            partners = partners.Skip(skipCount).Take(count);
            Result.partner = new List<Excel.Import.PartnerLoader.Partner>();
            foreach (var item in partners)
            {
                var tmp = new Excel.Import.PartnerLoader.Partner();
                tmp.email = item.email;
                tmp.family = item.family;
                tmp.ID = item.ID;
                tmp.innerTell = item.innerTell;
                tmp.level = item.level;
                tmp.name = item.name;
                tmp.optionalmobile = item.optionalmobile;
                tmp.registrationmobile = item.registrationmobile;

                Result.partner.Add(tmp);
            }
            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result));
            return;

        }

        #endregion

        #region poll
        /// <summary>
        /// بررسی میکند آیا یوزر در نظرسنجی شرکت کرده یا خیر
        /// </summary>
        /// <param name="key"></param>
        /// <param name="userID"></param>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void CheckUserPoll(string key, long userID, long pollID)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.BoolResult Result = new ClassCollection.Model.BoolResult();
            Result.result = new Model.Result();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }
            
            var db = new DataAccessDataContext();


            if (db.mUserTbls.Any(c => c.ID == userID) == false)
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.USER_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (db.mUserTbls.Single(c => c.ID == userID).isBlock == true)
            {
                Result.result.code = 2;
                Result.result.message = ClassCollection.Message.USER_BLOCKED;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            Result.value = db.mUserOptionTbls.Any(c => c.optionTbl.pollID == pollID && c.mUserID == userID);

            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getPoll(string key, long userID, long pollID)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.PollResult Result = new ClassCollection.Model.PollResult();
            Result.result = new Model.Result();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            DateTime dt = new DateTime();
            dt = DateTime.Now.Date;
            var db = new DataAccessDataContext();

            if (db.mUserTbls.Any(c => c.ID == userID && userID != -1) == false)
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.USER_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }
            if (db.pollTbls.Any(c => c.ID == pollID) == false)
            {
                Result.result.code = 2;
                Result.result.message = ClassCollection.Message.POLL_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            var poll = db.pollTbls.Single(c => c.ID == pollID);
            if (poll.isBlock == true)
            {
                Result.result.code = 3;
                Result.result.message = ClassCollection.Message.POLL_BLOCKED;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }
            bool pollFinished = poll.finishedDate < dt;
            bool isUserPoll = db.mUserOptionTbls.Any(c => c.optionTbl.pollID == pollID && c.mUserID == userID && userID != -1);
            //if (poll.finishedDate<dt == true)
            //{
            //    Result.result.code = 4;
            //    Result.result.message = ClassCollection.Message.POLL_FINISHED;
            //    Context.Response.Write(js.Serialize(Result).ToLower());
            //    return;
            //}

            Result.poll = new Model.Poll();
            Result.poll.option = new List<Model.OptionReport>();
            Result.poll.content = new List<Model.PollContent>();
            Result.poll.isPublic = !poll.pollSubGroupTbls.Any();
            Result.poll.ID = poll.ID;
            Result.poll.viewCount = poll.viewCount + 1;
            Result.poll.finishedDate = pollFinished ? ClassCollection.Methods.toFriendlyDate((DateTime)poll.finishedDate) : "";
            Result.poll.startDate = ClassCollection.Methods.toFriendlyDate(poll.startDate);
            poll.viewCount++;
            db.SubmitChanges();
            
            foreach (var item in poll.optionTbls)
            {
                var tmp = new ClassCollection.Model.OptionReport();
                tmp.text = item.text;
                tmp.ID = item.ID;
                if (pollFinished || isUserPoll)
                {
                    tmp.count = item.mUserOptionTbls.Count;
                    Result.poll.voteCount += tmp.count;
                }
                Result.poll.option.Add(tmp);
            }

            foreach (var item in poll.pollContentTbls)
            {
                var tmp = new ClassCollection.Model.PollContent();
                tmp.ID = item.ID;
                tmp.type = item.contentType;
                tmp.value = item.value;
                Result.poll.content.Add(tmp);
            }

            if(poll.pollContentTbls.Any(c=>c.contentType==0))
            {
                Result.poll.summery = poll.pollContentTbls.Where(c => c.contentType == 0).Take(1).Single().value;
                
            }

            if (poll.pollContentTbls.Any(c => c.contentType ==1))
            {
                Result.poll.image = poll.pollContentTbls.Where(c => c.contentType ==1).Take(1).Single().value;

            }

            Result.poll.groups = new List<ClassCollection.Model.pollGroupSubGroup>();
            var subgroups = poll.pollSubGroupTbls;
            foreach (var item in subgroups)
            {
                var tmp = new ClassCollection.Model.pollGroupSubGroup();
                tmp.groupID = item.SubGroupTbl.groupID;
                tmp.groupTitle = item.SubGroupTbl.GroupTbl.title;
                tmp.ID = item.ID;
                tmp.pollID = item.pollID;
                tmp.subGroupID = item.subGroupID;
                tmp.subGroupTitle = item.SubGroupTbl.title;
                Result.poll.groups.Add(tmp);

            }

            Result.poll.tag = new List<Model.Tag>();
            foreach (var t in poll.pollTagTbls)
            {
                Result.poll.tag.Add(new Model.Tag() { ID = t.tagTbl.ID, title = t.tagTbl.title });
            }

            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getPollList(string key, long userID, string fillterString, long tagID, long groupID, long subGroupID, int pageIndex, int count)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.PollListResult Result = new ClassCollection.Model.PollListResult();
            Result.result = new Model.Result();
            var skipCount = count * (pageIndex - 1);

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            DateTime dt = new DateTime();
            dt = DateTime.Now.Date;
            var db = new DataAccessDataContext();
            var user = new mUserTbl();

            if (db.mUserTbls.Any(c => c.ID == userID ) == false && userID != -1)
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.USER_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (userID == -1)
            {
                user.ID = -1;

            }
            else
            {
                user = db.mUserTbls.Single(c => c.ID == userID);
            }
            Result.poll = new List<Model.Poll>();

            IQueryable<pollTbl> poll = db.pollTbls;

            if (tagID != -1 && db.tagTbls.Any(c => c.ID == tagID))
            {
                poll = poll.Where(c => c.pollTagTbls.Any(p => p.tagID == tagID));
            }

            #region search
            if (fillterString != "")
            {
                var search = new Search(fillterString);
                var ands = search.getAND;
                var ors = search.getOR;


                var predicate = PredicateBuilder.False<pollTbl>();
                foreach (string keyword in ors)
                {
                    string temp = keyword;
                    predicate = predicate.Or(p => ((p.pollSubGroupTbls.Any(c => c.SubGroupTbl.userSubGroupTbls.Any(z => z.userID == user.ID) && user.ID != -1)) || (p.pollSubGroupTbls.Count() == 0 || user.ID == -1))
                        &&
                        p.pollContentTbls.Any(v => v.contentType == 0 && v.value.Contains(temp)) || p.pollTagTbls.Any(x => x.tagTbl.title.Contains(keyword))
                        && p.isBlock == false
                        );
                }

                poll = poll.Where(predicate);
            }
            else
            {
                poll = poll.Where(p => ((p.pollSubGroupTbls.Any(c => c.SubGroupTbl.userSubGroupTbls.Any(z => z.userID == user.ID) && user.ID != -1)) || (p.pollSubGroupTbls.Count() == 0 || user.ID == -1))
                        && p.isBlock == false
                        );
            }
            if (groupID != -1 && user.ID != -1)
            {
                if (db.GroupTbls.Any(c => c.ID == groupID) == false)
                {
                    Result.result.code = 5;
                    Result.result.message = ClassCollection.Message.GROUP_NOT_EXIST;
                    Result.poll = null;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                poll = poll.Where(c => c.pollSubGroupTbls.Any(p => p.SubGroupTbl.groupID == groupID));
            }
            if (subGroupID != -1 && user.ID != -1)
            {
                if (db.SubGroupTbls.Any(c => c.ID == subGroupID) == false)
                {
                    Result.result.code = 6;
                    Result.result.message = ClassCollection.Message.SUBGROUP_NOT_EXIST;
                    Result.poll = null;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                poll = poll.Where(x => x.pollSubGroupTbls.Any(c => c.subGroupID == subGroupID));
            }

            #endregion

            poll = poll.Where(c => c.pollContentTbls.Any()).OrderByDescending(c => c.startDate);
            poll = poll.Skip(skipCount).Take(count);


            //var polls = db.pollTbls.Where(c => c.isBlock == false && (c.isFinished == false ) && c.optionTbls.Any()).OrderByDescending(c => c.regDate).Skip(skipCount).Take(count);
            foreach (var item in poll)
            {
                var tmp = new Model.Poll();
                tmp.option = new List<Model.OptionReport>();
                tmp.ID = item.ID;


                var otext = item.pollContentTbls.Where(c => c.contentType == 0).Take(1).Single().value;
                var iimage = "";

                if (item.pollContentTbls.Any(c => c.contentType == 1))
                {
                    iimage = item.pollContentTbls.Where(c => c.contentType == 1).Take(1).Single().value;
                }
                tmp.image = iimage;
                tmp.summery = otext.Substring(0, Math.Min(47, otext.Length)) + "...";
                tmp.viewCount = item.viewCount;
                tmp.finishedDate = item.finishedDate < dt ? ClassCollection.Methods.toFriendlyDate((DateTime)item.finishedDate) : "";
                tmp.startDate = ClassCollection.Methods.toFriendlyDate(item.startDate);
                tmp.voteCount = item.optionTbls.Sum(c => c.mUserOptionTbls.Count);
                tmp.tag = new List<Model.Tag>();
                foreach (var ta in item.pollTagTbls)
                {
                    var t = new Model.Tag();
                    t.ID = ta.tagID;
                    t.title = ta.tagTbl.title;

                    tmp.tag.Add(t);
                }
                Result.poll.Add(tmp);
            }

            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getPollResult(string key, long userID, long pollID)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.PollReportResult Result = new ClassCollection.Model.PollReportResult();
            Result.result = new Model.Result();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            DateTime dt = new DateTime();
            dt = DateTime.Now.Date;
            var db = new DataAccessDataContext();

            if (db.mUserTbls.Any(c => c.ID == userID) == false)
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.USER_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }
            if (db.pollTbls.Any(c => c.ID == pollID) == false)
            {
                Result.result.code = 2;
                Result.result.message = ClassCollection.Message.POLL_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            var poll = db.pollTbls.Single(c => c.ID == pollID);
            if (poll.finishedDate>=dt)
            {
                Result.result.code = 3;
                Result.result.message = ClassCollection.Message.POLL_IS_RUNNING;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            Result.pollReport = new Model.PollReport();
            Result.pollReport.ID = poll.ID;
            Result.pollReport.Total = 0;
            Result.pollReport.option = new List<Model.OptionReport>();
            foreach (var item in poll.optionTbls)
            {
                var option = new ClassCollection.Model.OptionReport();
                option.count = item.mUserOptionTbls.Count;
                option.ID = item.ID;
                option.text = item.text;

                Result.pollReport.option.Add(option);
                Result.pollReport.Total += item.mUserOptionTbls.Count;
            }

            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void polling(string key, long userID, long optionID)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.PollReportResult Result = new ClassCollection.Model.PollReportResult();
            Result.result = new Model.Result();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            DateTime dt = new DateTime();
            dt = DateTime.Now;
            var db = new DataAccessDataContext();

            if (db.mUserTbls.Any(c => c.ID == userID) == false)
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.USER_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }
            if (db.optionTbls.Any(c => c.ID == optionID) == false)
            {
                Result.result.code = 2;
                Result.result.message = ClassCollection.Message.OPTION_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }
            if (db.mUserOptionTbls.Any(c => c.optionTbl.pollTbl.optionTbls.Any(p => p.ID == optionID) && c.mUserID == userID) == true)
            {
                Result.result.code = 3;
                Result.result.message = ClassCollection.Message.POLL_ALLREADY_DONE;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }
            if (db.optionTbls.Single(c => c.ID == optionID).pollTbl.isBlock == true)
            {
                Result.result.code = 4;
                Result.result.message = ClassCollection.Message.POLL_BLOCKED;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }
            if (db.optionTbls.Single(c => c.ID == optionID).pollTbl.finishedDate<dt.Date)
            {
                Result.result.code = 5;
                Result.result.message = ClassCollection.Message.POLL_FINISHED;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }
            var poll = db.optionTbls.Single(c => c.ID == optionID).pollTbl;

            var rec = new mUserOptionTbl();
            rec.optionID = optionID;
            rec.regDate = dt;
            rec.mUserID = userID;

            db.mUserOptionTbls.InsertOnSubmit(rec);
            db.SubmitChanges();


            Result.pollReport = new Model.PollReport();
            Result.pollReport.ID = poll.ID;
            Result.pollReport.Total = 0;
            Result.pollReport.option = new List<Model.OptionReport>();
            foreach (var item in poll.optionTbls)
            {
                var option = new ClassCollection.Model.OptionReport();
                option.count = item.mUserOptionTbls.Count;
                option.ID = item.ID;
                option.text = item.text;

                Result.pollReport.option.Add(option);
                Result.pollReport.Total += option.count;
            }

            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;

        }

        #endregion

        #region ican
        /// <summary>
        /// این تابع وظیفه ارسال توانایی برای کاربر مورد نظر را بر عهده دارد.
        /// </summary>
        /// <param name="key">کلمه عبور برنامه ای که درحال استفاده از متد جاری می باشد</param>
        /// <param name="mobileOrEmail">شماره موبایل یا ایمیل کاربر
        /// <remarks>
        ///  <para>در صورت قرار دادن شماره موبایل در این پارامتر طول آن باید برابر با 12 کاراکتر باشد</para>
        ///  <para>در صورت قرار دادن ایمیل در این پارامتر طول آن باید کمتر از 250 کاراکتر باید</para> 
        ///  <para>در صورتی که فقط نیاز به دریافت اخبار عمومی می باشد مقدار این پارامتر را مقدار رشته خالی قرار دهید</para>
        /// </remarks>
        /// </param>
        /// <param name="fillterString" >رشته ی مورد نظر کاربر جهت جستجو در بین توانایی ها
        /// <remarks>
        ///- اجزای جستجو با فاصله از یکدیگر جدا میشوند
        ///- درصورتی که کلمه مورد نظر در بین دو علامت دابل کوتیشن قرار بگیرد اخبار یافت شده حتما شامل آن کلمه خواهد بود
        /// </remarks>
        /// </param>
        /// <param name="count">تعداد نتایج درخواستی در هر صفحه</param>
        /// <param name="pageIndex">شماره صفحه مورد نظر از جستجو</param>
        /// 
        /// <returns>خروجی ساختار جی سون از کلاس زیر را دارد.
        /// <para><see cref="ClassCollection.Model.LatestNewsForListResult" /></para>
        /// <remarks>
        /// <para>Code : -1 , Message : <see cref="ClassCollection.Message.OPERATION_NO_ACCESS"/>, Result.ican : <c>null</c></para>
        /// <para>Code : 0 , Message : <see cref="ClassCollection.Message.OPERATION_SUCCESS" />, Result.ican : List of <see cref=" ClassCollection.Model.LatestIcanForListResult"/></para>
        /// <para>Code : 1 , Message : <see cref="ClassCollection.Message.ERROR_INPUT_PARAMETER" />, Result.ican : <c>null</c></para>
        /// <para>Code : 2 , Message : <see cref="ClassCollection.Message.USER_NOT_EXIST" />, Result.ican : <c>null</c></para>
        /// <para>Code : 3 , Message : <see cref="ClassCollection.Message.USER_BLOCKED" />, Result.ican : <c>null</c></para>
        /// </remarks>
        /// </returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getLatestIcanForList(string key, string mobileOrEmail, string fillterString, int count, int pageIndex)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.LatestIcanForListResult Result = new ClassCollection.Model.LatestIcanForListResult();
            Result.result = new ClassCollection.Model.Result();

            var skipCount = count * (pageIndex - 1);
            var db = new DataAccessDataContext();
            var user = new mUserTbl();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            mobileOrEmail = mobileOrEmail.Trim();

            if (mobileOrEmail == "")
            {
                mobileOrEmail = "-1";
                user.ID = -1;
            }
            else
            {

                if (mobileOrEmail.Length != 12)
                {
                    Result.result.code = 2;
                    Result.result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }

                if (!db.mUserTbls.Any(c => c.emailtell == mobileOrEmail))
                {
                    Result.result.code = 3;
                    Result.result.message = ClassCollection.Message.USER_NOT_EXIST;
                    Result.ican = null;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                else
                {
                    user = db.mUserTbls.Single(c => c.emailtell == mobileOrEmail);
                    if (user.isBlock)
                    {
                        Result.result.code = 4;
                        Result.result.message = ClassCollection.Message.USER_BLOCKED;
                        Result.ican = null;
                        Context.Response.Write(js.Serialize(Result));
                        return;
                    }
                }

            }

            IQueryable<icanTbl> ican = db.icanTbls;

            #region search
            if (fillterString != "")
            {
                var search = new Search(fillterString);
                var ands = search.getAND;
                var ors = search.getOR;


                var predicate = PredicateBuilder.False<icanTbl>();
                foreach (string keyword in ors)
                {
                    string temp = keyword;
                    predicate = predicate.Or(p => (p.title.Contains(temp) || p.icanContentTbls.Any(v => v.contentType == 0 && v.value.Contains(temp)))
                        && p.isBlock == false
                        );
                }


                foreach (var keyword in ands)
                {
                    ican = ican.Where(p => (p.title.Contains(keyword) || p.icanContentTbls.Any(v => v.contentType == 0 && v.value.Contains(keyword)))
                        && p.isBlock == false
                        );
                }
                ican = ican.Where(predicate);
            }
            else
            {
                ican = ican.Where(p => p.isBlock == false
                        );
            }


            #endregion

            ican = ican.Where(c => c.icanContentTbls.Any()).Skip(skipCount).Take(count);
            Result.ican = new List<ClassCollection.Model.LatestIcanForList>();
            foreach (var item in ican)
            {
                var tmp = new ClassCollection.Model.LatestIcanForList();
                tmp.ID = item.ID;
                try
                {
                    tmp.image = item.icanContentTbls.Where(c => c.contentType == 1).Take(1).Single().value;
                }
                catch
                {
                    tmp.image = "";
                }
                tmp.viewCount = item.viewCount;
                tmp.haveattachment = item.haveAttachment;
                tmp.userNameFamily = item.mUserTbl.name + " " + item.mUserTbl.family;

                tmp.title = item.title;
                try
                {
                    tmp.summary = item.icanContentTbls.Where(c => c.contentType == 0).Take(1).Single().value;
                }
                catch
                {
                    tmp.summary = item.title;
                }

                Result.ican.Add(tmp);
            }
            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result));
            return;

        }

        /// <summary>
        /// این تابع وظیفه ارسال توانایی برای کاربر مورد نظر را بر عهده دارد.
        /// </summary>
        /// <param name="key">کلمه عبور برنامه ای که درحال استفاده از متد جاری می باشد</param>
        /// <param name="icanID">کد توانایی مورد نظر جهت نمایش جزییات آن
        ///</param>
        /// <returns>خروجی ساختار جی سون از کلاس زیر را دارد.
        /// <para><see cref="ClassCollection.Model.NewsResult" /></para>
        /// <remarks>
        /// <para>Code : -1 , Message : <see cref="ClassCollection.Message.OPERATION_NO_ACCESS"/>, Result.ican : <c>null</c></para>
        /// <para>Code : 0 , Message : <see cref="ClassCollection.Message.OPERATION_SUCCESS" />, Result.ican : List of <see cref=" ClassCollection.Model.IcanResult"/> </para>
        /// <para>Code : 1 , Message : <see cref="ClassCollection.Message.ICAN_NOT_EXIST" />, Result.ican : <c>null</c></para>
        /// </remarks>
        /// </returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getIcan(string key, long icanID)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.IcanResult Result = new ClassCollection.Model.IcanResult();
            Result.result = new ClassCollection.Model.Result();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            
            var db = new DataAccessDataContext();

            if (db.icanTbls.Any(c => c.ID == icanID && c.isBlock == false) == false)
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.ICAN_NOT_EXIST;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            var ican = db.icanTbls.Single(c => c.ID == icanID);

            Result.ican = new ClassCollection.Model.fullIcan();
            Result.ican.creatorID = ican.creatorID;
            Result.ican.ID = ican.ID;
            Result.ican.title = ican.title;
            Result.ican.haveattachment = ican.haveAttachment;
            Result.ican.viewCount = ican.viewCount + 1;

            ican.viewCount += 1;
            db.SubmitChanges();

            Result.ican.contents = new List<ClassCollection.Model.icanContent>();
            var contents = ican.icanContentTbls.OrderBy(c => c.pri);

            foreach (var item in contents)
            {
                var tmp = new ClassCollection.Model.icanContent();
                tmp.ID = item.ID;
                tmp.type = item.contentType;
                tmp.value = item.value;
                if (item.contentType == 3)
                {
                    tmp.fileType = item.fileType;
                    tmp.fileSize = item.fileSize;
                    tmp.downloadCount = item.downloadCount == null ? 0 : (int)item.downloadCount;
                }
                Result.ican.contents.Add(tmp);
            }

            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Context.Response.Write(js.Serialize(Result));
            return;
        }

        /// <summary>
        /// این تابع وظیفه ارسال توانایی برای کاربر مورد نظر را بر عهده دارد.
        /// </summary>
        /// <param name="key">کلمه عبور برنامه ای که درحال استفاده از متد جاری می باشد</param>
        /// <param name="icanID">کد خبر مورد نظر جهت نمایش جزییات آن
        ///</param>
        /// <returns>خروجی ساختار جی سون از کلاس زیر را دارد.
        /// <para><see cref="ClassCollection.Model.SingleIcanResult" /></para>
        /// <remarks>
        /// <para>Code : -1 , Message : <see cref="ClassCollection.Message.OPERATION_NO_ACCESS"/>, Result.ican : <c>null</c></para>
        /// <para>Code : 0 , Message : <see cref="ClassCollection.Message.OPERATION_SUCCESS" />, Result.ican : <see cref=" ClassCollection.Model.singleFullIcan"/> </para>
        /// <para>Code : 1 , Message : <see cref="ClassCollection.Message.ICAN_NOT_EXIST" />, Result.ican : <c>null</c></para>
        /// </remarks>
        /// </returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getSingleIcan(string key, long icanID)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.SingleIcanResult Result = new ClassCollection.Model.SingleIcanResult();
            Result.result = new ClassCollection.Model.Result();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            
            var db = new DataAccessDataContext();

            if (db.icanTbls.Any(c => c.ID == icanID && c.isBlock == false) == false)
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.ICAN_NOT_EXIST;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            var ican = db.icanTbls.Single(c => c.ID == icanID);

            Result.singleican = new ClassCollection.Model.singleFullIcan();
            Result.singleican.creatorID = ican.creatorID;
            Result.singleican.ID = ican.ID;
            Result.singleican.title = ican.title;
            Result.singleican.viewCount = ican.viewCount + 1;

            ican.viewCount += 1;
            db.SubmitChanges();

            var contents = ican.icanContentTbls.OrderBy(c => c.pri);

            try
            {
                Result.singleican.image = contents.First(c => c.contentType == 1).value;
            }
            catch
            {
                Result.singleican.image = "";
            }

            try
            {
                Result.singleican.video = contents.First(c => c.contentType == 2).value;
            }
            catch
            {
                Result.singleican.video = "";
            }
            try
            {
                Result.singleican.text = contents.First(c => c.contentType == 0).value;
            }
            catch
            {
                Result.singleican.text = "";
            }


            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result));
            return;

        }

        /// <summary>
        /// این تابع وظیفه ارسال توانایی دریاقت نشده برای کاربر مورد نظر را بر عهده دارد.
        /// </summary>
        /// <param name="key">کلمه عبور برنامه ای که درحال استفاده از متد جاری می باشد</param>
        /// <param name="mobileOrEmail">شماره موبایل یا ایمیل کاربر
        /// <remarks>
        ///  <para>در صورت قرار دادن شماره موبایل در این پارامتر طول آن باید برابر با 12 کاراکتر باشد</para>
        ///  <para>در صورت قرار دادن ایمیل در این پارامتر طول آن باید کمتر از 250 کاراکتر باید</para> 
        ///  <para>در صورتی که فقط نیاز به دریافت توانایی ها عمومی می باشد مقدار این پارامتر را مقدار رشته خالی قرار دهید</para>
        /// </remarks>
        /// </param>
        /// <param name="lastIcanID" >کد آخرین توانایی جهت ارسال توانایی های درج شده ی بعد از آن
        /// </param>
        /// <returns>خروجی ساختار جی سون از کلاس زیر را دارد.
        /// <para><see cref="ClassCollection.Model.LatestIcanForNotificationResult" /></para>
        /// <remarks>
        /// <para>Code : -1, Message : <see cref="ClassCollection.Message.OPERATION_NO_ACCESS"/>, Result.ican : <c>null</c></para>
        /// <para>Code : 0 , Message : <see cref="ClassCollection.Message.OPERATION_SUCCESS" />, Result.ican : List of <see cref=" ClassCollection.Model.LatestIcanForNotification"/></para>
        /// <para>Code : 1 , Message : <see cref="ClassCollection.Message.ERROR_INPUT_PARAMETER" />, Result.ican : <c>null</c></para>
        /// <para>Code : 2 , Message : <see cref="ClassCollection.Message.USER_NOT_EXIST" />, Result.ican : <c>null</c></para>
        /// <para>Code : 3 , Message : <see cref="ClassCollection.Message.USER_BLOCKED" />, Result.ican : <c>null</c></para>
        /// </remarks>
        /// </returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getLatestIcanForNotification(string key, string mobileOrEmail, long lastIcanID)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.LatestIcanForNotificationResult Result = new ClassCollection.Model.LatestIcanForNotificationResult();
            Result.result = new ClassCollection.Model.Result();
            

            var db = new DataAccessDataContext();
            var user = new mUserTbl();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            mobileOrEmail = mobileOrEmail.Trim();

            if (mobileOrEmail == "")
            {
                mobileOrEmail = "-1";
                user.ID = -1;
            }
            else
            {
                if (mobileOrEmail.Length != 12)
                {
                    Result.result.code = 2;
                    Result.result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }

                if (!db.mUserTbls.Any(c => c.emailtell == mobileOrEmail))
                {
                    Result.result.code = 3;
                    Result.result.message = ClassCollection.Message.USER_NOT_EXIST;
                    Result.ican = null;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                else
                {
                    user = db.mUserTbls.Single(c => c.emailtell == mobileOrEmail);
                    if (user.isBlock)
                    {
                        Result.result.code = 4;
                        Result.result.message = ClassCollection.Message.USER_BLOCKED;
                        Result.ican = null;
                        Context.Response.Write(js.Serialize(Result));
                        return;
                    }
                }

            }

            IQueryable<icanTbl> ican = db.icanTbls;

            ican = ican.Where(p => (p.ID > lastIcanID) && p.isBlock == false);

            Result.ican = new List<ClassCollection.Model.LatestIcanForNotification>();
            foreach (var item in ican)
            {
                var tmp = new ClassCollection.Model.LatestIcanForNotification();
                tmp.ID = item.ID;
                tmp.title = item.title;

                Result.ican.Add(tmp);
            }
            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result));
            return;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void sendIcanComment(string key, long icanID, long userID, string mobileOrEmail, string fullName, string text)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.Result Result = new ClassCollection.Model.Result();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.code = -1;
                Result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            fullName = fullName.TrimEnd().TrimStart();
            text = text.TrimStart().TrimEnd();

            if (fullName.Length == 0)
            {
                Result.code = 1;
                Result.message = ClassCollection.Message.NAME_INCORRECT;

                Context.Response.Write(js.Serialize(Result));
                return;
            }
            if (text.Length == 0 || text.Length > 250)
            {
                Result.code = 2;
                Result.message = ClassCollection.Message.COMMENT_TEXT_INCORRECT;

                Context.Response.Write(js.Serialize(Result));
                return;
            }
            //if (Methods.isEmail(mobileOrEmail) == false)
            //{
            //    if (mobileOrEmail.Length != 11)
            //    {
            //        Result.code = 3;
            //        Result.message = ClassCollection.Message.MOBILE_INCORRECT;

            //        Context.Response.Write(js.Serialize(Result));
            //        return;
            //    }
            //}

            DateTime dt = new DateTime();
            dt = DateTime.Now;
            var db = new DataAccessDataContext();

            if (db.icanTbls.Any(c => c.ID == icanID && c.isBlock == false) == false)
            {
                Result.code = 4;
                Result.message = ClassCollection.Message.ICAN_NOT_EXIST;

                Context.Response.Write(js.Serialize(Result));
                return;
            }
            if (db.mUserTbls.Any(c => c.ID == userID) == false && userID != -1)
            {
                Result.code = 5;
                Result.message = ClassCollection.Message.USER_NOT_EXIST;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            var icanComment = new icanCommentTbl();
            icanComment.fullName = fullName;
            icanComment.text = text;
            icanComment.regDate = dt;
            icanComment.icanID = icanID;
            icanComment.mUserID = userID;
            icanComment.mobile = Methods.isEmail(mobileOrEmail) ? null : mobileOrEmail;
            icanComment.email = Methods.isEmail(mobileOrEmail) ? mobileOrEmail : null;
            icanComment.isBlock = true;
            db.icanCommentTbls.InsertOnSubmit(icanComment);
            db.SubmitChanges();


            Result.code = 0;
            Result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result));
            return;

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getIcanCommentList(string key, long icanID, int count, int pageIndex)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.CommentListResult Result = new ClassCollection.Model.CommentListResult();
            Result.result = new ClassCollection.Model.Result();


            var skipCount = count * (pageIndex - 1);
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }
            if (db.icanTbls.Any(c => c.ID == icanID) == false)
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.ICAN_NOT_EXIST;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            Result.comment = new List<Model.Comment>();
            var comments = db.icanCommentTbls.Where(c => c.isBlock == false && c.icanID == icanID).OrderByDescending(c => c.regDate);
            Result.comment = comments.Skip(skipCount).Take(count).Select(c => new Model.Comment() { fullName = c.fullName, ID = c.ID, text = c.text }).ToList<Model.Comment>();


            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result));
            return;

        }
        #endregion

        #region BestIdeaCompetition

        [WebMethod(Description = "status : 1 > درحال ارسال ایده" + " 2 > تمام شده" + " 3 > درحال رای گیری")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getLatestBestIdeaCompetitionForList(string key, string mobileOrEmail, int status, string fillterString, int count, int pageIndex, long tagID)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            Model.LatestBestIdeaCompetitionForListResult Result = new Model.LatestBestIdeaCompetitionForListResult();
            Result.result = new Model.Result();

            var skipCount = count * (pageIndex - 1);
            var db = new DataAccessDataContext();
            var user = new mUserTbl();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            mobileOrEmail = mobileOrEmail.Trim();

            if (mobileOrEmail == "")
            {
                Result.result.code = -1;
                Result.result.message = Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }
            else
            {

                if (mobileOrEmail.Contains("@") && mobileOrEmail.Length >= 250)
                {
                    Result.result.code = 1;
                    Result.result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                if (!mobileOrEmail.Contains("@") && mobileOrEmail.Length != 12)
                {
                    Result.result.code = 2;
                    Result.result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }

                if (!db.mUserTbls.Any(c => c.emailtell == mobileOrEmail))
                {
                    Result.result.code = 3;
                    Result.result.message = ClassCollection.Message.USER_NOT_EXIST;
                    Result.competition = null;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                else
                {
                    user = db.mUserTbls.Single(c => c.emailtell == mobileOrEmail);
                    if (user.isBlock)
                    {
                        Result.result.code = 4;
                        Result.result.message = ClassCollection.Message.USER_BLOCKED;
                        Result.competition = null;
                        Context.Response.Write(js.Serialize(Result));
                        return;
                    }
                }

            }

            IQueryable<bestIdeaCompetitionsTbl> bestidea = null;
            if ((int)DataAccessDataContext.BestIdeaCompetitionStatus.done == status)
            {
                bestidea = db.getDoneBestIdeaCompetition();
            }
            else if ((int)DataAccessDataContext.BestIdeaCompetitionStatus.sending == status)
            {
                bestidea = db.getSendingBestIdeaCompetition();
            }
            else if ((int)DataAccessDataContext.BestIdeaCompetitionStatus.voting == status)
            {
                bestidea = db.getVotingBestIdeaCompetition();
            }

            if (tagID != -1 && db.tagTbls.Any(c => c.ID == tagID))
            {
                bestidea = bestidea.Where(c => c.bestIdeaCompetitionsTagTbls.Any(p => p.tagID == tagID));
            }

            #region search
            if (fillterString != "")
            {
                var search = new Search(fillterString);
                var ands = search.getAND;
                var ors = search.getOR;


                var predicate = PredicateBuilder.False<bestIdeaCompetitionsTbl>();
                foreach (string keyword in ors)
                {
                    string temp = keyword;
                    predicate = predicate.Or(
                        p => (p.bestIdeaCompetitionsContentTbls.Any(c => c.contentType == 0 && c.value.Contains(keyword))
                       || p.bestIdeaCompetitionsTagTbls.Any(c => c.tagTbl.title.Contains(keyword)) || p.bestIdeaCompetitionsTagTbls.Any(x => x.tagTbl.title.Contains(keyword)) ||
                       p.title.Contains(keyword))
                        && p.isBlock == false
                        );
                }


                foreach (var keyword in ands)
                {
                    bestidea = bestidea.Where(
                        p => (p.bestIdeaCompetitionsContentTbls.Any(c => c.contentType == 0 && c.value.Contains(keyword))
                       || p.bestIdeaCompetitionsTagTbls.Any(c => c.tagTbl.title.Contains(keyword)) ||
                       p.title.Contains(keyword))
                        && p.isBlock == false
                        );
                }
                bestidea = bestidea.Where(predicate);
            }



            #endregion

            bestidea = bestidea.Where(c => c.bestIdeaCompetitionsContentTbls.Any()).OrderByDescending(c => c.startDate);
            bestidea = bestidea.Skip(skipCount).Take(count);
            Result.competition = new List<Model.LatestBestIdeaCompetitionForList>();
            foreach (var item in bestidea)
            {
                var tmp = new ClassCollection.Model.LatestBestIdeaCompetitionForList();
                tmp.ID = item.ID;
                try
                {
                    tmp.image = item.bestIdeaCompetitionsContentTbls.Where(c => c.contentType == 1).Take(1).Single().value;
                }
                catch
                {
                    tmp.image = "";
                }

                var st = db.getBestIdeaCompetitionStatus(tmp.ID);
                if (st == DataAccessDataContext.BestIdeaCompetitionStatus.done)
                    tmp.date = Persia.Calendar.ConvertToPersian(item.resultVoteDate).ToString("n");
                else if (st == DataAccessDataContext.BestIdeaCompetitionStatus.voting)
                    tmp.date = Persia.Calendar.ConvertToPersian(item.endDate).ToString("n");
                else if (st == DataAccessDataContext.BestIdeaCompetitionStatus.sending)
                    tmp.date = Persia.Calendar.ConvertToPersian(item.startDate).ToString("n");

                if (st == DataAccessDataContext.BestIdeaCompetitionStatus.done)
                {
                    try
                    {
                        tmp.firstIdeaTitle = item.ideasTbls.OrderByDescending(c => c.ideasLikeTbls.Count(p => p.isLike == true)).Take(1).Single().title;
                    }
                    catch
                    {
                        tmp.firstIdeaTitle = "";
                    }
                }
                tmp.tag = new List<Model.Tag>();

                tmp.title = item.title;
                tmp.status = (int)st;

                foreach (var t in item.bestIdeaCompetitionsTagTbls)
                {
                    tmp.tag.Add(new Model.Tag() { ID = t.tagTbl.ID, title = t.tagTbl.title });
                }
                Result.competition.Add(tmp);
            }
            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result));
            return;

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getBestIdeaCompetition(string key, long bestIdeaCompetitionID)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.BestIdeaCompetitionResult Result = new Model.BestIdeaCompetitionResult();
            Result.result = new ClassCollection.Model.Result();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            
            var db = new DataAccessDataContext();

            if (db.bestIdeaCompetitionsTbls.Any(c => c.ID == bestIdeaCompetitionID && c.isBlock == false) == false)
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.COMPETITIONS_NOT_EXIST;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            var bestIdeaComp = db.bestIdeaCompetitionsTbls.Single(c => c.ID == bestIdeaCompetitionID);

            Result.bestIdeasCompetition = new ClassCollection.Model.BestIdeaCompetition();
            Result.bestIdeasCompetition.title = bestIdeaComp.title;
            Result.bestIdeasCompetition.ID = bestIdeaComp.ID;

            var st = db.getBestIdeaCompetitionStatus(bestIdeaComp.ID);
            if (st == DataAccessDataContext.BestIdeaCompetitionStatus.done)
            {
                Result.bestIdeasCompetition.date = Persia.Calendar.ConvertToPersian(bestIdeaComp.resultVoteDate).ToString("n");
                Result.bestIdeasCompetition.winnerTitle = bestIdeaComp.ideasTbls.OrderByDescending(c => c.ideasLikeTbls.Count(p => p.isLike == true)).Take(1).Single().title;
                Result.bestIdeasCompetition.winnerID = bestIdeaComp.ideasTbls.OrderByDescending(c => c.ideasLikeTbls.Count(p => p.isLike == true)).Take(1).Single().ID;
            }
            else if (st == DataAccessDataContext.BestIdeaCompetitionStatus.voting)
                Result.bestIdeasCompetition.date = Persia.Calendar.ConvertToPersian(bestIdeaComp.endDate).ToString("n");
            else if (st == DataAccessDataContext.BestIdeaCompetitionStatus.sending)
                Result.bestIdeasCompetition.date = Persia.Calendar.ConvertToPersian(bestIdeaComp.startDate).ToString("n");

            if (st == DataAccessDataContext.BestIdeaCompetitionStatus.sending)
            {
                Result.bestIdeasCompetition.remainingTime = ClassCollection.Methods.toFriendlyDateF(bestIdeaComp.endDate);
            }

            Result.bestIdeasCompetition.contents = new List<ClassCollection.Model.BestIdeasCompetitionContent>();
            var contents = bestIdeaComp.bestIdeaCompetitionsContentTbls.OrderBy(c => c.pri);

            foreach (var item in contents)
            {
                var tmp = new ClassCollection.Model.BestIdeasCompetitionContent();
                tmp.ID = item.ID;
                tmp.type = item.contentType;
                tmp.value = item.value;
                Result.bestIdeasCompetition.contents.Add(tmp);
            }


            Result.bestIdeasCompetition.status = (int)st;
            Result.bestIdeasCompetition.tag = new List<Model.Tag>();
            foreach (var t in bestIdeaComp.bestIdeaCompetitionsTagTbls)
            {
                Result.bestIdeasCompetition.tag.Add(new Model.Tag() { ID = t.tagTbl.ID, title = t.tagTbl.title });
            }

            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Context.Response.Write(js.Serialize(Result));
            return;
        }

        #endregion

        #region  Idea
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getLatestIdeaOfBestIdeaCompetitionForList(string key, long bestIdeaCompetitionID, string fillterString, int count, int pageIndex, long tagID)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            Model.LatestIdeasForListResult Result = new Model.LatestIdeasForListResult();
            Result.result = new Model.Result();

            var skipCount = count * (pageIndex - 1);

            var db = new DataAccessDataContext();
            var user = new mUserTbl();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            if (db.bestIdeaCompetitionsTbls.Any(c => c.ID == bestIdeaCompetitionID && c.isBlock == false) == false)
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.COMPETITIONS_NOT_EXIST;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            var bestIdeaComp = db.bestIdeaCompetitionsTbls.Single(c => c.ID == bestIdeaCompetitionID);

            var status = db.getBestIdeaCompetitionStatus(bestIdeaCompetitionID);

            IQueryable<ideasTbl> bestidea = null;
            if (DataAccessDataContext.BestIdeaCompetitionStatus.done == status)
            {
                bestidea = bestIdeaComp.ideasTbls.Where(c => c.isBlock == false).OrderByDescending(c => c.ideasLikeTbls.Count(x => x.isLike == true)).Take(3).AsQueryable();
            }
            else
            {
                bestidea = bestIdeaComp.ideasTbls.Where(c => c.isBlock == false).AsQueryable();
            }

            if (tagID != -1 && db.tagTbls.Any(c => c.ID == tagID))
            {
                bestidea = bestidea.Where(c => c.ideasTagTbls.Any(p => p.tagID == tagID));
            }

            #region search
            if (fillterString != "")
            {
                var search = new Search(fillterString);
                var ands = search.getAND;
                var ors = search.getOR;


                var predicate = PredicateBuilder.False<ideasTbl>();
                foreach (string keyword in ors)
                {
                    string temp = keyword;
                    predicate = predicate.Or(
                        p => (p.ideasContentTbls.Any(c => c.contentType == 0 && c.value.Contains(keyword))
                       || p.ideasTagTbls.Any(c => c.tagTbl.title.Contains(keyword)) ||
                       p.title.Contains(keyword)) ||
                       p.mUserTbl.name.Contains(keyword) || p.mUserTbl.family.Contains(keyword)
                        && p.isBlock == false
                        );
                }

                bestidea = bestidea.Where(predicate);
            }



            #endregion

            bestidea = bestidea.Where(c => c.ideasContentTbls.Any()).Skip(skipCount).Take(count);
            Result.idea = new List<Model.LatestIdeasForList>();
            foreach (var item in bestidea)
            {
                var tmp = new ClassCollection.Model.LatestIdeasForList();
                tmp.ID = item.ID;
                try
                {
                    tmp.image = item.ideasContentTbls.Where(c => c.contentType == 1).Take(1).Single().value;
                }
                catch
                {
                    tmp.image = "";
                }
                tmp.date = Persia.Calendar.ConvertToPersian(item.regDate).ToString("n");
                tmp.fullname = item.mUserTbl.name + " " + item.mUserTbl.family;
                tmp.tag = new List<Model.Tag>();
                tmp.title = item.title;
                foreach (var t in item.ideasTagTbls)
                {
                    tmp.tag.Add(new Model.Tag() { ID = t.tagTbl.ID, title = t.tagTbl.title });
                }
                Result.idea.Add(tmp);

            }
            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result));
            return;

        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getIdea(string key, long userID, long ideaID)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            Model.IdeaResult Result = new Model.IdeaResult();
            Result.result = new Model.Result();
            

            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            if (db.ideasTbls.Any(c => c.ID == ideaID && c.isBlock == false && c.bestIdeaCompetitionsTbl.isBlock == false) == false)
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.IDEA_NOT_EXIST;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            if (db.mUserTbls.Any(c => c.ID == userID && c.isBlock == false) == false)
            {
                Result.result.code = 2;
                Result.result.message = ClassCollection.Message.USER_NOT_EXIST;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            var user = db.mUserTbls.Single(c => c.ID == userID);

            var idea = db.ideasTbls.Single(c => c.ID == ideaID);

            var status = db.getBestIdeaCompetitionStatus(idea.bestIdeaCompetitionsID);

            Result.idea = new Model.fullIdea();
            Result.idea.tag = new List<Model.Tag>();
            Result.idea.title = idea.title;
            Result.idea.date = Persia.Calendar.ConvertToPersian(idea.regDate).ToString("n");
            Result.idea.fullname = idea.mUserTbl.name + " " + idea.mUserTbl.family;
            Result.idea.mUserID = idea.mUserID;
            Result.idea.ID = idea.ID;
            Result.idea.status = (int)status;
            Result.idea.userCanVote = true;

            if (status == DataAccessDataContext.BestIdeaCompetitionStatus.voting)
            {
                if (idea.ideasLikeTbls.Any(c => c.userID == userID))
                    Result.idea.userCanVote = false;
            }
            else
            {
                Result.idea.userCanVote = false;
            }

            foreach (var t in idea.ideasTagTbls)
            {
                Result.idea.tag.Add(new Model.Tag() { ID = t.tagTbl.ID, title = t.tagTbl.title });
            }

            Result.idea.contents = new List<Model.ideaContent>();
            foreach (var t in idea.ideasContentTbls.OrderBy(c => c.pri))
            {
                Result.idea.contents.Add(new Model.ideaContent() { ID = t.ID, type = t.contentType, value = t.value });
            }

            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Context.Response.Write(js.Serialize(Result));
            return;

        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void voteIdea(string key, long userID, long ideaID, bool isLike)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            Model.Result Result = new Model.Result();
            Result = new Model.Result();

            DateTime dt = new DateTime();
            dt = DateTime.Now;

            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.code = -1;
                Result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            if (db.ideasTbls.Any(c => c.ID == ideaID && c.isBlock == false && c.bestIdeaCompetitionsTbl.isBlock == false) == false)
            {
                Result.code = 1;
                Result.message = ClassCollection.Message.IDEA_NOT_EXIST;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            if (db.mUserTbls.Any(c => c.ID == userID && c.isBlock == false) == false)
            {
                Result.code = 2;
                Result.message = ClassCollection.Message.USER_NOT_EXIST;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            var user = db.mUserTbls.Single(c => c.ID == userID);

            var idea = db.ideasTbls.Single(c => c.ID == ideaID);

            var status = db.getBestIdeaCompetitionStatus(idea.bestIdeaCompetitionsID);

            if (status == DataAccessDataContext.BestIdeaCompetitionStatus.voting)
            {
                if (idea.ideasLikeTbls.Any(c => c.userID == userID))
                {
                    Result.code = 10;
                    Result.message = ClassCollection.Message.USER_VOTED;

                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
            }
            else
            {
                Result.code = 11;
                Result.message = ClassCollection.Message.USER_CANT_VOTE;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            var like = new ideasLikeTbl();
            like.ideasID = ideaID;
            like.isLike = isLike;
            like.regDate = dt;
            like.userID = userID;

            db.ideasLikeTbls.InsertOnSubmit(like);
            db.SubmitChanges();

            Result.code = 0;
            Result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Context.Response.Write(js.Serialize(Result));
            return;

        }

        #endregion

        #region CreativityCompetition

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getLatestCreativityCompetitionForList(string key, string mobileOrEmail, int status, string fillterString, int count, int pageIndex, long tagID)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            Model.LatestCreativityCompetitionForListResult Result = new Model.LatestCreativityCompetitionForListResult();
            Result.result = new Model.Result();

            var skipCount = count * (pageIndex - 1);
            var db = new DataAccessDataContext();
            var user = new mUserTbl();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            mobileOrEmail = mobileOrEmail.Trim();

            if (mobileOrEmail == "")
            {
                Result.result.code = -1;
                Result.result.message = Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }
            else
            {

                if (mobileOrEmail.Contains("@") && mobileOrEmail.Length >= 250)
                {
                    Result.result.code = 1;
                    Result.result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                if (!mobileOrEmail.Contains("@") && mobileOrEmail.Length != 12)
                {
                    Result.result.code = 2;
                    Result.result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }

                if (!db.mUserTbls.Any(c => c.emailtell == mobileOrEmail))
                {
                    Result.result.code = 3;
                    Result.result.message = ClassCollection.Message.USER_NOT_EXIST;
                    Result.competition = null;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                else
                {
                    user = db.mUserTbls.Single(c => c.emailtell == mobileOrEmail);
                    if (user.isBlock)
                    {
                        Result.result.code = 4;
                        Result.result.message = ClassCollection.Message.USER_BLOCKED;
                        Result.competition = null;
                        Context.Response.Write(js.Serialize(Result));
                        return;
                    }
                }

            }

            IQueryable<creativityCompetitionTbl> creativity = null;
            if ((int)DataAccessDataContext.CreativityCompetitionStatus.done == status)
            {
                creativity = db.getDoneCreativityCompetition();
            }
            else if ((int)DataAccessDataContext.CreativityCompetitionStatus.sending == status)
            {
                creativity = db.getSendingCreativityCompetition();
            }
            else if ((int)DataAccessDataContext.CreativityCompetitionStatus.pending == status)
            {
                creativity = db.getPendingCreativityCompetition();
            }

            if (tagID != -1 && db.tagTbls.Any(c => c.ID == tagID))
            {
                creativity = creativity.Where(c => c.creativityCompetitionTagTbls.Any(p => p.tagID == tagID));
            }

            #region search
            if (fillterString != "")
            {
                var search = new Search(fillterString);
                var ands = search.getAND;
                var ors = search.getOR;


                var predicate = PredicateBuilder.False<creativityCompetitionTbl>();
                foreach (string keyword in ors)
                {
                    string temp = keyword;
                    predicate = predicate.Or(
                        p => (p.creativityCompetitionContentTbls.Any(c => c.contentType == 0 && c.value.Contains(keyword))
                       || p.creativityCompetitionTagTbls.Any(c => c.tagTbl.title.Contains(keyword)) ||
                       p.title.Contains(keyword))
                        && p.isBlock == false
                        );
                }


                creativity = creativity.Where(predicate);
            }



            #endregion

            creativity = creativity.Where(c => c.creativityCompetitionContentTbls.Any()).OrderByDescending(c => c.startDate);
            creativity = creativity.Skip(skipCount).Take(count);
            Result.competition = new List<Model.LatestCreativityCompetitionForList>();
            foreach (var item in creativity)
            {
                var tmp = new ClassCollection.Model.LatestCreativityCompetitionForList();
                tmp.ID = item.ID;
                try
                {
                    tmp.image = item.creativityCompetitionContentTbls.Where(c => c.contentType == 1).Take(1).Single().value;
                }
                catch
                {
                    tmp.image = "";
                }

                var st = db.getCreativityCompetitionStatus(tmp.ID);
                if (st == DataAccessDataContext.CreativityCompetitionStatus.done)
                    tmp.date = Persia.Calendar.ConvertToPersian(item.doneDate.Value).ToString("n");
                else if (st == DataAccessDataContext.CreativityCompetitionStatus.pending)
                    tmp.date = Persia.Calendar.ConvertToPersian(item.endDate).ToString("n");
                else if (st == DataAccessDataContext.CreativityCompetitionStatus.sending)
                    tmp.date = Persia.Calendar.ConvertToPersian(item.startDate).ToString("n");

                if (st == DataAccessDataContext.CreativityCompetitionStatus.done)
                {
                    try
                    {
                        tmp.firstAnswerTitle = item.answerTbls.Where(c => c.isCorrect == true && c.isWinner == true).Take(1).Single().title;
                    }
                    catch
                    {
                        tmp.firstAnswerTitle = "";
                    }
                }
                tmp.tag = new List<Model.Tag>();

                tmp.title = item.title;
                tmp.status = (int)st;

                foreach (var t in item.creativityCompetitionTagTbls)
                {
                    tmp.tag.Add(new Model.Tag() { ID = t.tagTbl.ID, title = t.tagTbl.title });
                }
                Result.competition.Add(tmp);
            }
            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result));
            return;

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getCreativityCompetition(string key, long creativityCompetitionID)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.CreativityCompetitionResult Result = new Model.CreativityCompetitionResult();
            Result.result = new ClassCollection.Model.Result();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }
            
            var db = new DataAccessDataContext();

            if (db.creativityCompetitionTbls.Any(c => c.ID == creativityCompetitionID && c.isBlock == false) == false)
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.COMPETITIONS_NOT_EXIST;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            var creativityComp = db.creativityCompetitionTbls.Single(c => c.ID == creativityCompetitionID);

            Result.creativityCompetition = new ClassCollection.Model.CreativityCompetition();
            Result.creativityCompetition.title = creativityComp.title;
            Result.creativityCompetition.ID = creativityComp.ID;

            var st = db.getCreativityCompetitionStatus(creativityComp.ID);
            if (st == DataAccessDataContext.CreativityCompetitionStatus.done)
            {
                Result.creativityCompetition.date = Persia.Calendar.ConvertToPersian(creativityComp.doneDate.Value).ToString("n");
                Result.creativityCompetition.winnerTitle = creativityComp.answerTbls.Where(c => c.isCorrect == true && c.isWinner == true).Take(1).Single().title;
                Result.creativityCompetition.winnerID = creativityComp.answerTbls.Where(c => c.isCorrect == true && c.isWinner == true).Take(1).Single().ID;
            }
            else if (st == DataAccessDataContext.CreativityCompetitionStatus.pending)
                Result.creativityCompetition.date = Persia.Calendar.ConvertToPersian(creativityComp.endDate).ToString("n");
            else if (st == DataAccessDataContext.CreativityCompetitionStatus.sending)
                Result.creativityCompetition.date = Persia.Calendar.ConvertToPersian(creativityComp.startDate).ToString("n");

            if (st == DataAccessDataContext.CreativityCompetitionStatus.sending)
            {
                Result.creativityCompetition.remainingTime = ClassCollection.Methods.toFriendlyDateF(creativityComp.endDate);
            }

            Result.creativityCompetition.contents = new List<ClassCollection.Model.CreativitysCompetitionContent>();
            var contents = creativityComp.creativityCompetitionContentTbls.OrderBy(c => c.pri);

            foreach (var item in contents)
            {
                var tmp = new ClassCollection.Model.CreativitysCompetitionContent();
                tmp.ID = item.ID;
                tmp.type = item.contentType;
                tmp.value = item.value;
                Result.creativityCompetition.contents.Add(tmp);
            }


            Result.creativityCompetition.status = (int)st;
            Result.creativityCompetition.tag = new List<Model.Tag>();
            foreach (var t in creativityComp.creativityCompetitionTagTbls)
            {
                Result.creativityCompetition.tag.Add(new Model.Tag() { ID = t.tagTbl.ID, title = t.tagTbl.title });
            }

            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Context.Response.Write(js.Serialize(Result));
            return;
        }

        #endregion

        #region MyIranCompetition

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getLatestMyIranCompetitionForList(string key, string mobileOrEmail, int status, string fillterString, int count, int pageIndex, long tagID)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            Model.LatestMyIranForListResult Result = new Model.LatestMyIranForListResult();
            Result.result = new Model.Result();

            var skipCount = count * (pageIndex - 1);
            var db = new DataAccessDataContext();
            var user = new mUserTbl();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            mobileOrEmail = mobileOrEmail.Trim();

            if (mobileOrEmail == "")
            {
                Result.result.code = -1;
                Result.result.message = Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }
            else
            {

                if (mobileOrEmail.Contains("@") && mobileOrEmail.Length >= 250)
                {
                    Result.result.code = 1;
                    Result.result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                if (!mobileOrEmail.Contains("@") && mobileOrEmail.Length != 12)
                {
                    Result.result.code = 2;
                    Result.result.message = string.Format(ClassCollection.Message.ERROR_INPUT_PARAMETER, "2");
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }

                if (!db.mUserTbls.Any(c => c.emailtell == mobileOrEmail))
                {
                    Result.result.code = 3;
                    Result.result.message = ClassCollection.Message.USER_NOT_EXIST;
                    Result.competition = null;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                else
                {
                    user = db.mUserTbls.Single(c => c.emailtell == mobileOrEmail);
                    if (user.isBlock)
                    {
                        Result.result.code = 4;
                        Result.result.message = ClassCollection.Message.USER_BLOCKED;
                        Result.competition = null;
                        Context.Response.Write(js.Serialize(Result));
                        return;
                    }
                }

            }

            IQueryable<myIranTbl> myIran = null;
            if ((int)DataAccessDataContext.MyIranStatus.done == status)
            {
                myIran = db.getDoneMyIran();
            }
            else if ((int)DataAccessDataContext.MyIranStatus.sending == status)
            {
                myIran = db.getSendingMyIran();
            }
            else if ((int)DataAccessDataContext.MyIranStatus.pending == status)
            {
                myIran = db.getPendingMyIran();
            }

            if (tagID != -1 && db.tagTbls.Any(c => c.ID == tagID))
            {
                myIran = myIran.Where(c => c.myIranTagTbls.Any(p => p.tagID == tagID));
            }

            #region search
            if (fillterString != "")
            {
                var search = new Search(fillterString);
                var ands = search.getAND;
                var ors = search.getOR;


                var predicate = PredicateBuilder.False<myIranTbl>();
                foreach (string keyword in ors)
                {
                    string temp = keyword;
                    predicate = predicate.Or(
                        p => (p.myIranContentTbls.Any(c => c.contentType == 0 && c.value.Contains(keyword))
                       || p.myIranTagTbls.Any(c => c.tagTbl.title.Contains(keyword)) ||
                       p.title.Contains(keyword))
                        && p.isBlock == false
                        );
                }


                myIran = myIran.Where(predicate);
            }



            #endregion

            myIran = myIran.Where(c => c.myIranContentTbls.Any()).OrderByDescending(c => c.startDate);
            myIran = myIran.Skip(skipCount).Take(count);
            Result.competition = new List<Model.LatestMyIranForList>();
            foreach (var item in myIran)
            {
                var tmp = new ClassCollection.Model.LatestMyIranForList();
                tmp.ID = item.ID;
                try
                {
                    tmp.image = item.myIranContentTbls.Where(c => c.contentType == 1).Take(1).Single().value;
                }
                catch
                {
                    tmp.image = "";
                }

                var st = db.getMyIranStatus(tmp.ID);
                if (st == DataAccessDataContext.MyIranStatus.done)
                    tmp.date = Persia.Calendar.ConvertToPersian(item.doneDate.Value).ToString("n");
                else if (st == DataAccessDataContext.MyIranStatus.pending)
                    tmp.date = Persia.Calendar.ConvertToPersian(item.endDate).ToString("n");
                else if (st == DataAccessDataContext.MyIranStatus.sending)
                    tmp.date = Persia.Calendar.ConvertToPersian(item.startDate).ToString("n");

                if (st == DataAccessDataContext.MyIranStatus.done)
                {
                    try
                    {
                        tmp.firstResponseTitle = item.responseTbls.Where(c => c.isCorrect == true && c.isWinner == true).Take(1).Single().title;
                    }
                    catch
                    {
                        tmp.firstResponseTitle = "";
                    }
                }
                tmp.tag = new List<Model.Tag>();

                tmp.title = item.title;
                tmp.status = (int)st;

                foreach (var t in item.myIranTagTbls)
                {
                    tmp.tag.Add(new Model.Tag() { ID = t.tagTbl.ID, title = t.tagTbl.title });
                }
                Result.competition.Add(tmp);
            }
            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;

            Context.Response.Write(js.Serialize(Result));
            return;

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getMyIranCompetition(string key, long myIranCompetitionID)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.MyIranResult Result = new Model.MyIranResult();
            Result.result = new ClassCollection.Model.Result();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            
            var db = new DataAccessDataContext();

            if (db.myIranTbls.Any(c => c.ID == myIranCompetitionID && c.isBlock == false) == false)
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.COMPETITIONS_NOT_EXIST;

                Context.Response.Write(js.Serialize(Result));
                return;
            }

            var myIranComp = db.myIranTbls.Single(c => c.ID == myIranCompetitionID);

            Result.myIran = new ClassCollection.Model.MyIran();
            Result.myIran.title = myIranComp.title;
            Result.myIran.ID = myIranComp.ID;

            var st = db.getMyIranStatus(myIranComp.ID);
            if (st == DataAccessDataContext.MyIranStatus.done)
            {
                Result.myIran.date = Persia.Calendar.ConvertToPersian(myIranComp.doneDate.Value).ToString("n");


                Result.myIran.winnerTitle = myIranComp.responseTbls.Where(c => c.isCorrect == true && c.isWinner == true).Take(1).Single().title;
                Result.myIran.winnerID = myIranComp.responseTbls.Where(c => c.isCorrect == true && c.isWinner == true).Take(1).Single().ID;

            }
            else if (st == DataAccessDataContext.MyIranStatus.pending)
                Result.myIran.date = Persia.Calendar.ConvertToPersian(myIranComp.endDate).ToString("n");
            else if (st == DataAccessDataContext.MyIranStatus.sending)
                Result.myIran.date = Persia.Calendar.ConvertToPersian(myIranComp.startDate).ToString("n");

            if (st == DataAccessDataContext.MyIranStatus.sending)
            {
                Result.myIran.remainingTime = ClassCollection.Methods.toFriendlyDateF(myIranComp.endDate);
            }

            Result.myIran.contents = new List<ClassCollection.Model.myIransContent>();
            var contents = myIranComp.myIranContentTbls.OrderBy(c => c.pri);

            foreach (var item in contents)
            {
                var tmp = new ClassCollection.Model.myIransContent();
                tmp.ID = item.ID;
                tmp.type = item.contentType;
                tmp.value = item.value;
                Result.myIran.contents.Add(tmp);
            }


            Result.myIran.status = (int)st;
            Result.myIran.tag = new List<Model.Tag>();
            foreach (var t in myIranComp.myIranTagTbls)
            {
                Result.myIran.tag.Add(new Model.Tag() { ID = t.tagTbl.ID, title = t.tagTbl.title });
            }

            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Context.Response.Write(js.Serialize(Result));
            return;
        }

        #endregion


        //شروع درج محتوا

        #region registerNews
        //soheila-start-registerNews
        /// <summary>
        /// </summary>
        /// <param name="newsID"></param>
        /// <param name="newscontent">[{"ID":"205","type":"0","value":"fffffffffffffffffffff"}]</param>
        /// 
        [WebMethod(Description = "این تابع وظیفه درج خبر جدید را بر عهده دارد")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void registerNews(string key, long userID, string title, string publishDate, string tagIDs)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.LongResult Result = new ClassCollection.Model.LongResult();
            Result.result = new Model.Result();
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            if (db.mUserTbls.Any(c => c.ID == userID) == false)
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.USER_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }
            if (db.mUserTbls.Single(c => c.ID == userID).isBlock == true)
            {
                Result.result.code = 2;
                Result.result.message = ClassCollection.Message.USER_BLOCKED;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            title = title.TrimStart().TrimEnd();
            if (title.Length == 0)
            {
                Result.result.code = 3;
                Result.result.message = ClassCollection.Message.TEXT_INCORRECT;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            var publishDate1 = new DateTime();
            if (publishDate != "")
            {
                publishDate = publishDate.Trim();
                int pyear = 0, pmonth = 0, pday = 0;
                var DateArray = publishDate.Split('-');
                if (DateArray.Count() != 3
                   || !int.TryParse(DateArray[0], out pyear)
                   || !int.TryParse(DateArray[1], out pmonth)
                   || !int.TryParse(DateArray[2], out pday))
                {
                    Result.result.code = 4;
                    Result.result.message = ClassCollection.Message.DATE_INCORRECT;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }

                try
                {
                    publishDate1 = new DateTime(pyear, pmonth, pday, 0, 0, 0);
                }
                catch
                {
                    Result.result.code = 2;
                    Result.result.message = ClassCollection.Message.DATE_INCORRECT;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
            }

            

            List<long> tags = new List<long>();
            try
            {
                if (tagIDs.Length != 0)
                {
                    tags = new JavaScriptSerializer().Deserialize<List<string>>(tagIDs).Select(c => long.Parse(c)).ToList();
                    if (tags.Count() == 0)
                    {
                        Result.result.code = 5;
                        Result.result.message = ClassCollection.Message.TAGS_NOT_EXIST;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                        return;
                    }
                }
            }
            catch
            {
                Result.result.code = 6;
                Result.result.message = ClassCollection.Message.TAGS_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }
            foreach (var item in tags)
            {
                if (db.tagTbls.Any(c => c.ID == item) == false)
                {
                    Result.result.code = 7;
                    Result.result.message = ClassCollection.Message.TAGS_NOT_EXIST;
                    Context.Response.Write(js.Serialize(Result).ToLower());
                    return;
                }
            }
            var news = new newsTbl();
            var dt = new DateTime();
            dt = DateTime.Now;
            news.title = title;
            news.isBlock = true;
            news.mUserID = userID;
            if (publishDate != "")
            {
                news.publishDate = publishDate1;
            }
            else
            {
                news.publishDate = dt.Date;
            }
            news.regDate = dt;
            news.viewCount = 0;
            db.newsTbls.InsertOnSubmit(news);
            db.SubmitChanges();

            foreach (var item in tags)
            {
                var tag = new newsTagTbl();
                tag.newsID = news.ID;
                tag.tagID = item;

                db.newsTagTbls.InsertOnSubmit(tag);
                db.SubmitChanges();
            }

            //new service.pushservice().XsendPush("1", "news", news.ID + "", news.title, "all", "false");
            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Result.value = news.ID;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }

        [WebMethod(Description = "این تابع وظیفه تغییر خبر را بر عهده دارد")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void editNews(string key, long newsID, long userID, string title, string publishDate, string tagIDs)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.LongResult Result = new ClassCollection.Model.LongResult();
            Result.result = new Model.Result();
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(js.Serialize(Result));
                return;
            }


            if (db.mUserTbls.Any(c => c.ID == userID) == false)
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.USER_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }
            if (db.mUserTbls.Single(c => c.ID == userID).isBlock == true)
            {
                Result.result.code = 2;
                Result.result.message = ClassCollection.Message.USER_BLOCKED;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (!db.newsTbls.Any(c => c.ID == newsID && c.mUserID.Value == userID))
            {
                Result.result.code = 3;
                Result.result.message = ClassCollection.Message.NEWS_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }


            var news = db.newsTbls.Single(c => c.ID == newsID);

            title = title.TrimStart().TrimEnd();
            if (title.Length == 0)
            {
                title = news.title;
            }
            
            var publishDate1 = new DateTime();
            if (publishDate != "")
            {
                publishDate = publishDate.Trim();
                int pyear = 0, pmonth = 0, pday = 0;
                var DateArray = publishDate.Split('-');
                if (DateArray.Count() != 3
                   || !int.TryParse(DateArray[0], out pyear)
                   || !int.TryParse(DateArray[1], out pmonth)
                   || !int.TryParse(DateArray[2], out pday))
                {
                    Result.result.code = 4;
                    Result.result.message = ClassCollection.Message.DATE_INCORRECT;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }

                try
                {
                    publishDate1 = new DateTime(pyear, pmonth, pday, 0, 0, 0);
                }
                catch
                {
                    Result.result.code = 2;
                    Result.result.message = ClassCollection.Message.DATE_INCORRECT;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
            }
            
            List<long> tags = new List<long>();
            bool tagchange = false;

            try
            {
                if (tagIDs.Length != 0)
                {
                    tags = new JavaScriptSerializer().Deserialize<List<string>>(tagIDs).Select(c => long.Parse(c)).ToList();
                    if (tags.Count() == 0)
                    {
                        tagchange = false;
                    }
                    else
                    {
                        tagchange = true;
                    }
                }
                else
                {
                    tagchange = false;
                }
            }
            catch
            {
                Result.result.code = 4;
                Result.result.message = ClassCollection.Message.TAGS_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (tagchange == true)
            {
                foreach (var item in tags)
                {
                    if (db.tagTbls.Any(c => c.ID == item) == false)
                    {
                        Result.result.code = 5;
                        Result.result.message = ClassCollection.Message.TAGS_NOT_EXIST;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                        return;
                    }
                }

                var oldTags = db.newsTagTbls.Where(c => c.newsID == newsID);
                db.newsTagTbls.DeleteAllOnSubmit(oldTags);
                db.SubmitChanges();
            }

            news.title = title;
            news.isBlock = true;
            var dt = new DateTime();
            dt = DateTime.Now;
            if (publishDate != "")
            {
                news.publishDate = publishDate1;
            }
            else
            {
                news.publishDate = dt.Date;
            }

            db.SubmitChanges();

            if (tagchange == true)
            {
                foreach (var item in tags)
                {
                    var tag = new newsTagTbl();
                    tag.newsID = news.ID;
                    tag.tagID = item;

                    db.newsTagTbls.InsertOnSubmit(tag);
                    db.SubmitChanges();
                }
            }

            //new service.pushservice().XsendPush("1", "news", news.ID + "", news.title, "all", "false");
            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }

        [WebMethod(Description = "این تابع وظیفه درج جزییات خبر را بر عهده دارد")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void registerNewsContent(string key, long newsID, int contentType, string value)
        {
            ClassCollection.Model.LongResult Result = new ClassCollection.Model.LongResult();
            Result.result = new Model.Result();
            JavaScriptSerializer js = new JavaScriptSerializer();
            DateTime dt = new DateTime();
            dt = DateTime.Now;
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(js.Serialize(Result));
                return;
            }
            if (!db.newsTbls.Any(c => c.ID == newsID))
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.NEWS_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (contentType != 0 && contentType != 1 && contentType != 2)
            {
                Result.result.code = 2;
                Result.result.message = ClassCollection.Message.NEWSContent_INCORRECT;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (value.Length == 0)
            {
                Result.result.code = 3;
                Result.result.message = ClassCollection.Message.TEXT_INCORRECT;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            var tmp = new newsContentTbl();

            if (contentType == 0)
            {
                tmp.value = "<p>" + value + "</p>";
            }
            if (contentType == 1 || contentType == 2)
            {
                try
                {
                    var totalPath = HttpContext.Current.Server.MapPath(ClassCollection.Methods.getfilePath()) + value;
                    if (!System.IO.File.Exists(totalPath))
                    {
                        Result.result.code = 4;
                        Result.result.message = ClassCollection.Message.ERROR_EXIST_FILE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                        return;
                    }
                    var ext = value.Split('.').Last();
                    var fileData = File.ReadAllBytes(totalPath);
                    File.Delete(totalPath);

                    var filename = "appinno" + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N") + "." + ext;
                    string fpath = "";
                    tmp.value = filename;
                    if (contentType == 1)
                    {
                        fpath = Server.MapPath(ClassCollection.Methods.getNewsImagesPath() + filename);
                    }
                    else if (contentType == 2)
                    {
                        fpath = Server.MapPath(ClassCollection.Methods.getNewsVideosPath() + filename);
                    }
                    try
                    {
                        File.WriteAllBytes(fpath, fileData);
                    }
                    catch
                    {
                        Result.result.code = 5;
                        Result.result.message = ClassCollection.Message.ERROR_SAVE_FILE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                        return;
                    }
                }
                catch
                {
                    Result.result.code = 6;
                    Result.result.message = ClassCollection.Message.ERROR_TRANSFER_FILE;
                    Context.Response.Write(js.Serialize(Result).ToLower());
                    return;
                }
            }

            tmp.newsID = newsID;
            tmp.contentType = contentType;
            if (!db.newsContentTbls.Any(c => c.newsID == newsID))
            {
                tmp.pri = 0;
            }
            else
            {
                tmp.pri = db.newsContentTbls.Where(c => c.newsID == newsID).Max(c => c.pri) + 1;
            }
            tmp.regDate = dt;
            db.newsContentTbls.InsertOnSubmit(tmp);
            db.SubmitChanges();

            //new service.pushservice().XsendPush("1", "news", news.ID + "", news.title, "all", "false");
            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Result.value = tmp.ID;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }

        [WebMethod(Description = "این تابع وظیفه حذف جزییات خبر را بر عهده دارد")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void deleteNewsContent(string key, long contentID)
        {
            ClassCollection.Model.Result Result = new ClassCollection.Model.Result();
            JavaScriptSerializer js = new JavaScriptSerializer();
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.code = -1;
                Result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            if (!db.newsContentTbls.Any(c => c.ID == contentID))
            {
                Result.code = 1;
                Result.message = ClassCollection.Message.NEWSContent_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            var deletingPar = db.newsContentTbls.Single(c => c.ID == contentID);

            if (deletingPar.contentType == 1)
            {
                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getNewsImagesPath() + deletingPar.value)))
                {
                    try
                    {
                        System.IO.File.Delete(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getNewsImagesPath() + deletingPar.value));
                    }
                    catch
                    {
                        Result.code = 2;
                        Result.message = ClassCollection.Message.ERROR_DELETE_IMAGE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                    }
                }
            }
            else if (deletingPar.contentType == 2)
            {
                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getNewsVideosPath() + deletingPar.value)))
                {
                    try
                    {
                        System.IO.File.Delete(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getNewsVideosPath() + deletingPar.value));
                    }
                    catch
                    {
                        Result.code = 3;
                        Result.message = ClassCollection.Message.ERROR_DELETE_VIDEO;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                    }
                }
            }
            db.newsContentTbls.DeleteOnSubmit(deletingPar);
            db.SubmitChanges();

            Result.code = 0;
            Result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }

        [WebMethod(Description = "این تابع وظیفه تغییر جزییات خبر را بر عهده دارد")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void editNewsContent(string key, long contentID, int contentType, string value)
        {
            ClassCollection.Model.LongResult Result = new ClassCollection.Model.LongResult();
            Result.result = new Model.Result();
            JavaScriptSerializer js = new JavaScriptSerializer();
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            if (!db.newsContentTbls.Any(c => c.ID == contentID))
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.NEWSContent_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            var newscontent = db.newsContentTbls.Single(c => c.ID == contentID);

            if (value.Length == 0)
            {
                value = newscontent.value;
            }

            if (contentType.ToString().Length != 0)
            {
                if (contentType != 0 && contentType != 1 && contentType != 2)
                {
                    Result.result.code = 2;
                    Result.result.message = ClassCollection.Message.NEWSContent_INCORRECT;
                    Context.Response.Write(js.Serialize(Result).ToLower());
                    return;
                }
            }
            else
            {
                contentType = newscontent.contentType;
            }

            if (newscontent.contentType == 1)
            {
                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getNewsImagesPath() + newscontent.value)) && newscontent.value != value)
                {
                    try
                    {
                        System.IO.File.Delete(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getNewsImagesPath() + newscontent.value));
                    }
                    catch
                    {
                        Result.result.code = 3;
                        Result.result.message = ClassCollection.Message.ERROR_DELETE_IMAGE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                    }
                }
            }
            else if (newscontent.contentType == 2)
            {
                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getNewsVideosPath() + newscontent.value)) && newscontent.value != value)
                {
                    try
                    {
                        System.IO.File.Delete(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getNewsVideosPath() + newscontent.value));
                    }
                    catch
                    {
                        Result.result.code = 4;
                        Result.result.message = ClassCollection.Message.ERROR_DELETE_VIDEO;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                    }
                }
            }

            if (newscontent.contentType == 1 || newscontent.contentType == 2)
            {
                try
                {
                    var totalPath = HttpContext.Current.Server.MapPath(ClassCollection.Methods.getfilePath()) + value;
                    if (!System.IO.File.Exists(totalPath))
                    {
                        Result.result.code = 4;
                        Result.result.message = ClassCollection.Message.ERROR_EXIST_FILE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                        return;
                    }
                    var ext = value.Split('.').Last();
                    var fileData = File.ReadAllBytes(totalPath);
                    File.Delete(totalPath);

                    var filename = "appinno" + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N") + "." + ext;
                    string fpath = "";
                    value = filename;
                    if (contentType == 1)
                    {
                        fpath = Server.MapPath(ClassCollection.Methods.getNewsImagesPath() + filename);
                    }
                    else if (contentType == 2)
                    {
                        fpath = Server.MapPath(ClassCollection.Methods.getNewsVideosPath() + filename);
                    }
                    try
                    {
                        File.WriteAllBytes(fpath, fileData);
                    }
                    catch
                    {
                        Result.result.code = 5;
                        Result.result.message = ClassCollection.Message.ERROR_SAVE_FILE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                        return;
                    }
                }
                catch
                {
                    Result.result.code = 6;
                    Result.result.message = ClassCollection.Message.ERROR_TRANSFER_FILE;
                    Context.Response.Write(js.Serialize(Result).ToLower());
                    return;
                }
            }

            newscontent.contentType = contentType;
            newscontent.value = value;
            db.SubmitChanges();

            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }
        //soheila-end-registerNews
        #endregion

        #region registerEvent 
        //soheila-start-registerEvent

        [WebMethod(Description = "این تابع وظیفه درج رویداد جدید را بر عهده دارد")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void registerEvent(string key, long userID, string title, string fromDate, string toDate, string tagIDs)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.LongResult Result = new ClassCollection.Model.LongResult();
            Result.result = new Model.Result();
            var db = new DataAccessDataContext();
            var Event = new eventsTbl();
            var dt = new DateTime();
            dt = DateTime.Now;

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            if (db.mUserTbls.Any(c => c.ID == userID) == false)
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.USER_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }
            if (db.mUserTbls.Single(c => c.ID == userID).isBlock == true)
            {
                Result.result.code = 2;
                Result.result.message = ClassCollection.Message.USER_BLOCKED;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            title = title.TrimStart().TrimEnd();
            if (title.Length == 0)
            {
                Result.result.code = 3;
                Result.result.message = ClassCollection.Message.TEXT_INCORRECT;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }
            
            var fromDate1 = new DateTime();
            if (fromDate != "")
            {
                fromDate = fromDate.Trim();
                int pyear = 0, pmonth = 0, pday = 0;
                var DateArray = fromDate.Split('-');
                if (DateArray.Count() != 3
                   || !int.TryParse(DateArray[0], out pyear)
                   || !int.TryParse(DateArray[1], out pmonth)
                   || !int.TryParse(DateArray[2], out pday))
                {
                    Result.result.code = 4;
                    Result.result.message = ClassCollection.Message.DATE_INCORRECT;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }

                try
                {
                    fromDate1 = new DateTime(pyear, pmonth, pday, 0, 0, 0);
                }
                catch
                {
                    Result.result.code = 5;
                    Result.result.message = ClassCollection.Message.DATE_INCORRECT;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
            }

            
            var toDate1 = new DateTime();
            if (toDate != "")
            {
                toDate = toDate.Trim();
                int pyear = 0, pmonth = 0, pday = 0;
                var DateArray = toDate.Split('-');
                if (DateArray.Count() != 3
                   || !int.TryParse(DateArray[0], out pyear)
                   || !int.TryParse(DateArray[1], out pmonth)
                   || !int.TryParse(DateArray[2], out pday))
                {
                    Result.result.code = 6;
                    Result.result.message = ClassCollection.Message.DATE_INCORRECT;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }

                try
                {
                    toDate1 = new DateTime(pyear, pmonth, pday, 0, 0, 0);
                }
                catch
                {
                    Result.result.code = 7;
                    Result.result.message = ClassCollection.Message.DATE_INCORRECT;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
            }

            var dateArrfromDate = fromDate.Split('-');
            Event.fromDate = Persia.Calendar.ConvertToGregorian(int.Parse(dateArrfromDate[0]), int.Parse(dateArrfromDate[1]), int.Parse(dateArrfromDate[2]), Persia.DateType.Persian);

            var dateArrtoDate = toDate.Split('-');
            Event.toDate = Persia.Calendar.ConvertToGregorian(int.Parse(dateArrtoDate[0]), int.Parse(dateArrtoDate[1]), int.Parse(dateArrtoDate[2]), Persia.DateType.Persian);

            if (Event.fromDate.CompareTo(Event.toDate) > 0)
            {
                Result.result.code = 8;
                Result.result.message = ClassCollection.Message.DATE_INCORRECT;
                Context.Response.Write(new JavaScriptSerializer().Serialize(Result));
                return;
            }

            List<long> tags = new List<long>();
            try
            {
                if (tagIDs.Length != 0)
                {
                    tags = new JavaScriptSerializer().Deserialize<List<string>>(tagIDs).Select(c => long.Parse(c)).ToList();
                    if (tags.Count() == 0)
                    {
                        Result.result.code = 9;
                        Result.result.message = ClassCollection.Message.TAGS_NOT_EXIST;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                        return;
                    }
                }
            }
            catch
            {
                Result.result.code = 10;
                Result.result.message = ClassCollection.Message.TAGS_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }
            foreach (var item in tags)
            {
                if (db.tagTbls.Any(c => c.ID == item) == false)
                {
                    Result.result.code = 11;
                    Result.result.message = ClassCollection.Message.TAGS_NOT_EXIST;
                    Context.Response.Write(js.Serialize(Result).ToLower());
                    return;
                }
            }

            

            if (fromDate != "")
            {
                Event.fromDate = fromDate1;
            }
            if (toDate != "")
            {
                Event.toDate = toDate1;
            }

            Event.title = title;
            Event.isBlock = true;
            Event.mUserID = userID;
            Event.regDate = dt;
            Event.viewCount = 0;
            db.eventsTbls.InsertOnSubmit(Event);
            db.SubmitChanges();

            foreach (var item in tags)
            {
                var tag = new eventTagTbl();
                tag.eventID = Event.ID;
                tag.tagID = item;

                db.eventTagTbls.InsertOnSubmit(tag);
                db.SubmitChanges();
            }

            //new service.pushservice().XsendPush("1", "news", news.ID + "", news.title, "all", "false");
            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Result.value = Event.ID;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }

        [WebMethod(Description = "این تابع وظیفه تغییر رویداد را بر عهده دارد")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void editEvent(string key, long EventID, long userID, string title, string fromDate, string toDate, string tagIDs)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.LongResult Result = new ClassCollection.Model.LongResult();
            Result.result = new Model.Result();
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            if (db.mUserTbls.Any(c => c.ID == userID) == false)
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.USER_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }
            if (db.mUserTbls.Single(c => c.ID == userID).isBlock == true)
            {
                Result.result.code = 2;
                Result.result.message = ClassCollection.Message.USER_BLOCKED;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (!db.eventsTbls.Any(c => c.ID == EventID && c.mUserID.Value == userID))
            {
                Result.result.code = 3;
                Result.result.message = ClassCollection.Message.EVENTS_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            var Event = db.eventsTbls.Single(c => c.ID == EventID);

            title = title.TrimStart().TrimEnd();
            if (title.Length == 0)
            {
                title = Event.title;
            }

            fromDate = fromDate.Trim();
            int pyear = 0, pmonth = 0, pday = 0;
            var DateArray = fromDate.Split('-');
            if (DateArray.Count() != 3
               || !int.TryParse(DateArray[0], out pyear)
               || !int.TryParse(DateArray[1], out pmonth)
               || !int.TryParse(DateArray[2], out pday))
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.DATE_INCORRECT;
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            toDate = toDate.Trim();
            DateArray = toDate.Split('-');
            if (DateArray.Count() != 3
               || !int.TryParse(DateArray[0], out pyear)
               || !int.TryParse(DateArray[1], out pmonth)
               || !int.TryParse(DateArray[2], out pday))
            {
                Result.result.code = 4;
                Result.result.message = ClassCollection.Message.DATE_INCORRECT;
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            var dateArrfromDate = fromDate.Split('-');
            Event.fromDate = Persia.Calendar.ConvertToGregorian(int.Parse(dateArrfromDate[0]), int.Parse(dateArrfromDate[1]), int.Parse(dateArrfromDate[2]), Persia.DateType.Persian);

            var dateArrtoDate = toDate.Split('-');
            Event.toDate = Persia.Calendar.ConvertToGregorian(int.Parse(dateArrtoDate[0]), int.Parse(dateArrtoDate[1]), int.Parse(dateArrtoDate[2]), Persia.DateType.Persian);

            if (Event.fromDate.CompareTo(Event.toDate) > 0)
            {
                Result.result.code = 5;
                Result.result.message = ClassCollection.Message.DATE_INCORRECT;
                Context.Response.Write(new JavaScriptSerializer().Serialize(Result));
                return;
            }

            List<long> tags = new List<long>();
            bool tagchange = false;

            try
            {
                if (tagIDs.Length != 0)
                {
                    tags = new JavaScriptSerializer().Deserialize<List<string>>(tagIDs).Select(c => long.Parse(c)).ToList();
                    if (tags.Count() == 0)
                    {
                        tagchange = false;
                    }
                    else
                    {
                        tagchange = true;
                    }
                }
                else
                {
                    tagchange = false;
                }
            }
            catch
            {
                Result.result.code = 6;
                Result.result.message = ClassCollection.Message.TAGS_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (tagchange == true)
            {
                foreach (var item in tags)
                {
                    if (db.tagTbls.Any(c => c.ID == item) == false)
                    {
                        Result.result.code = 7;
                        Result.result.message = ClassCollection.Message.TAGS_NOT_EXIST;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                        return;
                    }
                }

                var oldTags = db.eventTagTbls.Where(c => c.eventID == EventID);
                foreach (var item in oldTags)
                {
                    db.eventTagTbls.DeleteOnSubmit(item);
                    db.SubmitChanges();
                }
            }

            Event.title = title;
            Event.isBlock = true;
            db.SubmitChanges();


            if (tagchange == true)
            {
                foreach (var item in tags)
                {
                    var tag = new eventTagTbl();
                    tag.eventID = Event.ID;
                    tag.tagID = item;

                    db.eventTagTbls.InsertOnSubmit(tag);
                    db.SubmitChanges();
                }
            }

            //new service.pushservice().XsendPush("1", "news", news.ID + "", news.title, "all", "false");
            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }

        [WebMethod(Description = "این تابع وظیفه درج جزییات رویداد را بر عهده دارد")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void registerEventContent(string key, long EventID, int contentType, string value)
        {
            ClassCollection.Model.LongResult Result = new ClassCollection.Model.LongResult();
            Result.result = new Model.Result();
            JavaScriptSerializer js = new JavaScriptSerializer();
            DateTime dt = new DateTime();
            dt = DateTime.Now;
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(js.Serialize(Result));
                return;
            }
            if (!db.eventsTbls.Any(c => c.ID == EventID))
            {
                Result.result.code = 3;
                Result.result.message = ClassCollection.Message.EVENTS_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (contentType != 0 && contentType != 1 && contentType != 2)
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.EVENTContent_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (value.Length == 0)
            {
                Result.result.code = 2;
                Result.result.message = ClassCollection.Message.TEXT_INCORRECT;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            var tmp = new eventsContentTbl();

            if (contentType == 0)
            {
                tmp.value = "<p>" + value + "</p>";
            }
            if (contentType == 1 || contentType == 2)
            {
                try
                {
                    var totalPath = HttpContext.Current.Server.MapPath(ClassCollection.Methods.getfilePath()) + value;
                    if (!System.IO.File.Exists(totalPath))
                    {
                        Result.result.code = 4;
                        Result.result.message = ClassCollection.Message.ERROR_EXIST_FILE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                        return;
                    }
                    var ext = value.Split('.').Last();
                    var fileData = File.ReadAllBytes(totalPath);
                    File.Delete(totalPath);

                    var filename = "appinno" + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N") + "." + ext;
                    string fpath = "";
                    tmp.value = filename;
                    if (contentType == 1)
                    {
                        fpath = Server.MapPath(ClassCollection.Methods.geteventsImagesPath() + filename);
                    }
                    else if (contentType == 2)
                    {
                        fpath = Server.MapPath(ClassCollection.Methods.geteventsVideosPath() + filename);
                    }
                    try
                    {
                        File.WriteAllBytes(fpath, fileData);
                    }
                    catch
                    {
                        Result.result.code = 5;
                        Result.result.message = ClassCollection.Message.ERROR_SAVE_FILE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                        return;
                    }
                }
                catch
                {
                    Result.result.code = 6;
                    Result.result.message = ClassCollection.Message.ERROR_TRANSFER_FILE;
                    Context.Response.Write(js.Serialize(Result).ToLower());
                    return;
                }
            }
            tmp.eventsID = EventID;
            tmp.contentType = contentType;
            if (!db.eventsContentTbls.Any(c => c.eventsID == EventID))
            {
                tmp.pri = 0;
            }
            else
            {
                tmp.pri = db.eventsContentTbls.Where(c => c.eventsID == EventID).Max(c => c.pri) + 1;
            }
            tmp.regDate = dt;

            db.eventsContentTbls.InsertOnSubmit(tmp);
            db.SubmitChanges();

            //new service.pushservice().XsendPush("1", "news", news.ID + "", news.title, "all", "false");
            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Result.value = tmp.ID;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }

        [WebMethod(Description = "این تابع وظیفه حذف جزییات رویداد را بر عهده دارد")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void deleteEventContent(string key, long contentID)
        {
            ClassCollection.Model.Result Result = new ClassCollection.Model.Result();
            JavaScriptSerializer js = new JavaScriptSerializer();
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.code = -1;
                Result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            if (!db.eventsContentTbls.Any(c => c.ID == contentID))
            {
                Result.code = 1;
                Result.message = ClassCollection.Message.EVENTContent_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            var deletingPar = db.eventsContentTbls.Single(c => c.ID == contentID);

            if (deletingPar.contentType == 1)
            {
                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(ClassCollection.Methods.geteventsImagesPath() + deletingPar.value)))
                {
                    try
                    {
                        System.IO.File.Delete(HttpContext.Current.Server.MapPath(ClassCollection.Methods.geteventsImagesPath() + deletingPar.value));
                    }
                    catch
                    {
                        Result.code = 2;
                        Result.message = ClassCollection.Message.ERROR_DELETE_IMAGE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                    }
                }
            }
            else if (deletingPar.contentType == 2)
            {
                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(ClassCollection.Methods.geteventsVideosPath() + deletingPar.value)))
                {
                    try
                    {
                        System.IO.File.Delete(HttpContext.Current.Server.MapPath(ClassCollection.Methods.geteventsVideosPath() + deletingPar.value));
                    }
                    catch
                    {
                        Result.code = 3;
                        Result.message = ClassCollection.Message.ERROR_DELETE_VIDEO;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                    }
                }
            }
            db.eventsContentTbls.DeleteOnSubmit(deletingPar);
            db.SubmitChanges();

            Result.code = 0;
            Result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }

        [WebMethod(Description = "این تابع وظیفه تغییر جزییات رویداد را بر عهده دارد")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void editEventContent(string key, long contentID, int contentType, string value)
        {
            ClassCollection.Model.LongResult Result = new ClassCollection.Model.LongResult();
            Result.result = new Model.Result();
            JavaScriptSerializer js = new JavaScriptSerializer();
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            if (!db.eventsContentTbls.Any(c => c.ID == contentID))
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.EVENTContent_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            var eventcontent = db.eventsContentTbls.Single(c => c.ID == contentID);

            if (value.Length == 0)
            {
                value = eventcontent.value;
            }

            if (contentType.ToString().Length != 0)
            {
                if (contentType != 0 && contentType != 1 && contentType != 2)
                {
                    Result.result.code = 2;
                    Result.result.message = ClassCollection.Message.EVENTContent_INCORRECT;
                    Context.Response.Write(js.Serialize(Result).ToLower());
                    return;
                }
            }
            else
            {
                contentType = eventcontent.contentType;
            }

            if (eventcontent.contentType == 1)
            {
                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(ClassCollection.Methods.geteventsImagesPath() + eventcontent.value)) && eventcontent.value != value)
                {
                    try
                    {
                        System.IO.File.Delete(HttpContext.Current.Server.MapPath(ClassCollection.Methods.geteventsImagesPath() + eventcontent.value));
                    }
                    catch
                    {
                        Result.result.code = 3;
                        Result.result.message = ClassCollection.Message.ERROR_DELETE_IMAGE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                    }
                }
            }
            else if (eventcontent.contentType == 2)
            {
                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(ClassCollection.Methods.geteventsVideosPath() + eventcontent.value)) && eventcontent.value != value)
                {
                    try
                    {
                        System.IO.File.Delete(HttpContext.Current.Server.MapPath(ClassCollection.Methods.geteventsVideosPath() + eventcontent.value));
                    }
                    catch
                    {
                        Result.result.code = 4;
                        Result.result.message = ClassCollection.Message.ERROR_DELETE_VIDEO;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                    }
                }
            }

            if (contentType == 1 || contentType == 2)
            {
                try
                {
                    var totalPath = HttpContext.Current.Server.MapPath(ClassCollection.Methods.getfilePath()) + value;
                    if (!System.IO.File.Exists(totalPath))
                    {
                        Result.result.code = 4;
                        Result.result.message = ClassCollection.Message.ERROR_EXIST_FILE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                        return;
                    }
                    var ext = value.Split('.').Last();
                    var fileData = File.ReadAllBytes(totalPath);
                    File.Delete(totalPath);

                    var filename = "appinno" + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N") + "." + ext;
                    string fpath = "";
                    value = filename;
                    if (contentType == 1)
                    {
                        fpath = Server.MapPath(ClassCollection.Methods.geteventsImagesPath() + filename);
                    }
                    else if (contentType == 2)
                    {
                        fpath = Server.MapPath(ClassCollection.Methods.geteventsVideosPath() + filename);
                    }
                    try
                    {
                        File.WriteAllBytes(fpath, fileData);
                    }
                    catch
                    {
                        Result.result.code = 5;
                        Result.result.message = ClassCollection.Message.ERROR_SAVE_FILE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                        return;
                    }
                }
                catch
                {
                    Result.result.code = 6;
                    Result.result.message = ClassCollection.Message.ERROR_TRANSFER_FILE;
                    Context.Response.Write(js.Serialize(Result).ToLower());
                    return;
                }
            }

            eventcontent.contentType = contentType;
            eventcontent.value = value;
            db.SubmitChanges();

            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }
        //soheila-end-registerEvent
        #endregion

        #region registerIO 
        //soheila-start-registerIO
        [WebMethod(Description = "این تابع وظیفه درج سازمان جدید را بر عهده دارد")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void registerIO(string key, long userID, string title, string publishDate, string tagIDs)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.LongResult Result = new ClassCollection.Model.LongResult();
            Result.result = new Model.Result();
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            if (db.mUserTbls.Any(c => c.ID == userID) == false)
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.USER_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }
            if (db.mUserTbls.Single(c => c.ID == userID).isBlock == true)
            {
                Result.result.code = 2;
                Result.result.message = ClassCollection.Message.USER_BLOCKED;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            title = title.TrimStart().TrimEnd();
            if (title.Length == 0)
            {
                Result.result.code = 3;
                Result.result.message = ClassCollection.Message.TEXT_INCORRECT;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            
            var publishDate1 = new DateTime();
            if (publishDate != "")
            {
                publishDate = publishDate.Trim();
                int pyear = 0, pmonth = 0, pday = 0;
                var DateArray = publishDate.Split('-');
                if (DateArray.Count() != 3
                   || !int.TryParse(DateArray[0], out pyear)
                   || !int.TryParse(DateArray[1], out pmonth)
                   || !int.TryParse(DateArray[2], out pday))
                {
                    Result.result.code = 4;
                    Result.result.message = ClassCollection.Message.DATE_INCORRECT;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }

                try
                {
                    publishDate1 = new DateTime(pyear, pmonth, pday, 0, 0, 0);
                }
                catch
                {
                    Result.result.code = 2;
                    Result.result.message = ClassCollection.Message.DATE_INCORRECT;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
            }
            
            List<long> tags = new List<long>();
            try
            {
                if (tagIDs.Length != 0)
                {
                    tags = new JavaScriptSerializer().Deserialize<List<string>>(tagIDs).Select(c => long.Parse(c)).ToList();
                    if (tags.Count() == 0)
                    {
                        Result.result.code = 5;
                        Result.result.message = ClassCollection.Message.TAGS_NOT_EXIST;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                        return;
                    }
                }
            }
            catch
            {
                Result.result.code = 6;
                Result.result.message = ClassCollection.Message.TAGS_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }
            foreach (var item in tags)
            {
                if (db.tagTbls.Any(c => c.ID == item) == false)
                {
                    Result.result.code = 7;
                    Result.result.message = ClassCollection.Message.TAGS_NOT_EXIST;
                    Context.Response.Write(js.Serialize(Result).ToLower());
                    return;
                }
            }
            var io = new ioTbl();
            var dt = new DateTime();
            dt = DateTime.Now;
            io.title = title;
            io.isBlock = true;
            io.isPublished = true;
            io.mUserID = userID;
            if (publishDate != "")
            {
                io.publishDate = publishDate1;
            }
            else
            {
                io.publishDate = dt.Date;
            }
            
            io.regDate = dt;
            io.viewCount = 0;
            db.ioTbls.InsertOnSubmit(io);
            db.SubmitChanges();

            foreach (var item in tags)
            {
                var tag = new ioTagTbl();
                tag.ioID = io.ID;
                tag.tagID = item;

                db.ioTagTbls.InsertOnSubmit(tag);
                db.SubmitChanges();
            }

            //new service.pushservice().XsendPush("1", "news", news.ID + "", news.title, "all", "false");
            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Result.value = io.ID;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }

        [WebMethod(Description = "این تابع وظیفه تغییر سازمان را بر عهده دارد")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void editIO(string key, long IOID, long userID, string title, string publishDate, string tagIDs)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.LongResult Result = new ClassCollection.Model.LongResult();
            Result.result = new Model.Result();
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(js.Serialize(Result));
                return;
            }


            if (db.mUserTbls.Any(c => c.ID == userID) == false)
            {
                Result.result.code = 2;
                Result.result.message = ClassCollection.Message.USER_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (db.mUserTbls.Single(c => c.ID == userID).isBlock == true)
            {
                Result.result.code = 3;
                Result.result.message = ClassCollection.Message.USER_BLOCKED;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (!db.ioTbls.Any(c => c.ID == IOID && c.mUserID.Value == userID))
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.IO_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }


            var IO = db.ioTbls.Single(c => c.ID == IOID);

            title = title.TrimStart().TrimEnd();
            if (title.Length == 0)
            {
                title = IO.title;
            }

            publishDate = publishDate.Trim();
            DateTime publishdate = IO.publishDate;
            if (publishDate.Length != 0)
            {
                int pyear = 0, pmonth = 0, pday = 0;
                var DateArray = publishDate.Split('-');
                if (DateArray.Count() != 3
                    || !int.TryParse(DateArray[0], out pyear)
                    || !int.TryParse(DateArray[1], out pmonth)
                    || !int.TryParse(DateArray[2], out pday))
                {
                    Result.result.code = 4;
                    Result.result.message = ClassCollection.Message.DATE_INCORRECT;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                var dateArr = publishDate.Split('-');
                publishdate = Persia.Calendar.ConvertToGregorian(int.Parse(dateArr[0]), int.Parse(dateArr[1]), int.Parse(dateArr[2]), Persia.DateType.Persian);
            }

            List<long> tags = new List<long>();
            bool tagchange = false;

            try
            {
                if (tagIDs.Length != 0)
                {
                    tags = new JavaScriptSerializer().Deserialize<List<string>>(tagIDs).Select(c => long.Parse(c)).ToList();
                    if (tags.Count() == 0)
                    {
                        tagchange = false;
                    }
                    else
                    {
                        tagchange = true;
                    }
                }
                else
                {
                    tagchange = false;
                }
            }
            catch
            {
                Result.result.code = 5;
                Result.result.message = ClassCollection.Message.TAGS_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (tagchange == true)
            {
                foreach (var item in tags)
                {
                    if (db.tagTbls.Any(c => c.ID == item) == false)
                    {
                        Result.result.code = 6;
                        Result.result.message = ClassCollection.Message.TAGS_NOT_EXIST;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                        return;
                    }
                }

                var oldTags = db.ioTagTbls.Where(c => c.ioID == IOID);
                foreach (var item in oldTags)
                {
                    db.ioTagTbls.DeleteOnSubmit(item);
                    db.SubmitChanges();
                }
            }

            IO.title = title;
            IO.isBlock = true;
            IO.isPublished = true;
            IO.publishDate = publishdate;
            db.SubmitChanges();

            if (tagchange == true)
            {
                foreach (var item in tags)
                {
                    var tag = new ioTagTbl();
                    tag.ioID = IO.ID;
                    tag.tagID = item;

                    db.ioTagTbls.InsertOnSubmit(tag);
                    db.SubmitChanges();
                }
            }

            //new service.pushservice().XsendPush("1", "news", news.ID + "", news.title, "all", "false");
            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }

        [WebMethod(Description = "این تابع وظیفه درج جزییات سازمان را بر عهده دارد")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void registerIOContent(string key, long IOID, int contentType, string value)
        {
            ClassCollection.Model.LongResult Result = new ClassCollection.Model.LongResult();
            Result.result = new Model.Result();
            JavaScriptSerializer js = new JavaScriptSerializer();
            DateTime dt = new DateTime();
            dt = DateTime.Now;
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(js.Serialize(Result));
                return;
            }
            if (!db.ioTbls.Any(c => c.ID == IOID))
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.IO_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (contentType != 0 && contentType != 1 && contentType != 2)
            {
                Result.result.code = 2;
                Result.result.message = ClassCollection.Message.IOContent_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (value.Length == 0)
            {
                Result.result.code = 3;
                Result.result.message = ClassCollection.Message.TEXT_INCORRECT;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            var tmp = new ioContentTbl();

            if (contentType == 0)
            {
                tmp.value = "<p>" + value + "</p>";
            }
            if (contentType == 1 || contentType == 2)
            {
                try
                {
                    var totalPath = HttpContext.Current.Server.MapPath(ClassCollection.Methods.getfilePath()) + value;
                    if (!System.IO.File.Exists(totalPath))
                    {
                        Result.result.code = 4;
                        Result.result.message = ClassCollection.Message.ERROR_EXIST_FILE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                        return;
                    }
                    var ext = value.Split('.').Last();
                    var fileData = File.ReadAllBytes(totalPath);
                    File.Delete(totalPath);

                    var filename = "appinno" + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N") + "." + ext;
                    string fpath = "";
                    tmp.value = filename;
                    if (contentType == 1)
                    {
                        fpath = Server.MapPath(ClassCollection.Methods.getioImagesPath() + filename);
                    }
                    else if (contentType == 2)
                    {
                        fpath = Server.MapPath(ClassCollection.Methods.getioVideosPath() + filename);
                    }
                    try
                    {
                        File.WriteAllBytes(fpath, fileData);
                    }
                    catch
                    {
                        Result.result.code = 5;
                        Result.result.message = ClassCollection.Message.ERROR_SAVE_FILE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                        return;
                    }
                }
                catch
                {
                    Result.result.code = 6;
                    Result.result.message = ClassCollection.Message.ERROR_TRANSFER_FILE;
                    Context.Response.Write(js.Serialize(Result).ToLower());
                    return;
                }
            }
            tmp.ioID = IOID;
            tmp.contentType = contentType;
            if (!db.ioContentTbls.Any(c => c.ioID == IOID))
            {
                tmp.pri = 0;
            }
            else
            {
                tmp.pri = db.ioContentTbls.Where(c => c.ioID == IOID).Max(c => c.pri) + 1;
            }
            tmp.regDate = dt;

            db.ioContentTbls.InsertOnSubmit(tmp);
            db.SubmitChanges();

            //new service.pushservice().XsendPush("1", "news", news.ID + "", news.title, "all", "false");
            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Result.value = tmp.ID;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }

        [WebMethod(Description = "این تابع وظیفه حذف جزییات سازمان را بر عهده دارد")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void deleteIOContent(string key, long contentID)
        {
            ClassCollection.Model.Result Result = new ClassCollection.Model.Result();
            JavaScriptSerializer js = new JavaScriptSerializer();
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.code = -1;
                Result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            if (!db.ioContentTbls.Any(c => c.ID == contentID))
            {
                Result.code = 1;
                Result.message = ClassCollection.Message.IOContent_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            var deletingPar = db.ioContentTbls.Single(c => c.ID == contentID);

            if (deletingPar.contentType == 1)
            {
                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getioImagesPath() + deletingPar.value)))
                {
                    try
                    {
                        System.IO.File.Delete(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getioImagesPath() + deletingPar.value));
                    }
                    catch
                    {
                        Result.code = 2;
                        Result.message = ClassCollection.Message.ERROR_DELETE_IMAGE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                    }
                }
            }
            else if (deletingPar.contentType == 2)
            {
                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getioVideosPath() + deletingPar.value)))
                {
                    try
                    {
                        System.IO.File.Delete(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getioVideosPath() + deletingPar.value));
                    }
                    catch
                    {
                        Result.code = 3;
                        Result.message = ClassCollection.Message.ERROR_DELETE_VIDEO;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                    }
                }
            }
            db.ioContentTbls.DeleteOnSubmit(deletingPar);
            db.SubmitChanges();

            Result.code = 0;
            Result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }

        [WebMethod(Description = "این تابع وظیفه تغییر جزییات سازمان را بر عهده دارد")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void editIOContent(string key, long contentID, int contentType, string value)
        {
            ClassCollection.Model.LongResult Result = new ClassCollection.Model.LongResult();
            Result.result = new Model.Result();
            JavaScriptSerializer js = new JavaScriptSerializer();
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            if (!db.ioContentTbls.Any(c => c.ID == contentID))
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.IOContent_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            var IOcontent = db.ioContentTbls.Single(c => c.ID == contentID);

            if (value.Length == 0)
            {
                value = IOcontent.value;
            }

            if (contentType.ToString().Length != 0)
            {
                if (contentType != 0 && contentType != 1 && contentType != 2)
                {
                    Result.result.code = 2;
                    Result.result.message = ClassCollection.Message.IOContent_INCORRECT;
                    Context.Response.Write(js.Serialize(Result).ToLower());
                    return;
                }
            }
            else
            {
                contentType = IOcontent.contentType;
            }

            if (IOcontent.contentType == 1)
            {
                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getioImagesPath() + IOcontent.value)) && IOcontent.value != value)
                {
                    try
                    {
                        System.IO.File.Delete(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getioImagesPath() + IOcontent.value));
                    }
                    catch
                    {
                        Result.result.code = 3;
                        Result.result.message = ClassCollection.Message.ERROR_DELETE_IMAGE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                    }
                }
            }
            else if (IOcontent.contentType == 2)
            {
                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getioVideosPath() + IOcontent.value)) && IOcontent.value != value)
                {
                    try
                    {
                        System.IO.File.Delete(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getioVideosPath() + IOcontent.value));
                    }
                    catch
                    {
                        Result.result.code = 4;
                        Result.result.message = ClassCollection.Message.ERROR_DELETE_VIDEO;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                    }
                }
            }
            if (contentType == 1 || contentType == 2)
            {
                try
                {
                    var totalPath = HttpContext.Current.Server.MapPath(ClassCollection.Methods.getfilePath()) + value;
                    if (!System.IO.File.Exists(totalPath))
                    {
                        Result.result.code = 5;
                        Result.result.message = ClassCollection.Message.ERROR_EXIST_FILE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                        return;
                    }
                    var ext = value.Split('.').Last();
                    var fileData = File.ReadAllBytes(totalPath);
                    File.Delete(totalPath);

                    var filename = "appinno" + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N") + "." + ext;
                    string fpath = "";
                    value = filename;
                    if (contentType == 1)
                    {
                        fpath = Server.MapPath(ClassCollection.Methods.getioImagesPath() + filename);
                    }
                    else if (contentType == 2)
                    {
                        fpath = Server.MapPath(ClassCollection.Methods.getioVideosPath() + filename);
                    }
                    try
                    {
                        File.WriteAllBytes(fpath, fileData);
                    }
                    catch
                    {
                        Result.result.code = 6;
                        Result.result.message = ClassCollection.Message.ERROR_SAVE_FILE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                        return;
                    }
                }
                catch
                {
                    Result.result.code = 7;
                    Result.result.message = ClassCollection.Message.ERROR_TRANSFER_FILE;
                    Context.Response.Write(js.Serialize(Result).ToLower());
                    return;
                }
            }

            IOcontent.contentType = contentType;
            IOcontent.value = value;
            db.SubmitChanges();

            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }
        //soheila-end-registerIO
        #endregion

        #region registerPublication
        //soheila-start-registerPublication
        [WebMethod(Description = "این تابع وظیفه درج انتشارات جدید را بر عهده دارد")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void registerPublication(string key, long userID, string title, string publishDate, string tagIDs)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.LongResult Result = new ClassCollection.Model.LongResult();
            Result.result = new Model.Result();
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            if (db.mUserTbls.Any(c => c.ID == userID) == false)
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.USER_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }
            if (db.mUserTbls.Single(c => c.ID == userID).isBlock == true)
            {
                Result.result.code = 2;
                Result.result.message = ClassCollection.Message.USER_BLOCKED;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            title = title.TrimStart().TrimEnd();
            if (title.Length == 0)
            {
                Result.result.code = 3;
                Result.result.message = ClassCollection.Message.TEXT_INCORRECT;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            var publishDate1 = new DateTime();
            if (publishDate != "")
            {
                publishDate = publishDate.Trim();
                int pyear = 0, pmonth = 0, pday = 0;
                var DateArray = publishDate.Split('-');
                if (DateArray.Count() != 3
                   || !int.TryParse(DateArray[0], out pyear)
                   || !int.TryParse(DateArray[1], out pmonth)
                   || !int.TryParse(DateArray[2], out pday))
                {
                    Result.result.code = 4;
                    Result.result.message = ClassCollection.Message.DATE_INCORRECT;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }

                try
                {
                    publishDate1 = new DateTime(pyear, pmonth, pday, 0, 0, 0);
                }
                catch
                {
                    Result.result.code = 2;
                    Result.result.message = ClassCollection.Message.DATE_INCORRECT;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
            }

            List<long> tags = new List<long>();
            try
            {
                if (tagIDs.Length != 0)
                {
                    tags = new JavaScriptSerializer().Deserialize<List<string>>(tagIDs).Select(c => long.Parse(c)).ToList();
                    if (tags.Count() == 0)
                    {
                        Result.result.code = 5;
                        Result.result.message = ClassCollection.Message.TAGS_NOT_EXIST;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                        return;
                    }
                }
            }
            catch
            {
                Result.result.code = 6;
                Result.result.message = ClassCollection.Message.TAGS_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }
            foreach (var item in tags)
            {
                if (db.tagTbls.Any(c => c.ID == item) == false)
                {
                    Result.result.code = 7;
                    Result.result.message = ClassCollection.Message.TAGS_NOT_EXIST;
                    Context.Response.Write(js.Serialize(Result).ToLower());
                    return;
                }
            }
            var publication = new publicationTbl();
            var dt = new DateTime();
            dt = DateTime.Now;
            publication.title = title;
            publication.isBlock = true;
            publication.isPublished = false;
            publication.mUserID = userID;
            if (publishDate != "")
            {
                publication.publishDate = publishDate1;
            }
            else
            {
                publication.publishDate = dt.Date;
            }
            publication.regDate = dt;
            publication.viewCount = 0;
            db.publicationTbls.InsertOnSubmit(publication);
            db.SubmitChanges();

            foreach (var item in tags)
            {
                var tag = new publicationTagTbl();
                tag.publicationID = publication.ID;
                tag.tagID = item;

                db.publicationTagTbls.InsertOnSubmit(tag);
                db.SubmitChanges();
            }

            //new service.pushservice().XsendPush("1", "news", news.ID + "", news.title, "all", "false");
            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Result.value = publication.ID;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }

        [WebMethod(Description = "این تابع وظیفه تغییر انتشارات را بر عهده دارد")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void editPublication(string key, long PublicationID, long userID, string title, string publishDate, string tagIDs)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.LongResult Result = new ClassCollection.Model.LongResult();
            Result.result = new Model.Result();
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            if (db.mUserTbls.Any(c => c.ID == userID) == false)
            {
                Result.result.code = 2;
                Result.result.message = ClassCollection.Message.USER_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }
            if (db.mUserTbls.Single(c => c.ID == userID).isBlock == true)
            {
                Result.result.code = 3;
                Result.result.message = ClassCollection.Message.USER_BLOCKED;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (!db.publicationTbls.Any(c => c.ID == PublicationID && c.mUserID.Value == userID))
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.PUBLICATIONS_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            var Publication = db.publicationTbls.Single(c => c.ID == PublicationID);

            title = title.TrimStart().TrimEnd();
            if (title.Length == 0)
            {
                title = Publication.title;
            }

            publishDate = publishDate.Trim();
            DateTime publishdate = Publication.publishDate;
            if (publishDate.Length != 0)
            {
                int pyear = 0, pmonth = 0, pday = 0;
                var DateArray = publishDate.Split('-');
                if (DateArray.Count() != 3
                    || !int.TryParse(DateArray[0], out pyear)
                    || !int.TryParse(DateArray[1], out pmonth)
                    || !int.TryParse(DateArray[2], out pday))
                {
                    Result.result.code = 4;
                    Result.result.message = ClassCollection.Message.DATE_INCORRECT;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                var dateArr = publishDate.Split('-');
                publishdate = Persia.Calendar.ConvertToGregorian(int.Parse(dateArr[0]), int.Parse(dateArr[1]), int.Parse(dateArr[2]), Persia.DateType.Persian);
            }

            List<long> tags = new List<long>();
            bool tagchange = false;

            try
            {
                if (tagIDs.Length != 0)
                {
                    tags = new JavaScriptSerializer().Deserialize<List<string>>(tagIDs).Select(c => long.Parse(c)).ToList();
                    if (tags.Count() == 0)
                    {
                        tagchange = false;
                    }
                    else
                    {
                        tagchange = true;
                    }
                }
                else
                {
                    tagchange = false;
                }
            }
            catch
            {
                Result.result.code = 5;
                Result.result.message = ClassCollection.Message.TAGS_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (tagchange == true)
            {
                foreach (var item in tags)
                {
                    if (db.tagTbls.Any(c => c.ID == item) == false)
                    {
                        Result.result.code = 6;
                        Result.result.message = ClassCollection.Message.TAGS_NOT_EXIST;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                        return;
                    }
                }

                var oldTags = db.publicationTagTbls.Where(c => c.publicationID == PublicationID);
                foreach (var item in oldTags)
                {
                    db.publicationTagTbls.DeleteOnSubmit(item);
                    db.SubmitChanges();
                }
            }

            Publication.title = title;
            Publication.isBlock = true;
            Publication.isPublished = false;
            Publication.publishDate = publishdate;
            db.SubmitChanges();

            if (tagchange == true)
            {
                foreach (var item in tags)
                {
                    var tag = new publicationTagTbl();
                    tag.publicationID = Publication.ID;
                    tag.tagID = item;

                    db.publicationTagTbls.InsertOnSubmit(tag);
                    db.SubmitChanges();
                }
            }

            //new service.pushservice().XsendPush("1", "news", news.ID + "", news.title, "all", "false");
            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }

        [WebMethod(Description = "این تابع وظیفه درج جزییات انتشارات را بر عهده دارد")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void registerPublicationContent(string key, long PublicationID, int contentType, string value)
        {
            ClassCollection.Model.LongResult Result = new ClassCollection.Model.LongResult();
            Result.result = new Model.Result();
            JavaScriptSerializer js = new JavaScriptSerializer();
            DateTime dt = new DateTime();
            dt = DateTime.Now;
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(js.Serialize(Result));
                return;
            }
            if (!db.publicationTbls.Any(c => c.ID == PublicationID))
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.PUBLICATIONS_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (contentType != 0 && contentType != 1 && contentType != 2)
            {
                Result.result.code = 2;
                Result.result.message = ClassCollection.Message.PUBContent_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (value.Length == 0)
            {
                Result.result.code = 3;
                Result.result.message = ClassCollection.Message.TEXT_INCORRECT;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            var tmp = new publicationContentTbl();

            if (contentType == 0)
            {
                tmp.value = "<p>" + value + "</p>";
            }
            if (contentType == 1 || contentType == 2)
            {
                try
                {
                    var totalPath = HttpContext.Current.Server.MapPath(ClassCollection.Methods.getfilePath()) + value;
                    if (!System.IO.File.Exists(totalPath))
                    {
                        Result.result.code = 4;
                        Result.result.message = ClassCollection.Message.ERROR_EXIST_FILE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                        return;
                    }
                    var ext = value.Split('.').Last();
                    var fileData = File.ReadAllBytes(totalPath);
                    File.Delete(totalPath);

                    var filename = "appinno" + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N") + "." + ext;
                    string fpath = "";
                    tmp.value = filename;
                    if (contentType == 1)
                    {
                        fpath = Server.MapPath(ClassCollection.Methods.getpublicationImagesPath() + filename);
                    }
                    else if (contentType == 2)
                    {
                        fpath = Server.MapPath(ClassCollection.Methods.getpublicationVideosPath() + filename);
                    }
                    try
                    {
                        File.WriteAllBytes(fpath, fileData);
                    }
                    catch
                    {
                        Result.result.code = 5;
                        Result.result.message = ClassCollection.Message.ERROR_SAVE_FILE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                        return;
                    }
                }
                catch
                {
                    Result.result.code = 6;
                    Result.result.message = ClassCollection.Message.ERROR_TRANSFER_FILE;
                    Context.Response.Write(js.Serialize(Result).ToLower());
                    return;
                }
            }
            tmp.publicationsID = PublicationID;
            tmp.contentType = contentType;
            if (!db.publicationContentTbls.Any(c => c.publicationsID == PublicationID))
            {
                tmp.pri = 0;
            }
            else
            {
                tmp.pri = db.publicationContentTbls.Where(c => c.publicationsID == PublicationID).Max(c => c.pri) + 1;
            }
            tmp.regDate = dt;

            db.publicationContentTbls.InsertOnSubmit(tmp);
            db.SubmitChanges();

            //new service.pushservice().XsendPush("1", "news", news.ID + "", news.title, "all", "false");
            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Result.value = tmp.ID;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }

        [WebMethod(Description = "این تابع وظیفه حذف جزییات انتشارات را بر عهده دارد")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void deletePublicationContent(string key, long contentID)
        {
            ClassCollection.Model.Result Result = new ClassCollection.Model.Result();
            JavaScriptSerializer js = new JavaScriptSerializer();
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.code = -1;
                Result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            if (!db.publicationContentTbls.Any(c => c.ID == contentID))
            {
                Result.code = 1;
                Result.message = ClassCollection.Message.PUBContent_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            var deletingPar = db.publicationContentTbls.Single(c => c.ID == contentID);

            if (deletingPar.contentType == 1)
            {
                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getpublicationImagesPath() + deletingPar.value)))
                {
                    try
                    {
                        System.IO.File.Delete(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getpublicationImagesPath() + deletingPar.value));
                    }
                    catch
                    {
                        Result.code = 2;
                        Result.message = ClassCollection.Message.ERROR_DELETE_IMAGE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                    }
                }
            }
            else if (deletingPar.contentType == 2)
            {
                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getpublicationVideosPath() + deletingPar.value)))
                {
                    try
                    {
                        System.IO.File.Delete(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getpublicationVideosPath() + deletingPar.value));
                    }
                    catch
                    {
                        Result.code = 3;
                        Result.message = ClassCollection.Message.ERROR_DELETE_VIDEO;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                    }
                }
            }
            db.publicationContentTbls.DeleteOnSubmit(deletingPar);
            db.SubmitChanges();

            Result.code = 0;
            Result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }

        [WebMethod(Description = "این تابع وظیفه تغییر جزییات انتشارات را بر عهده دارد")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void editPublicationContent(string key, long contentID, int contentType, string value)
        {
            ClassCollection.Model.LongResult Result = new ClassCollection.Model.LongResult();
            Result.result = new Model.Result();
            JavaScriptSerializer js = new JavaScriptSerializer();
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            if (!db.publicationContentTbls.Any(c => c.ID == contentID))
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.PUBContent_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            var Publicationcontent = db.publicationContentTbls.Single(c => c.ID == contentID);

            if (value.Length == 0)
            {
                value = Publicationcontent.value;
            }

            if (contentType.ToString().Length != 0)
            {
                if (contentType != 0 && contentType != 1 && contentType != 2)
                {
                    Result.result.code = 2;
                    Result.result.message = ClassCollection.Message.PUBContent_INCORRECT;
                    Context.Response.Write(js.Serialize(Result).ToLower());
                    return;
                }
            }
            else
            {
                contentType = Publicationcontent.contentType;
            }

            if (Publicationcontent.contentType == 1)
            {
                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getpublicationImagesPath() + Publicationcontent.value)) && Publicationcontent.value != value)
                {
                    try
                    {
                        System.IO.File.Delete(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getpublicationImagesPath() + Publicationcontent.value));
                    }
                    catch
                    {
                        Result.result.code = 3;
                        Result.result.message = ClassCollection.Message.ERROR_DELETE_IMAGE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                    }
                }
            }
            else if (Publicationcontent.contentType == 2)
            {
                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getpublicationVideosPath() + Publicationcontent.value)) && Publicationcontent.value != value)
                {
                    try
                    {
                        System.IO.File.Delete(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getpublicationVideosPath() + Publicationcontent.value));
                    }
                    catch
                    {
                        Result.result.code = 4;
                        Result.result.message = ClassCollection.Message.ERROR_DELETE_VIDEO;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                    }
                }
            }
            if (contentType == 1 || contentType == 2)
            {
                try
                {
                    var totalPath = HttpContext.Current.Server.MapPath(ClassCollection.Methods.getfilePath()) + value;
                    if (!System.IO.File.Exists(totalPath))
                    {
                        Result.result.code = 5;
                        Result.result.message = ClassCollection.Message.ERROR_EXIST_FILE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                        return;
                    }
                    var ext = value.Split('.').Last();
                    var fileData = File.ReadAllBytes(totalPath);
                    File.Delete(totalPath);

                    var filename = "appinno" + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N") + "." + ext;
                    string fpath = "";
                    value = filename;
                    if (contentType == 1)
                    {
                        fpath = Server.MapPath(ClassCollection.Methods.getpublicationImagesPath() + filename);
                    }
                    else if (contentType == 2)
                    {
                        fpath = Server.MapPath(ClassCollection.Methods.getpublicationVideosPath() + filename);
                    }
                    try
                    {
                        File.WriteAllBytes(fpath, fileData);
                    }
                    catch
                    {
                        Result.result.code = 6;
                        Result.result.message = ClassCollection.Message.ERROR_SAVE_FILE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                        return;
                    }
                }
                catch
                {
                    Result.result.code = 7;
                    Result.result.message = ClassCollection.Message.ERROR_TRANSFER_FILE;
                    Context.Response.Write(js.Serialize(Result).ToLower());
                    return;
                }
            }
            Publicationcontent.contentType = contentType;
            Publicationcontent.value = value;
            db.SubmitChanges();

            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }
        //soheila-end-registerPublication
        #endregion

        #region registerDownload
        //soheila-start-registerDownload
        [WebMethod(Description = "این تابع وظیفه درج دانلود جدید را بر عهده دارد")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void registerDownload(string key, long userID, string title, string publishDate, string tagIDs)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.LongResult Result = new ClassCollection.Model.LongResult();
            Result.result = new Model.Result();
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            if (db.mUserTbls.Any(c => c.ID == userID) == false)
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.USER_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }
            if (db.mUserTbls.Single(c => c.ID == userID).isBlock == true)
            {
                Result.result.code = 2;
                Result.result.message = ClassCollection.Message.USER_BLOCKED;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            title = title.TrimStart().TrimEnd();
            if (title.Length == 0)
            {
                Result.result.code = 3;
                Result.result.message = ClassCollection.Message.TEXT_INCORRECT;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            var publishDate1 = new DateTime();
            if (publishDate != "")
            {
                publishDate = publishDate.Trim();
                int pyear = 0, pmonth = 0, pday = 0;
                var DateArray = publishDate.Split('-');
                if (DateArray.Count() != 3
                   || !int.TryParse(DateArray[0], out pyear)
                   || !int.TryParse(DateArray[1], out pmonth)
                   || !int.TryParse(DateArray[2], out pday))
                {
                    Result.result.code = 4;
                    Result.result.message = ClassCollection.Message.DATE_INCORRECT;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }

                try
                {
                    publishDate1 = new DateTime(pyear, pmonth, pday, 0, 0, 0);
                }
                catch
                {
                    Result.result.code = 2;
                    Result.result.message = ClassCollection.Message.DATE_INCORRECT;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
            }

            List<long> tags = new List<long>();
            try
            {
                if (tagIDs.Length != 0)
                {
                    tags = new JavaScriptSerializer().Deserialize<List<string>>(tagIDs).Select(c => long.Parse(c)).ToList();
                    if (tags.Count() == 0)
                    {
                        Result.result.code = 5;
                        Result.result.message = ClassCollection.Message.TAGS_NOT_EXIST;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                        return;
                    }
                }
            }
            catch
            {
                Result.result.code = 6;
                Result.result.message = ClassCollection.Message.TAGS_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }
            foreach (var item in tags)
            {
                if (db.tagTbls.Any(c => c.ID == item) == false)
                {
                    Result.result.code = 7;
                    Result.result.message = ClassCollection.Message.TAGS_NOT_EXIST;
                    Context.Response.Write(js.Serialize(Result).ToLower());
                    return;
                }
            }
            var download = new downloadTbl();
            var dt = new DateTime();
            dt = DateTime.Now;
            download.title = title;
            download.isBlock = true;
            download.isPublished = true;
            download.mUserID = userID;
            if (publishDate != "")
            {
                download.publishDate = publishDate1;
            }
            else
            {
                download.publishDate = dt.Date;
            }
            download.regDate = dt;
            download.viewCount = 0;
            db.downloadTbls.InsertOnSubmit(download);
            db.SubmitChanges();

            foreach (var item in tags)
            {
                var tag = new downloadTagTbl();
                tag.downloadID = download.ID;
                tag.tagID = item;

                db.downloadTagTbls.InsertOnSubmit(tag);
                db.SubmitChanges();
            }

            //new service.pushservice().XsendPush("1", "news", news.ID + "", news.title, "all", "false");
            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Result.value = download.ID;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }

        [WebMethod(Description = "این تابع وظیفه تغییر دانلود را بر عهده دارد")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void editDownload(string key, long downloadID, long userID, string title, string publishDate, string tagIDs)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.LongResult Result = new ClassCollection.Model.LongResult();
            Result.result = new Model.Result();
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(js.Serialize(Result));
                return;
            }


            if (db.mUserTbls.Any(c => c.ID == userID) == false)
            {
                Result.result.code = 2;
                Result.result.message = ClassCollection.Message.USER_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }
            if (db.mUserTbls.Single(c => c.ID == userID).isBlock == true)
            {
                Result.result.code = 3;
                Result.result.message = ClassCollection.Message.USER_BLOCKED;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (!db.downloadTbls.Any(c => c.ID == downloadID && c.mUserID.Value == userID))
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.DOWNLOAD_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }


            var download = db.downloadTbls.Single(c => c.ID == downloadID);

            title = title.TrimStart().TrimEnd();
            if (title.Length == 0)
            {
                title = download.title;
            }

            publishDate = publishDate.Trim();
            DateTime publishdate = download.publishDate;
            if (publishDate.Length != 0)
            {
                int pyear = 0, pmonth = 0, pday = 0;
                var DateArray = publishDate.Split('-');
                if (DateArray.Count() != 3
                    || !int.TryParse(DateArray[0], out pyear)
                    || !int.TryParse(DateArray[1], out pmonth)
                    || !int.TryParse(DateArray[2], out pday))
                {
                    Result.result.code = 4;
                    Result.result.message = ClassCollection.Message.DATE_INCORRECT;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                var dateArr = publishDate.Split('-');
                publishdate = Persia.Calendar.ConvertToGregorian(int.Parse(dateArr[0]), int.Parse(dateArr[1]), int.Parse(dateArr[2]), Persia.DateType.Persian);
            }

            List<long> tags = new List<long>();
            bool tagchange = false;

            try
            {
                if (tagIDs.Length != 0)
                {
                    tags = new JavaScriptSerializer().Deserialize<List<string>>(tagIDs).Select(c => long.Parse(c)).ToList();
                    if (tags.Count() == 0)
                    {
                        tagchange = false;
                    }
                    else
                    {
                        tagchange = true;
                    }
                }
                else
                {
                    tagchange = false;
                }
            }
            catch
            {
                Result.result.code = 5;
                Result.result.message = ClassCollection.Message.TAGS_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (tagchange == true)
            {
                foreach (var item in tags)
                {
                    if (db.tagTbls.Any(c => c.ID == item) == false)
                    {
                        Result.result.code = 6;
                        Result.result.message = ClassCollection.Message.TAGS_NOT_EXIST;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                        return;
                    }
                }

                var oldTags = db.downloadTagTbls.Where(c => c.downloadID == downloadID);
                foreach (var item in oldTags)
                {
                    db.downloadTagTbls.DeleteOnSubmit(item);
                    db.SubmitChanges();
                }
            }

            download.title = title;
            download.isBlock = true;
            download.isPublished = true;
            download.publishDate = publishdate;
            db.SubmitChanges();

            if (tagchange == true)
            {
                foreach (var item in tags)
                {
                    var tag = new downloadTagTbl();
                    tag.downloadID = download.ID;
                    tag.tagID = item;

                    db.downloadTagTbls.InsertOnSubmit(tag);
                    db.SubmitChanges();
                }
            }

            //new service.pushservice().XsendPush("1", "news", news.ID + "", news.title, "all", "false");
            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }

        [WebMethod(Description = "این تابع وظیفه درج جزییات دانلود را بر عهده دارد")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void registerDownloadContent(string key, long downloadID, int contentType, string value)
        {
            ClassCollection.Model.LongResult Result = new ClassCollection.Model.LongResult();
            Result.result = new Model.Result();
            JavaScriptSerializer js = new JavaScriptSerializer();
            DateTime dt = new DateTime();
            dt = DateTime.Now;
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(js.Serialize(Result));
                return;
            }
            if (!db.downloadTbls.Any(c => c.ID == downloadID))
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.DOWNLOAD_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (contentType != 0 && contentType != 1 && contentType != 2)
            {
                Result.result.code = 2;
                Result.result.message = ClassCollection.Message.DOWNLOADContent_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (value.Length == 0)
            {
                Result.result.code = 3;
                Result.result.message = ClassCollection.Message.TEXT_INCORRECT;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            var tmp = new downloadContentTbl();

            if (contentType == 0)
            {
                tmp.value = "<p>" + value + "</p>";
            }
            if (contentType == 1 || contentType == 2)
            {
                try
                {
                    var totalPath = HttpContext.Current.Server.MapPath(ClassCollection.Methods.getfilePath()) + value;
                    if (!System.IO.File.Exists(totalPath))
                    {
                        Result.result.code = 4;
                        Result.result.message = ClassCollection.Message.ERROR_EXIST_FILE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                        return;
                    }
                    var ext = value.Split('.').Last();
                    var fileData = File.ReadAllBytes(totalPath);
                    File.Delete(totalPath);

                    var filename = "appinno" + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N") + "." + ext;
                    string fpath = "";
                    tmp.value = filename;
                    if (contentType == 1)
                    {
                        fpath = Server.MapPath(ClassCollection.Methods.getdownloadImagesPath() + filename);
                    }
                    else if (contentType == 2)
                    {
                        fpath = Server.MapPath(ClassCollection.Methods.getdownloadFilePath()) + filename;
                    }
                    try
                    {
                        File.WriteAllBytes(fpath, fileData);
                    }
                    catch
                    {
                        Result.result.code = 5;
                        Result.result.message = ClassCollection.Message.ERROR_SAVE_FILE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                        return;
                    }
                    if (contentType == 2)
                    {
                        tmp.fileSize = (int)new System.IO.FileInfo(fpath).Length / 1024;
                        tmp.fileType = Path.GetExtension(fpath).Split('.').Last();
                        tmp.downloadCount = 0;
                    }
                }
                catch
                {
                    Result.result.code = 6;
                    Result.result.message = ClassCollection.Message.ERROR_TRANSFER_FILE;
                    Context.Response.Write(js.Serialize(Result).ToLower());
                    return;
                }
            }

            tmp.downloadID = downloadID;
            tmp.contentType = contentType;
            if (!db.downloadContentTbls.Any(c => c.downloadID == downloadID))
            {
                tmp.pri = 0;
            }
            else
            {
                tmp.pri = db.downloadContentTbls.Where(c => c.downloadID == downloadID).Max(c => c.pri) + 1;
            }
            tmp.regDate = dt;

            db.downloadContentTbls.InsertOnSubmit(tmp);
            db.SubmitChanges();

            //new service.pushservice().XsendPush("1", "news", news.ID + "", news.title, "all", "false");
            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Result.value = tmp.ID;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }

        [WebMethod(Description = "این تابع وظیفه حذف جزییات دانلود را بر عهده دارد")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void deleteDownloadContent(string key, long contentID)
        {
            ClassCollection.Model.Result Result = new ClassCollection.Model.Result();
            JavaScriptSerializer js = new JavaScriptSerializer();
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.code = -1;
                Result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            if (!db.downloadContentTbls.Any(c => c.ID == contentID))
            {
                Result.code = 1;
                Result.message = ClassCollection.Message.DOWNLOADContent_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            var deletingPar = db.downloadContentTbls.Single(c => c.ID == contentID);

            if (deletingPar.contentType == 1)
            {
                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getdownloadImagesPath() + deletingPar.value)))
                {
                    try
                    {
                        System.IO.File.Delete(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getdownloadImagesPath() + deletingPar.value));
                    }
                    catch
                    {
                        Result.code = 2;
                        Result.message = ClassCollection.Message.ERROR_DELETE_IMAGE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                    }
                }
            }
            else if (deletingPar.contentType == 2)
            {
                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getdownloadFilePath() + deletingPar.value)))
                {
                    try
                    {
                        System.IO.File.Delete(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getdownloadFilePath() + deletingPar.value));
                    }
                    catch
                    {
                        Result.code = 3;
                        Result.message = ClassCollection.Message.ERROR_DELETE_VIDEO;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                    }
                }
            }
            db.downloadContentTbls.DeleteOnSubmit(deletingPar);
            db.SubmitChanges();

            Result.code = 0;
            Result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }

        [WebMethod(Description = "این تابع وظیفه تغییر جزییات دانلود را بر عهده دارد")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void editDownloadContent(string key, long contentID, int contentType, string value)
        {
            ClassCollection.Model.LongResult Result = new ClassCollection.Model.LongResult();
            Result.result = new Model.Result();
            JavaScriptSerializer js = new JavaScriptSerializer();
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            if (!db.downloadContentTbls.Any(c => c.ID == contentID))
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.DOWNLOADContent_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            var downloadcontent = db.downloadContentTbls.Single(c => c.ID == contentID);

            if (value.Length == 0)
            {
                value = downloadcontent.value;
            }

            if (contentType.ToString().Length != 0)
            {
                if (contentType != 0 && contentType != 1 && contentType != 2)
                {
                    Result.result.code = 2;
                    Result.result.message = ClassCollection.Message.DOWNLOADContent_INCORRECT;
                    Context.Response.Write(js.Serialize(Result).ToLower());
                    return;
                }
            }
            else
            {
                contentType = downloadcontent.contentType;
            }

            if (downloadcontent.contentType == 1)
            {
                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getdownloadImagesPath() + downloadcontent.value)) && downloadcontent.value != value)
                {
                    try
                    {
                        System.IO.File.Delete(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getdownloadImagesPath() + downloadcontent.value));
                    }
                    catch
                    {
                        Result.result.code = 3;
                        Result.result.message = ClassCollection.Message.ERROR_DELETE_IMAGE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                    }
                }
            }
            else if (downloadcontent.contentType == 2)
            {
                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getdownloadFilePath() + downloadcontent.value)) && downloadcontent.value != value)
                {
                    try
                    {
                        System.IO.File.Delete(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getdownloadFilePath() + downloadcontent.value));
                    }
                    catch
                    {
                        Result.result.code = 4;
                        Result.result.message = ClassCollection.Message.ERROR_DELETE_VIDEO;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                    }
                }
            }

            if (contentType == 1 || contentType == 2)
            {
                try
                {
                    var totalPath = HttpContext.Current.Server.MapPath(ClassCollection.Methods.getfilePath()) + value;
                    if (!System.IO.File.Exists(totalPath))
                    {
                        Result.result.code = 4;
                        Result.result.message = ClassCollection.Message.ERROR_EXIST_FILE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                        return;
                    }
                    var ext = value.Split('.').Last();
                    var fileData = File.ReadAllBytes(totalPath);
                    File.Delete(totalPath);

                    var filename = "appinno" + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N") + "." + ext;
                    string fpath = "";
                    value = filename;
                    if (contentType == 1)
                    {
                        fpath = Server.MapPath(ClassCollection.Methods.getdownloadImagesPath() + filename);
                    }
                    else if (contentType == 2)
                    {
                        fpath = Server.MapPath(ClassCollection.Methods.getdownloadFilePath()) + filename;
                    }
                    try
                    {
                        File.WriteAllBytes(fpath, fileData);
                    }
                    catch
                    {
                        Result.result.code = 5;
                        Result.result.message = ClassCollection.Message.ERROR_SAVE_FILE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                        return;
                    }
                    if (contentType == 2)
                    {
                        downloadcontent.fileSize = (int)new System.IO.FileInfo(fpath).Length / 1024;
                        downloadcontent.fileType = Path.GetExtension(fpath).Split('.').Last();
                        downloadcontent.downloadCount = 0;
                    }
                }
                catch
                {
                    Result.result.code = 6;
                    Result.result.message = ClassCollection.Message.ERROR_TRANSFER_FILE;
                    Context.Response.Write(js.Serialize(Result).ToLower());
                    return;
                }
            }


            downloadcontent.contentType = contentType;
            downloadcontent.value = value;
            db.SubmitChanges();

            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }
        //soheila-end-registerDownload
        #endregion

        #region registerReport
        //soheila-start-registerReport
        [WebMethod(Description = "این تابع وظیفه درج گزارش جدید را بر عهده دارد")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void registerReport(string key, long userID, string title, string publishDate, string tagIDs)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.LongResult Result = new ClassCollection.Model.LongResult();
            Result.result = new Model.Result();
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            if (db.mUserTbls.Any(c => c.ID == userID) == false)
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.USER_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }
            if (db.mUserTbls.Single(c => c.ID == userID).isBlock == true)
            {
                Result.result.code = 2;
                Result.result.message = ClassCollection.Message.USER_BLOCKED;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            title = title.TrimStart().TrimEnd();
            if (title.Length == 0)
            {
                Result.result.code = 3;
                Result.result.message = ClassCollection.Message.TEXT_INCORRECT;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            var publishDate1 = new DateTime();
            if (publishDate != "")
            {
                publishDate = publishDate.Trim();
                int pyear = 0, pmonth = 0, pday = 0;
                var DateArray = publishDate.Split('-');
                if (DateArray.Count() != 3
                   || !int.TryParse(DateArray[0], out pyear)
                   || !int.TryParse(DateArray[1], out pmonth)
                   || !int.TryParse(DateArray[2], out pday))
                {
                    Result.result.code = 4;
                    Result.result.message = ClassCollection.Message.DATE_INCORRECT;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }

                try
                {
                    publishDate1 = new DateTime(pyear, pmonth, pday, 0, 0, 0);
                }
                catch
                {
                    Result.result.code = 2;
                    Result.result.message = ClassCollection.Message.DATE_INCORRECT;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
            }

            List<long> tags = new List<long>();
            try
            {
                if (tagIDs.Length != 0)
                {
                    tags = new JavaScriptSerializer().Deserialize<List<string>>(tagIDs).Select(c => long.Parse(c)).ToList();
                    if (tags.Count() == 0)
                    {
                        Result.result.code = 5;
                        Result.result.message = ClassCollection.Message.TAGS_NOT_EXIST;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                        return;
                    }
                }
            }
            catch
            {
                Result.result.code = 6;
                Result.result.message = ClassCollection.Message.TAGS_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }
            foreach (var item in tags)
            {
                if (db.tagTbls.Any(c => c.ID == item) == false)
                {
                    Result.result.code = 7;
                    Result.result.message = ClassCollection.Message.TAGS_NOT_EXIST;
                    Context.Response.Write(js.Serialize(Result).ToLower());
                    return;
                }
            }
            var report = new masterReportTbl();
            var dt = new DateTime();
            dt = DateTime.Now;
            report.title = title;
            report.isBlock = true;
            report.mUserID = userID;
            if (publishDate != "")
            {
                report.publishDate = publishDate1;
            }
            else
            {
                report.publishDate = dt.Date;
            }
            report.regDate = dt;
            report.viewCount = 0;
            db.masterReportTbls.InsertOnSubmit(report);
            db.SubmitChanges();

            foreach (var item in tags)
            {
                var tag = new masterReportTagTbl();
                tag.masterReportID = report.ID;
                tag.tagID = item;

                db.masterReportTagTbls.InsertOnSubmit(tag);
                db.SubmitChanges();
            }

            //new service.pushservice().XsendPush("1", "news", news.ID + "", news.title, "all", "false");
            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Result.value = report.ID;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }

        [WebMethod(Description = "این تابع وظیفه تغییر گزارش را بر عهده دارد")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void editReport(string key, long reportID, long userID, string title, string publishDate, string tagIDs)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.LongResult Result = new ClassCollection.Model.LongResult();
            Result.result = new Model.Result();
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            if (!db.masterReportTbls.Any(c => c.ID == reportID))
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.CHART_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (db.mUserTbls.Any(c => c.ID == userID) == false)
            {
                Result.result.code = 2;
                Result.result.message = ClassCollection.Message.USER_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }
            if (db.mUserTbls.Single(c => c.ID == userID).isBlock == true)
            {
                Result.result.code = 3;
                Result.result.message = ClassCollection.Message.USER_BLOCKED;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (!db.masterReportTbls.Any(c => c.ID == reportID && c.mUserID.Value == userID))
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.CHART_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            var report = db.masterReportTbls.Single(c => c.ID == reportID);

            title = title.TrimStart().TrimEnd();
            if (title.Length == 0)
            {
                title = report.title;
            }

            publishDate = publishDate.Trim();
            DateTime publishdate = report.publishDate;
            if (publishDate.Length != 0)
            {
                int pyear = 0, pmonth = 0, pday = 0;
                var DateArray = publishDate.Split('-');
                if (DateArray.Count() != 3
                    || !int.TryParse(DateArray[0], out pyear)
                    || !int.TryParse(DateArray[1], out pmonth)
                    || !int.TryParse(DateArray[2], out pday))
                {
                    Result.result.code = 4;
                    Result.result.message = ClassCollection.Message.DATE_INCORRECT;
                    Context.Response.Write(js.Serialize(Result));
                    return;
                }
                var dateArr = publishDate.Split('-');
                publishdate = Persia.Calendar.ConvertToGregorian(int.Parse(dateArr[0]), int.Parse(dateArr[1]), int.Parse(dateArr[2]), Persia.DateType.Persian);
            }

            List<long> tags = new List<long>();
            bool tagchange = false;

            try
            {
                if (tagIDs.Length != 0)
                {
                    tags = new JavaScriptSerializer().Deserialize<List<string>>(tagIDs).Select(c => long.Parse(c)).ToList();
                    if (tags.Count() == 0)
                    {
                        tagchange = false;
                    }
                    else
                    {
                        tagchange = true;
                    }
                }
                else
                {
                    tagchange = false;
                }
            }
            catch
            {
                Result.result.code = 5;
                Result.result.message = ClassCollection.Message.TAGS_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (tagchange == true)
            {
                foreach (var item in tags)
                {
                    if (db.tagTbls.Any(c => c.ID == item) == false)
                    {
                        Result.result.code = 6;
                        Result.result.message = ClassCollection.Message.TAGS_NOT_EXIST;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                        return;
                    }
                }

                var oldTags = db.masterReportTagTbls.Where(c => c.masterReportID == reportID);
                foreach (var item in oldTags)
                {
                    db.masterReportTagTbls.DeleteOnSubmit(item);
                    db.SubmitChanges();
                }
            }

            report.title = title;
            report.isBlock = true;
            report.publishDate = publishdate;
            db.SubmitChanges();

            if (tagchange == true)
            {
                foreach (var item in tags)
                {
                    var tag = new masterReportTagTbl();
                    tag.masterReportID = report.ID;
                    tag.tagID = item;

                    db.masterReportTagTbls.InsertOnSubmit(tag);
                    db.SubmitChanges();
                }
            }

            //new service.pushservice().XsendPush("1", "news", news.ID + "", news.title, "all", "false");
            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }

        [WebMethod(Description = "این تابع وظیفه درج جزییات گزارش را بر عهده دارد")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void registerReportContent(string key, long reportID, int contentType, string value)
        {
            ClassCollection.Model.LongResult Result = new ClassCollection.Model.LongResult();
            Result.result = new Model.Result();
            JavaScriptSerializer js = new JavaScriptSerializer();
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(js.Serialize(Result));
                return;
            }
            if (!db.masterReportTbls.Any(c => c.ID == reportID))
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.CHART_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (contentType != 0 && contentType != 1 && contentType != 2)
            {
                Result.result.code = 2;
                Result.result.message = ClassCollection.Message.CHARTContent_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (value.Length == 0)
            {
                Result.result.code = 3;
                Result.result.message = ClassCollection.Message.TEXT_INCORRECT;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            var tmp = new masterReportContentTbl();
            tmp.masterReportID = reportID;
            tmp.value = value;
            tmp.type = contentType;

            db.masterReportContentTbls.InsertOnSubmit(tmp);
            db.SubmitChanges();

            //new service.pushservice().XsendPush("1", "news", news.ID + "", news.title, "all", "false");
            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Result.value = tmp.ID;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }

        [WebMethod(Description = "این تابع وظیفه حذف جزییات گزارش را بر عهده دارد")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void deleteReportContent(string key, long contentID)
        {
            ClassCollection.Model.Result Result = new ClassCollection.Model.Result();
            JavaScriptSerializer js = new JavaScriptSerializer();
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.code = -1;
                Result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            if (!db.masterReportContentTbls.Any(c => c.ID == contentID))
            {
                Result.code = 1;
                Result.message = ClassCollection.Message.CHARTContent_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            var deletingPar = db.masterReportContentTbls.Single(c => c.ID == contentID);

            if (deletingPar.type == 1)
            {
                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getreportImagesPath() + deletingPar.value)))
                {
                    try
                    {
                        System.IO.File.Delete(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getreportImagesPath() + deletingPar.value));
                    }
                    catch
                    {
                        Result.code = 2;
                        Result.message = ClassCollection.Message.ERROR_DELETE_IMAGE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                    }
                }
            }

            db.masterReportContentTbls.DeleteOnSubmit(deletingPar);
            db.SubmitChanges();

            Result.code = 0;
            Result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }

        [WebMethod(Description = "این تابع وظیفه تغییر جزییات گزارش را بر عهده دارد")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void editReportContent(string key, long contentID, int contentType, string value)
        {
            ClassCollection.Model.LongResult Result = new ClassCollection.Model.LongResult();
            Result.result = new Model.Result();
            JavaScriptSerializer js = new JavaScriptSerializer();
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            if (!db.masterReportContentTbls.Any(c => c.ID == contentID))
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.CHARTContent_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            var reportcontent = db.masterReportContentTbls.Single(c => c.ID == contentID);

            if (value.Length == 0)
            {
                value = reportcontent.value;
            }

            if (contentType.ToString().Length != 0)
            {
                if (contentType != 0 && contentType != 1 && contentType != 2)
                {
                    Result.result.code = 2;
                    Result.result.message = ClassCollection.Message.CHARTContent_INCORRECT;
                    Context.Response.Write(js.Serialize(Result).ToLower());
                    return;
                }
            }
            else
            {
                contentType = reportcontent.type;
            }

            if (reportcontent.type == 1)
            {
                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getreportImagesPath() + reportcontent.value)) && reportcontent.value != value)
                {
                    try
                    {
                        System.IO.File.Delete(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getreportImagesPath() + reportcontent.value));
                    }
                    catch
                    {
                        Result.result.code = 3;
                        Result.result.message = ClassCollection.Message.ERROR_DELETE_IMAGE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                    }
                }
            }


            reportcontent.type = contentType;
            reportcontent.value = value;
            db.SubmitChanges();

            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }
        //soheila-end-registerReport
        #endregion

        #region registerIcan

        /// <summary>
        /// </summary>
        /// <param name="icanID"></param>
        /// <param name="icancontent">[{"ID":"205","type":"0","value":"fffffffffffffffffffff"}]</param>
        /// 
        [WebMethod(Description = "این تابع وظیفه درج توانایی جدید را بر عهده دارد")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void registerIcan(string key, long userID, string title)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.LongResult Result = new ClassCollection.Model.LongResult();
            Result.result = new Model.Result();
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            if (db.mUserTbls.Any(c => c.ID == userID) == false)
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.USER_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }
            if (db.mUserTbls.Single(c => c.ID == userID).isBlock == true)
            {
                Result.result.code = 2;
                Result.result.message = ClassCollection.Message.USER_BLOCKED;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            title = title.TrimStart().TrimEnd();
            if (title.Length == 0)
            {
                Result.result.code = 3;
                Result.result.message = ClassCollection.Message.TEXT_INCORRECT;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            var ican = new icanTbl();
            var dt = new DateTime();
            dt = DateTime.Now;
            ican.title = title;
            ican.isBlock = true;
            ican.creatorID = userID;
            ican.regDate = dt;
            ican.viewCount = 0;
            db.icanTbls.InsertOnSubmit(ican);
            db.SubmitChanges();

            //new service.pushservice().XsendPush("1", "news", news.ID + "", news.title, "all", "false");
            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Result.value = ican.ID;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }

        [WebMethod(Description = "این تابع وظیفه تغییر توانایی را بر عهده دارد")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void editIcan(string key, long icanID, long userID, string title)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.LongResult Result = new ClassCollection.Model.LongResult();
            Result.result = new Model.Result();
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            if (db.mUserTbls.Any(c => c.ID == userID) == false)
            {
                Result.result.code = 2;
                Result.result.message = ClassCollection.Message.USER_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }
            if (db.mUserTbls.Single(c => c.ID == userID).isBlock == true)
            {
                Result.result.code = 3;
                Result.result.message = ClassCollection.Message.USER_BLOCKED;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (!db.icanTbls.Any(c => c.ID == icanID && c.creatorID == userID))
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.ICAN_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            var ican = db.icanTbls.Single(c => c.ID == icanID);

            title = title.TrimStart().TrimEnd();
            if (title.Length == 0)
            {
                title = ican.title;
            }

            ican.title = title;
            ican.isBlock = true;
            ican.creatorID = userID;
            db.SubmitChanges();

            //new service.pushservice().XsendPush("1", "news", news.ID + "", news.title, "all", "false");
            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }

        [WebMethod(Description = "این تابع وظیفه درج جزییات توانایی را بر عهده دارد")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void registerIcanContent(string key, long icanID, int contentType, string value)
        {
            ClassCollection.Model.LongResult Result = new ClassCollection.Model.LongResult();
            Result.result = new Model.Result();
            JavaScriptSerializer js = new JavaScriptSerializer();
            DateTime dt = new DateTime();
            dt = DateTime.Now;
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(js.Serialize(Result));
                return;
            }
            if (!db.icanTbls.Any(c => c.ID == icanID))
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.ICAN_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (contentType != 0 && contentType != 1 && contentType != 2 && contentType != 3)
            {
                Result.result.code = 2;
                Result.result.message = ClassCollection.Message.ICANContent_INCORRECT;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (value.Length == 0)
            {
                Result.result.code = 3;
                Result.result.message = ClassCollection.Message.TEXT_INCORRECT;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            var tmp = new icanContentTbl();
            if (contentType == 0)
            {
                tmp.value = "<p>" + value + "</p>";
            }
            if (contentType == 1 || contentType == 2 || contentType == 3)
            {
                try
                {
                    var totalPath = HttpContext.Current.Server.MapPath(ClassCollection.Methods.getfilePath()) + value;
                    if (!System.IO.File.Exists(totalPath))
                    {
                        Result.result.code = 4;
                        Result.result.message = ClassCollection.Message.ERROR_EXIST_FILE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                        return;
                    }
                    var ext = value.Split('.').Last();
                    var fileData = File.ReadAllBytes(totalPath);
                    File.Delete(totalPath);

                    var filename = "appinno" + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N") + "." + ext;
                    string fpath = "";
                    tmp.value = filename;
                    if (contentType == 1)
                    {
                        fpath = Server.MapPath(ClassCollection.Methods.geticanImagesPath() + filename);
                    }
                    else if (contentType == 2)
                    {
                        fpath = Server.MapPath(ClassCollection.Methods.geticanVideosPath() + filename);
                    }
                    else if (contentType == 3)
                    {
                        fpath = Server.MapPath(ClassCollection.Methods.geticanFilePath()) + filename;
                    }
                    try
                    {
                        File.WriteAllBytes(fpath, fileData);
                    }
                    catch
                    {
                        Result.result.code = 5;
                        Result.result.message = ClassCollection.Message.ERROR_SAVE_FILE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                        return;
                    }
                    if (contentType == 3)
                    {
                        tmp.fileSize = (int)new System.IO.FileInfo(fpath).Length / 1024;
                        tmp.fileType = Path.GetExtension(fpath).Split('.').Last();
                        tmp.downloadCount = 0;
                    }
                }
                catch
                {
                    Result.result.code = 6;
                    Result.result.message = ClassCollection.Message.ERROR_TRANSFER_FILE;
                    Context.Response.Write(js.Serialize(Result).ToLower());
                    return;
                }
            }

            tmp.icanID = icanID;
            tmp.contentType = contentType;
            if (!db.icanContentTbls.Any(c => c.icanID == icanID))
            {
                tmp.pri = 0;
            }
            else
            {
                tmp.pri = db.icanContentTbls.Where(c => c.icanID == icanID).Max(c => c.pri) + 1;
            }
            tmp.regDate = dt;
            db.icanContentTbls.InsertOnSubmit(tmp);
            db.SubmitChanges();

            if (contentType == 3)
            {
                var ican = db.icanTbls.Single(c => c.ID == icanID);
                if (ican.haveAttachment == false)
                {
                    ican.haveAttachment = true;
                    db.SubmitChanges();
                }
            }

            //new service.pushservice().XsendPush("1", "news", news.ID + "", news.title, "all", "false");
            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Result.value = tmp.ID;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }

        [WebMethod(Description = "این تابع وظیفه حذف جزییات توانایی را بر عهده دارد")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void deleteIcanContent(string key, long contentID)
        {
            ClassCollection.Model.Result Result = new ClassCollection.Model.Result();
            JavaScriptSerializer js = new JavaScriptSerializer();
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.code = -1;
                Result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            if (!db.icanContentTbls.Any(c => c.ID == contentID))
            {
                Result.code = 1;
                Result.message = ClassCollection.Message.ICANContent_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            var deletingPar = db.icanContentTbls.Single(c => c.ID == contentID);
            var icanID = deletingPar.icanID;

            if (deletingPar.contentType == 1)
            {
                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(ClassCollection.Methods.geticanImagesPath() + deletingPar.value)))
                {
                    try
                    {
                        System.IO.File.Delete(HttpContext.Current.Server.MapPath(ClassCollection.Methods.geticanImagesPath() + deletingPar.value));
                    }
                    catch
                    {
                        Result.code = 2;
                        Result.message = ClassCollection.Message.ERROR_DELETE_IMAGE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                    }
                }
            }
            else if (deletingPar.contentType == 2)
            {
                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(ClassCollection.Methods.geticanVideosPath() + deletingPar.value)))
                {
                    try
                    {
                        System.IO.File.Delete(HttpContext.Current.Server.MapPath(ClassCollection.Methods.geticanVideosPath() + deletingPar.value));
                    }
                    catch
                    {
                        Result.code = 3;
                        Result.message = ClassCollection.Message.ERROR_DELETE_VIDEO;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                    }
                }
            }
            else if (deletingPar.contentType == 3)
            {
                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(ClassCollection.Methods.geticanFilePath() + deletingPar.value)))
                {
                    try
                    {
                        System.IO.File.Delete(HttpContext.Current.Server.MapPath(ClassCollection.Methods.geticanFilePath() + deletingPar.value));
                    }
                    catch
                    {
                        Result.code = 4;
                        Result.message = ClassCollection.Message.ERROR_DELETE_FILE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                    }
                }
            }

            db.icanContentTbls.DeleteOnSubmit(deletingPar);
            db.SubmitChanges();

            if (db.icanTbls.Any(c => c.ID == icanID) == true)
            {
                var ican = db.icanTbls.Single(c => c.ID == icanID);
                if (db.icanContentTbls.Any(c => c.icanID == icanID && (c.contentType == 1 || c.contentType == 2)) == false)
                {
                    ican.haveAttachment = false;
                    db.SubmitChanges();
                }
            }

            Result.code = 0;
            Result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }

        [WebMethod(Description = "این تابع وظیفه تغییر جزییات توانایی را بر عهده دارد")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void editIcanContent(string key, long contentID, int contentType, string value)
        {
            ClassCollection.Model.LongResult Result = new ClassCollection.Model.LongResult();
            Result.result = new Model.Result();
            JavaScriptSerializer js = new JavaScriptSerializer();
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            if (!db.icanContentTbls.Any(c => c.ID == contentID))
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.ICANContent_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            var icancontent = db.icanContentTbls.Single(c => c.ID == contentID);

            if (value.Length == 0)
            {
                value = icancontent.value;
            }

            if (contentType.ToString().Length != 0)
            {
                if (contentType != 0 && contentType != 1 && contentType != 2 && contentType != 3)
                {
                    Result.result.code = 2;
                    Result.result.message = ClassCollection.Message.ICANContent_INCORRECT;
                    Context.Response.Write(js.Serialize(Result).ToLower());
                    return;
                }
            }

            else
            {
                contentType = icancontent.contentType;
            }

            if (contentType == 1 || contentType == 2 || contentType == 3)
            {
                try
                {
                    var totalPath = HttpContext.Current.Server.MapPath(ClassCollection.Methods.getfilePath()) + value;
                    if (!System.IO.File.Exists(totalPath))
                    {
                        Result.result.code = 3;
                        Result.result.message = ClassCollection.Message.ERROR_EXIST_FILE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                        return;
                    }
                    var ext = value.Split('.').Last();
                    var fileData = File.ReadAllBytes(totalPath);
                    File.Delete(totalPath);

                    var filename = "appinno" + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N") + "." + ext;
                    string fpath = "";
                    value = filename;
                    if (contentType == 1)
                    {
                        fpath = Server.MapPath(ClassCollection.Methods.geticanImagesPath() + filename);
                    }
                    else if (contentType == 2)
                    {
                        fpath = Server.MapPath(ClassCollection.Methods.geticanVideosPath() + filename);
                    }
                    else if (contentType == 3)
                    {
                        fpath = Server.MapPath(ClassCollection.Methods.geticanFilePath()) + filename;
                    }
                    try
                    {
                        File.WriteAllBytes(fpath, fileData);
                    }
                    catch
                    {
                        Result.result.code = 4;
                        Result.result.message = ClassCollection.Message.ERROR_SAVE_FILE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                        return;
                    }
                    if (contentType == 3)
                    {
                        icancontent.fileSize = (int)new System.IO.FileInfo(fpath).Length / 1024;
                        icancontent.fileType = Path.GetExtension(fpath).Split('.').Last();
                        icancontent.downloadCount = 0;
                    }
                }
                catch
                {
                    Result.result.code = 5;
                    Result.result.message = ClassCollection.Message.ERROR_TRANSFER_FILE;
                    Context.Response.Write(js.Serialize(Result).ToLower());
                    return;
                }
            }


            icancontent.contentType = contentType;
            icancontent.value = value;
            db.SubmitChanges();

            var icanID = icancontent.icanID;
            if (contentType == 3)
            {
                var ican = db.icanTbls.Single(c => c.ID == icanID);
                if (ican.haveAttachment == false)
                {
                    ican.haveAttachment = true;
                    db.SubmitChanges();
                }
            }

            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }

        #endregion

        #region Message

        [WebMethod(Description = "این تابع وظیفه درج پیام جدید توسط کاربر (اپلیکیشن) را بر عهده دارد")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void registermessage(string key, string title, string text, string attachment, long senderID, bool toAll, bool toPartner/*, List<long> pushTo*/, string pushTo)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.LongResult Result = new ClassCollection.Model.LongResult();
            Result.result = new Model.Result();
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            if (db.mUserTbls.Any(c => c.ID == senderID) == false)
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.USER_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }
            var sender = db.mUserTbls.Single(c => c.ID == senderID);

            if (db.mUserTbls.Single(c => c.ID == senderID).isBlock == true)
            {
                Result.result.code = 2;
                Result.result.message = ClassCollection.Message.USER_BLOCKED;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            title = title.TrimStart().TrimEnd();
            if (title.Length == 0)
            {
                Result.result.code = 3;
                Result.result.message = ClassCollection.Message.TEXT_INCORRECT;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (text.Length == 0)
            {
                Result.result.code = 4;
                Result.result.message = ClassCollection.Message.TEXT_INCORRECT;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (attachment.Length != 0 && attachment.Length > 500)
            {
                Result.result.code = 5;
                Result.result.message = ClassCollection.Message.FILE_INVALID;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }
            if (attachment.Length != 0)
            {
                if (File.Exists(Server.MapPath(ClassCollection.Methods.getfilePath() + attachment)) == false)
                {
                    Result.result.code = 6;
                    Result.result.message = ClassCollection.Message.FILE_INVALID;
                    Context.Response.Write(js.Serialize(Result).ToLower());
                    return;
                }
            }

            var message = new messageTbl();
            var dt = new DateTime();
            dt = DateTime.Now;
            message.title = title;
            message.text = text;
            message.regDate = dt;
            message.mUserID = senderID;
            message.toAll = toAll;
            message.toPartner = toPartner;
            message.isBlock = true;
            db.messageTbls.InsertOnSubmit(message);
            db.SubmitChanges();

            if (attachment != "")
            {
                try
                {
                    var totalPath = HttpContext.Current.Server.MapPath(ClassCollection.Methods.getfilePath()) + attachment;

                    var ext = attachment.Split('.').Last();
                    var fileData = File.ReadAllBytes(totalPath);
                    File.Delete(totalPath);

                    var filename = "appinno" + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N") + "." + ext;
                    string fpath = Server.MapPath(ClassCollection.Methods.getmessagePath() + filename);
                    try
                    {
                        File.WriteAllBytes(fpath, fileData);
                    }
                    catch
                    {
                        Result.result.code = 5;
                        Result.result.message = ClassCollection.Message.ERROR_SAVE_FILE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                        return;
                    }

                    message.fileSize = (int)new System.IO.FileInfo(fpath).Length / 1024;
                    message.fileType = Path.GetExtension(fpath).Split('.').Last();
                    message.downloadCount = 0;
                    message.attachment = filename;
                    db.SubmitChanges();
                }
                catch
                {
                    Result.result.code = 6;
                    Result.result.message = ClassCollection.Message.ERROR_TRANSFER_FILE;
                    Context.Response.Write(js.Serialize(Result).ToLower());
                    return;
                }
            }

            if (!toPartner && !toAll)
            {
                var pushTos = new JavaScriptSerializer().Deserialize<List<string>>(pushTo).Select(c => long.Parse(c)).ToList();
                string pushtoString = "";
                foreach (var item in pushTos)
                {
                    if (db.SubGroupTbls.Any(c => c.ID == item))
                    {
                        var t = new messageSubGroupTbl();
                        t.subGroupID = item;
                        t.messageID = message.ID;
                        db.messageSubGroupTbls.InsertOnSubmit(t);
                        db.SubmitChanges();
                        pushtoString += item + ",";
                    }
                }
                pushtoString = pushtoString.Remove(pushtoString.LastIndexOf(","), 1);
                ClassCollection.Methods.unBlocker(message, sender, pushtoString, toAll, toPartner);
            }
            else if (toPartner == true)
            {
                ClassCollection.Methods.unBlocker(message, sender, null, toAll, toPartner);
            }
            else if (toAll == true)
            {
                ClassCollection.Methods.unBlocker(message, sender, null, toAll, toPartner);
            }

            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Result.value = message.ID;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }
        
        [WebMethod(Description = "این تابع وظیفه نمایش یک پیام در (اپلیکیشن) را بر عهده دارد")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getMessage(long messageID)
        {
            ClassCollection.Model.MessageResult result = new ClassCollection.Model.MessageResult();
            result.result = new Model.Result();

            var db = new DataAccessDataContext();

            if (db.messageTbls.Any(c => c.ID == messageID) == false)
            {
                result.result.code = 3;
                result.result.message = ClassCollection.Message.PUSH_MESSAGE_NOT_EXIST;
                Context.Response.Write(new JavaScriptSerializer().Serialize(result));
                return;
            }

            var tmp = db.messageTbls.Single(c => c.ID == messageID);
            result.Message = new Model.Message();

            result.Message.ID = tmp.ID;
            result.Message.title = tmp.title;
            result.Message.text = tmp.text;
            result.Message.regDate = Persia.Calendar.ConvertToPersian(tmp.regDate).ToString("m") + " - " + Persia.Calendar.ConvertToPersian(tmp.regDate).ToString("H");
            result.Message.attachment = tmp.attachment;
            result.Message.toPartner = tmp.toPartner;
            result.Message.mUserID = tmp.mUserID;
            result.Message.toAll = tmp.toAll;
            result.Message.downloadCount = tmp.downloadCount;
            result.Message.fileSize = tmp.fileSize;
            result.Message.fileType = tmp.fileType;
            result.Message.fullname = tmp.mUserTbl.name + " " + tmp.mUserTbl.family;

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
            Context.Response.Write(new JavaScriptSerializer().Serialize(result));
            return;
        }

        [WebMethod(Description = "این تابع وظیفه نمایش پیام ها در (اپلیکیشن) را بر عهده دارد")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getAllMessage(string key, long muserID)
        {
            ClassCollection.Model.MessageListResult result = new ClassCollection.Model.MessageListResult();
            result.result = new ClassCollection.Model.Result();
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                result.result.code = -1;
                result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(new JavaScriptSerializer().Serialize(result));
                return;
            }

            if (db.mUserTbls.Any(c => c.ID == muserID) == false && muserID != -1)
            {
                result.result.code = 1;
                result.result.message = ClassCollection.Message.USER_NOT_EXIST;
                Context.Response.Write(new JavaScriptSerializer().Serialize(result));
                return;
            }
            
            IQueryable<messageTbl> ac = db.messageTbls;
            if (muserID == -1)
            {
                ac = ac.Where(c => c.toAll == true);
            }
            else
            {
                ac = ac.Where(c => (c.toAll == true || c.toPartner == true) ||
                (c.messageSubGroupTbls.Any(x => x.SubGroupTbl.userSubGroupTbls.Any(p => p.userID == muserID))));
            }
            
            ac = ac.OrderByDescending(c => c.regDate).Take(5);
            var acList = ac.Distinct().ToList();
            

            result.message = new List<Model.Message>();
            foreach (var item in acList)
            {
                var tmp = new ClassCollection.Model.Message();
                tmp.ID = item.ID;
                tmp.title = item.title;
                tmp.text = item.text;
                tmp.regDate = Persia.Calendar.ConvertToPersian(item.regDate).ToString("m") + " - " + Persia.Calendar.ConvertToPersian(item.regDate).ToString("H");
                tmp.attachment = item.attachment;
                tmp.mUserID = item.mUserID;
                tmp.downloadCount = item.downloadCount;
                tmp.fileSize = item.fileSize;
                tmp.fileType = item.fileType;
                tmp.fullname = item.mUserTbl.name + " " + item.mUserTbl.family;
                result.message.Add(tmp);
            }
            
            result.result.code = 0;
            result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Context.Response.Write(new JavaScriptSerializer().Serialize(result));
            return;
        }

        #endregion

        #region tag

        [WebMethod(Description = "این تابع وظیفه درج تگ جدید را بر عهده دارد")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void registertag(string key, long userID, string title)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.LongResult Result = new ClassCollection.Model.LongResult();
            Result.result = new Model.Result();
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            if (db.mUserTbls.Any(c => c.ID == userID) == false)
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.USER_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }
            if (db.mUserTbls.Single(c => c.ID == userID).isBlock == true)
            {
                Result.result.code = 2;
                Result.result.message = ClassCollection.Message.USER_BLOCKED;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            title = title.TrimStart().TrimEnd();
            if (title.Length == 0)
            {
                Result.result.code = 3;
                Result.result.message = ClassCollection.Message.TEXT_INCORRECT;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (db.tagTbls.Any(c => c.title == title))
            {
                Result.result.code = 7;
                Result.result.message = ClassCollection.Message.TAG_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            var tag = new tagTbl();
            var dt = new DateTime();
            dt = DateTime.Now;
            tag.title = title;
            db.tagTbls.InsertOnSubmit(tag);
            db.SubmitChanges();

            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Result.value = tag.ID;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }
        #endregion

        #region Idea 

        [WebMethod(Description = "این تابع وظیفه درج ایده جدید را بر عهده دارد")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void registerIdea(string key, long userID, string title, long bestIdeaCompetitionsID, string tagIDs)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.LongResult Result = new ClassCollection.Model.LongResult();
            Result.result = new Model.Result();
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            if (db.mUserTbls.Any(c => c.ID == userID) == false)
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.USER_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }
            if (db.mUserTbls.Single(c => c.ID == userID).isBlock == true)
            {
                Result.result.code = 2;
                Result.result.message = ClassCollection.Message.USER_BLOCKED;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (!db.bestIdeaCompetitionsTbls.Any(c => c.ID == bestIdeaCompetitionsID && c.isBlock == false))
            {
                Result.result.code = 3;
                Result.result.message = ClassCollection.Message.COMPETITIONS_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            var status = db.getBestIdeaCompetitionStatus(bestIdeaCompetitionsID);
            if (status != DataAccessDataContext.BestIdeaCompetitionStatus.sending)
            {
                Result.result.code = 4;
                Result.result.message = ClassCollection.Message.IDEA_INSERT_INCORRECT;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (db.bestIdeaCompetitionsTbls.Any(c => c.ID == bestIdeaCompetitionsID) == false)
            {
                Result.result.code = 3;
                Result.result.message = ClassCollection.Message.COMPETITIONS_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            title = title.TrimStart().TrimEnd();
            if (title.Length == 0)
            {
                Result.result.code = 3;
                Result.result.message = ClassCollection.Message.TEXT_INCORRECT;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            List<long> tags = new List<long>();
            try
            {
                if (tagIDs.Length != 0)
                {
                    tags = new JavaScriptSerializer().Deserialize<List<string>>(tagIDs).Select(c => long.Parse(c)).ToList();
                    if (tags.Count() == 0)
                    {
                        Result.result.code = 5;
                        Result.result.message = ClassCollection.Message.TAGS_NOT_EXIST;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                        return;
                    }
                }
            }
            catch
            {
                Result.result.code = 6;
                Result.result.message = ClassCollection.Message.TAGS_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }
            foreach (var item in tags)
            {
                if (db.tagTbls.Any(c => c.ID == item) == false)
                {
                    Result.result.code = 7;
                    Result.result.message = ClassCollection.Message.TAGS_NOT_EXIST;
                    Context.Response.Write(js.Serialize(Result).ToLower());
                    return;
                }
            }

            var Idea = new ideasTbl();
            var dt = new DateTime();
            dt = DateTime.Now;
            Idea.title = title;
            Idea.isBlock = true;
            Idea.mUserID = userID;
            Idea.regDate = dt;
            Idea.bestIdeaCompetitionsID = bestIdeaCompetitionsID;
            db.ideasTbls.InsertOnSubmit(Idea);
            db.SubmitChanges();

            foreach (var item in tags)
            {
                var tag = new ideasTagTbl();
                tag.ideasID = Idea.ID;
                tag.tagID = item;

                db.ideasTagTbls.InsertOnSubmit(tag);
                db.SubmitChanges();
            }

            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Result.value = Idea.ID;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }

        [WebMethod(Description = "این تابع وظیفه تغییر ایده را بر عهده دارد")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void editIdea(string key, long ideaID, long userID, string title, string tagIDs)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.LongResult Result = new ClassCollection.Model.LongResult();
            Result.result = new Model.Result();
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(js.Serialize(Result));
                return;
            }


            if (db.mUserTbls.Any(c => c.ID == userID) == false)
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.USER_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (db.mUserTbls.Single(c => c.ID == userID).isBlock == true)
            {
                Result.result.code = 2;
                Result.result.message = ClassCollection.Message.USER_BLOCKED;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (!db.ideasTbls.Any(c => c.ID == ideaID && c.mUserID == userID && c.isBlock == false && c.bestIdeaCompetitionsTbl.isBlock == false))
            {
                Result.result.code = 3;
                Result.result.message = ClassCollection.Message.IDEA_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            var idea = db.ideasTbls.Single(c => c.ID == ideaID);
            var status = db.getBestIdeaCompetitionStatus(idea.bestIdeaCompetitionsID);
            if (status != DataAccessDataContext.BestIdeaCompetitionStatus.sending)
            {
                Result.result.code = 4;
                Result.result.message = ClassCollection.Message.IDEA_INSERT_INCORRECT;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            title = title.TrimStart().TrimEnd();
            if (title.Length == 0)
            {
                title = idea.title;
            }


            List<long> tags = new List<long>();
            bool tagchange = false;

            try
            {
                if (tagIDs.Length != 0)
                {
                    tags = new JavaScriptSerializer().Deserialize<List<string>>(tagIDs).Select(c => long.Parse(c)).ToList();
                    if (tags.Count() == 0)
                    {
                        tagchange = false;
                    }
                    else
                    {
                        tagchange = true;
                    }
                }
                else
                {
                    tagchange = false;
                }
            }
            catch
            {
                Result.result.code = 4;
                Result.result.message = ClassCollection.Message.TAGS_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (tagchange == true)
            {
                foreach (var item in tags)
                {
                    if (db.tagTbls.Any(c => c.ID == item) == false)
                    {
                        Result.result.code = 5;
                        Result.result.message = ClassCollection.Message.TAGS_NOT_EXIST;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                        return;
                    }
                }

                var oldTags = db.ideasTagTbls.Where(c => c.ideasID == ideaID);
                db.ideasTagTbls.DeleteAllOnSubmit(oldTags);
                db.SubmitChanges();
            }

            idea.title = title;
            idea.isBlock = true;

            db.SubmitChanges();

            if (tagchange == true)
            {
                foreach (var item in tags)
                {
                    var tag = new ideasTagTbl();
                    tag.ideasID = tag.ID;
                    tag.tagID = item;

                    db.ideasTagTbls.InsertOnSubmit(tag);
                    db.SubmitChanges();
                }
            }

            //new service.pushservice().XsendPush("1", "news", news.ID + "", news.title, "all", "false");
            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }

        [WebMethod(Description = "این تابع وظیفه حذف ایده را بر عهده دارد")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void deleteIdea(string key, long ideaID, long userID)
        {
            ClassCollection.Model.Result Result = new ClassCollection.Model.Result();
            JavaScriptSerializer js = new JavaScriptSerializer();
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.code = -1;
                Result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            if (db.mUserTbls.Any(c => c.ID == userID) == false)
            {
                Result.code = 1;
                Result.message = ClassCollection.Message.USER_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (db.mUserTbls.Single(c => c.ID == userID).isBlock == true)
            {
                Result.code = 2;
                Result.message = ClassCollection.Message.USER_BLOCKED;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (!db.ideasTbls.Any(c => c.ID == ideaID && c.mUserID == userID && c.bestIdeaCompetitionsTbl.isBlock == false))
            {
                Result.code = 3;
                Result.message = ClassCollection.Message.IDEA_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            var idea = db.ideasTbls.Single(c => c.ID == ideaID);
            var status = db.getBestIdeaCompetitionStatus(idea.bestIdeaCompetitionsID);
            if (status != DataAccessDataContext.BestIdeaCompetitionStatus.sending)
            {
                Result.code = 4;
                Result.message = ClassCollection.Message.IDEA_INSERT_INCORRECT;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            var contentlist = db.ideasContentTbls.Where(c => c.ideasID == ideaID);
            foreach (var item in contentlist)
            {
                deleteIdeaContent("1", item.ID);
            }

            var tags = db.ideasTagTbls.Where(c => c.ideasID == ideaID);
            foreach (var item in tags)
            {
                db.ideasTagTbls.DeleteOnSubmit(item);
                db.SubmitChanges();
            }

            var likes = db.ideasLikeTbls.Where(c => c.ideasID == ideaID);
            foreach (var item in likes)
            {
                db.ideasLikeTbls.DeleteOnSubmit(item);
                db.SubmitChanges();
            }

            db.ideasTbls.DeleteOnSubmit(idea);
            db.SubmitChanges();

            Result.code = 0;
            Result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }

        [WebMethod(Description = "این تابع وظیفه درج جزییات ایده را بر عهده دارد")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void registerIdeaContent(string key, long ideaID, int contentType, string value)
        {
            ClassCollection.Model.LongResult Result = new ClassCollection.Model.LongResult();
            Result.result = new Model.Result();
            JavaScriptSerializer js = new JavaScriptSerializer();
            DateTime dt = new DateTime();
            dt = DateTime.Now;
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(js.Serialize(Result));
                return;
            }
            if (!db.ideasTbls.Any(c => c.ID == ideaID && c.bestIdeaCompetitionsTbl.isBlock == false))
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.IDEA_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (contentType != 0 && contentType != 1 && contentType != 2)
            {
                Result.result.code = 2;
                Result.result.message = ClassCollection.Message.IDEAContent_INCORRECT;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (value.Length == 0)
            {
                Result.result.code = 3;
                Result.result.message = ClassCollection.Message.TEXT_INCORRECT;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            var tmp = new ideasContentTbl();

            if (contentType == 0)
            {
                tmp.value = "<p>" + value + "</p>";
            }
            if (contentType == 1 || contentType == 2)
            {
                try
                {
                    var totalPath = HttpContext.Current.Server.MapPath(ClassCollection.Methods.getfilePath()) + value;
                    if (!System.IO.File.Exists(totalPath))
                    {
                        Result.result.code = 4;
                        Result.result.message = ClassCollection.Message.ERROR_EXIST_FILE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                        return;
                    }
                    var ext = value.Split('.').Last();
                    var fileData = File.ReadAllBytes(totalPath);
                    File.Delete(totalPath);

                    var filename = "appinno" + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N") + "." + ext;
                    string fpath = "";
                    tmp.value = filename;
                    if (contentType == 1)
                    {
                        fpath = Server.MapPath(ClassCollection.Methods.getIdeaImagesPath() + filename);
                    }
                    else if (contentType == 2)
                    {
                        fpath = Server.MapPath(ClassCollection.Methods.getIdeaVideosPath() + filename);
                    }
                    try
                    {
                        File.WriteAllBytes(fpath, fileData);
                    }
                    catch
                    {
                        Result.result.code = 5;
                        Result.result.message = ClassCollection.Message.ERROR_SAVE_FILE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                        return;
                    }
                }
                catch
                {
                    Result.result.code = 6;
                    Result.result.message = ClassCollection.Message.ERROR_TRANSFER_FILE;
                    Context.Response.Write(js.Serialize(Result).ToLower());
                    return;
                }
            }

            tmp.ideasID = ideaID;
            tmp.contentType = contentType;
            if (!db.ideasContentTbls.Any(c => c.ideasID == ideaID))
            {
                tmp.pri = 0;
            }
            else
            {
                tmp.pri = db.ideasContentTbls.Where(c => c.ideasID == ideaID).Max(c => c.pri) + 1;
            }
            tmp.regDate = dt;
            db.ideasContentTbls.InsertOnSubmit(tmp);
            db.SubmitChanges();

            //new service.pushservice().XsendPush("1", "news", news.ID + "", news.title, "all", "false");

            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Result.value = tmp.ID;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }

        [WebMethod(Description = "این تابع وظیفه حذف جزییات ایده را بر عهده دارد")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void deleteIdeaContent(string key, long contentID)
        {
            ClassCollection.Model.Result Result = new ClassCollection.Model.Result();
            JavaScriptSerializer js = new JavaScriptSerializer();
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.code = -1;
                Result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            if (!db.ideasContentTbls.Any(c => c.ID == contentID))
            {
                Result.code = 1;
                Result.message = ClassCollection.Message.IDEAContent_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            var deletingPar = db.ideasContentTbls.Single(c => c.ID == contentID);

            if (deletingPar.contentType == 1)
            {
                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getIdeaImagesPath() + deletingPar.value)))
                {
                    try
                    {
                        System.IO.File.Delete(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getIdeaImagesPath() + deletingPar.value));
                    }
                    catch
                    {
                        Result.code = 2;
                        Result.message = ClassCollection.Message.ERROR_DELETE_IMAGE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                    }
                }
            }
            else if (deletingPar.contentType == 2)
            {
                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getIdeaVideosPath() + deletingPar.value)))
                {
                    try
                    {
                        System.IO.File.Delete(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getIdeaVideosPath() + deletingPar.value));
                    }
                    catch
                    {
                        Result.code = 3;
                        Result.message = ClassCollection.Message.ERROR_DELETE_VIDEO;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                    }
                }
            }
            db.ideasContentTbls.DeleteOnSubmit(deletingPar);
            db.SubmitChanges();

            Result.code = 0;
            Result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }

        [WebMethod(Description = "این تابع وظیفه تغییر جزییات ایده را بر عهده دارد")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void editIdeaContent(string key, long contentID, int contentType, string value)
        {
            ClassCollection.Model.LongResult Result = new ClassCollection.Model.LongResult();
            Result.result = new Model.Result();
            JavaScriptSerializer js = new JavaScriptSerializer();
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            if (!db.ideasContentTbls.Any(c => c.ID == contentID))
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.IDEAContent_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            var ideacontent = db.ideasContentTbls.Single(c => c.ID == contentID);

            if (value.Length == 0)
            {
                value = ideacontent.value;
            }

            if (contentType.ToString().Length != 0)
            {
                if (contentType != 0 && contentType != 1 && contentType != 2)
                {
                    Result.result.code = 2;
                    Result.result.message = ClassCollection.Message.IDEAContent_INCORRECT;
                    Context.Response.Write(js.Serialize(Result).ToLower());
                    return;
                }
            }
            else
            {
                contentType = ideacontent.contentType;
            }

            if (ideacontent.contentType == 1)
            {
                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getIdeaImagesPath() + ideacontent.value)) && ideacontent.value != value)
                {
                    try
                    {
                        System.IO.File.Delete(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getIdeaImagesPath() + ideacontent.value));
                    }
                    catch
                    {
                        Result.result.code = 3;
                        Result.result.message = ClassCollection.Message.ERROR_DELETE_IMAGE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                    }
                }
            }
            else if (ideacontent.contentType == 2)
            {
                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getIdeaVideosPath() + ideacontent.value)) && ideacontent.value != value)
                {
                    try
                    {
                        System.IO.File.Delete(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getIdeaVideosPath() + ideacontent.value));
                    }
                    catch
                    {
                        Result.result.code = 4;
                        Result.result.message = ClassCollection.Message.ERROR_DELETE_VIDEO;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                    }
                }
            }

            if (ideacontent.contentType == 1 || ideacontent.contentType == 2)
            {
                try
                {
                    var totalPath = HttpContext.Current.Server.MapPath(ClassCollection.Methods.getfilePath()) + value;
                    if (!System.IO.File.Exists(totalPath))
                    {
                        Result.result.code = 5;
                        Result.result.message = ClassCollection.Message.ERROR_EXIST_FILE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                        return;
                    }
                    var ext = value.Split('.').Last();
                    var fileData = File.ReadAllBytes(totalPath);
                    File.Delete(totalPath);

                    var filename = "appinno" + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N") + "." + ext;
                    string fpath = "";
                    value = filename;
                    if (contentType == 1)
                    {
                        fpath = Server.MapPath(ClassCollection.Methods.getIdeaImagesPath() + filename);
                    }
                    else if (contentType == 2)
                    {
                        fpath = Server.MapPath(ClassCollection.Methods.getIdeaVideosPath() + filename);
                    }
                    try
                    {
                        File.WriteAllBytes(fpath, fileData);
                    }
                    catch
                    {
                        Result.result.code = 6;
                        Result.result.message = ClassCollection.Message.ERROR_SAVE_FILE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                        return;
                    }
                }
                catch
                {
                    Result.result.code = 7;
                    Result.result.message = ClassCollection.Message.ERROR_TRANSFER_FILE;
                    Context.Response.Write(js.Serialize(Result).ToLower());
                    return;
                }
            }

            ideacontent.contentType = contentType;
            ideacontent.value = value;
            db.SubmitChanges();

            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }

        #endregion

        #region Answer 

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void registerAnswer(string key, long userID, string title, long creativityCompetitionsID)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.LongResult Result = new ClassCollection.Model.LongResult();
            Result.result = new Model.Result();
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            if (db.mUserTbls.Any(c => c.ID == userID) == false)
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.USER_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }
            if (db.mUserTbls.Single(c => c.ID == userID).isBlock == true)
            {
                Result.result.code = 2;
                Result.result.message = ClassCollection.Message.USER_BLOCKED;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (!db.creativityCompetitionTbls.Any(c => c.ID == creativityCompetitionsID && c.isBlock == false))
            {
                Result.result.code = 3;
                Result.result.message = ClassCollection.Message.COMPETITIONS_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            var status = db.getCreativityCompetitionStatus(creativityCompetitionsID);
            if (status != DataAccessDataContext.CreativityCompetitionStatus.sending)
            {
                Result.result.code = 4;
                Result.result.message = ClassCollection.Message.ANSWER_INSERT_INCORRECT;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (db.creativityCompetitionTbls.Any(c => c.ID == creativityCompetitionsID) == false)
            {
                Result.result.code = 3;
                Result.result.message = ClassCollection.Message.COMPETITIONS_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            title = title.TrimStart().TrimEnd();
            if (title.Length == 0)
            {
                Result.result.code = 3;
                Result.result.message = ClassCollection.Message.TEXT_INCORRECT;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }


            var Answer = new answerTbl();
            var dt = new DateTime();
            dt = DateTime.Now;
            Answer.title = title;
            Answer.isBlock = true;
            Answer.mUserID = userID;
            Answer.regDate = dt;
            Answer.creativityCompetitionID = creativityCompetitionsID;
            db.answerTbls.InsertOnSubmit(Answer);
            db.SubmitChanges();

            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Result.value = Answer.ID;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void editAnswer(string key, long answerID, long userID, string title)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.LongResult Result = new ClassCollection.Model.LongResult();
            Result.result = new Model.Result();
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(js.Serialize(Result));
                return;
            }


            if (db.mUserTbls.Any(c => c.ID == userID) == false)
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.USER_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (db.mUserTbls.Single(c => c.ID == userID).isBlock == true)
            {
                Result.result.code = 2;
                Result.result.message = ClassCollection.Message.USER_BLOCKED;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (!db.answerTbls.Any(c => c.ID == answerID && c.mUserID == userID && c.isBlock == false && c.creativityCompetitionTbl.isBlock == false))
            {
                Result.result.code = 3;
                Result.result.message = ClassCollection.Message.ANSWER_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            var answer = db.answerTbls.Single(c => c.ID == answerID);
            var status = db.getCreativityCompetitionStatus(answer.creativityCompetitionID);
            if (status != DataAccessDataContext.CreativityCompetitionStatus.sending)
            {
                Result.result.code = 4;
                Result.result.message = ClassCollection.Message.ANSWER_INSERT_INCORRECT;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            title = title.TrimStart().TrimEnd();
            if (title.Length == 0)
            {
                title = answer.title;
            }


            answer.title = title;
            answer.isBlock = true;

            db.SubmitChanges();


            //new service.pushservice().XsendPush("1", "news", news.ID + "", news.title, "all", "false");
            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void deleteAnswer(string key, long answerID, long userID)
        {
            Model.Result Result = new Model.Result();
            JavaScriptSerializer js = new JavaScriptSerializer();
            var db = new DataAccessDataContext();

            if (!Methods.checkUserKey(key))
            {
                Result.code = -1;
                Result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            if (db.mUserTbls.Any(c => c.ID == userID) == false)
            {
                Result.code = 1;
                Result.message = ClassCollection.Message.USER_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (db.mUserTbls.Single(c => c.ID == userID).isBlock == true)
            {
                Result.code = 2;
                Result.message = ClassCollection.Message.USER_BLOCKED;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (!db.answerTbls.Any(c => c.ID == answerID && c.mUserID == userID && c.creativityCompetitionTbl.isBlock == false))
            {
                Result.code = 3;
                Result.message = ClassCollection.Message.ANSWER_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            var answer = db.answerTbls.Single(c => c.ID == answerID);
            var status = db.getCreativityCompetitionStatus(answer.creativityCompetitionID);
            if (status != DataAccessDataContext.CreativityCompetitionStatus.sending)
            {
                Result.code = 4;
                Result.message = ClassCollection.Message.ANSWER_INSERT_INCORRECT;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            var contentlist = db.answerContentTbls.Where(c => c.answerID == answerID);
            foreach (var item in contentlist)
            {
                deleteAnswerContent("1", item.ID);
            }

            db.answerTbls.DeleteOnSubmit(answer);
            db.SubmitChanges();

            Result.code = 0;
            Result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void registerAnswerContent(string key, long answerID, int contentType, string value)
        {
            ClassCollection.Model.LongResult Result = new ClassCollection.Model.LongResult();
            Result.result = new Model.Result();
            JavaScriptSerializer js = new JavaScriptSerializer();
            DateTime dt = new DateTime();
            dt = DateTime.Now;
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(js.Serialize(Result));
                return;
            }
            if (!db.answerTbls.Any(c => c.ID == answerID && c.creativityCompetitionTbl.isBlock == false))
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.ANSWER_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (contentType != 0 && contentType != 1 && contentType != 2)
            {
                Result.result.code = 2;
                Result.result.message = ClassCollection.Message.IDEAContent_INCORRECT;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (value.Length == 0)
            {
                Result.result.code = 3;
                Result.result.message = ClassCollection.Message.TEXT_INCORRECT;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            var tmp = new answerContentTbl();

            if (contentType == 0)
            {
                tmp.value = "<p>" + value + "</p>";
            }
            if (contentType == 1 || contentType == 2)
            {
                try
                {
                    var totalPath = HttpContext.Current.Server.MapPath(ClassCollection.Methods.getfilePath()) + value;
                    if (!System.IO.File.Exists(totalPath))
                    {
                        Result.result.code = 4;
                        Result.result.message = ClassCollection.Message.ERROR_EXIST_FILE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                        return;
                    }
                    var ext = value.Split('.').Last();
                    var fileData = File.ReadAllBytes(totalPath);
                    File.Delete(totalPath);

                    var filename = "appinno" + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N") + "." + ext;
                    string fpath = "";
                    tmp.value = filename;
                    if (contentType == 1)
                    {
                        fpath = Server.MapPath(ClassCollection.Methods.getCreativityCompetitionImagesPath() + filename);
                    }
                    else if (contentType == 2)
                    {
                        fpath = Server.MapPath(ClassCollection.Methods.getCreativityCompetitionVideosPath() + filename);
                    }
                    try
                    {
                        File.WriteAllBytes(fpath, fileData);
                    }
                    catch
                    {
                        Result.result.code = 5;
                        Result.result.message = ClassCollection.Message.ERROR_SAVE_FILE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                        return;
                    }
                }
                catch
                {
                    Result.result.code = 6;
                    Result.result.message = ClassCollection.Message.ERROR_TRANSFER_FILE;
                    Context.Response.Write(js.Serialize(Result).ToLower());
                    return;
                }
            }

            tmp.answerID = answerID;
            tmp.contentType = contentType;
            if (!db.answerContentTbls.Any(c => c.answerID == answerID))
            {
                tmp.pri = 0;
            }
            else
            {
                tmp.pri = db.answerContentTbls.Where(c => c.answerID == answerID).Max(c => c.pri) + 1;
            }
            tmp.regDate = dt;
            db.answerContentTbls.InsertOnSubmit(tmp);
            db.SubmitChanges();

            //new service.pushservice().XsendPush("1", "news", news.ID + "", news.title, "all", "false");

            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Result.value = tmp.ID;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void deleteAnswerContent(string key, long contentID)
        {
            ClassCollection.Model.Result Result = new ClassCollection.Model.Result();
            JavaScriptSerializer js = new JavaScriptSerializer();
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.code = -1;
                Result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            if (!db.answerContentTbls.Any(c => c.ID == contentID))
            {
                Result.code = 1;
                Result.message = ClassCollection.Message.IDEAContent_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            var deletingPar = db.answerContentTbls.Single(c => c.ID == contentID);

            if (deletingPar.contentType == 1)
            {
                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getCreativityCompetitionImagesPath() + deletingPar.value)))
                {
                    try
                    {
                        System.IO.File.Delete(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getCreativityCompetitionImagesPath() + deletingPar.value));
                    }
                    catch
                    {
                        Result.code = 2;
                        Result.message = ClassCollection.Message.ERROR_DELETE_IMAGE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                    }
                }
            }
            else if (deletingPar.contentType == 2)
            {
                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getCreativityCompetitionVideosPath() + deletingPar.value)))
                {
                    try
                    {
                        System.IO.File.Delete(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getCreativityCompetitionVideosPath() + deletingPar.value));
                    }
                    catch
                    {
                        Result.code = 3;
                        Result.message = ClassCollection.Message.ERROR_DELETE_VIDEO;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                    }
                }
            }
            db.answerContentTbls.DeleteOnSubmit(deletingPar);
            db.SubmitChanges();

            Result.code = 0;
            Result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void editAnswerContent(string key, long contentID, int contentType, string value)
        {
            ClassCollection.Model.LongResult Result = new ClassCollection.Model.LongResult();
            Result.result = new Model.Result();
            JavaScriptSerializer js = new JavaScriptSerializer();
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            if (!db.answerContentTbls.Any(c => c.ID == contentID))
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.ANSWER_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            var answercontent = db.answerContentTbls.Single(c => c.ID == contentID);

            if (value.Length == 0)
            {
                value = answercontent.value;
            }

            if (contentType.ToString().Length != 0)
            {
                if (contentType != 0 && contentType != 1 && contentType != 2)
                {
                    Result.result.code = 2;
                    Result.result.message = ClassCollection.Message.IDEAContent_INCORRECT;
                    Context.Response.Write(js.Serialize(Result).ToLower());
                    return;
                }
            }
            else
            {
                contentType = answercontent.contentType;
            }

            if (answercontent.contentType == 1)
            {
                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getAnswerImagesPath() + answercontent.value)) && answercontent.value != value)
                {
                    try
                    {
                        System.IO.File.Delete(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getAnswerImagesPath() + answercontent.value));
                    }
                    catch
                    {
                        Result.result.code = 3;
                        Result.result.message = ClassCollection.Message.ERROR_DELETE_IMAGE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                    }
                }
            }
            else if (answercontent.contentType == 2)
            {
                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getAnswerVideosPath() + answercontent.value)) && answercontent.value != value)
                {
                    try
                    {
                        System.IO.File.Delete(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getAnswerVideosPath() + answercontent.value));
                    }
                    catch
                    {
                        Result.result.code = 4;
                        Result.result.message = ClassCollection.Message.ERROR_DELETE_VIDEO;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                    }
                }
            }

            if (answercontent.contentType == 1 || answercontent.contentType == 2)
            {
                try
                {
                    var totalPath = HttpContext.Current.Server.MapPath(ClassCollection.Methods.getfilePath()) + value;
                    if (!System.IO.File.Exists(totalPath))
                    {
                        Result.result.code = 5;
                        Result.result.message = ClassCollection.Message.ERROR_EXIST_FILE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                        return;
                    }
                    var ext = value.Split('.').Last();
                    var fileData = File.ReadAllBytes(totalPath);
                    File.Delete(totalPath);

                    var filename = "appinno" + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N") + "." + ext;
                    string fpath = "";
                    value = filename;
                    if (contentType == 1)
                    {
                        fpath = Server.MapPath(ClassCollection.Methods.getAnswerImagesPath() + filename);
                    }
                    else if (contentType == 2)
                    {
                        fpath = Server.MapPath(ClassCollection.Methods.getAnswerVideosPath() + filename);
                    }
                    try
                    {
                        File.WriteAllBytes(fpath, fileData);
                    }
                    catch
                    {
                        Result.result.code = 6;
                        Result.result.message = ClassCollection.Message.ERROR_SAVE_FILE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                        return;
                    }
                }
                catch
                {
                    Result.result.code = 7;
                    Result.result.message = ClassCollection.Message.ERROR_TRANSFER_FILE;
                    Context.Response.Write(js.Serialize(Result).ToLower());
                    return;
                }
            }

            answercontent.contentType = contentType;
            answercontent.value = value;
            db.SubmitChanges();

            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }

        #endregion

        #region Response 

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void registerResponse(string key, long userID, string title, long myIranCompetitionsID)
        {

            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.LongResult Result = new ClassCollection.Model.LongResult();
            Result.result = new Model.Result();
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            if (db.mUserTbls.Any(c => c.ID == userID) == false)
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.USER_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }
            if (db.mUserTbls.Single(c => c.ID == userID).isBlock == true)
            {
                Result.result.code = 2;
                Result.result.message = ClassCollection.Message.USER_BLOCKED;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (!db.myIranTbls.Any(c => c.ID == myIranCompetitionsID && c.isBlock == false))
            {
                Result.result.code = 3;
                Result.result.message = ClassCollection.Message.COMPETITIONS_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            var status = db.getMyIranStatus(myIranCompetitionsID);
            if (status != DataAccessDataContext.MyIranStatus.sending)
            {
                Result.result.code = 4;
                Result.result.message = ClassCollection.Message.ANSWER_INSERT_INCORRECT;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (db.myIranTbls.Any(c => c.ID == myIranCompetitionsID) == false)
            {
                Result.result.code = 3;
                Result.result.message = ClassCollection.Message.COMPETITIONS_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            title = title.TrimStart().TrimEnd();
            if (title.Length == 0)
            {
                Result.result.code = 3;
                Result.result.message = ClassCollection.Message.TEXT_INCORRECT;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }


            var Response = new responseTbl();
            var dt = new DateTime();
            dt = DateTime.Now;
            Response.title = title;
            Response.isBlock = true;
            Response.mUserID = userID;
            Response.regDate = dt;
            Response.myIranID = myIranCompetitionsID;
            db.responseTbls.InsertOnSubmit(Response);
            db.SubmitChanges();

            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Result.value = Response.ID;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void editResponse(string key, long responseID, long userID, string title)
        {
            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.LongResult Result = new ClassCollection.Model.LongResult();
            Result.result = new Model.Result();
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(js.Serialize(Result));
                return;
            }


            if (db.mUserTbls.Any(c => c.ID == userID) == false)
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.USER_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (db.mUserTbls.Single(c => c.ID == userID).isBlock == true)
            {
                Result.result.code = 2;
                Result.result.message = ClassCollection.Message.USER_BLOCKED;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (!db.responseTbls.Any(c => c.ID == responseID && c.mUserID == userID && c.isBlock == false && c.myIranTbl.isBlock == false))
            {
                Result.result.code = 3;
                Result.result.message = ClassCollection.Message.ANSWER_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            var response = db.responseTbls.Single(c => c.ID == responseID);
            var status = db.getMyIranStatus(response.myIranID);
            if (status != DataAccessDataContext.MyIranStatus.sending)
            {
                Result.result.code = 4;
                Result.result.message = ClassCollection.Message.ANSWER_INSERT_INCORRECT;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            title = title.TrimStart().TrimEnd();
            if (title.Length == 0)
            {
                title = response.title;
            }


            response.title = title;
            response.isBlock = true;

            db.SubmitChanges();


            //new service.pushservice().XsendPush("1", "news", news.ID + "", news.title, "all", "false");
            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void deleteResponse(string key, long responseID, long userID)
        {
            Model.Result Result = new Model.Result();
            JavaScriptSerializer js = new JavaScriptSerializer();
            var db = new DataAccessDataContext();

            if (!Methods.checkUserKey(key))
            {
                Result.code = -1;
                Result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            if (db.mUserTbls.Any(c => c.ID == userID) == false)
            {
                Result.code = 1;
                Result.message = ClassCollection.Message.USER_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (db.mUserTbls.Single(c => c.ID == userID).isBlock == true)
            {
                Result.code = 2;
                Result.message = ClassCollection.Message.USER_BLOCKED;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (!db.responseTbls.Any(c => c.ID == responseID && c.mUserID == userID && c.myIranTbl.isBlock == false))
            {
                Result.code = 3;
                Result.message = ClassCollection.Message.ANSWER_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            var response = db.responseTbls.Single(c => c.ID == responseID);
            var status = db.getMyIranStatus(response.myIranID);
            if (status != DataAccessDataContext.MyIranStatus.sending)
            {
                Result.code = 4;
                Result.message = ClassCollection.Message.ANSWER_INSERT_INCORRECT;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            var contentlist = db.responseContentTbls.Where(c => c.responseID == responseID);
            foreach (var item in contentlist)
            {
                deleteResponseContent("1", item.ID);
            }

            db.responseTbls.DeleteOnSubmit(response);
            db.SubmitChanges();

            Result.code = 0;
            Result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void registerResponseContent(string key, long responseID, int contentType, string value)
        {
            ClassCollection.Model.LongResult Result = new ClassCollection.Model.LongResult();
            Result.result = new Model.Result();
            JavaScriptSerializer js = new JavaScriptSerializer();
            DateTime dt = new DateTime();
            dt = DateTime.Now;
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(js.Serialize(Result));
                return;
            }
            if (!db.responseTbls.Any(c => c.ID == responseID && c.myIranTbl.isBlock == false))
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.ANSWER_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (contentType != 0 && contentType != 1 && contentType != 2)
            {
                Result.result.code = 2;
                Result.result.message = ClassCollection.Message.IDEAContent_INCORRECT;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            if (value.Length == 0)
            {
                Result.result.code = 3;
                Result.result.message = ClassCollection.Message.TEXT_INCORRECT;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            var tmp = new responseContentTbl();

            if (contentType == 0)
            {
                tmp.value = "<p>" + value + "</p>";
            }
            if (contentType == 1 || contentType == 2)
            {
                try
                {
                    var totalPath = HttpContext.Current.Server.MapPath(ClassCollection.Methods.getfilePath()) + value;
                    if (!System.IO.File.Exists(totalPath))
                    {
                        Result.result.code = 4;
                        Result.result.message = ClassCollection.Message.ERROR_EXIST_FILE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                        return;
                    }
                    var ext = value.Split('.').Last();
                    var fileData = File.ReadAllBytes(totalPath);
                    File.Delete(totalPath);

                    var filename = "appinno" + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N") + "." + ext;
                    string fpath = "";
                    tmp.value = filename;
                    if (contentType == 1)
                    {
                        fpath = Server.MapPath(ClassCollection.Methods.getMyIranImagesPath() + filename);
                    }
                    else if (contentType == 2)
                    {
                        fpath = Server.MapPath(ClassCollection.Methods.getMyIranVideosPath() + filename);
                    }
                    try
                    {
                        File.WriteAllBytes(fpath, fileData);
                    }
                    catch
                    {
                        Result.result.code = 5;
                        Result.result.message = ClassCollection.Message.ERROR_SAVE_FILE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                        return;
                    }
                }
                catch
                {
                    Result.result.code = 6;
                    Result.result.message = ClassCollection.Message.ERROR_TRANSFER_FILE;
                    Context.Response.Write(js.Serialize(Result).ToLower());
                    return;
                }
            }

            tmp.responseID = responseID;
            tmp.contentType = contentType;
            if (!db.responseContentTbls.Any(c => c.responseID == responseID))
            {
                tmp.pri = 0;
            }
            else
            {
                tmp.pri = db.responseContentTbls.Where(c => c.responseID == responseID).Max(c => c.pri) + 1;
            }
            tmp.regDate = dt;
            db.responseContentTbls.InsertOnSubmit(tmp);
            db.SubmitChanges();

            //new service.pushservice().XsendPush("1", "news", news.ID + "", news.title, "all", "false");

            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Result.value = tmp.ID;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void deleteResponseContent(string key, long contentID)
        {
            ClassCollection.Model.Result Result = new ClassCollection.Model.Result();
            JavaScriptSerializer js = new JavaScriptSerializer();
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.code = -1;
                Result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            if (!db.responseContentTbls.Any(c => c.ID == contentID))
            {
                Result.code = 1;
                Result.message = ClassCollection.Message.IDEAContent_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            var deletingPar = db.responseContentTbls.Single(c => c.ID == contentID);

            if (deletingPar.contentType == 1)
            {
                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getMyIranImagesPath() + deletingPar.value)))
                {
                    try
                    {
                        System.IO.File.Delete(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getMyIranImagesPath() + deletingPar.value));
                    }
                    catch
                    {
                        Result.code = 2;
                        Result.message = ClassCollection.Message.ERROR_DELETE_IMAGE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                    }
                }
            }
            else if (deletingPar.contentType == 2)
            {
                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getMyIranVideosPath() + deletingPar.value)))
                {
                    try
                    {
                        System.IO.File.Delete(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getMyIranVideosPath() + deletingPar.value));
                    }
                    catch
                    {
                        Result.code = 3;
                        Result.message = ClassCollection.Message.ERROR_DELETE_VIDEO;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                    }
                }
            }
            db.responseContentTbls.DeleteOnSubmit(deletingPar);
            db.SubmitChanges();

            Result.code = 0;
            Result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void editResponseContent(string key, long contentID, int contentType, string value)
        {
            ClassCollection.Model.LongResult Result = new ClassCollection.Model.LongResult();
            Result.result = new Model.Result();
            JavaScriptSerializer js = new JavaScriptSerializer();
            var db = new DataAccessDataContext();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                Context.Response.Write(js.Serialize(Result));
                return;
            }

            if (!db.responseContentTbls.Any(c => c.ID == contentID))
            {
                Result.result.code = 1;
                Result.result.message = ClassCollection.Message.ANSWER_NOT_EXIST;
                Context.Response.Write(js.Serialize(Result).ToLower());
                return;
            }

            var responsecontent = db.responseContentTbls.Single(c => c.ID == contentID);

            if (value.Length == 0)
            {
                value = responsecontent.value;
            }

            if (contentType.ToString().Length != 0)
            {
                if (contentType != 0 && contentType != 1 && contentType != 2)
                {
                    Result.result.code = 2;
                    Result.result.message = ClassCollection.Message.IDEAContent_INCORRECT;
                    Context.Response.Write(js.Serialize(Result).ToLower());
                    return;
                }
            }
            else
            {
                contentType = responsecontent.contentType;
            }

            if (responsecontent.contentType == 1)
            {
                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getResponseImagesPath() + responsecontent.value)) && responsecontent.value != value)
                {
                    try
                    {
                        System.IO.File.Delete(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getResponseVideosPath() + responsecontent.value));
                    }
                    catch
                    {
                        Result.result.code = 3;
                        Result.result.message = ClassCollection.Message.ERROR_DELETE_IMAGE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                    }
                }
            }
            else if (responsecontent.contentType == 2)
            {
                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getResponseVideosPath() + responsecontent.value)) && responsecontent.value != value)
                {
                    try
                    {
                        System.IO.File.Delete(HttpContext.Current.Server.MapPath(ClassCollection.Methods.getResponseVideosPath() + responsecontent.value));
                    }
                    catch
                    {
                        Result.result.code = 4;
                        Result.result.message = ClassCollection.Message.ERROR_DELETE_VIDEO;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                    }
                }
            }

            if (responsecontent.contentType == 1 || responsecontent.contentType == 2)
            {
                try
                {
                    var totalPath = HttpContext.Current.Server.MapPath(ClassCollection.Methods.getfilePath()) + value;
                    if (!System.IO.File.Exists(totalPath))
                    {
                        Result.result.code = 5;
                        Result.result.message = ClassCollection.Message.ERROR_EXIST_FILE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                        return;
                    }
                    var ext = value.Split('.').Last();
                    var fileData = File.ReadAllBytes(totalPath);
                    File.Delete(totalPath);

                    var filename = "appinno" + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N") + "." + ext;
                    string fpath = "";
                    value = filename;
                    if (contentType == 1)
                    {
                        fpath = Server.MapPath(ClassCollection.Methods.getResponseImagesPath() + filename);
                    }
                    else if (contentType == 2)
                    {
                        fpath = Server.MapPath(ClassCollection.Methods.getResponseVideosPath() + filename);
                    }
                    try
                    {
                        File.WriteAllBytes(fpath, fileData);
                    }
                    catch
                    {
                        Result.result.code = 6;
                        Result.result.message = ClassCollection.Message.ERROR_SAVE_FILE;
                        Context.Response.Write(js.Serialize(Result).ToLower());
                        return;
                    }
                }
                catch
                {
                    Result.result.code = 7;
                    Result.result.message = ClassCollection.Message.ERROR_TRANSFER_FILE;
                    Context.Response.Write(js.Serialize(Result).ToLower());
                    return;
                }
            }

            responsecontent.contentType = contentType;
            responsecontent.value = value;
            db.SubmitChanges();

            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Context.Response.Write(js.Serialize(Result).ToLower());
            return;
        }

        #endregion


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void adminMessage(string key)
        {

            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.AdminMessageResult Result = new ClassCollection.Model.AdminMessageResult();
            Result.result = new Model.Result();

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.result.code = -1;
                Result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
               
                Context.Response.Write(js.Serialize(Result));
                return;
            }
            var db = new DataAccessDataContext();
            Result.result.code = 0;
            Result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            Result.message = db.settingTbls.Single(c => c.title == "adminmessage").value;
            Result.image = db.settingTbls.Single(c => c.title == "adminimage").value;
            Context.Response.Write(js.Serialize(Result));
            return;

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getPolicy(string key)
        {

            Context.Response.ContentEncoding = Encoding.UTF8;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ClassCollection.Model.Result Result = new ClassCollection.Model.Result();
         

            if (!ClassCollection.Methods.checkUserKey(key))
            {
                Result.code = -1;
                Result.message = ClassCollection.Message.OPERATION_NO_ACCESS;

                Context.Response.Write(js.Serialize(Result));
                return;
            }
            var db = new DataAccessDataContext();
            Result.code = 0;
            Result.message = db.settingTbls.Single(c => c.title == "policy").value;
            Context.Response.Write(js.Serialize(Result));
            return;

        }
    }
}
