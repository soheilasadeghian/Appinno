var PageIndex = 1;
var permission;
var editingAccessID = -1;

$(function () {
    access_permission();

    $("#parts").simlComboBox();
    $("#action").simlComboBox();
    $('#parts').on('onChanged', function () {

        type = $(this).attr('value');
        if (type == "") {
            load_combo_actions(false);

        }
        else {
            load_combo_actions(true);
        }

    });

    PageIndex = 1;
    getAllAccess();
    search();
    setup_access_window();
});

function search() {
    $('#fillter-input').keyup(function (e) {
        if (e.keyCode == 13) {
            $("#search-btn").click();
        }
    });

    var item = $("#search-btn");
    item.click(function () {
        PageIndex = 1;
        getAllAccess();

    });
}

function load_combo_actions(all) {
    var item = $("#action").parent();
    item.html('');
    if (all) {
        var html =
                    '<div id="action" class=" select">'
                    + '<span class="option" value="">همه</span>'
                    + '<span class="option" value="full">دسترسی کامل</span>'
                    + '<span class="option" value="showlist">نمایش لیست</span>'
                    + '<span class="option" value="insert">درج</span>'
                    + '<span class="option" value="edit">ویرایش</span>'
                    + '<span class="option" value="delete">حذف</span>'
                    + '</div>';
        item.html(html);

    }
    else {
        var html =
            '<div id="action" class=" select">'
            + '<span class="option" value="">همه</span>'
            + '</div>';
        item.html(html);
    }
    $("#action").simlComboBox();
}
function getAllAccess() {

    var fillterString = $("#fillter-input").val();
    var part = $("#parts").attr('value');
    var action = $('#action').attr('value');

    var result = paccess.f1(fillterString, part, action, PageIndex);
    if (result.result.code == 0) {

        var items = result.access;
        $("#total-count").html("( " + result.totalCount + " )");
        var list = $("#list");
        $(list).html('');
        $.each(items, function (index, el) {
            var canDetele = permission.access.access.delete ? '<a itemid="' + el.ID + '" class="delete"></a>' : "";
            var canEdit = permission.access.access.edit ? '<a itemid="' + el.ID + '" class="edit" title="ویرایش"></a>' : "";
            var html =
                        $('<div class="item" itemid="' + el.ID + '">'
                        + '<div class="tools">'
                        + canDetele
                        + canEdit
                        + '</div>'
                        + '<span class="title">' + el.title + '</span>'
                        + '</div>');
            $(list).append(html);
            setup_edit_access(html);
            setup_delete_access(html);
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
                    getAllAccess();
                }
            });
        }
    }
}

function access_permission() {
    var ac = Siml.Access.get('dashboard.aspx');

    if (ac.result.code != 0) {
        Siml.messages.add({ text: ac.result.message, type: "error" });
        return;
    }
    else {
        var per = ac.access;
        permission = ac.access
        if (!per.access.access.insert) {
            $("#insert-btn").remove();
            return;
        }
        $("#insert-btn").click(function () {

            //var insertwindow = $("#new-access-window");
            //insertwindow.css('display', 'block');

            if (permission.access.access.insert)
                open_insert_access_window();
            else {
                Siml.messages.add({ text: "شما دسترسی لازم جهت درج نقش جدید را ندارید", type: "error" });
            }
        });
    }
}

function setup_access_window() {
    $(document).keyup(function (e) {
        if (e.keyCode == 27) {
            close_insert_access_window();
            close_edit_access_window();
        }
        if (e.keyCode == 45) {
            if (permission.access.access.insert)
                open_insert_access_window();
        }
    });

    setup_insert_access_window();
    setup_edit_access_window();
}

function set_access_tick_check(This) {
    $(This).click(function () {
        if ($(this).hasClass('access')) {
            if ($(this).hasClass('showlist')) {
                $(this).parent().parent().find('.check').removeClass('access');
                $(this).parent().parent().find('.check').addClass('deny');

            }
            else if ($(this).hasClass('full')) {
                $(this).parent().parent().find('.check').removeClass('access');
                $(this).parent().parent().find('.check').addClass('deny');
            }
            else {
                $(this).removeClass('access');
                $(this).addClass('deny');
                $(this).parent().parent().find('.check.full').removeClass('access');
                $(this).parent().parent().find('.check.full').addClass('deny');
            }

        } else if ($(this).hasClass('deny')) {
            if ($(this).hasClass('full')) {
                $(this).parent().parent().find('.check').removeClass('deny');
                $(this).parent().parent().find('.check').addClass('access');

            }
            else {
                $(this).removeClass('deny');
                $(this).addClass('access');
                $(this).parent().parent().find('.check.showlist').addClass('access');
                $(this).parent().parent().find('.check.showlist').removeClass('deny');
            }
        }

    });
}

function open_insert_access_window() {
    var insertwindow = $("#new-access-window");
    var title = $("#input-title-insert-access-window");
    title.val('');
    $(title).focus();
    $('.new-access-window').find('.check').removeClass('access');
    $('.new-access-window').find('.check').addClass('deny');
    insertwindow.css('display', 'block');

    var currentAccess = permission;

    var rows = $(insertwindow).find('.row');

    $.each(rows, function (index, row) {

        var checks = $(row).find('.check');
        var full = 0;
        $.each(checks, function (innerIndex, chk) {
            $(chk).removeClass('access');
            $(chk).removeClass('deny');
            if (innerIndex == 4) {

                if (full == 4) {
                    set_access_tick_check($(chk));
                    $(chk).addClass('access');
                }
                else {
                    $(chk).addClass('deny');
                }
            }

            if (innerIndex == 0)//showlist
            {
                if (index == 0) {//news
                    if (currentAccess.news.access.showlist) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 1) {//event
                    if (currentAccess.events.access.showlist) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 2) {//io
                    if (currentAccess.io.access.showlist) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 3) {//pub
                    if (currentAccess.publication.access.showlist) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 4) {//chart
                    if (currentAccess.chart.access.showlist) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 5) {//download
                    if (currentAccess.download.access.showlist) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 6) {//grouping
                    if (currentAccess.grouping.access.showlist) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 7) {//users
                    if (currentAccess.user.access.showlist) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 8) {//access
                    if (currentAccess.access.access.showlist) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 9) {//operators
                    if (currentAccess.operators.access.showlist) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 10) {//ican
                    if (currentAccess.ican.access.showlist) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 11) {//poll
                    if (currentAccess.poll.access.showlist) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 12) {//bestIdeasCompetition
                    if (currentAccess.bestIdeasCompetition.access.showlist) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 13) {//creativityCompetition
                    if (currentAccess.creativityCompetition.access.showlist) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 14) {//myIranCompetition
                    if (currentAccess.myIranCompetition.access.showlist) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
            }
            else if (innerIndex == 1)//insert
            {
                if (index == 0) {//news
                    if (currentAccess.news.access.insert) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 1) {//event
                    if (currentAccess.events.access.insert) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 2) {//io
                    if (currentAccess.io.access.insert) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 3) {//pub
                    if (currentAccess.publication.access.insert) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 4) {//chart
                    if (currentAccess.chart.access.insert) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 5) {//download
                    if (currentAccess.download.access.insert) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 6) {//grouping
                    if (currentAccess.grouping.access.insert) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 7) {//users
                    if (currentAccess.user.access.insert) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 8) {//access
                    if (currentAccess.access.access.insert) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 9) {//operators
                    if (currentAccess.operators.access.insert) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 10) {//ican
                    if (currentAccess.ican.access.insert) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 11) {//poll
                    if (currentAccess.poll.access.insert) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 12) {//bestIdeasCompetition
                    if (currentAccess.bestIdeasCompetition.access.insert) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 13) {//creativityCompetition
                    if (currentAccess.creativityCompetition.access.insert) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 14) {//myIranCompetition
                    if (currentAccess.myIranCompetition.access.insert) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
            }
            else if (innerIndex == 2)//delete
            {
                if (index == 0) {//news
                    if (currentAccess.news.access.delete) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 1) {//event
                    if (currentAccess.events.access.delete) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 2) {//io
                    if (currentAccess.io.access.delete) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 3) {//pub
                    if (currentAccess.publication.access.delete) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 4) {//chart
                    if (currentAccess.chart.access.delete) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 5) {//download
                    if (currentAccess.download.access.delete) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 6) {//grouping
                    if (currentAccess.grouping.access.delete) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 7) {//users
                    if (currentAccess.user.access.delete) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 8) {//access
                    if (currentAccess.access.access.delete) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 9) {//operators
                    if (currentAccess.operators.access.delete) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 10) {//ican
                    if (currentAccess.ican.access.delete) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 11) {//poll
                    if (currentAccess.poll.access.delete) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 12) {//bestIdeasCompetition
                    if (currentAccess.bestIdeasCompetition.access.delete) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 13) {//creativityCompetition
                    if (currentAccess.creativityCompetition.access.delete) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 14) {//myIranCompetition
                    if (currentAccess.myIranCompetition.access.delete) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
            }
            else if (innerIndex == 3)//edit
            {
                if (index == 0) {//news
                    if (currentAccess.news.access.edit) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 1) {//event
                    if (currentAccess.events.access.edit) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 2) {//io
                    if (currentAccess.io.access.edit) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 3) {//pub
                    if (currentAccess.publication.access.edit) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 4) {//chart
                    if (currentAccess.chart.access.edit) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 5) {//download
                    if (currentAccess.download.access.edit) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 6) {//grouping
                    if (currentAccess.grouping.access.edit) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 7) {//users
                    if (currentAccess.user.access.edit) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 8) {//access
                    if (currentAccess.access.access.edit) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 9) {//operators
                    if (currentAccess.operators.access.edit) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 10) {//ican
                    if (currentAccess.ican.access.edit) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 11) {//poll
                    if (currentAccess.poll.access.edit) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 12) {//bestIdeasCompetition
                    if (currentAccess.bestIdeasCompetition.access.edit) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 13) {//creativityCompetition
                    if (currentAccess.creativityCompetition.access.edit) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 14) {//myIranCompetition
                    if (currentAccess.myIranCompetition.access.edit) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
            }
        });

    });

}

function setup_edit_windows_for_tick_check() {
    var editwindow = $("#edit-access-window");

    var currentAccess = permission;

    var rows = $(editwindow).find('.row');
    $(rows).find(".check").unbind( "click" );

    $.each(rows, function (index, row) {

        var checks = $(row).find('.check');
        var full = 0;
        $.each(checks, function (innerIndex, chk) {
            $(chk).removeClass('access');
            $(chk).removeClass('deny');
            if (innerIndex == 4) {

                if (full == 4) {
                    set_access_tick_check($(chk));
                    $(chk).addClass('access');
                }
                else {
                    $(chk).addClass('deny');
                }
            }

            if (innerIndex == 0)//showlist
            {
                if (index == 0) {//news
                    if (currentAccess.news.access.showlist) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 1) {//event
                    if (currentAccess.events.access.showlist) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 2) {//io
                    if (currentAccess.io.access.showlist) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 3) {//pub
                    if (currentAccess.publication.access.showlist) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 4) {//chart
                    if (currentAccess.chart.access.showlist) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 5) {//download
                    if (currentAccess.download.access.showlist) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 6) {//grouping
                    if (currentAccess.grouping.access.showlist) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 7) {//users
                    if (currentAccess.user.access.showlist) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 8) {//access
                    if (currentAccess.access.access.showlist) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 9) {//operators
                    if (currentAccess.operators.access.showlist) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 10) {//ican
                    if (currentAccess.ican.access.showlist) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 11) {//poll
                    if (currentAccess.poll.access.showlist) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 12) {//bestIdeasCompetition
                    if (currentAccess.bestIdeasCompetition.access.showlist) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 13) {//creativityCompetition
                    if (currentAccess.creativityCompetition.access.showlist) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 14) {//myIranCompetition
                    if (currentAccess.myIranCompetition.access.showlist) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
            }
            else if (innerIndex == 1)//insert
            {
                if (index == 0) {//news
                    if (currentAccess.news.access.insert) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 1) {//event
                    if (currentAccess.events.access.insert) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 2) {//io
                    if (currentAccess.io.access.insert) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 3) {//pub
                    if (currentAccess.publication.access.insert) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 4) {//chart
                    if (currentAccess.chart.access.insert) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 5) {//download
                    if (currentAccess.download.access.insert) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 6) {//grouping
                    if (currentAccess.grouping.access.insert) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 7) {//users
                    if (currentAccess.user.access.insert) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 8) {//access
                    if (currentAccess.access.access.insert) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 9) {//operators
                    if (currentAccess.operators.access.insert) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 10) {//ican
                    if (currentAccess.ican.access.insert) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 11) {//poll
                    if (currentAccess.poll.access.insert) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 12) {//bestIdeasCompetition
                    if (currentAccess.bestIdeasCompetition.access.insert) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 13) {//creativityCompetition
                    if (currentAccess.creativityCompetition.access.insert) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 14) {//myIranCompetition
                    if (currentAccess.myIranCompetition.access.insert) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
            }
            else if (innerIndex == 2)//delete
            {
                if (index == 0) {//news
                    if (currentAccess.news.access.delete) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 1) {//event
                    if (currentAccess.events.access.delete) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 2) {//io
                    if (currentAccess.io.access.delete) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 3) {//pub
                    if (currentAccess.publication.access.delete) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 4) {//chart
                    if (currentAccess.chart.access.delete) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 5) {//download
                    if (currentAccess.download.access.delete) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 6) {//grouping
                    if (currentAccess.grouping.access.delete) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 7) {//users
                    if (currentAccess.user.access.delete) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 8) {//access
                    if (currentAccess.access.access.delete) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 9) {//operators
                    if (currentAccess.operators.access.delete) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 10) {//ican
                    if (currentAccess.ican.access.delete) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 11) {//poll
                    if (currentAccess.poll.access.delete) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 12) {//bestIdeasCompetition
                    if (currentAccess.bestIdeasCompetition.access.delete) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 13) {//creativityCompetition
                    if (currentAccess.creativityCompetition.access.delete) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 14) {//myIranCompetition
                    if (currentAccess.myIranCompetition.access.delete) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
            }
            else if (innerIndex == 3)//edit
            {
                if (index == 0) {//news
                    if (currentAccess.news.access.edit) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 1) {//event
                    if (currentAccess.events.access.edit) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 2) {//io
                    if (currentAccess.io.access.edit) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 3) {//pub
                    if (currentAccess.publication.access.edit) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 4) {//chart
                    if (currentAccess.chart.access.edit) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 5) {//download
                    if (currentAccess.download.access.edit) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 6) {//grouping
                    if (currentAccess.grouping.access.edit) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 7) {//users
                    if (currentAccess.user.access.edit) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 8) {//access
                    if (currentAccess.access.access.edit) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 9) {//operators
                    if (currentAccess.operators.access.edit) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 10) {//ican
                    if (currentAccess.ican.access.edit) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 11) {//poll
                    if (currentAccess.poll.access.edit) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 12) {//bestIdeasCompetition
                    if (currentAccess.bestIdeasCompetition.access.edit) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 13) {//creativityCompetition
                    if (currentAccess.creativityCompetition.access.edit) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
                else if (index == 14) {//myIranCompetition
                    if (currentAccess.myIranCompetition.access.edit) { $(chk).addClass('access'); full++; set_access_tick_check($(chk)); } else $(chk).addClass('deny');
                }
            }
        });

    });
}

function close_insert_access_window() {
    var insertwindow = $("#new-access-window");
    insertwindow.css('display', 'none');

}

function setup_insert_access_window() {

    var title = $("#input-title-insert-access-window");
    title.keyup(function (e) {
        if (e.keyCode == 13) {
            $("#btn-save-insert-access-window").click();
        }

    });
    var btn_close_insert_access_window = $("#btn-close-insert-access-window");
    btn_close_insert_access_window.click(function () {

        close_insert_access_window();
    });

    var btn_save_insert_access_window = $("#btn-save-insert-access-window");
    btn_save_insert_access_window.click(function () {

        var title = $("#input-title-insert-access-window");
        var titleText = title.val();

        if (titleText == "") {
            Siml.messages.add({ text: "عنوان نقش را وارد کنید", type: "error" });
            return;
        }

        var rows = $('.new-access-window').find('.row');

        var news = new Object(), events = new Object(), io = new Object()
            , pub = new Object(), chart = new Object(), download = new Object()
            , grouping = new Object(), users = new Object(), access = new Object(), operators = new Object(), ican = new Object(), poll = new Object(), bestIdeasCompetition = new Object(), creativityCompetition = new Object(), myIranCompetition = new Object();

        $.each(rows, function (index, row) {
            var del, insert, edit, showlist;

            var checks = $(row).find('.check');
            $.each(checks, function (innerIndex, chk) {

                if (innerIndex == 0)//showlist
                {
                    showlist = $(chk).hasClass('access');
                }
                else if (innerIndex == 1)//insert
                {
                    insert = $(chk).hasClass('access');
                }
                else if (innerIndex == 2)//delete
                {
                    del = $(chk).hasClass('access');
                }
                else if (innerIndex == 3)//edit
                {
                    edit = $(chk).hasClass('access');
                }
            });
            if (index == 0) {//news

                news.insert = insert;
                news.edit = edit;
                news.delete = del;
                news.showlist = showlist;
            }
            else if (index == 1) {//event
                events.showlist = showlist;
                events.insert = insert;
                events.edit = edit;
                events.delete = del;
            }
            else if (index == 2) {//io
                io.showlist = showlist;
                io.insert = insert;
                io.edit = edit;
                io.delete = del;
            }
            else if (index == 3) {//pub
                pub.showlist = showlist;
                pub.insert = insert;
                pub.edit = edit;
                pub.delete = del;
            }
            else if (index == 4) {//chart
                chart.showlist = showlist;
                chart.insert = insert;
                chart.edit = edit;
                chart.delete = del;
            }
            else if (index == 5) {//download
                download.showlist = showlist;
                download.insert = insert;
                download.edit = edit;
                download.delete = del;
            }
            else if (index == 6) {//grouping
                grouping.showlist = showlist;
                grouping.insert = insert;
                grouping.edit = edit;
                grouping.delete = del;
            }
            else if (index == 7) {//users
                users.showlist = showlist;
                users.insert = insert;
                users.edit = edit;
                users.delete = del;
            }
            else if (index == 8) {//access
                access.showlist = showlist;
                access.insert = insert;
                access.edit = edit;
                access.delete = del;
            }
            else if (index == 9) {//operators
                operators.showlist = showlist;
                operators.insert = insert;
                operators.edit = edit;
                operators.delete = del;
            }
            else if (index == 10) {//ican
                ican.showlist = showlist;
                ican.insert = insert;
                ican.edit = edit;
                ican.delete = del;
            }
            else if (index == 11) {//poll
                poll.showlist = showlist;
                poll.insert = insert;
                poll.edit = edit;
                poll.delete = del;
            }
            else if (index == 12) {//bestIdeasCompetition
                bestIdeasCompetition.showlist = showlist;
                bestIdeasCompetition.insert = insert;
                bestIdeasCompetition.edit = edit;
                bestIdeasCompetition.delete = del;
            }
            else if (index == 13) {//creativityCompetition
                creativityCompetition.showlist = showlist;
                creativityCompetition.insert = insert;
                creativityCompetition.edit = edit;
                creativityCompetition.delete = del;
            }
            else if (index == 14) {//myIranCompetition
                myIranCompetition.showlist = showlist;
                myIranCompetition.insert = insert;
                myIranCompetition.edit = edit;
                myIranCompetition.delete = del;
            }
        });

        var res = paccess.f2(titleText, news, events, io, pub, chart, download, grouping, users, access, operators, ican, poll, bestIdeasCompetition, creativityCompetition, myIranCompetition)

        if (res.code == 0) {

            Siml.messages.add({ text: res.message, type: "success" });
            close_insert_access_window();

            getAllAccess();
        }
        else {
            Siml.messages.add({ text: res.message, type: "error" });
            return;
        }

    });

}

function setup_edit_access(item) {

    var edit = $(item).find('.edit');
    $(edit).click(function () {

        var itemID = $(this).attr('itemID');

        var result = paccess.f4(itemID);
        if (result.result.code == 0) {
            editingAccessID = itemID;
            open_edit_access_window(result);
        }
        else {
            Siml.messages.add({ text: result.result.message, type: "error" });
            return;
        }
    });
}
function setup_delete_access(item) {

    var del = $(item).find('.delete');
    $(del).click(function () {
        if (confirm('آیا از حذف این نقش اطمینان دارید؟')) {
            var itemID = $(this).attr('itemID');
            var result = paccess.f5(itemID);
            if (result.result.code == 0) {
                getAllAccess();
            }
            else {
                Siml.messages.add({ text: result.result.message, type: "error" });
                return;
            }
        }
    });
}

function open_edit_access_window(currentAccess) {
    setup_edit_windows_for_tick_check();

    var editwindow = $("#edit-access-window");
    var title = $("#input-title-edit-access-window");
    title.val(currentAccess.access.title);
    title.focus();

    var rows = $(editwindow).find('.row');

    $.each(rows, function (index, row) {

        var checks = $(row).find('.check');
        var full = 0;
        $.each(checks, function (innerIndex, chk) {
            $(chk).removeClass('access');
            $(chk).removeClass('deny');
            if (innerIndex == 4) {

                if (full == 4) {
                    $(chk).addClass('access');
                }
                else {
                    $(chk).addClass('deny');
                }
            }

            if (innerIndex == 0)//showlist
            {
                if (index == 0) {//news
                    if (currentAccess.access.per.news.access.showlist) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 1) {//event
                    if (currentAccess.access.per.events.access.showlist) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 2) {//io
                    if (currentAccess.access.per.io.access.showlist) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 3) {//pub
                    if (currentAccess.access.per.publication.access.showlist) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 4) {//chart
                    if (currentAccess.access.per.chart.access.showlist) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 5) {//download
                    if (currentAccess.access.per.download.access.showlist) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 6) {//grouping
                    if (currentAccess.access.per.grouping.access.showlist) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 7) {//users
                    if (currentAccess.access.per.user.access.showlist) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 8) {//access
                    if (currentAccess.access.per.access.access.showlist) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 9) {//operators
                    if (currentAccess.access.per.operators.access.showlist) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 10) {//ican
                    if (currentAccess.access.per.ican.access.showlist) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 11) {//poll
                    if (currentAccess.access.per.poll.access.showlist) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 12) {//bestIdeasCompetition
                    if (currentAccess.access.per.bestIdeasCompetition.access.showlist) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 13) {//creativityCompetition
                    if (currentAccess.access.per.creativityCompetition.access.showlist) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 14) {//myIranCompetition
                    if (currentAccess.access.per.myIranCompetition.access.showlist) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
            }
            else if (innerIndex == 1)//insert
            {
                if (index == 0) {//news
                    if (currentAccess.access.per.news.access.insert) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 1) {//event
                    if (currentAccess.access.per.events.access.insert) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 2) {//io
                    if (currentAccess.access.per.io.access.insert) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 3) {//pub
                    if (currentAccess.access.per.publication.access.insert) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 4) {//chart
                    if (currentAccess.access.per.chart.access.insert) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 5) {//download
                    if (currentAccess.access.per.download.access.insert) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 6) {//grouping
                    if (currentAccess.access.per.grouping.access.insert) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 7) {//users
                    if (currentAccess.access.per.user.access.insert) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 8) {//access
                    if (currentAccess.access.per.access.access.insert) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 9) {//operators
                    if (currentAccess.access.per.operators.access.insert) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 10) {//ican
                    if (currentAccess.access.per.ican.access.insert) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 11) {//poll
                    if (currentAccess.access.per.poll.access.insert) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 12) {//bestIdeasCompetition
                    if (currentAccess.access.per.bestIdeasCompetition.access.insert) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 13) {//creativityCompetition
                    if (currentAccess.access.per.creativityCompetition.access.insert) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 14) {//myIranCompetition
                    if (currentAccess.access.per.myIranCompetition.access.insert) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
            }
            else if (innerIndex == 2)//delete
            {
                if (index == 0) {//news
                    if (currentAccess.access.per.news.access.delete) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 1) {//event
                    if (currentAccess.access.per.events.access.delete) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 2) {//io
                    if (currentAccess.access.per.io.access.delete) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 3) {//pub
                    if (currentAccess.access.per.publication.access.delete) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 4) {//chart
                    if (currentAccess.access.per.chart.access.delete) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 5) {//download
                    if (currentAccess.access.per.download.access.delete) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 6) {//grouping
                    if (currentAccess.access.per.grouping.access.delete) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 7) {//users
                    if (currentAccess.access.per.user.access.delete) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 8) {//access
                    if (currentAccess.access.per.access.access.delete) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 9) {//operators
                    if (currentAccess.access.per.operators.access.delete) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 10) {//ican
                    if (currentAccess.access.per.ican.access.delete) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 11) {//poll
                    if (currentAccess.access.per.poll.access.delete) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 12) {//bestIdeasCompetition
                    if (currentAccess.access.per.bestIdeasCompetition.access.delete) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 13) {//creativityCompetition
                    if (currentAccess.access.per.creativityCompetition.access.delete) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 14) {//myIranCompetition
                    if (currentAccess.access.per.myIranCompetition.access.delete) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
            }
            else if (innerIndex == 3)//edit
            {
                if (index == 0) {//news
                    if (currentAccess.access.per.news.access.edit) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 1) {//event
                    if (currentAccess.access.per.events.access.edit) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 2) {//io
                    if (currentAccess.access.per.io.access.edit) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 3) {//pub
                    if (currentAccess.access.per.publication.access.edit) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 4) {//chart
                    if (currentAccess.access.per.chart.access.edit) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 5) {//download
                    if (currentAccess.access.per.download.access.edit) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 6) {//grouping
                    if (currentAccess.access.per.grouping.access.edit) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 7) {//users
                    if (currentAccess.access.per.user.access.edit) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 8) {//access
                    if (currentAccess.access.per.access.access.edit) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 9) {//operators
                    if (currentAccess.access.per.operators.access.edit) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 10) {//ican
                    if (currentAccess.access.per.ican.access.edit) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 11) {//poll
                    if (currentAccess.access.per.poll.access.edit) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 12) {//bestIdeasCompetition
                    if (currentAccess.access.per.bestIdeasCompetition.access.edit) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 13) {//creativityCompetition
                    if (currentAccess.access.per.creativityCompetition.access.edit) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
                else if (index == 14) {//myIranCompetition
                    if (currentAccess.access.per.myIranCompetition.access.edit) { $(chk).addClass('access'); full++; } else $(chk).addClass('deny');
                }
            }
        });

    });

    editwindow.css('display', 'block');

}

function close_edit_access_window() {
    var editwindow = $("#edit-access-window");
    editwindow.css('display', 'none');

}

function setup_edit_access_window() {
    var title = $("#input-title-edit-access-window");
    title.keyup(function (e) {
        if (e.keyCode == 13) {
            $("#btn-save-edit-access-window").click();
        }

    });
    var btn_close_edit_access_window = $("#btn-close-edit-access-window");
    btn_close_edit_access_window.click(function () {

        close_edit_access_window();
    });

    var btn_save_edit_access_window = $("#btn-save-edit-access-window");
    btn_save_edit_access_window.click(function () {

        var title = $("#input-title-edit-access-window");
        var titleText = title.val();

        if (titleText == "") {
            Siml.messages.add({ text: "عنوان نقش را وارد کنید", type: "error" });
            return;
        }

        var rows = $('#edit-access-window').find('.row');

        var news = new Object(), events = new Object(), io = new Object()
            , pub = new Object(), chart = new Object(), download = new Object()
            , grouping = new Object(), users = new Object(), access = new Object(), operators = new Object(), ican = new Object(), poll = new Object(), bestIdeasCompetition = new Object(), creativityCompetition = new Object(), myIranCompetition = new Object();

        $.each(rows, function (index, row) {
            var del, insert, edit, showlist;

            var checks = $(row).find('.check');
            $.each(checks, function (innerIndex, chk) {

                if (innerIndex == 0)//showlist
                {
                    showlist = $(chk).hasClass('access');
                }
                else if (innerIndex == 1)//insert
                {
                    insert = $(chk).hasClass('access');
                }
                else if (innerIndex == 2)//delete
                {
                    del = $(chk).hasClass('access');
                }
                else if (innerIndex == 3)//edit
                {
                    edit = $(chk).hasClass('access');
                }
            });
            if (index == 0) {//news

                news.insert = insert;
                news.edit = edit;
                news.delete = del;
                news.showlist = showlist;
            }
            else if (index == 1) {//event
                events.showlist = showlist;
                events.insert = insert;
                events.edit = edit;
                events.delete = del;
            }
            else if (index == 2) {//io
                io.showlist = showlist;
                io.insert = insert;
                io.edit = edit;
                io.delete = del;
            }
            else if (index == 3) {//pub
                pub.showlist = showlist;
                pub.insert = insert;
                pub.edit = edit;
                pub.delete = del;
            }
            else if (index == 4) {//chart
                chart.showlist = showlist;
                chart.insert = insert;
                chart.edit = edit;
                chart.delete = del;
            }
            else if (index == 5) {//download
                download.showlist = showlist;
                download.insert = insert;
                download.edit = edit;
                download.delete = del;
            }
            else if (index == 6) {//grouping
                grouping.showlist = showlist;
                grouping.insert = insert;
                grouping.edit = edit;
                grouping.delete = del;
            }
            else if (index == 7) {//users
                users.showlist = showlist;
                users.insert = insert;
                users.edit = edit;
                users.delete = del;
            }
            else if (index == 8) {//access
                access.showlist = showlist;
                access.insert = insert;
                access.edit = edit;
                access.delete = del;
            }
            else if (index == 9) {//operators
                operators.showlist = showlist;
                operators.insert = insert;
                operators.edit = edit;
                operators.delete = del;
            }
            else if (index == 10) {//ican
                ican.showlist = showlist;
                ican.insert = insert;
                ican.edit = edit;
                ican.delete = del;
            }
            else if (index == 11) {//poll
                poll.showlist = showlist;
                poll.insert = insert;
                poll.edit = edit;
                poll.delete = del;
            }
            else if (index == 12) {//bestIdeasCompetition
                bestIdeasCompetition.showlist = showlist;
                bestIdeasCompetition.insert = insert;
                bestIdeasCompetition.edit = edit;
                bestIdeasCompetition.delete = del;
            }
            else if (index == 13) {//creativityCompetition
                creativityCompetition.showlist = showlist;
                creativityCompetition.insert = insert;
                creativityCompetition.edit = edit;
                creativityCompetition.delete = del;
            }
            else if (index == 14) {//myIranCompetition
                myIranCompetition.showlist = showlist;
                myIranCompetition.insert = insert;
                myIranCompetition.edit = edit;
                myIranCompetition.delete = del;
            }
        });

        var res = paccess.f3(editingAccessID, titleText, news, events, io, pub, chart, download, grouping, users, access, operators, ican, poll, bestIdeasCompetition, creativityCompetition, myIranCompetition);

        if (res.code == 0) {
            Siml.messages.add({ text: res.message, type: "success" });
            close_edit_access_window();

            getAllAccess();
        }
        else {
            Siml.messages.add({ text: res.message, type: "error" });
            return;
        }

    });

}


