var oprators = {

    urls: [
                 "oprators.aspx/getAllAccess",
                 "oprators.aspx/getAllOprators",
                 "oprators.aspx/insertOprators",
                 "oprators.aspx/editOprators",
                 "oprators.aspx/getOprator",
                 "oprators.aspx/deleteOprator"

    ],

    f1: function () {
        var result;

        $.ajax({
            type: "POST",
            url: oprators.urls[0],
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
    f2: function (fillterString, accessID, pageIndex) {

        var result;

        $.ajax({
            type: "POST",
            url: oprators.urls[1],
            data: JSON.stringify({ fillterString: fillterString, accessID: accessID, pageIndex: pageIndex }),
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
    f3: function (name, family, password, emailtell, accessID) {

        var result;

        $.ajax({
            type: "POST",
            url: oprators.urls[2],
            data: JSON.stringify({ name: name, family: family, password: password, emailtell: emailtell, accessID: accessID }),
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
    f4: function (opratorID, name, family, password, emailtell, accessID) {

        var result;

        $.ajax({
            type: "POST",
            url: oprators.urls[3],
            data: JSON.stringify({ opratorID: opratorID, name: name, family: family, password: password, emailtell: emailtell, accessID: accessID }),
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
    f5: function (opratorID) {

        var result;

        $.ajax({
            type: "POST",
            url: oprators.urls[4],
            data: JSON.stringify({ opratorID: opratorID }),
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
    f6: function (opratorID) {

        var result;

        $.ajax({
            type: "POST",
            url: oprators.urls[5],
            data: JSON.stringify({ opratorID: opratorID }),
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