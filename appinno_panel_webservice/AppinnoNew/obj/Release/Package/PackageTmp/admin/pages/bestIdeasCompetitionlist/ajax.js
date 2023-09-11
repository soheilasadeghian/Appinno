var bestIdeasCompetitionlist = {
    urls: [
            "bestIdeasCompetitionlist.aspx/getAllBestIdeasCompetitions",
            "bestIdeasCompetitionlist.aspx/getAllGroup",//later coment
            "bestIdeasCompetitionlist.aspx/getAllSubGroup",//later coment
            "bestIdeasCompetitionlist.aspx/deleteBestIdeasCompetitions",
            "bestIdeasCompetitionlist.aspx/deleteAllBestIdeasCompetitions",
            "bestIdeasCompetitionlist.aspx/SendBestIdeasCompetitionNotification",
            "bestIdeasCompetitionlist.aspx/BlockUnblockBestIdeasCompetition"
    ],
    f1: function (regDateFrom, regDateTo, startDateFrom, startDateTo, endDateFrom, endDateTo, resultVoteDateFrom, resultVoteDateTo, Competitionstate, Competitionstatus, fillterString, orderType, sortby, pageIndex) {

        var result;
        $.ajax({
            type: "POST",
            url: bestIdeasCompetitionlist.urls[0],
            data: JSON.stringify({ regDateFrom: regDateFrom, regDateTo: regDateTo, startDateFrom: startDateFrom, startDateTo: startDateTo, endDateFrom: endDateFrom, endDateTo: endDateTo, resultVoteDateFrom: resultVoteDateFrom, resultVoteDateTo: resultVoteDateTo, Competitionstate: Competitionstate, Competitionstatus: Competitionstatus, fillterString: fillterString, orderType: orderType, sortby: sortby, pageIndex: pageIndex }),
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
            url: bestIdeasCompetitionlist.urls[1],
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
            url: bestIdeasCompetitionlist.urls[2],
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
            url: bestIdeasCompetitionlist.urls[3],
            data: JSON.stringify({ bestIdeasCompetitionsID: itemID }),
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
            url: bestIdeasCompetitionlist.urls[4],
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
    f6: function (bestIdeaCompetitionsID, text) {
        var result;

        $.ajax({
            type: "POST",
            url: bestIdeasCompetitionlist.urls[5],
            data: JSON.stringify({ bestIdeaCompetitionsID: bestIdeaCompetitionsID, text: text }),
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
            url: bestIdeasCompetitionlist.urls[6],
            data: JSON.stringify({ bestIdeasCompetitionsID: itemID, isBlock: isBlock }),
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