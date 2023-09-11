var userlist = {
    urls: [
            "userlist.aspx/getAllUser",
            "userlist.aspx/editUser",
            "userlist.aspx/getUser",
            "userlist.aspx/deleteUser",
            "userlist.aspx/deleteAllUser",
            "userlist.aspx/getAllGroup",
            "userlist.aspx/getAllSubGroup"
    ],
    f1: function (from, to, userstate, fillterString, subgroupID, orderType, sortby, pageIndex) {

        var result;
        $.ajax({
            type: "POST",
            url: userlist.urls[0],
            data: JSON.stringify({ from: from, to: to, userstate: userstate, fillterString: fillterString, subgroupID: subgroupID, orderType: orderType, sortby: sortby, pageIndex: pageIndex }),
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
    f2: function (userID, block, subgroups) {

        var result;
        $.ajax({
            type: "POST",
            url: userlist.urls[1],
            data: JSON.stringify({ userID: userID, block: block, subgroups: subgroups }),
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
    f3: function (userID) {

        var result;
        $.ajax({
            type: "POST",
            url: userlist.urls[2],
            data: JSON.stringify({ userID: userID }),
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
    f4: function (userID) {

        var result;
        $.ajax({
            type: "POST",
            url: userlist.urls[3],
            data: JSON.stringify({ userID: userID }),
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
            url: userlist.urls[4],
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
            url: userlist.urls[5],
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
            url: userlist.urls[6],
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