<%@ Page Title="" Language="C#" MasterPageFile="~/admin/master.Master" AutoEventWireup="true" CodeBehind="eanswer.aspx.cs" Inherits="AppinnoNew.admin.eanswer" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="pages/eidea/css.css" rel="stylesheet" />
    <link href="component/css/search.css" rel="stylesheet" />
    <link href="../component/simlComboBox/css.css" rel="stylesheet" />
    <script src="../component/simlComboBox/jquery.comboBox.js"></script>
    <link href="../component/tooltip/css/tipped/tipped.css" rel="stylesheet" />
    <script src="../component/tooltip/js/tipped/tipped.js"></script>
    <script src="pages/eanswer/ajax.js"></script>
    <script src="pages/eanswer/js.js"></script>
    <link href="../component/datepicker/styles/jquery-ui-1.8.14.css" rel="stylesheet" />
    <script src="../component/datepicker/scripts/jquery.ui.datepicker-cc.all.min.js"></script>
    <title>ویرایش پاسخ</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="path" runat="server">
    <a href="dashboard.aspx" class="loc history">داشبورد</a>
    <a class="spacer">></a>
    <a class="loc none" href="creativityCompetitionlist.aspx" >لیست مسابقات خلاقیت</a>
    <a class="spacer">></a>
    <a class="loc none" id="pagetitle">لیست پاسخ ها</a>
    <a class="spacer">></a>
    <a class="loc none">ویرایش پاسخ</a>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="content" runat="server">
    <div class="vertical_min"></div>
    <div class="right-panel">
        <span class="header-news-panel  mci-1b"  id="main-fullname">محتوای پاسخ</span>
        <div class="vertical_min"></div>
        <div class="field">
            <span class="span">عنوان اصلی پاسخ</span>
            <input id="main-title" type="text" name="answer-title" value="" placeholder="عنوان اصلی پاسخ" maxlength="250" readonly />
            <img class="hint" src="component/images/info.png" alt="راهنما" title="- عنوان اصلی پاسخ که در لیست پاسخ های نرم افزار نمایش داده خواهد شد.<br/>- طول این پارامتر حداکثر 250 کاراکتر میباشد." data-tipped-options="position: 'left'" />
        </div>
        <div class="vertical_min"></div>
         <div class="clear"></div>
        <hr />
         <input class="check" type="checkbox" id="fchk_iscorrect" name="2" />
        <label class="label" for="fchk_iscorrect">پاسخ صحیح است</label>
        <hr />
         <input class="check" type="checkbox" id="fchk_iswinner" name="2" />
        <label class="label" for="fchk_iswinner">برنده مسابقه</label>
        <hr />
        <input class="check" type="checkbox" id="fchk_block" name="2" />
        <label class="label" for="fchk_block">پاسخ مسدود شود</label>
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
            <div class="window-header mci-1b"><span>درحال ذخیره سازی پاسخ...</span></div>
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
