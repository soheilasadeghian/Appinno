jQuery.fn.simlComboBox = function () {
    return this.each(function () {

        var _this = this;
        $(_this).wrapAll(' <div style="overflow-x:hidden">');

        $(this).addClass('select').attr('simlCombobox', '1');

        var options = $(this).find('span.option');

        var selectdOption = $(options).eq(0);
        var shtml = "<div class='list-simlcombobox'>" + $(_this).html() + "</div>";
       
        if (options.length != 0) {
            $(_this).after(shtml).html("<span class='selected' value = '" + $(selectdOption).attr('value') + "'>" + $(selectdOption).html() + "</span>");
            $(_this).attr('value', $(selectdOption).attr('value'));
        }
        else {
            $(_this).after(shtml).html("<span class='selected' value = '-1'>ندارد</span>");
            $(_this).attr('value', '-1');
        }
        var list = $(_this).next('.list-simlcombobox');
        $(list).width($(_this).parent().width() + 'px');

        $(list).css('display', 'none');

        $(_this).click(function (event) {

            var list = $(_this).next('.list-simlcombobox');

            $(list).width($(_this).parent().width() + 'px');


            if (!$(list).is(":visible")) {

                var pselects = $('.select[simlCombobox=1]');
                $.each(pselects, function (index, pselect) {
                    var plist = $(pselect).next('.list-simlcombobox');
                    $(plist).slideUp(100);
                });

                $(list).slideDown(100);
            }
            else
                $(list).slideUp(100);

        });

        options = $(_this).next('.list-simlcombobox').find('.option');
        $(options).click(function (event) {

            $(_this).find('.selected').attr('value', $(this).attr('value'));
            $(_this).attr('value', $(this).attr('value'));
            $(_this).find('.selected').html($(this).html());


            $(_this).trigger('onChanged');

            var list = $(_this).next('.list-simlcombobox');
            $(list).slideUp(100);
        });

        $(document).click(function (event) {

            var selects = $('.select[simlCombobox=1]');

            $.each(selects, function (index, select) {
                var target = $(event.target);

                if (target.attr('class') != 'selected' && target.attr('class') != 'select' && target.attr('simlCombobox') != '1') {
                    var list = $(select).next('.list-simlcombobox');
                    $(list).slideUp(100);
                }


            });

        });

    });
}
jQuery.fn.selectByValue = function (value) {

    var _this = this;

    var oo = $(_this).next('.list-simlcombobox').find('.option');
    $.each(oo, function (index, el) {

        if ($(el).attr('value') == value) {
          
            $(_this).find('.selected').attr('value', $(el).attr('value'));
            $(_this).attr('value', $(el).attr('value'));
            $(_this).find('.selected').html($(el).html());


            $(_this).trigger('onChanged');

            var list = $(_this).next('.list-simlcombobox');
            $(list).slideUp(100);
            return;
        }

    });

    return this;

};
jQuery.fn.setBackgroundColor = function (color) {

    var _this = this;
    $(_this).css('background-color', color);
    $(_this).next('.list-simlcombobox').css('background-color', color);
    var oo = $(_this).next('.list-simlcombobox').find('.option');
    $.each(oo, function (index, el) {

        $(el).css('background-color',color);

    });

    return this;

};
