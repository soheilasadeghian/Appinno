using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppinnoNew.ClassCollection
{
    public static class Message
    {
        public static String LOGIN_FIRST = "ابتدا وارد سایت شوید";
        public static String OPERATION_NO_ACCESS = "شما دسترسی لازم به این عملیات را ندارید";
        public static String OPERATION_SUCCESS = "عملیات با موفقیت انجام شد";
        public static String USER_EXIST = "کاربر مورد نظر وجود دارد";
        public static String USER_NOT_EXIST = "کاربر درخواستی وجود ندارد";
        public static String USER_VOTED = "کاربر قبلا به این ایده رای داده است";
        public static String USER_CANT_VOTE = "امکان رای دادن به این ایده وجود ندارد";
        public static String DATE_INCORRECT = "تاریخ وارد شده صحیح نمی باشد";
        public static String USER_BLOCKED = "کاربر درخواستی توسط مدیر مسدود شده است";
        public static String ACCESS_NOT_EXIST = "نقش مورد نظر وجود ندارد";
        public static String ACCESS_IN_USE = "نقش مورد نظر درحال استفاده است";
        public static String TYPE_INCORRECT = "نوع ورودی مورد نظر اشتباه است";
        public static String EDIT_INCORRECT = "امکان ویرایش مسابقه وجود ندارد.";
        public static String DELETE_OPERATOR_INCORRECT = "کاربر مورد نظر در حال استفاده از سیستم می باشد";


        public static String COMMENT_TEXT_INCORRECT = "طول متن مورد نظر مجاز نمی باشد";
        public static String COMMENT_NOT_EXIST = "نظر وجود ندارد";

        public static String NAME_INCORRECT = "نام وارد شده صحیح نمی باشد";
        public static String FAMILY_INCORRECT = "نام خانوادگی صحیح نمی باشد";
        public static String EMAILTELL_INCORRECT = "نام کاربری صحیح نمی باشد";
        public static String PASSWORD_INCORRECT = "کلمه عبور صحیح نمی باشد";
        public static String GROUP_TITLE_INCORRECT = "عنوان گروه صحیح نمی باشد";
        public static String GROUP_NOT_EXIST = "گروه مورد نظر وجود ندارد";
        public static String SUBGROUP_TITLE_INCORRECT = "عنوان زیرگروه صحیح نمی باشد";
        public static String SUBGROUP_NOT_EXIST = "زیرگروه مورد نظر وجود ندارد";
        public static String DELETE_SUBCATEGORY_FIRST = "ابتدا تمام زیرگروه های این گروه را حذف نمایید";
        public static String DELETE_USER_FIRST = "ابتدا تمام کاربران این زیرگروه را حذف نمایید";
        public static String DATE_INCORRECT_PRIORITY = "ترتیب تاریخ ها وارد شده ناصحیح است.";
        public static String DATE_INCORRECT_NOW = "تاریخ شروع مسابقه نباید قبل از تاریخ جاری باشد.";

        //soheila-start-registerNews
        public static String SUBGROUPS_NOT_EXIST = "زیر گروه انتخاب نشده است";
        public static String TAGS_NOT_EXIST = "برچسب صحیح انتخاب نشده است";
        public static String NEWSID_NOT_EXIST = "خبر مورد نظر انتخاب نشده است";
        public static String NEWSContent_NOT_EXIST = "جزییات خبر مورد نظر وجود ندارد";
        public static String NEWSContent_INCORRECT = "جزییات خبر مورد نظر اشتباه است";
        //soheila-start-registerNews

        //soheila-start-registerEvent
        public static String EVENTID_NOT_EXIST = "رویداد مورد نظر انتخاب نشده است";
        public static String EVENTContent_NOT_EXIST = "جزییات رویداد مورد نظر وجود ندارد";
        public static String EVENTContent_INCORRECT = "جزییات رویداد مورد نظر اشتباه است";
        //soheila-start-registerEvent

        //soheila-start-registerIo
        public static String IOID_NOT_EXIST = "سازمان مورد نظر انتخاب نشده است";
        public static String IOContent_NOT_EXIST = "جزییات سازمان مورد نظر وجود ندارد";
        public static String IOContent_INCORRECT = "جزییات سازمان مورد نظر اشتباه است";
        //soheila-start-registerIo

        //soheila-start-registerPub
        public static String PUBID_NOT_EXIST = "انتشارات مورد نظر انتخاب نشده است";
        public static String PUBContent_NOT_EXIST = "جزییات انتشارات مورد نظر وجود ندارد";
        public static String PUBContent_INCORRECT = "جزییات انتشارات مورد نظر اشتباه است";
        //soheila-start-registerPub

        //soheila-start-registerDownload
        public static String DOWNLOADID_NOT_EXIST = "دانلود فایل مورد نظر انتخاب نشده است";
        public static String DOWNLOADContent_NOT_EXIST = "جزییات دانلود مورد نظر وجود ندارد";
        public static String DOWNLOADContent_INCORRECT = "جزییات دانلود مورد نظر اشتباه است";
        //soheila-start-registerDownload

        //soheila-start-registerreport
        public static String CHARTID_NOT_EXIST = "نمودار مورد نظر انتخاب نشده است";
        public static String CHARTContent_NOT_EXIST = "جزییات نمودار مورد نظر وجود ندارد";
        public static String CHARTContent_INCORRECT = "جزییات نمودار مورد نظر اشتباه است";
        //soheila-start-registerreport

        //soheila-start-registerIcan
        public static String ICANID_NOT_EXIST = "توانایی مورد نظر انتخاب نشده است";
        public static String ICANContent_NOT_EXIST = "جزییات توانایی مورد نظر وجود ندارد";
        public static String ICANContent_INCORRECT = "جزییات توانایی مورد نظر اشتباه است";
        //soheila-start-registerIcan

        //soheila-start-registerIcan
        public static String IDEAID_NOT_EXIST = "ایده مورد نظر انتخاب نشده است";
        public static String IDEAContent_NOT_EXIST = "جزییات ایده مورد نظر وجود ندارد";
        public static String IDEAContent_INCORRECT = "جزییات مورد نظر اشتباه است";
        //soheila-start-registerIcan

        public static String FILE_INVALID = "فایل معتبر نیست.";
        public static String PARTNAER_NOT_EXIST = "همکار مورد نظر وجود ندارد.";
        public static String PARTNAER_MOBILE_EXIST = "شماره موبایل وارد شده تکراری است";

        public static String PUSH_MESSAGE_NOT_EXIST = "پیام مورد نظر وجود ندارد.";

        public static String TOKEN_ERROR_NOT_EXIST = "توکن مورد نظر وجود ندارد";

        public static String PUSH_TOKEN_INCORRECT = "توکن وارد شده صحیح نمی باشد";
        public static String DEVICE_ID_INCORRECT = "توکن وارد شده صحیح نمی باشد";
        public static String INPUT_ERROR_CUSTOMER_LATITUD_LONGITUD_EMPTY = "پر نمودن هردو مقدار مختصات الزامی می باشد";
        public static String PUSH_COMMAND_INCORRECT = "فرمت گیرندگان پوش صحیح نمی باشد.";
        public static String PUSH_FAILED = "خطا در ارسال پوش.";

        public static String NEWS_NOT_EXIST = "خبر مورد نظر وجود ندارد.";
        public static String IO_NOT_EXIST = "سازمان مورد نظر وجود ندارد.";
        public static String PUBLICATIONS_NOT_EXIST = "انتشارات مورد نظر وجود ندارد.";
        public static String EVENTS_NOT_EXIST = "رویداد مورد نظر وجود ندارد.";
        public static String DOWNLOAD_NOT_EXIST = "پست دانلود مورد نظر وجود ندارد.";
        public static String CHART_NOT_EXIST = "نمودار مورد نظر وجود ندارد.";
        public static String ICAN_NOT_EXIST = "توانایی مورد نظر وجود ندارد.";
        public static String COMPETITIONS_NOT_EXIST = "مسابقه مورد نظر وجود ندارد.";
        public static String IDEA_NOT_EXIST = "ایده مورد نظر وجود ندارد.";
        public static String IDEA_INSERT_INCORRECT = "امکان ثبت ایده وجود ندارد.";
        public static String ANSWER_NOT_EXIST = "پاسخ مورد نظر وجود ندارد.";
        public static String ANSWER_INSERT_INCORRECT = "امکان ثبت پاسخ وجود ندارد.";
        public static String ANSWER_WINNER_EXIST = "برای این مسابقه برنده ثبت شده است.";
        public static String ANSWER_WINNER_EXIST_ISSENDING = "مسابقه درحال برگزاری می باشد.";
        public static String WINNER_EDIT_INCORRECT = "برنده قابل ویرایش نمی باشد.";
        public static String WINNER_ISCORRECT_INCORRECT = "پاسخ ناصحیح را نمی توانید برنده اعلام نمایید.";
        public static String ANSWER_WINNER_STATUS ="مسابقه به اتمام نرسیده است.نمیتوانید برای مسابقه برنده اعلام نمایید.";



        public static String ERROR_DELETE_IMAGE = "خطا در حذف تصویر.";
        public static String ERROR_DELETE_VIDEO = "خطا در حذف فیلم.";
        public static String ERROR_DELETE_FILE = "خطا در حذف فایل.";
        public static String ERROR_EXIST_FILE = "فایل مورد نظر موجود نمی باشد.";
        public static String ERROR_SAVE_FILE = "خطا در ذخیره فایل.";
        public static String ERROR_TRANSFER_FILE = "خطا در جابه جایی فایل.";

        public static String ERROR_INPUT_PARAMETER = "پارامتر شماره " + "{0}" + " ناصحیح می باشد";
        public static String VALIDATION_CODE_SENT = "کد فعال سازی برای کاربر ارسال شد.";
        public static String VALIDATION_CODE_INCORRECT = "کد فعال سازی صحیح نمی باشد.";
        public static String VALIDATION_CODE_CORRECT = "کد فعال سازی صحیح می باشد.";

        public static String EMAIL_EXIST = "ایمیل وجود دارد.";
        public static String MOBILE_EXIST = "شماره موبایل وجود دارد.";
        public static String MOBILE_EXIST_INContactList = "شماره موبایل در دفترچه تلفن وجود دارد.";
        public static String NEW_PASSWORD_SENT = "کلمه عبور جدید با موفقیت ارسال شد.";
        public static String EMAIL_INCORRECT = "ایمیل وارد شده صحیح نمی باشد";
        public static String MOBILE_INCORRECT = "موبایل وارد شده صحیح نمی باشد";
        public static String TAG_EXIST = "تگ وارد شده تکراری می باشد. ";
        public static String LEVEL_INCORRECT = "ایمیل وارد شده صحیح نمی باشد";
        public static String OPTIONALMOBILE_EXIST = "شماره تلفن اختیاری صحیح نمی باشد."; 
        public static String INNERTELL_EXIST = "شماره داخلی صحیح نمی باشد.";

        //soheila-start-poll
        public static String INPUT_ERROR_PACKET_COUNT_INCORRECT = "تعداد بسته های ارسالی جهت آپلود  صحیح نیست";
        public static String INPUT_ERROR_FILE_FORMAT_INCORRECT = "نوع فایل مورد نظر جهت آپلود فایل صحیح نیست";
        public static String UPLOAD_TOKEN_SENT = "توکن با موفقیت ارسال شد";
        public static String UPLOAD_FILE_SUCCESS = "فایل مورد نظر با موفقیت آپلود شد";
        public static String UPLOAD_FILE_FAILED = "فایل مورد نظر بارگذاری نشد";

        public static String UPLOAD_PACKET_SUCCESS = "بسته مورد نظر از فایل با موفقیت آپلود شد";
        public static String UPLOAD_PACKET_ERROR_OVERFLOW = "آپلود فایل مورد نظر قبلا به پایان رسیده است";
        public static String UPLOAD_FILE_ERROR_NOT_FINISHED = "آپلود فایل مورد نظر هنوز به پایان نرسیده است";

        public static String TITLE_INCORRECT = "عنوان صحیح نمی باشد";
        public static String TEXT_INCORRECT = "متن صحیح نمی باشد";

        public static String BASESONG_AUTHER_NOT_EXIST = "آهنگ ساز آهنگ پایه وجود ندارد";
        public static String BASESONG_AUTHER_INUSE = "آهنگ ساز آهنگ پایه درحال استفاده می باشد";

        public static String BASESONG_TYPE_NOT_EXIST = "سبک آهنگ پایه وجود ندارد";
        public static String BASESONG_GROUP_NOT_EXIST = "گروه آهنگ پایه وجود ندارد";

        public static String BASESONG_NOT_EXIST = "آهنگ پایه وجود ندارد";
        public static String BASESONG_INUSE = "آهنگ پایه درحال استفاده می باشد";

        public static String POEM_INUSE = "شعر درحال استفاده می باشد"; 
        public static String POEM_NOT_EXIST = "شعر وجود ندارد"; 

        public static String POLL_NOT_EXIST = "نظرسنجی مورد نظر وجود ندارد";
        public static String POLL_BLOCKED = "نظرسنجی تایید نشده است";
        public static String POLL_FINISHED = "نظرسنجی به پایان رسیده است";
        public static String POLL_IS_RUNNING = "نظرسنجی درحال برگذاری می باشد";
        public static String POLL_ALLREADY_DONE = "شما قبلا در این نظرسنجی شرکت نموده اید";

        public static String POLL_CONTENT_INCORRECT = "در هر نظرسنجی تنها قادر هستید یک فیلم یا یک عکس قرار دهید و امکان استفاده از هردو مورد وجود ندارد.";
        public static String VIEWER_NOT_EXIST = "یکی از کاربران انتخاب شده وجود ندارد";
        public static String VIEWERS_NOT_EXIST = "کاربری انتخاب نشده است";

        public static String OPTION_NOT_EXIST = "گزینه درخواستی وجود ندارد";

        public static String POLL_NOT_ACCESS = "امکان ارسال نظر برای این نظرسنجی برای کاربر مورد نظر وجود ندارد";

        //soheila-end-poll
    }
}