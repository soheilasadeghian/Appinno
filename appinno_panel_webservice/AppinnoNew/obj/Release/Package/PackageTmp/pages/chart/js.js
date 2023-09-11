var linechart, piechart, barchart;
var newsID;
var currentNews;
var chartType = "line";

$(function () {
    check_queryString();
    setup_charts();
    loadData();
});
function loadData() {

    linechart.xAxis[0].setTitle({
        text: currentNews.xTitle
    });
    barchart.xAxis[0].setTitle({
        text: currentNews.xTitle
    });
    barchart.yAxis[0].setTitle({
        text: currentNews.yTitle
    });
    linechart.yAxis[0].setTitle({
        text: currentNews.yTitle
    });

    var list_value = $("#list_value");
    $(list_value).html('');
    var cats = [];
    var vals = [];
    var pie_data = [];

    $.each(currentNews.values, function (index, el) {
        cats.push(el.xTitle);
        vals.push(parseFloat(el.yVlaue + ""));
        pie_data.push([el.xTitle, parseFloat(el.yVlaue + "")]);

    });
    line_chart_setCategories(cats);
    line_chart_setData(vals);

    bar_chart_setCategories(cats);
    bar_chart_setData(vals);

    pie_chart_setData(pie_data);

    if (chartType == "line")
    {
        $('.chart').css('z-index', '100');
        $("#line_chart").css('z-index', '101');
       
       // linechart.exportChart();
    }
    else if (chartType == "pie")
    {
        $('.chart').css('z-index', '100');
        $("#pie_chart").css('z-index', '101');
       // piechart.exportChart();
    }
    else if (chartType == "bar")
    {
        $('.chart').css('z-index', '100');
        $("#bar_chart").css('z-index', '101');
        //barchart.exportChart();
    }
    else
    {
        $('.chart').css('z-index', '100');
        $("#line_chart").css('z-index', '101');
        //linechart.exportChart();
    }

}
function check_queryString() {
    newsID = Siml.page.url.getParam('id');
   
    var int = parseInt(newsID, 10);

    if (newsID == undefined || isNaN(int)) {

        return;

    }
    else {

        var result = chart.f1(newsID);

        if (result.result.code != 0) {
            return;
        }
        else {

            currentNews = result.report;
            chartType = currentNews.type;

        }
    }
}
function setup_charts() {
    linechart = new Highcharts.Chart({
        chart: {
            renderTo: 'line_chart',
            defaultSeriesType: 'line'
        },
        credits: {
            enabled: false
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
        }],
        navigation: {
            buttonOptions: {
                enabled: false
            }
        }
    });
    barchart = new Highcharts.Chart({
        chart: {
            renderTo: 'bar_chart',
            defaultSeriesType: 'column'
        },
        credits: {
            enabled: false
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
        }],
        navigation: {
            buttonOptions: {
                enabled: false
            }
        }
    });
    piechart = new Highcharts.Chart({
        chart: {
            renderTo: 'pie_chart',
            defaultSeriesType: 'pie'
        },
        credits: {
            enabled: false
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
        }],
        navigation: {
            buttonOptions: {
                enabled: false
            }
        }
    });
}
function line_chart_setData(values) {

    linechart.series[0].setData(values);
}
function line_chart_setCategories(value) {
    linechart.xAxis[0].setCategories(value, true);
}
function bar_chart_setData(values) {
    barchart.series[0].setData(values);
}
function bar_chart_setCategories(value) {
    barchart.xAxis[0].setCategories(value, false);
}
function pie_chart_setData(values) {
    piechart.series[0].setData(values);
}


