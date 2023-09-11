var itemID;
var currentItem;
var ID = 0;
var chartData = [];
var chartID = 0;
$(function () {
    ID = 0;
    Tipped.create('.hint,.tools .button');
    $('#date1').datepicker({ isRTL: false, showButtonPanel: true });
    $("#fchk_forall").on('change', function () {
        if ($("#fchk_forall").is(':checked')) {
            $("#group-panel").slideUp(120);
        }
    });
    $("#fchk_toGroup").on('change', function () {
        if ($("#fchk_toGroup").is(':checked')) {
            $("#group-panel").slideDown(120);
        }
    });
    $("#fchk_noTag").on('change', function () {
        if ($("#fchk_noTag").is(':checked')) {
            $("#tag-panel").slideUp(120);
        }
    });
    $("#fchk_hasTag").on('change', function () {
        if ($("#fchk_hasTag").is(':checked')) {
            $("#tag-panel").slideDown(120);
        }
    });
    setup_insert_image();
    setup_chart();
    setup_insert_para();
    setup_group();
    setup_tag();
    setup_add_group_subgroup();

    check_queryString();
    loadData();

    setup_save_all();
});

/// set currentItem
function check_queryString() {
    itemID = Siml.page.url.getParam('id');
    var int = parseInt(itemID, 10);

    if (itemID == undefined || isNaN(int)) {

        $('body').html("");
        Siml.messages.add({ type: "error", text: "صفحه مورد نظر نادرست فراخوانی شده است." });
        setTimeout(function myfunction() {
            Siml.page.url.goto('reportlist.aspx');
        }, 2500);

    }
    else {

        var result = ereport.f4(itemID);


        if (result.result.code != 0) {

            $('body').html("");
            Siml.messages.add({ type: "error", text: result.result.message });
            setTimeout(function myfunction() {
                Siml.page.url.goto('reportlist.aspx');
            }, 2500);
        }
        else {
            currentItem = result.report;
        }
    }
}
function loadData() {
    $("#main-title").val(currentItem.title);
    $('#date1').val(currentItem.publishDate);

    var list = $('#items');
    $(list).html('');
    $.each(currentItem.contents, function (index, el) {
        var record;

        if (el.type == 0) {
            record = $('<div class="item par" id="lp_' + el.ID + '" >'
                + el.value
                + '<input type="button" class="delete" title="حذف" value="x" /></div>');
            setup_delete_para_item(record);
            $(list).append(record);
        }
        else if (el.type == 1) {
            record = $(
                        '<div id="li_' + (el.ID) + '" class="item img" >'
                        + '<img class="image" src="' + el.value + '"  />'
                        + '<input type="button" class="delete" title="حذف" value="x" />'
                        + '</div>'
                        );
            setup_delete_image_item(record);
            $(list).append(record);
        }
        else if (el.type == 2) {
            if (el.minReport.type == "line") {
                chartID = Math.max(el.minReport.ID, chartID) + 1;
                var obj = new Object();
                obj.xtitle = el.minReport.xTitle;
                obj.ytitle = el.minReport.yTitle;
                obj.type = el.minReport.type;
                obj.id = "llinechart_" + el.minReport.ID;
                obj.data = [];
                $.each(el.minReport.chart, function (index, ele) {
                    var item = new Object();
                    item.title = ele.xTitle;
                    item.value = ele.yValue;
                    obj.data.push(item);
                });
                chartData.push(obj);
                
                var html = $('<div class="item chart" ><div id="llinechart_' + el.minReport.ID + '" style="width: 100%; height: 300px;"></div>' + '<input type="button" class="delete" title="حذف" value="x" />' + '</div>');
                $("#items").append(html);
                setup_delete_chart_item(html);
                var chart = setup_line_charts("llinechart_" + el.minReport.ID);
                chart.xAxis[0].setTitle({
                    text: el.minReport.xTitle
                });
                chart.yAxis[0].setTitle({
                    text: el.minReport.yTitle
                });

                if (obj.data.length > 0) {
                    var cats = [];
                    var vals = [];

                    $.each(obj.data, function (index, el) {
                        cats.push(el.title);
                        vals.push(parseFloat(el.value));
                    });

                }
                chart.xAxis[0].setCategories(cats, true);
                chart.series[0].setData(vals);

            } else if (el.minReport.type == "bar") {
                chartID = Math.max(el.minReport.ID, chartID) + 1;
                var obj = new Object();
                obj.xtitle = el.minReport.xTitle;
                obj.ytitle = el.minReport.yTitle;
                obj.type = el.minReport.type;
                obj.id = "lbarchart_" + el.minReport.ID;
                obj.data = [];
                $.each(el.minReport.chart, function (index, ele) {
                    var item = new Object();
                    item.title = ele.xTitle;
                    item.value = ele.yValue;
                    obj.data.push(item);
                });
                chartData.push(obj);

                var html = $('<div class="item chart" ><div id="lbarchart_' + el.minReport.ID + '" style="width: 100%; height: 300px;"></div>' + '<input type="button" class="delete" title="حذف" value="x" />' + '</div>');
                $("#items").append(html);
                setup_delete_chart_item(html);
                var chart = setup_bar_charts("lbarchart_" + el.minReport.ID);
                chart.xAxis[0].setTitle({
                    text: el.minReport.xTitle
                });
                chart.yAxis[0].setTitle({
                    text: el.minReport.yTitle
                });

                if (obj.data.length > 0) {
                    var cats = [];
                    var vals = [];
                    $.each(obj.data, function (index, el) {
                        cats.push(el.title);
                        vals.push(parseFloat(el.value));
                    });
                }
                chart.xAxis[0].setCategories(cats, true);
                chart.series[0].setData(vals);
            }
            else if (el.minReport.type == "pie") {
                chartID = Math.max(el.minReport.ID, chartID) + 1;
                var obj = new Object();
                obj.xtitle = el.minReport.xTitle;
                obj.ytitle = el.minReport.yTitle;
                obj.type = el.minReport.type;
                obj.id = "lpiechart_" + el.minReport.ID;
                obj.data = [];
                $.each(el.minReport.chart, function (index, ele) {
                    var item = new Object();
                    item.title = ele.xTitle;
                    item.value = ele.yValue;
                    obj.data.push(item);
                });
                chartData.push(obj);
                var html = $('<div class="item chart" ><div id="lpiechart_' + el.minReport.ID + '" style="width: 100%; height: 300px;"></div>' + '<input type="button" class="delete" title="حذف" value="x" />' + '</div>');
                $("#items").append(html);
                setup_delete_chart_item(html);
                var chart = setup_pie_charts("lpiechart_" + el.minReport.ID);

                chart.xAxis[0].setTitle({
                    text: el.minReport.xTitle
                });
                chart.yAxis[0].setTitle({
                    text: el.minReport.yTitle
                });

                if (obj.data.length > 0) {
                    var cats = [];
                    var vals = [];
                    $.each(obj.data, function (index, el) {

                        vals.push([el.title, parseFloat(el.value)]);

                    });
                }
                chart.series[0].setData(vals);
            }
        }


    });

    $("#fchk_block").prop("checked", currentItem.isBlock);

    if (currentItem.groups.length == 0) {
        $("#fchk_forall").prop("checked", true);
    } else {

        $("#fchk_toGroup").prop("checked", true);
        $("#fchk_toGroup").trigger('change');

        $.each(currentItem.groups, function (index, el) {

            var html = $('<div class="record" itemid="' + el.subGroupID + '">'
                + '<span>' + el.groupTitle + " / " + el.subGroupTitle + '</span>'
                + '<label class="delete">X</label>'
                + '</div>');

            $(html).find('.delete').click(function () {
                $(this).parent().remove();
            });

            $("#sub-left").append(html);
        });
    }
    if (currentItem.tag.length == 0) {
        $("#fchk_noTag").prop("checked", true);
    } else {

        $("#fchk_hasTag").prop("checked", true);
        $("#fchk_hasTag").trigger('change');

        $.each(currentItem.tag, function (index, el) {

            var html = $('<div class="record" itemid="' + el.ID + '">'
                + '<span>' + el.title + '</span>'
                + '<label class="delete">X</label>'
                + '</div>');

            $(html).find('.delete').click(function () {
                $(this).parent().remove();
            });

            $("#sub-left-tag").append(html);
        });
    }
}
function setup_delete_image_item(item) {
    $(item).find(".delete").click(function () {
        var id = $(this).parent().attr('id').replace('li_', "").replace('i_', "");
        $("#u_" + id).remove();
        $(item).remove();
    });
}
function setup_delete_video_item(item) {

    $(item).find(".delete").click(function () {
        var id = $(this).parent().attr('id').replace('lv_', "").replace('v_', "");
        $("#u_" + id).remove();
        $(item).remove();
    });
}
function setup_delete_para_item(item) {
    $(item).find(".delete").click(function () {
        var id = $(this).parent().attr('id').replace('lp_', "").replace('p_', "");
        $("#u_" + id).remove();
        $(item).remove();
    });
}

/// insert image
function setup_insert_image() {
    image_initOnChange();
}
function image_initOnChange() {
    document.getElementById('file_image').addEventListener('change', null, false);
    document.getElementById('file_image').addEventListener('change', function (e) {

        if (this.disabled)
            return alert('فایل پشتیبانی نمی شود!');
        var F = this.files;

        if (F && F[0]) for (var i = 0; i < F.length; i++) {
            ID++;
            Siml.page.readImage(F[i], function (result) {

                var html = $(
                    '<div id="i_' + ID + '" class="item img" >'
                    + '<img class="image" src="' + result.src + '"  />'
                    + '<input type="button" class="delete" title="حذف" value="x" />'
                    + '</div>'
                    );

                var uploaded = $('#uploaded');

                var pfi = $("#file_image").parent();

                $("#file_image").attr('id', id = "u_" + ID);

                pfi.append('<input class="file_image" type="file" style="display: none" id="file_image" />');

                $(uploaded).append($("#file_image"));

                var list = $('#items');
                image_initOnChange();
                $(list).append(html);
                setup_delete_image_item(html);
            });
        }
    }, false);
}

/// insert chart
function setup_chart() {

    $("#file_chart").click(function () {
        open_chart_window();
    });
    $("#btn-close-insert-chart-window").click(function () {
        close_chart_window();
    });

    $("#btn-upload-chart").click(function () {

        $("#file-x").click();

    });

    $("#btn-save-insert-chart-window").click(function () {
        var x_title = $("#input-xtitle-chart-window").val();
        var y_value = $("#input-ytitle-chart-window").val();
        if (x_title == "" || y_value == "") {
            Siml.messages.add({ type: "error", text: "پرکردن هردو عنوان محورها الزامی است" });
            return;
        }
        var list = $(".row");
        var data = [];
        var can = true;
        $.each(list, function (index, el) {
            var item = new Object();
            item.title = $(el).find(".xtitle").val();
            item.value = $(el).find(".yvalue").val();

            if (isNaN(parseFloat(item.value))) {
                Siml.messages.add({ type: "error", text: "محور عمودی باید مقدار عددی داشته باشد" });
                can = false;
                return;
            }

            data.push(item);
        });
        if (!can) return;

        if ($("#rdo-line").is(":checked") == true) {

            chartID++;
            var html = $('<div class="item chart" ><div id="linechart_' + chartID + '" style="width: 100%; height: 300px;"></div>' + '<input type="button" class="delete" title="حذف" value="x" />' + '</div>');
            $("#items").append(html);
            setup_delete_chart_item(html);
            var chart = setup_line_charts("linechart_" + chartID);

            chart.xAxis[0].setTitle({
                text: x_title
            });
            chart.yAxis[0].setTitle({
                text: y_value
            });

            if (data.length > 0) {
                var cats = [];
                var vals = [];
                var obj = new Object();
                obj.xtitle = x_title;
                obj.ytitle = y_value;
                obj.type = "line";
                obj.id = "linechart_" + chartID;
                obj.data = [];
                $.each(data, function (index, el) {
                    cats.push(el.title);
                    vals.push(parseFloat(el.value));
                    obj.data.push(el);

                });
                chartData.push(obj);
            }
            chart.xAxis[0].setCategories(cats, true);
            chart.series[0].setData(vals);
            close_chart_window();
        }
        else if ($("#rdo-bar").is(":checked") == true) {

            chartID++;
            var html = $('<div class="item chart" ><div id="barchart_' + chartID + '" style="width: 100%; height: 300px;"></div>' + '<input type="button" class="delete" title="حذف" value="x" />' + '</div>');
            $("#items").append(html);
            setup_delete_chart_item(html);
            var chart = setup_bar_charts("barchart_" + chartID);

            chart.xAxis[0].setTitle({
                text: x_title
            });
            chart.yAxis[0].setTitle({
                text: y_value
            });

            if (data.length > 0) {
                var cats = [];
                var vals = [];
                var obj = new Object();
                obj.xtitle = x_title;
                obj.ytitle = y_value;
                obj.type = "bar";
                obj.id = "barchart_" + chartID;
                obj.data = [];
                $.each(data, function (index, el) {
                    cats.push(el.title);
                    vals.push(parseFloat(el.value));

                    obj.data.push(el);

                });
                chartData.push(obj);
            }
            chart.xAxis[0].setCategories(cats, true);
            chart.series[0].setData(vals);
            close_chart_window();
        }
        else if ($("#rdo-pie").is(":checked") == true) {

            chartID++;
            var html = $('<div class="item chart" ><div id="piechart_' + chartID + '" style="width: 100%; height: 300px;"></div>' + '<input type="button" class="delete" title="حذف" value="x" />' + '</div>');
            $("#items").append(html);
            setup_delete_chart_item(html);
            var chart = setup_pie_charts("piechart_" + chartID);

            chart.xAxis[0].setTitle({
                text: x_title
            });
            chart.yAxis[0].setTitle({
                text: y_value
            });

            if (data.length > 0) {
                var cats = [];
                var vals = [];
                var obj = new Object();
                obj.xtitle = x_title;
                obj.ytitle = y_value;
                obj.type = "pie";
                obj.id = "piechart_" + chartID;
                obj.data = [];
                $.each(data, function (index, el) {

                    vals.push([el.title, parseFloat(el.value)]);

                    obj.data.push(el);

                });
                chartData.push(obj);
            }
            chart.series[0].setData(vals);
            close_chart_window();
        }
    });

    file_initOnChange();
}
function setup_delete_chart_item(item) {
    $(item).find('.delete').click(function () {
        var id = $(this).parent().find('div').attr('id');

        chartData = chartData.filter(function (i) {
            return i.id != id;
        });

        $(this).parent().remove();
    });

}
function setup_yvalues() {

    $('.xtitle').unbind();
    $('.xtitle').keydown(function (e) {
        if (e.keyCode == 13) {
            $(this).parent().find('.yvalue').focus();
        }
    });
    $('.yvalue').unbind();
    $('.yvalue').keydown(function (e) {
        if (e.keyCode == 13) {
            if (isNaN(parseFloat($(this).val()))) {
                Siml.messages.add({ type: "error", text: "محور عمودی باید مقدار عددی داشته باشد" });
                this.select();
                return;
            }
            var html = $(
                    '<div class="row">'
                    + '<span class="delete">x</span>'
                    + '<input class="xtitle" type="text" name="name" value="" />'
                    + '<input class="yvalue" type="text" name="name" value="" />'
                    + '</div>'
                );

            $("#values").append(html);
            $(html).find('.xtitle').focus();
            $(html).find('.delete').click(function () {
                var del = $(".row").length - 1 == 0 ? false : true;
                if (del)
                    $(this).parent().remove();
                else {
                    $(this).parent().remove();

                    close_chart_window();
                }
            });
            setup_yvalues();
        }
    });


}
function file_initOnChange() {
    $('#file-x').unbind();
    document.getElementById('file-x').addEventListener('change', function (e) {

        var fileName = e.target.files[0].name;
        var ext = fileName.substring(fileName.lastIndexOf('.'), fileName.length);
        var size = Math.floor(e.target.files[0].size / 1024);

        if (this.disabled)
            return alert('فایل پشتیبانی نمی شود!');
        var F = this.files;


        $("#bar").css('width', '0%');
        $("#bar").unbind();
        $("#bar").on("onFinished_upload_excel", function (event, result) {

            $.each(result, function (index, el) {
                var html = $(
               '<div class="row">'
               + '<span class="delete">x</span>'
               + '<input class="xtitle" type="text" name="name" value="' + el.xtitle + '" />'
               + '<input class="yvalue" type="text" name="name" value="' + el.yvalue + '" />'
               + '</div>'
           );

                $("#values").append(html);
                $(html).find('.xtitle').focus();
                $(html).find('.delete').click(function () {
                    var del = $(".row").length - 1 == 0 ? false : true;
                    if (del)
                        $(this).parent().remove();
                    else {
                        $(this).parent().remove();
                        close_chart_window();
                    }
                });


            });
            setup_yvalues();
            var p = $("#file-x").parent();
            $("#file-x").remove();
            p.append('<input type="file" style="display: none" id="file-x" />');
            file_initOnChange();
            $("#upload-news-window").css('display', 'none');
        });

        $("#uploading-title").html("درحال ارسال فایل نمودار...");
        $("#upload-news-window").css('display', 'block');
        var form = new FormData();
        form.append("excel", F[0]);
        ereport.f5(form, $("#progress"), $("#bar"));

    }, false);
}
function close_chart_window() {
    $("#values").html('');
    $("#new-chart-window").css('display', 'none');
}
function open_chart_window() {
    var html = $(
           '<div class="row">'
           + '<span class="delete">x</span>'
           + '<input class="xtitle" type="text" name="name" value="" />'
           + '<input class="yvalue" type="text" name="name" value="" />'
           + '</div>'
       );

    $("#values").append(html);
    $(html).find('.xtitle').focus();
    $(html).find('.delete').click(function () {
        var del = $(".row").length - 1 == 0 ? false : true;
        if (del)
            $(this).parent().remove();
        else {
            $(this).parent().remove();
            close_chart_window();
        }
    });

    setup_yvalues();
    $("#new-chart-window").css('display', 'block');
}

function setup_pie_charts(elementID) {
    piechart = new Highcharts.Chart({
        chart: {
            renderTo: elementID,
            defaultSeriesType: 'pie'
        },
        tooltip: {
            useHTML: true,
            pointFormat: '{point.percentage:.1f}%</b>'
        },
        title: { text: "" },

        xAxis: {
            title: { text: 'محور افقی' }
        },
        yAxis: {
            title: {
                text: 'محور عمودی'
            }
        },
        series: [{
            data: []
        }]
    });
    return piechart;
}
function setup_bar_charts(elementID) {
    barchart = new Highcharts.Chart({
        chart: {
            renderTo: elementID,
            defaultSeriesType: 'column'
        },
        tooltip: {
            //pointFormat: '{point.percentage:.1f}%</b>'
            useHTML: true,
            formatter: function () {
                return 'مقدار برای  <b>' + this.x + '</b> برابر است با <b>' + this.y + '</b>';
            }
        },
        title: { text: "" },

        xAxis: {
            categories: [],
            title: { text: 'محور افقی' }
        },
        yAxis: {
            title: {
                text: 'محور عمودی'
            }
        },
        series: [{
            data: []
        }]
    });
    return barchart;

}

function setup_line_charts(elementID) {
    var linechart = new Highcharts.Chart({
        chart: {
            renderTo: elementID,
            defaultSeriesType: 'line'
        },
        tooltip: {
            //pointFormat: '{point.percentage:.1f}%</b>'
            useHTML: true,
            formatter: function () {
                return 'مقدار برای  <b>' + this.x + '</b> برابر است با <b>' + this.y + '</b>';
            }
        },
        title: { text: "" },
        xAxis: {
            categories: [],
            title: { text: 'محور افقی' }
        },
        yAxis: {
            title: {
                text: 'محور عمودی'
            }
        },
        series: [{
            data: []
        }]
    });
    return linechart;
}

/// insert para
function setup_insert_para() {
    $(document).keyup(function (e) {
        if (e.keyCode == 27) {
            var window = $("#new-para-window");
            window.css('display', 'none');
        }
    });
    $("#add-para").click(function () {

        var window = $("#new-para-window");
        $("#input-title-insert-para-window").val('');
        $("#input-text-insert-para-window").val('');
        window.css('display', 'block');
        $("#input-title-insert-para-window").focus();
    });
    $("#btn-close-insert-para-window").click(function () {
        var window = $("#new-para-window");
        window.css('display', 'none');
    });
    $("#btn-save-insert-para-window").click(function () {
        var title = $("#input-title-insert-para-window").val();
        var text = $("#input-text-insert-para-window").val();
        if (title == "" && text == "") {
            Siml.messages.add({ text: "عنوان یا متن پاراگراف را وارد کنید.", type: "error" });
            return;
        }
        text = text.replace(/\n/gi, "<br /> \n");
        ID++;
        var html = $(
                    '<div id="p_' + ID + '" class="item par">'
                    + '<h2>' + title + '</h2>'
                    + '<p>' + text + '</p>'
                    + '<input type="button" name="" value="x" class="delete" />'
                    + '</div>'
                );
        var list = $('#items');
        $(list).append(html);
        setup_delete_para_item(html);

        var window = $("#new-para-window");
        window.css('display', 'none');

    });
}

//tag
function setup_tag() {
    var result = ereport.f6();
    if (result.result.code != 0) {
        Siml.messages.add({ text: result.result.message, type: "error" });
        return;
    }
    var grp = $("#tag");
    var grp_combo = grp.parent();
    grp_combo.html('');
    var html = '<div id="tag" class=" select">';
    $.each(result.tag, function (index, el) {
        html += '<span class="option" value="' + el.ID + '">' + el.title + '</span>';
    });
    html += "</div>";
    var c = $(html);
    $(grp_combo).html(html);
    $("#tag").simlComboBox();
    var groupId = $("#tag").attr('value');

    var btn_save_add_tag = $("#btn-save-add-tag");

    btn_save_add_tag.click(function () {

        var tagId = $("#tag").attr('value');
        var tagTitle = $("#tag").find('.selected').html();

        var canInsert = true;

        $.each($("#sub-left-tag").find('.record'), function (index, el) {

            if ($(el).attr('itemid') == tagId + "") {
                canInsert = false;
            }

        });
        if (canInsert) {
            var html = $('<div class="record" itemid="' + tagId + '">'
            + '<span>' + tagTitle + '</span>'
            + '<label class="delete">X</label>'
            + '</div>');

            $(html).find('.delete').click(function () {
                $(this).parent().remove();
            });

            $("#sub-left-tag").append(html);
        }

    });

}
//group subgroup
function setup_group() {
    var result = ereport.f2();
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
function getAllSubGroupForCombo(groupId) {
    var result = ereport.f3(groupId);
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
function setup_add_group_subgroup() {
    var btn_save_add_group = $("#btn-save-add-group");

    btn_save_add_group.click(function () {

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

function setup_save_all() {
    $("#btn-save-all").click(function () {
        var title = $("#main-title");
        var date = $("#date1");
        var tagstring = $("#tagstring");

        if (title.val() == "") {
            Siml.messages.add({ text: "عنوان اصلی نمودار را وارد کنید", type: "error" });
            return;
        }
        if (date.val() == "") {
            Siml.messages.add({ text: "تاریخ انتشار نمودار را وارد کنید", type: "error" });
            return;
        }

        var subgroups = [];
        var subs = $("#sub-left").find('.record');
        if ($("#fchk_forall").is(':checked') == false) {
            $.each(subs, function (index, el) {
                var itemID = $(el).attr('itemid');
                subgroups.push(itemID);
            });
        }
        var tags = [];
        var subs2 = $("#sub-left-tag").find('.record');
        if ($("#fchk_noTag").is(':checked') == false) {
            $.each(subs2, function (index, el) {
                var itemID = $(el).attr('itemid');
                tags.push(itemID);
            });
        }

        var form = new FormData();
        form.append("reportID", currentItem.ID);
        form.append("title", title.val());
        form.append("date", date.val());
        form.append("tagstring", tagstring.val());
        form.append("subgroups", JSON.stringify(subgroups));
        form.append("tags", JSON.stringify(tags));
        form.append("block", $("#fchk_block").is(':checked'));

        var its = $("#items").find('.item');
        if ($(its).length == 0) {
            Siml.messages.add({ text: "درج حداقل یک آیتم الزامی می باشد", type: "error" });
            return;
        }
        $.each(its, function (index, el) {

            if ($(el).hasClass("chart") && $(el).find('div').attr('id').indexOf('llinechart_') < 0 && $(el).find('div').attr('id').indexOf('lbarchart_') < 0 && $(el).find('div').attr('id').indexOf('lpiechart_') < 0) {
                var curchart;
                $.each(chartData, function (index, ch) {

                    if (ch.id == $(el).find('div').attr('id')) {
                        curchart = ch;

                    }

                });
                form.append("chart_" + index, JSON.stringify(curchart));
            }
            else if ($(el).hasClass("chart") && ($(el).find('div').attr('id').indexOf('llinechart_') >= 0 || $(el).find('div').attr('id').indexOf('lbarchart_') >= 0 || $(el).find('div').attr('id').indexOf('lpiechart_') >= 0)) {
                var curchart;
                $.each(chartData, function (index, ch) {

                    if (ch.id == $(el).find('div').attr('id')) {
                        curchart = ch;

                    }

                });
                form.append("lchart_" + index, curchart.id);
            }
            else if ($(el).hasClass("img") && $(el).attr('id').indexOf('li_') < 0) {
                var id = "u_" + $(el).attr('id').replace('i_', '');
                var file = document.getElementById(id).files[0];
                form.append("img_" + index, file);
            }
            else if ($(el).hasClass("img") && $(el).attr('id').indexOf('li_') >= 0) {
                var id = $(el).attr('id').replace('li_', '');
                form.append("limg_" + index, id);
            }
            else if ($(el).hasClass("par") && $(el).attr('id').indexOf('lp_') < 0) {
                var h = "<h2>" + $(el).find('h2').html() + '</h2>';
                var p = "<p>" + $(el).find('p').html() + '</p>';
                form.append("par_" + index, h + p);
            }
            else if ($(el).hasClass("par") && $(el).attr('id').indexOf('lp_') >= 0) {
                var id = $(el).attr('id').replace('lp_', '');
                form.append("lpar_" + index, id);
            }

        });

        $("#bar").css('width', '0%');
        $("#bar").on("onFinished", function (event, result) {
            $("#upload-news-window").css('display', 'none');
            try {
                if (result.code == 0) {
                    Siml.messages.add({ type: "success", text: "نمودار با موفقیت ویرایش شد" });
                    setTimeout(function () { window.location.href = "reportlist.aspx"; }, 1500);
                }
                else {
                    Siml.messages.add({ type: "error", text: result.message });
                }
            } catch (e) {
                Siml.messages.add({ type: "error", text: result.message });
            }
        });
        $("#upload-news-window").css('display', 'block');
        ereport.f1(form, $("#progress"), $("#bar"));
    });

}

