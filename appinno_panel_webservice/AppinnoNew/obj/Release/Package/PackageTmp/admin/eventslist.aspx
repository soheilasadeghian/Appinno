<%@ Page Title="" Language="C#" MasterPageFile="~/admin/master.Master" AutoEventWireup="true" CodeBehind="eventslist.aspx.cs" Inherits="AppinnoNew.admin.eventslist" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="pages/eventslist/css.css" rel="stylesheet" />
    <link href="component/css/search.css" rel="stylesheet" />
    <link href="../component/simlComboBox/css.css" rel="stylesheet" />
    <script src="../component/simlComboBox/jquery.comboBox.js"></script>
    <script src="pages/eventslist/ajax.js"></script>
    <script src="pages/eventslist/js.js"></script>
    <link href="../component/datepicker/styles/jquery-ui-1.8.14.css" rel="stylesheet" />
    <script src="../component/datepicker/scripts/jquery.ui.datepicker-cc.all.min.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="path" runat="server">
    <a href="dashboard.aspx" class="loc history">داشبورد</a>
    <a class="spacer">></a>
    <a class="loc none">رویدادها</a>
    <a class="spacer">></a>
    <a class="loc none">لیست رویدادها</a>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="content" runat="server">
    <div class="vertical_min"></div>
    <div class="right-panel">
        <div class="search-box">
            <span class="search-header mci-1b">جستجو</span>
            <div class="vertical_small"></div>
            <input class="check" type="radio" id="fchk_nogrp" checked="checked" name="2" />
            <label for="fchk_nogrp">رویدادهای عمومی</label>
            <div class="vertical_min"></div>

            <input class="check" type="radio" id="fchk_allitem" name="2" />
            <label for="fchk_allitem">همه رویدادهای گروه بندی شده</label>
            <div class="vertical_min"></div>

            <input class="check" type="radio" id="fchk_toGroup" name="2" />
            <label for="fchk_toGroup">رویدادهای قرارگرفته در گروه زیر:</label>
            <div class="vertical_small"></div>

            <div id="fgroup_panel" style="display: none">
                <div class="combo">
                    <div id="group" class=" select">
                    </div>
                </div>
                <div class="vertical_small"></div>
                <div class="combo">
                    <div id="subgroup" class=" select">
                    </div>
                </div>
                <div class="vertical_min"></div>
            </div>
            <hr />
            <input type="checkbox" class="check" id="chk_date" />
            <label for="chk_date">تاریخ درج رویداد از : </label>
            <input type="text" class="search-input" name="name" id="date" readonly />
            <div class="vertical_min"></div>

            <input type="checkbox" class="check" id="chk_dateto" />
            <label for="chk_dateto">تاریخ درج رویداد تا : </label>
            <input type="text" class="search-input" name="name" id="dateto" readonly />
            <div class="vertical_min"></div>
            <hr />

            <input type="checkbox" class="check" id="chk_fromdatefrom" />
            <label for="chk_fromdatefrom">تاریخ شروع رویداد از : </label>
            <input type="text" class="search-input" name="name" id="fromdatefrom" readonly />
            <div class="vertical_min"></div>

            <input type="checkbox" class="check" id="chk_fromdateto" />
            <label for="chk_fromdateto">تاریخ شروع رویداد تا : </label>
            <input type="text" class="search-input" name="name" id="fromdateto" readonly />
            <div class="vertical_min"></div>
            <hr />
            <input type="checkbox" class="check" id="chk_todatefrom" />
            <label for="chk_todatefrom">تاریخ پایان رویداد از : </label>
            <input type="text" class="search-input" name="name" id="todatefrom" readonly />
            <div class="vertical_min"></div>

            <input type="checkbox" class="check" id="chk_todateto" />
            <label for="chk_todateto">تاریخ پایان رویداد تا : </label>
            <input type="text" class="search-input" name="name" id="todateto" readonly />
            <div class="vertical_min"></div>

            <label class="inner-caption">نمایش رویداد : </label>
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
            <div class="vertical_small"></div>
            <input id="fillter-input" type="text" class="search-input" placeholder="جستجو در رویدادها..." value="" />
            <div class="vertical_small"></div>
            <hr />

            
            <label class="inner-caption">مرتب سازی براساس : </label>
            <div class="vertical_small"></div>
            <div class="combo">
                <div id="sortby" class=" select">
                     <span class="option" value="reg">تاریخ درج</span>
                    <span class="option" value="from">تاریخ شروع</span>
                    <span class="option" value="to">تاریخ پایان</span>
                   
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
            <span class="header-group mci-1b"><span id="lt">لیست رویدادهای عمومی</span> <span id="total-count"></span></span>
            <a id="deleteall-btn" class="delete-all" title="حذف همه ی اخبار"></a>
            <div class="clear"></div>
            <div class="item-header">
                <div class="tools"><span>ابزار</span></div>
                <span class="ptitle">عنوان</span><span class="text">تاریخ شروع</span><span class="date">تاریخ پایان</span><span class="date">تاریخ درج</span>
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
