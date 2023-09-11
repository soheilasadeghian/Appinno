var itemID;
var currentItem;
var ID = 0;

$(function () {
    ID = 0;
    Tipped.create('.hint,.tools .button');
    $('#date1').datepicker({ isRTL: false, showButtonPanel: true });
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
        setTimeout(function myfunction() {
            Siml.page.url.goto('responselist.aspx');
        }, 2500);

    }
    else {

        var result = eresponse.f4(itemID);

        if (result.result.code != 0) {

            $('body').html("");
            Siml.messages.add({ type: "error", text: result.result.message });
            setTimeout(function myfunction() {
                Siml.page.url.goto('responselist.aspx');
            }, 2500);
        }
        else {
            currentItem = result.response;
        }
    }
}
function loadData() {
    $("#main-title").val(currentItem.title);
    $("#main-fullname").html("محتوای پاسخ  (" + currentItem.fullname + ")");
    $('#date1').val(currentItem.publishDate);

    $("#pagetitle").attr("href", "responselist.aspx?id=" + currentItem.myIranID);

    var list = $('#items');
    $(list).html('');
    $.each(currentItem.contents, function (index, el) {
        var record;

        if (el.type == 0) {
            record = $('<div class="item par" id="lp_' + el.ID + '" >'
                + el.value
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
                        '</div>');
            setup_delete_video_item(record);
            setup_arrange_item(record);
            //contentedit-end
        }
        $(list).append(record);

    });

    $("#fchk_block").prop("checked", currentItem.isBlock);
    $("#fchk_iscorrect").prop("checked", currentItem.isCorrect);
    $("#fchk_iswinner").prop("checked", currentItem.isWinner);

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



function setup_save_all() {
    $("#btn-save-all").click(function () {
        var title = $("#main-title");

        if (title.val() == "") {
            Siml.messages.add({ text: "عنوان اصلی پاسخ را وارد کنید", type: "error" });
            return;
        }


        var form = new FormData();
        form.append("responseID", currentItem.ID);
        form.append("title", title.val());
        form.append("block", $("#fchk_block").is(':checked'));
        form.append("iscorrect", $("#fchk_iscorrect").is(':checked'));
        form.append("iswinner", $("#fchk_iswinner").is(':checked'));

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


        if ($("#fchk_iswinner").is(':checked'))
            if (!confirm('آیا از اعلام نمودن برنده مسابقه اطمینان دارید؟')) {
                return;
            }

        $("#bar").css('width', '0%');
        $("#bar").on("onFinished", function (event, result) {
            $("#upload-news-window").css('display', 'none');
            try {
                if (result.code == 0) {
                    Siml.messages.add({ type: "success", text: "پاسخ با موفقیت ویرایش شد" });
                    setTimeout(function () { window.location.href = "responselist.aspx?id=" + Siml.page.url.getParam("p") }, 1500);
                }
                else {
                    Siml.messages.add({ type: "error", text: result.message });
                }
            } catch (e) {
                Siml.messages.add({ type: "error", text: result.message });
            }
        });
        $("#upload-news-window").css('display', 'block');
        eresponse.f1(form, $("#progress"), $("#bar"));
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


