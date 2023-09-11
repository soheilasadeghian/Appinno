using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppinnoNew.ClassCollection
{
    public static class Model
    {
        public class Result
        {
            public int code { set; get; }
            public string message { set; get; }
        }
        public class AdminMessageResult
        {
            public Result result { get; set; }
            public string message { get; set; }
            public string image { get; set; }
        }
        public class LongResult
        {
            public long value { get; set; }
            public Result result { get; set; }
        }
        //soheila-start-registerNews
        public class LongResultFullMyIran
        {
            public long value { get; set; }
            public Result result { get; set; }
        }
        //soheila-end-registerNews
        public class AccessingResult
        {
            public Result result { get; set; }
            public ClassCollection.Accessing access { get; set; }
        }
        public class AccessListResult
        {
            public Result result { get; set; }
            public int pageCount { get; set; }
            public List<Access> access { get; set; }
            public int totalCount { get; set; }
        }
        public class AccessResult
        {
            public Result result { get; set; }
            public Access access { get; set; }
        }
        public class Access
        {
            public int ID { get; set; }
            public string title { get; set; }
            public ClassCollection.Accessing per { get; set; }
        }
        public class OpratorListResult
        {
            public Result result { get; set; }
            public int pageCount { get; set; }
            public List<Oprator> oprator { get; set; }
            public int totalCount { get; set; }
        }
        public class OpratorResult
        {
            public Result result { get; set; }
            public Oprator oprator { get; set; }
        }
        public class Oprator
        {
            public long ID { get; set; }
            public int accessID { get; set; }
            public string accessTitle { get; set; }
            public string name { get; set; }
            public string family { get; set; }
            public string emailtell { get; set; }
        }
        public class GroupListResult
        {
            public Result result { get; set; }
            public List<Group> groups { get; set; }
        }
        public class GroupResult
        {
            public Result result { get; set; }
            public Group group { get; set; }
        }
        public class Group
        {
            public long ID { get; set; }
            public string title { get; set; }
            public string regDate { get; set; }
        }
        public class subGroupListResult
        {
            public Result result { get; set; }
            public List<SubGroup> subGroup { get; set; }
        }

        public class SubGroupResult
        {
            public Result result { get; set; }
            public SubGroup subGroup { get; set; }
        }
        public class SubGroup
        {
            public long ID { get; set; }
            public string title { get; set; }
            public bool canPush { get; set; }
            public bool toPartner { get; set; }
            public bool toAll { get; set; }
            public List<PushTo> pushTo { get; set; }
        }
        public class PushTo
        {
            public long groupID { get; set; }
            public long subGroupID { get; set; }
            public string subGroupTitle { get; set; }
            public string groupTitle { get; set; }
        }
        public class PartnerListResult
        {

            public Result result { get; set; }
            public int pageCount { get; set; }
            public List<ClassCollection.Excel.Import.PartnerLoader.Partner> partner { get; set; }
            public int totalCount { get; set; }
        }
        public class PartnerResult
        {
            public Result result { get; set; }
            public ClassCollection.Excel.Import.PartnerLoader.Partner partner { get; set; }
        }

        public class Tag
        {
            public long ID { get; set; }
            public string title { get; set; }
        }
        public class TagResult
        {
            public Result result { get; set; }
            public Tag tag { get; set; }
        }
        public class TagListResult
        {
            public int pageCount { get; set; }
            public Result result { get; set; }
            public List<Tag> tag { get; set; }
            public int totalCount { get; set; }
        }

        public class Push
        {
            public long ID { get; set; }
            public bool toAll { get; set; }
            public bool toPartner { get; set; }
            public string title { get; set; }
            public string text { get; set; }
            public string date { get; set; }
        }
        public class PushResult
        {
            public Result result { get; set; }
            public Push push { get; set; }
            public List<PushTo> pushTo { get; set; }

        }
        public class PushListResult
        {
            public Result result { get; set; }
            public int pageCount { get; set; }
            public List<Push> push { get; set; }
            public int totalCount { get; set; }
        }
        public class mUserResult
        {
            public Result result { get; set; }
            public mUser user { get; set; }

        }
        public class userCanPushResult
        {
            public Result result { get; set; }
            public List<PushTo> groups { get; set; }
        }
        public class mUserListResult
        {
            public int pageCount { get; set; }
            public Result result { get; set; }
            public List<mUser> user { get; set; }
            public int totalCount { get; set; }

        }
        public class adminComment
        {
            public long ID { get; set; }
            public long newsID { get; set; }
            public long mUserID { get; set; }
            public string fullName { get; set; }
            public string mobileOrTell { get; set; }
            public string text { get; set; }
            public bool isBlock { get; set; }
            public string regDate { get; set; }
            public string newsTitle { get; set; }
        }
        public class adminCommentListResult
        {
            public int pageCount { get; set; }
            public Result result { get; set; }
            public List<adminComment> comment { get; set; }
            public int totalCount { get; set; }
        }
        public class adminCommentResult
        {
            public Result result { get; set; }
            public adminComment comment { get; set; }
        }

        public class mUser
        {
            public long ID { get; set; }
            public string name { get; set; }
            public string family { get; set; }
            public string emailormobile { get; set; }
            public string regDate { get; set; }
            public bool isBlock { get; set; }
            public string level { get; set; }
            public string innerTell { get; set; }
            public List<PushTo> groups { get; set; }
        }
        public class mUserInfoResult
        {
            public Result result { get; set; }
            public mUserInfo userInfo { get; set; }
        }
        public class mUserInfo
        {
            public long ID { get; set; }
            public string name { get; set; }
            public string family { get; set; }
            public string emailormobile { get; set; }
            public string email { get; set; }
            public string innerTell { get; set; }
            public string optionalMobile { get; set; }
            public string level { get; set; }
        }

        #region News

        public class NewsResult
        {
            public Result result { get; set; }
            public fullNews news { get; set; }
        }
        public class fullNews
        {
            public long ID { get; set; }
            public long? userID { get; set; }
            public long? mUserID { get; set; }
            public string title { get; set; }
            public int like { get; set; }
            public int unlike { get; set; }
            public string publishDate { get; set; }
            public bool isBlock { get; set; }
            public int viewCount { get; set; }
            public List<newsContent> contents { get; set; }
            public List<newsGroupSubGroup> groups { get; set; }
            public List<Tag> tag { get; set; }
        }
        public class newsContent
        {
            public long ID { get; set; }
            public int type { get; set; }
            public string value { get; set; }
        }
        public class PollContent
        {
            public long ID { get; set; }
            public int type { get; set; }
            public string value { get; set; }
        }
        public class newsGroupSubGroup
        {
            public long ID { get; set; }
            public long newsID { get; set; }
            public long subGroupID { get; set; }
            public long groupID { get; set; }
            public string subGroupTitle { get; set; }
            public string groupTitle { get; set; }
        }
        public class pollGroupSubGroup
        {
            public long ID { get; set; }
            public long pollID { get; set; }
            public long subGroupID { get; set; }
            public long groupID { get; set; }
            public string subGroupTitle { get; set; }
            public string groupTitle { get; set; }
        }
        public class NewsAdminResult
        {
            public Result result { get; set; }
            public int pageCount { get; set; }
            public List<NewsAdmin> news { get; set; }
            public int totalCount { get; set; }
        }
        public class NewsAdmin
        {
            public long ID { get; set; }
            public string title { get; set; }
            public string regDate { get; set; }
            public string publishDate { get; set; }
            public bool isBlock { get; set; }
        }
        public class Comment
        {
            public long ID { get; set; }
            public string text { get; set; }
            public string fullName { get; set; }
        }
        public class CommentListResult
        {
            public Result result { get; set; }
            public List<Comment> comment { get; set; }
        }
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
        public class SingleNewsResult
        {
            public Result result { get; set; }
            public singleFullNews singlenews { get; set; }
        }
        public class singleFullNews
        {
            public long ID { get; set; }
            public long? userID { get; set; }
            public string title { get; set; }
            public int like { get; set; }
            public int unlike { get; set; }
            public string publishDate { get; set; }
            public int viewCount { get; set; }
            public string text { get; set; }
            public string image { get; set; }
            public string video { get; set; }

        }
        public class LatestNewsForNotificationResult
        {
            public Result result { get; set; }
            public List<LatestNewsForNotification> news { get; set; }
        }
        public class LatestNewsForNotification
        {
            public long ID { get; set; }
            public string title { get; set; }

        }
        #endregion

        #region IO

        public class IoResult
        {
            public Result result { get; set; }
            public fullIo io { get; set; }
        }
        public class fullIo
        {
            public long ID { get; set; }
            public long? userID { get; set; }
            public long? mUserID { get; set; }
            public string title { get; set; }
            public int like { get; set; }
            public int unlike { get; set; }
            public string publishDate { get; set; }
            public bool isBlock { get; set; }
            public int viewCount { get; set; }
            public List<ioContent> contents { get; set; }
            public List<ioGroupSubGroup> groups { get; set; }
            public List<Tag> tag { get; set; }
        }
        public class ioContent
        {
            public long ID { get; set; }
            public int type { get; set; }
            public string value { get; set; }
        }
        public class ioGroupSubGroup
        {
            public long ID { get; set; }
            public long ioID { get; set; }
            public long subGroupID { get; set; }
            public long groupID { get; set; }
            public string subGroupTitle { get; set; }
            public string groupTitle { get; set; }
        }

        public class IoAdminResult
        {
            public Result result { get; set; }
            public int pageCount { get; set; }
            public List<IoAdmin> io { get; set; }
            public int totalCount { get; set; }
        }
        public class IoAdmin
        {
            public long ID { get; set; }
            public string title { get; set; }
            public string regDate { get; set; }
            public string publishDate { get; set; }
            public bool isBlock { get; set; }
        }

        #endregion

        #region publication

        public class publicationResult
        {
            public Result result { get; set; }
            public fullpublication publication { get; set; }
        }
        public class fullpublication
        {
            public long ID { get; set; }
            public long creatorID { get; set; }
            public long? userID { get; set; }
            public long? mUserID { get; set; }
            public string title { get; set; }
            public int like { get; set; }
            public int unlike { get; set; }
            public string publishDate { get; set; }
            public bool isBlock { get; set; }
            public int viewCount { get; set; }
            public List<publicationContent> contents { get; set; }
            public List<publicationGroupSubGroup> groups { get; set; }
            public List<Tag> tag { get; set; }
        }
        public class publicationContent
        {
            public long ID { get; set; }
            public int type { get; set; }
            public string value { get; set; }
        }
        public class publicationGroupSubGroup
        {
            public long ID { get; set; }
            public long publicationID { get; set; }
            public long subGroupID { get; set; }
            public long groupID { get; set; }
            public string subGroupTitle { get; set; }
            public string groupTitle { get; set; }
        }

        public class publicationAdminResult
        {
            public Result result { get; set; }
            public int pageCount { get; set; }
            public List<publicationAdmin> publication { get; set; }
            public int totalCount { get; set; }
        }
        public class publicationAdmin
        {
            public long ID { get; set; }
            public string title { get; set; }
            public string regDate { get; set; }
            public string publishDate { get; set; }
            public bool isBlock { get; set; }
        }
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
        public class SinglePublicationsResult
        {
            public Result result { get; set; }
            public singleFullPublications singlepublications { get; set; }
        }
        public class singleFullPublications
        {
            public long ID { get; set; }
            public long? userID { get; set; }
            public long? mUserID { get; set; }
            public string title { get; set; }
            public int like { get; set; }
            public int unlike { get; set; }
            public string publishDate { get; set; }
            public int viewCount { get; set; }
            public string text { get; set; }
            public string image { get; set; }
            public string video { get; set; }
        }
        public class fullPublications
        {
            public long ID { get; set; }
            public long creatorID { get; set; }
            public string title { get; set; }
            public int like { get; set; }
            public int unlike { get; set; }
            public string publishDate { get; set; }
            public int viewCount { get; set; }
            public List<publicationContent> contents { get; set; }
            public List<publicationGroupSubGroup> groups { get; set; }
        }
        public class LatestPublicationsForNotificationResult
        {
            public Result result { get; set; }
            public List<LatestPublicationsForNotification> publications { get; set; }
        }
        public class LatestPublicationsForNotification
        {
            public long ID { get; set; }
            public string title { get; set; }

        }
        #endregion

        #region events

        public class eventsResult
        {
            public Result result { get; set; }
            public fullevents events { get; set; }
        }
        public class fullevents
        {
            public long ID { get; set; }
            public long creatorID { get; set; }
            public long? userID { get; set; }
            public long? mUserID { get; set; }
            public string title { get; set; }
            public int like { get; set; }
            public int unlike { get; set; }
            public string toDate { get; set; }
            public string fromDate { get; set; }
            public bool isBlock { get; set; }
            public int viewCount { get; set; }
            public List<eventsContent> contents { get; set; }
            public List<eventsGroupSubGroup> groups { get; set; }
            public List<Tag> tag { get; set; }
        }
        public class eventsContent
        {
            public long ID { get; set; }
            public int type { get; set; }
            public string value { get; set; }
        }
        public class eventsGroupSubGroup
        {
            public long ID { get; set; }
            public long eventsID { get; set; }
            public long subGroupID { get; set; }
            public long groupID { get; set; }
            public string subGroupTitle { get; set; }
            public string groupTitle { get; set; }
        }

        public class eventsAdminResult
        {
            public Result result { get; set; }
            public int pageCount { get; set; }
            public List<eventsAdmin> events { get; set; }
            public int totalCount { get; set; }
        }
        public class eventsAdmin
        {
            public long ID { get; set; }
            public string title { get; set; }
            public string regDate { get; set; }
            public string toDate { get; set; }
            public string fromDate { get; set; }
            public bool isBlock { get; set; }
        }
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
        public class EventsResult
        {
            public Result result { get; set; }
            public fullEvents events { get; set; }
        }
        public class SingleEventsResult
        {
            public Result result { get; set; }
            public singleFullEvents singleevents { get; set; }
        }

        public class fullEvents
        {
            public long ID { get; set; }
            public long? userID { get; set; }
            public long? mUserID { get; set; }
            public string title { get; set; }
            public int like { get; set; }
            public int unlike { get; set; }
            public string toDate { get; set; }
            public string fromDate { get; set; }
            public int viewCount { get; set; }
            public List<eventsContent> contents { get; set; }
            public List<eventsGroupSubGroup> groups { get; set; }
            public List<Tag> tag { get; set; }
        }
        public class singleFullEvents
        {
            public long ID { get; set; }
            public long creatorID { get; set; }
            public long? userID { get; set; }
            public long? mUserID { get; set; }
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
        public class LatestEventsForNotificationResult
        {
            public Result result { get; set; }
            public List<LatestEventsForNotification> events { get; set; }
        }
        public class LatestEventsForNotification
        {
            public long ID { get; set; }
            public string title { get; set; }
        }
        public class DaysWithEventForListResult
        {
            public Result result { get; set; }
            public List<DaysWithEventForList> date { get; set; }
        }
        public class DaysWithEventForList
        {
            public int year { get; set; }
            public int month { get; set; }
            public int day { get; set; }
        }
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
            public List<Model.Tag> tag { get; set; }

        }
        public class SingleIoResult
        {
            public Result result { get; set; }
            public singleFullIo singleio { get; set; }
        }
        public class singleFullIo
        {
            public long ID { get; set; }
            public long? userID { get; set; }
            public long? mUserID { get; set; }
            public string title { get; set; }
            public int like { get; set; }
            public int unlike { get; set; }
            public string publishDate { get; set; }
            public int viewCount { get; set; }
            public string text { get; set; }
            public string image { get; set; }
            public string video { get; set; }
        }

        public class LatestIoForNotificationResult
        {
            public Result result { get; set; }
            public List<LatestIoForNotification> io { get; set; }
        }
        public class LatestIoForNotification
        {
            public long ID { get; set; }
            public string title { get; set; }

        }

        #endregion

        #region download
        public class downloadResult
        {
            public Result result { get; set; }
            public fulldownload download { get; set; }
        }
        public class fulldownload
        {
            public long ID { get; set; }
            public long creatorID { get; set; }
            public long? userID { get; set; }
            public long? mUserID { get; set; }
            public string title { get; set; }
            public int like { get; set; }
            public int unlike { get; set; }
            public string publishDate { get; set; }
            public bool isBlock { get; set; }
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

        public class downloadAdminResult
        {
            public Result result { get; set; }
            public int pageCount { get; set; }
            public List<downloadAdmin> download { get; set; }
        }
        public class downloadAdmin
        {
            public long ID { get; set; }
            public string title { get; set; }
            public string regDate { get; set; }
            public string publishDate { get; set; }
            public bool isBlock { get; set; }
        }
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
        public class DownloadResult
        {
            public Result result { get; set; }
            public fullDownload download { get; set; }
        }
        public class SingleDownloadResult
        {
            public Result result { get; set; }
            public SingleFullDownload singledownload { get; set; }
        }
        public class SingleFullDownload
        {
            public long ID { get; set; }
            public long? userID { get; set; }
            public long? mUserID { get; set; }
            public string title { get; set; }
            public int like { get; set; }
            public int unlike { get; set; }
            public string publishDate { get; set; }
            public int viewCount { get; set; }
            public string text { get; set; }
            public string image { get; set; }
            public string file { get; set; }
        }
        public class fullDownload
        {
            public long ID { get; set; }
            public long? userID { get; set; }
            public long? mUserID { get; set; }
            public string title { get; set; }
            public int like { get; set; }
            public int unlike { get; set; }
            public string publishDate { get; set; }
            public int viewCount { get; set; }
            public List<downloadContent> contents { get; set; }
            public List<downloadGroupSubGroup> groups { get; set; }
            public List<Tag> tag { get; set; }
        }
        public class LatestDownloadForNotificationResult
        {
            public Result result { get; set; }
            public List<LatestDownloadForNotification> download { get; set; }
        }
        public class LatestDownloadForNotification
        {
            public long ID { get; set; }
            public string title { get; set; }

        }
        #endregion

        #region poll
        //soheila-start-poll
        public class PollListPanelResult
        {
            public Result result { get; set; }
            public int pageCount { get; set; }
            public List<PollPanel> poll { get; set; }
            public int totalCount { get; set; }
        }
        public class PollPanel
        {
            public long ID { get; set; }
            public string text { get; set; }
            public string image { get; set; }
            public string startDate { get; set; }
            public string regDate { get; set; }
            public string finishedDate { get; set; }
            public long viewCount { get; set; }
            public bool isPublic { get; set; }
            public bool isBlock { get; set; }
            public bool isFinished { get; set; }
            public List<OptionReport> option { get; set; }

        }
        public class OptionReport
        {
            public long ID { get; set; }
            public long count { get; set; }
            public string text { get; set; }
        }

        public class PollPanelResult
        {
            public Result result { get; set; }
            public PollPanel poll { get; set; }
        }
        public class BoolResult
        {
            public bool value { get; set; }
            public Result result { get; set; }
        }
        public class PollReportResult
        {
            public Result result { get; set; }
            public PollReport pollReport { get; set; }
        }
        public class PollReport
        {
            public long ID { get; set; }
            public long Total { get; set; }
            public List<OptionReport> option { get; set; }

        }
        public class PollResult
        {
            public Result result { get; set; }
            public Poll poll { get; set; }
        }
        public class Poll
        {
            public long ID { get; set; }
            public long voteCount { get; set; }
            public long viewCount { get; set; }
            public bool isPublic { get; set; }
            public bool isBlock { get; set; }
            public bool canEdit { get; set; }
            public string finishedDate { get; set; }
            public string startDate { get; set; }
            public List<PollContent> content { get; set; }
            public List<OptionReport> option { get; set; }
            public List<pollGroupSubGroup> groups { get; set; }
            public List<Tag> tag { get; set; }
            public string summery { get; set; }
            public string image { get; set; }
        }
        public class PollListResult
        {
            public Result result { get; set; }
            public List<Poll> poll { get; set; }
        }
        public class UploadRequestResult
        {
            public Result Result { get; set; }
            public string Token { get; set; }
        }
        public class UploadFileResult
        {
            public Result Result { get; set; }
            public string image { get; set; }
        }
        //soheila-end-poll
        #endregion

        #region ican
        public class IcanAdminResult
        {
            public Result result { get; set; }
            public int pageCount { get; set; }
            public List<IcanAdmin> Ican { get; set; }
            public int totalCount { get; set; }
        }
        public class IcanAdmin
        {
            public long ID { get; set; }
            public string title { get; set; }
            public string regDate { get; set; }
            public bool isBlock { get; set; }
            public string userNameFamily { get; set; }
        }
        public class IcanResult
        {
            public Result result { get; set; }
            public fullIcan ican { get; set; }
        }
        public class fullIcan
        {
            public long ID { get; set; }
            public long creatorID { get; set; }
            public string title { get; set; }
            public bool isBlock { get; set; }
            public int viewCount { get; set; }
            public bool haveattachment { get; set; }
            public List<icanContent> contents { get; set; }
        }
        public class icanContent
        {
            public long ID { get; set; }
            public int type { get; set; }
            public string value { get; set; }
            public int? fileSize { get; set; }
            public string fileType { get; set; }
            public long? downloadCount { get; set; }

        }
        public class LatestIcanForListResult
        {
            public Result result { get; set; }
            public List<LatestIcanForList> ican { get; set; }
        }
        public class LatestIcanForList
        {
            public long ID { get; set; }
            public string title { get; set; }
            public string userNameFamily { get; set; }
            public bool haveattachment { get; set; }
            public string summary { get; set; }
            public string image { get; set; }
            public int viewCount { get; set; }

        }
        public class SingleIcanResult
        {
            public Result result { get; set; }
            public singleFullIcan singleican { get; set; }
        }
        public class singleFullIcan
        {
            public long ID { get; set; }
            public long creatorID { get; set; }
            public string title { get; set; }
            public int viewCount { get; set; }
            public string text { get; set; }
            public string image { get; set; }
            public string video { get; set; }

        }
        public class LatestIcanForNotificationResult
        {
            public Result result { get; set; }
            public List<LatestIcanForNotification> ican { get; set; }
        }
        public class LatestIcanForNotification
        {
            public long ID { get; set; }
            public string title { get; set; }

        }
        #endregion

        #region report

        public class reportResult
        {
            public Result result { get; set; }
            public fullreport report { get; set; }
        }
        public class fullreport
        {
            public long ID { get; set; }
            public long? userID { get; set; }
            public long? mUserID { get; set; }
            public string title { get; set; }
            public int like { get; set; }
            public int unlike { get; set; }
            public string publishDate { get; set; }
            public bool isBlock { get; set; }
            public long viewCount { get; set; }
            public List<reportContent> contents { get; set; }
            public List<reportGroupSubGroup> groups { get; set; }
            public List<Tag> tag { get; set; }
        }
        public class reportContent
        {
            public long ID { get; set; }
            public int type { get; set; }
            public string value { get; set; }
            public int? fileSize { get; set; }
            public string fileType { get; set; }
            public long? reportCount { get; set; }
            public minReport minReport { get; set; }
        }
        public class reportGroupSubGroup
        {
            public long ID { get; set; }
            public long reportID { get; set; }
            public long subGroupID { get; set; }
            public long groupID { get; set; }
            public string subGroupTitle { get; set; }
            public string groupTitle { get; set; }
        }

        public class reportAdminResult
        {
            public Result result { get; set; }
            public int pageCount { get; set; }
            public List<reportAdmin> report { get; set; }
        }
        public class reportAdmin
        {
            public long ID { get; set; }
            public string title { get; set; }
            public string regDate { get; set; }
            public string publishDate { get; set; }
            public bool isBlock { get; set; }
        }

        public class minReport
        {
            public long ID { get; set; }
            public string xTitle { get; set; }
            public string yTitle { get; set; }
            public string type { get; set; }
            public List<chart> chart { get; set; }
        }
        public class chart
        {
            public long ID { get; set; }
            public string xTitle { get; set; }
            public double yValue { get; set; }
            public long reportID { get; set; }
        }
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


        public class report
        {
            public long ID { get; set; }
            public string title { get; set; }
            public string regDate { get; set; }
            public string publishDate { get; set; }
        }

        public class ReportContent
        {
            public long ID { get; set; }
            public string xTitle { get; set; }
            public string yTitle { get; set; }
            public string type { get; set; }
            public string chart { get; set; }
        }
        public class singleReportResult
        {
            public Result result { get; set; }
            public singleReport report { get; set; }
        }
        public class singleReport
        {
            public long ID { get; set; }
            public string title { get; set; }
            public string regDate { get; set; }
            public string publishDate { get; set; }
            public List<sreportContent> content { get; set; }
            public List<ReportGroupSubGroup> group { get; set; }
            public List<Tag> tag { get; set; }
        }
        public class ReportGroupSubGroup
        {
            public long ID { get; set; }
            public long reportID { get; set; }
            public long subGroupID { get; set; }
            public long groupID { get; set; }
            public string subGroupTitle { get; set; }
            public string groupTitle { get; set; }
        }
        public class sreportContent
        {
            public long ID { get; set; }
            public int type { get; set; }
            public string value { get; set; }
        }
        public class fullReport
        {
            public long ID { get; set; }
            public long creatorID { get; set; }
            public string title { get; set; }
            public string publishDate { get; set; }
            public string xTitle { get; set; }
            public string yTitle { get; set; }
            public string type { get; set; }
            public int viewCount { get; set; }
            public List<ReportValue> values { get; set; }
            public List<ReportGroupSubGroup> groups { get; set; }
        }
        public class ReportValue
        {
            public long ID { get; set; }
            public double yVlaue { get; set; }
            public string xTitle { get; set; }
        }
        public class ReportResult
        {
            public Result result { get; set; }
            public fullReport report { get; set; }
        }
        #endregion

        #region report

        public class NumberResult
        {
            public int number { get; set; }
            public string date { get; set; }
            public Result result { get; set; }
        }
        public class dashboardUserResult
        {
            public string date { get; set; }
            public Result result { get; set; }
            public List<dashboardUser> user { get; set; }
        }
        public class dashboardUser
        {
            public string info { get; set; }
            public string tell { get; set; }
            public bool isPartner { get; set; }
        }
        #endregion

        #region message
        public class Message
        {
            public long ID { get; set; }
            public string title { get; set; }
            public string text { get; set; }
            public string regDate { get; set; }
            public string attachment { get; set; }
            public long mUserID { get; set; }
            public bool toPartner { get; set; }
            public bool toAll { get; set; }
            public int? fileSize { get; set; }
            public string fileType { get; set; }
            public long? downloadCount { get; set; }
            public bool isBlock { get; set; }
            public string fullname { get; set; }

        }

        public class MessageResult
        {
            public Result result { get; set; }
            public Message Message { get; set; }
            public List<PushTo> pushTo { get; set; }
        }

        public class MessageListResult
        {
            public Result result { get; set; }
            public int pageCount { get; set; }
            public List<Message> message { get; set; }
            public int totalCount { get; set; }
        }
        #endregion

        #region  BestIdeasCompetition

        public class LatestBestIdeaCompetitionForListResult
        {
            public Result result { get; set; }
            public List<LatestBestIdeaCompetitionForList> competition { get; set; }
        }
        public class LatestBestIdeaCompetitionForList
        {
            public long ID { get; set; }
            public string title { get; set; }
            public int status { get; set; }
            public string firstIdeaTitle { get; set; }
            public string image { get; set; }
            public string date { get; set; }

            public List<Tag> tag { get; set; }

        }

        public class BestIdeasCompetitionResult
        {
            public Result result { get; set; }
            public FullBestIdeasCompetition bestIdeasCompetition { get; set; }
        }
        public class FullBestIdeasCompetition
        {
            public long ID { get; set; }
            public long creatorID { get; set; }
            public string title { get; set; }
            public int like { get; set; }
            public int unlike { get; set; }
            public string startDate { get; set; }
            public string endDate { get; set; }
            public string resultVoteDate { get; set; }
            public bool isBlock { get; set; }
            public int viewCount { get; set; }
            public bool isSendNotification { get; set; }
            public string notificationText { get; set; }
            public bool canEdit { get; set; }
            public List<BestIdeasCompetitionContent> contents { get; set; }
            public List<Tag> tag { get; set; }
        }
        public class BestIdeasCompetitionContent
        {
            public long ID { get; set; }
            public int type { get; set; }
            public string value { get; set; }
        }
        public class BestIdeasCompetitionAdminResult
        {
            public Result result { get; set; }
            public int pageCount { get; set; }
            public List<BestIdeasCompetitionAdmin> bestIdeasCompetition { get; set; }
            public int totalCount { get; set; }
        }
        public class BestIdeasCompetitionAdmin
        {
            public long ID { get; set; }
            public string title { get; set; }
            public string status { get; set; }
            public string regDate { get; set; }
            public string startDate { get; set; }
            public string endDate { get; set; }
            public string resultVoteDate { get; set; }
            public bool isBlock { get; set; }
            public bool isSendNotification { get; set; }
        }

        public class BestIdeaCompetition
        {
            public long ID { get; set; }
            public string date { get; set; }
            public string title { get; set; }
            public string remainingTime { get; set; }
            public List<BestIdeasCompetitionContent> contents { get; set; }
            public List<Tag> tag { get; set; }
            public int status { get;  set; }
            public string winnerTitle { get; set; }
            public long winnerID { get; set; }
        }
        public class BestIdeaCompetitionResult
        {
            public Result result { get; set; }
            public BestIdeaCompetition bestIdeasCompetition { get; set; }

        }
            #endregion

        #region Idea

        public class IdeaResult
        {
            public Result result { get; set; }
            public fullIdea idea { get; set; }

        }
        public class fullIdea
        {
            public long ID { get; set; }
            public long mUserID { get; set; }
            public string title { get; set; }
            public string date { get; set; }
            public int status { get; set; }
            public bool userCanVote { get; set; }
            public bool isBlock { get; set; }
            public string fullname { get; set; }
            public List<ideaContent> contents { get; set; }
            public List<Tag> tag { get; set; }
            public long bestIdeaCompetitionsID { get; set; }
        }
        public class ideaContent
        {
            public long ID { get; set; }
            public int type { get; set; }
            public string value { get; set; }
        }

        public class IdeaAdminResult
        {
            public string pageTitle { get; set; }
            public Result result { get; set; }
            public int pageCount { get; set; }
            public List<IdeaAdmin> idea { get; set; }
        }
        public class IdeaAdmin
        {
            public long ID { get; set; }
            public string title { get; set; }
            public string regDate { get; set; }
            public bool isBlock { get; set; }
            public string mUserName { get; set; }
        }


        public class LatestIdeasForListResult
        {
            public Result result { get; set; }
            public List<LatestIdeasForList> idea { get; set; }
        }
        public class LatestIdeasForList
        {
            public long ID { get; set; }
            public string title { get; set; }
            public string fullname { get; set; }
            public string date { get; set; }
            public string image { get; set; }
            public List<Tag> tag { get; set; }

        }

        #endregion

        #region  CreativityCompetition

        public class LatestCreativityCompetitionForListResult
        {
            public Result result { get; set; }
            public List<LatestCreativityCompetitionForList> competition { get; set; }
        }
        public class LatestCreativityCompetitionForList
        {
            public long ID { get; set; }
            public string title { get; set; }
            public int status { get; set; }
            public string firstAnswerTitle { get; set; }
            public string image { get; set; }
            public string date { get; set; }
            public List<Tag> tag { get; set; }

        }

        public class CreativitysCompetitionResult
        {
            public Result result { get; set; }
            public FullCreativityCompetition creativityCompetition { get; set; }
        }
        public class FullCreativityCompetition
        {
            public long ID { get; set; }
            public long userID { get; set; }
            public string title { get; set; }
            public string startDate { get; set; }
            public string endDate { get; set; }
            public bool isBlock { get; set; }
            public bool isDone { get; set; }
            public bool isSendNotification { get; set; }
            public string notificationText { get; set; }
            public bool canEdit { get; set; }
            public List<CreativitysCompetitionContent> contents { get; set; }
            public List<Tag> tag { get; set; }
        }
        public class CreativitysCompetitionContent
        {
            public long ID { get; set; }
            public int type { get; set; }
            public string value { get; set; }
        }
        public class CreativityCompetitionAdminResult
        {
            public Result result { get; set; }
            public int pageCount { get; set; }
            public List<CreativityCompetitionAdmin> creativityCompetition { get; set; }
            public int totalCount { get; set; }
        }
        public class CreativityCompetitionAdmin
        {
            public long ID { get; set; }
            public string title { get; set; }
            public string status { get; set; }
            public string regDate { get; set; }
            public string startDate { get; set; }
            public string endDate { get; set; }
            public bool isBlock { get; set; }
            public bool isSendNotification { get; set; }
        }

        public class CreativityCompetition
        {
            public long ID { get; set; }
            public string date { get; set; }
            public string title { get; set; }
            public string remainingTime { get; set; }
            public List<CreativitysCompetitionContent> contents { get; set; }
            public List<Tag> tag { get; set; }
            public int status { get; set; }
            public long winnerID { get; set; }
            public string winnerTitle { get; set; }
        }
        public class CreativityCompetitionResult
        {
            public Result result { get; set; }
            public CreativityCompetition creativityCompetition { get; set; }

        }
        #endregion

        #region Answer

        public class AnswerResult
        {
            public Result result { get; set; }
            public fullAnswer answer { get; set; }

        }
        public class fullAnswer
        {
            public long ID { get; set; }
            public long mUserID { get; set; }
            public string title { get; set; }
            public string date { get; set; }
            public int status { get; set; }
            public bool userCanAnswer { get; set; }
            public string fullname { get; set; }
            public bool isBlock { get; set; }
            public bool isCorrect { get; set; }
            public bool isWinner { get; set; }
            public List<answerContent> contents { get; set; }
            public List<Tag> tag { get; set; }
            public long creativityCompetitionID { get; set; }
        }
        public class answerContent
        {
            public long ID { get; set; }
            public int type { get; set; }
            public string value { get; set; }
        }

        public class AnswerAdminResult
        {
            public string pageTitle { get; set; }
            public string winnerTitle { get; set; }
            public long winnerID { get; set; }
            public Result result { get; set; }
            public int pageCount { get; set; }
            public List<AnswerAdmin> answer { get; set; }
        }
        public class AnswerAdmin
        {
            public long ID { get; set; }
            public string title { get; set; }
            public string regDate { get; set; }
            public bool isBlock { get; set; }
            public bool isCorrect { get; set; }
            public bool isWinner { get; set; }
            public string mUserName { get; set; }

        }


        public class LatestAnswersForListResult
        {
            public Result result { get; set; }
            public List<LatestAnswersForList> answer { get; set; }
        }
        public class LatestAnswersForList
        {
            public long ID { get; set; }
            public string title { get; set; }
            public string fullname { get; set; }
            public string date { get; set; }
            public string image { get; set; }
            public List<Tag> tag { get; set; }

        }

        #endregion

        #region  MyIran

        public class LatestMyIranForListResult
        {
            public Result result { get; set; }
            public List<LatestMyIranForList> competition { get; set; }
        }
        public class LatestMyIranForList
        {
            public long ID { get; set; }
            public string title { get; set; }
            public int status { get; set; }
            public string firstResponseTitle { get; set; }
            public string image { get; set; }
            public string date { get; set; }
            public List<Tag> tag { get; set; }

        }

        public class myIransResult
        {
            public Result result { get; set; }
            public FullMyIran myIran { get; set; }
        }
        public class FullMyIran
        {
            public long ID { get; set; }
            public long userID { get; set; }
            public string title { get; set; }
            public string startDate { get; set; }
            public string endDate { get; set; }
            public bool isBlock { get; set; }
            public bool isDone { get; set; }
            public bool isSendNotification { get; set; }
            public string notificationText { get; set; }
            public bool canEdit { get; set; }
            public List<myIransContent> contents { get; set; }
            public List<Tag> tag { get; set; }
        }
        public class myIransContent
        {
            public long ID { get; set; }
            public int type { get; set; }
            public string value { get; set; }
        }
        public class MyIranAdminResult
        {
            public Result result { get; set; }
            public int pageCount { get; set; }
            public List<MyIranAdmin> myIran { get; set; }
            public int totalCount { get; set; }
        }
        public class MyIranAdmin
        {
            public long ID { get; set; }
            public string title { get; set; }
            public string status { get; set; }
            public string regDate { get; set; }
            public string startDate { get; set; }
            public string endDate { get; set; }
            public bool isBlock { get; set; }
            public bool isSendNotification { get; set; }
        }

        public class MyIran
        {
            public long ID { get; set; }
            public string date { get; set; }
            public string title { get; set; }
            public string remainingTime { get; set; }
            public List<myIransContent> contents { get; set; }
            public List<Tag> tag { get; set; }
            public int status { get; set; }
            public string winnerTitle { get; set; }
            public long winnerID { get; set; }
        }
        public class MyIranResult
        {
            public Result result { get; set; }
            public MyIran myIran { get; set; }

        }
        #endregion

        #region Response

        public class ResponseResult
        {
            public Result result { get; set; }
            public fullResponse response { get; set; }

        }
        public class fullResponse
        {
            public long ID { get; set; }
            public long mUserID { get; set; }
            public string title { get; set; }
            public string date { get; set; }
            public int status { get; set; }
            public bool userCanResponse { get; set; }
            public string fullname { get; set; }
            public bool isBlock { get; set; }
            public bool isCorrect { get; set; }
            public bool isWinner { get; set; }
            public List<responseContent> contents { get; set; }
            public List<Tag> tag { get; set; }
            public long myIranID { get; set; }
        }
        public class responseContent
        {
            public long ID { get; set; }
            public int type { get; set; }
            public string value { get; set; }
        }

        public class ResponseAdminResult
        {
            public string pageTitle { get; set; }
            public string winnerTitle { get; set; }
            public long winnerID { get; set; }
            public Result result { get; set; }
            public int pageCount { get; set; }
            public List<ResponseAdmin> response { get; set; }
        }
        public class ResponseAdmin
        {
            public long ID { get; set; }
            public string title { get; set; }
            public string regDate { get; set; }
            public bool isBlock { get; set; }
            public bool isCorrect { get; set; }
            public bool isWinner { get; set; }
            public string mUserName { get; set; }
        }


        public class LatestResponsesForListResult
        {
            public Result result { get; set; }
            public List<LatestResponseForList> response { get; set; }
        }
        public class LatestResponseForList
        {
            public long ID { get; set; }
            public string title { get; set; }
            public string fullname { get; set; }
            public string date { get; set; }
            public string image { get; set; }
            public List<Tag> tag { get; set; }

        }

        #endregion
    }

}