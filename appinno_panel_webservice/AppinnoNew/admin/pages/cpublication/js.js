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

    $("#lt").html("لیست نظرات");
    PageIndex = 1;
    getAllComment();
    search();

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

    var item = $("#search-btn");
    item.click(function () {
        PageIndex = 1;
        getAllComment();
    });
}
function getAllComment() {
    var filter = $("#fillter-input").val();
    var order = $("#order").attr('value');
    var sortby = $("#sortby").attr('value');
    var date = $("#date").val();
    var dateto = $("#dateto").val();
    var userstate = $("#userstate").attr('value');

    var result = cpublication.f1(date, dateto, userstate, filter, order, sortby, PageIndex);
    if (result.result.code != 0) {
        Siml.messages.add({ text: result.result.message, type: "error" });
        return;
    }
    var list = $("#list");
    $("#total-count").html("( " + result.totalCount + " )");

    $(list).html('');
    $.each(result.comment, function (index, el) {
        var block = el.isBlock ? '<a itemid="' + el.ID + '" class="block" title="نظر مسدود شده است"></a>' : '<a itemid="' + el.ID + '" class="edit" title="نظر درحال نمایش است"></a>';
        var html =
                    $('<div class="item" itemid="' + el.ID + '">'
                    + '<div class="tools">'
                    + '<a itemid="' + el.ID + '" class="delete"></a>'
                    + block
                    + '</div>'
                    + '<span class="ptitle">' + el.fullName + '</span>'
                    + '<span class="text">' + el.mobileOrTell + '</span>'
                    + '<span class="date">' + el.regDate + '</span>'
                    + '<div class="clear" />'
                    + '<div class="comment-text">' + el.text + '</div>'
                    + '<div class="clear" />'
                    + '<a target="_blank" href="/admin/epublication.aspx?id=' + el.newsID + '"><div class="link-icon" /><div class="link-text">' + el.newsTitle + '</div></a>'
                    + '</div>');

        $(list).append(html);
        setup_delete_comment(html);
        setup_edit_comment(html);
        setup_block_comment(html);
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

        if (confirm('آیا از حذف تمام نظرات اطمینان دارید؟')) {
            var result = cpublication.f5();
            if (result.code != 0) {
                Siml.messages.add({ text: result.message, type: "error" });
                return;
            }
            Siml.messages.add({ text: result.message, type: "success" });
            getAllComment();

        }

    });
}


///
function setup_delete_comment(item) {

    var deleteBtn = $(item).find('.delete');
    $(deleteBtn).click(function () {
        if (confirm("آیا از حذف این نظر اطمینان دارید؟")) {
            var itemid = $(this).attr('itemid');
            var result = cpublication.f4(itemid);
            if (result.code != 0) {
                Siml.messages.add({ text: result.message, type: "error" });
                return;
            }
            Siml.messages.add({ type: "success", text: result.message });
            getAllComment();
        }
    });
}

//edit subgroup
function setup_edit_comment(item) {

    var edit = $(item).find('.edit');
    $(edit).click(function () {

        var itemID = $(this).attr('itemID');
        if (confirm("آیا از مسدود کردن نظر اطمینان دارید؟"))
        {
            var result = cpublication.f2(itemID,true);
            if (result.code == 0) {
                Siml.messages.add({ type: "success", text: result.message });
                getAllComment();
            }
            else {
                Siml.messages.add({ text: result.message, type: "error" });
                return;
            }
        }
       
    });
}
//edit subgroup
function setup_block_comment(item) {

    var edit = $(item).find('.block');
    $(edit).click(function () {

        var itemID = $(this).attr('itemID');
        if (confirm("آیا از نمایش نظر اطمینان دارید؟")) {
            var result = cpublication.f2(itemID, false);
            if (result.code == 0) {
                Siml.messages.add({ type: "success", text: result.message });
                getAllComment();
            }
            else {
                Siml.messages.add({ text: result.message, type: "error" });
                return;
            }
        }

    });
}
function open_edit_comment_window(item) {

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
function setup_edit_comment_window() {

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
function close_edit_comment_window() {
    var insertwindow = $("#edituserwindow");
    insertwindow.css('display', 'none');

}

