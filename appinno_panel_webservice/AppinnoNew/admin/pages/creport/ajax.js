var creport = {
    urls: [
            "creport.aspx/getAllComment",
            "creport.aspx/blockComment",
            "creport.aspx/deleteComment",
            "creport.aspx/deleteAllComment"
    ],
    f1: function (from, to, commentstate, fillterString, orderType, sortby, pageIndex) {

        var result;
        $.ajax({
            type: "POST",
            url: creport.urls[0],
            data: JSON.stringify({ from: from, to: to, commentstate: commentstate, fillterString: fillterString, orderType: orderType, sortby: sortby, pageIndex: pageIndex }),
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
    f2: function (commentID, isBlock) {

        var result;
        $.ajax({
            type: "POST",
            url: creport.urls[1],
            data: JSON.stringify({ commentID: commentID, isBlock: isBlock}),
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
    f4: function (commentID) {
        var result;
        $.ajax({
            type: "POST",
            url: creport.urls[2],
            data: JSON.stringify({ commentID: commentID }),
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
            url: creport.urls[3],
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