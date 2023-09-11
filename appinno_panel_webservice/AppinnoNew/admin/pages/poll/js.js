var permission;
$(function () {
    ID = 0;
    access_permission();
    Tipped.create('.hint,.tools .button');
    $('#date1').datepicker({ isRTL: false, showButtonPanel: true });
    $('#date2').datepicker({ isRTL: false, showButtonPanel: true });
    $("#fchk_forall").on('change', function () {
        if ($("#fchk_forall").is(':checked')) {
            $("#group-panel").slideUp(120);
        }
    });
    $("#fchk_toGroup").on('change', function () {
        if ($("#fchk_toGroup").is(':checked')) {
            $("#group-panel").slideDown(120);
        }
    });
    $("#fchk_noTag").on('change', function () {
        if ($("#fchk_noTag").is(':checked')) {
            $("#tag-panel").slideUp(120);
        }
    });
    $("#fchk_hasTag").on('change', function () {
        if ($("#fchk_hasTag").is(':checked')) {
            $("#tag-panel").slideDown(120);
        }
    });
    // $("#sortby").simlComboBox();
    // $("#order").simlComboBox();
    setup_insert_image();

    setup_insert_para();
    init_edit_para(); //contentedit
    setup_group();
    setup_add_group_subgroup();
    setup_tag();
    setup_save_all();

    setup_option();

});

function setup_option() {

    $("#new-option-input").keyup(function (e) {
        if (e.keyCode == 13) {
            $("#btn-add-option").click();
        }
    });
    $("#btn-add-option").click(function () {

        var newOption = $("#new-option-input").val();

        if (newOption == "") {
            Siml.messages.add({ text: "گزینه جدید را وارد نمایید", type: "error" });
            return;
        }

        var lst = $("#option-list");

        var html = $(
        '<div class="op-item">'
        + '<input type="text" class="option-record" name="name" value="' + newOption + '" />'
        + '<input  type="button" class="remove-option" value="ˣ" />'
        + '</div>');

        lst.append(html);
        setup_remove_option(html);
        $("#new-option-input").val("");
        $("#new-option-input").focus();

    });
}

function setup_remove_option(el) {
    $(el).find(".remove-option").click(function () {
        $(this).parent().remove();
    });
}

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

/// insert image

function setup_insert_image() {
    image_initOnChange();
}
function image_initOnChange() {
    document.getElementById('file_image').addEventListener('change', null, false);
    document.getElementById('file_image').addEventListener('change', function (e) {

        var lst = $("#items");
        if (lst.find(".img").length > 0) {
            Siml.messages.add({ text: "تنها قادر به اضافه نمودن یک تصویر هستید", type: "error" });
            return;
        }
        if (this.disabled)
            return alert('فایل پشتیبانی نمی شود!');
        var F = this.files;

        if (F && F[0]) for (var i = 0; i < F.length; i++) {
            ID++;
            Siml.page.readImage(F[i], function (result) {

                var html = $(
                    '<div id="i_' + ID + '" class="item img" >'
                    + '<img class="image" src="' + result.src + '"  />'
                    + '<input type="button" class="delete" title="حذف" value="x" />'
                    + '</div>'
                    );

                var uploaded = $('#uploaded');

                var pfi = $("#file_image").parent();

                $("#file_image").attr('id', id = "u_" + ID);

                pfi.append('<input class="file_image" type="file" style="display: none" id="file_image" />');

                $(uploaded).append($("#file_image"));

                var list = $('#items');
                image_initOnChange();
                $(list).append(html);
                setup_delete_image_item(html);
                setup_arrange_item(html);//contentedit
            });
        }
    }, false);
}
function setup_delete_image_item(item) {
    $(item).find('.delete').click(function () {
        var id = $(this).parent().attr('id').replace('i_', "");
        $("#u_" + id).remove();
        $(item).remove();
    });
}

/// insert para
function setup_insert_para() {
    $(document).keyup(function (e) {
        if (e.keyCode == 27) {
            var window = $("#new-para-window");
            window.css('display', 'none');
        }
    });
    $("#add-para").click(function () {

        var window = $("#new-para-window");

        var lst = $("#items");
        if (lst.find(".par").length > 0) {
            Siml.messages.add({ text: "تنها قادر به اضافه نمودن یک پاراگراف هستید", type: "error" });
            return;
        }

        $("#input-text-insert-para-window").val('');
        window.css('display', 'block');
        $("#input-title-insert-para-window").focus();

    });
    $("#btn-close-insert-para-window").click(function () {
        var window = $("#new-para-window");
        window.css('display', 'none');
    });
    $("#btn-save-insert-para-window").click(function () {
        var text = $("#input-text-insert-para-window").val();

        text = text.replace(/\n/gi, "<br /> \n");
        ID++;
        var html = $(
                    '<div id="p_' + ID + '" class="item par">'
                    + '<p>' + text + '</p>'
                    + '<input type="button" name="" value="x" class="delete" />'
                    + '</div>'
                );
        var list = $('#items');
        $(list).append(html);
        setup_delete_para_item(html);
        setup_edit_para(html);
        setup_arrange_item(html);//contentedit
        var window = $("#new-para-window");
        window.css('display', 'none');

    });
}
function setup_delete_para_item(item) {
    $(item).find('.delete').click(function () {
        $(item).remove();
    });
}


/// edit para
//contentedit-start
function setup_edit_para(element) {
    $(document).keyup(function (e) {
        if (e.keyCode == 27) {
            var window = $("#edit-para-window");
            window.css('display', 'none');
        }
    });

    $(element).dblclick(function () {
        var window = $("#edit-para-window");

        $("#input-text-edit-para-window").val($(element).find("p").first().text());
        window.css('display', 'block');
        $("#input-title-edit-para-window").focus();

        $(window).attr("data-id", $(element).attr('id'));

    });

}
function init_edit_para() {

    $("#btn-close-edit-para-window").click(function () {
        var window = $("#edit-para-window");
        window.css('display', 'none');
    });
    $("#btn-save-edit-para-window").click(function () {
        var text = $("#input-text-edit-para-window").val();

        text = text.replace(/\n/gi, "<br /> \n");
        ID++;
        var html = $(
                    '<div id="p_' + ID + '" class="item par">'
                    + '<p>' + text + '</p>'
                    + '<input type="button" name="" value="x" class="delete" />'
                    + '</div>'
                );


        var window = $("#edit-para-window");
        $("#" + (window).attr("data-id")).before(html);
        $("#" + (window).attr("data-id")).remove();
        window.css('display', 'none');
        setup_delete_para_item(html);
        setup_edit_para(html);

    });

}
//contentedit-end

//group subgroup
function setup_group() {
    var result = poll.f2();
    if (result.result.code != 0) {
        Siml.messages.add({ text: result.result.message, type: "error" });
        return;
    }
    var grp = $("#grp");
    var grp_combo = grp.parent();
    grp_combo.html('');
    var html = '<div id="grp" class=" select">';
    $.each(result.groups, function (index, el) {
        html += '<span class="option" value="' + el.ID + '">' + el.title + '</span>';
    });
    html += "</div>";
    var c = $(html);
    $(grp_combo).html(html);
    $("#grp").simlComboBox();
    var groupId = $("#grp").attr('value');
    getAllSubGroupForCombo(groupId);
    $("#grp").on("onChanged", function () {
        var groupId = $(this).attr('value');
        getAllSubGroupForCombo(groupId);
    });
}
function getAllSubGroupForCombo(groupId) {
    var result = poll.f3(groupId);
    if (result.result.code != 0) {
        Siml.messages.add({ text: result.result.message, type: "error" });
        return;
    }
    var sgrp = $("#sgrp");
    var sgrp_combo = sgrp.parent();
    sgrp_combo.html('');
    var shtml = '<div id="sgrp" class=" select">';

    $.each(result.subGroup, function (index, el) {
        shtml += '<span class="option" value="' + el.ID + '">' + el.title + '</span>';
    });
    shtml += "</div>";

    $(sgrp_combo).html(shtml);
    $("#sgrp").simlComboBox();
}
function setup_add_group_subgroup() {
    var btn_save_add_group = $("#btn-save-add-group");

    btn_save_add_group.click(function () {

        var subgroupId = $("#sgrp").attr('value');
        var groupTitle = $("#grp").find('.selected').html();
        var subgroupTitle = $("#sgrp").find('.selected').html();

        if (subgroupId == -1 || subgroupId == "-1") {
            Siml.messages.add({ text: "زیرگروهی انتخاب نشده است", type: "error" });
            return;
        }
        var canInsert = true;

        $.each($("#sub-left").find('.record'), function (index, el) {

            if ($(el).attr('itemid') == subgroupId + "") {
                canInsert = false;
            }

        });
        if (canInsert) {
            var html = $('<div class="record" itemid="' + subgroupId + '">'
            + '<span>' + groupTitle + " / " + subgroupTitle + '</span>'
            + '<label class="delete">X</label>'
            + '</div>');

            $(html).find('.delete').click(function () {
                $(this).parent().remove();
            });

            $("#sub-left").append(html);
        }

    });
}

//tag
function setup_tag() {
    var result = poll.f4();
    if (result.result.code != 0) {
        Siml.messages.add({ text: result.result.message, type: "error" });
        return;
    }
    var grp = $("#tag");
    var grp_combo = grp.parent();
    grp_combo.html('');
    var html = '<div id="tag" class=" select">';
    $.each(result.tag, function (index, el) {
        html += '<span class="option" value="' + el.ID + '">' + el.title + '</span>';
    });
    html += "</div>";
    var c = $(html);
    $(grp_combo).html(html);
    $("#tag").simlComboBox();
    var groupId = $("#tag").attr('value');

    var btn_save_add_tag = $("#btn-save-add-tag");

    btn_save_add_tag.click(function () {

        var tagId = $("#tag").attr('value');
        var tagTitle = $("#tag").find('.selected').html();

        var canInsert = true;

        $.each($("#sub-left-tag").find('.record'), function (index, el) {

            if ($(el).attr('itemid') == tagId + "") {
                canInsert = false;
            }

        });
        if (canInsert) {
            var html = $('<div class="record" itemid="' + tagId + '">'
            + '<span>' + tagTitle + '</span>'
            + '<label class="delete">X</label>'
            + '</div>');

            $(html).find('.delete').click(function () {
                $(this).parent().remove();
            });

            $("#sub-left-tag").append(html);
        }

    });

}

function setup_save_all() {
    $("#btn-save-all").click(function () {
        var date = $("#date1");
        var date2 = $("#date2");
        var tagstring = $("#tagstring");

        if (date.val() == "") {
            Siml.messages.add({ text: "تاریخ شروع نظرسنجی  را وارد کنید", type: "error" });
            return;
        }

        if (date2.val() == "") {
            Siml.messages.add({ text: "تاریخ پایان نظرسنجی  را وارد کنید", type: "error" });
            return;
        }

        var subgroups = [];
        var subs = $("#sub-left").find('.record');
        if ($("#fchk_forall").is(':checked') == false) {
            $.each(subs, function (index, el) {
                var itemID = $(el).attr('itemid');
                subgroups.push(itemID);
            });
        }

        var tags = [];
        var subs2 = $("#sub-left-tag").find('.record');
        if ($("#fchk_noTag").is(':checked') == false) {
            $.each(subs2, function (index, el) {
                var itemID = $(el).attr('itemid');
                tags.push(itemID);
            });
        }
        var options = [];
        var opel = $("#option-list").find('.option-record');

        $.each(opel, function (index, el) {
            options.push($(el).val());
        });

        if (options.length < 2) {
            Siml.messages.add({ text: "درج حداقل دو گزینه الزامی است", type: "error" });
            return;
        }
        var form = new FormData();
        form.append("date", date.val());
        form.append("date2", date2.val());
        form.append("tagstring", tagstring.val());
        form.append("subgroups", JSON.stringify(subgroups));
        form.append("tags", JSON.stringify(tags));
        form.append("options", JSON.stringify(options));

        var its = $("#items").find('.item');
        if ($(its).length == 0) {
            Siml.messages.add({ text: "درج حداقل یک آیتم الزامی می باشد", type: "error" });
            return;
        }
        $.each(its, function (index, el) {

            if ($(el).hasClass("img")) {
                var id = "u_" + $(el).attr('id').replace('i_', '');
                var file = document.getElementById(id).files[0];
                form.append("img_" + index, file);

            }
            else if ($(el).hasClass("par")) {
                var p = "<p>" + $(el).find('p').html() + '</p>';
                form.append("par_" + index, p);
            }

        });

        $("#bar").css('width', '0%');
        $("#bar").on("onFinished", function (event, result) {
            $("#upload-news-window").css('display', 'none');
            try {
                if (result.code == 0) {
                    Siml.messages.add({ type: "success", text: "نظرسنجی  با موفقیت درج شد" });
                    setTimeout(function () { window.location.href = "poll.aspx"; }, 1500);
                }
                else {
                    Siml.messages.add({ type: "error", text: result.message });
                }
            } catch (e) {
                Siml.messages.add({ type: "error", text: result.message });
            }
        });
        $("#upload-news-window").css('display', 'block');
        poll.f1(form, $("#progress"), $("#bar"));
    });

}

//contentedit-start
function setup_arrange_item(element) {

    $(element).find('.up').click(function () {

        var index = $(".items").index($(element));
        if (index == 0) {
            return;
        }
        $(element).prev().before($(element));

    });
    $(element).find('.down').click(function () {
        var index = $(".items").index($(element));
        if (index == $(".items").length - 1) {
            return;
        }
        $(element).next().after($(element));
    });
}
//contentedit-end
