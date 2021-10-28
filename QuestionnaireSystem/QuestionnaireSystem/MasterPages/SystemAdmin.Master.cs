using DBSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QuestionnaireSystem.MasterPages
{
    public partial class SystemAdmin : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnReload_Click(object sender, EventArgs e)
        {
            Response.Redirect(this.Request.RawUrl);
        }

        protected void btnToDefault_Click(object sender, EventArgs e)
        {
            Response.Redirect("../SystemAdminPages/SList.aspx");
        }

        protected void btnExit_Click(object sender, EventArgs e)
        {
            AuthManager.Logout();
            Response.Redirect("../Default.aspx");
        }

    }
}