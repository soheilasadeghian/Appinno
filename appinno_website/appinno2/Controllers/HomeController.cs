using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace appinno2.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //DateTime dt = DateTime.Now;
            //string date = String.Format("{0:yyyy-dd-mm}", dt);

            var news = appinno2.ClassCollection.WebService.getLatestNewsForList("", "", -1, -1, 1, 1, -1);
            var downloads = appinno2.ClassCollection.WebService.getLatestDownloadForList("", "", -1, -1, 1, 1, -1);
            var events = appinno2.ClassCollection.WebService.getAllEvents("", "", -1, -1, 1, 1, -1);
            var publications = appinno2.ClassCollection.WebService.getLatestPubForList("", "", -1, -1, 1, 1, -1);
            var organizations = appinno2.ClassCollection.WebService.getLatestIoForList("", "", -1, -1, 1, 1, -1);
            var charts = appinno2.ClassCollection.WebService.getLatestChartForList("", "", -1, -1, 1, 1, -1);
            var ListNewsOnHomePage = appinno2.ClassCollection.WebService.getLatestNewsForList("", "", -1, -1, 13, 1, -1);

            var vm = new Models.Index();
            vm.downloads = downloads.download;
            vm.news = news.news;
            vm.events = events.events;
            vm.publications = publications.publications;
            vm.organizations = organizations.io;
            vm.charts = charts.charts;
            vm.ListNewsOnHomePage = ListNewsOnHomePage.news;

            if (downloads.result.code == 1)
            {
                return RedirectToAction("ErrorPage", "Home", new { message = downloads.result.message });
            }
            else if(news.result.code == 1)
            {
                return RedirectToAction("ErrorPage", "Home", new { message = news.result.message });
            }
            else if (events.result.code == 1)
            {
                return RedirectToAction("ErrorPage", "Home", new { message = events.result.message });
            }
            else if (publications.result.code == 1)
            {
                return RedirectToAction("ErrorPage", "Home", new { message = publications.result.message });
            }
            else if (organizations.result.code == 1)
            {
                return RedirectToAction("ErrorPage", "Home", new { message = organizations.result.message });
            }
            else if (charts.result.code == 1)
            {
                return RedirectToAction("ErrorPage", "Home", new { message = charts.result.message });
            }
            else if (ListNewsOnHomePage.result.code == 1)
            {
                return RedirectToAction("ErrorPage", "Home", new { message = ListNewsOnHomePage.result.message });
            }
            else
            {
                return View(vm as object);
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult ErrorPage(string message)

        {
            var vm = new Models.ErrorPage();
            vm.message = message;
            return View(vm as object);
        }

        public ActionResult News(int pagenumber)
        {
            var News = appinno2.ClassCollection.WebService.getLatestNewsForList("", "", -1, -1, 3, pagenumber, -1);
            var vm = new Models.News();
            vm.ListNews = News.news;

            if (News.result.code == 0)
            {
                foreach (var list in vm.ListNews)
                {
                    var tags = News.news.FirstOrDefault(p => p.ID == list.ID).tag;
                    vm.tags = tags;
                }
                vm.pagenumber = pagenumber;
                return View(vm as object);
            }
            else
            {
                return RedirectToAction("ErrorPage", "Home", new { message = News.result.message });
            }
        }

        public ActionResult Events(int pagenumber)
        {
            var ListEvents = appinno2.ClassCollection.WebService.getAllEvents("", "", -1, -1, 3, pagenumber, -1);
            var vm = new Models.Event();
            vm.Events = ListEvents.events;

            if (ListEvents.result.code == 0)
            {
                foreach (var list in vm.Events)
                {
                    var tags = ListEvents.events.FirstOrDefault(p => p.ID == list.ID).tag;
                    vm.tags = tags;
                }
                vm.pagenumber = pagenumber;
                return View(vm as object);
            }
            else
            {
                return RedirectToAction("ErrorPage", "Home", new { message = ListEvents.result.message });
            }
        }

        public ActionResult Publications(int pagenumber)
        {
            var ListPublications = appinno2.ClassCollection.WebService.getLatestPubForList("", "", -1, -1, 3, pagenumber, -1);
            var vm = new Models.Publication();
            vm.Publications = ListPublications.publications;

            if (ListPublications.result.code == 0)
            {
                foreach (var list in vm.Publications)
                {
                    var tags = ListPublications.publications.FirstOrDefault(p => p.ID == list.ID).tag;
                    vm.tags = tags;
                }

                vm.pagenumber = pagenumber;
                return View(vm as object);
            }
            else
            {
                return RedirectToAction("ErrorPage", "Home", new { message = ListPublications.result.message });
            }
        }

        public ActionResult Downloads(int pagenumber)
        {
            var ListDownloads = appinno2.ClassCollection.WebService.getLatestDownloadForList("", "", -1, -1, 3, pagenumber, -1);
            var vm = new Models.Download();
            vm.Downloads = ListDownloads.download;

            if (ListDownloads.result.code == 0)
            {
                foreach (var list in vm.Downloads)
                {
                    var tags = ListDownloads.download.FirstOrDefault(p => p.ID == list.ID).tag;
                    vm.tags = tags;
                }

                vm.pagenumber = pagenumber;
                return View(vm as object);
            }
            else
            {
                return RedirectToAction("ErrorPage", "Home", new { message = ListDownloads.result.message });
            }
        }

        public ActionResult Organizations(int pagenumber)
        {
            var ListOrganizations = appinno2.ClassCollection.WebService.getLatestIoForList("", "", -1, -1, 3, pagenumber, -1);
            var vm = new Models.Organization();
            vm.Organizations = ListOrganizations.io;

            if (ListOrganizations.result.code == 0)
            {
                foreach (var list in vm.Organizations)
                {
                    var tags = ListOrganizations.io.FirstOrDefault(p => p.ID == list.ID).tag;
                    vm.tags = tags;
                }

                vm.pagenumber = pagenumber;
                return View(vm as object);
            }
            else
            {
                return RedirectToAction("ErrorPage", "Home", new { message = ListOrganizations.result.message });
            }
        }

        public ActionResult Charts(int pagenumber)
        {
            var ListCharts = appinno2.ClassCollection.WebService.getLatestChartForList("", "", -1, -1, 3, pagenumber, -1);
            var vm = new Models.Chart();
            vm.Charts = ListCharts.charts;

            if (ListCharts.result.code == 0)
            {
                foreach (var list in vm.Charts)
                {
                    var tags = ListCharts.charts.FirstOrDefault(p => p.ID == list.ID).tag;
                    vm.tags = tags;
                }

                vm.pagenumber = pagenumber;
                return View(vm as object);
            }
            else
            {
                return RedirectToAction("ErrorPage", "Home", new { message = ListCharts.result.message });
            }
        }

        public ActionResult DownloadItem(string Id)
        {
            var result = appinno2.ClassCollection.WebService.getDownload(Id);
            if (result.result.code == 0)
            {
                return View(result as object);
            }
            else
            {
                return RedirectToAction("ErrorPage", "Home", new { message = result.result.message });
            }
        }

        public ActionResult NewsDetail(int Id)
        {
            var SingleNews = appinno2.ClassCollection.WebService.getSingleNews(Id);
            var vm = new Models.NewsDetail();
            vm.ListNewsDetail = SingleNews.singlenews;

            if (SingleNews.result.code == 0)
            {
                vm.Id = Id;
                return View(vm as object);
            }
            else
            {
                return RedirectToAction("ErrorPage", "Home", new { message = SingleNews.result.message });
            }
        }

        public ActionResult EventDetail(int Id)
        {
            var SingleEvent = appinno2.ClassCollection.WebService.getSingleEvents(Id);
            var vm = new Models.EventDetail();
            vm.ListEventDetail = SingleEvent.singleevents;
            if (SingleEvent.result.code == 0)
            {
                vm.Id = Id;
                return View(vm as object);
            }
            else
            {
                return RedirectToAction("ErrorPage", "Home", new { message = SingleEvent.result.message });
            }
        }

        public ActionResult PublicationDetail(int Id)
        {
            var SinglePublication = appinno2.ClassCollection.WebService.getSinglePub(Id);
            var vm = new Models.PublicationDetail();
            vm.ListPublicationDetail = SinglePublication.singlepublications;
            if (SinglePublication.result.code == 0)
            {
                vm.Id = Id;
                return View(vm as object);
            }
            else
            {
                return RedirectToAction("ErrorPage", "Home", new { message = SinglePublication.result.message });
            }
        }

        public ActionResult DownloadDetail(int Id)
        {
            var SingleDownload = appinno2.ClassCollection.WebService.getSingleDownload(Id);
            var vm = new Models.DownloadDetail();
            vm.ListDownloadDetail = SingleDownload.singledownload;
            if (SingleDownload.result.code == 0)
            {
                vm.Id = Id;
                return View(vm as object);
            }
            else
            {
                return RedirectToAction("ErrorPage", "Home", new { message = SingleDownload.result.message });
            }
        }

        public ActionResult OrganizationDetail(int Id)
        {
            var SingleOrganization = appinno2.ClassCollection.WebService.getSingleIo(Id);
            var vm = new Models.OrganizationDetail();
            vm.ListOrganizationDetail = SingleOrganization.singleio;
            if (SingleOrganization.result.code == 0)
            {
                vm.Id = Id;
                return View(vm as object);
            }
            else
            {
                return RedirectToAction("ErrorPage", "Home", new { message = SingleOrganization.result.message });
            }
        }

        public ActionResult NewsFilterTag(long tagID,int pagenumber)
        {
            var News = appinno2.ClassCollection.WebService.getLatestNewsForList("", "", -1, -1, 3, pagenumber, tagID);
            var vm = new Models.News();
            vm.ListNews = News.news;

            if (News.result.code == 0)
            {
                foreach (var list in vm.ListNews)
                {
                    var tags = News.news.FirstOrDefault(p => p.ID == list.ID).tag;
                    vm.tags = tags;
                }
                vm.pagenumber = pagenumber;
                vm.tagID = tagID;
                return View(vm as object);
            }
            else
            {
                return RedirectToAction("ErrorPage", "Home", new { message = News.result.message });
            }
        }

        public ActionResult EventsFilterTag(long tagID, int pagenumber)
        {
            var ListEvents = appinno2.ClassCollection.WebService.getAllEvents("", "", -1, -1, 3, pagenumber, tagID);
            var vm = new Models.Event();
            vm.Events = ListEvents.events;

            if (ListEvents.result.code == 0)
            {
                foreach (var list in vm.Events)
                {
                    var tags = ListEvents.events.FirstOrDefault(p => p.ID == list.ID).tag;
                    vm.tags = tags;
                }

                vm.pagenumber = pagenumber;
                vm.tagID = tagID;
                return View(vm as object);
            }
            else
            {
                return RedirectToAction("ErrorPage", "Home", new { message = ListEvents.result.message });
            }
        }

        public ActionResult PublicationsFilterTag(long tagID, int pagenumber)
        {
            var ListPublications = appinno2.ClassCollection.WebService.getLatestPubForList("", "", -1, -1, 3, pagenumber, tagID);
            var vm = new Models.Publication();
            vm.Publications = ListPublications.publications;

            if (ListPublications.result.code == 0)
            {
                foreach (var list in vm.Publications)
                {
                    var tags = ListPublications.publications.FirstOrDefault(p => p.ID == list.ID).tag;
                    vm.tags = tags;
                }

                vm.pagenumber = pagenumber;
                vm.tagID = tagID;
                return View(vm as object);
            }
            else
            {
                return RedirectToAction("ErrorPage", "Home", new { message = ListPublications.result.message });
            }
        }

        public ActionResult DownloadsFilterTag(long tagID, int pagenumber)
        {
            var ListDownloads = appinno2.ClassCollection.WebService.getLatestDownloadForList("", "", -1, -1, 3, pagenumber, tagID);
            var vm = new Models.Download();
            vm.Downloads = ListDownloads.download;

            if (ListDownloads.result.code == 0)
            {
                foreach (var list in vm.Downloads)
                {
                    var tags = ListDownloads.download.FirstOrDefault(p => p.ID == list.ID).tag;
                    vm.tags = tags;
                }

                vm.pagenumber = pagenumber;
                vm.tagID = tagID;
                return View(vm as object);
            }
            else
            {
                return RedirectToAction("ErrorPage", "Home", new { message = ListDownloads.result.message });
            }
        }

        public ActionResult OrganizationsFilterTag(long tagID, int pagenumber)
        {
            var ListOrganizations = appinno2.ClassCollection.WebService.getLatestIoForList("", "", -1, -1, 3, pagenumber, tagID);
            var vm = new Models.Organization();
            vm.Organizations = ListOrganizations.io;

            if (ListOrganizations.result.code == 0)
            {
                foreach (var list in vm.Organizations)
                {
                    var tags = ListOrganizations.io.FirstOrDefault(p => p.ID == list.ID).tag;
                    vm.tags = tags;
                }

                vm.pagenumber = pagenumber;
                vm.tagID = tagID;
                return View(vm as object);
            }
            else
            {
                return RedirectToAction("ErrorPage", "Home", new { message = ListOrganizations.result.message });
            }
        }

        public ActionResult ChartsFilterTag(long tagID, int pagenumber)
        {
            var ListCharts = appinno2.ClassCollection.WebService.getLatestChartForList("", "", -1, -1, 3, pagenumber, tagID);
            var vm = new Models.Chart();
            vm.Charts = ListCharts.charts;

            if (ListCharts.result.code == 0)
            {
                foreach (var list in vm.Charts)
                {
                    var tags = ListCharts.charts.FirstOrDefault(p => p.ID == list.ID).tag;
                    vm.tags = tags;
                }

                vm.pagenumber = pagenumber;
                vm.tagID = tagID;
                return View(vm as object);
            }
            else
            {
                return RedirectToAction("ErrorPage", "Home", new { message = ListCharts.result.message });
            }
        }

        public ActionResult LikeUnlike(long newsid,long userid,bool LikeOrUnlike)
        {
            var likeUnlike = appinno2.ClassCollection.WebService.likeUnlikeNews(userid, newsid, LikeOrUnlike);
            if (likeUnlike.code == 0)
            {
                //var code = likeUnlike.code;
                return new EmptyResult();
            }
            else
            {
                return RedirectToAction("ErrorPage", "Home", new { message = likeUnlike.message });
            }
        }

    }
}