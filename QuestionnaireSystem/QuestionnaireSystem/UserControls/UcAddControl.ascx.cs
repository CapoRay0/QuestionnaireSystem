using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QuestionnaireSystem.UserControls
{
    public partial class UcAddControl : System.Web.UI.UserControl
    {
        // 測試用

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Answer"] != null)
            {
                this.AddControls();
            }

        }
        private void AddControls()
        {
            Label lbl = new Label();
            lbl.ID = "lbl1";
            lbl.Text = "lbl~";

            TextBox txt = new TextBox();
            txt.ID = "txt1";
            txt.Text = "txt~";

            Button btn = new Button();
            btn.ID = "btn1";
            btn.Text = "btn~";
            btn.Click += Btn_Click;

            this.Controls.Add(lbl);
            this.Controls.Add(txt);
            this.Controls.Add(btn);

        }

        private void Btn_Click(object sender, EventArgs e)
        {
            var txt = this.FindControl("txt1") as TextBox;
            Response.Write(txt.Text);
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            this.AddControls();
            Session["Answer"] = new string[] { "lbl1", "txt1", "btn1" };
        }
    }
}