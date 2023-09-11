var myIranlist = {
    urls: [
            "myIranlist.aspx/getAllMyIran",
            "myIranlist.aspx/getAllGroup",
            "myIranlist.aspx/getAllSubGroup",
            "myIranlist.aspx/deleteMyIran",
            "myIranlist.aspx/deleteAllMyIran",
            "myIranlist.aspx/SendMyIranNotification",
            "myIranlist.aspx/BlockUnblockMyIran"
    ],
    f1: function (regDateFrom, regDateTo, startDateFrom, startDateTo, endDateFrom, endDateTo, Competitionstate, Competitionstatus, fillterString, orderType, sortby, pageIndex) {

        var result;
        $.ajax({
            type: "POST",
            url: myIranlist.urls[0],
            data: JSON.stringify({ regDateFrom: regDateFrom, regDateTo: regDateTo, startDateFrom: startDateFrom, startDateTo: startDateTo, endDateFrom: endDateFrom, endDateTo: endDateTo, Competitionstate: Competitionstate, Competitionstatus: Competitionstatus, fillterString: fillterString, orderType: orderType, sortby: sortby, pageIndex: pageIndex }),
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
            url: myIranlist.urls[1],
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
    f3: function (GroupID) {
        var result;

        $.ajax({
            type: "POST",
            url: myIranlist.urls[2],
            data: JSON.stringify({ GroupID: GroupID }),
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
    f4: function (itemID) {
        var result;

        $.ajax({
            type: "POST",
            url: myIranlist.urls[3],
            data: JSON.stringify({ myIranID: itemID }),
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
    f5: function () {
        var result;

        $.ajax({
            type: "POST",
            url: myIranlist.urls[4],
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
    f6: function (myIranID, text) {
        var result;

        $.ajax({
            type: "POST",
            url: myIranlist.urls[5],
            data: JSON.stringify({ myIranID: myIranID, text: text }),
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
    f7: function (itemID, isBlock) {
        var result;

        $.ajax({
            type: "POST",
            url: myIranlist.urls[6],
            data: JSON.stringify({ myIranID: itemID, isBlock: isBlock }),
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