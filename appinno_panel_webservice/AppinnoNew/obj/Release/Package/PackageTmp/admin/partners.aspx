<%@ Page Title="" Language="C#" MasterPageFile="~/admin/master.Master" AutoEventWireup="true" CodeBehind="partners.aspx.cs" Inherits="AppinnoNew.admin.partners" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="pages/partners/css.css" rel="stylesheet" />
    <link href="component/css/search.css" rel="stylesheet" />
    <link href="../component/simlComboBox/css.css" rel="stylesheet" />
    <script src="../component/simlComboBox/jquery.comboBox.js"></script>
    <script src="pages/partners/ajax.js"></script>
    <script src="pages/partners/js.js"></script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="path" runat="server">
    <a href="dashboard.aspx" class="loc history">داشبورد</a>
    <a class="spacer">></a>
    <a class="loc none">مدیریت ارتباطات</a>
    <a class="spacer">></a>
    <a class="loc none">معرفی همکاران</a>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="content" runat="server">
    <div class="vertical_min"></div>
    <div class="right-panel">
        <div class="search-box">
            <span class="search-header mci-1b">جستجو</span>
            <div class="vertical_small"></div>
            <input id="fillter-input" type="text" class="search-input" placeholder="جستجو در همکاران..." value="" />
            <div class="vertical_min"></div>
            <label class="inner-caption">مرتب سازی براساس : </label>
            <div class="vertical_small"></div>
            <div class="combo">
                <div id="sortby" class=" select">
                    <span class="option" value="name">نام</span>
                    <span class="option" value="family">نام خانوادگی</span>
                    <span class="option" value="innerTell">شماره داخلی</span>
                    <span class="option" value="email">ایمیل</span>
                    <span class="option" value="level">سمت سازمانی</span>
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
            <span class="header-group mci-1b">لیست همکاران <span style="display:inline" id="total-count"></span></span>
            <a id="deleteall-btn" title="حذف همه ی همکاران" class="delete-all"></a>
             <div class="clear"></div>
            <div class="item-header">
                <div class="tools"><span>ابزار</span></div>
                <span class="ptitle">عنوان</span><span class="text">شماره داخلی</span><span class="date">ایمیل</span><span class="text">سمت</span>
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

                <input id="insert-btn" type="button" value="✔" class="button" title="درج همکار جدید" />
                <input id="file-btn" type="button" class="button-file" title="دریافت مشخصات همکاران از فایل اکسل" />
               
            </div>
        </div>
    </div>
    <div class="clear"></div>

    <div id="upload-partner-window" class="upload-partner-window">
        <div class="background"></div>
        <div class="window">
            <div class="window-header mci-1b"><span>بارگذاری فایل اکسل مشخصات همکاران</span></div>
            <div class="vertical_min"></div>
            <div class="upload">
                <input class="text" type="text" id="fileName" value="" readonly="true" disabled="disabled" />
                <input id="btn-browse-upload-partner-window" type="button" class="button-browse" />
                <div class="clear"></div>
                <input type="file" id="file" />
                <div id="progress" class="progress">
                    <div id="bar" class="bar"></div>
                </div>

            </div>
            <div class="vertical_min"></div>
            <input id="btn-save-upload-partner-window" type="button" class="button-upload" />
            <input id="btn-close-upload-upload-window" type="button" class="save" value="-" />
        </div>

    </div>

    <div id="new-partner-window" class="new-partner-window">
        <div class="background"></div>
        <div class="window">
            <div class="window-header mci-1b"><span>درج همکار جدید</span></div>
            <div class="vertical_min"></div>
            <div class="group-title"><span>نام</span><input id="input-name-insert-partner-window" type="text" value="" placeholder="نام" /></div>
            <div class="group-title"><span>نام خانوادگی</span><input id="input-family-insert-partner-window" type="text" value="" placeholder="نام خانوادگی" /></div>
            <div class="group-title"><span>ایمیل</span><input id="input-email-insert-partner-window" type="text" value="" placeholder="ایمیل" /></div>
            <div class="group-title"><span>سمت سازمانی</span><input id="input-level-insert-partner-window" type="text" value="" placeholder="سمت سازمانی" /></div>
            <div class="group-title"><span>شماره داخلی</span><input id="input-innerTell-insert-partner-window" type="text" value="" placeholder="شماره داخلی" /></div>
            <div class="group-title"><span>شماره موبایل*</span><input id="input-registrationmobile-insert-partner-window" type="text" value="" placeholder="شماره موبایل" /></div>
            <div class="group-title"><span>شماره تلفن اختیاری</span><input id="input-optionalmobile-insert-partner-window" type="text" value="" placeholder="شماره تلفن اختیاری" /></div>

            <div class="vertical_min"></div>
            <hr />
            <input id="btn-save-insert-partner-window" type="button" class="save" value="✔" />
            <input id="btn-close-insert-partner-window" type="button" class="save" style="font-weight:bolder;font-size:23px" value="ˣ" />
        </div>

    </div>

    <div id="edit-partner-window" class="new-partner-window">
        <div class="background"></div>
        <div class="window">
            <div class="window-header mci-1b"><span>ویرایش مشخصات همکار</span></div>
            <div class="vertical_min"></div>
            <div class="group-title"><span>نام</span><input id="input-name-edit-partner-window" type="text" value="" placeholder="نام" /></div>
            <div class="group-title"><span>نام خانوادگی</span><input id="input-family-edit-partner-window" type="text" value="" placeholder="نام خانوادگی" /></div>
            <div class="group-title"><span>ایمیل</span><input id="input-email-edit-partner-window" type="text" value="" placeholder="ایمیل" /></div>
            <div class="group-title"><span>سمت سازمانی</span><input id="input-level-edit-partner-window" type="text" value="" placeholder="سمت سازمانی" /></div>
            <div class="group-title"><span>شماره داخلی</span><input id="input-innerTell-edit-partner-window" type="text" value="" placeholder="شماره داخلی" /></div>
            <div class="group-title"><span>شماره موبایل*</span><input id="input-registrationmobile-edit-partner-window" type="text" value="" placeholder="شماره موبایل" /></div>
            <div class="group-title"><span>شماره تلفن اختیاری</span><input id="input-optionalmobile-edit-partner-window" type="text" value="" placeholder="شماره تلفن اختیاری" /></div>


            <div class="vertical_min"></div>
            <hr />
            <input id="btn-save-edit-partner-window" type="button" class="save" value="✔" />
            <input id="btn-close-edit-partner-window" type="button" class="save" style="font-weight:bolder;font-size:23px" value="ˣ" />
        </div>

    </div>
</asp:Content>
