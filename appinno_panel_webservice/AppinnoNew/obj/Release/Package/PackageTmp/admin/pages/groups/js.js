var permission;
var editingAccessID = -1;
var editingsubAccessID = -1;
$(function () {

    grouping_permission();
    getAllGroup();
    setup_group_window();
    setup_subgroup_window();

});

function getAllGroup() {
    var list = $("#list");
    var result = groups.f1();

    if (result.result.code != 0) {
        Siml.messages.add({ text: result.result.message, type: "error" });
        return;
    }

    list.html('');

    $.each(result.groups, function (index, grp) {
        var canDetele = permission.delete && grp.ID != 8 ? '<a itemid="' + grp.ID + '" class="delete"></a>' : "";
        var canEdit = permission.edit && grp.ID != 8 ? '<a itemid="' + grp.ID + '" class="edit"  title="ویرایش"></a>' : "";
        var html =
                $('<div class="item" itemid="' + grp.ID + '">'
                + '<div class="tools">'
                + canDetele
                + canEdit
                + '</div>'
                + '<span class="title">' + grp.title + '</span>'
                + '</div>');

        list.append(html);
        setup_edit_group(html);
        select_group(html);
        setup_delete_group(html);
    });

}

function setup_edit_group(item) {

    var edit = $(item).find('.edit');
    $(edit).click(function () {

        var itemID = $(this).attr('itemID');

        var result = groups.f4(itemID);
        if (result.result.code == 0) {
            editingAccessID = itemID;
            open_edit_group_window(result.group);

        }
        else {
            Siml.messages.add({ text: result.result.message, type: "error" });
            return;
        }
    });
}
function setup_delete_group(item) {

    var del = $(item).find('.delete');
    $(del).click(function () {
        if (confirm('آیا از حذف این گروه اطمینان دارید؟')) {
            var itemID = $(this).attr('itemID');
            var result = groups.f5(itemID);
            if (result.code == 0) {
                Siml.messages.add({ text: result.message, type: "success" });
                getAllGroup();
            }
            else {
                Siml.messages.add({ text: result.message, type: "error" });
                return;
            }
        }
    });
}

function grouping_permission() {
    var ac = Siml.Access.get('dashboard.aspx');

    if (ac.result.code != 0) {
        Siml.messages.add({ text: ac.result.message, type: "error" });
        return;
    }
    else {
        var per = ac.access.grouping.access;
        permission = ac.access.grouping.access;
        if (!per.insert) {
            $("#btn-insert-group").remove();
            $("#btn-insert-subgroup").remove();
            return;
        }
        $("#btn-insert-group").click(function () {

            open_insert_group_window();
        });
        $("#btn-insert-subgroup").click(function () {
            open_insert_subgroup_window();
        });
    }
}
function setup_group_window() {
    $(document).keyup(function (e) {
        if (e.keyCode == 27) {
            close_insert_group_window();
            close_edit_group_window();
        }
    });
    setup_insert_group_window();
    setup_edit_group_window();
}
function setup_insert_group_window() {

    $("#input-title-insert-group-window").keyup(function (e) {
        if (e.keyCode == 13) {
            $("#btn-save-insert-group-window").click();
        }
    });
    var btn_save_insert_group_window = $("#btn-save-insert-group-window");
    btn_save_insert_group_window.click(function () {

        var title = $("#input-title-insert-group-window").val();

        if (title == "") {
            Siml.messages.add({ text: "نام گروه را وارد نمایید", type: "error" });
            return;
        }

        var result = groups.f2(title);
        if (result.code != 0) {
            Siml.messages.add({ text: result.message, type: "error" });
            return;
        }

        Siml.messages.add({ text: result.message, type: "success" });
        getAllGroup();
        close_insert_group_window();

    });
    var btn_close_insert_group_window = $("#btn-close-insert-group-window");
    btn_close_insert_group_window.click(function () {

        close_insert_group_window();
    });
}
function open_insert_group_window() {

    var insertwindow = $("#new-group-window");
    $("#input-title-insert-group-window").val('');

    insertwindow.css('display', 'block');
}
function close_insert_group_window() {
    var insertwindow = $("#new-group-window");
    insertwindow.css('display', 'none');
}

function setup_edit_group_window() {

    $("#input-title-edit-group-window").keyup(function (e) {
        if (e.keyCode == 13) {
            $("#btn-save-edit-group-window").click();
        }
    });
    var btn_save_edit_group_window = $("#btn-save-edit-group-window");
    btn_save_edit_group_window.click(function () {

        var title = $("#input-title-edit-group-window").val();

        if (title == "") {
            Siml.messages.add({ text: "نام گروه را وارد نمایید", type: "error" });
            return;
        }

        var result = groups.f3(editingAccessID, title);
        if (result.code != 0) {
            Siml.messages.add({ text: result.message, type: "error" });
            return;
        }

        Siml.messages.add({ text: result.message, type: "success" });
        getAllGroup();
        close_edit_group_window();

    });
    var btn_close_edit_group_window = $("#btn-close-edit-group-window");
    btn_close_edit_group_window.click(function () {

        close_edit_group_window();
    });
}
function open_edit_group_window(item) {

    var editwindow = $("#edit-group-window");
    $("#input-title-edit-group-window").val(item.title);
    editwindow.css('display', 'block');
}
function close_edit_group_window() {
    var editwindow = $("#edit-group-window");
    editwindow.css('display', 'none');
}

function select_group(item) {
    var title = $(item).find('.title');

    $(title).click(function () {

        $('.item').removeClass('selected');
        $(this).parent().addClass('selected');
        var itemID = $(this).parent().attr('itemid');

        $('#subtitle').html('لیست زیرگروه های ' + '[ ' + $(this).html() + ' ]');
        getAllSubGroup(itemID);

    });
}

function getAllSubGroup(groupID) {
    var sublist = $('#sublist');
    sublist.html('');

    var result = groups.f6(groupID);
    if (result.result.code != 0) {
        Siml.messages.add({ text: result.result.message, type: "error" });
        return;
    }

    $.each(result.subGroup, function (index, el) {
        var canDetele = permission.delete && el.ID != 2 ? '<a itemid="' + el.ID + '" class="delete"></a>' : "";
        var canEdit = permission.edit ? '<a itemid="' + el.ID + '" class="edit"  title="ویرایش"></a>' : "";
        var canPush = el.canPush ? '<a class="push" title="امکان ارسال پیام دارد"></a>' : "";
        var html =
            $('<div class="item" itemid="' + el.ID + '">'
            + '<div class="tools">'
            + canDetele
            + canEdit
            + canPush
            + '</div>'
            + '<span class="title">' + el.title + '</span>'
            + '</div>');
        sublist.append(html);
        setup_edit_subgroup(html);
        setup_delete_subgroup(html);
    });
}
function setup_subgroup_window() {
    $(document).keyup(function (e) {
        if (e.keyCode == 27) {
            close_insert_subgroup_window();
            close_edit_subgroup_window();
        }
    });
    setup_insert_subgroup_window();
    setup_edit_subgroup_window();
}
function setup_insert_subgroup_window() {

    $("#input-title-insert-subgroup-window").keyup(function (e) {
        if (e.keyCode == 13) {
            $("#btn-save-insert-subgroup-window").click();
        }
    });

    $(".check").change(function () {

        if ($("#chk_toGroup").is(':checked')) {
            $("#group-panel").slideDown(100);
        }
        else {
            $("#group-panel").slideUp(100);
        }
    });
    $("#chk_canPush").change(function () {

        if (this.checked) {
            $("#pushType").slideDown(100);
        }
        else {
            $("#pushType").slideUp(100);
        }
    });
    var btn_save_insert_subgroup = $("#btn-save-insert-subgroup");
    btn_save_insert_subgroup.click(function () {

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
    var btn_save_insert_subgroup_window = $("#btn-save-insert-subgroup-window");
    btn_save_insert_subgroup_window.click(function () {

        var title = $("#input-title-insert-subgroup-window").val();
        var canPush = $("#chk_canPush").attr('checked') == "checked";
        var toAll = $("#chk_toAll").attr('checked') == "checked";
        var toPartner = $("#chk_toPartner").attr('checked') == "checked";
        var toGroup = $("#chk_toGroup").attr('checked') == "checked";
        var pushTo = [];

        var groupID = $("#list").find('.selected').attr('itemid');
        if (canPush && toGroup) {
            $.each($("#sub-left").find('.record'), function (index, el) {

                pushTo.push(parseInt($(el).attr('itemid')));

            });
        }
        var result = groups.f7(title, parseInt(groupID), canPush, toAll, toPartner, pushTo);
        if (result.code != 0) {
            Siml.messages.add({ text: result.message, type: "error" });
            return;
        }

        Siml.messages.add({ text: result.message, type: "success" });

        getAllSubGroup(groupID);
        close_insert_subgroup_window();

    });
    var btn_close_insert_subgroup_window = $("#btn-close-insert-subgroup-window");
    btn_close_insert_subgroup_window.click(function () {


        close_insert_subgroup_window();
    });
}
function open_insert_subgroup_window() {

    if ($("#list").find('.selected').length == 0) {
        Siml.messages.add({ text: "ابتدا یک گروه را انتخاب کنید", type: "error" });
        return;
    }
    var insertwindow = $("#new-subgroup-window");
    $("#input-title-insert-subgroup-window").val('');
    insertwindow.css('display', 'block');

    var result = groups.f1();
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
    getAllSubGroupForCombo(groupId);
    $("#grp").on("onChanged", function () {
        var groupId = $(this).attr('value');
        getAllSubGroupForCombo(groupId);
    });
}

function close_insert_subgroup_window() {
    var insertwindow = $("#new-subgroup-window");
    insertwindow.css('display', 'none');

}
function getAllSubGroupForCombo(groupId) {
    var result = groups.f6(groupId);
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

//edit subgroup

function setup_edit_subgroup(item) {

    var edit = $(item).find('.edit');
    $(edit).click(function () {

        var itemID = $(this).attr('itemID');
        var result = groups.f9(itemID);
        if (result.result.code == 0) {
            editingsubAccessID = itemID;
            open_edit_subgroup_window(result.subGroup);

        }
        else {
            Siml.messages.add({ text: result.result.message, type: "error" });
            return;
        }
    });
}
function open_edit_subgroup_window(item) {

    var editwindow = $("#edit-subgroup-window");
    $("#input-title-edit-subgroup-window").val(item.title);
    $("#echk_canPush").prop('checked', item.canPush);
    $("#echk_toAll").prop('checked', item.toAll);
    $("#echk_toPartner").prop('checked', item.toPartner);
    $("#echk_toGroup").prop('checked', !item.toAll && !item.toPartner && item.canPush);

    editwindow.css('display', 'block');

    var result = groups.f1();
    if (result.result.code != 0) {
        Siml.messages.add({ text: result.result.message, type: "error" });
        return;
    }
    var grp = $("#egrp");
    var grp_combo = grp.parent();
    grp_combo.html('');
    var html = '<div id="egrp" class=" select">';
    $.each(result.groups, function (index, el) {
        html += '<span class="option" value="' + el.ID + '">' + el.title + '</span>';
    });
    html += "</div>";
    var c = $(html);
    $(grp_combo).html(html);
    $("#egrp").simlComboBox();
    var groupId = $("#egrp").attr('value');
    getAllSubGroupForCombo2(groupId);
    $("#egrp").on("onChanged", function () {
        var groupId = $(this).attr('value');
        getAllSubGroupForCombo2(groupId);
    });

    var llist = $("#esub-left");
    llist.html('');
    $.each(item.pushTo, function (index, el) {
        var html = $('<div class="record" itemid="' + el.subGroupID + '">'
            + '<span>' + el.groupTitle + " / " + el.subGroupTitle + '</span>'
            + '<label class="delete">X</label>'
            + '</div>');

        $(html).find('.delete').click(function () {
            $(this).parent().remove();
        });

        $(llist).append(html);
    });
    $(".check").change();
    $("#echk_canPush").change();
}
function setup_edit_subgroup_window() {

    $("#input-title-edit-subgroup-window").keyup(function (e) {
        if (e.keyCode == 13) {
            $("#btn-save-edit-subgroup-window").click();
        }
    });
    $(".check").change(function () {

        if ($("#echk_toGroup").is(':checked')) {
            $("#egroup-panel").slideDown(100);
        }
        else {
            $("#egroup-panel").slideUp(100);
        }
    });
    $("#echk_canPush").change(function () {

        if (this.checked) {
            $("#epushType").slideDown(100);
        }
        else {
            $("#epushType").slideUp(100);
        }
    });
    var btn_save_edit_subgroup = $("#btn-save-edit-subgroup");
    btn_save_edit_subgroup.click(function () {

        var subgroupId = $("#esgrp").attr('value');
        var groupTitle = $("#egrp").find('.selected').html();
        var subgroupTitle = $("#esgrp").find('.selected').html();

        if (subgroupId == -1 || subgroupId == "-1") {
            Siml.messages.add({ text: "زیرگروهی انتخاب نشده است", type: "error" });
            return;
        }
        var canInsert = true;

        $.each($("#esub-left").find('.record'), function (index, el) {

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

            $("#esub-left").append(html);
        }

    });
    var btn_save_edit_subgroup_window = $("#btn-save-edit-subgroup-window");
    btn_save_edit_subgroup_window.click(function () {

        var title = $("#input-title-edit-subgroup-window").val();
        var canPush = $("#echk_canPush").attr('checked') == "checked";
        var toAll = $("#echk_toAll").attr('checked') == "checked";
        var toPartner = $("#echk_toPartner").attr('checked') == "checked";
        var toGroup = $("#echk_toGroup").attr('checked') == "checked";
        var pushTo = [];

        var groupID = $("#list").find('.selected').attr('itemid');
        if (canPush && toGroup) {
            $.each($("#esub-left").find('.record'), function (index, el) {

                pushTo.push(parseInt($(el).attr('itemid')));

            });
        }
        var result = groups.f8(editingsubAccessID,title, canPush, toAll, toPartner, pushTo);
        if (result.code != 0) {
            Siml.messages.add({ text: result.message, type: "error" });
            return;
        }

        Siml.messages.add({ text: result.message, type: "success" });

        getAllSubGroup(groupID);
        close_edit_subgroup_window();

    });
    var btn_close_edit_subgroup_window = $("#btn-close-edit-subgroup-window");
    btn_close_edit_subgroup_window.click(function () {


        close_edit_subgroup_window();
    });
}
function close_edit_subgroup_window() {
    var insertwindow = $("#edit-subgroup-window");
    insertwindow.css('display', 'none');

}
function getAllSubGroupForCombo2(groupId) {
    var result = groups.f6(groupId);
    if (result.result.code != 0) {
        Siml.messages.add({ text: result.result.message, type: "error" });
        return;
    }
    var sgrp = $("#esgrp");
    var sgrp_combo = sgrp.parent();
    sgrp_combo.html('');
    var shtml = '<div id="esgrp" class=" select">';

    $.each(result.subGroup, function (index, el) {
        shtml += '<span class="option" value="' + el.ID + '">' + el.title + '</span>';
    });
    shtml += "</div>";

    $(sgrp_combo).html(shtml);
    $("#esgrp").simlComboBox();
}

//delete subgroup

function setup_delete_subgroup(item) {

    var del = $(item).find('.delete');
    $(del).click(function () {
        if (confirm('آیا از حذف این زیرگروه اطمینان دارید؟')) {
            var itemID = $(this).attr('itemID');
            var result = groups.f10(itemID);
            if (result.code == 0) {
                Siml.messages.add({ text: result.message, type: "success" });
                var groupID = $("#list").find('.selected').attr('itemid');
                getAllSubGroup(groupID);
            }
            else {
                Siml.messages.add({ text: result.message, type: "error" });
                return;
            }
        }
    });
}