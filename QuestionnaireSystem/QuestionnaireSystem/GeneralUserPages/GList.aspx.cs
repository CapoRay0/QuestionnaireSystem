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
    public partial class GList : System.Web.UI.Page
    {
        private DataTable dt;

        protected void Page_Load(object sender, EventArgs e)
        {
            Session.Abandon(); // 清空所有 Session

            dt = QuestionnaireData.GetQuestionnaire();

            // 取得現在時間來判斷是否關閉問卷
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i]["EndDate"].ToString() != "")
                {
                    DateTime dbStart = Convert.ToDateTime(dt.Rows[i]["StartDate"]);
                    DateTime dbEnd = Convert.ToDateTime(dt.Rows[i]["EndDate"]);
                    DateTime timeNow = DateTime.Now;
                    int quesIDToClose = Convert.ToInt32(dt.Rows[i][0].ToString()); //找到對應的流水號
                    if ((timeNow - dbEnd).Days > 0)
                        QuestionnaireData.CloseQuesStateByTime(quesIDToClose);
                    if ((timeNow - dbStart).Days < 0)
                        QuestionnaireData.CloseQuesStateByTime(quesIDToClose);
                }
            }

            var dtPaged = this.GetPagedDataTable(dt);

            if (dt.Rows.Count == 0)
                this.ltlMsg.Text = "<br /><br /><br />查無資料";

            this.gvGList.DataSource = dtPaged;

            if (!IsPostBack)
            {
                this.gvGList.DataBind();
                this.UcPager.TotalSize = dt.Rows.Count;
                this.UcPager.Bind();
            }
        }

        /// <summary>
        /// 搜尋按鈕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            var search = this.txtTitle.Text.Trim();
            dt = QuestionnaireData.SearchQuestionnaire(search);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DateTime dbStart = Convert.ToDateTime(dt.Rows[i]["StartDate"]);
                DateTime dbEnd = Convert.ToDateTime(dt.Rows[i]["EndDate"]);

                if (this.txtDateStart.Text != "" && this.txtDateEnd.Text != "") //開始結束都有填
                {
                    DateTime searchStart = Convert.ToDateTime(this.txtDateStart.Text);
                    DateTime searchEnd = Convert.ToDateTime(this.txtDateEnd.Text);
                    if ((searchEnd - searchStart).Days < 0)
                        Response.Write("<Script language='JavaScript'>alert('日期不可為負的'); location.href='GList.aspx'; </Script>");
                    if ((dbEnd - searchStart).Days < 0 || (dbStart - searchEnd).Days > 0)
                        dt.Rows[i].Delete();
                }
                else if (this.txtDateStart.Text != "" && this.txtDateEnd.Text == "") //只填開始時間
                {
                    DateTime searchStart = Convert.ToDateTime(this.txtDateStart.Text);
                    if ((dbEnd - searchStart).Days < 0)
                        dt.Rows[i].Delete();
                }
                else if (this.txtDateStart.Text == "" && this.txtDateEnd.Text != "") //只填結束時間
                {
                    DateTime searchEnd = Convert.ToDateTime(this.txtDateEnd.Text);
                    if ((dbStart - searchEnd).Days > 0)
                        dt.Rows[i].Delete();
                }
            }

            if (dt.Rows.Count == 0)
                this.ltlMsg.Text = "<br /><br /><br />查無資料";
            else
                this.ltlMsg.Text = "";

            this.gvGList.DataSource = dt;
            this.gvGList.DataBind();

            UcPager.Visible = false;
        }

        /// <summary>
        /// GridView 內容呈現
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvGList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            var row = e.Row;
            if (row.RowType == DataControlRowType.DataRow)
            {
                Label lblState = row.FindControl("lblState") as Label;
                Label lblCaption = row.FindControl("lblCaption") as Label;
                Label lblStartDate = row.FindControl("lblStartDate") as Label;
                Label lblEndDate = row.FindControl("lblEndDate") as Label;

                var dr = row.DataItem as DataRowView;
                int QuesState = dr.Row.Field<int>("State");
                string QuesCaption = dr.Row.Field<string>("Caption");
                Guid QuesGuid = dr.Row.Field<Guid>("QuesGuid");

                switch (QuesState)
                {
                    case 0:
                        lblState.Text = "已完結";
                        lblCaption.Text = QuesCaption;
                        break;
                    case 1:
                        lblState.Text = "投票中";
                        lblCaption.Text = $"<a href='Form.aspx?ID={QuesGuid}'>{QuesCaption}</a>";
                        break;
                }

                DateTime QuesStartDate = dr.Row.Field<DateTime>("StartDate");
                if (QuesStartDate.ToString("yyyy-MM-dd") == "1800-01-01")
                    lblStartDate.Text = "-";
                else
                    lblStartDate.Text = QuesStartDate.ToString("yyyy-MM-dd");

                DateTime QuesEndDate = dr.Row.Field<DateTime>("EndDate");
                if (QuesEndDate.ToString("yyyy-MM-dd") == "3000-12-31")
                    lblEndDate.Text = "-";
                else
                    lblEndDate.Text = QuesEndDate.ToString("yyyy-MM-dd");
            }
        }

        #region UcPager 換頁
        private int GetCurrentPage()
        {
            string pageText = Request.QueryString["Page"];

            if (string.IsNullOrWhiteSpace(pageText)) // 空的時候，給第一頁
                return 1;

            int intPage;
            if (!int.TryParse(pageText, out intPage)) // (錯誤) 數字轉換失敗，給第一頁
                return 1;

            if (intPage <= 0) // (錯誤) 0或以下，也給第一頁
                return 1;

            return intPage;
        }
        private DataTable GetPagedDataTable(DataTable dt)
        {
            DataTable dtPaged = dt.Clone();

            int startIndex = (this.GetCurrentPage() - 1) * 10;
            int endIndex = (this.GetCurrentPage()) * 10;

            if (endIndex > dt.Rows.Count)
                endIndex = dt.Rows.Count;

            for (var i = startIndex; i < endIndex; i++)
            {
                DataRow dr = dt.Rows[i];
                var drNew = dtPaged.NewRow();

                foreach (DataColumn dc in dt.Columns)
                {
                    drNew[dc.ColumnName] = dr[dc];
                }
                dtPaged.Rows.Add(drNew);
            }
            return dtPaged;
        }
        #endregion

    }
}