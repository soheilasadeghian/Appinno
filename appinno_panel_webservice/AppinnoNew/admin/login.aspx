<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="AppinnoNew.admin.login" %>

<!DOCTYPE html>


<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <script src="../component/jquery/jquery-1.11.3.min.js"></script>
    <script src="../component/jquery/jquery-migrate-1.0.0.js"></script>
    <script src="pages/login/ajax.js"></script>
    <script src="component/js/Core.js"></script>
    <script src="pages/login/js.js"></script>

    <link href="component/css/main.css" rel="stylesheet" />
    <link href="pages/login/css.css" rel="stylesheet" />
    <title>ورود به سامانه مدیریت اپی‌نو</title>
</head>
<body>
    <form id="form1" runat="server">
        <div class="login mci-1b mci-1b shadow">
            <div class="wrap">
                <div class="caption">
                    <span class="title">سامانه مدیریت محتوا </span>
                    <span class="sys-title mci-2-1c">اَپی‌نو </span>
                    <div class="vertical_min"></div>
                </div>
                <div class="vertical_min"></div>
                <div class="email">
                    <input id="email" type="text" name="email" placeholder="نام کاربری" />
                    <div class="tile mci-2b"></div>
                </div>
                <div class="password">
                    <input id="password" type="password" name="password" placeholder="کلمه عبور" />
                    <div class="tile mci-2b"></div>
                </div>

                <input id="submit" class="button mci-1-1b" type="button" name="submit" value="ورود" />
                <div class="clear"></div>
            </div>
        </div>

        <div class="vertical_max"></div>
        <div class="vertical_max"></div>
        <div class="download">
            <a class="android shadow" href="/com.production.behrad.appinno-2.apk">
                <img src="component/images/android.png" alt="android" />
                <label>دانلود اپلیکیشن</label>
                <span>Android</span>
            </a>
            <a class="ios shadow">
                <img src="component/images/apple.png" alt="ios" />
                <label>دانلود اپلیکیشن ios</label>
                <span>App Store</span>
            </a>
        </div>


    </form>
</body>
</html>
