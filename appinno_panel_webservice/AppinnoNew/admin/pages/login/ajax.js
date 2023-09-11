var login = {

    urls: [
                    "login.aspx/isExistUser" //بررسی صحت درست بودن یا نبودن یوزر و پسورد 
    ],

    //بررسی صحت درست بودن یا نبودن یوزر و پسورد
    f1: function (email, password) {
        var result;

        $.ajax({
            type: "POST",
            url: login.urls[0],
            data: JSON.stringify({ emailtell: email, password: password }),
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