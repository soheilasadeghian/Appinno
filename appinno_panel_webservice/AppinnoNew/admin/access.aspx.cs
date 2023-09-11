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
    public partial class access : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var db = new DataAccessDataContext();
            if (ClassCollection.Methods.isLogin(Context))
            {
                var user = ClassCollection.Methods.getUserInSession(HttpContext.Current);
                var permission = ClassCollection.Accessing.Deserialize(db.accessTbls.Single(c => c.ID == user.accessID).per);

                if (!permission.access.access.showlist)
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
        public static string getAllAccess(string fillterString, string part, string action, int pageIndex)
        {
            const int pageSize = 30;

            ClassCollection.Model.AccessListResult result = new ClassCollection.Model.AccessListResult();
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

            if (!permission.access.access.showlist)
            {
                result.result.code = 2;
                result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }
            var ac = db.accessTbls.Where(c => c.accessID == user.accessID || user.accessID == 1).AsQueryable<accessTbl>();

            #region search

            if (fillterString != "")
            {
                var search = new Search(fillterString);

                var ors = search.getOR;

                var predicate = PredicateBuilder.False<accessTbl>();
                foreach (string keyword in ors)
                {
                    string temp = keyword;
                    predicate = predicate.Or(p => p.title.Contains(keyword));
                }

                ac = ac.Where(predicate);
            }
            var acList = ac.ToList();
            var acSecList = new List<accessTbl>();
            var thirdList = new List<accessTbl>();

            if (part != "")
            {
                foreach (var item in acList)
                {
                    var acc = ClassCollection.Accessing.Deserialize((item.per));
                    if (part == "news")
                    {
                        if (acc.news.access.delete == true || acc.news.access.edit == true || acc.news.access.insert == true || acc.news.access.showlist == true)
                        {
                            acSecList.Add(item);
                        }
                    }
                    else if (part == "events")
                    {
                        if (acc.events.access.delete == true || acc.events.access.edit == true || acc.events.access.insert == true || acc.events.access.showlist == true)
                        {
                            acSecList.Add(item);
                        }
                    }
                    else if (part == "pub")
                    {
                        if (acc.publication.access.delete == true || acc.publication.access.edit == true || acc.publication.access.insert == true || acc.publication.access.showlist == true)
                        {
                            acSecList.Add(item);
                        }
                    }
                    else if (part == "io")
                    {
                        if (acc.io.access.delete == true || acc.io.access.edit == true || acc.io.access.insert == true || acc.io.access.showlist == true)
                        {
                            acSecList.Add(item);
                        }
                    }
                    else if (part == "chart")
                    {
                        if (acc.chart.access.delete == true || acc.chart.access.edit == true || acc.chart.access.insert == true || acc.chart.access.showlist == true)
                        {
                            acSecList.Add(item);
                        }
                    }
                    else if (part == "download")
                    {
                        if (acc.download.access.delete == true || acc.download.access.edit == true || acc.download.access.insert == true || acc.download.access.showlist == true)
                        {
                            acSecList.Add(item);
                        }
                    }
                    else if (part == "grouping")
                    {
                        if (acc.grouping.access.delete == true || acc.grouping.access.edit == true || acc.grouping.access.insert == true || acc.grouping.access.showlist == true)
                        {
                            acSecList.Add(item);
                        }
                    }
                    else if (part == "user")
                    {
                        if (acc.user.access.delete == true || acc.user.access.edit == true || acc.user.access.insert == true || acc.user.access.showlist == true)
                        {
                            acSecList.Add(item);
                        }
                    }
                    else if (part == "operators")
                    {
                        if (acc.operators.access.delete == true || acc.operators.access.edit == true || acc.operators.access.insert == true || acc.operators.access.showlist == true)
                        {
                            acSecList.Add(item);
                        }
                    }
                    else if (part == "access")
                    {
                        if (acc.access.access.delete == true || acc.access.access.edit == true || acc.access.access.insert == true || acc.access.access.showlist == true)
                        {
                            acSecList.Add(item);
                        }
                    }
                    else if (part == "ican")
                    {
                        if (acc.ican.access.delete == true || acc.ican.access.edit == true || acc.ican.access.insert == true || acc.ican.access.showlist == true)
                        {
                            acSecList.Add(item);
                        }
                    }
                    else if (part == "poll")
                    {
                        if (acc.poll.access.delete == true || acc.poll.access.edit == true || acc.poll.access.insert == true || acc.poll.access.showlist == true)
                        {
                            acSecList.Add(item);
                        }
                    }
                    else if (part == "bestIdeasCompetition")
                    {
                        if (acc.bestIdeasCompetition.access.delete == true || acc.bestIdeasCompetition.access.edit == true || acc.bestIdeasCompetition.access.insert == true || acc.bestIdeasCompetition.access.showlist == true)
                        {
                            acSecList.Add(item);
                        }
                    }
                    else if (part == "creativityCompetition")
                    {
                        if (acc.creativityCompetition.access.delete == true || acc.creativityCompetition.access.edit == true || acc.creativityCompetition.access.insert == true || acc.creativityCompetition.access.showlist == true)
                        {
                            acSecList.Add(item);
                        }
                    }
                    else if (part == "myIranCompetition")
                    {
                        if (acc.myIranCompetition.access.delete == true || acc.myIranCompetition.access.edit == true || acc.myIranCompetition.access.insert == true || acc.myIranCompetition.access.showlist == true)
                        {
                            acSecList.Add(item);
                        }
                    }
                }
            }
            if (action != "" && part != "")
            {

                foreach (var item in acSecList)
                {
                    var acc = ClassCollection.Accessing.Deserialize(ClassCollection.Methods.Decrypt(item.per));
                    if (action == "insert")
                    {
                        if (part == "news")
                        {
                            if (acc.news.access.insert == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "events")
                        {
                            if (acc.events.access.insert == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "pub")
                        {
                            if (acc.publication.access.insert == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "io")
                        {
                            if (acc.io.access.insert == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "chart")
                        {
                            if (acc.chart.access.insert == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "download")
                        {
                            if (acc.download.access.insert == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "grouping")
                        {
                            if (acc.grouping.access.insert == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "user")
                        {
                            if (acc.user.access.insert == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "operators")
                        {
                            if (acc.operators.access.insert == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "access")
                        {
                            if (acc.access.access.insert == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "ican")
                        {
                            if (acc.ican.access.insert == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "poll")
                        {
                            if (acc.poll.access.insert == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "bestIdeasCompetition")
                        {
                            if (acc.bestIdeasCompetition.access.insert == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "creativityCompetition")
                        {
                            if (acc.creativityCompetition.access.insert == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "myIranCompetition")
                        {
                            if (acc.myIranCompetition.access.insert == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                    }
                    else if (action == "edit")
                    {
                        if (part == "news")
                        {
                            if (acc.news.access.edit == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "events")
                        {
                            if (acc.events.access.edit == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "pub")
                        {
                            if (acc.publication.access.edit == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "io")
                        {
                            if (acc.io.access.edit == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "chart")
                        {
                            if (acc.chart.access.edit == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "download")
                        {
                            if (acc.download.access.edit == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "grouping")
                        {
                            if (acc.grouping.access.edit == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "user")
                        {
                            if (acc.user.access.edit == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "operators")
                        {
                            if (acc.operators.access.edit == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "access")
                        {
                            if (acc.access.access.edit == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "ican")
                        {
                            if (acc.ican.access.edit == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "poll")
                        {
                            if (acc.poll.access.edit == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "bestIdeasCompetition")
                        {
                            if (acc.bestIdeasCompetition.access.edit == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "creativityCompetition")
                        {
                            if (acc.creativityCompetition.access.edit == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "myIranCompetition")
                        {
                            if (acc.myIranCompetition.access.edit == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                    }
                    else if (action == "delete")
                    {
                        if (part == "news")
                        {
                            if (acc.news.access.delete == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "events")
                        {
                            if (acc.events.access.delete == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "pub")
                        {
                            if (acc.publication.access.delete == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "io")
                        {
                            if (acc.io.access.delete == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "chart")
                        {
                            if (acc.chart.access.delete == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "download")
                        {
                            if (acc.download.access.delete == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "grouping")
                        {
                            if (acc.grouping.access.delete == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "user")
                        {
                            if (acc.user.access.delete == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "operators")
                        {
                            if (acc.operators.access.delete == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "access")
                        {
                            if (acc.access.access.delete == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "ican")
                        {
                            if (acc.ican.access.delete == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "poll")
                        {
                            if (acc.poll.access.delete == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "bestIdeasCompetition")
                        {
                            if (acc.bestIdeasCompetition.access.delete == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "creativityCompetition")
                        {
                            if (acc.creativityCompetition.access.delete == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "myIranCompetition")
                        {
                            if (acc.myIranCompetition.access.delete == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                    }
                    else if (action == "showlist")
                    {
                        if (part == "news")
                        {
                            if (acc.news.access.showlist == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "events")
                        {
                            if (acc.events.access.showlist == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "pub")
                        {
                            if (acc.publication.access.showlist == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "io")
                        {
                            if (acc.io.access.showlist == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "chart")
                        {
                            if (acc.chart.access.showlist == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "download")
                        {
                            if (acc.download.access.showlist == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "grouping")
                        {
                            if (acc.grouping.access.showlist == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "user")
                        {
                            if (acc.user.access.showlist == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "operators")
                        {
                            if (acc.operators.access.showlist == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "access")
                        {
                            if (acc.access.access.showlist == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "ican")
                        {
                            if (acc.ican.access.showlist == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "poll")
                        {
                            if (acc.poll.access.showlist == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "bestIdeasCompetition")
                        {
                            if (acc.bestIdeasCompetition.access.showlist == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "creativityCompetition")
                        {
                            if (acc.creativityCompetition.access.showlist == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "myIranCompetition")
                        {
                            if (acc.myIranCompetition.access.showlist == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                    }
                    else if (action == "full")
                    {
                        if (part == "news")
                        {
                            if (acc.news.access.delete == true && acc.user.access.edit == true && acc.user.access.insert == true && acc.user.access.showlist == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "events")
                        {
                            if (acc.events.access.delete == true && acc.events.access.edit == true && acc.events.access.insert == true && acc.events.access.showlist == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "pub")
                        {
                            if (acc.publication.access.delete == true && acc.publication.access.edit == true && acc.publication.access.insert == true && acc.publication.access.showlist == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "io")
                        {
                            if (acc.io.access.delete == true && acc.io.access.edit == true && acc.io.access.insert == true && acc.io.access.showlist == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "chart")
                        {
                            if (acc.chart.access.delete == true && acc.chart.access.edit == true && acc.chart.access.insert == true && acc.chart.access.showlist == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "download")
                        {
                            if (acc.download.access.delete == true && acc.download.access.edit == true && acc.download.access.insert == true && acc.download.access.showlist == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "grouping")
                        {
                            if (acc.grouping.access.delete == true && acc.grouping.access.edit == true && acc.grouping.access.insert == true && acc.grouping.access.showlist == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "user")
                        {
                            if (acc.user.access.delete == true && acc.user.access.edit == true && acc.user.access.insert == true && acc.user.access.showlist == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "operators")
                        {
                            if (acc.operators.access.delete == true && acc.operators.access.edit == true && acc.operators.access.insert == true && acc.operators.access.showlist == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "access")
                        {
                            if (acc.access.access.delete == true && acc.access.access.edit == true && acc.access.access.insert == true && acc.access.access.showlist == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "ican")
                        {
                            if (acc.ican.access.delete == true && acc.ican.access.edit == true && acc.ican.access.insert == true && acc.ican.access.showlist == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "poll")
                        {
                            if (acc.poll.access.delete == true && acc.poll.access.edit == true && acc.poll.access.insert == true && acc.poll.access.showlist == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "bestIdeasCompetition")
                        {
                            if (acc.bestIdeasCompetition.access.delete == true && acc.bestIdeasCompetition.access.edit == true && acc.bestIdeasCompetition.access.insert == true && acc.bestIdeasCompetition.access.showlist == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "creativityCompetition")
                        {
                            if (acc.creativityCompetition.access.delete == true && acc.creativityCompetition.access.edit == true && acc.creativityCompetition.access.insert == true && acc.creativityCompetition.access.showlist == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                        else if (part == "myIranCompetition")
                        {
                            if (acc.myIranCompetition.access.delete == true && acc.myIranCompetition.access.edit == true && acc.myIranCompetition.access.insert == true && acc.myIranCompetition.access.showlist == true)
                            {
                                thirdList.Add(item);
                            }
                        }
                    }

                }
            }
            if (part != "" && action != "")
                acList = thirdList;
            else if (part != "")
            {
                acList = acSecList;
            }
            var count = acList.Count();
            result.totalCount = count;
            acList = acList.Skip(skipCount).Take(pageSize).ToList();

            #endregion

            result.access = new List<ClassCollection.Model.Access>();
            foreach (var item in acList)
            {
                var tmp = new ClassCollection.Model.Access();
                tmp.ID = item.ID;

                tmp.per = ClassCollection.Accessing.Deserialize(item.per);
                tmp.title = item.title;
                result.access.Add(tmp);
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
        public static string insertAccess(
            string title,
            Accessing.AccessDetail news,
            Accessing.AccessDetail events,
            Accessing.AccessDetail io,
            Accessing.AccessDetail pub,
            Accessing.AccessDetail chart,
            Accessing.AccessDetail download,
            Accessing.AccessDetail grouping,
            Accessing.AccessDetail users,
            Accessing.AccessDetail access,
            Accessing.AccessDetail operators,
            Accessing.AccessDetail ican,
            Accessing.AccessDetail poll,
            Accessing.AccessDetail bestIdeasCompetition,
            Accessing.AccessDetail creativityCompetition,
            Accessing.AccessDetail myIranCompetition
            )
        {
            ClassCollection.Model.Result result = new Model.Result();

            if (!ClassCollection.Methods.isLogin(HttpContext.Current))
            {
                result.code = 1;
                result.message = ClassCollection.Message.LOGIN_FIRST;
                return new JavaScriptSerializer().Serialize(result);
            }

            var db = new DataAccessDataContext();
            var user = ClassCollection.Methods.getUserInSession(HttpContext.Current);
            var permission = ClassCollection.Accessing.Deserialize(db.accessTbls.Single(c => c.ID == user.accessID).per);

            if (!permission.access.access.insert)
            {
                result.code = 2;
                result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }

            #region chack new access vs user permission

            if ((!permission.access.access.delete && access.delete) || (!permission.access.access.edit && access.edit) || (!permission.access.access.insert && access.insert) || (!permission.access.access.showlist && access.showlist))
            {
                result.code = 2;
                result.message = "موارد انتخاب شده برای نقش مورد نظر صحیح نمی باشد";
                return new JavaScriptSerializer().Serialize(result);
            }
            if ((!permission.bestIdeasCompetition.access.delete && bestIdeasCompetition.delete) ||
                (!permission.bestIdeasCompetition.access.edit && bestIdeasCompetition.edit) ||
                (!permission.bestIdeasCompetition.access.insert && bestIdeasCompetition.insert) ||
                (!permission.bestIdeasCompetition.access.showlist && bestIdeasCompetition.showlist))
            {
                result.code = 2;
                result.message = "موارد انتخاب شده برای نقش مورد نظر صحیح نمی باشد";
                return new JavaScriptSerializer().Serialize(result);
            }
            if ((!permission.chart.access.delete && chart.delete) ||
                (!permission.chart.access.edit && chart.edit) ||
                (!permission.chart.access.insert && chart.insert) ||
                (!permission.chart.access.showlist && chart.showlist))
            {
                result.code = 2;
                result.message = "موارد انتخاب شده برای نقش مورد نظر صحیح نمی باشد";
                return new JavaScriptSerializer().Serialize(result);
            }
            if ((!permission.creativityCompetition.access.delete && creativityCompetition.delete) ||
                (!permission.creativityCompetition.access.edit && creativityCompetition.edit) ||
                (!permission.creativityCompetition.access.insert && creativityCompetition.insert) ||
                (!permission.creativityCompetition.access.showlist && creativityCompetition.showlist))
            {
                result.code = 2;
                result.message = "موارد انتخاب شده برای نقش مورد نظر صحیح نمی باشد";
                return new JavaScriptSerializer().Serialize(result);
            }
            if ((!permission.download.access.delete && download.delete) ||
                (!permission.download.access.edit && download.edit) ||
                (!permission.download.access.insert && download.insert) ||
                (!permission.download.access.showlist && download.showlist))
            {
                result.code = 2;
                result.message = "موارد انتخاب شده برای نقش مورد نظر صحیح نمی باشد";
                return new JavaScriptSerializer().Serialize(result);
            }
            if ((!permission.events.access.delete && events.delete) ||
                (!permission.events.access.edit && events.edit) ||
                (!permission.events.access.insert && events.insert) ||
                (!permission.events.access.showlist && events.showlist))
            {
                result.code = 2;
                result.message = "موارد انتخاب شده برای نقش مورد نظر صحیح نمی باشد";
                return new JavaScriptSerializer().Serialize(result);
            }
            if ((!permission.grouping.access.delete && grouping.delete) ||
                (!permission.grouping.access.edit && grouping.edit) ||
                (!permission.grouping.access.insert && grouping.insert) ||
                (!permission.grouping.access.showlist && grouping.showlist))
            {
                result.code = 2;
                result.message = "موارد انتخاب شده برای نقش مورد نظر صحیح نمی باشد";
                return new JavaScriptSerializer().Serialize(result);
            }
            if ((!permission.ican.access.delete && ican.delete) ||
                (!permission.ican.access.edit && ican.edit) ||
                (!permission.ican.access.insert && ican.insert) ||
                (!permission.ican.access.showlist && ican.showlist))
            {
                result.code = 2;
                result.message = "موارد انتخاب شده برای نقش مورد نظر صحیح نمی باشد";
                return new JavaScriptSerializer().Serialize(result);
            }
            if ((!permission.io.access.delete && io.delete) ||
                (!permission.io.access.edit && io.edit) ||
                (!permission.io.access.insert && io.insert) ||
                (!permission.io.access.showlist && io.showlist))
            {
                result.code = 2;
                result.message = "موارد انتخاب شده برای نقش مورد نظر صحیح نمی باشد";
                return new JavaScriptSerializer().Serialize(result);
            }
            if ((!permission.myIranCompetition.access.delete && myIranCompetition.delete) ||
                (!permission.myIranCompetition.access.edit && myIranCompetition.edit) ||
                (!permission.myIranCompetition.access.insert && myIranCompetition.insert) ||
                (!permission.myIranCompetition.access.showlist && myIranCompetition.showlist))
            {
                result.code = 2;
                result.message = "موارد انتخاب شده برای نقش مورد نظر صحیح نمی باشد";
                return new JavaScriptSerializer().Serialize(result);
            }
            if ((!permission.news.access.delete && news.delete) ||
                (!permission.news.access.edit && news.edit) ||
                (!permission.news.access.insert && news.insert) ||
                (!permission.news.access.showlist && news.showlist))
            {
                result.code = 2;
                result.message = "موارد انتخاب شده برای نقش مورد نظر صحیح نمی باشد";
                return new JavaScriptSerializer().Serialize(result);
            }
            if ((!permission.operators.access.delete && operators.delete) ||
                (!permission.operators.access.edit && operators.edit) ||
                (!permission.operators.access.insert && operators.insert) ||
                (!permission.operators.access.showlist && operators.showlist))
            {
                result.code = 2;
                result.message = "موارد انتخاب شده برای نقش مورد نظر صحیح نمی باشد";
                return new JavaScriptSerializer().Serialize(result);
            }
            if ((!permission.poll.access.delete && poll.delete) ||
                (!permission.poll.access.edit && poll.edit) ||
                (!permission.poll.access.insert && poll.insert) ||
                (!permission.poll.access.showlist && poll.showlist))
            {
                result.code = 2;
                result.message = "موارد انتخاب شده برای نقش مورد نظر صحیح نمی باشد";
                return new JavaScriptSerializer().Serialize(result);
            }
            if ((!permission.publication.access.delete && pub.delete) ||
                (!permission.publication.access.edit && pub.edit) ||
                (!permission.publication.access.insert && pub.insert) ||
                (!permission.publication.access.showlist && pub.showlist))
            {
                result.code = 2;
                result.message = "موارد انتخاب شده برای نقش مورد نظر صحیح نمی باشد";
                return new JavaScriptSerializer().Serialize(result);
            }
            if ((!permission.user.access.delete && users.delete) ||
                (!permission.user.access.edit && users.edit) ||
                (!permission.user.access.insert && users.insert) ||
                (!permission.user.access.showlist && users.showlist))
            {
                result.code = 2;
                result.message = "موارد انتخاب شده برای نقش مورد نظر صحیح نمی باشد";
                return new JavaScriptSerializer().Serialize(result);
            }
            #endregion

            var accessing = new ClassCollection.Accessing();

            accessing.access = new Accessing.Access();
            accessing.access.access = access;

            accessing.chart = new Accessing.Chart();
            accessing.chart.access = chart;

            accessing.download = new Accessing.Download();
            accessing.download.access = download;

            accessing.events = new Accessing.Event();
            accessing.events.access = events;

            accessing.grouping = new Accessing.Grouping();
            accessing.grouping.access = grouping;

            accessing.io = new Accessing.IO();
            accessing.io.access = io;

            accessing.news = new Accessing.News();
            accessing.news.access = news;

            accessing.operators = new Accessing.Operator();
            accessing.operators.access = operators;

            accessing.publication = new Accessing.Publication();
            accessing.publication.access = pub;

            accessing.user = new Accessing.User();
            accessing.user.access = users;

            accessing.ican = new Accessing.Ican();
            accessing.ican.access = ican;

            accessing.poll = new Accessing.Poll();
            accessing.poll.access = poll;

            accessing.bestIdeasCompetition = new Accessing.BestIdeasCompetition();
            accessing.bestIdeasCompetition.access = bestIdeasCompetition;

            accessing.creativityCompetition = new Accessing.CreativityCompetition();
            accessing.creativityCompetition.access = creativityCompetition;

            accessing.myIranCompetition = new Accessing.MyIranCompetition();
            accessing.myIranCompetition.access = myIranCompetition;


            var acce = new accessTbl();
            acce.title = title;
            acce.per = accessing.ToString(true);
            acce.accessID = user.accessID;
            db.accessTbls.InsertOnSubmit(acce);
            db.SubmitChanges();

            result.code = 0;
            result.message = Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
        }
        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string editAccess(
            int accessID,
            string title,
            Accessing.AccessDetail news,
            Accessing.AccessDetail events,
            Accessing.AccessDetail io,
            Accessing.AccessDetail pub,
            Accessing.AccessDetail chart,
            Accessing.AccessDetail download,
            Accessing.AccessDetail grouping,
            Accessing.AccessDetail users,
            Accessing.AccessDetail access,
            Accessing.AccessDetail operators,
            Accessing.AccessDetail ican,
            Accessing.AccessDetail poll,
            Accessing.AccessDetail bestIdeasCompetition,
            Accessing.AccessDetail creativityCompetition,
            Accessing.AccessDetail myIranCompetition
            )
        {
            ClassCollection.Model.Result result = new Model.Result();

            if (!ClassCollection.Methods.isLogin(HttpContext.Current))
            {
                result.code = 1;
                result.message = ClassCollection.Message.LOGIN_FIRST;
                return new JavaScriptSerializer().Serialize(result);
            }

            var db = new DataAccessDataContext();
            var user = Methods.getUserInSession(HttpContext.Current);
            var userAccess = db.accessTbls.Single(c => c.ID == user.accessID);

            var permission = Accessing.Deserialize(userAccess.per);

            if (!permission.access.access.edit)
            {
                result.code = 2;
                result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }

            if (db.accessTbls.Any(c => c.ID == accessID) == false)
            {
                result.code = 3;
                result.message = ClassCollection.Message.ACCESS_NOT_EXIST;
                return new JavaScriptSerializer().Serialize(result);
            }

            if(db.accessTbls.Where(c=>c.accessID==userAccess.ID).Any(c=>c.ID==accessID)==false)
            {
                result.code = 3;
                result.message = "امکان ویرایش این نقش برای حساب کاربری شما وجود ندارد";
                return new JavaScriptSerializer().Serialize(result);
            }

            var obj = db.accessTbls.Single(c => c.ID == accessID);


            #region chack new access vs user permission

            if ((!permission.access.access.delete && access.delete) || (!permission.access.access.edit && access.edit) || (!permission.access.access.insert && access.insert) || (!permission.access.access.showlist && access.showlist))
            {
                result.code = 2;
                result.message = "موارد انتخاب شده برای نقش مورد نظر صحیح نمی باشد";
                return new JavaScriptSerializer().Serialize(result);
            }
            if ((!permission.bestIdeasCompetition.access.delete && bestIdeasCompetition.delete) ||
                (!permission.bestIdeasCompetition.access.edit && bestIdeasCompetition.edit) ||
                (!permission.bestIdeasCompetition.access.insert && bestIdeasCompetition.insert) ||
                (!permission.bestIdeasCompetition.access.showlist && bestIdeasCompetition.showlist))
            {
                result.code = 2;
                result.message = "موارد انتخاب شده برای نقش مورد نظر صحیح نمی باشد";
                return new JavaScriptSerializer().Serialize(result);
            }
            if ((!permission.chart.access.delete && chart.delete) ||
                (!permission.chart.access.edit && chart.edit) ||
                (!permission.chart.access.insert && chart.insert) ||
                (!permission.chart.access.showlist && chart.showlist))
            {
                result.code = 2;
                result.message = "موارد انتخاب شده برای نقش مورد نظر صحیح نمی باشد";
                return new JavaScriptSerializer().Serialize(result);
            }
            if ((!permission.creativityCompetition.access.delete && creativityCompetition.delete) ||
                (!permission.creativityCompetition.access.edit && creativityCompetition.edit) ||
                (!permission.creativityCompetition.access.insert && creativityCompetition.insert) ||
                (!permission.creativityCompetition.access.showlist && creativityCompetition.showlist))
            {
                result.code = 2;
                result.message = "موارد انتخاب شده برای نقش مورد نظر صحیح نمی باشد";
                return new JavaScriptSerializer().Serialize(result);
            }
            if ((!permission.download.access.delete && download.delete) ||
                (!permission.download.access.edit && download.edit) ||
                (!permission.download.access.insert && download.insert) ||
                (!permission.download.access.showlist && download.showlist))
            {
                result.code = 2;
                result.message = "موارد انتخاب شده برای نقش مورد نظر صحیح نمی باشد";
                return new JavaScriptSerializer().Serialize(result);
            }
            if ((!permission.events.access.delete && events.delete) ||
                (!permission.events.access.edit && events.edit) ||
                (!permission.events.access.insert && events.insert) ||
                (!permission.events.access.showlist && events.showlist))
            {
                result.code = 2;
                result.message = "موارد انتخاب شده برای نقش مورد نظر صحیح نمی باشد";
                return new JavaScriptSerializer().Serialize(result);
            }
            if ((!permission.grouping.access.delete && grouping.delete) ||
                (!permission.grouping.access.edit && grouping.edit) ||
                (!permission.grouping.access.insert && grouping.insert) ||
                (!permission.grouping.access.showlist && grouping.showlist))
            {
                result.code = 2;
                result.message = "موارد انتخاب شده برای نقش مورد نظر صحیح نمی باشد";
                return new JavaScriptSerializer().Serialize(result);
            }
            if ((!permission.ican.access.delete && ican.delete) ||
                (!permission.ican.access.edit && ican.edit) ||
                (!permission.ican.access.insert && ican.insert) ||
                (!permission.ican.access.showlist && ican.showlist))
            {
                result.code = 2;
                result.message = "موارد انتخاب شده برای نقش مورد نظر صحیح نمی باشد";
                return new JavaScriptSerializer().Serialize(result);
            }
            if ((!permission.io.access.delete && io.delete) ||
                (!permission.io.access.edit && io.edit) ||
                (!permission.io.access.insert && io.insert) ||
                (!permission.io.access.showlist && io.showlist))
            {
                result.code = 2;
                result.message = "موارد انتخاب شده برای نقش مورد نظر صحیح نمی باشد";
                return new JavaScriptSerializer().Serialize(result);
            }
            if ((!permission.myIranCompetition.access.delete && myIranCompetition.delete) ||
                (!permission.myIranCompetition.access.edit && myIranCompetition.edit) ||
                (!permission.myIranCompetition.access.insert && myIranCompetition.insert) ||
                (!permission.myIranCompetition.access.showlist && myIranCompetition.showlist))
            {
                result.code = 2;
                result.message = "موارد انتخاب شده برای نقش مورد نظر صحیح نمی باشد";
                return new JavaScriptSerializer().Serialize(result);
            }
            if ((!permission.news.access.delete && news.delete) ||
                (!permission.news.access.edit && news.edit) ||
                (!permission.news.access.insert && news.insert) ||
                (!permission.news.access.showlist && news.showlist))
            {
                result.code = 2;
                result.message = "موارد انتخاب شده برای نقش مورد نظر صحیح نمی باشد";
                return new JavaScriptSerializer().Serialize(result);
            }
            if ((!permission.operators.access.delete && operators.delete) ||
                (!permission.operators.access.edit && operators.edit) ||
                (!permission.operators.access.insert && operators.insert) ||
                (!permission.operators.access.showlist && operators.showlist))
            {
                result.code = 2;
                result.message = "موارد انتخاب شده برای نقش مورد نظر صحیح نمی باشد";
                return new JavaScriptSerializer().Serialize(result);
            }
            if ((!permission.poll.access.delete && poll.delete) ||
                (!permission.poll.access.edit && poll.edit) ||
                (!permission.poll.access.insert && poll.insert) ||
                (!permission.poll.access.showlist && poll.showlist))
            {
                result.code = 2;
                result.message = "موارد انتخاب شده برای نقش مورد نظر صحیح نمی باشد";
                return new JavaScriptSerializer().Serialize(result);
            }
            if ((!permission.publication.access.delete && pub.delete) ||
                (!permission.publication.access.edit && pub.edit) ||
                (!permission.publication.access.insert && pub.insert) ||
                (!permission.publication.access.showlist && pub.showlist))
            {
                result.code = 2;
                result.message = "موارد انتخاب شده برای نقش مورد نظر صحیح نمی باشد";
                return new JavaScriptSerializer().Serialize(result);
            }
            if ((!permission.user.access.delete && users.delete) ||
                (!permission.user.access.edit && users.edit) ||
                (!permission.user.access.insert && users.insert) ||
                (!permission.user.access.showlist && users.showlist))
            {
                result.code = 2;
                result.message = "موارد انتخاب شده برای نقش مورد نظر صحیح نمی باشد";
                return new JavaScriptSerializer().Serialize(result);
            }
            #endregion

            var allAccessChild = new List<int>();
            getAllChildAccess(ref allAccessChild, accessID);

            var accessing = Accessing.Deserialize(obj.per);


            #region chack new access vs old access

            if ((accessing.access.access.delete && !access.delete) ||
                (accessing.access.access.edit && !access.edit) ||
                (accessing.access.access.insert && !access.insert) ||
                (accessing.access.access.showlist && !access.showlist))
            {
                foreach (var ID in allAccessChild)
                {
                    var TAccess = db.accessTbls.Single(c => c.ID == ID);
                    var ac = Accessing.Deserialize(TAccess.per);

                    ac.access.access = access;
                    TAccess.per = ac.ToString(true);
                    db.SubmitChanges();
                }
            }
            if ((accessing.bestIdeasCompetition.access.delete && !bestIdeasCompetition.delete) ||
                (accessing.bestIdeasCompetition.access.edit && !bestIdeasCompetition.edit) ||
                (accessing.bestIdeasCompetition.access.insert && !bestIdeasCompetition.insert) ||
                (accessing.bestIdeasCompetition.access.showlist && !bestIdeasCompetition.showlist))
            {
                foreach (var ID in allAccessChild)
                {
                    var TAccess = db.accessTbls.Single(c => c.ID == ID);
                    var ac = Accessing.Deserialize(TAccess.per);

                    ac.bestIdeasCompetition.access = bestIdeasCompetition;
                    TAccess.per = ac.ToString(true);
                    db.SubmitChanges();
                }
            }
            if ((accessing.chart.access.delete && !chart.delete) ||
                (accessing.chart.access.edit && !chart.edit) ||
                (accessing.chart.access.insert && !chart.insert) ||
                (accessing.chart.access.showlist && !chart.showlist))
            {
                foreach (var ID in allAccessChild)
                {
                    var TAccess = db.accessTbls.Single(c => c.ID == ID);
                    var ac = Accessing.Deserialize(TAccess.per);

                    ac.chart.access = chart;
                    TAccess.per = ac.ToString(true);
                    db.SubmitChanges();
                }
            }
            if ((accessing.creativityCompetition.access.delete && !creativityCompetition.delete) ||
                (accessing.creativityCompetition.access.edit && !creativityCompetition.edit) ||
                (accessing.creativityCompetition.access.insert && !creativityCompetition.insert) ||
                (accessing.creativityCompetition.access.showlist && !creativityCompetition.showlist))
            {
                foreach (var ID in allAccessChild)
                {
                    var TAccess = db.accessTbls.Single(c => c.ID == ID);
                    var ac = Accessing.Deserialize(TAccess.per);

                    ac.creativityCompetition.access = creativityCompetition;
                    TAccess.per = ac.ToString(true);
                    db.SubmitChanges();
                }
            }
            if ((accessing.download.access.delete && !download.delete) ||
                (accessing.download.access.edit && !download.edit) ||
                (accessing.download.access.insert && !download.insert) ||
                (accessing.download.access.showlist && !download.showlist))
            {
                foreach (var ID in allAccessChild)
                {
                    var TAccess = db.accessTbls.Single(c => c.ID == ID);
                    var ac = Accessing.Deserialize(TAccess.per);

                    ac.download.access = download;
                    TAccess.per = ac.ToString(true);
                    db.SubmitChanges();
                }
            }
            if ((accessing.events.access.delete && !events.delete) ||
                (accessing.events.access.edit && !events.edit) ||
                (accessing.events.access.insert && !events.insert) ||
                (accessing.events.access.showlist && !events.showlist))
            {
                foreach (var ID in allAccessChild)
                {
                    var TAccess = db.accessTbls.Single(c => c.ID == ID);
                    var ac = Accessing.Deserialize(TAccess.per);

                    ac.events.access = events;
                    TAccess.per = ac.ToString(true);
                    db.SubmitChanges();
                }
            }
            if ((accessing.grouping.access.delete && !grouping.delete) ||
                (accessing.grouping.access.edit && !grouping.edit) ||
                (accessing.grouping.access.insert && !grouping.insert) ||
                (accessing.grouping.access.showlist && !grouping.showlist))
            {
                foreach (var ID in allAccessChild)
                {
                    var TAccess = db.accessTbls.Single(c => c.ID == ID);
                    var ac = Accessing.Deserialize(TAccess.per);

                    ac.grouping.access = grouping;
                    TAccess.per = ac.ToString(true);
                    db.SubmitChanges();
                }
            }
            if ((accessing.ican.access.delete && !ican.delete) ||
                (accessing.ican.access.edit && !ican.edit) ||
                (accessing.ican.access.insert && !ican.insert) ||
                (accessing.ican.access.showlist && !ican.showlist))
            {
                foreach (var ID in allAccessChild)
                {
                    var TAccess = db.accessTbls.Single(c => c.ID == ID);
                    var ac = Accessing.Deserialize(TAccess.per);

                    ac.ican.access = ican;
                    TAccess.per = ac.ToString(true);
                    db.SubmitChanges();
                }
            }
            if ((accessing.io.access.delete && !io.delete) ||
                (accessing.io.access.edit && !io.edit) ||
                (accessing.io.access.insert && !io.insert) ||
                (accessing.io.access.showlist && !io.showlist))
            {
                foreach (var ID in allAccessChild)
                {
                    var TAccess = db.accessTbls.Single(c => c.ID == ID);
                    var ac = Accessing.Deserialize(TAccess.per);

                    ac.io.access = io;
                    TAccess.per = ac.ToString(true);
                    db.SubmitChanges();
                }
            }
            if ((accessing.myIranCompetition.access.delete && !myIranCompetition.delete) ||
                (accessing.myIranCompetition.access.edit && !myIranCompetition.edit) ||
                (accessing.myIranCompetition.access.insert && !myIranCompetition.insert) ||
                (accessing.myIranCompetition.access.showlist && !myIranCompetition.showlist))
            {
                foreach (var ID in allAccessChild)
                {
                    var TAccess = db.accessTbls.Single(c => c.ID == ID);
                    var ac = Accessing.Deserialize(TAccess.per);

                    ac.myIranCompetition.access = myIranCompetition;
                    TAccess.per = ac.ToString(true);
                    db.SubmitChanges();
                }
            }
            if ((accessing.news.access.delete && !news.delete) ||
                (accessing.news.access.edit && !news.edit) ||
                (accessing.news.access.insert && !news.insert) ||
                (accessing.news.access.showlist && !news.showlist))
            {
                foreach (var ID in allAccessChild)
                {
                    var TAccess = db.accessTbls.Single(c => c.ID == ID);
                    var ac = Accessing.Deserialize(TAccess.per);

                    ac.news.access = news;
                    TAccess.per = ac.ToString(true);
                    db.SubmitChanges();
                }
            }
            if ((accessing.operators.access.delete && !operators.delete) ||
                (accessing.operators.access.edit && !operators.edit) ||
                (accessing.operators.access.insert && !operators.insert) ||
                (accessing.operators.access.showlist && !operators.showlist))
            {
                foreach (var ID in allAccessChild)
                {
                    var TAccess = db.accessTbls.Single(c => c.ID == ID);
                    var ac = Accessing.Deserialize(TAccess.per);

                    ac.operators.access = operators;
                    TAccess.per = ac.ToString(true);
                    db.SubmitChanges();
                }
            }
            if ((accessing.poll.access.delete && !poll.delete) ||
                (accessing.poll.access.edit && !poll.edit) ||
                (accessing.poll.access.insert && !poll.insert) ||
                (accessing.poll.access.showlist && !poll.showlist))
            {
                foreach (var ID in allAccessChild)
                {
                    var TAccess = db.accessTbls.Single(c => c.ID == ID);
                    var ac = Accessing.Deserialize(TAccess.per);

                    ac.poll.access = poll;
                    TAccess.per = ac.ToString(true);
                    db.SubmitChanges();
                }
            }
            if ((accessing.publication.access.delete && !pub.delete) ||
                (accessing.publication.access.edit && !pub.edit) ||
                (accessing.publication.access.insert && !pub.insert) ||
                (accessing.publication.access.showlist && !pub.showlist))
            {
                foreach (var ID in allAccessChild)
                {
                    var TAccess = db.accessTbls.Single(c => c.ID == ID);
                    var ac = Accessing.Deserialize(TAccess.per);

                    ac.publication.access = pub;
                    TAccess.per = ac.ToString(true);
                    db.SubmitChanges();
                }
            }
            if ((accessing.user.access.delete && !users.delete) ||
                (accessing.user.access.edit && !users.edit) ||
                (accessing.user.access.insert && !users.insert) ||
                (accessing.user.access.showlist && !users.showlist))
            {
                foreach (var ID in allAccessChild)
                {
                    var TAccess = db.accessTbls.Single(c => c.ID == ID);
                    var ac = Accessing.Deserialize(TAccess.per);

                    ac.user.access = users;
                    TAccess.per = ac.ToString(true);
                    db.SubmitChanges();
                }
            }
            #endregion



            accessing.access.access = access;
            accessing.chart.access = chart;
            accessing.download.access = download;
            accessing.events.access = events;
            accessing.grouping.access = grouping;
            accessing.io.access = io;
            accessing.news.access = news;
            accessing.operators.access = operators;
            accessing.publication.access = pub;
            accessing.user.access = users;
            accessing.ican.access = ican;
            accessing.poll.access = poll;
            accessing.bestIdeasCompetition.access = bestIdeasCompetition;
            accessing.creativityCompetition.access = creativityCompetition;
            accessing.myIranCompetition.access = myIranCompetition;

            obj.title = title;
            obj.per = accessing.ToString(true);

            db.SubmitChanges();

            result.code = 0;
            result.message = Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
        }

        private static void getAllChildAccess(ref List<int> ids , int accessID)
        {
            var db = new DataAccessDataContext();

            var obj = db.accessTbls.Where(c => c.accessID == accessID).Select(c=>c.ID).ToList();
            
            foreach (var item in obj)
            {
                ids.Add(item);
                getAllChildAccess(ref ids, item);
            }

        }

        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string getAccess(int accessID)
        {
            ClassCollection.Model.AccessResult result = new Model.AccessResult();
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


            if (!permission.access.access.edit)
            {
                result.result.code = 2;
                result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }

            if (db.accessTbls.Any(c => c.ID == accessID && ( c.accessID == user.accessID || user.accessID==1)) == false)
            {
                result.result.code = 3;
                result.result.message = ClassCollection.Message.ACCESS_NOT_EXIST;
                return new JavaScriptSerializer().Serialize(result);
            }
            var obj = db.accessTbls.Single(c => c.ID == accessID);
            var accessing = Accessing.Deserialize(obj.per);


            result.access = new Model.Access();
            result.access.ID = obj.ID;
            result.access.title = obj.title;
            result.access.per = accessing;

            //result.relation = relation;
            result.result.code = 0;
            result.result.message = Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
        }

        [System.Web.Services.WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string deleteAccess(int accessID)
        {
            ClassCollection.Model.AccessResult result = new Model.AccessResult();
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

            if (!permission.access.access.delete)
            {
                result.result.code = 2;
                result.result.message = ClassCollection.Message.OPERATION_NO_ACCESS;
                return new JavaScriptSerializer().Serialize(result);
            }

            if (db.accessTbls.Any(c => c.ID == accessID) == false)
            {
                result.result.code = 3;
                result.result.message = ClassCollection.Message.ACCESS_NOT_EXIST;
                return new JavaScriptSerializer().Serialize(result);
            }
            var obj = db.accessTbls.Single(c => c.ID == accessID);
            if (obj.userTbls.Any() || db.accessTbls.Any(c => c.accessID == accessID))
            {
                result.result.code = 4;
                result.result.message = ClassCollection.Message.ACCESS_IN_USE;
                return new JavaScriptSerializer().Serialize(result);
            }
            db.accessTbls.DeleteOnSubmit(obj);
            db.SubmitChanges();

            result.result.code = 0;
            result.result.message = Message.OPERATION_SUCCESS;
            return new JavaScriptSerializer().Serialize(result);
        }
    }
}

