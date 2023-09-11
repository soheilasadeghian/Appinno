$(function () {

    $('#email').keyup(function (e) {
        if (e.keyCode == 13) {
            $("#submit").click();
        }
    });
    $('#password').keyup(function (e) {
        if (e.keyCode == 13) {
            $("#submit").click();
        }
    });
    isExistUser();

});
//جهت بررسی ایمیل و کلمه عبور وارد شده
function isExistUser() {

    $("#submit").click(function () {
        var email = $("#email").val();
        var password = $("#password").val();

        var result = login.f1(email, password);

        if (result.code == 0) {
            //Siml.messages.add({ text: result.message, type: "success" });
            Siml.page.url.goto("dashboard.aspx");
        }
        else {
            Siml.messages.add({ text: result.message, type: "error" });
        }

    });

}

