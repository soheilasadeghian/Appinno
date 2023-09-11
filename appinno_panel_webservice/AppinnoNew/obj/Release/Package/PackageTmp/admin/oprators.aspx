<%@ Page Title="" Language="C#" MasterPageFile="~/admin/master.Master" AutoEventWireup="true" CodeBehind="oprators.aspx.cs" Inherits="AppinnoNew.admin.oprators" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="pages/oprators/css.css" rel="stylesheet" />
    <link href="component/css/search.css" rel="stylesheet" />
    <link href="../component/simlComboBox/css.css" rel="stylesheet" />
    <script src="../component/simlComboBox/jquery.comboBox.js"></script>
    <script src="pages/oprators/ajax.js"></script>
    <script src="pages/oprators/js.js"></script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="path" runat="server">
    <a href="dashboard.aspx" class="loc history">داشبورد</a>
    <a class="spacer">></a>
    <a class="loc none">مدیریت دسترسی</a>
    <a class="spacer">></a>
    <a class="loc none">مدیریت اپراتورها</a>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="content" runat="server">
    <div class="vertical_min"></div>
    <div class="right-panel">
        <div class="search-box">
            <span class="search-header mci-1b">جستجو</span>
            <div class="vertical_small"></div>
            <input id="fillter-input" type="text" class="search-input" placeholder="جستجو در اپراتورها..." value="" />
            <div class="vertical_small"></div>
            <div class="combo">
                <div id="parts" class=" select">
                </div>
            </div>
            <div class="vertical_small"></div>
            <input id="search-btn" type="button" class="search-button" value="جستجو" />
        </div>
    </div>
    <div class="left-panel">
        <div class="group-list">
            <span class="header-group mci-1b">لیست اپراتورها <span id="total-count"></span></span>
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
                <input id="insert-btn" type="button" value="✔" class="button" />
            </div>
        </div>
    </div>
    <div class="clear"></div>

    <div id="new-oprator-window" class="new-oprator-window">
        <div class="background"></div>
        <div class="window">
            <div class="window-header mci-1b"><span>درج اپراتور جدید</span></div>
            <div class="vertical_min"></div>
            <div class="oprator-name"><span>نام کاربری</span><input id="input-emailtell-insert-oprator-window" type="text" value="" placeholder="نام کاربری" /></div>
            <div class="oprator-name">
                <span>نقش</span>
            </div>
            <div class="combo">
                <div id="parts2" class=" select">
                </div>
            </div>
            <div class="vertical_small"></div>
            <div class="oprator-name"><span>کلمه عبور</span><input id="input-password-insert-oprator-window" type="password" value="" placeholder="کلمه عبور" /></div>
            <div class="oprator-name"><span>تکرار کلمه عبور</span><input id="input-repassword-insert-oprator-window" type="password" value="" placeholder="تکرار کلمه عبور" /></div>
            <div class="vertical_small"></div>
            <div class="oprator-name"><span>نام</span><input id="input-name-insert-oprator-window" type="text" value="" placeholder="نام" /></div>
            <div class="oprator-name"><span>نام خانوادگی</span><input id="input-family-insert-oprator-window" type="text" value="" placeholder="نام خانوادگی" /></div>
            <div class="vertical_min"></div>
            <input id="btn-save-insert-oprator-window" type="button" class="save" value="✔" />
            <input id="btn-close-insert-oprator-window" type="button" class="save" style="font-weight:bolder;font-size:23px" value="ˣ" />
        </div>

    </div>
    <div id="edit-oprator-window" class="new-oprator-window">
        <div class="background"></div>
        <div class="window">
            <div class="window-header mci-1b"><span>ویرایش اپراتور</span></div>
            <div class="vertical_min"></div>
            <div class="oprator-name"><span>نام کاربری</span><input id="input-emailtell-edit-oprator-window" disabled="disabled" readonly type="text" value="" placeholder="نام کاربری" /></div>
            <div class="oprator-name">
                <span>نقش</span>
            </div>
            <div class="combo">
                <div id="parts3" class=" select">
                </div>
            </div>
            <div class="vertical_small"></div>
            <div class="oprator-name"><span>کلمه عبور</span><input id="input-password-edit-oprator-window" type="password" value="" placeholder="کلمه عبور جدید..." /></div>
            <div class="oprator-name"><span>تکرار کلمه عبور</span><input id="input-repassword-edit-oprator-window" type="password" value="" placeholder="تکرار کلمه عبور جدید..." /></div>
            <div class="vertical_small"></div>
            <div class="oprator-name"><span>نام</span><input id="input-name-edit-oprator-window" type="text" value="" placeholder="نام" /></div>
            <div class="oprator-name"><span>نام خانوادگی</span><input id="input-family-edit-oprator-window" type="text" value="" placeholder="نام خانوادگی" /></div>
            <div class="vertical_min"></div>
            <input id="btn-save-edit-oprator-window" type="button" class="save" value="✔" />
            <input id="btn-close-edit-oprator-window" type="button" class="save" style="font-weight:bolder;font-size:23px" value="ˣ" />
        </div>

    </div>

</asp:Content>
