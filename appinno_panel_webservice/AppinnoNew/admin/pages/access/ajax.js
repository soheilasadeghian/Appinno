var paccess = {

    urls: [
                 "access.aspx/getAllAccess",
                 "access.aspx/insertAccess",
                 "access.aspx/editAccess",
                 "access.aspx/getAccess",
                 "access.aspx/deleteAccess"
    ],

    f1: function (fillterString, part, action, pageIndex) {
        var result;

        $.ajax({
            type: "POST",
            url: paccess.urls[0],
            data: JSON.stringify({ fillterString: fillterString, part: part, action: action, pageIndex: pageIndex }),
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
    f2: function (title, news, events, io, pub, chart, download, grouping, users, access, operators, ican, poll, bestIdeasCompetition, creativityCompetition, myIranCompetition) {

        var result;

        $.ajax({
            type: "POST",
            url: paccess.urls[1],
            data: JSON.stringify({ title: title, news: news, events: events, io: io, pub: pub, chart: chart, download: download, grouping: grouping, users: users, access: access, operators: operators, ican: ican, poll: poll, bestIdeasCompetition: bestIdeasCompetition, creativityCompetition: creativityCompetition, myIranCompetition: myIranCompetition }),
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
    f3: function (accessID, title, news, events, io, pub, chart, download, grouping, users, access, operators, ican, poll, bestIdeasCompetition, creativityCompetition, myIranCompetition) {

        var result;

        $.ajax({
            type: "POST",
            url: paccess.urls[2],
            data: JSON.stringify({ accessID: accessID, title: title, news: news, events: events, io: io, pub: pub, chart: chart, download: download, grouping: grouping, users: users, access: access, operators: operators, ican: ican, poll: poll, bestIdeasCompetition: bestIdeasCompetition, creativityCompetition: creativityCompetition, myIranCompetition: myIranCompetition }),
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
    f4: function (accessID) {

        var result;

        $.ajax({
            type: "POST",
            url: paccess.urls[3],
            data: JSON.stringify({ accessID: accessID }),
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
    f5: function (accessID) {

        var result;

        $.ajax({
            type: "POST",
            url: paccess.urls[4],
            data: JSON.stringify({ accessID: accessID }),
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