var answerlist = {
    urls: [
            "answerlist.aspx/getAllAnswer"
    ],
    f1: function (regDateFrom, regDateTo, answerstate, fillterString, orderType, sortby, pageIndex, creativityCompetitionId, justWinner) {

        var result;
        $.ajax({
            type: "POST",
            url: answerlist.urls[0],
            data: JSON.stringify({ regDateFrom: regDateFrom, regDateTo: regDateTo, answerstate: answerstate, fillterString: fillterString, orderType: orderType, sortby: sortby, pageIndex: pageIndex, creativityCompetitionId: creativityCompetitionId, justWinner: justWinner }),
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