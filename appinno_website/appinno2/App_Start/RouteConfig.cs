using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace appinno2
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "News",
                url: "News/{pagenumber}",
                defaults: new { controller = "Home", action = "News" }
          );
            routes.MapRoute(
              name: "Events",
              url: "Events/{pagenumber}",
              defaults: new { controller = "Home", action = "Events" }
          );
            routes.MapRoute(
              name: "Publications",
              url: "Publications/{pagenumber}",
              defaults: new { controller = "Home", action = "Publications" }
          );
            routes.MapRoute(
              name: "Downloads",
              url: "Downloads/{pagenumber}",
              defaults: new { controller = "Home", action = "Downloads" }
          );
            routes.MapRoute(
              name: "Organizations",
              url: "Organizations/{pagenumber}",
              defaults: new { controller = "Home", action = "Organizations" }
          );
            routes.MapRoute(
              name: "Charts",
              url: "Charts/{pagenumber}",
              defaults: new { controller = "Home", action = "Charts" }
          );
            routes.MapRoute(
            name: "NewsDetail",
            url: "NewsDetail/{Id}",
            defaults: new { controller = "Home", action = "NewsDetail" }
          );
            routes.MapRoute(
            name: "EventDetail",
            url: "EventDetail/{Id}",
            defaults: new { controller = "Home", action = "EventDetail" }
          );
            routes.MapRoute(
            name: "PublicationDetail",
            url: "PublicationDetail/{Id}",
            defaults: new { controller = "Home", action = "PublicationDetail" }
          );
            routes.MapRoute(
            name: "DownloadDetail",
            url: "DownloadDetail/{Id}",
            defaults: new { controller = "Home", action = "DownloadDetail" }
          );
            routes.MapRoute(
            name: "OrganizationDetail",
            url: "OrganizationDetail/{Id}",
            defaults: new { controller = "Home", action = "OrganizationDetail" }
          );
            routes.MapRoute(
            name: "NewsFilterTag",
            url: "NewsFilterTag/{tagID}/{pagenumber}",
            defaults: new { controller = "Home", action = "NewsFilterTag" }
          );
            routes.MapRoute(
             name: "EventsFilterTag",
             url: "EventsFilterTag/{tagID}/{pagenumber}",
             defaults: new { controller = "Home", action = "EventsFilterTag" }
           );
            routes.MapRoute(
             name: "PublicationsFilterTag",
             url: "PublicationsFilterTag/{tagID}/{pagenumber}",
             defaults: new { controller = "Home", action = "PublicationsFilterTag" }
           );
            routes.MapRoute(
             name: "DownloadsFilterTag",
             url: "DownloadsFilterTag/{tagID}/{pagenumber}",
             defaults: new { controller = "Home", action = "DownloadsFilterTag" }
           );
            routes.MapRoute(
             name: "OrganizationsFilterTag",
             url: "OrganizationsFilterTag/{tagID}/{pagenumber}",
             defaults: new { controller = "Home", action = "OrganizationsFilterTag" }
           );
            routes.MapRoute(
             name: "ChartsFilterTag",
             url: "ChartsFilterTag/{tagID}/{pagenumber}",
             defaults: new { controller = "Home", action = "ChartsFilterTag" }
           );

            routes.MapRoute(
             name: "Default",
             url: "{controller}/{action}/{id}",
             defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
          );
        }
    }
}
