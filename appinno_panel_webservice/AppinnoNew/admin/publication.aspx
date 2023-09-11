﻿<%@ Page Title="" Language="C#" MasterPageFile="~/admin/master.Master" AutoEventWireup="true" CodeBehind="publication.aspx.cs" Inherits="AppinnoNew.admin.publication" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="pages/publication/css.css" rel="stylesheet" />
    <link href="component/css/search.css" rel="stylesheet" />
    <link href="../component/simlComboBox/css.css" rel="stylesheet" />
    <script src="../component/simlComboBox/jquery.comboBox.js"></script>
    <link href="../component/tooltip/css/tipped/tipped.css" rel="stylesheet" />
    <script src="../component/tooltip/js/tipped/tipped.js"></script>
    <script src="pages/publication/ajax.js"></script>
    <script src="pages/publication/js.js"></script>
    <link href="../component/datepicker/styles/jquery-ui-1.8.14.css" rel="stylesheet" />
    <script src="../component/datepicker/scripts/jquery.ui.datepicker-cc.all.min.js"></script>
    <title>درج انتشارات جدید</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="path" runat="server">
    <a href="dashboard.aspx" class="loc history">داشبورد</a>
    <a class="spacer">></a>
    <a class="loc none">انتشارات</a>
    <a class="spacer">></a>
    <a class="loc none">درج انتشارات جدید</a>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="content" runat="server">
    <div class="vertical_min"></div>
    <div class="right-panel">
        <span class="header-news-panel  mci-1b">محتوای انتشارات</span>
        <div class="vertical_min"></div>
        <div class="field">
            <span class="span">نام انتشارات</span>
            <input id="main-title" type="text" name="news-title" value="" placeholder="نام انتشارات" maxlength="250" />
            <img class="hint" src="component/images/info.png" alt="راهنما" title="- نام انتشارات که در لیست انتشارات نرم افزار نمایش داده خواهد شد.<br/>- طول این پارامتر حداکثر 250 کاراکتر میباشد." data-tipped-options="position: 'left'" />
        </div>
        <div class="vertical_small"></div>
        <div class="field">
            <span class="span">تاریخ انتشار</span>
            <input id="date1" type="text" name="news-pubdate" value="" placeholder="تاریخ انتشار" maxlength="10" />
            <img class="hint" src="component/images/info.png" alt="راهنما" title="انتشارات درج شده از این تاریخ به بعد در نرم افزار نمایش داده می شود" data-tipped-options="position: 'left'" />
        </div>
         <div class="vertical_small"></div>
        <div class="field">
            <span class="span">تگ</span>
            <input id="tagstring" type="text" name="news-pubdate" value="" placeholder="تگ ها"  />
            <img class="hint" src="component/images/info.png" alt="راهنما" title="درصورت تمایل می توانید برای انتشارات مورد نظر تگ وارد نمایید.مثال:#همراه اول #اپی نو" data-tipped-options="position: 'left'" />
        </div>
        <div class="vertical_min"></div>
        <hr />
        <div class="vertical_min"></div>
        <div class="tools">
            <a class="button image" title="این دکمه جهت درج تصویر به محتوای انتشارات می باشد" data-tipped-options="position: 'top'" onclick="$('#file_image').click();"></a>
            <input class="file_image" type="file" style="display: none" id="file_image" />

            <a class="button video" title="این دکمه جهت درج ویدئو به محتوای انتشارات می باشد<br />پسوند قابل قبول ویدیو mp4 می باشد." data-tipped-options="position: 'top'" onclick="$('#file_video').click();"></a>
            <input class="file_video" type="file" style="display: none" id="file_video" />

            <a id="add-para" class="button text" title="این دکمه جهت درج متن به محتوای انتشارات می باشد" data-tipped-options="position: 'top'"></a>
        </div>
        <div class="vertical_min"></div>
         <hr />
        <input class="check" type="radio" id="fchk_noTag" name="3" checked="checked" />
        <label class="label" for="fchk_noTag">تگ ندارد</label>
        <div class="vertical_min"></div>

        <input class="check" type="radio" id="fchk_hasTag" name="3" />
        <label class="label" for="fchk_hasTag">انتخاب تگ:</label>
        <div class="vertical_small"></div>

        <div id="tag-panel" style="display: none; /*border: 1px solid #cbcaca; padding: 5px 5px 5px 0px; */">
            <div class="sub-right">
                <div class="field">
                    <span class="label span">تگ</span>

                    <div class="combo">
                        <div id="tag" class=" select">
                        </div>
                    </div>
                </div>
                
                <div class="vertical_max">
                </div>
                <input id="btn-save-add-tag" type="button" class="add-group" value="✔" />
            </div>
            <div id="sub-left-tag" class="sub-left">
            </div>
            <div class="clear"></div>
        </div>
        <hr />
        <input class="check" type="radio" id="fchk_forall" name="2" checked="checked" />
        <label class="label" for="fchk_forall">انتشار انتشارات برای همه</label>
        <div class="vertical_min"></div>

        <input class="check" type="radio" id="fchk_toGroup" name="2" />
        <label class="label" for="fchk_toGroup">انتشار انتشارات برای گروه های زیر:</label>
        <div class="vertical_small"></div>

        <div id="group-panel" style="display: none; /*border: 1px solid #cbcaca; padding: 5px 5px 5px 0px; */">
            <div class="sub-right">
                <div class="field">
                    <span class="label span">گروه</span>

                    <div class="combo">
                        <div id="grp" class=" select">
                        </div>
                    </div>
                </div>
                <div class="clear"></div>
                <div class="field">
                    <span class="label span">زیرگروه</span>

                    <div class="combo">
                        <div id="sgrp" class=" select">
                        </div>
                    </div>
                </div>
                <div class="clear">
                </div>
                <input id="btn-save-add-group" type="button" class="add-group" value="✔" />
            </div>
            <div id="sub-left" class="sub-left">
            </div>
            <div class="clear"></div>
        </div>
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
            <input id="btn-save-insert-para-window" type="button" class="save" value="✔" />
            <input id="btn-close-insert-para-window" type="button" class="save" style="font-weight:bolder;font-size:23px" value="ˣ" />
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
            <div class="window-header mci-1b"><span>درحال ذخیره سازی انتشارات...</span></div>
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
