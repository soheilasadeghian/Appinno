<%@ Page Title="" Language="C#" MasterPageFile="~/admin/master.Master" AutoEventWireup="true" CodeBehind="cpublication.aspx.cs" Inherits="AppinnoNew.admin.cpublication" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="pages/cnews/css.css" rel="stylesheet" />
    <link href="component/css/search.css" rel="stylesheet" />
    <link href="../component/simlComboBox/css.css" rel="stylesheet" />
    <script src="../component/simlComboBox/jquery.comboBox.js"></script>
    <script src="pages/cpublication/ajax.js"></script>
    <script src="pages/cpublication/js.js"></script>
    <link href="../component/datepicker/styles/jquery-ui-1.8.14.css" rel="stylesheet" />
    <script src="../component/datepicker/scripts/jquery.ui.datepicker-cc.all.min.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="path" runat="server">
    <a href="dashboard.aspx" class="loc history">داشبورد</a>
    <a class="spacer">></a>
    <a class="loc none">انتشارات</a>
    <a class="spacer">></a>
    <a class="loc none">لیست نظرات</a>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="content" runat="server">
    <div class="vertical_min"></div>
    <div class="right-panel">
        <div class="search-box">
            <span class="search-header mci-1b">جستجو</span>
            <input type="checkbox" class="check" id="chk_date" />
            <label for="chk_date">تاریخ ثبت نام از : </label>
            <div class="vertical_small"></div>
            <input type="text" class="search-input" name="name" id="date" readonly />
            <div class="vertical_min"></div>

            <input type="checkbox" class="check" id="chk_dateto" />
            <label for="chk_dateto">تاریخ ثبت نام تا : </label>
            <div class="vertical_small"></div>
            <input type="text" class="search-input" name="name" id="dateto" readonly />
            <div class="vertical_min"></div>

            <label class="inner-caption">نمایش نظرها : </label>
            <div class="vertical_small"></div>
            <div class="combo">
                <div id="userstate" class=" select">
                    
                    <span class="option" value="block">مسدود شده</span>
                    <span class="option" value="unblock">مسدود نشده</span>
                    <span class="option" value="all">همه</span>
                </div>
            </div>
            <div class="vertical_small"></div>

            <hr />
            <div class="vertical_small"></div>
            <input id="fillter-input" type="text" class="search-input" placeholder="جستجو در نظرات..." value="" />
            <div class="vertical_small"></div>
            <hr />

           
            <label class="inner-caption">مرتب سازی براساس : </label>
            <div class="vertical_small"></div>
            <div class="combo">
                <div id="sortby" class=" select">
                     <span class="option" value="regdate">تاریخ ثبت</span>
                    <span class="option" value="name">نام</span>
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
            <span class="header-group mci-1b"><span id="lt" >لیست نظرات</span> <span id="total-count"></span></span>
            <a id="deleteall-btn" class="delete-all" title="حذف همه ی نظرات"></a>
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
