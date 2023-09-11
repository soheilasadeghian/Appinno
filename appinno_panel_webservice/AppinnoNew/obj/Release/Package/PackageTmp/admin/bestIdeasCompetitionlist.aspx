<%@ Page Title="" Language="C#" MasterPageFile="~/admin/master.Master" AutoEventWireup="true" CodeBehind="bestIdeasCompetitionlist.aspx.cs" Inherits="AppinnoNew.admin.bestIdeasCompetitionlist" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="pages/newslist/css.css" rel="stylesheet" />
    <link href="component/css/search.css" rel="stylesheet" />
    <link href="../component/simlComboBox/css.css" rel="stylesheet" />
    <script src="../component/simlComboBox/jquery.comboBox.js"></script>
    <script src="pages/bestIdeasCompetitionlist/ajax.js"></script>
    <script src="pages/bestIdeasCompetitionlist/js.js"></script>
    <link href="../component/datepicker/styles/jquery-ui-1.8.14.css" rel="stylesheet" />
    <script src="../component/datepicker/scripts/jquery.ui.datepicker-cc.all.min.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="path" runat="server">
    <a href="dashboard.aspx" class="loc history">داشبورد</a>
    <a class="spacer">></a>
    <a class="loc none">مسابقات</a>
    <a class="spacer">></a>
    <a class="loc none" href="bestIdeasCompetitionlist.aspx"> لیست مسابقات ایده برتر</a>
    <a class="loc none" href="bestIdeasCompetition.aspx" style="float: left;background-color: #b0b0b0;color: #0c3a4a;">درج مسابقه  جدید</a>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="content" runat="server">
    <div class="vertical_min"></div>
    <div class="right-panel">
        <div class="search-box">
            <span class="search-header mci-1b">جستجو</span>
            <input type="checkbox" class="check" id="chk_date" />
            <label for="chk_date">تاریخ درج مسابقه از : </label>
            <div class="vertical_small"></div>
            <input type="text" class="search-input" name="name" id="date" readonly />
            <div class="vertical_min"></div>
            <input type="checkbox" class="check" id="chk_dateto" />
            <label for="chk_dateto">تاریخ درج مسابقه تا : </label>
            <div class="vertical_small"></div>
            <input type="text" class="search-input" name="name" id="dateto" readonly />
            <div class="vertical_min"></div>

            <hr />
            <div class="vertical_min"></div>
            <input type="checkbox" class="check" id="chk_startDate" />
            <label for="chk_startdate">تاریخ شروع مسابقه از : </label>
            <div class="vertical_small"></div>
            <input type="text" class="search-input" name="name" id="startDate" readonly />
            <div class="vertical_min"></div>
            <input type="checkbox" class="check" id="chk_startDateto" />
            <label for="chk_startdateto">تاریخ شروع مسابقه تا : </label>
            <div class="vertical_small"></div>
            <input type="text" class="search-input" name="name" id="startDateto" readonly />
            <div class="vertical_min"></div>

            <hr />
            <div class="vertical_min"></div>
            <input type="checkbox" class="check" id="chk_endDate" />
            <label for="chk_enddate">تاریخ پایان مسابقه از : </label>
            <div class="vertical_small"></div>
            <input type="text" class="search-input" name="name" id="endDate" readonly />
            <div class="vertical_min"></div>
            <input type="checkbox" class="check" id="chk_endDateto" />
            <label for="chk_enddateto">تاریخ پایان مسابقه تا : </label>
            <div class="vertical_small"></div>
            <input type="text" class="search-input" name="name" id="endDateto" readonly />
            <div class="vertical_min"></div>

            <hr />
            <div class="vertical_min"></div>
            <input type="checkbox" class="check" id="chk_resultVoteDate" />
            <label for="chk_resultVotedate">تاریخ اعلام نتایج مسابقه از : </label>
            <div class="vertical_small"></div>
            <input type="text" class="search-input" name="name" id="resultVoteDate" readonly />
            <div class="vertical_min"></div>
            <input type="checkbox" class="check" id="chk_resultVoteDateto" />
            <label for="chk_resultVotedateto">تاریخ اعلام نتایج مسابقه تا : </label>
            <div class="vertical_small"></div>
            <input type="text" class="search-input" name="name" id="resultVoteDateto" readonly />
            <div class="vertical_min"></div>


            <label class="inner-caption">نمایش مسابقات : </label>
            <div class="vertical_small"></div>
            <div class="combo">
                <div id="itemstate" class=" select">
                    <span class="option" value="all">همه</span>
                    <span class="option" value="block">مسدود شده</span>
                    <span class="option" value="unblock">مسدود نشده</span>
                </div>
            </div>
            <div class="vertical_small"></div>
            <hr />

            <label class="inner-caption">وضعیت مسابقات : </label>
            <div class="vertical_small"></div>
            <div class="combo">
                <div id="itemstatus" class=" select">
                    <span class="option" value="همه">همه</span>
                    <span class="option" value="ثبت شده">ثبت شده</span>
                    <span class="option" value="ارسال ایده ها">ارسال ایده ها</span>
                    <span class="option" value="رأی گیری">رأی گیری</span>
                    <span class="option" value="پایان یافته">پایان یافته</span>
                </div>
            </div>
            <div class="vertical_small"></div>

            <hr />
            <div class="vertical_small"></div>
            <input id="fillter-input" type="text" class="search-input" placeholder="جستجو در مسابقات..." value="" />
            <div class="vertical_small"></div>
            
            <hr />
            <label class="inner-caption">مرتب سازی براساس : </label>
            <div class="vertical_small"></div>
            <div class="combo">
                <div id="sortby" class=" select">
                    <span class="option" value="reg">تاریخ درج</span>
                    <span class="option" value="startDate">تاریخ شروع مسابقه</span>
                    <span class="option" value="endDate">تاریخ پایان مسابقه</span>
                    <span class="option" value="resultVoteDate">تاریخ اعلام نتایج مسابقه</span>
                </div>
            </div>
            <div class="vertical_small"></div>
            <div class="combo">
                <div id="order" class=" select"><span class="option" value="d">نزولي</span><span class="option" value="a">صعودي</span>
                     
                     
                </div>
            </div>
            <div class="vertical_small"></div>
            <input id="search-btn" type="button" class="search-button" value="جستجو" />
        </div>
    </div>
    <div class="left-panel">
        <div class="group-list">
            <span class="header-group mci-1b"><span id="lt">لیست مسابقات</span> <span id="total-count"></span></span>
            <%--<a id="deleteall-btn" class="delete-all" title="حذف همه ی مسابقات"></a>--%>
            <div class="clear"></div>
            <div class="item-header">
                <div class="tools"><span>ابزار</span></div>
                <span class="ptitle">عنوان</span>
                <span class="text">وضعیت</span>
                <span class="date">تاریخ درج</span>
            </div>
            <div id="list">
                <span>درحال دریافت اطلاعات...</span>
            </div>

            <div class="insert">
                <div class="pager" id="pager">
                    <div class="clear"></div>
                </div>
                <div style="float: left;">
                    <a class="loc none" href="bestIdeasCompetition.aspx" style="float: left;background-color: #b0b0b0;color: #0c3a4a;padding: 10px;text-decoration: none;font-size: 0.8em;">درج مسابقه  جدید</a>
                </div>
            </div>
        </div>
    </div>
    <div class="clear"></div>
    <div id="new-para-window" class="new-para-window">
        <div class="background"></div>
        <div class="window">
            <div class="window-header mci-1b"><span>درج متن پیام اطلاع رسانی شروع مسابقه</span></div>
            <div class="vertical_min"></div>
            <%--<div class="para-title"><span>عنوان پاراگراف</span><input id="input-title-insert-para-window" type="text" value="" placeholder="عنوان پاراگراف" /></div>--%>
            <textarea id="input-text-insert-para-window" placeholder="متن پیام"></textarea>

            <div class="vertical_min"></div>
            <hr />
            <input id="btn-save-insert-para-window" type="button" class="save" value="✔" />
            <input id="btn-close-insert-para-window"type="button" class="save " style="font-weight:bolder;font-size:23px" value="ˣ" />
        </div>

    </div>
    <div class="clear"></div>
</asp:Content>
