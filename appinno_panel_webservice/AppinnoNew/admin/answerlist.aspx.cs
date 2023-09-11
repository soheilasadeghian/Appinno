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
    public partial class answerlist : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var db = new DataAccessDataContext();
            if (ClassCollection.Methods.isLogin(Context))
            {
                var user = ClassCollection.Methods.getUserInSession(HttpContext.Current);
                var permission = ClassCollection.Accessing.Deserialize(db.accessTbls.Single(c => c.ID == user.accessID).per);

                if (!permission.creativityCompetition.access.showlist)
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
        public static string getAllAnswer(string regDateFrom, string regDateTo
            , string answerstate
            , string fillterString, string orderType,
            string sortby, int pageIndex, int creativityCompetitionId, bool justWinner = false)
        {
            ClassCollection.Model.AnswerAdminResult result = new ClassCollection.Model.AnswerAdminResult();
            result.result = new ClassCollection.Model.Result();
            const int pageSize = 30;
            var db = new DataAccessDataContext();

            int skipCount = (pageIndex - 1) * pageSize;

            if (!ClassCollection.Methods.isLogin(HttpContext.Current))
            {
                result.result.code = 1;
                result.result.message = ClassCollection.Message.LOGIN_FIRST;
                return new JavaScriptSerializer().Serialize(result);
            }

            if (!db.creativityCompetitionTbls.Any(c => c.ID == creativityCompetitionId))
            {
                result.result.code = 3;
                result.result.message = ClassCollection.Message.COMPETITIONS_NOT_EXIST;
                return new JavaScriptSerializer().Serialize(result);
            }
            var user = ClassCollection.Methods.getUserInSession(HttpContext.Current);
            var permission = ClassCollection.Accessing.Deserialize(db.accessTbls.Single(c => c.ID == user.accessID).per);

            if (!permission.creativityCompetition.access.showlist)
            {
                result.result.code = 2;
                result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }

            if (justWinner)
            {
                var answers = db.answerTbls.Where(c => c.creativityCompetitionID == creativityCompetitionId && c.isCorrect == true && c.isWinner == true && c.answerContentTbls.Any()).Take(1).OrderBy(c => c.regDate).AsQueryable();
                if (answers.Count() != 0)
                {
                    var winnerTitle = answers.Single(c => c.creativityCompetitionID == creativityCompetitionId && c.isCorrect == true && c.isWinner == true && c.isBlock == false).title;
                    result.winnerTitle = winnerTitle;
                    var winnerID = answers.Single(c => c.creativityCompetitionID == creativityCompetitionId && c.isCorrect == true && c.isWinner == true && c.isBlock == false).ID;
                    result.winnerID = winnerID;
                }
                result.pageTitle = db.creativityCompetitionTbls.Single(c => c.ID == creativityCompetitionId).title;
                result.answer = new List<Model.AnswerAdmin>();
                foreach (var item in answers)
                {
                    var tmp = new ClassCollection.Model.AnswerAdmin();
                    tmp.ID = item.ID;
                    tmp.title = item.title;
                    tmp.isBlock = item.isBlock;
                    tmp.mUserName = item.mUserTbl.name + " " + item.mUserTbl.family;
                    tmp.regDate = Persia.Calendar.ConvertToPersian(item.regDate).ToString("m") + " - " + Persia.Calendar.ConvertToPersian(item.regDate).ToString("H");
                    result.answer.Add(tmp);
                }
                
                result.result.code = 0;
                result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
                return new JavaScriptSerializer().Serialize(result);
            }

            #region  regDate

            var regDatefromD = new DateTime();
            var regDatetoD = new DateTime();
            if (regDateFrom != "")
            {
                try
                {
                    var tt = regDateFrom.Split('/');
                    regDatefromD = Persia.Calendar.ConvertToGregorian(int.Parse(tt[0]), int.Parse(tt[1]), int.Parse(tt[2]), Persia.DateType.Persian);
                }
                catch
                {
                    result.result.code = 3;
                    result.result.message = ClassCollection.Message.DATE_INCORRECT;
                    return new JavaScriptSerializer().Serialize(result);
                }
            }
            if (regDateTo != "")
            {
                try
                {
                    var tt = regDateTo.Split('/');
                    regDatetoD = Persia.Calendar.ConvertToGregorian(int.Parse(tt[0]), int.Parse(tt[1]), int.Parse(tt[2]), Persia.DateType.Persian);
                }
                catch
                {
                    result.result.code = 3;
                    result.result.message = ClassCollection.Message.DATE_INCORRECT;
                    return new JavaScriptSerializer().Serialize(result);
                }
            }
            if (regDateTo != "" && regDateFrom != "")
            {
                if (regDatefromD.CompareTo(regDatetoD) > 0)
                {
                    result.result.code = 3;
                    result.result.message = ClassCollection.Message.DATE_INCORRECT;
                    return new JavaScriptSerializer().Serialize(result);
                }
            }
            #endregion
            
            var us = db.answerTbls.Where(c => c.creativityCompetitionID == creativityCompetitionId && c.answerContentTbls.Any()).Take(1).AsQueryable();

            

            if (regDateTo != "")
            {
                us = us.Where(c => c.regDate <= regDatetoD);
            }
            if (regDateFrom != "")
            {
                us = us.Where(c => c.regDate >= regDatefromD);
            }


            if (answerstate == "block")
            {
                us = us.Where(c => c.isBlock == true);
            }
            else if (answerstate == "unblock")
            {
                us = us.Where(c => c.isBlock == false);
            }
            else if (answerstate == "correct")
            {
                us = us.Where(c => c.isCorrect == true);
            }
            else if (answerstate == "incorrect")
            {
                us = us.Where(c => c.isCorrect == false);
            }
            #region search

            if (fillterString != "")
            {
                var search = new Search(fillterString);
                var ors = search.getOR;

                var predicate = PredicateBuilder.False<answerTbl>();
                foreach (string keyword in ors)
                {
                    string temp = keyword;
                    predicate = predicate.Or(p => p.title.Contains(keyword) || p.answerContentTbls.Any(c => c.contentType == 0 && c.value.Contains(keyword)));
                }

                us = us.Where(predicate);
            }

            if (orderType == "a")
            {
                switch (sortby)
                {
                    case "reg":
                        us = us.OrderBy(c => c.regDate);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (sortby)
                {
                    case "reg":
                        us = us.OrderByDescending(c => c.regDate);
                        break;
                    default:
                        break;
                }
            }

            var acList = us.ToList();

            var count = acList.Count();
            acList = acList.Skip(skipCount).Take(pageSize).ToList();
           
            #endregion

            var answer = db.answerTbls.Where(c => c.creativityCompetitionID == creativityCompetitionId && c.isCorrect == true && c.isWinner == true && c.isBlock == false ).OrderBy(c => c.regDate).AsQueryable();
            if (answer.Count() != 0)
            {
                var winnerTitle = answer.Single(c => c.creativityCompetitionID == creativityCompetitionId && c.isCorrect == true && c.isWinner == true && c.isBlock == false).title;
                result.winnerTitle = winnerTitle;
                var winnerID = answer.Single(c => c.creativityCompetitionID == creativityCompetitionId && c.isCorrect == true && c.isWinner == true && c.isBlock == false).ID;
                result.winnerID = winnerID;
            }

            result.answer = new List<Model.AnswerAdmin>();
            foreach (var item in acList)
            {
                var tmp = new ClassCollection.Model.AnswerAdmin();
                tmp.ID = item.ID;
                tmp.title = item.title;
                tmp.isBlock = item.isBlock;
                tmp.mUserName = item.mUserTbl.name + " " + item.mUserTbl.family;
                tmp.regDate = Persia.Calendar.ConvertToPersian(item.regDate).ToString("m") + " - " + Persia.Calendar.ConvertToPersian(item.regDate).ToString("H");
                result.answer.Add(tmp);
            }

            if (count % pageSize == 0)
                count = count / pageSize;
            else
                count = (count / pageSize) + 1;

            result.pageTitle = db.creativityCompetitionTbls.Single(c => c.ID == creativityCompetitionId).title;
            result.pageCount = count;
            result.result.code = 0;
            result.result.message = ClassCollection.Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
        }

    }
}

