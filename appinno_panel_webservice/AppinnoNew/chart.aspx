<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="chart.aspx.cs" Inherits="AppinnoNew.chart" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
   
    <script src="component/jquery/jquery-1.11.3.min.js"></script>
     <script src="component/jquery/jquery-migrate-1.0.0.js"></script>
    <script src="component/chart/highcharts.js"></script>
    <script src="component/chart/exporting.js"></script>
    <script src="admin/component/js/Core.js"></script>
    <script src="pages/chart/js.js"></script>
    <script src="pages/chart/ajax.js"></script>
    <link href="pages/chart/css.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <div class="chart" id="line_chart" ></div>
            <div class="vertical_small"></div>
            <div class="chart" id="bar_chart" ></div>
            <div class="vertical_small"></div>
            <div class="chart" id="pie_chart" ></div>
        </div>
    </form>
</body>
</html>
