var chart = {

    urls:
    [
        "chart.aspx/isReportExist"//ابن متد وظیفه بررسی وجود و عدم وجود نمودار را بر عهده دارد
    ],

    //این متد وظیفه بررسی وجود و عدم وجود نمودار را برعهده دارد
    f1: function (reportID) {
        var result;
        $.ajax({
            type: "POST",
            url: chart.urls[0],
            data: '{reportID:"' + reportID + '" }',
            contentType: "application/json; charset=utf-8",
            dataType: "json", async: false,
            success: function (response) {

                 var json = JSON.parse(response.d + "");
                result = json;

            },
            failure: function (response) {

                result = "error";
            }
        });

        return result;
    }
}