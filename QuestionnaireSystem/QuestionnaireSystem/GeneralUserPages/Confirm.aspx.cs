using DBSource;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QuestionnaireSystem.GeneralUserPages
{
    public partial class Confirm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string id = this.Request.QueryString["ID"];

            if (!string.IsNullOrWhiteSpace(id) && id.Length == 36)
            {
                Guid idToGuid = Guid.Parse(id);

                DataRow QuesRow = QuestionnaireData.GetQuestionnaireDataRow(idToGuid);  // 問卷
                DataTable ProblemDT = QuestionnaireData.GetProblem(idToGuid); // 問題

                if (QuesRow != null)
                {
                    DateTime startDate = DateTime.Parse(QuesRow["StartDate"].ToString()); // 帶入開始日期
                    string StartString = startDate.ToString("yyyy/MM/dd");
                    if (StartString != "1800/01/01")
                        this.lblDuring.Text += StartString;
                    else
                        this.lblDuring.Text += "";

                    this.lblDuring.Text += " ~ "; // 開始日期與結束日期間的分隔

                    DateTime endDate = DateTime.Parse(QuesRow["EndDate"].ToString()); // 帶入結束日期
                    string EndString = endDate.ToString("yyyy/MM/dd");
                    if (EndString != "3000/12/31")
                        this.lblDuring.Text += EndString;
                    else
                        this.lblDuring.Text += "";

                    this.lblCaption.Text = QuesRow["Caption"].ToString(); // 帶入標題
                    this.lblDescription.Text = QuesRow["Description"].ToString(); // 帶入描述
                }

                // 從確認頁返回問卷填寫頁修改資料時回填
                if (Session["Name"] != null)
                    this.lblNameValue.Text = Session["Name"].ToString();
                if (Session["Phone"] != null)
                    this.lblPhoneValue.Text = Session["Phone"].ToString();
                if (Session["Email"] != null)
                    this.lblEmailValue.Text = Session["Email"].ToString();
                if (Session["Age"] != null)
                    this.lblAgeValue.Text = Session["Age"].ToString();

                if (ProblemDT.Rows.Count > 0)
                {
                    if (Session["Reply"] != null)
                    {
                        string[] reply = Session["Reply"].ToString().Split(';');
                        for (int i = 0; i < ProblemDT.Rows.Count; i++)
                            this.ltlReply.Text += $"<p>{i + 1}. {ProblemDT.Rows[i]["Text"]} <br /> &nbsp &nbsp {reply[i]}</p><br />";
                    }
                }

            }
        }

        protected void btnCancelC_Click(object sender, EventArgs e)
        {
            string id = this.Request.QueryString["ID"];
            Response.Redirect("Form.aspx?ID=" + id);
        }

        protected void btnSendC_Click(object sender, EventArgs e)
        {

        }
    }
}