var groups = {

    urls: [
                    "groups.aspx/getAllGroup",
                    "groups.aspx/insertGroup",
                    "groups.aspx/editGroup",
                    "groups.aspx/getGroup",
                    "groups.aspx/deleteGroup",

                    "groups.aspx/getAllSubGroup",//5
                    "groups.aspx/insertSubGroup",
                    "groups.aspx/editSubGroup",
                    "groups.aspx/getSubGroup",
                    "groups.aspx/deleteSubGroup"
    ],
    f1: function () {
        var result;

        $.ajax({
            type: "POST",
            url: groups.urls[0],
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
    f2: function (title) {
        var result;

        $.ajax({
            type: "POST",
            url: groups.urls[1],
            data: JSON.stringify({ title: title }),
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
    f3: function (GroupID, title) {
        var result;

        $.ajax({
            type: "POST",
            url: groups.urls[2],
            data: JSON.stringify({ GroupID: GroupID, title: title }),
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
    f4: function (GroupID) {
        var result;

        $.ajax({
            type: "POST",
            url: groups.urls[3],
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
    f5: function (GroupID) {
        var result;

        $.ajax({
            type: "POST",
            url: groups.urls[4],
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
    f6: function (GroupID) {
        var result;

        $.ajax({
            type: "POST",
            url: groups.urls[5],
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
    f7: function (title, groupID, canPush, toAll,toPartner, pushTo) {
        var result;

        $.ajax({
            type: "POST",
            url: groups.urls[6],
            data: JSON.stringify({ title: title, groupID: groupID, canPush: canPush, toAll: toAll, toPartner: toPartner, pushTo: pushTo }),
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
    f8: function (subgroupID,title, canPush, toAll, toPartner, pushTo) {
        var result;

        $.ajax({
            type: "POST",
            url: groups.urls[7],
            data: JSON.stringify({ subgroupID:subgroupID,title: title,canPush: canPush, toAll: toAll, toPartner: toPartner, pushTo: pushTo }),
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
    ,
    f9: function (SubGroupID) {
        var result;

        $.ajax({
            type: "POST",
            url: groups.urls[8],
            data: JSON.stringify({ SubGroupID: SubGroupID }),
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
    f10: function (SubGroupID) {
        var result;

        $.ajax({
            type: "POST",
            url: groups.urls[9],
            data: JSON.stringify({ SubGroupID: SubGroupID }),
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