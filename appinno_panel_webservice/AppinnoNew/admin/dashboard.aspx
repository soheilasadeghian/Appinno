<%@ Page Title="" Language="C#" MasterPageFile="~/admin/master.Master" AutoEventWireup="true" CodeBehind="dashboard.aspx.cs" Inherits="AppinnoNew.admin.dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="pages/dashboard/css.css" rel="stylesheet" />
    <link href="../component/datepicker/styles/jquery-ui-1.8.14.css" rel="stylesheet" />
    <script src="../component/datepicker/scripts/jquery.ui.datepicker-cc.all.min.js"></script>
    <script src="pages/dashboard/ajax.js"></script>
    <script src="pages/dashboard/js.js"></script>
    <link href="../component/tooltip/css/tipped/tipped.css" rel="stylesheet" />
    <script src="../component/tooltip/js/tipped/tipped.js"></script>
    <link href="../component/monthpicker/styles/monthpicker.css" rel="stylesheet" type="text/css" />
    <script src="../component/monthpicker/scripts/monthpicker.min.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="path" runat="server">
    <a href="dashboard.aspx" class="loc none">داشبورد</a>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="content" runat="server">
    <div class="vertical_max"></div>

    <div class="box daily-user-list">
        <div class="title">
            <span>لیست کاربران</span>
        </div>
        <div id="rows">
        </div>
        <input class="input" type="text" name="date" id="daily-date-list" readonly />
    </div>
    <div class="box daily-user">
        <div class="title">
            <span>تعداد کاربران روزانه</span>
        </div>
        <span id="user-count" class="count" style="float:none;">...</span>
        <input class="input" type="text" name="date" id="daily-date" readonly />
    </div>
    <div class="box ungroup">
        <div class="title">
            <span>تعداد کاربران گروه بندی نشده</span>
        </div>
        <span id="ungroup-user-count" class="count" style="float:none;">...</span>
    </div>
    <div class="box partners">
        <div class="title">
            <span>تعداد همکاران</span>
        </div>
        <span id="partner-count" class="count" style="float:none;">...</span>
    </div>


    <div class="divcontainer">
        <div id="newscontainer" class="container" style="margin: 10px 17px 10px 10px; max-width: 640px;">
            <input id="newsmonthpicker" class="inp-pick" type="text" />
             <div class="clear"></div>
        </div>
        <div class="container" style="margin: 3px 13px 10px 10px; max-width: 640px;">
            <span id="newscount" class="count"  ></span>
             <div class="clear"></div>
           <%-- <div  class="recentlylist" style="display:none;padding:8px 0px;margin-top:10px; border-top:1px solid #1398b4; border-bottom:1px solid #1398b4;">
                <a style="color:#035a6c;font-size:0.7em;text-decoration:none;display:block;text-align:right;padding:3px 0px;" href="#1">1. افزایش قیمت ...</a>
                 <a style="color:#035a6c;font-size:0.7em;text-decoration:none;display:block;text-align:right;padding:3px 0px;" href="#2">2. هاشمی مرد ...</a>
                 <a style="color:#035a6c;font-size:0.7em;text-decoration:none;display:block;text-align:right;padding:3px 0px;" href="#3">3. فردا تعطیل است ...</a>
            </div>--%>
        </div>
         <div class="clear"></div>
        <a id="newsbtn" href="news.aspx" style="display: block; bottom: 0px; height: 22px; text-align: center; color: #fff; left: 0px; right: 0px; font-size: 0.85em; border-radius: 0px 0px 3px 3px; background-color: #1398b4; padding-top: 10px; text-decoration: none;">درج خبر</a>
         <div class="clear"></div>
    </div>

     <div class="divcontainer">
        <div id="eventcontainer" class="container" style="margin: 10px 17px 10px 10px; max-width: 640px;">
            <input id="eventmonthpicker" class="inp-pick" type="text" />
             <div class="clear"></div>
        </div>
        <div class="container" style="margin: 3px 13px 10px 10px; max-width: 640px;">
            <span id="eventcount" class="count"  ></span>
             <div class="clear"></div>
    <%--<div class="recentlylist" style="display:none;padding:8px 0px;margin-top:10px; border-top:1px solid #1398b4; border-bottom:1px solid #1398b4;">
                <a style="color:#035a6c;font-size:0.7em;text-decoration:none;display:block;text-align:right;padding:3px 0px;" href="#1">1. افزایش قیمت ...</a>
                 <a style="color:#035a6c;font-size:0.7em;text-decoration:none;display:block;text-align:right;padding:3px 0px;" href="#2">2. هاشمی مرد ...</a>
                 <a style="color:#035a6c;font-size:0.7em;text-decoration:none;display:block;text-align:right;padding:3px 0px;" href="#3">3. فردا تعطیل است ...</a>
            </div>--%>
        </div>
          <div class="clear"></div>
        <a id="eventbtn" href="events.aspx" style=" display: block; bottom: 0px; height: 22px; text-align: center; color: #fff; left: 0px; right: 0px; font-size: 0.85em; border-radius: 0px 0px 3px 3px; background-color: #1398b4; padding-top: 10px; text-decoration: none;">درج رویداد</a>
          <div class="clear"></div>
    </div>

    <div class="divcontainer">
        <div id="iocontainer" class="container" style="margin: 10px 17px 10px 10px; max-width: 640px;">
            <input id="iomonthpicker" class="inp-pick" type="text" />
             <div class="clear"></div>
        </div>
        <div class="container" style="margin: 3px 13px 10px 10px; max-width: 640px;">
            <span id="iocount" class="count"  ></span>
             <div class="clear"></div>
            <%--<div class="recentlylist" style="display:none;padding:8px 0px;margin-top:10px; border-top:1px solid #1398b4; border-bottom:1px solid #1398b4;">
                <a style="color:#035a6c;font-size:0.7em;text-decoration:none;display:block;text-align:right;padding:3px 0px;" href="#1">1. افزایش قیمت ...</a>
                 <a style="color:#035a6c;font-size:0.7em;text-decoration:none;display:block;text-align:right;padding:3px 0px;" href="#2">2. هاشمی مرد ...</a>
                 <a style="color:#035a6c;font-size:0.7em;text-decoration:none;display:block;text-align:right;padding:3px 0px;" href="#3">3. فردا تعطیل است ...</a>
            </div>--%>
        </div>
         <div class="clear"></div>
        <a id="iobtn" href="io.aspx" style=" display: block; bottom: 0px; height: 22px; text-align: center; color: #fff; left: 0px; right: 0px; font-size: 0.85em; border-radius: 0px 0px 3px 3px; background-color: #1398b4; padding-top: 10px; text-decoration: none;">درج سازمان</a>
         <div class="clear"></div>
    </div>

     <div class="divcontainer">
        <div id="pubcontainer" class="container" style="margin: 10px 17px 10px 10px; max-width: 640px;">
            <input id="pubmonthpicker" class="inp-pick" type="text" />
             <div class="clear"></div>
        </div>
        <div class="container" style="margin: 3px 13px 10px 10px; max-width: 640px;">
            <span id="pubcount" class="count"  ></span>
             <div class="clear"></div>
            <%--<div class="recentlylist" style="display:none;padding:8px 0px;margin-top:10px; border-top:1px solid #1398b4; border-bottom:1px solid #1398b4;">
                <a style="color:#035a6c;font-size:0.7em;text-decoration:none;display:block;text-align:right;padding:3px 0px;" href="#1">1. افزایش قیمت ...</a>
                 <a style="color:#035a6c;font-size:0.7em;text-decoration:none;display:block;text-align:right;padding:3px 0px;" href="#2">2. هاشمی مرد ...</a>
                 <a style="color:#035a6c;font-size:0.7em;text-decoration:none;display:block;text-align:right;padding:3px 0px;" href="#3">3. فردا تعطیل است ...</a>
            </div>--%>
        </div>
          <div class="clear"></div>
        <a id="pubbtn" href="publication.aspx" style="display: block; bottom: 0px; height: 22px; text-align: center; color: #fff; left: 0px; right: 0px; font-size: 0.85em; border-radius: 0px 0px 3px 3px; background-color: #1398b4; padding-top: 10px; text-decoration: none;">درج انتشارات</a>
          <div class="clear"></div>
    </div>

     <div class="divcontainer">
        <div id="bestideacontainer" class="container" style="margin: 10px 17px 10px 10px; max-width: 640px;">
            <input id="bestideamonthpicker" class="inp-pick" type="text" />
             <div class="clear"></div>
        </div>
        <div class="container" style="margin: 3px 13px 10px 10px; max-width: 640px;">
            <span id="bestideacount" class="count"  ></span>
             <div class="clear"></div>
            <%--<div class="recentlylist" >
                 <a href="#1">1. افزایش قیمت ...</a>
                 <a href="#2">2. هاشمی مرد ...</a>
                 <a href="#3">3. فردا تعطیل است ...</a>
            </div>--%>
        </div>
          <div class="clear"></div>
        <a id="bestideabtn" href="bestIdeasCompetition.aspx" style=" display: block; bottom: 0px; height: 22px; text-align: center; color: #fff; left: 0px; right: 0px; font-size: 0.85em; border-radius: 0px 0px 3px 3px; background-color: #1398b4; padding-top: 10px; text-decoration: none;">درج مسابقه ایده برتر </a>
          <div class="clear"></div>
    </div>

    <div class="divcontainer">
        <div id="creativitycontainer" class="container" style="margin: 10px 17px 10px 10px; max-width: 640px;">
            <input id="creativitymonthpicker" class="inp-pick" type="text" />
             <div class="clear"></div>
        </div>
        <div class="container" style="margin: 3px 13px 10px 10px; max-width: 640px;">
            <span id="creativitycount" class="count"  ></span>
             <div class="clear"></div>
            <%--<div class="recentlylist" style="display:none;padding:8px 0px;margin-top:10px; border-top:1px solid #1398b4; border-bottom:1px solid #1398b4;">
                <a style="color:#035a6c;font-size:0.7em;text-decoration:none;display:block;text-align:right;padding:3px 0px;" href="#1">1. افزایش قیمت ...</a>
                 <a style="color:#035a6c;font-size:0.7em;text-decoration:none;display:block;text-align:right;padding:3px 0px;" href="#2">2. هاشمی مرد ...</a>
                 <a style="color:#035a6c;font-size:0.7em;text-decoration:none;display:block;text-align:right;padding:3px 0px;" href="#3">3. فردا تعطیل است ...</a>
            </div>--%>
        </div>
         <div class="clear"></div>
        <a id="creativitybtn" href="creativityCompetition.aspx" style="display: block; bottom: 0px; height: 22px; text-align: center; color: #fff; left: 0px; right: 0px; font-size: 0.85em; border-radius: 0px 0px 3px 3px; background-color: #1398b4; padding-top: 10px; text-decoration: none;">درج مسابقه خلاقیت </a>
         <div class="clear"></div>
    </div>

    <div class="divcontainer">
        <div id="myirancontainer" class="container" style="margin: 10px 17px 10px 10px; max-width: 640px;">
            <input id="myiranmonthpicker" class="inp-pick" type="text" />
               <div class="clear"></div>
        </div>
        <div class="container" style="margin: 3px 13px 10px 10px; max-width: 640px;">
            <span id="myirancount" class="count"  ></span>
               <div class="clear"></div>
           <%-- <div class="recentlylist" style="display:none;padding:8px 0px;margin-top:10px; border-top:1px solid #1398b4; border-bottom:1px solid #1398b4;">
                <a style="color:#035a6c;font-size:0.7em;text-decoration:none;display:block;text-align:right;padding:3px 0px;" href="#1">1. افزایش قیمت ...</a>
                 <a style="color:#035a6c;font-size:0.7em;text-decoration:none;display:block;text-align:right;padding:3px 0px;" href="#2">2. هاشمی مرد ...</a>
                 <a style="color:#035a6c;font-size:0.7em;text-decoration:none;display:block;text-align:right;padding:3px 0px;" href="#3">3. فردا تعطیل است ...</a>
            </div>--%>
        </div>
          <div class="clear"></div>
        <a id="myiranbtn" href="myIran.aspx" style=" display: block; bottom: 0px; height: 22px; text-align: center; color: #fff; left: 0px; right: 0px; font-size: 0.85em; border-radius: 0px 0px 3px 3px; background-color: #1398b4; padding-top: 10px; text-decoration: none;">درج مسابقه ایران من </a>

        <div class="clear"></div>
    </div>

   <%-- <a id="eventbtn" href="events.aspx" class="icon event"><span>درج رویداد</span></a>
    <a id="iobtn" href="io.aspx" class="icon io"><span>درج سازمان</span></a>
    <a id="pubbtn" href="publication.aspx" class="icon pub"><span>درج انتشارات</span></a>
    <a id="bestideabtn" href="bestIdeasCompetitionlist.aspx" class="icon pub"><span>مسابقه ایده برتر</span></a>
    <a id="creativitybtn" href="creativityCompetitionlist.aspx" class="icon pub"><span>مسابقه خلاقیت</span></a>
    <a id="myiranbtn" href="myIranlist.aspx" class="icon pub"><span>مسابقه ایران من</span></a>--%>

    <div class="clear"></div>

</asp:Content>


