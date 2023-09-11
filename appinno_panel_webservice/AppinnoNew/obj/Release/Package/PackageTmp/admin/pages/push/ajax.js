var push = {
    urls: [
            "push.aspx/getAllPush",
            "push.aspx/insertPush",
            "push.aspx/getPush",
            "push.aspx/deletePush",
            "push.aspx/deleteAllPush",
            "push.aspx/getAllGroup",
            "push.aspx/getAllSubGroup"
    ],
    f1: function (fillterString, toAll, toPartner, subgroupID, orderType, pageIndex) {

        var result;

        $.ajax({
            type: "POST",
            url: push.urls[0],
            data: JSON.stringify({ fillterString: fillterString, toAll: toAll, toPartner: toPartner, subgroupID: subgroupID, orderType: orderType, pageIndex: pageIndex }),
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
    f2: function (title, text, toAll, toPartner, pushTo) {

        var result;
        $.ajax({
            type: "POST",
            url: push.urls[1],
            data: JSON.stringify({ title: title, text: text, toAll: toAll, toPartner: toPartner, pushTo: pushTo }),
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
    f3: function (pushID) {

        var result;
        $.ajax({
            type: "POST",
            url: push.urls[2],
            data: JSON.stringify({ pushID: pushID }),
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
    f4: function (pushID) {

        var result;
        $.ajax({
            type: "POST",
            url: push.urls[3],
            data: JSON.stringify({ pushID: pushID }),
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
            url: push.urls[4],
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
    f6: function () {
        var result;

        $.ajax({
            type: "POST",
            url: push.urls[5],
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
    f7: function (GroupID) {
        var result;

        $.ajax({
            type: "POST",
            url: push.urls[6],
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
    }
}