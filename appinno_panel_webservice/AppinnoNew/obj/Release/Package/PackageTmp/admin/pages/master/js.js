$(function () {

    setup_toolbar();
    master_permission();

});
function setup_toolbar() {

    $(document).click(function (event) {

        var selects = $('.menu').find('.submenu');

        $.each(selects, function (index, select) {
            var target = $(event.target);
            if ($(target).parent().parent().parent().attr('class') != 'menu') {
                $('.menu').find('.submenu').css('display', 'none');
            }


        });

    });
    $("#menu_chart").click(function (e) {

        var submenu = $(this).next('div.submenu');

        var display = $(submenu).css('display') == 'block' ? true : false;
        var all_submenu = $('.menu').find('.submenu').css('display', 'none');
        if (!display) {
            var pos = $(this).position();
            var width = $(submenu).width();
            $(submenu).css('left', (pos.left - 40) + 'px');
            $(submenu).css('top', (pos.top + 60) + 'px');
            $(submenu).css('display', 'block');

        }
        else {
            $(submenu).css('display', 'none');
        }
        e.preventDefault();
        return false;

    });
    $("#menu_download").click(function (e) {

        var submenu = $(this).next('div.submenu');

        var display = $(submenu).css('display') == 'block' ? true : false;
        var all_submenu = $('.menu').find('.submenu').css('display', 'none');
        if (!display) {
            var pos = $(this).position();
            var width = $(submenu).width();
            $(submenu).css('left', (pos.left) + 'px');
            $(submenu).css('top', (pos.top + 60) + 'px');
            $(submenu).css('display', 'block');

        }
        else {
            $(submenu).css('display', 'none');
        }
        e.preventDefault();
        return false;

    });
    $("#menu_access").click(function (e) {

        var submenu = $(this).next('div.submenu');

        var display = $(submenu).css('display') == 'block' ? true : false;
        var all_submenu = $('.menu').find('.submenu').css('display', 'none');
        if (!display) {
            var pos = $(this).position();
            var width = $(submenu).width();
            $(submenu).css('left', (pos.left - 35) + 'px');
            $(submenu).css('top', (pos.top + 60) + 'px');
            $(submenu).css('display', 'block');

        }
        else {
            $(submenu).css('display', 'none');
        }
        e.preventDefault();
        return false;

    });
    $("#menu_users").click(function (e) {

        var submenu = $(this).next('div.submenu');

        var display = $(submenu).css('display') == 'block' ? true : false;
        var all_submenu = $('.menu').find('.submenu').css('display', 'none');
        if (!display) {
            var pos = $(this).position();
            var width = $(submenu).width();
            $(submenu).css('left', (pos.left - 35) + 'px');
            $(submenu).css('top', (pos.top + 60) + 'px');
            $(submenu).css('display', 'block');

        }
        else {
            $(submenu).css('display', 'none');
        }
        e.preventDefault();
        return false;

    });
    $("#menu_relations").click(function (e) {

        var submenu = $(this).next('div.submenu');

        var display = $(submenu).css('display') == 'block' ? true : false;
        var all_submenu = $('.menu').find('.submenu').css('display', 'none');
        if (!display) {
            var pos = $(this).position();
            var width = $(submenu).width();
            $(submenu).css('left', (pos.left - 35) + 'px');
            $(submenu).css('top', (pos.top + 60) + 'px');
            $(submenu).css('display', 'block');

        }
        else {
            $(submenu).css('display', 'none');
        }
        e.preventDefault();
        return false;

    });
    $("#menu_content2").click(function (e) {

        var submenu = $(this).next('div.submenu');

        var display = $(submenu).css('display') == 'block' ? true : false;
        var all_submenu = $('.menu').find('.submenu').css('display', 'none');
        if (!display) {
            var pos = $(this).position();
            var width = $(submenu).width();
            $(submenu).css('left', (pos.left - 40) + 'px');
            $(submenu).css('top', (pos.top + 60) + 'px');
            $(submenu).css('display', 'block');

        }
        else {
            $(submenu).css('display', 'none');
        }
        e.preventDefault();
        return false;

    });
    $("#menu_news").click(function (e) {

        var submenu = $(this).next('div.submenu');

        var display = $(submenu).css('display') == 'block' ? true : false;
        var all_submenu = $('.menu').find('.submenu').css('display', 'none');
        if (!display) {
            var pos = $(this).position();
            var width = $(submenu).width();
            $(submenu).css('left', (pos.left - 40) + 'px');
            $(submenu).css('top', (pos.top + 60) + 'px');
            $(submenu).css('display', 'block');

        }
        else {
            $(submenu).css('display', 'none');
        }
        e.preventDefault();
        return false;

    });
    $("#menu_event").click(function (e) {

        var submenu = $(this).next('div.submenu');

        var display = $(submenu).css('display') == 'block' ? true : false;
        var all_submenu = $('.menu').find('.submenu').css('display', 'none');
        if (!display) {
            var pos = $(this).position();
            var width = $(submenu).width();
            $(submenu).css('left', (pos.left - 35) + 'px');
            $(submenu).css('top', (pos.top + 60) + 'px');
            $(submenu).css('display', 'block');

        }
        else {
            $(submenu).css('display', 'none');
        }
        e.preventDefault();
        return false;

    });
    $("#menu_io").click(function (e) {

        var submenu = $(this).next('div.submenu');

        var display = $(submenu).css('display') == 'block' ? true : false;
        var all_submenu = $('.menu').find('.submenu').css('display', 'none');
        if (!display) {
            var pos = $(this).position();
            var width = $(submenu).width();
            $(submenu).css('left', (pos.left - 25) + 'px');
            $(submenu).css('top', (pos.top + 60) + 'px');
            $(submenu).css('display', 'block');

        }
        else {
            $(submenu).css('display', 'none');
        }
        e.preventDefault();
        return false;

    });
    $("#menu_pub").click(function (e) {

        var submenu = $(this).next('div.submenu');

        var display = $(submenu).css('display') == 'block' ? true : false;
        var all_submenu = $('.menu').find('.submenu').css('display', 'none');
        if (!display) {
            var pos = $(this).position();
            var width = $(submenu).width();
            $(submenu).css('left', (pos.left - 35) + 'px');
            $(submenu).css('top', (pos.top + 60) + 'px');
            $(submenu).css('display', 'block');

        }
        else {
            $(submenu).css('display', 'none');
        }
        e.preventDefault();
        return false;

    });
    //soheila-start-poll
    $("#menu_poem").click(function (e) {

        var submenu = $(this).next('div.submenu');

        var display = $(submenu).css('display') == 'block' ? true : false;
        var all_submenu = $('.menu').find('.submenu').css('display', 'none');
        if (!display) {
            var pos = $(this).position();
            var width = $(submenu).width();
            $(submenu).css('left', (pos.left - 45) + 'px');
            $(submenu).css('top', (pos.top + 60) + 'px');
            $(submenu).css('display', 'block');

        }
        else {
            $(submenu).css('display', 'none');
        }
        e.preventDefault();
        return false;

    });
    //soheila-end-poll

    //soheila-start-ican
    $("#menu_ican").click(function (e) {

        var submenu = $(this).next('div.submenu');

        var display = $(submenu).css('display') == 'block' ? true : false;
        var all_submenu = $('.menu').find('.submenu').css('display', 'none');
        if (!display) {
            var pos = $(this).position();
            var width = $(submenu).width();
            $(submenu).css('left', (pos.left - 45) + 'px');
            $(submenu).css('top', (pos.top + 60) + 'px');
            $(submenu).css('display', 'block');

        }
        else {
            $(submenu).css('display', 'none');
        }
        e.preventDefault();
        return false;

    });
    //soheila-end-ican

    $("#menu_competition").click(function (e) {

        var submenu = $(this).next('div.submenu');

        var display = $(submenu).css('display') == 'block' ? true : false;
        var all_submenu = $('.menu').find('.submenu').css('display', 'none');
        if (!display) {
            var pos = $(this).position();
            var width = $(submenu).width();
            $(submenu).css('left', (pos.left - 45) + 'px');
            $(submenu).css('top', (pos.top + 60) + 'px');
            $(submenu).css('display', 'block');

        }
        else {
            $(submenu).css('display', 'none');
        }
        e.preventDefault();
        return false;

    });
}

function master_permission() {
    var ac = Siml.Access.get('dashboard.aspx');

    if (ac.result.code != 0) {
        Siml.messages.add({ text: ac.result.message, type: "error" });
        return;
    }
    else {
        var per = ac.access;

        if (!per.access.access.showlist && !per.operators.access.showlist) {
            $("#menu_access").remove();
        }
        else if (!per.access.access.showlist) {
            $("#manage_access").remove();
        }
        else if (!per.operators.access.showlist) {
            $("#manage_oprator").remove();
        }

        if (!per.user.access.showlist && !per.grouping.access.showlist) {
            $("#menu_users").remove();
        } else if (!per.user.access.showlist) {

        $("#manage-userlist").remove();
        } else if (!per.grouping.access.showlist) {
            $("#manage-group").remove();
        }

        if (!per.news.access.showlist) {
            $("#menu_news").remove();
        } else if (!per.news.access.insert) {
            $("#manage-news").remove();
        }

        if (!per.events.access.showlist) {
            $("#menu_event").remove();
        } else if (!per.events.access.insert) {
            $("#manage-events").remove();
        }

        if (!per.io.access.showlist) {
            $("#menu_io").remove();
        } else if (!per.io.access.insert) {
            $("#manage-io").remove();
        }

        if (!per.publication.access.showlist) {
            $("#menu_pub").remove();
        } else if (!per.publication.access.insert) {
            $("#manage-pub").remove();
        }

        if (!per.download.access.showlist) {
            $("#menu_download").remove();
        } else if (!per.download.access.insert) {
            $("#manage-download").remove();
        }

        if (!per.chart.access.showlist) {
            $("#menu_chart").remove();
        } else if (!per.chart.access.insert) {
            $("#manage-chart").remove();
        }

        if (!per.poll.access.showlist) {
            $("#menu_poem").remove();
        } else if (!per.poll.access.insert) {
            $("#manage_poem_list").remove();
        }

        if (!per.ican.access.showlist) {
            $("#menu_ican").remove();
        } else if (!per.ican.access.insert) {
            $("#manage_ican_list").remove();
        }
        
        if (!per.news.access.insert && !per.events.access.insert && !per.io.access.insert && !per.publication.access.insert
           && !per.chart.access.insert && !per.download.access.insert && !per.poll.access.insert && !per.ican.access.insert
            && !per.creativityCompetition.access.insert && !per.myIranCompetition.access.insert && !per.bestIdeasCompetition.access.insert ) {
            $("#menu_content2").remove();
        }

        if (!per.bestIdeasCompetition.access.showlist) {
            $("#bestIdeasCompetition").remove();
        }

        if (!per.creativityCompetition.access.showlist) {
            $("#creativityCompetition").remove();
        }

        if (!per.myIranCompetition.access.showlist) {
            $("#myIran").remove();
        }

        if (per.relation == false) {
            $("#menu_relations").remove();
        }
       
    }

}