﻿var PageIndex = 1;
var editingAccessID = -1;

$(function () {

    $("#order").simlComboBox();
    $("#sortby").simlComboBox();
    $("#itemstate").simlComboBox();

    $('#date').datepicker({ isRTL: false, showButtonPanel: true });
    $('#dateto').datepicker({ isRTL: false, showButtonPanel: true });
    $('#pubdate').datepicker({ isRTL: false, showButtonPanel: true });
    $('#pubdateto').datepicker({ isRTL: false, showButtonPanel: true });

    getAllGroupForCombo();
    $("#lt").html("لیست سازمان های عمومی");
    PageIndex = 1;
    getAllItem();
    search();

});

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

    $("#pubdate").on('change', function () {
        if ($("#chk_pubdate").is(':checked') == false) {
            $("#date").val('');
        }
    });
    $("#pubdateto").on('change', function () {
        if ($("#chk_pubdateto").is(':checked') == false) {
            $("#dateto").val('');
        }
    });
    $("#chk_pubdate").on('change', function () {
        if ($("#chk_pubdate").is(':checked')) {
            $("#pubdate").trigger('focus');
        }
        else {
            $("#pubdate").val('');
        }
    });
    $("#chk_pubdateto").on('change', function () {
        if ($("#chk_pubdateto").is(':checked')) {
            $("#pubdateto").trigger('focus');
        }
        else {
            $("#pubdateto").val('');
        }
    });

    var item = $("#search-btn");
    item.click(function () {
        PageIndex = 1;
        getAllItem();

        if ($("#fchk_nogrp").is(':checked')) {
            $("#lt").html("لیست سازمان ها عمومی");

        }
        if ($("#fchk_allitem").is(':checked')) {
            $("#lt").html("لیست سازمان های گروه بندی شده");

        }

        if ($("#fchk_toGroup").is(':checked')) {
            $("#lt").html("لیست سازمان هایی که در گروه [ " + $("#group").find('.selected').html() + " ] و زیرگروه  [ " + $("#subgroup").find('.selected').html() + " ] قرار دارند");


        }
    });
    $(".check").on('change', function () {

        if ($("#fchk_nogrp").is(':checked')) {
            $("#fgroup_panel").slideUp(100);
        }


        if ($("#fchk_allitem").is(':checked')) {
            $("#fgroup_panel").slideUp(100);
        }


        if ($("#fchk_toGroup").is(':checked')) {
            $("#fgroup_panel").slideDown(100);
        }
        else {
            $("#fgroup_panel").slideUp(100);
           
        }

    });

}
function getAllItem() {
    var filter = $("#fillter-input").val();
    var subgroupID = $("#subgroup").attr('value');
    var order = $("#order").attr('value');
    var sortby = $("#sortby").attr('value');
    var date = $("#date").val();
    var dateto = $("#dateto").val();
    var pubdate = $("#pubdate").val();
    var pubdateto = $("#pubdateto").val();
    var itemstate = $("#itemstate").attr('value');

    var fchk_nogrp = $("#fchk_nogrp").attr('checked') == "checked";
    var fchk_allitem = $("#fchk_allitem").attr('checked') == "checked";
    var fchk_toGroup = $("#fchk_toGroup").attr('checked') == "checked";

    if (fchk_nogrp)
        subgroupID = -1;
    else if (fchk_allitem)
        subgroupID = -2;

    var result = iolist.f1(date, dateto, pubdate, pubdateto, itemstate, filter, subgroupID, order, sortby, PageIndex);
    if (result.result.code != 0) {
        Siml.messages.add({ text: result.result.message, type: "error" });
        return;
    }
    var list = $("#list");
    $("#total-count").html(" ( " + result.totalCount + " )");
    $(list).html('');
    $.each(result.io, function (index, el) {
        var block = el.isBlock ? '<a itemid="' + el.ID + '" class="block" title="سازمان مسدود شده است"></a>' : "";
        var html =
                    $('<div class="item" itemid="' + el.ID + '">'
                    + '<div class="tools">'
                    + '<a itemid="' + el.ID + '" class="delete"></a>'
                    + '<a  href="eio.aspx?id=' + el.ID + '" class="edit" title="ویرایش"></a>'
                    + block
                    + '</div>'
                    + '<span class="ptitle">' + el.title + '</span>'
                    + '<span class="text">' + el.publishDate + '</span>'
                    + '<span class="date">' + el.regDate + '</span>'
                    + '</div>');

        $(list).append(html);
        setup_delete_item(html);
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
                getAllItem();
            }
        });
    }
    $('#deleteall-btn').unbind();
    $('#deleteall-btn').click(function () {

        if (confirm('آیا از حذف تمام سازمان ها اطمینان دارید؟')) {
            var result = iolist.f5();
            if (result.code != 0) {
                Siml.messages.add({ text: result.message, type: "error" });
                return;
            }
            Siml.messages.add({ text: result.message, type: "success" });
            getAllItem();

        }

    });
}

function getAllGroupForCombo() {
    var result = iolist.f2();
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
        $("#lt").html("لیست سازمان هایی که در گروه [ " + $("#group").find('.selected').html() + " ] و زیرگروه  [ " + $("#subgroup").find('.selected').html() + " ] قرار دارند");
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
    var result = iolist.f3(groupId);
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

        $("#lt").html("لیست سازمان هایی که در گروه [ " + $("#group").find('.selected').html() + " ] و زیرگروه  [ " + $("#subgroup").find('.selected').html() + " ] قرار دارند");

    });
    $("#subgroup").trigger("onChanged");

}


//delete item
function setup_delete_item(item) {

    var deleteBtn = $(item).find('.delete');
    $(deleteBtn).click(function () {
        if (confirm("آیا از حذف این سازمان اطمینان دارید؟")) {
            var itemid = $(this).attr('itemid');
            var result = iolist.f4(itemid);
            if (result.code != 0) {
                Siml.messages.add({ text: result.message, type: "error" });
                return;
            }
            Siml.messages.add({ type: "success", text: result.message });
            getAllItem();
        }
    });
}
///


