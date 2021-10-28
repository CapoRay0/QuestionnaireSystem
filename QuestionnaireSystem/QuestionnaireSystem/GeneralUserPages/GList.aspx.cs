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
            dt = QuestionnaireData.GetQuestionnaire();
            var dtPaged = this.GetPagedDataTable(dt);

            if (dt.Rows.Count == 0)
                this.ltlMsg.Text = "查無資料";

            this.gvGList.DataSource = dtPaged;

            if (!IsPostBack)
            {
                this.gvGList.DataBind();
                this.UcPager.TotalSize = dt.Rows.Count;
                this.UcPager.Bind();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e) //待優化
        {
            var search = this.txtTitle.Text.Trim();
            dt = QuestionnaireData.SearchQuestionnaire(search);

            if (this.txtDateStart.Text != "" && this.txtDateEnd.Text != "") //開始結束都有填
            {
                DateTime searchStart = Convert.ToDateTime(this.txtDateStart.Text);
                DateTime searchEnd = Convert.ToDateTime(this.txtDateEnd.Text);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i][5].ToString() != "")
                    {
                        DateTime dbStart = Convert.ToDateTime(dt.Rows[i][4]);
                        DateTime dbEnd = Convert.ToDateTime(dt.Rows[i][5]);
                        if ((searchEnd - dbEnd).Days > 0 || (searchStart - dbStart).Days < 0)
                            dt.Rows[i].Delete();
                    }
                    else
                    {
                        DateTime dbStart = Convert.ToDateTime(dt.Rows[i][4]);
                        if ((searchStart - dbStart).Days < 0)
                            dt.Rows[i].Delete();
                    }
                }
            }
            else if (this.txtDateStart.Text != "" && this.txtDateEnd.Text == "") //只填開始時間
            {
                DateTime searchStart = Convert.ToDateTime(this.txtDateStart.Text);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DateTime dbStart = Convert.ToDateTime(dt.Rows[i][4]);
                    if (dt.Rows[i][5].ToString() != "") //若DB有結束時間
                    {
                        DateTime dbEnd = Convert.ToDateTime(dt.Rows[i][5]);
                        if ((dbEnd - searchStart).Days < 0)
                            dt.Rows[i].Delete();
                    }
                    if ((dbStart - searchStart).Days < 0)
                        dt.Rows[i].Delete();
                }
            }
            else if (this.txtDateStart.Text == "" && this.txtDateEnd.Text != "") //只填結束時間
            {
                DateTime searchEnd = Convert.ToDateTime(this.txtDateEnd.Text);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i][5].ToString() != "") //若DB有結束時間
                    {
                        DateTime dbStart = Convert.ToDateTime(dt.Rows[i][4]);
                        DateTime dbEnd = Convert.ToDateTime(dt.Rows[i][5]);
                        if ((searchEnd - dbEnd).Days > 0 || (searchEnd - dbStart).Days <= 0)
                            dt.Rows[i].Delete();
                    }
                    else //若DB無結束時間
                    {
                        DateTime dbStart = Convert.ToDateTime(dt.Rows[i][4]);
                        if ((searchEnd - dbStart).Days < 0)
                            dt.Rows[i].Delete();
                    }

                }
            }

            if (dt.Rows.Count == 0)
                this.ltlMsg.Text = "<br /><br /><br />查無資料";

            this.gvGList.DataSource = dt;
            this.gvGList.DataBind();
        }

        protected void gvGList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            var row = e.Row;
            if (row.RowType == DataControlRowType.DataRow)
            {
                Label lblState = row.FindControl("lblState") as Label;
                Label lblCaption = row.FindControl("lblCaption") as Label;
                
                var dr = row.DataItem as DataRowView;

                int quesState = dr.Row.Field<int>("State");
                string quesCaption = dr.Row.Field<string>("Caption");
                Guid quesGuid = dr.Row.Field<Guid>("QuesGuid");

                switch (quesState)
                {
                    case 0:
                        lblState.Text = "已關閉";
                        lblCaption.Text = quesCaption;
                        break;
                    case 1:
                        lblState.Text = "開放中";
                        lblCaption.Text = $"<a href='Form.aspx?ID={quesGuid}' target='_blank'>{quesCaption}</a>";
                        break;
                }
            }
        }

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

            int pageSize = this.UcPager.PageSize;
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


        ///// <summary>
        ///// GridView 內建換頁
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //protected void gvGList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        //{
        //    gvGList.PageIndex = e.NewPageIndex;
        //    this.gvGList.DataBind();
        //}
    }
}