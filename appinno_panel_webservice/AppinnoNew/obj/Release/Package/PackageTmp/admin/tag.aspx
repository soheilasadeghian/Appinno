<%@ Page Title="" Language="C#" MasterPageFile="~/admin/master.Master" AutoEventWireup="true" CodeBehind="tag.aspx.cs" Inherits="AppinnoNew.admin.tag" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="pages/partners/css.css" rel="stylesheet" />
    <link href="component/css/search.css" rel="stylesheet" />
    <link href="../component/simlComboBox/css.css" rel="stylesheet" />
    <script src="../component/simlComboBox/jquery.comboBox.js"></script>
    <script src="pages/tag/ajax.js"></script>
    <script src="pages/tag/js.js"></script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="path" runat="server">
    <a href="dashboard.aspx" class="loc history">داشبورد</a>
    <a class="spacer">></a>
    <a class="loc none">مدیریت محتوا</a>
    <a class="spacer">></a>
    <a class="loc none">مدیریت تگ</a>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="content" runat="server">
    <div class="vertical_min"></div>
    <div class="right-panel">
        <div class="search-box">
            <span class="search-header mci-1b">جستجو</span>
            <div class="vertical_small"></div>
            <input id="fillter-input" type="text" class="search-input" placeholder="جستجو در تگ ها..." value="" />
            <div class="vertical_min"></div>
            <label class="inner-caption">مرتب سازی : </label>
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
            <span class="header-group mci-1b">لیست تگ ها  <span style="display:inline" id="total-count"></span></span>
            <a id="deleteall-btn" title="حذف همه ی تگ ها" class="delete-all"></a>
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

                <input id="insert-btn" type="button" value="✔" class="button" title="درج تگ جدید" />
            </div>
        </div>
    </div>
    <div class="clear"></div>

    <div id="new-partner-window" class="new-partner-window">
        <div class="background"></div>
        <div class="window">
            <div class="window-header mci-1b"><span>درج تگ جدید</span></div>
            <div class="vertical_min"></div>
            <div class="group-title"><span>عنوان تگ</span><input id="input-name-insert-partner-window" type="text" value="" placeholder="عنوان تگ" /></div>
           
            <div class="vertical_min"></div>
            <hr />
            <input id="btn-save-insert-partner-window" type="button" class="save" value="✔" />
            <input id="btn-close-insert-partner-window" type="button" class="save" style="font-weight:bolder;font-size:23px" value="ˣ" />
        </div>

    </div>

    <div id="edit-partner-window" class="new-partner-window">
        <div class="background"></div>
        <div class="window">
            <div class="window-header mci-1b"><span>ویرایش عنوان تگ</span></div>
            <div class="vertical_min"></div>
            <div class="group-title"><span>عنوان تگ</span><input id="input-name-edit-partner-window" type="text" value="" placeholder="عنوان تگ" /></div>
            <div class="vertical_min"></div>
            <hr />
            <input id="btn-save-edit-partner-window" type="button" class="save" value="✔" />
            <input id="btn-close-edit-partner-window" type="button" class="save" style="font-weight:bolder;font-size:23px" value="ˣ" />
        </div>

    </div>
</asp:Content>
