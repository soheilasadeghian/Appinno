<%@ Page Title="" Language="C#" MasterPageFile="~/admin/master.Master" AutoEventWireup="true" CodeBehind="eican.aspx.cs" Inherits="AppinnoNew.admin.eican" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="pages/enews/css.css" rel="stylesheet" />
    <link href="component/css/search.css" rel="stylesheet" />
    <link href="../component/simlComboBox/css.css" rel="stylesheet" />
    <script src="../component/simlComboBox/jquery.comboBox.js"></script>
    <link href="../component/tooltip/css/tipped/tipped.css" rel="stylesheet" />
    <script src="../component/tooltip/js/tipped/tipped.js"></script>
    <script src="pages/eican/ajax.js"></script>
    <script src="pages/eican/js.js"></script>
    <link href="../component/datepicker/styles/jquery-ui-1.8.14.css" rel="stylesheet" />
    <script src="../component/datepicker/scripts/jquery.ui.datepicker-cc.all.min.js"></script>
    <title>ویرایش خبر</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="path" runat="server">
    <a href="dashboard.aspx" class="loc history">داشبورد</a>
    <a class="spacer">></a>
    <a class="loc none">توانایی ها</a>
    <a class="spacer">></a>
    <a href="newslist.aspx" class="loc history">لیست توانایی ها</a>
    <a class="spacer">></a>
    <a class="loc none">ویرایش توانایی</a>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="content" runat="server">
    <div class="vertical_min"></div>
    <div class="right-panel">
        <span class="header-news-panel  mci-1b">محتوای توانایی </span>
        <div class="vertical_min"></div>
        <div class="field">
            <span class="span">عنوان اصلی </span>
            <input id="main-title" type="text" name="news-title" value="" placeholder="عنوان اصلی توانایی" maxlength="250" />
            <img class="hint" src="component/images/info.png" alt="راهنما" title="- عنوان اصلی توانایی که در لیست توانایی های نرم افزار نمایش داده خواهد شد.<br/>- طول این پارامتر حداکثر 250 کاراکتر میباشد." data-tipped-options="position: 'left'" />
        </div>
        <hr />
        <div class="vertical_min"></div>
        <div class="tools">
            <a class="button image" title="این دکمه جهت درج تصویر به محتوای توانایی می باشد" data-tipped-options="position: 'top'" onclick="$('#file_image').click();"></a>
            <input class="file_image" type="file" style="display: none" id="file_image" />

            <a class="button video" title="این دکمه جهت درج ویدئو به محتوای توانایی می باشد<br />پسوند قابل قبول ویدیو mp4 می باشد." data-tipped-options="position: 'top'" onclick="$('#file_video').click();"></a>
            <input class="file_video" type="file" style="display: none" id="file_video" />

            <a class="button file" title="این دکمه جهت درج فایل به محتوای توانایی می باشد" data-tipped-options="position: 'top'" onclick="$('#file_file').click();"></a>
            <input class="file_file" type="file" style="display: none" id="file_file" />

            <a id="add-para" class="button text" title="این دکمه جهت درج متن به محتوای توانایی می باشد" data-tipped-options="position: 'top'"></a>
        </div>
        <div class="vertical_min"></div>
        <hr />
        <input class="check" type="checkbox" id="fchk_block" name="2" />
        <label class="label" for="fchk_block">توانایی مسدود شود</label>
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
    <div id="new-para-window" class="new-para-window">
        <div class="background"></div>
        <div class="window">
            <div class="window-header mci-1b"><span>درج پاراگراف جدید</span></div>
            <div class="vertical_min"></div>
            <div class="para-title"><span>عنوان پاراگراف</span><input id="input-title-insert-para-window" type="text" value="" placeholder="عنوان پاراگراف" /></div>
            <textarea id="input-text-insert-para-window" placeholder="متن پاراگراف"></textarea>

            <div class="vertical_min"></div>
            <hr />
            <input id="btn-save-insert-para-window"  type="button" class="save" value="✔" />
            <input id="btn-close-insert-para-window" type="button" class="save " style="font-weight:bolder;font-size:23px" value="ˣ" />
        </div>
    </div>
    <%--contentedit-start--%>
    <div class="clear"></div>
    <div id="edit-para-window" class="new-para-window">
        <div class="background"></div>
        <div class="window">
            <div class="window-header mci-1b"><span>ویرایش پاراگراف </span></div>
            <div class="vertical_min"></div>
            <div class="para-title"><span>عنوان پاراگراف</span><input id="input-title-edit-para-window" type="text" value="" placeholder="عنوان پاراگراف" /></div>
            <textarea id="input-text-edit-para-window" placeholder="متن پاراگراف"></textarea>
            <div class="vertical_min"></div>
            <hr />
            <input id="btn-save-edit-para-window" type="button" class="save" value="✔" />
            <input id="btn-close-edit-para-window"type="button" class="save " style="font-weight:bolder;font-size:23px" value="ˣ" />
        </div>
    </div>
    <%--contentedit-end--%>
    <div id="upload-news-window" class="upload-news-window">
        <div class="background"></div>
        <div class="window">
            <div class="window-header mci-1b"><span>درحال ذخیره سازی توانایی...</span></div>
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
