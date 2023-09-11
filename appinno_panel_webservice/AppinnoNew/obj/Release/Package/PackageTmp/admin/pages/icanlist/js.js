var PageIndex = 1;
var editingAccessID = -1;

$(function () {

    $("#order").simlComboBox();
    $("#sortby").simlComboBox();
    $("#itemstate").simlComboBox();

    $('#date').datepicker({ isRTL: false, showButtonPanel: true });
    $('#dateto').datepicker({ isRTL: false, showButtonPanel: true });

    $("#lt").html("لیست توانایی های عمومی");
    PageIndex = 1;
    getAllItem();
    search();
    //setup_edit_user_window();

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

    var item = $("#search-btn");
    item.click(function () {
        PageIndex = 1;
        getAllItem();

        if ($("#fchk_nogrp").is(':checked')) {
            $("#lt").html("لیست توانایی های عمومی");
            $("#fgroup_panel").slideUp(100);
        }
        if ($("#fchk_allitem").is(':checked')) {
            $("#lt").html("لیست توانایی های گروه بندی شده");
            $("#fgroup_panel").slideUp(100);
        }

        if ($("#fchk_toGroup").is(':checked')) {
            $("#lt").html("لیست توانایی های که در گروه [ " + $("#group").find('.selected').html() + " ] و زیرگروه  [ " + $("#subgroup").find('.selected').html() + " ] قرار دارند");
            $("#fgroup_panel").slideDown(100);
        }
        else {
            $("#fgroup_panel").slideUp(100);
        }
        
    });
    $("#fchk_nogrp").on('change', function () {
        if ($("#fchk_nogrp").is(':checked')) {
            $("#fgroup_panel").slideUp(100);
        }
    });
    $("#fchk_allitem").on('change', function () {
        if ($("#fchk_allitem").is(':checked')) {
            $("#fgroup_panel").slideUp(100);
        }
    });
    $("#fchk_toGroup").on('change', function () {
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
    var itemstate = $("#itemstate").attr('value');

    var fchk_nogrp = $("#fchk_nogrp").attr('checked') == "checked";
    var fchk_allitem = $("#fchk_allitem").attr('checked') == "checked";
    var fchk_toGroup = $("#fchk_toGroup").attr('checked') == "checked";

    if (fchk_nogrp)
        subgroupID = -1;
    else if (fchk_allitem)
        subgroupID = -2;

    var result = icanlist.f1(date, dateto, itemstate, filter, order, sortby, PageIndex);
    if (result.result.code != 0) {
        Siml.messages.add({ text: result.result.message, type: "error" });
        return;
    }
    var list = $("#list");
    $("#total-count").html(" ( " + result.totalCount + " ) ");
    $(list).html('');
    $.each(result.Ican, function (index, el) {
        var block = el.isBlock ? '<a itemid="' + el.ID + '" class="block" title="توانایی مسدود شده است"></a>' : "";
        var html =
                    $('<div class="item" itemid="' + el.ID + '">'
                    + '<div class="tools">'
                    + '<a itemid="' + el.ID + '" class="delete"></a>'
                    + '<a href="eican.aspx?id=' + el.ID + '" class="edit" title="ویرایش"></a>'
                    + block
                    + '</div>'
                    + '<span class="ptitle">' + el.title + '</span>'
                    + '<span class="date">' + el.regDate + '</span>'
                    + '<span class="ptitle" style="width:130px;">' + el.userNameFamily + '</span>'
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

        if (confirm('آیا از حذف تمام توانایی ها اطمینان دارید؟')) {
            var result = icanlist.f5();
            if (result.code != 0) {
                Siml.messages.add({ text: result.message, type: "error" });
                return;
            }
            Siml.messages.add({ text: result.message, type: "success" });
            getAllItem();

        }

    });
}

//delete item
function setup_delete_item(item) {

    var deleteBtn = $(item).find('.delete');
    $(deleteBtn).click(function () {
        if (confirm("آیا از حذف این توانایی اطمینان دارید؟")) {
            var itemid = $(this).attr('itemid');
            var result = icanlist.f4(itemid);
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


