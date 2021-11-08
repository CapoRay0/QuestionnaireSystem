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
            string str = "~~~歡迎來到我的動態問卷系統~~~";
            string text = "<MARQUEE>" + str + "</MARQUEE>";
            this.lblMarquee.Text = text;
        }

        /// <summary>
        /// 進入前台 - 問卷填寫
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnGeneral_Click(object sender, EventArgs e)
        {
            Response.Redirect("GeneralUserPages/GList.aspx");
        }

        /// <summary>
        /// 登入後台 - 問卷管理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnSystem_Click(object sender, EventArgs e)
        {
            Response.Redirect("Login.aspx");
        }
    }
}