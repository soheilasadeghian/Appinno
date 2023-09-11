var itemID;
var currentItem;
var ID = 0;

$(function () {
    ID = 0;
    Tipped.create('.hint,.tools .button');

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

    setup_group();
    setup_tag();
    setup_add_group_subgroup();
    init_edit_para(); //contentedit
    check_queryString();
    loadData();
    setup_save_all();

    if (currentItem.canEdit) {
        $('#date1').datepicker({ isRTL: false, showButtonPanel: true });
        $('#date2').datepicker({ isRTL: false, showButtonPanel: true });
        setup_insert_image();
        setup_insert_para();
        setup_option();
    }
    else {
        $("#fchk_forall").prop("disabled", true);
        $("#fchk_toGroup").prop("disabled", true);
        $("#fchk_noTag").prop("disabled", true);
        $("#fchk_hasTag").prop("disabled", true);
        $("#op-tool").remove();
        $("#btn-save-add-tag").remove();
        $("#btn-save-add-group").remove();
        $("#sub-left-tag").find(".delete").remove();
        $("#sub-left").find(".delete").remove();
        $('#date1').attr("readonly", "readonly");
        $('#date2').attr("readonly", "readonly");
    }
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
        ID++;
        var html = $(
        '<div class="op-item">'
        + '<input type="text" id="' + ID + '" class="option-record" name="name" value="' + newOption + '" />'
        + '<input  type="button" class="remove-option" value="ˣ" />'
        + '</div>');

        lst.append(html);
        setup_remove_option(html);
        $("#new-option-input").val("");
        $("#new-option-input").focus();

    });
}
/// set currentItem
function check_queryString() {
    itemID = Siml.page.url.getParam('id');
    var int = parseInt(itemID, 10);

    if (itemID == undefined || isNaN(int)) {

        $('body').html("");
        Siml.messages.add({ type: "error", text: "صفحه مورد نظر نادرست فراخوانی شده است." });
        setTimeout(function myfunction() {
            Siml.page.url.goto('polllist.aspx');
        }, 2500);

    }
    else {

        var result = epoll.f4(itemID);


        if (result.result.code != 0) {

            $('body').html("");
            Siml.messages.add({ type: "error", text: result.result.message });
            setTimeout(function myfunction() {
                Siml.page.url.goto('polllist.aspx');
            }, 2500);
        }
        else {
            currentItem = result.poll;
        }
    }
}
function loadData() {
    $("#main-title").val(currentItem.title);
    $('#date1').val(currentItem.startDate);
    $('#date2').val(currentItem.finishedDate);
    var list = $('#items');
    $(list).html('');
    $.each(currentItem.content, function (index, el) {
        var record;

        if (el.type == 0) {
            record = $('<div class="item par" id="lp_' + el.ID + '" >'
                + el.value
                + '<input type="button" class="delete" title="حذف" value="x" />'
                //contentedit-start
               );
            if (currentItem.canEdit) {
                setup_delete_para_item(record);

                setup_edit_para(record); addEventListener
            }
            //contentedit-end
        }
        else if (el.type == 1) {
            record = $(
                        '<div id="li_' + (el.ID) + '" class="item img" >'
                        + '<img class="image" src="' + el.value + '"  />'
                          + '<input type="button" class="delete" title="حذف" value="x" />'
                        );
            if (currentItem.canEdit)
                setup_delete_image_item(record);
            //contentedit-end
        }

        $(list).append(record);

    });
    if (currentItem.canEdit == false)
        $(list).find('.delete').remove();

    var lst = $("#option-list");

    $.each(currentItem.option, function (index, el) {
        var cr = "";
        if (currentItem.canEdit)
            cr = '<input type="text" id="lo_' + el.ID + '" class="option-record" name="name" value="' + el.text + '" />' + '<input  type="button" class="remove-option" value="ˣ" />';
        else
            cr = '<input type="text" id="lo_' + el.ID + '" readonly class="option-record" name="name" value="' + el.text + '" />' + '<input  type="button" class="remove-option" value="' + el.count + '" />';

        var html = $(
           '<div class="op-item">'

           + cr
           + '</div>');

        lst.append(html);
        if (currentItem.canEdit) {
            setup_remove_option(html);
        }
    });

    $("#fchk_block").prop("checked", currentItem.isBlock);

    if (currentItem.groups.length == 0) {
        $("#fchk_forall").prop("checked", true);
    } else {

        $("#fchk_toGroup").prop("checked", true);
        $("#fchk_toGroup").trigger('change');

        $.each(currentItem.groups, function (index, el) {

            var html = $('<div class="record" itemid="' + el.subGroupID + '">'
                + '<span>' + el.groupTitle + " / " + el.subGroupTitle + '</span>'
                + '<label class="delete">X</label>'
                + '</div>');

            $(html).find('.delete').click(function () {
                $(this).parent().remove();
            });

            $("#sub-left").append(html);
        });
    }

    if (currentItem.tag.length == 0) {
        $("#fchk_noTag").prop("checked", true);
    } else {

        $("#fchk_hasTag").prop("checked", true);
        $("#fchk_hasTag").trigger('change');

        $.each(currentItem.tag, function (index, el) {

            var html = $('<div class="record" itemid="' + el.ID + '">'
                + '<span>' + el.title + '</span>'
                + '<label class="delete">X</label>'
                + '</div>');

            $(html).find('.delete').click(function () {
                $(this).parent().remove();
            });

            $("#sub-left-tag").append(html);
        });
    }
}
function setup_remove_option(el) {
    $(el).find(".remove-option").click(function () {
        $(this).parent().remove();
    });
}
function setup_delete_image_item(item) {
    $(item).find(".delete").click(function () {
        var id = $(this).parent().attr('id').replace('li_', "").replace('i_', "");
        $("#u_" + id).remove();
        $(item).remove();
    });
}

function setup_delete_para_item(item) {
    $(item).find(".delete").click(function () {
        var id = $(this).parent().attr('id').replace('lp_', "").replace('p_', "");
        $("#u_" + id).remove();
        $(item).remove();
    });
}

/// insert image
function setup_insert_image() {
    image_initOnChange();
}
function image_initOnChange() {
    document.getElementById('file_image').addEventListener('change', null, false);
    document.getElementById('file_image').addEventListener('change', function (e) {

        if (currentItem.canEdit == false)
            return;

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
            });
        }
    }, false);
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
        if (text == "") {
            Siml.messages.add({ text: "متن پاراگراف را وارد کنید.", type: "error" });
            return;
        }
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
        setup_edit_para(html);//contentedit
        var window = $("#new-para-window");
        window.css('display', 'none');

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
        var title = $("#input-title-edit-para-window").val();
        var text = $("#input-text-edit-para-window").val();
        if (title == "" && text == "") {
            Siml.messages.add({ text: "متن پاراگراف را وارد کنید.", type: "error" });
            return;
        }
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
    var result = epoll.f2();
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
//tag
function setup_tag() {
    var result = epoll.f5();
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

function getAllSubGroupForCombo(groupId) {
    var result = epoll.f3(groupId);
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

function setup_save_all() {
    $("#btn-save-all").click(function () {

        var date = $("#date1");
        var date2 = $("#date2");
        var tagstring = $("#tagstring");

        if (date.val() == "") {
            Siml.messages.add({ text: "تاریخ شروع نظرسنجی را وارد کنید", type: "error" });
            return;
        }
        if (date2.val() == "") {
            Siml.messages.add({ text: "تاریخ پایان نظرسنجی را وارد کنید", type: "error" });
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

        var opel = $("#option-list").find('.option-record');
        if (opel.length < 2) {
            Siml.messages.add({ text: "درج حداقل دو گزینه الزامی است", type: "error" });
            return;
        } var form = new FormData();

        $.each(opel, function (index, el) {
            var str = $(el).attr('id');
            if (str.indexOf('lo_') < 0) {
                form.append("no_" + index, $(el).val());
            }
            else if (str.indexOf('lo_') >= 0) {
                var id = $(el).attr('id').replace('lo_', '');
                form.append("lo_" + id, $(el).val());
            }
        });



        form.append("pollID", currentItem.ID);
        form.append("date", date.val());
        form.append("date2", date2.val());
        form.append("tagstring", tagstring.val());
        form.append("subgroups", JSON.stringify(subgroups));
        form.append("tags", JSON.stringify(tags));
        form.append("block", $("#fchk_block").is(':checked'));

        var its = $("#items").find('.item');
        if ($(its).length == 0) {
            Siml.messages.add({ text: "درج حداقل یک آیتم الزامی می باشد", type: "error" });
            return;
        }
        $.each(its, function (index, el) {

            if ($(el).hasClass("img") && $(el).attr('id').indexOf('li_') < 0) {
                var id = "u_" + $(el).attr('id').replace('i_', '');
                var file = document.getElementById(id).files[0];
                form.append("img_" + index, file);
            }
            else if ($(el).hasClass("img") && $(el).attr('id').indexOf('li_') >= 0) {
                var id = $(el).attr('id').replace('li_', '');
                form.append("limg_" + index, id);
            }
            else if ($(el).hasClass("par") && $(el).attr('id').indexOf('lp_') < 0) {
                var p = "<p>" + $(el).find('p').html() + '</p>';
                form.append("par_" + index,  p);
            }
            else if ($(el).hasClass("par") && $(el).attr('id').indexOf('lp_') >= 0) {
                var id = $(el).attr('id').replace('lp_', '');
                form.append("lpar_" + index, id);
            }

        });

        $("#bar").css('width', '0%');
        $("#bar").on("onFinished", function (event, result) {
            $("#upload-news-window").css('display', 'none');
            try {
                if (result.code == 0) {
                    Siml.messages.add({ type: "success", text: "نظرسنجی با موفقیت ویرایش شد" });
                    setTimeout(function () { window.location.href = "polllist.aspx"; }, 1500);
                }
                else {
                    Siml.messages.add({ type: "error", text: result.message });
                }
            } catch (e) {
                Siml.messages.add({ type: "error", text: result.message });
            }
        });
        $("#upload-news-window").css('display', 'block');
        epoll.f1(form, $("#progress"), $("#bar"));
    });

}

