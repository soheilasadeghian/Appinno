var PageIndex = 1;
var permission;
var editingAccessID = -1;

$(function () {

    access_permission();

    $("#order").simlComboBox();
    getAllGroupForCombo();

    PageIndex = 1;
    getAllMessage();
    search();

    setup_new_message_window();
});

function access_permission() {
    var ac = Siml.Access.get('dashboard.aspx');

    if (ac.result.code != 0) {
        Siml.messages.add({ text: ac.result.message, type: "error" });
        return;
    }
    else {
        var per = ac.access;
        permission = ac.access;
    }
}
function search() {
    $('#fillter-input').keyup(function (e) {
        if (e.keyCode == 13) {
            $("#search-btn").click();
        }
    });
    $(".check").change(function () {

        if ($("#fchk_toAll").is(':checked')) {
            $("#lt").html("لیست پیام هایی که برای همه ارسال شده است");
        }
        if ($("#fchk_toPartner").is(':checked')) {
            $("#lt").html("لیست پیام هایی که برای همه همکاران ارسال شده است");
        }

        if ($("#fchk_toGroup").is(':checked')) {
            $("#lt").html("لیست پیام هایی که برای گروه [ " + $("#group").find('.selected').html() + " ] و زیرگروه  [ " + $("#subgroup").find('.selected').html() + " ] ارسال شده است");
            $("#fgroup_panel").slideDown(100);
        }
        else {
            $("#fgroup_panel").slideUp(100);
        }
    });
    var item = $("#search-btn");
    item.click(function () {
        PageIndex = 1;
        getAllMessage();
    });
}
function getAllMessage() {
    var filter = $("#fillter-input").val();
    var subgroupID = $("#subgroup").attr('value');
    var order = $("#order").attr('value');
    var fchk_toAll = $("#fchk_toAll").attr('checked') == "checked";
    var fchk_toPartner = $("#fchk_toPartner").attr('checked') == "checked";
    var fchk_toGroup = $("#fchk_toGroup").attr('checked') == "checked";

    if (!fchk_toGroup)
        subgroupID = -1;

    var result = message.f1(filter, fchk_toAll, fchk_toPartner, subgroupID, order, PageIndex);
    if (result.result.code != 0) {
        Siml.messages.add({ text: result.result.message, type: "error" });
        return;
    }
    $("#total-count").html(" ( " + result.totalCount + " )");
    var list = $("#list");
    $(list).html('');
    $.each(result.message, function (index, el) {
        var html =
                    $('<div class="item" itemid="' + el.ID + '">'
                    + '<div class="tools">'
                    + '<a itemid="' + el.ID + '" class="delete"></a>'
                    + '<a itemid="' + el.ID + '" class="edit" title="ویرایش"></a>'
                    + '</div>'
                    + '<span class="ptitle">' + el.title + '</span>'
                    + '<span class="text">' + el.text + '</span>'
                    + '<span class="date">' + el.regDate + '</span>'
                    + '</div>');

        $(list).append(html);
        setup_delete_message(html);
        setup_view_message(html);
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
                getAllMessage();
            }
        });
    }
    $('#deleteall-btn').unbind();
    $('#deleteall-btn').click(function () {

        if (confirm('آیا از حذف تمام پیام های ارسال شده اطمینان دارید؟')) {
            var result = message.f5();
            if (result.code != 0) {
                Siml.messages.add({ text: result.message, type: "error" });
                return;
            }
            Siml.messages.add({ text: result.message, type: "success" });
            getAllMessage();

        }

    });
}

function getAllGroupForCombo() {
    var result = message.f6();
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
    var result = message.f7(groupId);
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

        $("#lt").html("لیست پیام هایی که برای گروه [ " + $("#group").find('.selected').html() + " ] و زیرگروه  [ " + $("#subgroup").find('.selected').html() + " ] ارسال شده است");

    });
    $("#subgroup").trigger("onChanged");
}

///new message window

function setup_new_message_window() {
    $("#insert-btn").click(function () {
        open_new_message_window();
    });

    $("#btn-save-insert-push-window").click(function () {
        var title = $("#input-title-insert-push-window").val();
        var text = $("#input-text-insert-push-window").val();
        var toAll = $("#chk_toAll").attr('checked') == "checked";
        var toPartner = $("#chk_toPartner").attr('checked') == "checked";
        var toGroup = $("#chk_toGroup").attr('checked') == "checked";
        var messageTo = [];

        var groupID = $("#list").find('.selected').attr('itemid');
        if (toGroup) {
            $.each($("#sub-left").find('.record'), function (index, el) {

                messageTo.message(parseInt($(el).attr('itemid')));

            });
        }
        var result = message.f2(title, text, toAll, toPartner, messageTo);
        if (result.code != 0) {
            Siml.messages.add({ text: result.message, type: "error" });
            return;
        }

        Siml.messages.add({ text: result.message, type: "success" });

        getAllMessage();
        close_new_message_window();

    });
    $("#btn-close-insert-push-window").click(function () {
        close_new_message_window();
    });
    $(".check").change(function () {

        if ($("#chk_toGroup").is(':checked')) {
            $("#group-panel").slideDown(100);
        }
        else {
            $("#group-panel").slideUp(100);
        }
    });
    var btn_save_insert_message = $("#btn-save-insert-push");
    btn_save_insert_message.click(function () {

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

}
function getAllSubGroupForCombo2(groupId) {
    var result = message.f7(groupId);
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
function open_new_message_window() {

    $('#newpushwindow').show();

    var result = message.f6();
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
}
function close_new_message_window() {
    $('#newpushwindow').css('display', 'none');
}

//delete message
function setup_delete_message(item) {

    var deleteBtn = $(item).find('.delete');
    $(deleteBtn).click(function () {
        if (confirm("آیا از حذف این پیام اطمینان دارید؟")) {
            var itemid = $(this).attr('itemid');
            var result = message.f4(itemid);
            if (result.code != 0) {
                Siml.messages.add({ text: result.message, type: "error" });
                return;
            }
            Siml.messages.add({ type: "success", text: result.message });
            getAllMessage();
        }
    });
}

/// view message
function setup_view_message(item) {
    var editBtn = $(item).find('.edit');
    $(editBtn).click(function () {
        var itemid = $(this).attr('itemid');
        var result = message.f3(itemid);
        if (result.result.code != 0) {
            Siml.messages.add({ text: result.result.message, type: "error" });
            return;
        }

        $("#input-title-view-push-window").val(result.Message.title);
        $("#input-text-view-push-window").val(result.Message.text);
        $("#input-date-view-push-window").val(result.Message.regDate);

        if (result.Message.attachment != null) {
            $("#image-title").css('display', 'block');
            $("#input-image-view-push-window").html('<img class="image" src="/uploads/message/' + result.Message.attachment + '"  />');
        }
        else {
            $("#image-title").css('display', 'none');
        }


        $("#viewpushwindow").css('display', 'block');
        if (result.Message.toPartner) {
            $("#viewpushType").html('این پیام برای <span style="color:#ff0000">همه همکاران</span> ارسال شده است');
        }
        else if (result.Message.toAll) {
            $("#viewpushType").html('این پیام برای <span style="color:#ff0000">همه</span> ارسال شده است');
        }
        else {
            $("#viewpushType").html('این پیام برای <span style="color:#ff0000">گروه های زیر</span> ارسال شده است');
            var list = $("#viewpushTo");
            list.html('');
            $.each(result.pushTo, function (index, el) {
                var record = "<span>" + el.groupTitle + " / " + el.subGroupTitle + "</span>";
                list.append(record);
            });
            list.append('<div class="clear" />');
        }
       
        //اگر آقای یوسف زاده تایید کردند که ارسال پیام باید با تایید مدیر باشد آنگاه کامنت این قسمت را برمیداریم.
        //if (result.Message.isBlock != 0) {
        //    var el = $('<input id="btn_sendmessage" itemid="' + result.Message.ID + '" type="button" class="save" style="float: right; width: 60px;font-size: 0.8em;text-align: right;" value="ارسال پیام"/>');
        //    $("#sendmessage").html(el);
        //    sendmessage(el);
        //}
        //else {
        //    $("#btn_sendmessage").css('display', 'none');
        //}

    });

    $("#btn-view-insert-push-window").click(function () {
        $("#viewpushwindow").css('display', 'none');
    });
    
}
function sendmessage(This) {
    $("#btn_sendmessage").click(function () {
        var itemid = $(This).attr('itemid');

        if (confirm('آیا می خواهید پیام مورد نظر را ارسال نمایید؟')) {
            var result = message.f8(itemid);
            if (result.code != 0) {
                Siml.messages.add({ text: result.message, type: "error" });
                return;
            }
            $("#btn_sendmessage").css('display', 'none');
            Siml.messages.add({ text: result.message, type: "success" });
            
          }
    });
}