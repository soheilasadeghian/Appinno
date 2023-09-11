using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;

namespace AppinnoNew.ClassCollection
{
    public static class Methods
    {
        static readonly string PasswordHash = "P@@Sw0rd";
        static readonly string SaltKey = "S@LT&KEY";
        static readonly string VIKey = "@1B2c3D4e5F6g7H8";

        public static string getreportImagesPath()
        {
            return ConfigurationManager.AppSettings["ReportImagePath"];
        }
        public static string getdownloadImagesPath()
        {
            return ConfigurationManager.AppSettings["DownloadImagePath"];
        }
        public static string getdownloadFilePath()
        {
            return ConfigurationManager.AppSettings["DownloadFilePath"];
        }
        public static string geteventsImagesPath()
        {
            return ConfigurationManager.AppSettings["EventImagePath"];
        }
        public static string geteventsVideosPath()
        {
            return ConfigurationManager.AppSettings["EventVideoPath"];
        }
        public static string getpublicationImagesPath()
        {
            return ConfigurationManager.AppSettings["PublicationImagePath"];
        }
        public static string getpublicationVideosPath()
        {
            return ConfigurationManager.AppSettings["PublicationVideoPath"];
        }
        public static string getNewsImagesPath()
        {
            return ConfigurationManager.AppSettings["NewsImagePath"];
        }
        public static string getNewsVideosPath()
        {
            return ConfigurationManager.AppSettings["NewsVideoPath"];
        }
        public static string getPollImagesPath()
        {
            return ConfigurationManager.AppSettings["PollImagePath"];
        }
        public static string getioImagesPath()
        {
            return ConfigurationManager.AppSettings["IoImagePath"];
        }
        public static string getioVideosPath()
        {
            return ConfigurationManager.AppSettings["IoVideoPath"];
        }
        public static string getmessagePath()
        {
            return ConfigurationManager.AppSettings["MessagePath"];
        }
        public static string geticanImagesPath()
        {
            return ConfigurationManager.AppSettings["IcanImagePath"];
        }
        public static string geticanVideosPath()
        {
            return ConfigurationManager.AppSettings["IcanVideoPath"];
        }
        public static string geticanFilePath()
        {
            return ConfigurationManager.AppSettings["IcanFilePath"];
        }
        public static string getBestIdeaCompetitionsImagesPath()
        {
            return ConfigurationManager.AppSettings["BestIdeaCompetitionsImagePath"];
        }
        public static string getBestIdeaCompetitionsVideosPath()
        {
            return ConfigurationManager.AppSettings["BestIdeaCompetitionsVideoPath"];
        }
        public static string getCreativityCompetitionImagesPath()
        {
            return ConfigurationManager.AppSettings["CreativityCompetitionImagePath"];
        }
        public static string getCreativityCompetitionVideosPath()
        {
            return ConfigurationManager.AppSettings["CreativityCompetitionVideoPath"];
        }
        public static string getMyIranImagesPath()
        {
            return ConfigurationManager.AppSettings["MyIranImagePath"];
        }
        public static string getMyIranVideosPath()
        {
            return ConfigurationManager.AppSettings["MyIranVideoPath"];
        }
        public static string getIdeaImagesPath()
        {
            return ConfigurationManager.AppSettings["IdeaImagesPath"];
        }
        public static string getIdeaVideosPath()
        {
            return ConfigurationManager.AppSettings["IdeaVideoPath"];
        }
        public static string getAnswerImagesPath()
        {
            return ConfigurationManager.AppSettings["AnswerImagesPath"];
        }
        public static string getAnswerVideosPath()
        {
            return ConfigurationManager.AppSettings["AnswerVideoPath"];
        }
        public static string getResponseImagesPath()
        {
            return ConfigurationManager.AppSettings["ResponseImagesPath"];
        }
        public static string getResponseVideosPath()
        {
            return ConfigurationManager.AppSettings["ResponseVideoPath"];
        }

        public static string getAdminImagePath()
        {
            return ConfigurationManager.AppSettings["AdminImagePath"];
        }
        //soheila-start-poll
        public static string getuploadPath()
        {
            return ConfigurationManager.AppSettings["uploadPath"];
        }
        public static string getPollImagePath()
        {
            return ConfigurationManager.AppSettings["PollImagePath"];
        }
        public static string getPollVideoPath()
        {
            return ConfigurationManager.AppSettings["PollVideoPath"];
        }
        public static string toFriendlyDate(DateTime date)
        {
            var subdate = DateTime.Now - date;
            string result = "";
            if (subdate.TotalMinutes < 60)
            {
                result = Persia.Calendar.ConvertToPersian(date).ToRelativeDateString("M");
            }
            else if (subdate.TotalHours < 24)
            {
                result = Persia.Calendar.ConvertToPersian(date).ToRelativeDateString("H");
            }
            else if (subdate.Days < 7)
            {
                //result = Persia.Calendar.ConvertToPersian(date).ToRelativeDateString("TY") + " - " + Persia.Calendar.ConvertToPersian(date).ToString("H");
                result = Persia.Calendar.ConvertToPersian(date).ToRelativeDateString("TY");
            }
            else
            {
                //result = Persia.Calendar.ConvertToPersian(date).ToString("m") + " - " + Persia.Calendar.ConvertToPersian(date).ToString("H");
                result = Persia.Calendar.ConvertToPersian(date).ToString("m");
            }

            return result;
        }
        public static string toFriendlyDateF(DateTime date)
        {
            var subdate = date - DateTime.Now;
            string result = "";
            if (subdate.TotalMinutes < 60)
            {
                result = ((int)subdate.TotalMinutes) + " دقیقه دیگر";
            }
            else if (subdate.TotalHours < 24)
            {
                result = ((int)subdate.TotalHours) + " ساعت دیگر";
            }
            else
            {
                result = ((int)subdate.Days) + " روز دیگر";
            }


            return result;
        }
        public static string getfilePath()
        {
            return ConfigurationManager.AppSettings["filePath"];
        }

        public static string saveBase64FileToServer(string data, string path)
        {
            string fullName = null;

            try
            {
                Byte[] bytes = Convert.FromBase64String(data);
                File.WriteAllBytes(path, bytes);
                fullName = path;
            }
            catch (Exception rrr)
            {
                fullName = "errr " + rrr.Message;
            }

            return fullName;
        }
        //soheila-end-poll
        //public static long getAdminID()
        //{
        //    return 12;
        //}
        public static string getGCMAPIKEY()
        {
            return ConfigurationManager.AppSettings["GCMAPIKEY"];
        }
        public static string Encrypt(string plainText)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));

            byte[] cipherTextBytes;

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherTextBytes = memoryStream.ToArray();
                    cryptoStream.Close();
                }
                memoryStream.Close();
            }
            return Convert.ToBase64String(cipherTextBytes);
        }
        public static string Decrypt(string encryptedText)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

            var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));
            var memoryStream = new MemoryStream(cipherTextBytes);
            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
        }
        public static bool isLogin(HttpContext Context)
        {
            if (Context.Session["user"] == null)
                return false;
            else
                return true;
        }
        public static userTbl getUserInSession(HttpContext Context)
        {
            return ((userTbl)(Context.Session["user"]));
        }
        public static string md5(string sPassword)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(sPassword);
            bs = x.ComputeHash(bs);
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToLower());
            }
            return s.ToString();
        }
        public static void sendMail(string to, string subject, string body)
        {
            new Thread(() =>
            {
                try
                {
                    var fromEmailAddress = ConfigurationManager.AppSettings["FromEmailAddress"].ToString();
                    var fromEmailDisplayName = ConfigurationManager.AppSettings["FromEmailDisplayName"].ToString();
                    var fromEmailPassword = ConfigurationManager.AppSettings["FromEmailPassword"].ToString();
                    var smtpHost = ConfigurationManager.AppSettings["SMTPHost"].ToString();
                    var smtpPort = ConfigurationManager.AppSettings["SMTPPort"].ToString();

                    MailMessage message = new MailMessage(new MailAddress(fromEmailAddress, fromEmailDisplayName), new MailAddress(to));
                    message.Subject = subject;
                    message.IsBodyHtml = true;
                    message.Body = body;

                    var client = new SmtpClient();
                    client.Credentials = new NetworkCredential(fromEmailAddress, fromEmailPassword);
                    client.Host = smtpHost;
                    client.EnableSsl = false;
                    client.Port = !string.IsNullOrEmpty(smtpPort) ? Convert.ToInt32(smtpPort) : 0;
                    client.Send(message);
                }
                catch
                {

                }
            }).Start();
        }
        public static void sendSms(string to, string text)
        {
            new Thread(() =>
            {
                try
                {
                    var smsUserName = ConfigurationManager.AppSettings["smsUserName"];
                    var smsPassword = ConfigurationManager.AppSettings["smsPassword"];
                    var smsFrom = ConfigurationManager.AppSettings["smsFrom"];

                    smsProxy.Send sms = new smsProxy.Send();
                    sms.SendSimpleSMS2(smsUserName, smsPassword, to, smsFrom, text, false);

                }
                catch
                {

                }
            }).Start();
        }
        public static bool isEmail(string email)
        {
            try
            {
                MailAddress m = new MailAddress(email);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public static bool checkUserKey(string value)
        {
            return ConfigurationManager.AppSettings["UserKey"] == value;
        }
        public static string randomValidationCode()
        {
            string _numbers = "0123456789";
            Random random = new Random(DateTime.Now.Millisecond);

            StringBuilder builder = new StringBuilder(6);
            string numberAsString = "";

            for (var i = 0; i < 6; i++)
            {
                builder.Append(_numbers[random.Next(0, _numbers.Length)]);
            }

            numberAsString = builder.ToString();
            return numberAsString;

        }
        public static void unBlocker(messageTbl message, mUserTbl sender,string pushtoString, bool toAll, bool toPartner)
        {
            var db = new DataAccessDataContext();
            var message1 = db.messageTbls.Single(c => c.ID == message.ID);
            message1.isBlock = false;
            db.SubmitChanges();


            if (!toPartner && !toAll)
            {
                new service.pushservice().XsendPush("1", "mail_" + message.ID, sender.name + " " + sender.family, message.text.Substring(0, Math.Min(message.text.Length, 15)) + "...", "subgroups", pushtoString);
            }
            else if (toPartner == true)
            {
                new service.pushservice().XsendPush("1", "mail_" + message.ID, sender.name + " " + sender.family, message.text.Substring(0, Math.Min(message.text.Length, 15)) + "...", "topartners", null);
            }
            else if (toAll == true)
            {
                new service.pushservice().XsendPush("1", "mail_" + message.ID, sender.name + " " + sender.family, message.text.Substring(0, Math.Min(message.text.Length, 15)) + "...", "toall", null);
            }

        }

    }
}