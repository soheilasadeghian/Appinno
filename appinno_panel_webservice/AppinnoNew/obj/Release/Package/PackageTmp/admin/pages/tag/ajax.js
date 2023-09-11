var tag = {

    urls: [
            "tag.aspx/getAllTag",
            "tag.aspx/insertTag",
            "tag.aspx/editTag",
            "tag.aspx/deleteTag",
            "tag.aspx/deleteAllTag",
            "tag.aspx/getTag"
    ],
   
    f1: function (fillterString,  orderType, pageIndex) {

        var result;

        $.ajax({
            type: "POST",
            url: tag.urls[0],
            data: JSON.stringify({ fillterString: fillterString, orderType: orderType, pageIndex: pageIndex }),
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
    f2: function (title) {

        var result;
        $.ajax({
            type: "POST",
            url: tag.urls[1],
            data: JSON.stringify({ title: title}),
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
    f3: function (tagID, title) {

        var result;
        $.ajax({
            type: "POST",
            url: tag.urls[2],
            data: JSON.stringify({ tagID: tagID, title: title}),
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
    f4: function (tagID) {

        var result;
        $.ajax({
            type: "POST",
            url: tag.urls[3],
            data: JSON.stringify({ tagID: tagID }),
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
            url: tag.urls[4],
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
    f6: function (tagID) {

        var result;
        $.ajax({
            type: "POST",
            url: tag.urls[5],
            data: JSON.stringify({ tagID: tagID }),
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
}