using DBSource;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QuestionnaireSystem.SystemAdminPages
{
    public partial class SList : System.Web.UI.Page
    {
        private DataTable dt;

        protected void Page_Load(object sender, EventArgs e)
        {

            #region 開發時隱藏
            if (!AuthManager.IsLogined())
            {
                Response.Redirect("../Login.aspx");
                return;
            }
            #endregion

            dt = QuestionnaireData.GetQuestionnaire();
            var dtPaged = this.GetPagedDataTable(dt);

            if (dt.Rows.Count == 0)
                this.ltlMsg.Text = "<br /><br /><br />查無資料";

            this.gvSList.DataSource = dtPaged;

            if (!IsPostBack)
            {
                this.gvSList.DataBind();
                this.UcPager.TotalSize = dt.Rows.Count;
                this.UcPager.Bind();
            }

            btnDelete.Attributes.Add("onclick ", "return confirm( '確定要將選取問卷刪除嗎?');");

        }

        #region search
        protected void btnSearch_Click(object sender, EventArgs e) // 待優化 >> 資料庫的 NULL 問題
        {
            var search = this.txtTitle.Text.Trim();
            dt = QuestionnaireData.SearchQuestionnaire(search);

            if (this.txtDateStart.Text != "" && this.txtDateEnd.Text != "") //開始結束都有填
            {
                DateTime searchStart = Convert.ToDateTime(this.txtDateStart.Text);
                DateTime searchEnd = Convert.ToDateTime(this.txtDateEnd.Text);

                if ((searchEnd - searchStart).Days < 0)
                    Response.Write("<Script language='JavaScript'>alert('日期不可為負的'); location.href='SList.aspx'; </Script>");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DateTime dbStart = Convert.ToDateTime(dt.Rows[i]["StartDate"]);
                    if (dt.Rows[i]["EndDate"].ToString() != "")
                    {
                        DateTime dbEnd = Convert.ToDateTime(dt.Rows[i]["EndDate"]);
                        if ((searchEnd - dbEnd).Days < 0 || (searchStart - dbStart).Days > 0)
                            dt.Rows[i].Delete();
                    }
                    if ((searchStart - dbStart).Days > 0)
                        dt.Rows[i].Delete();

                }
            }
            else if (this.txtDateStart.Text != "" && this.txtDateEnd.Text == "") //只填開始時間
            {
                DateTime searchStart = Convert.ToDateTime(this.txtDateStart.Text);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DateTime dbStart = Convert.ToDateTime(dt.Rows[i]["StartDate"]);
                    if (dt.Rows[i][5].ToString() != "") //若DB有結束時間
                    {
                        DateTime dbEnd = Convert.ToDateTime(dt.Rows[i]["EndDate"]);
                        if ((dbEnd - searchStart).Days < 0 && (dbEnd - searchStart).Days < 0)
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
                    if (dt.Rows[i]["EndDate"].ToString() != "") //若DB有結束時間
                    {
                        DateTime dbStart = Convert.ToDateTime(dt.Rows[i]["StartDate"]);
                        DateTime dbEnd = Convert.ToDateTime(dt.Rows[i]["EndDate"]);
                        if ((searchEnd - dbEnd).Days > 0 || (searchEnd - dbStart).Days <= 0)
                            dt.Rows[i].Delete();
                    }
                    else //若DB無結束時間
                    {
                        DateTime dbStart = Convert.ToDateTime(dt.Rows[i]["StartDate"]);
                        if ((searchEnd - dbStart).Days < 0)
                            dt.Rows[i].Delete();
                    }

                }
            }

            if (dt.Rows.Count == 0)
                this.ltlMsg.Text = "<br /><br /><br />查無資料";

            this.gvSList.DataSource = dt;
            this.gvSList.DataBind();

            UcPager.Visible = false;
        }
        #endregion

        protected void btnDelete_Click(object sender, ImageClickEventArgs e)
        {

            for (int i = 0; i < gvSList.Rows.Count; i++)
            {
                CheckBox cb = (CheckBox)gvSList.Rows[i].FindControl("ckbDelete");
                if (cb.Checked == true)
                {
                    int quesIDToDelete = Convert.ToInt32(gvSList.Rows[i].Cells[1].Text);
                    QuestionnaireData.DeleteQuestionnaireData(quesIDToDelete);
                    Response.Write($"<Script language='JavaScript'>alert('問卷刪除成功!!'); location.href='{this.Request.RawUrl}'; </Script>");
                }
            }
            this.gvSList.DataBind();

        }

        protected void btnNewForm_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("Detail.aspx");
        }

        protected void gvSlist_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            var row = e.Row;
            if (row.RowType == DataControlRowType.DataRow)
            {
                Label lbl = row.FindControl("lblState") as Label;
                var dr = row.DataItem as DataRowView;
                int QuesState = dr.Row.Field<int>("State");
                switch (QuesState)
                {
                    case 0:
                        lbl.Text = "已關閉";
                        break;
                    case 1:
                        lbl.Text = "開放中";
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
        //protected void gvSList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        //{
        //    gvSList.PageIndex = e.NewPageIndex;
        //    this.gvSList.DataBind();
        //}
    }
}