var setting = {

    urls: [
            "setting.aspx/getDailyText",
            "/admin/settingUploader.ashx",
            "setting.aspx/gePolicyText",
            "setting.aspx/getAdminImage",
    ],
    f1: function () {
        var result;

        $.ajax({
            type: "POST",
            url: setting.urls[0],
            data: {},
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
    f2: function (frm, progress, bar) {
        var result;
        $.ajax({
            type: 'POST',
            url: setting.urls[1],
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
    f3: function () {
        var result;

        $.ajax({
            type: "POST",
            url: setting.urls[2],
            data: {},
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
    f4: function () {
        var result;

        $.ajax({
            type: "POST",
            url: setting.urls[3],
            data: {},
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