var permi;
var datetime;
$(function () {
    dashboard_permission();
    Tipped.create('.row');
    $('#daily-date').datepicker({ isRTL: false, showButtonPanel: true, autoSize: true });
    user_count();
    ungroup_user_count();
    partner_count();
    user_list();
    getdatetime();


    getCount(datetime, "news");
    getCount(datetime, "event");
    getCount(datetime, "io");
    getCount(datetime, "pub");
    getCount(datetime, "bestidea");
    getCount(datetime, "creativity");
    getCount(datetime, "myiran");


    $('.inp-pick').addClass('visi');
    $('#newsmonthpicker').Monthpicker();
    $("#newscontainer").find(".monthpicker_input").html(datetime);
    $('#newscontainer').find('.month').bind('click', function () {
        var date = $("#newscontainer").find(".monthpicker_input").eq(0).text();
        getCount(date, "news");
    });

    $('#eventmonthpicker').Monthpicker();
    $("#eventcontainer").find(".monthpicker_input").html(datetime);
    $('#eventcontainer').find('.month').bind('click', function () {
        var date = $("#eventcontainer").find(".monthpicker_input").eq(0).text();
        getCount(date, "event");
    });

    $('#iomonthpicker').Monthpicker();
    $("#iocontainer").find(".monthpicker_input").html(datetime);
    $('#iocontainer').find('.month').bind('click', function () {
        var date = $("#iocontainer").find(".monthpicker_input").eq(0).text();
        getCount(date, "io");
    });

    $('#pubmonthpicker').Monthpicker();
    $("#pubcontainer").find(".monthpicker_input").html(datetime);
    $('#pubcontainer').find('.month').bind('click', function () {
        var date = $("#pubcontainer").find(".monthpicker_input").eq(0).text();
        getCount(date, "pub");
    });
    
    $('#bestideamonthpicker').Monthpicker();
    $("#bestideacontainer").find(".monthpicker_input").html(datetime);
    $('#bestideacontainer').find('.month').bind('click', function () {
        var date = $("#bestideacontainer").find(".monthpicker_input").eq(0).text();
        getCount(date, "bestidea");
    });

    $('#creativitymonthpicker').Monthpicker();
    $("#creativitycontainer").find(".monthpicker_input").html(datetime);
    $('#creativitycontainer').find('.month').bind('click', function () {
        var date = $("#creativitycontainer").find(".monthpicker_input").eq(0).text();
        getCount(date, "creativity");
    });

    $('#myiranmonthpicker').Monthpicker();
    $("#myirancontainer").find(".monthpicker_input").html(datetime);
    $('#myirancontainer').find('.month').bind('click', function () {
        var date = $("#myirancontainer").find(".monthpicker_input").eq(0).text();
        getCount(date, "myiran");
    });

    $('.count').click(function () {
        $(this).parent().find('.recentlylist').slideToggle(100);
    });
   
});
function user_list() {


    getAllUser("");

    $('#daily-date-list').on('change', function () {

        getAllUser($(this).val());

    });
}
function getAllUser(date) {

    var rows = $("#rows");
    if (permi.user.access.showlist) {
        $('#daily-date-list').datepicker({ isRTL: false, showButtonPanel: true, autoSize: true });
        var result = dashboard.f4(date);
        if (result.result.code != 0) {
            Siml.message.add({ text: result.result.message, type: "error" });
            return;
        }

        $('#daily-date-list').val(result.date);
        rows.html('');
        $.each(result.user, function (index, row) {
            var classtype = row.isPartner == true ? "<div class='row part' data-tipped-options='position: " + '"right"' + "' title='همکار'>" : "<div class='row' data-tipped-options='position: " + '"right"' + "' title='گروه بندی نشده'>";
            var html = $(
                    classtype
                    + '<span class="info">' + row.info + '</span>'
                    + '<span class="tell">' + row.tell + '</span>'
                    + '</div>'
                );
            rows.append(html);
        });
        Tipped.create('.row');
    }
}
function user_count() {

    var result = dashboard.f1("");
    if (result.result.code != 0) {
        Siml.message.add({ text: result.result.message, type: "error" });
        return;
    }
    $("#user-count").html(result.number);
    $('#daily-date').val(result.date);

    $('#daily-date').on('change', function () {

        var result = dashboard.f1($(this).val());
        if (result.result.code != 0) {
            Siml.message.add({ text: result.result.message, type: "error" });
            return;
        }
        $("#user-count").html(result.number);

    });
}
function ungroup_user_count() {

    var result = dashboard.f2();
    if (result.result.code != 0) {
        Siml.message.add({ text: result.result.message, type: "error" });
        return;
    }
    $("#ungroup-user-count").html(result.number);
}
function partner_count() {

    var result = dashboard.f3();
    if (result.result.code != 0) {
        Siml.message.add({ text: result.result.message, type: "error" });
        return;
    }
    $("#partner-count").html(result.number);
}
function dashboard_permission() {
    var ac = Siml.Access.get('dashboard.aspx');

    if (ac.result.code != 0) {
        Siml.messages.add({ text: ac.result.message, type: "error" });
        return;
    }
    else {
        var per = ac.access;
        permi = per;

        if (!per.relation) {
            $(".divcontainer").addClass('div-disable');
        }

        if (!per.user.access.showlist) {
            $(".daily-user-list").addClass('icon-disable');
        }

        if (!per.news.access.showlist) {
            $("#newsbtn").addClass('icon-disable').removeAttr('href');
        } else if (!per.news.access.insert) {
            $("#newsbtn").addClass('icon-disable').removeAttr('href');
        }


        if (!per.events.access.showlist) {
            $("#eventbtn").addClass('icon-disable').removeAttr('href');
        } else if (!per.events.access.insert) {
            $("#eventbtn").addClass('icon-disable').removeAttr('href');
        }


        if (!per.io.access.showlist) {
            $("#iobtn").addClass('icon-disable').removeAttr('href');
        } else if (!per.io.access.insert) {
            $("#iobtn").addClass('icon-disable').removeAttr('href');
        }


        if (!per.publication.access.showlist) {
            $("#pubbtn").addClass('icon-disable').removeAttr('href');
        } else if (!per.publication.access.insert) {
            $("#pubbtn").addClass('icon-disable').removeAttr('href');
        }

        if (!per.chart.access.showlist) {
            $("chartbtn").addClass('icon-disable').removeAttr('href');
        } else if (!per.chart.access.insert) {
            $("#chartbtn").addClass('icon-disable').removeAttr('href');
        }


        if (!per.download.access.showlist) {
            $("#downloadbtn").addClass('icon-disable').removeAttr('href');
        } else if (!per.download.access.insert) {
            $("#downloadbtn").addClass('icon-disable').removeAttr('href');
        }

      

    }

}
function getdatetime() {
    var date = $('#daily-date').val();
    var array = date.split('/');

    var year = array[0];
    var monthnumber = array[1];

    var month = "";
    switch (monthnumber) {
        case "1":
            month = "فروردین";
            break;
        case "2":
            month = "اردیبهشت";
            break;
        case "3":
            month = "خرداد";
            break;
        case "4":
            month = "تیر";
            break;
        case "5":
            month = "مرداد";
            break;
        case "6":
            month = "شهریور";
            break;
        case "7":
            month = "مهر";
            break;
        case "8":
            month = "آبان";
            break;
        case "9":
            month = "آذر";
            break;
        case "10":
            month = "دی";
            break;
        case "11":
            month = "بهمن";
            break;
        case "12":
            month = "اسفند";
    }
    datetime = month + ' ' + year;
}

function getCount(date,content) {

    var result = dashboard.f5(date, content);
    if (result.result.code != 0) {
        Siml.message.add({ text: result.result.message, type: "error" });
        return;
    }
    $("#" + content  + "count").html(result.number);
}

