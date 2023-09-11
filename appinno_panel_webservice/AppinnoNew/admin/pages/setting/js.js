var permission;
$(function () {
    ID = 0;
    access_permission();
    Tipped.create('.hint,.tools .button');

    load_dailyText();
    load_Policy();
    load_adminimage()
    setup_save_all();
    setup_insert_image();
});
function access_permission() {
    var ac = Siml.Access.get('dashboard.aspx');

    if (ac.result.code != 0) {
        Siml.messages.add({ text: ac.result.message, type: "error" });
        return;
    }
    else {
        var per = ac.access;
        permission = ac.access;
    }
}

function load_dailyText() {

    var result = setting.f1();


    if (result.code != 0) {

        $('body').html("");
        Siml.messages.add({ type: "error", text: result.message });
        setTimeout(function myfunction() {
            Siml.page.url.goto('setting.aspx');
        }, 2500);
    }
    else {
        var title = $("#daily-title");
        title.val(result.message);
    }
}
function load_Policy() {

    var result = setting.f3();


    if (result.code != 0) {

        $('body').html("");
        Siml.messages.add({ type: "error", text: result.message });
        setTimeout(function myfunction() {
            Siml.page.url.goto('setting.aspx');
        }, 2500);
    }
    else {
        var title = $("#policy");
        title.val(result.message);
    }
}
function load_adminimage() {

    var result = setting.f4();


    if (result.code != 0) {

        $('body').html("");
        Siml.messages.add({ type: "error", text: result.message });
        setTimeout(function myfunction() {
            Siml.page.url.goto('setting.aspx');
        }, 2500);
    }
    else {
        $("#adminimage").html('<img class="image adminimage" id="mainimage" src="' + result.message + '"  />');

    }
}
function setup_save_all() {

    $("#btn-save-all").click(function () {
            var title = $("#daily-title");
            var policy = $("#policy");

            if (title.val() == "") {
                Siml.messages.add({ text: "سخن روز را وارد کنید", type: "error" });
                return;
            }
            if (policy.val() == "") {
                Siml.messages.add({ text: "قوانین درج محتوا را وارد کنید", type: "error" });
                return;
            }
            if (!$("#adminimage").find(".image").length >= 1) {
                Siml.messages.add({ text: "عکس را وارد کنید", type: "error" });
                return;
            }
        
        var form = new FormData();
        form.append("daily-title", title.val());
        form.append("policy", policy.val());

        var file = document.getElementById("file_image").files[0];
        form.append("img", file);
        

        $("#bar").css('width', '0%');
        $("#bar").on("onFinished", function (event, result) {
            $("#upload-news-window").css('display', 'none');
            try {
                if (result.code == 0) {
                    Siml.messages.add({ type: "success", text: "تنظیمات با موفقیت درج شد" });
                    setTimeout(function () { window.location.href = "setting.aspx"; }, 1500);
                }
                else {
                    Siml.messages.add({ type: "error", text: result.message });
                }
            } catch (e) {
                Siml.messages.add({ type: "error", text: result.message });
            }
        });
        $("#upload-news-window").css('display', 'block');
        setting.f2(form, $("#progress"), $("#bar"));
    });


}

/// insert image

function setup_insert_image() {
    image_initOnChange();
}
function image_initOnChange() {
    document.getElementById('file_image').addEventListener('change', null, false);
    document.getElementById('file_image').addEventListener('change', function (e) {

        if (this.disabled)
            return alert('فایل پشتیبانی نمی شود!');
        var F = this.files;

        if (F && F[0]) for (var i = 0; i < F.length; i++) {
            Siml.page.readImage(F[i], function (result) {

                var html = $(
                     '<img class="image adminimage" id="mainimage" src="' + result.src + '"  />'
                    );

                var list = $('#adminimage');
                image_initOnChange();
                $(list).children(".image").remove();
                $(list).append(html);
            });
        }
    }, false);
}

