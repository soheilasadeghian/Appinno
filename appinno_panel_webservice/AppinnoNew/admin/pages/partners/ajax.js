var partners = {

    urls: [
            "/admin/partnersUploader.ashx",
            "partners.aspx/getAllPartners",
            "partners.aspx/insertPartner",
            "partners.aspx/getPartner",
            "partners.aspx/editPartner",
            "partners.aspx/deletePartner",
            "partners.aspx/deleteAllPartner"
    ],
    f1: function (frm, progress, bar, browser) {
        var form = new FormData();
        form.append("file", frm);

        var result;
        $.ajax({
            type: 'POST',
            url: partners.urls[0],
            data: form,
            success: function (response) {
                // do something
                var json = JSON.parse(response + "");
                $(bar).trigger('onFinished', [json]);

                browser.files[0] = null;
                $(bar).css('width', '0%');
            },
            xhr: function () {

                var xhr = $.ajaxSettings.xhr();

                xhr.upload.onprogress = function (evt) { console.log('progress', evt.loaded / evt.total * 100); $(bar).css("width", (evt.loaded / evt.total * 100) + "%"); };

                xhr.upload.onload = function () { console.log('DONE!'); };

                return xhr;
            },
            processData: false,
            contentType: false
        });

        return result;
    },
    f2: function (fillterString, sortBy, orderType, pageIndex) {

        var result;

        $.ajax({
            type: "POST",
            url: partners.urls[1],
            data: JSON.stringify({ fillterString: fillterString, sortBy: sortBy, orderType: orderType, pageIndex: pageIndex }),
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
    f3: function (name, family, email, innerTell, level, registrationmobile, optionalmobile) {

        var result;
        $.ajax({
            type: "POST",
            url: partners.urls[2],
            data: JSON.stringify({ name: name, family: family, email: email, innerTell: innerTell, level: level, registrationmobile:registrationmobile, optionalmobile: optionalmobile }),
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
    f4: function (partnerID) {

        var result;
        $.ajax({
            type: "POST",
            url: partners.urls[3],
            data: JSON.stringify({ partnerID: partnerID }),
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
    f5: function (partnerID, name, family, email, innerTell, level, registrationmobile, optionalmobile) {

        var result;
        $.ajax({
            type: "POST",
            url: partners.urls[4],
            data: JSON.stringify({ partnerID: partnerID, name: name, family: family, email: email, innerTell: innerTell, level: level, registrationmobile:registrationmobile, optionalmobile: optionalmobile }),
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
    f6: function (partnerID) {

        var result;
        $.ajax({
            type: "POST",
            url: partners.urls[5],
            data: JSON.stringify({ partnerID: partnerID }),
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
    f7: function () {

        var result;
        $.ajax({
            type: "POST",
            url: partners.urls[6],
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