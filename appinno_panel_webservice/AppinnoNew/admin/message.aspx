<%@ Page Title="" Language="C#" MasterPageFile="~/admin/master.Master" AutoEventWireup="true" CodeBehind="message.aspx.cs" Inherits="AppinnoNew.admin.message" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="pages/message/css.css" rel="stylesheet" />
    <link href="component/css/search.css" rel="stylesheet" />
    <link href="../component/simlComboBox/css.css" rel="stylesheet" />
    <script src="../component/simlComboBox/jquery.comboBox.js"></script>
    <script src="pages/message/ajax.js"></script>
    <script src="pages/message/js.js"></script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="path" runat="server">
    <a href="dashboard.aspx" class="loc history">داشبورد</a>
    <a class="spacer">></a>
    <a class="loc none">مدیریت ارتباطات</a>
    <a class="spacer">></a>
    <a class="loc none">ارسال پیام کاربر</a>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="content" runat="server">
    <div class="vertical_min"></div>
    <div class="right-panel">
        <div class="search-box">
            <span class="search-header mci-1b">جستجو</span>
            <div class="vertical_small"></div>
            <input id="fillter-input" type="text" class="search-input" placeholder="جستجو در پیام ها..." value="" />
            <div class="vertical_min"></div>
            <input class="check" type="radio" id="fchk_toAll" checked="checked" name="2" />
            <label for="fchk_toAll">ارسال شده برای همه</label>
            <div class="vertical_min"></div>
            <input class="check" type="radio" id="fchk_toPartner" name="2" />
            <label for="fchk_toPartner">ارسال شده برای همکاران</label>
            <div class="vertical_min"></div>
            <input class="check" type="radio" id="fchk_toGroup" name="2" />
            <label for="fchk_toGroup">ارسال شده برای گروه مورد نظر:</label>
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
            <label class="inner-caption">مرتب سازی براساس تاریخ ارسال : </label>
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
            <span  class="header-group mci-1b"><span id="lt">لیست پیام هایی که برای همه ارسال شده است</span>  <span style="display:inline" id="total-count"></span></span>
            <a id="deleteall-btn" class="delete-all" title="حذف همه ی پیام های ارسال شده"></a>
            <div class="clear"></div>
            <div class="item-header">
                <div class="tools"><span>ابزار</span></div>
                <span class="ptitle">عنوان</span><span class="text">متن پیام</span><span class="date">تاریخ</span>
            </div>
            <div id="list">
                <span>درحال دریافت اطلاعات...</span>
            </div>

            <div class="insert">
                <div class="pager" id="pager">
                    <a class="page">1</a>
                    <a class="page selected">2</a>
                    <a class="page">3</a>
                    <div class="clear"></div>
                </div>

                <input id="insert-btn" type="button" value="✔" class="button" title="ارسال پیام جدید" />
            </div>
        </div>
    </div>
    <div class="clear"></div>

    <div id="newpushwindow" class="new-push-window">
        <div class="background"></div>
        <div class="window">
            <div class="window-header mci-1b"><span>ارسال پیام جدید</span></div>
            <div class="vertical_min"></div>
            <div class="push-title"><span>عنوان پیام</span><input id="input-title-insert-push-window" type="text" value="" placeholder="عنوان پیام" /></div>
            <div class="vertical_small"></div>
            <div class="push-title"><span>متن پیام</span><input id="input-text-insert-push-window" type="text" value="" placeholder="متن پیام" /></div>
            <div class="vertical_small"></div>
            <div id="pushType">
                <input class="check" type="radio" id="chk_toAll" checked="checked" name="1" />
                <label for="chk_toAll">ارسال پیام برای همه</label>
                <div class="vertical_min"></div>
                <input class="check" type="radio" id="chk_toPartner" name="1" />
                <label for="chk_toPartner">ارسال پیام برای همکاران</label>
                <div class="vertical_min"></div>
                <input class="check" type="radio" id="chk_toGroup" name="1" />
                <label for="chk_toGroup">ارسال پیام برای گروه های مورد نظر:</label>
                <div class="vertical_min"></div>
                <div id="group-panel" style="display: none; border: 1px solid #cbcaca; padding: 5px 5px 5px 0px; margin: 0px 10px 0px 10px;">
                    <div class="sub-right">
                        <div class="push-name">
                            <span>گروه</span>
                        </div>
                        <div class="combo">
                            <div id="grp" class=" select">
                            </div>
                        </div>
                        <div class="clear"></div>
                        <div class="push-name">
                            <span>زیرگروه</span>
                        </div>
                        <div class="combo">
                            <div id="sgrp" class=" select">
                            </div>
                        </div>
                        <div class="clear">
                        </div>
                        <input id="btn-save-insert-push" type="button" class="insert-push" value="✔" />
                    </div>
                    <div id="sub-left" class="sub-left">
                    </div>
                    <div class="clear"></div>
                </div>
            </div>

            <div class="vertical_small"></div>
            <hr />
            <input id="btn-save-insert-push-window" type="button" class="save" value="✔" />
            <input id="btn-close-insert-push-window" type="button" class="save" style="font-weight:bolder;font-size:23px" value="ˣ" />
        </div>
    </div>

    <div id="viewpushwindow" class="view-push-window">
        <div class="background"></div>
        <div class="window">
            <div class="window-header mci-1b"><span>نمایش پیام ارسال شده</span></div>
            <div class="vertical_min"></div>
            <div class="push-title"><span>عنوان پیام</span><input id="input-title-view-push-window" readonly type="text" value="" placeholder="عنوان پیام" /></div>
            <div class="vertical_small"></div>
            <div class="push-title"><span>متن پیام</span><input id="input-text-view-push-window" readonly type="text" value="" placeholder="متن پیام" /></div>
            <div class="vertical_small"></div>
            <div class="push-title"><span>تاریخ ارسال</span><input id="input-date-view-push-window" readonly type="text" value="" placeholder="تاریخ ارسال" /></div>
            <div class="vertical_small"></div>
            <div class="image-title" id="image-title"><span>عکس</span><div id="input-image-view-push-window" class="input-image-view-push-window"></div></div>
            <div class="vertical_small"></div>
            <span style="display: block; font-size: 0.9em; margin: 10px auto 10px auto; text-align: center;" id="viewpushType"></span>
            <div class="vertical_small"></div>
            <div class="pushto" id="viewpushTo"></div>
            <div class="vertical_small"></div>
            <hr />
            <input id="btn-view-insert-push-window" type="button" class="save" value="-" />
            <div id="sendmessage"></div>
        </div>
    </div>

</asp:Content>
