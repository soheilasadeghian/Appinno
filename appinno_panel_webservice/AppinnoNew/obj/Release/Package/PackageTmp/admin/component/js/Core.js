var Siml = {
    page: {
        url: {

            getParam: function (sParam) {
                var sPageURL = decodeURIComponent(window.location.search.substring(1)), sURLVariables = sPageURL.split('&'), sParameterName, i;

                for (i = 0; i < sURLVariables.length; i++) {
                    sParameterName = sURLVariables[i].split('=');

                    if (sParameterName[0] === sParam) {
                        return sParameterName[1] === undefined ? true : sParameterName[1];
                    }
                }

            },
            goto: function (href) {

                window.location.href = href;

            },
            setTitle: function (value) {
                window.document.title = value;

            }
        },
        loadIn: function (element) {

            var divcss = "width:100%; height:30px;text-align:center;padding:10px";
            var imagecss = "width:30px; height:30px;margin:auto";
            var tag = "<div style='" + divcss + "' ><img  src='component/images/loading.gif' style='" + imagecss + "' />";

            $(element).html(tag);
        },
        readImage: function (file, result) {

            var _this = this;
            var reader = new FileReader();
            var image = new Image();
            var res = {};

            reader.readAsDataURL(file);
            reader.onload = function (_file) {
                image.src = _file.target.result;              // url.createObjectURL(file);
                image.onload = function () {
                    var w = this.width,
                        h = this.height,
                        t = file.type,                           // ext only: // file.type.split('/')[1],
                        n = file.name,
                        s = ~ ~(file.size / 1024) + 'KB';

                    res.src = this.src;
                    res.image = '<img src="' + this.src + '" /> ';;
                    res.width = w;
                    res.height = h;
                    res.type = t;
                    res.name = n;
                    res.size = s;
                    result(res);
                };
                image.onerror = function () {
                    alert('نوع فایل صحیح نمی باشد.: ' + file.type);
                };
            };

        }
    },
    messages: {

        queue: [],
        add: function (message) {

            var delay = 2000;

            var text = message.text;
            var type = message.type;

            var size = Siml.messages.queue.length;
            var destination_bottom = size * 70 + 10;


            var css = "position:fixed;"
              + "z-index:30000;"
              + "text-align:right;"
              + "padding:20px;"
              + "font-family:BYekan !important;"
              + "font-size:1.5em;"
              + "color:#fff;"
              + "height:20px;";

            css += type == "error" ? "background-color:#b51c44;" : " background-color:#009600;";

            var msg = document.createElement("div");
            $(msg).attr("style", css);
            $(msg).html('<span>' + text + '</span>');

            var width = $(msg).width();
            var bwidth = $(window).width();

            $(msg).css('bottom', '-' + 70 + 'px');
            $(msg).hide(0);

            $(msg).animate({ bottom: destination_bottom + "px" }, { queue: false });
            $(msg).fadeIn(800);
            $(msg).css('left', 10 + "px");
            $('body').append(msg);
            Siml.messages.queue.push(msg);


            setTimeout(function (element) {

                Siml.messages.queue = $.grep(Siml.messages.queue, function (o) { return !$(o).is($(element)); });
                $(element).fadeOut(200, function () {

                    $(element).remove();

                    $.each(Siml.messages.queue, function (index, el) {

                        var bottom = $(el).css('bottom').replace("px", "");
                        bottom = parseInt(bottom) - 70;
                        $(el).css('bottom', bottom + "px");

                    });

                });
            }, delay, $(msg));
        }
    },
    Access:
        {
            get: function (page) {
                var result = "";

                $.ajax({
                    type: "POST",
                    url: page + "/getaccess",
                    data: '{}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json", async: false,
                    success: function (response) {

                        var json = JSON.parse(response.d + "");
                        result = json;

                    },
                    failure: function (response) {

                        result = "error";
                    }
                });

                return result;
            }
        }
}