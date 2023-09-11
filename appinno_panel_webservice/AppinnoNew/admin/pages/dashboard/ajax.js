var dashboard = {

    urls: [
             "dashboard.aspx/getUserCount",
             "dashboard.aspx/getUngroupUserCount",
             "dashboard.aspx/getPartnerCount",
             "dashboard.aspx/getUserList",
             "dashboard.aspx/getCount"
    ],

    f1: function (date) {
        var result;

        $.ajax({
            type: "POST",
            url: dashboard.urls[0],
            data: JSON.stringify({ date: date }),
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
    },
    f2: function () {
        var result;

        $.ajax({
            type: "POST",
            url: dashboard.urls[1],
            data: JSON.stringify({}),
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
    },
    f3: function () {
        var result;

        $.ajax({
            type: "POST",
            url: dashboard.urls[2],
            data: JSON.stringify({}),
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
    },
    f4: function (date) {
        var result;

        $.ajax({
            type: "POST",
            url: dashboard.urls[3],
            data: JSON.stringify({ date: date }),
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
    },
    f5: function (date, content) {
        var result;

        $.ajax({
            type: "POST",
            url: dashboard.urls[4],
            data: JSON.stringify({ date: date, content: content }),
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