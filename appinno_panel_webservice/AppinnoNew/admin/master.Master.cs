using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AppinnoNew.admin
{
    public partial class master : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (ClassCollection.Methods.isLogin(Context))
            {
                fullname.InnerText = ClassCollection.Methods.getUserInSession(Context).name + " " + ClassCollection.Methods.getUserInSession(Context).family;
            }
            else
            {
                Response.Redirect("login.aspx");
            }
        }
    }
}