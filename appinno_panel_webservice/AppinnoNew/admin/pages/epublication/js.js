var itemID;
var currentItem;
var ID = 0;

$(function () {
    ID = 0;
    Tipped.create('.hint,.tools .button');
    $('#date1').datepicker({ isRTL: false, showButtonPanel: true });
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
    setup_insert_image();
    setup_insert_video();
    setup_insert_para();
    setup_group();
    setup_tag();
    setup_add_group_subgroup();
    init_edit_para(); //contentedit
    check_queryString();
    loadData();

    setup_save_all();
});

/// set currentItem
function check_queryString() {
    itemID = Siml.page.url.getParam('id');
    var int = parseInt(itemID, 10);

    if (itemID == undefined || isNaN(int)) {

        $('body').html("");
        Siml.messages.add({ type: "error", text: "صفحه مورد نظر نادرست فراخوانی شده است." });
        setTimeout(function myfunction() {
            Siml.page.url.goto('publicationlist.aspx');
        }, 2500);

    }
    else {

        var result = epublication.f4(itemID);


        if (result.result.code != 0) {

            $('body').html("");
            Siml.messages.add({ type: "error", text: result.result.message });
            setTimeout(function myfunction() {
                Siml.page.url.goto('publicationlist.aspx');
            }, 2500);
        }
        else {
            currentItem = result.publication;
        }
    }
}
function loadData() {
    $("#main-title").val(currentItem.title);
    $('#date1').val(currentItem.publishDate);

    var list = $('#items');
    $(list).html('');
    $.each(currentItem.contents, function (index, el) {
        var record;

        if (el.type == 0) {
            record = $('<div class="item par" id="lp_' + el.ID + '" >'
                + el.value
                + '<input type="button" class="delete" title="حذف" value="x" />'
                //contentedit-start
                + '<input type="button" class="up" title="بالا" value="▲" />'
                + '<input type="button" class="down" title="پایین" value="▼" />'
                + '</div>');
                setup_delete_para_item(record);
                setup_arrange_item(record);
                setup_edit_para(record);
                //contentedit-end
        }
        else if (el.type == 1) {
            record = $(
                        '<div id="li_' + (el.ID) + '" class="item img" >'
                        + '<img class="image" src="' + el.value + '"  />'
                        + '<input type="button" class="delete" title="حذف" value="x" />'
                        //contentedit-start
                        + '<input type="button" class="up" title="بالا" value="▲" />'
                        + '<input type="button" class="down" title="پایین" value="▼" />'
                        + '</div>'
                        );
            setup_delete_image_item(record);
            setup_arrange_item(record);
            //contentedit-end
        }
        else if (el.type == 2) {
            record = $('<div class="item vid" id="lv_' + el.ID + '"  >' +
                        '<video class="loaded_video"  controls>' +
                        '<source  src="' + el.value + '" type="video/mp4" />' +
                        '</video>' +
                        '<input type="button" class="delete"  title="حذف" value="x" />' +
                         //contentedit-start
                        '<input type="button" class="up" title="بالا" value="▲" />' +
                        '<input type="button" class="down" title="پایین" value="▼" />' +
                        '</div>');
            setup_delete_video_item(record);
            setup_arrange_item(record);
            //contentedit-end
        }
        $(list).append(record);

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
function setup_delete_image_item(item) {
    $(item).find(".delete").click(function () {
        var id = $(this).parent().attr('id').replace('li_', "").replace('i_', "");
        $("#u_" + id).remove();
        $(item).remove();
    });
}
function setup_delete_video_item(item) {

    $(item).find(".delete").click(function () {
        var id = $(this).parent().attr('id').replace('lv_', "").replace('v_', "");
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
                    //contentedit-start
                    + '<input type="button" class="up" title="بالا" value="▲" />'
                    + '<input type="button" class="down" title="پایین" value="▼" />'
                    //contentedit-end
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

/// insert video

function setup_insert_video() {
    video_initOnChange();
}
function video_initOnChange() {
    document.getElementById('file_video').addEventListener('change', null, false);
    document.getElementById('file_video').addEventListener('change', function (e) {

        var fileName = e.target.files[0].name;
        var ext = fileName.substring(fileName.lastIndexOf('.'), fileName.length);

        if (ext.toLowerCase() != '.mp4')
            return alert("نوع فایل های مجاز : \nmp4");

        if (this.disabled)
            return alert('فایل پشتیبانی نمی شود!');
        var F = this.files;

        if (F && F[0]) for (var i = 0; i < F.length; i++) {
            ID++;

            var html = $(
                '<div id="v_' + ID + '" class="item vid" >'
                + '<img  class="video"/>'
                + '<span class="title">' + fileName + '</span>'
                + '<input type="button" class="delete" title="حذف" value="x" />'
                //contentedit-start
                + '<input type="button" class="up" title="بالا" value="▲" />'
                + '<input type="button" class="down" title="پایین" value="▼" />'
                //contentedit-end
                + '</div>'
                );

            var uploaded = $('#uploaded');

            var pfi = $("#file_video").parent();
            $("#file_video").attr('id', id = "u_" + ID);
            pfi.append('<input class="file_video" type="file" style="display: none" id="file_video" />');
            $(uploaded).append($("#file_video"));

            var list = $('#items');
            video_initOnChange();
            $(list).append(html);
            setup_delete_video_item(html);
            setup_arrange_item(html);//contentedit
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
        $("#input-title-insert-para-window").val('');
        $("#input-text-insert-para-window").val('');
        window.css('display', 'block');
        $("#input-title-insert-para-window").focus();
    });
    $("#btn-close-insert-para-window").click(function () {
        var window = $("#new-para-window");
        window.css('display', 'none');
    });
    $("#btn-save-insert-para-window").click(function () {
        var title = $("#input-title-insert-para-window").val();
        var text = $("#input-text-insert-para-window").val();
        if (title == "" && text == "") {
            Siml.messages.add({ text: "عنوان یا متن پاراگراف را وارد کنید.", type: "error" });
            return;
        }
        text = text.replace(/\n/gi, "<br /> \n");
        ID++;
        var html = $(
                    '<div id="p_' + ID + '" class="item par">'
                    + '<h2>' + title + '</h2>'
                    + '<p>' + text + '</p>'
                    + '<input type="button" name="" value="x" class="delete" />'
                    //contentedit-start
                    + '<input type="button" class="up" title="بالا" value="▲" />'
                    + '<input type="button" class="down" title="پایین" value="▼" />'
                    //contentedit-end
                    + '</div>'
                );
        var list = $('#items');
        $(list).append(html);
        setup_delete_para_item(html);
        setup_edit_para(html);//contentedit
        setup_arrange_item(html);//contentedit
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

        $("#input-title-edit-para-window").val($(element).find("h2").first().text());
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
            Siml.messages.add({ text: "عنوان یا متن پاراگراف را وارد کنید.", type: "error" });
            return;
        }
        text = text.replace(/\n/gi, "<br /> \n");
        ID++;
        var html = $(
                    '<div id="p_' + ID + '" class="item par">'
                    + '<h2>' + title + '</h2>'
                    + '<p>' + text + '</p>'
                    + '<input type="button" name="" value="x" class="delete" />'
                    + '<input type="button" class="up" title="بالا" value="▲" />'
                    + '<input type="button" class="down" title="پایین" value="▼" />'
                    + '</div>'
                );


        //var list = $('#items');
        //$(list).append(html);
        //setup_delete_para_item(html);
        //setup_edit_paragraph(html);
        var window = $("#edit-para-window");
        $("#" + (window).attr("data-id")).before(html);
        $("#" + (window).attr("data-id")).remove();
        window.css('display', 'none');
        setup_delete_para_item(html);
        setup_edit_para(html);
        setup_arrange_item(html);//contentedit
    });

}
//contentedit-end

//group subgroup
function setup_group() {
    var result = epublication.f2();
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
    var result = epublication.f3(groupId);
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
    var result = epublication.f5();
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
        var title = $("#main-title");
        var date = $("#date1");
        var tagstring = $("#tagstring");

        if (title.val() == "") {
            Siml.messages.add({ text: "نام انتشارات را وارد کنید", type: "error" });
            return;
        }
        if (date.val() == "") {
            Siml.messages.add({ text: "تاریخ انتشار انتشارات را وارد کنید", type: "error" });
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
        var form = new FormData();
        form.append("publicationID", currentItem.ID);
        form.append("title", title.val());
        form.append("date", date.val());
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

            if ($(el).hasClass("vid") && $(el).attr('id').indexOf('lv_') < 0) {
                var id = "u_" + $(el).attr('id').replace('v_', '');
                var file = document.getElementById(id).files[0];
                form.append("vid_" + index, file);
            }
            else if ($(el).hasClass("vid") && $(el).attr('id').indexOf('lv_') >= 0) {
                var id = $(el).attr('id').replace('lv_', '');
              
                form.append("lvid_" + index, id);
            }
            else if ($(el).hasClass("img") && $(el).attr('id').indexOf('li_') < 0) {
                var id = "u_" + $(el).attr('id').replace('i_', '');
                var file = document.getElementById(id).files[0];
                form.append("img_" + index, file);
            }
            else if ($(el).hasClass("img") && $(el).attr('id').indexOf('li_') >= 0) {
                var id = $(el).attr('id').replace('li_', '');
                form.append("limg_" + index, id);
            }
            else if ($(el).hasClass("par") && $(el).attr('id').indexOf('lp_') < 0) {
                var h = "<h2>" + $(el).find('h2').html() + '</h2>';
                var p = "<p>" + $(el).find('p').html() + '</p>';
                form.append("par_" + index, h + p);
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
                    Siml.messages.add({ type: "success", text: "انتشارات با موفقیت ویرایش شد" });
                    setTimeout(function () { window.location.href = "publicationlist.aspx"; }, 1500);
                }
                else {
                    Siml.messages.add({ type: "error", text: result.message });
                }
            } catch (e) {
                Siml.messages.add({ type: "error", text: result.message });
            }
        });
        $("#upload-news-window").css('display', 'block');
        epublication.f1(form, $("#progress"), $("#bar"));
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


