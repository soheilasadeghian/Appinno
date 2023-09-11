var PageIndex = 1;
var permission;
var editingAccessID = -1;

$(function () {

    access_permission();
    $("#sortby").simlComboBox();
    $("#order").simlComboBox();

    setup_insert_windows();
    setup_all_delete_windows();
    PageIndex = 1;
    getAlltag();
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
        getAlltag();

    });
}
function getAlltag() {
    var filter = $("#fillter-input").val();
    var order = $("#order").attr('value');

    var result = tag.f1(filter, order, PageIndex);
    if (result.result.code != 0) {
        Siml.messages.add({ text: result.result.message, type: "error" });
        return;
    }
    $("#total-count").html(" ( " + result.totalCount + " )");
    var list = $("#list");
    $(list).html('');
    $.each(result.tag, function (index, el) {
        var html =
                    $('<div class="item" itemid="' + el.ID + '">'
                    + '<div class="tools">'
                    + '<a itemid="' + el.ID + '" class="delete"></a>'
                    + '<a itemid="' + el.ID + '" class="edit" title="ویرایش"></a>'
                    + '</div>'
                    + '<span class="fullname">' + el.title  + '</span>'
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
                getAlltag();
            }
        });
    }
}

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

        var title = $("#input-name-insert-partner-window").val();

        if (title == "") {
            Siml.messages.add({ text: "عنوان تگ را وارد نمایید", type: "error" });
            return;
        } 

        var result = tag.f2(title);
        if (result.code != 0) {
            Siml.messages.add({ text: result.message, type: "error" });
            return;
        }
        Siml.messages.add({ text: result.message, type: "success" });
        getAlltag();
        close_insert_windows();
    });

}
function open_insert_windows() {

    $("#input-name-insert-partner-window").val('');
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

        var title = $("#input-name-edit-partner-window").val();

        if (title == "") {
            Siml.messages.add({ text: "عنوان تگ را وارد نمایید", type: "error" });
            return;
        } 

        var result = tag.f3(editingAccessID,title);
        if (result.code != 0) {
            Siml.messages.add({ text: result.message, type: "error" });
            return;
        }
        Siml.messages.add({ text: result.message, type: "success" });
        getAlltag();
        close_edit_windows();
        return;
    });

}
function open_edit_windows() {
    var result = tag.f6(editingAccessID);
    if (result.result.code != 0)
    {
        Siml.messages.add({ text: result.result.message, type: "error" });
        return;
    }

    $("#input-name-edit-partner-window").val(result.tag.title);
  
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

        if (confirm('آیا از حذف این تگ اطمینان دارید؟')) {
            var result = tag.f4(itemid);
            if (result.code != 0) {
                Siml.messages.add({ text: result.message, type: "error" });
                return;
            }
            Siml.messages.add({ text: result.message, type: "success" });
            getAlltag();
        }

    });
}
function setup_all_delete_windows() {


    $("#deleteall-btn").click(function () {
        var itemid = $(this).attr('itemid');

        if (confirm('آیا از حذف همه ی تگ ها اطمینان دارید؟')) {
            var result = tag.f5();
            if (result.code != 0) {
                Siml.messages.add({ text: result.message, type: "error" });
                return;
            }
            Siml.messages.add({ text: result.message, type: "success" });
            getAlltag();
        }

    });
}
///



