var message = {
    urls: [
            "message.aspx/getAllMessage",
            "message.aspx/insertMessage",
            "message.aspx/getMessage",
            "message.aspx/deleteMessage",
            "message.aspx/deleteAllMessage",
            "message.aspx/getAllGroup",
            "message.aspx/getAllSubGroup",
            "message.aspx/sendMessage"
    ],
    f1: function (fillterString, toAll, toPartner, subgroupID, orderType, pageIndex) {

        var result;

        $.ajax({
            type: "POST",
            url: message.urls[0],
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
    f2: function (title, text, toAll, toPartner, pushTo) { // badan bayad pak shavad_karbord nadarad

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
    f3: function (messageID) {

        var result;
        $.ajax({
            type: "POST",
            url: message.urls[2],
            data: JSON.stringify({ messageID: messageID }),
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
    f4: function (messageID) {

        var result;
        $.ajax({
            type: "POST",
            url: message.urls[3],
            data: JSON.stringify({ messageID: messageID }),
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
            url: message.urls[4],
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
            url: message.urls[5],
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
            url: message.urls[6],
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
    f8: function (messageID) {

        var result;
        $.ajax({
            type: "POST",
            url: message.urls[7],
            data: JSON.stringify({ messageID: messageID }),
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