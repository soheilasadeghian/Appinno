var creativityCompetitionlist = {
    urls: [
            "creativityCompetitionlist.aspx/getAllCreativityCompetition",
            "creativityCompetitionlist.aspx/getAllGroup",
            "creativityCompetitionlist.aspx/getAllSubGroup",
            "creativityCompetitionlist.aspx/deleteCreativityCompetition",
            "creativityCompetitionlist.aspx/deleteAllCreativityCompetition",
            "creativityCompetitionlist.aspx/SendCreativityCompetitionNotification",
            "creativityCompetitionlist.aspx/BlockUnblockCreativityCompetition"
    ],
    f1: function (regDateFrom, regDateTo, startDateFrom, startDateTo, endDateFrom, endDateTo, Competitionstate, Competitionstatus, fillterString, orderType, sortby, pageIndex) {

        var result;
        $.ajax({
            type: "POST",
            url: creativityCompetitionlist.urls[0],
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
            url: creativityCompetitionlist.urls[1],
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
            url: creativityCompetitionlist.urls[2],
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
            url: creativityCompetitionlist.urls[3],
            data: JSON.stringify({ creativityCompetitionID: itemID }),
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
            url: creativityCompetitionlist.urls[4],
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
    f6: function (creativityCompetitionID, text) {
        var result;

        $.ajax({
            type: "POST",
            url: creativityCompetitionlist.urls[5],
            data: JSON.stringify({ creativityCompetitionID: creativityCompetitionID, text: text }),
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
            url: creativityCompetitionlist.urls[6],
            data: JSON.stringify({ creativityCompetitionID: itemID, isBlock: isBlock }),
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