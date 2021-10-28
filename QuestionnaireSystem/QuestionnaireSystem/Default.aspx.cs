using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QuestionnaireSystem
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string str = "~~~歡迎來到動態問卷系統~~~";
            string text = "<MARQUEE>" + str + "</MARQUEE>";
            Label1.Text = text;
        }

        protected void BtnGeneral_Click(object sender, EventArgs e)
        {
            Response.Redirect("GeneralUserPages/GList.aspx");
        }

        protected void BtnSystem_Click(object sender, EventArgs e)
        {
            Response.Redirect("Login.aspx");
        }
    }
}