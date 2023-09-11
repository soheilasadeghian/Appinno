var eventslist = {
    urls: [
            "eventslist.aspx/getAllevents",
            "eventslist.aspx/getAllGroup",
            "eventslist.aspx/getAllSubGroup",
            "eventslist.aspx/deleteevents",
            "eventslist.aspx/deleteAllevents"
    ],
    f1: function (regDateFrom, regDateTo, toDateFrom, toDateTo, fromDateFrom, fromDateTo, eventsstate, fillterString, subgroupID, orderType, sortby, pageIndex) {

        var result;
        $.ajax({
            type: "POST",
            url: eventslist.urls[0],
            data: JSON.stringify({ regDateFrom: regDateFrom, regDateTo: regDateTo, toDateFrom: toDateFrom, toDateTo: toDateTo, fromDateFrom: fromDateFrom, fromDateTo: fromDateTo, eventsstate: eventsstate, fillterString: fillterString, subgroupID: subgroupID, orderType: orderType, sortby: sortby, pageIndex: pageIndex }),
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
            url: eventslist.urls[1],
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
            url: eventslist.urls[2],
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
            url: eventslist.urls[3],
            data: JSON.stringify({ eventsID: itemID }),
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
            url: eventslist.urls[4],
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
    }
}