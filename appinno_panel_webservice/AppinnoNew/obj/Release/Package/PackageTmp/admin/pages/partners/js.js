var PageIndex = 1;
var permission;
var editingAccessID = -1;

$(function () {

    access_permission();
    $("#sortby").simlComboBox();
    $("#order").simlComboBox();
    setup_upload_windows();
    setup_insert_windows();
    uploadFile();
    setup_all_delete_windows();
    PageIndex = 1;
    getAllPartners();
    search();
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
function search() {
    $('#fillter-input').keyup(function (e) {
        if (e.keyCode == 13) {
            $("#search-btn").click();
        }
    });

    var item = $("#search-btn");
    item.click(function () {
        PageIndex = 1;
        getAllPartners();

    });
}
function getAllPartners() {
    var filter = $("#fillter-input").val();
    var sortBy = $("#sortby").attr('value');
    var order = $("#order").attr('value');

    var result = partners.f2(filter, sortBy, order, PageIndex);
    if (result.result.code != 0) {
        Siml.messages.add({ text: result.result.message, type: "error" });
        return;
    }
    var list = $("#list");
    $("#total-count").html("( "+result.totalCount+" )");
    $(list).html('');
    $.each(result.partner, function (index, el) {
        var html =
                    $('<div class="item" itemid="' + el.ID + '">'
                    + '<div class="tools">'
                    + '<a itemid="' + el.ID + '" class="delete"></a>'
                    + '<a itemid="' + el.ID + '" class="edit" title="ویرایش"></a>'
                    + '</div>'
                    + '<span class="fullname">' + el.name + " " + el.family + '</span>'
                    + '<span class="info">' + el.innerTell + '</span>'
                    + '<span class="email">' + el.email + '</span>'
                    + '<span class="info">' + el.level + '</span>'
                    + '</div>');

        $(list).append(html);
        setup_edit_windows(html);
        setup_delete_windows(html);
    });

    var pager = $("#pager");
    $(pager).find('.page').remove();
    for (var i = 0; i < result.pageCount; i++) {
        var className = (i + 1) == PageIndex ? "page selected" : "page";
        var html =
            $('<a class="' + className + '">' + (i + 1) + '</a>');
        $(pager).append(html);
        $(html).click(function () {
            if ($(this).css('class') != "selected") {
                var index = parseInt($(this).html());
                PageIndex = index;
                getAllPartners();
            }
        });
    }
}
function uploadFile() {
    initOnChange();
    $("#btn-browse-upload-partner-window").click(function () {
        $("#file").click();
    });
    $("#btn-close-upload-upload-window").click(function () {
        close_upload_windows();
    });
    $("#btn-save-upload-partner-window").click(function () {
        if ($("#fileName").val() != "")
            partners.f1(document.getElementById("file").files[0], $("#progress"), $("#bar"), document.getElementById("file"));
    });
}
function initOnChange() {
    document.getElementById('file').addEventListener('change', null, false);
    document.getElementById('file').addEventListener('change', function (e) {

        try {
            var F = this.files;

            if (F && F[0]) {
                $("#fileName").val(F[0].name);
            }
        } catch (e) {

        }

    }, false);

    $("#bar").on("onFinished", function (event, result) {

        if (result.code != 0) {
            Siml.messages.add({ text: result.message, type: "error" });
            return;
        }
        Siml.messages.add({ text: result.message, type: "success" });
        PageIndex = 1;
        getAllPartners();
        close_upload_windows();
    });
}

//upload windows
function setup_upload_windows() {
    $("#file-btn").click(function () {
        open_upload_windows();
    });

}
function open_upload_windows() {

    $("#upload-partner-window").css("display", "block");
}
function close_upload_windows() {

    $("#upload-partner-window").css("display", "none");
}
////

//insert windows
function setup_insert_windows() {
    $("#insert-btn").click(function () {
        open_insert_windows();
    });
    $("#btn-close-insert-partner-window").click(function () {
        close_insert_windows();
    });
    $("#input-name-insert-partner-window,#input-family-insert-partner-window,#input-level-insert-partner-window,#input-email-insert-partner-window,#input-innerTell-insert-partner-window").keyup(function (e) {
        if (e.keyCode == 13) {
            $("#btn-save-insert-partner-window").click();
        }
    });
    $("#btn-save-insert-partner-window").click(function () {

        var name = $("#input-name-insert-partner-window").val();
        var family = $("#input-family-insert-partner-window").val();
        var level = $("#input-level-insert-partner-window").val();
        var email = $("#input-email-insert-partner-window").val();
        var innerTell = $("#input-innerTell-insert-partner-window").val();
        var registrationmobile = $("#input-registrationmobile-insert-partner-window").val();
        var optionalmobile = $("#input-optionalmobile-insert-partner-window").val();

        if (name == "") {
            Siml.messages.add({ text: "نام را وارد نمایید", type: "error" });
            return;
        } if (family == "") {
            Siml.messages.add({ text: "نام خانوادگی را وارد نمایید", type: "error" });
            return;
        } if (level == "") {
            Siml.messages.add({ text: "سمت سازمانی را وارد نمایید", type: "error" });
            return;
        } if (email == "") {
            Siml.messages.add({ text: "ایمیل را وارد نمایید", type: "error" });
            return;
        } if (innerTell == "") {
            Siml.messages.add({ text: "شماره داخلی را وارد نمایید", type: "error" });
            return;
        } if (innerTell == "") {
            Siml.messages.add({ text: "شماره موبایل را وارد نمایید", type: "error" });
            return;
        } if (innerTell == "") {
            Siml.messages.add({ text: "شماره تلفن اختیاری را وارد نمایید", type: "error" });
            return;
        }

        var result = partners.f3(name, family, email, innerTell, level, registrationmobile, optionalmobile);
        if (result.code != 0) {
            Siml.messages.add({ text: result.message, type: "error" });
            return;
        }
        Siml.messages.add({ text: result.message, type: "success" });
        getAllPartners();
        close_insert_windows();
    });

}
function open_insert_windows() {

    $("#input-name-insert-partner-window").val('');
    $("#input-family-insert-partner-window").val('');
    $("#input-level-insert-partner-window").val('');
    $("#input-email-insert-partner-window").val('');
    $("#input-innerTell-insert-partner-window").val('');
    $("#input-optionalmobile-insert-partner-window").val('');
    $("#input-registrationmobile-insert-partner-window").val('');
    $("#new-partner-window").css('display', 'block');

}
function close_insert_windows() {
    $("#new-partner-window").css('display', 'none');
}
///

//Edit windows
function setup_edit_windows(item) {

    var edit_btn = $(item).find('.edit');

    $(edit_btn).click(function () {
        editingAccessID = $(this).attr('itemid');
        open_edit_windows();
    });
    $("#btn-close-edit-partner-window").click(function () {
        close_edit_windows();
    });
    $("#input-name-edit-partner-window,#input-family-edit-partner-window,#input-level-edit-partner-window,#input-email-edit-partner-window,#input-innerTell-edit-partner-window,#input-registrationmobile-edit-partner-window,#input-optionalmobile-edit-partner-window").keyup(function (e) {
        if (e.keyCode == 13) {
            $("#btn-save-edit-partner-window").click();
        }
    });
    $("#btn-save-edit-partner-window").unbind();
    $("#btn-save-edit-partner-window").click(function () {

        var name = $("#input-name-edit-partner-window").val();
        var family = $("#input-family-edit-partner-window").val();
        var level = $("#input-level-edit-partner-window").val();
        var email = $("#input-email-edit-partner-window").val();
        var innerTell = $("#input-innerTell-edit-partner-window").val();
        var registrationmobile = $("#input-registrationmobile-edit-partner-window").val();
        var optionalmobile = $("#input-optionalmobile-edit-partner-window").val();

        if (name == "") {
            Siml.messages.add({ text: "نام را وارد نمایید", type: "error" });
            return;
        } if (family == "") {
            Siml.messages.add({ text: "نام خانوادگی را وارد نمایید", type: "error" });
            return;
        } if (level == "") {
            Siml.messages.add({ text: "سمت سازمانی را وارد نمایید", type: "error" });
            return;
        } if (email == "") {
            Siml.messages.add({ text: "ایمیل را وارد نمایید", type: "error" });
            return;
        } if (innerTell == "") {
            Siml.messages.add({ text: "شماره داخلی را وارد نمایید", type: "error" });
            return;
        } if (innerTell == "") {
            Siml.messages.add({ text: "شماره موبایل را وارد نمایید", type: "error" });
            return;
        } if (innerTell == "") {
            Siml.messages.add({ text: "شماره تلفن اختیاری را وارد نمایید", type: "error" });
            return;
        }

        var result = partners.f5(editingAccessID, name, family, email, innerTell, level, registrationmobile, optionalmobile);
        if (result.code != 0) {
            Siml.messages.add({ text: result.message, type: "error" });
            return;
        }
        Siml.messages.add({ text: result.message, type: "success" });
        getAllPartners();
        close_edit_windows();
        return;
    });

}
function open_edit_windows() {
    var result = partners.f4(editingAccessID);
    if (result.result.code != 0)
    {
        Siml.messages.add({ text: result.result.message, type: "error" });
        return;
    }

    $("#input-name-edit-partner-window").val(result.partner.name);
    $("#input-family-edit-partner-window").val(result.partner.family);
    $("#input-level-edit-partner-window").val(result.partner.level);
    $("#input-email-edit-partner-window").val(result.partner.email);
    $("#input-innerTell-edit-partner-window").val(result.partner.innerTell);
    $("#input-registrationmobile-edit-partner-window").val(result.partner.registrationmobile);
    $("#input-optionalmobile-edit-partner-window").val(result.partner.optionalmobile);

    $("#edit-partner-window").css('display', 'block');

}
function close_edit_windows() {
    $("#edit-partner-window").css('display', 'none');
}
///

//delete 
function setup_delete_windows(item) {

    var delete_btn = $(item).find('.delete');

    $(delete_btn).click(function () {
        var itemid = $(this).attr('itemid');

        if (confirm('آیا از حذف این همکار اطمینان دارید؟')) {
            var result = partners.f6(itemid);
            if (result.code != 0) {
                Siml.messages.add({ text: result.message, type: "error" });
                return;
            }
            Siml.messages.add({ text: result.message, type: "success" });
            getAllPartners();
        }

    });
}
function setup_all_delete_windows() {


    $("#deleteall-btn").click(function () {
        var itemid = $(this).attr('itemid');

        if (confirm('آیا از حذف همه ی مشخصات همکاران اطمینان دارید؟')) {
            var result = partners.f7();
            if (result.code != 0) {
                Siml.messages.add({ text: result.message, type: "error" });
                return;
            }
            Siml.messages.add({ text: result.message, type: "success" });
            getAllPartners();
        }

    });
}
///



