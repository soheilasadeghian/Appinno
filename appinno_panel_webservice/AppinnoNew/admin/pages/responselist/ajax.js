var responselist = {
    urls: [
            "responselist.aspx/getAllResponse"
    ],
    f1: function (regDateFrom, regDateTo, responsestate, fillterString, orderType, sortby, pageIndex, myIranId, justWinner) {

        var result;
        $.ajax({
            type: "POST",
            url: responselist.urls[0],
            data: JSON.stringify({ regDateFrom: regDateFrom, regDateTo: regDateTo, responsestate: responsestate, fillterString: fillterString, orderType: orderType, sortby: sortby, pageIndex: pageIndex, myIranId: myIranId, justWinner: justWinner }),
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