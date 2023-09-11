<%@ Page Title="" Language="C#" MasterPageFile="~/admin/master.Master" AutoEventWireup="true" CodeBehind="userlist.aspx.cs" Inherits="AppinnoNew.admin.userlist" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="pages/userlist/css.css" rel="stylesheet" />
    <link href="component/css/search.css" rel="stylesheet" />
    <link href="../component/simlComboBox/css.css" rel="stylesheet" />
    <script src="../component/simlComboBox/jquery.comboBox.js"></script>
    <script src="pages/userlist/ajax.js"></script>
    <script src="pages/userlist/js.js"></script>
    <link href="../component/datepicker/styles/jquery-ui-1.8.14.css" rel="stylesheet" />
    <script src="../component/datepicker/scripts/jquery.ui.datepicker-cc.all.min.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="path" runat="server">
    <a href="dashboard.aspx" class="loc history">داشبورد</a>
    <a class="spacer">></a>
    <a class="loc none">کاربران</a>
    <a class="spacer">></a>
    <a class="loc none">لیست کاربران</a>
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

            <label class="inner-caption">نمایش کاربران : </label>
            <div class="vertical_small"></div>
            <div class="combo">
                <div id="userstate" class=" select">
                    <span class="option" value="all">همه</span>
                    <span class="option" value="block">مسدود شده</span>
                    <span class="option" value="unblock">مسدود نشده</span>
                </div>
            </div>
            <div class="vertical_small"></div>

            <hr />
            <div class="vertical_small"></div>
            <input id="fillter-input" type="text" class="search-input" placeholder="جستجو در کاربران..." value="" />
            <div class="vertical_small"></div>
            <hr />

            <div class="vertical_small"></div>
            <input class="check" type="radio" id="fchk_nogrp" checked="checked" name="2" />
            <label for="fchk_nogrp">کاربران گروه بندی نشده</label>
            <div class="vertical_min"></div>

            <input class="check" type="radio" id="fchk_alluser" name="2" />
            <label for="fchk_alluser">همه کاربران گروه بندی شده</label>
            <div class="vertical_min"></div>

            <input class="check" type="radio" id="fchk_toGroup" name="2" />
            <label for="fchk_toGroup">کاربران قرارگرفته در گروه زیر:</label>
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
            <label class="inner-caption">مرتب سازی براساس : </label>
            <div class="vertical_small"></div>
            <div class="combo">
                <div id="sortby" class=" select">
                     <span class="option" value="regdate">تاریخ ثبت نام</span>
                    <span class="option" value="name">نام</span>
                    <span class="option" value="family">نام خانوادگی</span>
                    <span class="option" value="emailtell">نام کاربری</span>
                   
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
            <span class="header-group mci-1b"><span id="lt">لیست کاربران گروه بندی نشده</span> <span style="display:inline" id="total-count"></span></span>
           <%-- <a id="deleteall-btn" class="delete-all" title="حذف همه ی کاربران"></a>--%>
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

    <div id="edituserwindow" class="edit-user-window">
        <div class="background"></div>
        <div class="window">
            <div class="window-header mci-1b"><span>ویرایش کاربر</span></div>
            <div class="vertical_min"></div>
            <div class="user-title"><span>شماره تماس</span><input id="input-tell-edit-user-window" type="text" value="" readonly /></div>
            <div class="user-title"><span>تاریخ ثبت نام</span><input id="input-regdate-edit-user-window" type="text" value="" readonly /></div>
            <div class="vertical_small"></div>
            <div class="user-title"><span>نام </span>
                <input id="input-name-edit-user-window" type="text" value="" readonly /></div>
            <div class="user-title"><span>نام خانوادگی </span>
                <input id="input-family-edit-user-window" type="text" value="" readonly /></div>

            <div class="vertical_small"></div>

            <label>گروه های کاربر:</label>

            <div id="group-panel" style="display: block; border: 1px solid #cbcaca; padding: 5px 5px 5px 0px; margin: 0px 10px 0px 10px;">
                <div class="sub-right">
                    <div class="user-name">
                        <span>گروه</span>
                    </div>
                    <div class="combo">
                        <div id="grp" class=" select">
                        </div>
                    </div>
                    <div class="clear"></div>
                    <div class="user-name">
                        <span>زیرگروه</span>
                    </div>
                    <div class="combo">
                        <div id="sgrp" class=" select">
                        </div>
                    </div>
                    <div class="clear">
                    </div>
                    <input id="btn-save-edit-user" type="button" class="edit-user" value="✔" />
                </div>
                <div id="sub-left" class="sub-left">
                </div>
                <div class="clear"></div>
            </div>

            <div class="vertical_small"></div>
            <input class="check" type="checkbox" id="block" />
            <label for="block">کاربر مسدود شده</label>
            <hr />
            <div class="vertical_small"></div>
            <input id="btn-save-edit-user-window" type="button" class="save" value="✔" />
            <input id="btn-close-edit-user-window" type="button" class="save" style="font-weight:bolder;font-size:23px" value="ˣ" />
        </div>
    </div>

</asp:Content>
