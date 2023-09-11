var PageIndex = 1;
var permission;
var editingAccessID = -1;

$(function () {

    access_permission();

    $("#order").simlComboBox();
    $("#sortby").simlComboBox();
    $("#userstate").simlComboBox();

    $('#date').datepicker({ isRTL: false, showButtonPanel: true });
    $('#dateto').datepicker({ isRTL: false, showButtonPanel: true });

    getAllGroupForCombo();
    $("#lt").html("لیست کاربران گروه بندی نشده");
    PageIndex = 1;
    getAllUser();
    search();
    setup_edit_user_window();
    // setup_new_push_window();
});

function access_permission() {
    var ac = Siml.Access.get('dashboard.aspx');

    if (ac.result.code != 0) {
        Siml.messages.add({ text: ac.result.message, type: "error" });
        return;
    }
    else {
        permission = ac.user;
    }
}
function search() {
    $('#fillter-input').keyup(function (e) {
        if (e.keyCode == 13) {
            $("#search-btn").click();
        }
    });
    $("#date").on('change', function () {
        if ($("#chk_date").is(':checked') == false) {
            $("#date").val('');
        }
    });
    $("#dateto").on('change', function () {
        if ($("#chk_dateto").is(':checked') == false) {
            $("#dateto").val('');
        }
    });
    $("#chk_date").on('change', function () {
        if ($("#chk_date").is(':checked')) {
            $("#date").trigger('focus');
        }
        else {
            $("#date").val('');
        }
    });
    $("#chk_dateto").on('change', function () {
        if ($("#chk_dateto").is(':checked')) {
            $("#dateto").trigger('focus');
        }
        else {
            $("#dateto").val('');
        }
    });
    $(".check").change(function () {

        if ($("#fchk_nogrp").is(':checked')) {
            $("#lt").html("لیست کاربران گروه بندی نشده");
        }
        if ($("#fchk_alluser").is(':checked')) {
            $("#lt").html("لیست همه کاربران گروه بندی شده");
        }

        if ($("#fchk_toGroup").is(':checked')) {
            $("#lt").html("لیست کاربرانی که در گروه [ " + $("#group").find('.selected').html() + " ] و زیرگروه  [ " + $("#subgroup").find('.selected').html() + " ] قرار دارند");
            $("#fgroup_panel").slideDown(100);
        }
        else {
            $("#fgroup_panel").slideUp(100);
        }
    });
    var item = $("#search-btn");
    item.click(function () {
        PageIndex = 1;
        getAllUser();
    });
}
function getAllUser() {
    var filter = $("#fillter-input").val();
    var subgroupID = $("#subgroup").attr('value');
    var order = $("#order").attr('value');
    var sortby = $("#sortby").attr('value');
    var date = $("#date").val();
    var dateto = $("#dateto").val();
    var userstate = $("#userstate").attr('value');

    var fchk_nogrp = $("#fchk_nogrp").attr('checked') == "checked";
    var fchk_alluser = $("#fchk_alluser").attr('checked') == "checked";
    var fchk_toGroup = $("#fchk_toGroup").attr('checked') == "checked";

    if (fchk_nogrp)
        subgroupID = -1;
    else if (fchk_alluser)
        subgroupID = -2;

    var result = userlist.f1(date, dateto, userstate, filter, subgroupID, order, sortby, PageIndex);
    if (result.result.code != 0) {
        Siml.messages.add({ text: result.result.message, type: "error" });
        return;
    }
    var list = $("#list");
    $("#total-count").html(" ( " + result.totalCount + " )");

    $(list).html('');
    $.each(result.user, function (index, el) {
        var block = el.isBlock ? '<a itemid="' + el.ID + '" class="block" title="کاربر مسدود شده است"></a>' : "";
        var html =
                    $('<div class="item" itemid="' + el.ID + '">'
                    + '<div class="tools">'
                    //+ '<a itemid="' + el.ID + '" class="delete"></a>'
                    + '<a itemid="' + el.ID + '" class="edit" title="ویرایش"></a>'
                    + block
                    + '</div>'
                    + '<span class="ptitle">' + el.name + ' ' + el.family + '</span>'
                    + '<span class="text">' + el.emailormobile + '  ( '+el.level+' )  ( '+el.innerTell+' )</span>'
                    + '<span class="date">' + el.regDate + '</span>'
                    + '</div>');

        $(list).append(html);
        setup_delete_user(html);
        setup_edit_user(html);
    });

    var pager = $("#pager");
    $(pager).find('.page').remove();
    for (var i = 0; i < result.pageCount; i++) {
        var className = (i + 1) == PageIndex ? "page selected" : "page";
        var html =
            $('<a class="' + className + '">' + (i + 1) + '</a>');
        $(pager).append(html);
        $(html).click(function () {
            if ($(this).css('class') != "selected") {
                var index = parseInt($(this).html());
                PageIndex = index;
                getAllUser();
            }
        });
    }
    $('#deleteall-btn').unbind();
    $('#deleteall-btn').click(function () {

        if (confirm('آیا از حذف تمام کاربران اطمینان دارید؟')) {
            var result = userlist.f5();
            if (result.code != 0) {
                Siml.messages.add({ text: result.message, type: "error" });
                return;
            }
            Siml.messages.add({ text: result.message, type: "success" });
            getAllUser();

        }

    });
}

function getAllGroupForCombo() {
    var result = userlist.f6();
    var grp = $("#group");
    var grp_combo = grp.parent();
    grp_combo.html('');
    var html = '<div id="group" class=" select">';
    $.each(result.groups, function (index, el) {
        html += '<span class="option" value="' + el.ID + '">' + el.title + '</span>';
    });
    html += "</div>";
    var c = $(html);
    $(grp_combo).html(html);
    $("#group").simlComboBox();
    var groupId = $("#group").attr('value');
    getAllSubGroupForCombo(groupId);


    $("#group").on("onChanged", function () {
        var groupId = $(this).attr('value');
        $("#lt").html("لیست پیام هایی که برای گروه [ " + $("#group").find('.selected').html() + " ] و زیرگروه  [ " + $("#subgroup").find('.selected').html() + " ] ارسال شده است");
        getAllSubGroupForCombo(groupId);
    });

}
function getAllSubGroupForCombo(groupId) {

    if (groupId == -1) {
        var sgrp = $("#subgroup");
        var sgrp_combo = sgrp.parent();
        sgrp_combo.html('');
        var shtml = '<div id="subgroup" class=" select"></div>';
        $(sgrp_combo).html(shtml);
        $("#subgroup").simlComboBox();
        return;
    }
    var result = userlist.f7(groupId);
    if (result.result.code != 0) {
        Siml.messages.add({ text: result.result.message, type: "error" });
        return;
    }
    var sgrp = $("#subgroup");
    var sgrp_combo = sgrp.parent();
    sgrp_combo.html('');
    var shtml = '<div id="subgroup" class=" select">';

    $.each(result.subGroup, function (index, el) {
        shtml += '<span class="option" value="' + el.ID + '">' + el.title + '</span>';
    });
    shtml += "</div>";

    $(sgrp_combo).html(shtml);
    $("#subgroup").simlComboBox();


    $("#subgroup").on("onChanged", function () {

        $("#lt").html("لیست کاربرانی که گروه [ " + $("#group").find('.selected').html() + " ] و زیرگروه  [ " + $("#subgroup").find('.selected').html() + " ] قرار دارند");

    });
    $("#subgroup").trigger("onChanged");

}


//delete user
function setup_delete_user(item) {

    var deleteBtn = $(item).find('.delete');
    $(deleteBtn).click(function () {
        if (confirm("آیا از حذف این کاربر اطمینان دارید؟")) {
            var itemid = $(this).attr('itemid');
            var result = userlist.f4(itemid);
            if (result.code != 0) {
                Siml.messages.add({ text: result.message, type: "error" });
                return;
            }
            Siml.messages.add({ type: "success", text: result.message });
            getAllUser();
        }
    });
}
///

//edit subgroup
function setup_edit_user(item) {

    var edit = $(item).find('.edit');
    $(edit).click(function () {

        var itemID = $(this).attr('itemID');
        var result = userlist.f3(itemID);
        if (result.result.code == 0) {
            editingAccessID = itemID;
            open_edit_user_window(result.user);

        }
        else {
            Siml.messages.add({ text: result.result.message, type: "error" });
            return;
        }
    });
}
function open_edit_user_window(item) {

    var editwindow = $("#edituserwindow");

    $("#input-tell-edit-user-window").val(item.emailormobile);
    $("#input-name-edit-user-window").val(item.name);
    $("#input-family-edit-user-window").val(item.family);
    $("#input-regdate-edit-user-window").val(item.regDate);
    $("#block").prop("checked", item.isBlock);
    editwindow.css('display', 'block');

    var result = userlist.f6();
    if (result.result.code != 0) {
        Siml.messages.add({ text: result.result.message, type: "error" });
        return;
    }
    var grp = $("#grp");
    var grp_combo = grp.parent();
    grp_combo.html('');
    var html = '<div id="grp" class=" select">';
    $.each(result.groups, function (index, el) {
        html += '<span class="option" value="' + el.ID + '">' + el.title + '</span>';
    });
    html += "</div>";
    var c = $(html);
    $(grp_combo).html(html);
    $("#grp").simlComboBox();
    var groupId = $("#grp").attr('value');
    getAllSubGroupForCombo2(groupId);
    $("#grp").on("onChanged", function () {
        var groupId = $(this).attr('value');
        getAllSubGroupForCombo2(groupId);
    });

    var llist = $("#sub-left");
    llist.html('');
    $.each(item.groups, function (index, el) {
        var html = $('<div class="record" itemid="' + el.subGroupID + '">'
            + '<span>' + el.groupTitle + " / " + el.subGroupTitle + '</span>'
            + '<label class="delete">X</label>'
            + '</div>');

        $(html).find('.delete').click(function () {
            $(this).parent().remove();
        });

        $(llist).append(html);
    });

}
function setup_edit_user_window() {

    var btn_save_edit_user = $("#btn-save-edit-user");
    btn_save_edit_user.click(function () {

        var subgroupId = $("#sgrp").attr('value');
        var groupTitle = $("#grp").find('.selected').html();
        var subgroupTitle = $("#sgrp").find('.selected').html();

        if (subgroupId == -1 || subgroupId == "-1") {
            Siml.messages.add({ text: "زیرگروهی انتخاب نشده است", type: "error" });
            return;
        }
        var canInsert = true;

        $.each($("#sub-left").find('.record'), function (index, el) {

            if ($(el).attr('itemid') == subgroupId + "") {
                canInsert = false;
            }

        });
        if (canInsert) {
            var html = $('<div class="record" itemid="' + subgroupId + '">'
            + '<span>' + groupTitle + " / " + subgroupTitle + '</span>'
            + '<label class="delete">X</label>'
            + '</div>');

            $(html).find('.delete').click(function () {
                $(this).parent().remove();
            });

            $("#sub-left").append(html);
        }

    });
    var btn_save_edit_user_window = $("#btn-save-edit-user-window");
    btn_save_edit_user_window.click(function () {


        var block = $("#block").attr('checked') == "checked";

        var groups = [];

        $.each($("#sub-left").find('.record'), function (index, el) {

            groups.push(parseInt($(el).attr('itemid')));

        });
        var result = userlist.f2(editingAccessID, block, groups);
        if (result.code != 0) {
            Siml.messages.add({ text: result.message, type: "error" });
            return;
        }

        Siml.messages.add({ text: result.message, type: "success" });
        getAllUser();
        close_edit_user_window();

    });
    var btn_close_edit_user_window = $("#btn-close-edit-user-window");
    btn_close_edit_user_window.click(function () {


        close_edit_user_window();
    });
}
function close_edit_user_window() {
    var insertwindow = $("#edituserwindow");
    insertwindow.css('display', 'none');

}
function getAllSubGroupForCombo2(groupId) {
    var result = userlist.f7(groupId);
    if (result.result.code != 0) {
        Siml.messages.add({ text: result.result.message, type: "error" });
        return;
    }
    var sgrp = $("#sgrp");
    var sgrp_combo = sgrp.parent();
    sgrp_combo.html('');
    var shtml = '<div id="sgrp" class=" select">';

    $.each(result.subGroup, function (index, el) {
        shtml += '<span class="option" value="' + el.ID + '">' + el.title + '</span>';
    });
    shtml += "</div>";

    $(sgrp_combo).html(shtml);
    $("#sgrp").simlComboBox();
}

