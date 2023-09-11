<%@ Page Title="" Language="C#" MasterPageFile="~/admin/master.Master" AutoEventWireup="true" CodeBehind="groups.aspx.cs" Inherits="AppinnoNew.admin.groups" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="pages/groups/css.css" rel="stylesheet" />
    <script src="pages/groups/ajax.js"></script>
    <script src="pages/groups/js.js"></script>
    <link href="../component/simlComboBox/css.css" rel="stylesheet" />
    <script src="../component/simlComboBox/jquery.comboBox.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="path" runat="server">
    <a href="dashboard.aspx" class="loc history">داشبورد</a>
    <a class="spacer">></a>
    <a class="loc none">مدیریت کاربران</a>
    <a class="spacer">></a>
    <a class="loc none">مدیریت گروه ها</a>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="content" runat="server">
    <div class="vertical_min"></div>
    <div class="right-panel">
        <div class="group-list">
            <span class="header-group mci-1b">لیست گروه ها <span id="total-count-group"></span></span>
            <div id="list">
                <div class="item">
                    <div class="tools">
                        <a class="delete"></a>
                        <a class="edit"></a>
                    </div>
                    <span class="title">مدیران</span>

                </div>
            </div>
            <div class="insert">
                <input id="btn-insert-group" type="button" value="✔" class="button" />
            </div>
            <div class="clear"></div>
        </div>

    </div>
    <div class="left-panel">
        <div class="group-list">
            <span id="subtitle" class="header-subgroup mci-1b">- </span>
            <div id="sublist">
            </div>
            <div class="insert">
                <input id="btn-insert-subgroup" type="button" value="✔" class="button" />
            </div>
        </div>

    </div>
    <div class="clear"></div>
    <div id="new-group-window" class="new-group-window">
        <div class="background"></div>
        <div class="window">
            <div class="window-header mci-1b"><span>درج گروه جدید</span></div>
            <div class="vertical_min"></div>
            <div class="group-title"><span>نام گروه</span><input id="input-title-insert-group-window" type="text" value="" placeholder="نام گروه" /></div>

            <input id="btn-save-insert-group-window" type="button" class="save" value="✔" />
            <input id="btn-close-insert-group-window" type="button" class="save" style="font-weight:bolder;font-size:23px" value="ˣ" />
        </div>
    </div>
    <div id="edit-group-window" class="new-group-window">
        <div class="background"></div>
        <div class="window">
            <div class="window-header mci-1b"><span>ویرایش گروه</span></div>
            <div class="vertical_min"></div>
            <div class="group-title"><span>نام گروه</span><input id="input-title-edit-group-window" type="text" value="" placeholder="نام گروه" /></div>

            <input id="btn-save-edit-group-window" type="button" class="save" value="✔" />
            <input id="btn-close-edit-group-window" type="button" class="save" style="font-weight:bolder;font-size:23px" value="ˣ" />
        </div>
    </div>

    <div id="new-subgroup-window" class="new-subgroup-window">
        <div class="background"></div>
        <div class="window">
            <div class="window-header mci-1b"><span>درج زیرگروه جدید</span></div>
            <div class="vertical_min"></div>
            <div class="subgroup-title"><span>نام زیرگروه</span><input id="input-title-insert-subgroup-window" type="text" value="" placeholder="نام زیرگروه" /></div>
            <div class="vertical_small"></div>
            <input class="check" type="checkbox" id="chk_canPush" value="توانایی ارسال پیام" />
            <label for="chk_canPush">توانایی ارسال پیام</label>
            <div class="vertical_min"></div>

            <div id="pushType" style="display: none">
                <input class="check" type="radio" id="chk_toAll" name="1" />
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
                        <div class="subgroup-name">
                            <span>گروه</span>
                        </div>
                        <div class="combo">
                            <div id="grp" class=" select">
                            </div>
                        </div>
                        <div class="clear"></div>
                        <div class="subgroup-name">
                            <span>زیرگروه</span>
                        </div>
                        <div class="combo">
                            <div id="sgrp" class=" select">
                            </div>
                        </div>
                        <div class="clear">
                        </div>
                        <input id="btn-save-insert-subgroup" type="button" class="insert-subgroup" value="✔" />
                    </div>
                    <div id="sub-left" class="sub-left">
                    </div>
                    <div class="clear"></div>
                </div>
            </div>

            <div class="vertical_small"></div>
            <hr />
            <input id="btn-save-insert-subgroup-window" type="button" class="save" value="✔" />
            <input id="btn-close-insert-subgroup-window" type="button" class="save" style="font-weight:bolder;font-size:23px" value="ˣ" />
        </div>
    </div>

    
    <div id="edit-subgroup-window" class="edit-subgroup-window">
        <div class="background"></div>
        <div class="window">
            <div class="window-header mci-1b"><span>ویرایش زیرگروه</span></div>
            <div class="vertical_min"></div>
            <div class="subgroup-title"><span>نام زیرگروه</span><input id="input-title-edit-subgroup-window" type="text" value="" placeholder="نام زیرگروه" /></div>
            <div class="vertical_small"></div>
            <input class="check" type="checkbox" id="echk_canPush" value="توانایی ارسال پیام" />
            <label for="echk_canPush">توانایی ارسال پیام</label>
            <div class="vertical_min"></div>

            <div id="epushType" style="display: none">
                <input class="check" type="radio" id="echk_toAll" name="1" />
                <label for="echk_toAll">ارسال پیام برای همه</label>
                <div class="vertical_min"></div>
                <input class="check" type="radio" id="echk_toPartner" name="1" />
                <label for="echk_toPartner">ارسال پیام برای همکاران</label>
                <div class="vertical_min"></div>
                <input class="check" type="radio" id="echk_toGroup" name="1" />
                <label for="echk_toGroup">ارسال پیام برای گروه های مورد نظر:</label>
                <div class="vertical_min"></div>
                <div id="egroup-panel" style="display: none; border: 1px solid #cbcaca; padding: 5px 5px 5px 0px; margin: 0px 10px 0px 10px;">
                    <div class="sub-right">
                        <div class="subgroup-name">
                            <span>گروه</span>
                        </div>
                        <div class="combo">
                            <div id="egrp" class=" select">
                            </div>
                        </div>
                        <div class="clear"></div>
                        <div class="subgroup-name">
                            <span>زیرگروه</span>
                        </div>
                        <div class="combo">
                            <div id="esgrp" class=" select">
                            </div>
                        </div>
                        <div class="clear">
                        </div>
                        <input id="btn-save-edit-subgroup" type="button" class="insert-subgroup" value="✔" />
                    </div>
                    <div id="esub-left" class="sub-left">
                    </div>
                    <div class="clear"></div>
                </div>
            </div>

            <div class="vertical_small"></div>
            <hr />
            <input id="btn-save-edit-subgroup-window" type="button" class="save" value="✔" />
            <input id="btn-close-edit-subgroup-window" type="button" class="save" style="font-weight:bolder;font-size:23px" value="ˣ" />
        </div>
    </div>
</asp:Content>
