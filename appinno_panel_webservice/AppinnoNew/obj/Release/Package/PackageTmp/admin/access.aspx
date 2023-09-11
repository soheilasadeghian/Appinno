<%@ Page Title="" Language="C#" MasterPageFile="~/admin/master.Master" AutoEventWireup="true" CodeBehind="access.aspx.cs" Inherits="AppinnoNew.admin.access" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="pages/access/css.css" rel="stylesheet" />
    <link href="component/css/search.css" rel="stylesheet" />
    <link href="../component/simlComboBox/css.css" rel="stylesheet" />
    <script src="../component/simlComboBox/jquery.comboBox.js"></script>
    <script src="pages/access/ajax.js"></script>
    <script src="pages/access/js.js"></script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="path" runat="server">
    <a href="dashboard.aspx" class="loc history">داشبورد</a>
    <a class="spacer">></a>
    <a class="loc none">مدیریت دسترسی</a>
    <a class="spacer">></a>
    <a class="loc none">مدیریت نقش ها</a>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="content" runat="server">
    <div class="vertical_min"></div>
    <div class="right-panel">
        <div class="search-box">
            <span class="search-header mci-1b">جستجو</span>
            <div class="vertical_small"></div>
            <input id="fillter-input" type="text" class="search-input" placeholder="جستجو در نقش..." value="" />
            <div class="vertical_small"></div>
            <div class="combo">
                <div id="parts" class=" select">
                    <span class="option" value="">همه</span>
                    <span class="option" value="news">اخبار</span>
                    <span class="option" value="events">رویداد</span>
                    <span class="option" value="io">معرفی سازمان</span>
                    <span class="option" value="pub">انتشارات</span>
                    <span class="option" value="chart">نمودارها</span>
                    <span class="option" value="download">دانلود مقاله و کتاب</span>
                    <span class="option" value="grouping">مدیریت گروه ها</span>
                    <span class="option" value="user">مدیریت کاربران</span>
                    <span class="option" value="access">مدیریت نقش ها</span>
                    <span class="option" value="operators">مدیریت اپراتورها</span>
                    <span class="option" value="poll">نظرسنجی</span>
                    <span class="option" value="ican">من می توانم</span>
                    <span class="option" value="bestIdeasCompetition">مسابقه ایده برتر</span>
                    <span class="option" value="creativityCompetition">مسابقه خلاقیت</span>
                    <span class="option" value="myIranCompetition">مسابقه ایران من</span>
                </div>
            </div>
            <div class="vertical_small"></div>
            <div class="combo">
                <div id="action" class=" select">
                    <span class="option" value="">همه</span>
                </div>
            </div>
            <div class="vertical_small"></div>
            <input id="search-btn" type="button" class="search-button" value="جستجو" />
        </div>
    </div>
    <div class="left-panel">
        <div class="group-list">
            <span class="header-group mci-1b">لیست نقش ها <span id="total-count"></span></span>
            <div id="list">
                <span>درحال دریافت اطلاعات...</span>
            </div>

            <div class="insert">
                <div class="pager" id="pager">
                    <a class="page">1</a>
                    <a class="page selected">2</a>
                    <a class="page">3</a>
                    <div class="clear"></div>
                </div>
                <input id="insert-btn" type="button" value="✔" class="button" />
            </div>
        </div>
    </div>
    <div class="clear"></div>

    <div id="new-access-window" class="new-access-window">
        <div class="background"></div>
        <div class="window">
            <div class="vertical_small"></div>
            <div class="access-title"><span>عنوان</span><input id="input-title-insert-access-window" type="text" value="" placeholder="عنوان نقش" /></div>
            <div class="header-row">
                <div class="cell header-title"><span>بخش</span></div>
                <div class="cell header-even"><span>نمایش لیست</span></div>
                <div class="cell header-odd">درج</div>
                <div class="cell header-even">حذف</div>
                <div class="cell header-odd">ویرایش</div>
                <div class="cell header-even">دسترسی کامل</div>
            </div>
            <div class="row">
                <div class="cell title">خبر</div>
                <div class="cell even">
                    <div class="check access showlist"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access full"></div>
                </div>
            </div>
            <div class="row">
                <div class="cell title">رویداد</div>
                <div class="cell even">
                    <div class="check access showlist"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access full"></div>
                </div>
            </div>
            <div class="row">
                <div class="cell title">معرفی سازمان</div>
                <div class="cell even">
                    <div class="check access showlist"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access full"></div>
                </div>
            </div>
            <div class="row">
                <div class="cell title">انتشارات</div>
                <div class="cell even">
                    <div class="check access showlist"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access full"></div>
                </div>
            </div>
            <div class="row">
                <div class="cell title">نمودار</div>
                <div class="cell even">
                    <div class="check access showlist"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access full"></div>
                </div>
            </div>
            <div class="row">
                <div class="cell title">دانلود مقاله و کتاب</div>
                <div class="cell even">
                    <div class="check access showlist"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access full"></div>
                </div>
            </div>
            <div class="row">
                <div class="cell title">مدیریت گروه ها</div>
                <div class="cell even">
                    <div class="check access showlist"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access full"></div>
                </div>
            </div>
            <div class="row">
                <div class="cell title">مدیریت کاربران</div>
                <div class="cell even">
                    <div class="check access showlist"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access full"></div>
                </div>
            </div>
            <div class="row">
                <div class="cell title">مدیریت نقش ها</div>
                <div class="cell even">
                    <div class="check access showlist"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access full"></div>
                </div>
            </div>
            <div class="row">
                <div class="cell title">مدیریت اپراتور ها</div>
                <div class="cell even">
                    <div class="check access showlist"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access full"></div>
                </div>
            </div>

            <div class="row">
                <div class="cell title">من می توانم</div>
                <div class="cell even">
                    <div class="check access showlist"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access full"></div>
                </div>
            </div>
            <div class="row">
                <div class="cell title">نظرسنجی</div>
                <div class="cell even">
                    <div class="check access showlist"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access full"></div>
                </div>
            </div>
            <div class="row">
                <div class="cell title">مسابقه ایده برتر</div>
                <div class="cell even">
                    <div class="check access showlist"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access full"></div>
                </div>
            </div>
            <div class="row">
                <div class="cell title">مسابقه خلاقیت</div>
                <div class="cell even">
                    <div class="check access showlist"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access full"></div>
                </div>
            </div>
            <div class="row">
                <div class="cell title">مسابقه ایران من</div>
                <div class="cell even">
                    <div class="check access showlist"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access full"></div>
                </div>
            </div>
            <div class="vertical_min"></div>
            <input id="btn-save-insert-access-window" type="button" class="save" value="✔" />
            <input id="btn-close-insert-access-window" type="button" class="save " style="font-weight:bolder;font-size:23px" value="ˣ" />
        </div>

    </div>

    <div id="edit-access-window" class="new-access-window">
        <div class="background"></div>
        <div class="window">
            <div class="vertical_small"></div>
            <div class="access-title"><span>عنوان</span><input id="input-title-edit-access-window" type="text" value="" placeholder="عنوان نقش" /></div>
            <div class="header-row">
                <div class="cell header-title"><span>بخش</span></div>
                <div class="cell header-even"><span>نمایش لیست</span></div>
                <div class="cell header-odd">درج</div>
                <div class="cell header-even">حذف</div>
                <div class="cell header-odd">ویرایش</div>
                <div class="cell header-even">دسترسی کامل</div>
            </div>
            <div class="row">
                <div class="cell title">خبر</div>
                <div class="cell even">
                    <div class="check access showlist"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access full"></div>
                </div>
            </div>
            <div class="row">
                <div class="cell title">رویداد</div>
                <div class="cell even">
                    <div class="check access showlist"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access full"></div>
                </div>
            </div>
            <div class="row">
                <div class="cell title">معرفی سازمان</div>
                <div class="cell even">
                    <div class="check access showlist"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access full"></div>
                </div>
            </div>
            <div class="row">
                <div class="cell title">انتشارات</div>
                <div class="cell even">
                    <div class="check access showlist"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access full"></div>
                </div>
            </div>
            <div class="row">
                <div class="cell title">نمودار</div>
                <div class="cell even">
                    <div class="check access showlist"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access full"></div>
                </div>
            </div>
            <div class="row">
                <div class="cell title">دانلود مقاله و کتاب</div>
                <div class="cell even">
                    <div class="check access showlist"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access full"></div>
                </div>
            </div>
            <div class="row">
                <div class="cell title">مدیریت گروه ها</div>
                <div class="cell even">
                    <div class="check access showlist"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access full"></div>
                </div>
            </div>
            <div class="row">
                <div class="cell title">مدیریت کاربران</div>
                <div class="cell even">
                    <div class="check access showlist"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access full"></div>
                </div>
            </div>
            <div class="row">
                <div class="cell title">مدیریت نقش ها</div>
                <div class="cell even">
                    <div class="check access showlist"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access full"></div>
                </div>
            </div>
            <div class="row">
                <div class="cell title">مدیریت اپراتور ها</div>
                <div class="cell even">
                    <div class="check access showlist"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access full"></div>
                </div>
            </div>
           
            <div class="row">
                <div class="cell title">من می توانم</div>
                <div class="cell even">
                    <div class="check access showlist"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access full"></div>
                </div>
            </div>
             <div class="row">
                <div class="cell title">نظرسنجی</div>
                <div class="cell even">
                    <div class="check access showlist"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access full"></div>
                </div>
            </div>
            <div class="row">
                <div class="cell title">مسابقه ایده برتر</div>
                <div class="cell even">
                    <div class="check access showlist"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access full"></div>
                </div>
            </div>
            <div class="row">
                <div class="cell title">مسابقه خلاقیت</div>
                <div class="cell even">
                    <div class="check access showlist"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access full"></div>
                </div>
            </div>
            <div class="row">
                <div class="cell title">مسابقه ایران من</div>
                <div class="cell even">
                    <div class="check access showlist"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access"></div>
                </div>
                <div class="cell odd">
                    <div class="check access"></div>
                </div>
                <div class="cell even">
                    <div class="check access full"></div>
                </div>
            </div>

            <div class="vertical_min"></div>
            <input id="btn-save-edit-access-window" type="button" class="save" value="✔" />
            <input id="btn-close-edit-access-window"type="button" class="save " style="font-weight:bolder;font-size:23px" value="ˣ" />
        </div>

    </div>

</asp:Content>
