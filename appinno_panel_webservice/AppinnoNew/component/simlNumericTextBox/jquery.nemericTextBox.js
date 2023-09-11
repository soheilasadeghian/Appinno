jQuery.fn.simlNumericTextBox = function () {
    return this.each(function () {
        var _this = this;
        
        $(_this).wrapAll("<div style='overflow:hidden' />");

        $(_this).width($(_this).parent().width()-12);

        $(_this).bind("paste", function (e) {
            e.preventDefault();
            return false;
        });

        $(_this).bind("keydown", function (event) {
            
            if (event.keyCode == 46 || event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 27 || event.keyCode == 13 ||
                // Allow: Ctrl+A
                (event.keyCode == 65 && event.ctrlKey === true) ||

                // Allow: Ctrl+C
                (event.keyCode == 67 && event.ctrlKey === true) ||

                // Allow: Ctrl+V
                (event.keyCode == 86 && event.ctrlKey === true) ||

                // Allow: home, end, left, right
                (event.keyCode >= 35 && event.keyCode <= 39)) {
                // let it happen, don't do anything
                return;
            } else {
                // Ensure that it is a number and stop the keypress
                if (event.shiftKey || (event.keyCode < 48 || event.keyCode > 57) && (event.keyCode < 96 || event.keyCode > 105)) {
                    event.preventDefault();
                    return false;
                }
            }
        });

    });
}
