var PageIndex = 1;
var editingAccessID = -1;
var curID;

$(function () {

    $("#order").simlComboBox();
    $("#sortby").simlComboBox();
    $("#itemstate").simlComboBox();
    $("#itemstatus").simlComboBox();

    $('#date').datepicker({ isRTL: false, showButtonPanel: true });
    $('#dateto').datepicker({ isRTL: false, showButtonPanel: true });
    $('#startDate').datepicker({ isRTL: false, showButtonPanel: true });
    $('#startDateto').datepicker({ isRTL: false, showButtonPanel: true });
    $('#endDate').datepicker({ isRTL: false, showButtonPanel: true });
    $('#endDateto').datepicker({ isRTL: false, showButtonPanel: true });

    $("#lt").html("لیست مسابقات");

    
    PageIndex = 1;
    getAllItem();
    setup_insert_para();
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

    $("#startdate").on('change', function () {
        if ($("#chk_startdate").is(':checked') == false) {
            $("#startdate").val('');
        }
    });
    $("#startdateto").on('change', function () {
        if ($("#chk_startdateto").is(':checked') == false) {
            $("#startdateto").val('');
        }
    });
    $("#chk_startdate").on('change', function () {
        if ($("#chk_startdate").is(':checked')) {
            $("#startdate").trigger('focus');
        }
        else {
            $("#startdate").val('');
        }
    });
    $("#chk_startdateto").on('change', function () {
        if ($("#chk_startdateto").is(':checked')) {
            $("#startdateto").trigger('focus');
        }
        else {
            $("#startdateto").val('');
        }
    });

    $("#enddate").on('change', function () {
        if ($("#chk_enddate").is(':checked') == false) {
            $("#enddate").val('');
        }
    });
    $("#enddateto").on('change', function () {
        if ($("#chk_enddateto").is(':checked') == false) {
            $("#enddateto").val('');
        }
    });
    $("#chk_enddate").on('change', function () {
        if ($("#chk_enddate").is(':checked')) {
            $("#enddate").trigger('focus');
        }
        else {
            $("#enddate").val('');
        }
    });
    $("#chk_enddateto").on('change', function () {
        if ($("#chk_enddateto").is(':checked')) {
            $("#endtdateto").trigger('focus');
        }
        else {
            $("#enddateto").val('');
        }
    });


    var item = $("#search-btn");
    item.click(function () {
        PageIndex = 1;
        getAllItem();
    });
}

function getAllItem() {
    var filter = $("#fillter-input").val();
    var order = $("#order").attr('value');
    var sortby = $("#sortby").attr('value');
    var date = $("#date").val();
    var dateto = $("#dateto").val();
    var startdate = $("#startDate").val();
    var startdateto = $("#startDateto").val();
    var enddate = $("#endDate").val();
    var enddateto = $("#endDateto").val();
    var itemstate = $("#itemstate").attr('value');
    var itemstatus = $("#itemstatus").attr('value');

    var result = myIranlist.f1(date, dateto, startdate, startdateto, enddate, enddateto, itemstate, itemstatus, filter, order, sortby, PageIndex);

    if (result.result.code != 0) {
        Siml.messages.add({ text: result.result.message, type: "error" });
        return;
    }
    var list = $("#list");
    $(list).html('');
    $("#total-count").html(" ( " + result.totalCount + " )");

    $.each(result.myIran, function (index, el) {
        var block = el.isBlock ? '<a itemid="' + el.ID + '" class="block" title="مسابقه مسدود شده است"></a>' : '<a itemid="' + el.ID + '" class="unblock" title="مسابقه درحال نمایش است"></a>';
        if (el.status == "ثبت شده")
            var html =
                    $('<div class="item" itemid="' + el.ID + '">'
                    + '<div class="tools">'
                    + '<a  href="emyIran.aspx?id=' + el.ID + '" class="edit" title="ویرایش"></a>'
                    + block
                    + '</div>'
                    + '<a href="responselist.aspx?id=' + el.ID + '" class="ptitle" style="text-decoration: none;color: black;">' + el.title + '</a>'
                    + '<span class="text">' + el.status + '</span>'
                    + '<span class="date">' + el.regDate + '</span>'
                    + '</div>');
        else if (el.status == "درحال برگزاری" && el.isSendNotification == false)
            var html =
                    $('<div class="item" itemid="' + el.ID + '">'
                    + '<div class="tools">'
                    + '<a  href="emyIran.aspx?id=' + el.ID + '& statu=e" class="edit" title="ویرایش"></a>'
                    + block
                    + '<a class="sendpush" data-id="' + el.ID + '" title="ارسال پیام شروع مسابقه">↑</a>'
                    + '</div>'
                    + '<a href="responselist.aspx?id=' + el.ID + '" class="ptitle" style="text-decoration: none;color: black;">' + el.title + '</a>'
                    + '<span class="text">' + el.status + '</span>'
                    + '<span class="date">' + el.regDate + '</span>'
                    + '</div>');
        else if (el.status == "درحال برگزاری" && el.isSendNotification == true)
            var html =
                    $('<div class="item" itemid="' + el.ID + '">'
                    + '<div class="tools">'
                    + '<a  href="emyIran.aspx?id=' + el.ID + '" class="edit" title="ویرایش"></a>'
                    + block
                    + '</div>'
                    + '<a href="responselist.aspx?id=' + el.ID + '" class="ptitle" style="text-decoration: none;color: black;">' + el.title + '</a>'
                    + '<span class="text">' + el.status + '</span>'
                    + '<span class="date">' + el.regDate + '</span>'
                    + '</div>');
        else
            var html =
                    $('<div class="item" itemid="' + el.ID + '">'
                    + '<div class="tools">'
                    + '<a  href="emyIran.aspx?id=' + el.ID + '" class="edit" title="ویرایش"></a>'
                    + block
                    + '</div>'
                    + '<a href="responselist.aspx?id=' + el.ID + '" class="ptitle" style="text-decoration: none;color: black;">' + el.title + '</a>'
                    + '<span class="text">' + el.status + '</span>'
                    + '<span class="date">' + el.regDate + '</span>'
                    + '</div>');

        $(list).append(html);
        setup_block_item(html);
        setup_unblock_item(html);
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
}

/// insert para
function setup_insert_para() {
    $(document).keyup(function (e) {
        if (e.keyCode == 27) {
            var window = $("#new-para-window");
            window.css('display', 'none');
        }
    });
    $(".sendpush").click(function () {
        curID = $(this);
        var window = $("#new-para-window");
        $("#input-text-insert-para-window").val('');
        window.css('display', 'block');
    });
    $("#btn-close-insert-para-window").click(function () {
        var window = $("#new-para-window");
        window.css('display', 'none');
    });
    $("#btn-save-insert-para-window").click(function () {
        var id = $(curID).attr("data-id");
        var text = $("#input-text-insert-para-window").val();
        if (text == "") {
            Siml.messages.add({ text: "متن پیام را وارد کنید.", type: "error" });
            return;
        }


        var result = myIranlist.f6(id, text);

        if (result.code != 0) {
            Siml.messages.add({ text: result.message, type: "error" });
            return;
        }

        $(curID).remove();
        var window = $("#new-para-window");
        window.css('display', 'none');
        Siml.messages.add({ text: "پیام با موفقیت ارسال شد.", type: "success" });
        return;

    });
}

//block item
function setup_block_item(item) {

    var blockBtn = $(item).find('.block');
    $(blockBtn).click(function () {
        if (confirm("آیا از خارج نمودن این مسابقه از حالت مسدود اطمینان دارید؟")) {
            var itemid = $(this).attr('itemid');
            var result = myIranlist.f7(itemid, false);
            if (result.code != 0) {
                Siml.messages.add({ text: result.message, type: "error" });
                return;
            }
            Siml.messages.add({ type: "success", text: result.message });
            getAllItem();
        }
    });
}

//unblock item
function setup_unblock_item(item) {

    var blockBtn = $(item).find('.unblock');
    $(blockBtn).click(function () {
        if (confirm("آیا از مسدود نمودن این مسابقه اطمینان دارید؟")) {
            var itemid = $(this).attr('itemid');
            var result = myIranlist.f7(itemid, true);
            if (result.code != 0) {
                Siml.messages.add({ text: result.message, type: "error" });
                return;
            }
            Siml.messages.add({ type: "success", text: result.message });
            getAllItem();
        }
    });
}