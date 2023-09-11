<%@ Page Title="" Language="C#" MasterPageFile="~/admin/master.Master" AutoEventWireup="true" CodeBehind="answerlist.aspx.cs" Inherits="AppinnoNew.admin.answerlist" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="pages/newslist/css.css" rel="stylesheet" />
    <link href="component/css/search.css" rel="stylesheet" />
    <link href="../component/simlComboBox/css.css" rel="stylesheet" />
    <script src="../component/simlComboBox/jquery.comboBox.js"></script>
    <script src="pages/answerlist/ajax.js"></script>
    <script src="pages/answerlist/js.js"></script>
    <link href="../component/datepicker/styles/jquery-ui-1.8.14.css" rel="stylesheet" />
    <script src="../component/datepicker/scripts/jquery.ui.datepicker-cc.all.min.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="path" runat="server">
    <a href="dashboard.aspx" class="loc history">داشبورد</a>
    <a class="spacer">></a>
    <a class="loc none"  href="creativityCompetitionlist.aspx">مسابقات خلاقیت</a>
    <a class="spacer">></a>
    <a class="loc none">لیست پاسخ های های مسابقه خلاقیت</a>
    <a class="spacerer">:</a>
    <a class="loc none"><span id="pagetitle"></span></a>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="content" runat="server">
    <div class="vertical_min"></div>
    <div class="right-panel">
        <div class="search-box">
              <span class="search-header mci-1b">جستجو</span>
             <div class="vertical_small"></div>
             <div class="vertical_small"></div>
            <input type="checkbox" class="check" id="chk_winner" />
            <label for="chk_winner">نمایش برندگان</label>
            <div class="vertical_small"></div>
             <hr />

            <div id="other-p">
                <input type="checkbox" class="check" id="chk_date" />
                <label for="chk_date">تاریخ درج پاسخ از : </label>
                <div class="vertical_small"></div>
                <input type="text" class="search-input" name="name" id="date" readonly />
                <div class="vertical_min"></div>

                <input type="checkbox" class="check" id="chk_dateto" />
                <label for="chk_dateto">تاریخ درج پاسخ تا : </label>
                <div class="vertical_small"></div>
                <input type="text" class="search-input" name="name" id="dateto" readonly />
                <div class="vertical_min"></div>
                <hr />

                <label class="inner-caption">نمایش پاسخ ها  : </label>
                <div class="vertical_small"></div>
                <div class="combo">
                    <div id="itemstate" class=" select">
                        <span class="option" value="all">همه</span>
                        <span class="option" value="block">مسدود شده</span>
                        <span class="option" value="unblock">مسدود نشده</span>
                         <span class="option" value="correct">پاسخ صحیح</span>
                        <span class="option" value="incorrect">پاسخ غلط</span>
                    </div>
                </div>
                <div class="vertical_small"></div>

                <hr />
                <div class="vertical_small"></div>
                <input id="fillter-input" type="text" class="search-input" placeholder="جستجو در پاسخ ها..." value="" />
                <div class="vertical_small"></div>
                <hr />

                <label class="inner-caption">مرتب سازی براساس : </label>
                <div class="vertical_small"></div>
                <div class="combo">
                    <div id="sortby" class=" select">
                        <span class="option" value="reg">تاریخ درج</span>
                    </div>
                </div>
                <div class="vertical_small"></div>
                <div class="combo">
                    <div id="order" class=" select"><span class="option" value="d">نزولي</span><span class="option" value="a">صعودي</span>
                         
                         
                    </div>
                </div>

            </div>
            <div class="vertical_small"></div>
            <input id="search-btn" type="button" class="search-button" value="جستجو" />
        </div>
    </div>

    <div class="left-panel">
        <div class="group-list">
            <div class="header-group mci-1b">
                <a class="loc none" id="winnerID" style="float: left;text-decoration: none;color: white;"><span id="winnertitle"></span></a>
                <span id="winnerspan" style="float:left;display:none;">برنده مسابقه: &nbsp&nbsp</span>
                <span id="lt" >لیست پاسخ ها</span>
            </div>
            <div class="clear"></div>
            <div class="item-header">
                <div class="tools"><span>ابزار</span></div>
                <span class="title">عنوان</span><span class="mUserName">شرکت کننده</span><span class="date">تاریخ درج</span>
            </div>
            <div id="list">
                <span>درحال دریافت اطلاعات...</span>
            </div>

            <div class="insert">
                <div class="pager" id="pager">
                    <div class="clear"></div>
                </div>
            </div>
        </div>
    </div>
    <div class="clear"></div>

</asp:Content>
