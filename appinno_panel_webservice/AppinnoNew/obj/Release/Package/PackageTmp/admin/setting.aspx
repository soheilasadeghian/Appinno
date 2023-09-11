<%@ Page Title="" Language="C#" MasterPageFile="~/admin/master.Master" AutoEventWireup="true" CodeBehind="setting.aspx.cs" Inherits="AppinnoNew.admin.setting" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="pages/setting/css.css" rel="stylesheet" />
    <link href="component/css/search.css" rel="stylesheet" />
    <link href="../component/simlComboBox/css.css" rel="stylesheet" />
    <script src="../component/simlComboBox/jquery.comboBox.js"></script>
    <link href="../component/tooltip/css/tipped/tipped.css" rel="stylesheet" />
    <script src="../component/tooltip/js/tipped/tipped.js"></script>
    <script src="pages/setting/ajax.js"></script>
    <script src="pages/setting/js.js"></script>
    <link href="../component/datepicker/styles/jquery-ui-1.8.14.css" rel="stylesheet" />
    <script src="../component/datepicker/scripts/jquery.ui.datepicker-cc.all.min.js"></script>
    <title>تنظیمات</title>
</asp:Content> 
<asp:Content ID="Content2" ContentPlaceHolderID="path" runat="server">
    <a href="dashboard.aspx" class="loc history">داشبورد</a>
    <a class="spacer">></a>
    <a class="loc none">تنظیمات</a>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="content" runat="server">
    <div class="vertical_min"></div>
    <div class="right-panel">
        <span class="header-news-panel  mci-1b">تنظیمات اپلیکیشن</span>
        <div class="vertical_min"></div>
        <div class="field">
            <span class="span">سخن روز</span>
            <input id="daily-title" type="text" name="daily-title" value="" placeholder="سخن روز" maxlength="250" />
            <img class="hint" src="component/images/info.png" alt="راهنما" title="- این متن به عنوان سخن روز بعد از صفحه ی اسپلش در اپلیکیشن نمایش داده می شود.<br/>- طول این پارامتر حداکثر 400 کاراکتر میباشد." data-tipped-options="position: 'left'" />
        </div>
        <div class="vertical_min"></div>
        <div class="field" style="position:relative;">
            <span class="span">عکس</span>
            <div id="adminimage" style="display: inline-block;"></div>
            <img class="hint" src="component/images/info.png" alt="راهنما"  title="این عکس بعد از صفحه ی اسپلش در اپلیکیشن نمایش داده می شود." data-tipped-options="position: 'left'" style="vertical-align: top;" />
            <a class="button image1" title="این دکمه جهت درج تصویر به محتوای خبر می باشد" data-tipped-options="position: 'top'" onclick="$('#file_image').click();" style="position: absolute;left: 40px;bottom: 20px;"></a>
            <input class="file_image" type="file" style="display: none" id="file_image" />
        </div>
        <div class="vertical_min"></div>
        
        <div class="field">
            <span class="span">قوانین درج محتوا</span>
            <textarea id="policy" placeholder="متن قوانین"></textarea>
            <img class="hint" src="component/images/info.png" alt="راهنما" title="این متن بیانگر قوانین کلی جهت درج محتوا در اپلیکیشن می باشد." data-tipped-options="position: 'left'" style="vertical-align: top;" />
        </div>
        <div class="vertical_small"></div>
        <hr />
        <input id="btn-save-all" type="button" class="save-all" value="ذخیره سازی همه ی اطلاعات درج شده" />
    </div>
    <div class="left-panel">
        <span class="header-news-panel mci-1b">تنظیمات سایت</span>
        <div class="vertical_min"></div>
        <div id="items">
        </div>

    </div>
    <div id="upload-news-window" class="upload-news-window">
        <div class="background"></div>
        <div class="window">
            <div class="window-header mci-1b"><span>درحال ذخیره سازی تنظیمات...</span></div>
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
    <div class="clear"></div>
</asp:Content>
