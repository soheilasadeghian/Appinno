<%@ Page Title="" Language="C#" MasterPageFile="~/admin/master.Master" AutoEventWireup="true" CodeBehind="eidea.aspx.cs" Inherits="AppinnoNew.admin.eidea" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="pages/eidea/css.css" rel="stylesheet" />
    <link href="component/css/search.css" rel="stylesheet" />
    <link href="../component/simlComboBox/css.css" rel="stylesheet" />
    <script src="../component/simlComboBox/jquery.comboBox.js"></script>
    <link href="../component/tooltip/css/tipped/tipped.css" rel="stylesheet" />
    <script src="../component/tooltip/js/tipped/tipped.js"></script>
    <script src="pages/eidea/ajax.js"></script>
    <script src="pages/eidea/js.js"></script>
    <link href="../component/datepicker/styles/jquery-ui-1.8.14.css" rel="stylesheet" />
    <script src="../component/datepicker/scripts/jquery.ui.datepicker-cc.all.min.js"></script>
    <title>ویرایش ایده</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="path" runat="server">
    <a href="dashboard.aspx" class="loc history">داشبورد</a>
    <a class="spacer">></a>
    <a class="loc none" href="bestIdeasCompetitionlist.aspx">مسابقات ایده برتر</a>
    <a class="spacer">></a>
    <a class="loc none" id="pagetitle">لیست ایده ها</a>
    <a class="spacer">></a>
    <a class="loc none">ویرایش ایده</a>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="content" runat="server">
    <div class="vertical_min"></div>
    <div class="right-panel">
        <span class="header-news-panel  mci-1b"  id="main-fullname">محتوای ایده</span>
        <div class="vertical_min"></div>
        <div class="field">
            <span class="span">عنوان اصلی ایده</span>
            <input id="main-title" type="text" name="idea-title" value="" placeholder="عنوان اصلی ایده" maxlength="250" readonly />
            <img class="hint" src="component/images/info.png" alt="راهنما" title="- عنوان اصلی ایده که در لیست ایده های نرم افزار نمایش داده خواهد شد.<br/>- طول این پارامتر حداکثر 250 کاراکتر میباشد." data-tipped-options="position: 'left'" />
        </div>
        <div class="vertical_min"></div>
        <div class="vertical_small"></div>
        <hr />

        <label class="label" for="fchk_hasTag">تگ ها:</label>
        <div class="vertical_small"></div>

        <div id="tag-panel" /*border: 1px solid #cbcaca; padding: 5px 5px 5px 0px; */">

           
        </div>
         <div class="clear"></div>
        <hr />
        <input class="check" type="checkbox" id="fchk_block" name="2" />
        <label class="label" for="fchk_block">ایده مسدود شود</label>
        <hr />
        <input id="btn-save-all" type="button" class="save-all" value="ذخیره سازی همه ی اطلاعات درج شده" />
    </div>
    <div class="left-panel">
        <span class="header-news-panel mci-1b">پیش نمایش محتوا</span>
        <div class="vertical_min"></div>

        <div id="items">
        </div>
        <div id="uploaded" style="display: none" />
    </div>
    <div class="clear"></div>
  
    <div id="upload-news-window" class="upload-news-window">
        <div class="background"></div>
        <div class="window">
            <div class="window-header mci-1b"><span>درحال ذخیره سازی ایده...</span></div>
            <div class="vertical_min"></div>
            <div class="upload">
                <div class="clear"></div>
                <div id="progress" class="progress">
                    <div id="bar" class="bar"></div>
                </div>

            </div>
            <div class="vertical_min"></div>
        </div>

    </div>
</asp:Content>
