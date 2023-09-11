var PageIndex = 1;
var editingAccessID = -1;
var ID = 0;

$(function () {
    ID = 0;
    $("#order").simlComboBox();
    $("#sortby").simlComboBox();
    $("#itemstate").simlComboBox();

    $('#date').datepicker({ isRTL: false, showButtonPanel: true });
    $('#dateto').datepicker({ isRTL: false, showButtonPanel: true });

    $("#lt").html("لیست پاسخ ها");
    PageIndex = 1;
    check_queryString();
    getAllItem(ID);
    search();
    //setup_edit_user_window();

});
function check_queryString() {
    itemID = Siml.page.url.getParam('id');
    var int = parseInt(itemID, 10);

    if (itemID == undefined || isNaN(int)) {

        $('body').html("");
        Siml.messages.add({ type: "error", text: "صفحه مورد نظر نادرست فراخوانی شده است." });
        setTimeout(function myfunction() {
            Siml.page.url.goto('myIranlist.aspx');
        }, 2500);

    }
    else {
        ID = itemID;
    }
}

function search() {
    $('#fillter-input').keyup(function (e) {
        if (e.keyCode == 13) {
            $("#search-btn").click();
        }
    });
    $("#date").on('change', function () {
        if ($("#chk_date").is(':checked') == false) {
            $("#date").val('');
        }
    });
    $("#dateto").on('change', function () {
        if ($("#chk_dateto").is(':checked') == false) {
            $("#dateto").val('');
        }
    });
    $("#chk_date").on('change', function () {
        if ($("#chk_date").is(':checked')) {
            $("#date").trigger('focus');
        }
        else {
            $("#date").val('');
        }
    });
    $("#chk_dateto").on('change', function () {
        if ($("#chk_dateto").is(':checked')) {
            $("#dateto").trigger('focus');
        }
        else {
            $("#dateto").val('');
        }
    });
    $("#chk_winner").on('change', function () {
        if ($("#chk_winner").is(':checked')) {
            $("#other-p").children().attr("disabled", "disabled");
        }
        else {
            $("#other-p *").prop('disabled', false);
        }
    });
    var item = $("#search-btn");
    item.click(function () {
        PageIndex = 1;
        check_queryString();
        getAllItem(ID);
 
    });

}
function getAllItem(ID) {
    var filter = $("#fillter-input").val();
    var order = $("#order").attr('value');
    var sortby = $("#sortby").attr('value');
    var date = $("#date").val();
    var dateto = $("#dateto").val();
    var responsestate = $("#itemstate").attr('value');
    var justWinner = $("#chk_winner").is(':checked');
    var result = responselist.f1(date, dateto, responsestate, filter, order, sortby, PageIndex, ID, justWinner);

    if (result.result.code != 0) {
        Siml.messages.add({ text: result.result.message, type: "error" });
        return;
    }
    var list = $("#list");
    $(list).html('');
    $("#pagetitle").html(result.pageTitle);
    if (result.winnerID != 0)
    {
        var winnerspan = document.getElementById("winnerspan");
        winnerspan.setAttribute("style", "display:block;float:left;");
        $("#winnertitle").html(result.winnerTitle);
        var link = document.getElementById("winnerID");
        link.setAttribute("href", "eresponse.aspx?id=" + result.winnerID + "&p=" + ID);
    }
    $.each(result.response, function (index, el) {
        var block = el.isBlock ? '<a itemid="' + el.ID + '" class="block" title="پاسخ مسدود شده است"></a>' : "";
        var html =
                    $('<div class="item" itemid="' + el.ID + '">'
                    + '<div class="tools">'
                    //+ '<a itemid="' + el.ID + '" class="delete"></a>'
                    + '<a  href="eresponse.aspx?id=' + el.ID + '&p=' + ID + '" class="edit" title="ویرایش"></a>'
                    + block
                    + '</div>'
                    + '<span class="title">' + el.title + '</span>'
                    + '<span class="mUserName">' + el.mUserName + '</span>'
                    + '<span class="date">' + el.regDate + '</span>'
                    + '</div>');

        $(list).append(html);
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
                getAllItem(ID);
            }
        });
    }

}

