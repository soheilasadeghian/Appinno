var eican = {

    urls: [
            "/admin/icanUploader.ashx?mode=e",
            "eican.aspx/getIcan"
    ],
    f1: function (frm, progress, bar) {
        var result;
        $.ajax({
            type: 'POST',
            url: eican.urls[0],
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
    f2: function (icanID) {
        var result;
        $.ajax({
            type: "POST",
            url: eican.urls[1],
            data: '{icanID:"' + icanID + '" }',
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