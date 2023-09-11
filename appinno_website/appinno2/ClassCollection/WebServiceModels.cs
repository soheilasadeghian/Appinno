using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace appinno2.ClassCollection
{
    public class WebServiceModels
    {
        public class Result
        {
            public int code { set; get; }
            public string message { set; get; }
        }
        public class Tag
        {
            public long ID { get; set; }
            public string title { get; set; }
        }

        #region News
        public class LatestNewsForListResult
        {
            public Result result { get; set; }
            public List<LatestNewsForList> news { get; set; }
        }
        public class LatestNewsForList
        {
            public long ID { get; set; }
            public string title { get; set; }
            public string summary { get; set; }
            public string image { get; set; }

            public int like { get; set; }
            public int unlike { get; set; }
            public string publishDate { get; set; }
            public int viewCount { get; set; }
            public List<Tag> tag { get; set; }

        }
        #endregion

        #region downloads
        public class LatestDownloadForListResult
        {
            public Result result { get; set; }
            public List<LatestDownloadForList> download { get; set; }
        }
        public class LatestDownloadForList
        {
            public long ID { get; set; }
            public string title { get; set; }
            public string summary { get; set; }
            public string image { get; set; }

            public int like { get; set; }
            public int unlike { get; set; }
            public string publishDate { get; set; }
            public int viewCount { get; set; }
            public List<Tag> tag { get; set; }

        }
        #endregion

        #region DownloadItem
        public class DownloadResult
        {
            public Result result { get; set; }
            public fullDownload download { get; set; }
        }
        public class fullDownload
        {
            public long ID { get; set; }
            public long creatorID { get; set; }
            public string title { get; set; }
            public int like { get; set; }
            public int unlike { get; set; }
            public string publishDate { get; set; }
            public int viewCount { get; set; }
            public List<downloadContent> contents { get; set; }
            public List<downloadGroupSubGroup> groups { get; set; }
            public List<Tag> tag { get; set; }
        }
        public class downloadContent
        {
            public long ID { get; set; }
            public int type { get; set; }
            public string value { get; set; }
            public int? fileSize { get; set; }
            public string fileType { get; set; }
            public long? downloadCount { get; set; }
        }
        public class downloadGroupSubGroup
        {
            public long ID { get; set; }
            public long downloadID { get; set; }
            public long subGroupID { get; set; }
            public long groupID { get; set; }
            public string subGroupTitle { get; set; }
            public string groupTitle { get; set; }
        }
        #endregion

        #region events
        public class LatestEventsForListResult
        {
            public Result result { get; set; }
            public List<LatestEventsForList> events { get; set; }
        }
        public class LatestEventsForList
        {
            public long ID { get; set; }
            public string title { get; set; }
            public string summary { get; set; }
            public string image { get; set; }

            public int like { get; set; }
            public int unlike { get; set; }
            public string toDate { get; set; }
            public string fromDate { get; set; }
            public int viewCount { get; set; }
            public List<Tag> tag { get; set; }

        }
        #endregion

        #region publication
        public class LatestPublicationsForListResult
        {
            public Result result { get; set; }
            public List<LatestPublicationsForList> publications { get; set; }
        }
        public class LatestPublicationsForList
        {
            public long ID { get; set; }
            public string title { get; set; }
            public string summary { get; set; }
            public string image { get; set; }

            public int like { get; set; }
            public int unlike { get; set; }
            public string publishDate { get; set; }
            public int viewCount { get; set; }
            public List<Tag> tag { get; set; }

        }
        #endregion

        #region organization
        public class LatestIoForListResult
        {
            public Result result { get; set; }
            public List<LatestIoForList> io { get; set; }
        }
        public class LatestIoForList
        {
            public long ID { get; set; }
            public string title { get; set; }
            public string summary { get; set; }
            public string image { get; set; }
            public int like { get; set; }
            public int unlike { get; set; }
            public string publishDate { get; set; }
            public int viewCount { get; set; }
            public List<Tag> tag { get; set; }

        }
        #endregion

        #region Chart
        public class LatestChartForListResult
        {
            public Result result { get; set; }
            public List<LatestChartForList> charts { get; set; }
        }
        public class LatestChartForList
        {
            public long ID { get; set; }
            public string title { get; set; }
            public string publishDate { get; set; }
            public int viewCount { get; set; }
            public List<Tag> tag { get; set; }
        }
        #endregion

        #region SingleNews
        public class SingleNewsResult
        {
            public Result result { get; set; }
            public singleFullNews singlenews { get; set; }
        }
        public class singleFullNews
        {
            public long ID { get; set; }
            public long creatorID { get; set; }
            public string title { get; set; }
            public int like { get; set; }
            public int unlike { get; set; }
            public string publishDate { get; set; }
            public int viewCount { get; set; }
            public string text { get; set; }
            public string image { get; set; }
            public string video { get; set; }

        }
        #endregion

        #region SingleEvents
        public class SingleEventsResult
        {
            public Result result { get; set; }
            public singleFullEvents singleevents { get; set; }
        }
        public class singleFullEvents
        {
            public long ID { get; set; }
            public long creatorID { get; set; }
            public string title { get; set; }
            public int like { get; set; }
            public int unlike { get; set; }
            public string toDate { get; set; }
            public string fromDate { get; set; }
            public int viewCount { get; set; }
            public string text { get; set; }
            public string image { get; set; }
            public string video { get; set; }
        }
        #endregion

        #region SinglePublications
        public class SinglePublicationsResult
        {
            public Result result { get; set; }
            public singleFullPublications singlepublications { get; set; }
        }
        public class singleFullPublications
        {
            public long ID { get; set; }
            public long creatorID { get; set; }
            public string title { get; set; }
            public int like { get; set; }
            public int unlike { get; set; }
            public string publishDate { get; set; }
            public int viewCount { get; set; }
            public string text { get; set; }
            public string image { get; set; }
            public string video { get; set; }
        }
        #endregion

        #region SingleDownloads
        public class SingleDownloadResult
        {
            public Result result { get; set; }
            public SingleFullDownload singledownload { get; set; }
        }
        public class SingleFullDownload
        {
            public long ID { get; set; }
            public long creatorID { get; set; }
            public string title { get; set; }
            public int like { get; set; }
            public int unlike { get; set; }
            public string publishDate { get; set; }
            public int viewCount { get; set; }
            public string text { get; set; }
            public string image { get; set; }
            public string file { get; set; }
        }
        #endregion

        #region SingleOrganization
        public class SingleIoResult
        {
            public Result result { get; set; }
            public singleFullIo singleio { get; set; }
        }
        public class singleFullIo
        {
            public long ID { get; set; }
            public long creatorID { get; set; }
            public string title { get; set; }
            public int like { get; set; }
            public int unlike { get; set; }
            public string publishDate { get; set; }
            public int viewCount { get; set; }
            public string text { get; set; }
            public string image { get; set; }
            public string video { get; set; }
        }
        #endregion

        public class LikeUnlike
        {
            public int code { set; get; }
            public string message { set; get; }
        }
    }
}