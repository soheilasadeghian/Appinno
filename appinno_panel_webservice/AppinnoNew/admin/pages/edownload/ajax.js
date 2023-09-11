var edownload = {

    urls: [
            "/admin/downloadUploader.ashx?mode=e",
            "edownload.aspx/getAllGroup",
            "edownload.aspx/getAllSubGroup",
            "edownload.aspx/getdownload",
            "edownload.aspx/getAllTag"
    ],
    f1: function (frm, progress, bar) {
        var result;
        $.ajax({
            type: 'POST',
            url: edownload.urls[0],
            data: frm,
            success: function (response) {
                var json = JSON.parse(response + "");
                result = json;
                $(bar).trigger('onFinished', [json]);
            },
            processData: false,
            contentType: false,
            xhr: function () {

                var xhr = $.ajaxSettings.xhr();

                xhr.upload.onprogress = function (evt) { console.log('progress', evt.loaded / evt.total * 100); $(bar).css("width", (evt.loaded / evt.total * 100) + "%"); };

                xhr.upload.onload = function () { console.log('DONE!'); };

                return xhr;
            }
        });

        return result;
    },
    f2: function () {
        var result;

        $.ajax({
            type: "POST",
            url: edownload.urls[1],
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
            url: edownload.urls[2],
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
    f4: function (downloadID) {
        var result;
        $.ajax({
            type: "POST",
            url: edownload.urls[3],
            data: '{downloadID:"' + downloadID + '" }',
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
            url: edownload.urls[4],
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