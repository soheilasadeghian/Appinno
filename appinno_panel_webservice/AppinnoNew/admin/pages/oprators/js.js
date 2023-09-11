var PageIndex = 1;
var permission;
var editingAccessID = -1;

$(function () {
    operators_permission();
    get_all_access($("#parts"));
    get_all_access_new_window($("#parts2"));
    get_all_access_edit_window($("#parts3"));

    PageIndex = 1;
    getAllOprators();
    search();
    setup_oprator_window();
});

function search() {
    $('#fillter-input').keyup(function (e) {
        if (e.keyCode == 13) {
            $("#search-btn").click();
        }
    });

    var item = $("#search-btn");
    item.click(function () {
        PageIndex = 1;
        getAllOprators();

    });
}

function getAllOprators() {

    var fillterString = $("#fillter-input").val();
    var accessID = $("#parts").attr('value');

    var result = oprators.f2(fillterString, accessID, PageIndex);
    if (result.result.code != 0) {
        Siml.messages.add({ text: result.result.message, type: "error" });
        return;
    }
    var items = result.oprator;
    $("#total-count").html("( " + result.totalCount + " )");
    var list = $("#list");
    $(list).html('');
    $.each(items, function (index, el) {
        var canDetele = permission.delete && el.ID != 1 ? '<a itemid="' + el.ID + '" class="delete"></a>' : "";
        var canEdit = permission.edit ? '<a itemid="' + el.ID + '" class="edit"  title="ویرایش"></a>' : "";
        var html =
                    $('<div class="item" itemid="' + el.ID + '">'
                    + '<div class="tools">'
                    + canDetele
                    + canEdit
                    + '</div>'
                    + '<span class="info">' + el.emailtell + '</span>'
                    + '<span class="info">' + el.accessTitle + '</span>'
                    + '<span class="title">' + el.name + " " + el.family + '</span>'
                    + '</div>');

        $(list).append(html);
        setup_edit_oprator(html);
        setup_delete_oprator(html);
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
                getAllOprators();
            }
        });
    }

}
function get_all_access_edit_window(combo) {

    var item = $(combo).parent();
    item.html('');
    var result = oprators.f1();
    if (result.result.code == 0) {
        var html =
            '<div id="parts3" class="select">';

        $.each(result.access, function (index, ac) {

            html += '<span class="option" value="' + ac.ID + '">' + ac.title + '</span>';

        });

        html += '</div>';
        item.html(html);
        $("#parts3").simlComboBox();

    } else {
        Siml.messages.add({ text: result.result.message, type: "error" });
        return;
    }

}
function get_all_access_new_window(combo) {

    var item = $(combo).parent();
    item.html('');
    var result = oprators.f1();
    if (result.result.code == 0) {
        var html =
            '<div id="parts2" class="select">';

        $.each(result.access, function (index, ac) {

            html += '<span class="option" value="' + ac.ID + '">' + ac.title + '</span>';

        });

        html += '</div>';
        item.html(html);
        $("#parts2").simlComboBox();

    } else {
        Siml.messages.add({ text: result.result.message, type: "error" });
        return;
    }

}
function get_all_access(combo) {

    var item = $(combo).parent();
    item.html('');
    var result = oprators.f1();
    if (result.result.code == 0) {
        var html =
            '<div id="parts" class="select">'
            + '<span class="option" value="-1">همه</span>';

        $.each(result.access, function (index, ac) {

            html += '<span class="option" value="' + ac.ID + '">' + ac.title + '</span>';

        });


        html += '</div>';
        item.html(html);


        $("#parts").simlComboBox();


    } else {
        Siml.messages.add({ text: result.result.message, type: "error" });
        return;
    }

}

function operators_permission() {
    var ac = Siml.Access.get('dashboard.aspx');

    if (ac.result.code != 0) {
        Siml.messages.add({ text: ac.result.message, type: "error" });
        return;
    }
    else {
        var per = ac.access.operators.access;
        permission = ac.access.operators.access;
        if (!per.insert) {
            $("#insert-btn").remove();
            return;
        }
        $("#insert-btn").click(function () {

            open_insert_oprator_window();

        });
    }
}

function setup_oprator_window() {
    $(document).keyup(function (e) {
        if (e.keyCode == 27) {
            close_insert_oprator_window();
            close_edit_oprator_window();
        }
        if (e.keyCode == 45) {
            if (permission.insert)
                open_insert_oprator_window();
        }
    });

    setup_insert_oprator_window();
    setup_edit_oprator_window();
}
function open_insert_oprator_window() {
    var insertwindow = $("#new-oprator-window");
    $("#input-name-insert-oprator-window").val('');
    $("#input-family-insert-oprator-window").val('');
    $("#input-password-insert-oprator-window").val('');
    $("#input-repassword-insert-oprator-window").val('');
    $("#input-emailtell-insert-oprator-window").val('');
    $('#parts2').selectByValue("-1");

    insertwindow.css('display', 'block');
}

function close_insert_oprator_window() {
    var insertwindow = $("#new-oprator-window");
    insertwindow.css('display', 'none');
}

function setup_insert_oprator_window() {

    $("#input-name-insert-oprator-window,#input-family-insert-oprator-window,#input-password-insert-oprator-window,#input-repassword-insert-oprator-window,#input-emailtell-insert-oprator-window").keyup(function (e) {
        if (e.keyCode == 13) {
            $("#btn-save-insert-oprator-window").click();
        }
    });
    var btn_save_oprator_access_window = $("#btn-save-insert-oprator-window");
    btn_save_oprator_access_window.click(function () {

        var name = $("#input-name-insert-oprator-window").val();
        var family = $("#input-family-insert-oprator-window").val();
        var password = $("#input-password-insert-oprator-window").val();
        var repassword = $("#input-repassword-insert-oprator-window").val();
        var accessID = $("#parts2").attr('value');
        var emailtell = $("#input-emailtell-insert-oprator-window").val();

        if (name == "") {
            Siml.messages.add({ text: "نام را وارد نمایید", type: "error" });
            return;
        }
        if (family == "") {
            Siml.messages.add({ text: "نام خانوادگی را وارد نمایید", type: "error" });
            return;
        }
        if (password == "") {
            Siml.messages.add({ text: "کلمه عبور را وارد نمایید", type: "error" });
            return;
        }
        if (password != repassword) {
            Siml.messages.add({ text: "کلمه عبور وارد شده با تکرار آن مطابقت ندارد", type: "error" });
            return;
        }
        if (emailtell == "") {
            Siml.messages.add({ text: "نام کاربری را وارد نمایید", type: "error" });
            return;
        }

        var result = oprators.f3(name, family, password, emailtell, accessID);
        if (result.code == 0) {
            Siml.messages.add({ text: "عملیات با موفقیت انجام شد", type: "success" });
            getAllOprators();
            close_insert_oprator_window();
        }
        else {
            Siml.messages.add({ text: result.message, type: "error" });
            return;
        }

    });
    var btn_close_insert_oprator_window = $("#btn-close-insert-oprator-window");
    btn_close_insert_oprator_window.click(function () {

        close_insert_oprator_window();
    });
}

function setup_edit_oprator(item) {

    var edit = $(item).find('.edit');
    $(edit).click(function () {

        var itemID = $(this).attr('itemID');

        var result = oprators.f5(itemID);
        if (result.result.code == 0) {
            editingAccessID = itemID;
            open_edit_oprator_window(result.oprator);
        }
        else {
            Siml.messages.add({ text: result.result.message, type: "error" });
            return;
        }
    });
}
function setup_delete_oprator(item) {

    var del = $(item).find('.delete');
    $(del).click(function () {
        if (confirm('آیا از حذف این نقش اطمینان دارید؟')) {
            var itemID = $(this).attr('itemID');
            var result = oprators.f6(itemID);
            if (result.code == 0) {
                Siml.messages.add({ text: result.message, type: "success" });
                getAllOprators();
            }
            else {
                Siml.messages.add({ text: result.message, type: "error" });
                return;
            }
        }
    });
}


function open_edit_oprator_window(currentAccess) {

    var editwindow = $("#edit-oprator-window");

    var name = $("#input-name-edit-oprator-window");
    var family = $("#input-family-edit-oprator-window");
    var password = $("#input-password-edit-oprator-window");
    var repassword = $("#input-repassword-edit-oprator-window");
    var emailtell = $("#input-emailtell-edit-oprator-window");

    name.val(currentAccess.name);
    family.val(currentAccess.family);
    $("#parts3").selectByValue(currentAccess.accessID);
    emailtell.val(currentAccess.emailtell);

    editwindow.css('display', 'block');
}

function close_edit_oprator_window() {

    var editwindow = $("#edit-oprator-window");
    editwindow.css('display', 'none');

}

function setup_edit_oprator_window() {
    $("#input-name-edit-oprator-window,#input-family-edit-oprator-window,#input-password-edit-oprator-window,#input-repassword-edit-oprator-window,#input-emailtell-edit-oprator-window").keyup(function (e) {
        if (e.keyCode == 13) {
            $("#btn-save-edit-oprator-window").click();
        }
    });


    var btn_close_edit_oprator_window = $("#btn-close-edit-oprator-window");
    btn_close_edit_oprator_window.click(function () {

        close_edit_oprator_window();
    });

    var btn_save_edit_oprator_window = $("#btn-save-edit-oprator-window");
    btn_save_edit_oprator_window.click(function () {

        var name = $("#input-name-edit-oprator-window").val();
        var family = $("#input-family-edit-oprator-window").val();
        var password = $("#input-password-edit-oprator-window").val();
        var repassword = $("#input-repassword-edit-oprator-window").val();
        var accessID = $("#parts3").attr('value');
        var emailtell = $("#input-emailtell-edit-oprator-window").val();

        if (name == "") {
            Siml.messages.add({ text: "نام را وارد نمایید", type: "error" });
            return;
        }
        if (family == "") {
            Siml.messages.add({ text: "نام خانوادگی را وارد نمایید", type: "error" });
            return;
        }
        if (password != repassword && password != "") {
            Siml.messages.add({ text: "کلمه عبور وارد شده با تکرار آن مطابقت ندارد", type: "error" });
            return;
        }
        if (emailtell == "") {
            Siml.messages.add({ text: "نام کاربری را وارد نمایید", type: "error" });
            return;
        }

        var result = oprators.f4(editingAccessID, name, family, password, emailtell, accessID);
        if (result.code == 0) {
            Siml.messages.add({ text: "عملیات با موفقیت انجام شد", type: "success" });
            getAllOprators();
            close_edit_oprator_window();
        }
        else {
            Siml.messages.add({ text: result.message, type: "error" });
            return;
        }


    });

}


