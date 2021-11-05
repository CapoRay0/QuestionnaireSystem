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
    public partial class Detail : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string id = this.Request.QueryString["ID"];


            if (!IsPostBack)
            {
                if (string.IsNullOrWhiteSpace(id)) //新增問卷
                {
                    this.txtStartDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                    this.chkStatic.Checked = true;
                }
                else //編輯問卷
                {
                    Guid idToGuid = Guid.Parse(id);

                    #region 問卷
                    DataRow dr = QuestionnaireData.GetQuestionnaireDataRow(idToGuid);

                    this.txtCaption.Text = dr["Caption"].ToString(); //帶入標題
                    this.txtDescription.Text = dr["Description"].ToString(); //帶入描述

                    if (dr["StartDate"].ToString() != "") //帶入開始日期
                    {
                        DateTime startDate = DateTime.Parse(dr["StartDate"].ToString());
                        string StartString = startDate.ToString("yyyy-MM-dd");
                        if (StartString != "1800-01-01")
                            this.txtStartDate.Text = StartString;
                    }
                    else
                        this.txtStartDate.Text = "";

                    if (dr["EndDate"].ToString() != "") //帶入結束日期
                    {
                        DateTime endDate = DateTime.Parse(dr["EndDate"].ToString());
                        string EndString = endDate.ToString("yyyy-MM-dd");
                        if (EndString != "3000-12-31")
                            this.txtEndDate.Text = EndString;
                    }
                    else
                        this.txtEndDate.Text = "";

                    if (dr["State"].ToString() == "1")  //帶入開放與否
                        this.chkStatic.Checked = true;
                    else
                        this.chkStatic.Checked = false;
                    #endregion

                    DataTable ProblemDT = QuestionnaireData.GetProblemForBind(idToGuid); // 從DB抓

                    if (Session["ProblemList"] == null)
                    {
                        this.gvProb.DataSource = ProblemDT;
                        this.gvProb.DataBind();
                    }
                    else
                    {
                        var NewProblemList = (DataTable)Session["ProblemList"];
                        this.gvProb.DataSource = NewProblemList;
                        this.gvProb.DataBind();
                    }
                }
            }





        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            this.txtCaption.Text = "";
            this.txtDescription.Text = "";
            this.txtStartDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            this.txtEndDate.Text = "";
            this.chkStatic.Checked = true;
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            // 開始或結束時間若沒有填則預設為 1800/01/01 及 3000/12/31

            string id = this.Request.QueryString["ID"];

            #region 新增及修改問卷
            if (string.IsNullOrWhiteSpace(id)) //新增問卷
            {
                Guid inpQuesGuid = Guid.NewGuid();
                string inpCaption = this.txtCaption.Text;
                string inpDescription = this.txtDescription.Text;

                DateTime inpStartDate;
                if (this.txtStartDate.Text != "")
                    inpStartDate = Convert.ToDateTime(this.txtStartDate.Text);
                else
                    inpStartDate = new DateTime(1800, 1, 1);

                DateTime inpEndDate;
                if (this.txtEndDate.Text != "")
                    inpEndDate = Convert.ToDateTime(this.txtEndDate.Text);
                else
                    inpEndDate = new DateTime(3000, 12, 31);

                int inpState;
                if (chkStatic.Checked == true)
                    inpState = 1;
                else
                    inpState = 0;

                int inpCount = 0;
                if (this.txtCaption.Text != "")
                {
                    if ((inpEndDate - inpStartDate).Days > 0)
                        QuestionnaireData.CreateQuestionnaire(inpQuesGuid, inpCaption, inpDescription, inpStartDate, inpEndDate, inpState, inpCount);
                    else
                    {
                        this.ClientScript.RegisterStartupScript(this.GetType(), "", "<script>alert('日期不可為負的')</script>");
                        return;
                    }
                    Response.Write("<Script language='JavaScript'>alert('問卷新增成功!!'); location.href='SList.aspx'; </Script>");
                }
                else
                    this.ClientScript.RegisterStartupScript(this.GetType(), "", "<script>alert('請輸入問卷名稱')</script>");
            }
            else //編輯問卷
            {
                Guid idToGuid = Guid.Parse(id);
                string editCaption = this.txtCaption.Text;
                string editDescription = this.txtDescription.Text;

                DateTime editStartDate;
                if (this.txtStartDate.Text != "")
                    editStartDate = Convert.ToDateTime(this.txtStartDate.Text);
                else
                    editStartDate = new DateTime(1800, 1, 1);

                DateTime editEndDate;
                if (this.txtEndDate.Text != "")
                    editEndDate = Convert.ToDateTime(this.txtEndDate.Text);
                else
                    editEndDate = new DateTime(3000, 12, 31);

                int editState;
                if (chkStatic.Checked == true)
                    editState = 1;
                else
                    editState = 0;

                if (this.txtCaption.Text != "")
                {
                    if ((editEndDate - editStartDate).Days > 0)
                        QuestionnaireData.EditQuestionnaire(idToGuid, editCaption, editDescription, editStartDate, editEndDate, editState);
                    else
                    {
                        this.ClientScript.RegisterStartupScript(this.GetType(), "", "<script>alert('日期不可為負的')</script>");
                        return;
                    }
                    Response.Write("<Script language='JavaScript'>alert('問卷修改成功!!'); location.href='SList.aspx'; </Script>");
                }
                else
                    this.ClientScript.RegisterStartupScript(this.GetType(), "", "<script>alert('請輸入問卷名稱')</script>");

            }
            #endregion
        }

        protected void gvProb_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            var row = e.Row;
            if (row.RowType == DataControlRowType.DataRow)
            {
                Label lbl = row.FindControl("lblSelectionType") as Label;
                var dr = row.DataItem as DataRowView;
                int ProbType = dr.Row.Field<int>("SelectionType");
                switch (ProbType)
                {
                    case 0:
                        lbl.Text = "單選方塊";
                        break;
                    case 1:
                        lbl.Text = "複選方塊";
                        break;
                    case 2:
                        lbl.Text = "文字";
                        break;
                    case 3:
                        lbl.Text = "文字(數字)";
                        break;
                    case 4:
                        lbl.Text = "文字(Email)";
                        break;
                    case 5:
                        lbl.Text = "文字(日期)";
                        break;
                }
            }
        }

        protected void btnAddSelection_Click(object sender, EventArgs e)
        {
            string id = this.Request.QueryString["ID"];
            Guid idToGuid = Guid.Parse(id);
            //Session 有沒有DT 有S 沒有才DB
            if (this.txtText.Text == "")
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "", "<script>alert('請將資料填妥')</script>");
            }
            else
            {
                // 新增 DataTable 將問題加入原本 DataTable 並存入 Session 之中

                DataTable ProblemDT = new DataTable();

                if (Session["ProblemList"] == null)
                    ProblemDT = QuestionnaireData.GetProblemForBind(idToGuid);
                else
                    ProblemDT = (DataTable)Session["ProblemList"];


                DataTable dtProb = new DataTable();
                dtProb.Columns.Add(new DataColumn("ProbGuid", typeof(Guid)));
                dtProb.Columns.Add(new DataColumn("QuesGuid", typeof(Guid)));
                dtProb.Columns.Add(new DataColumn("Text", typeof(string)));
                dtProb.Columns.Add(new DataColumn("SelectionType", typeof(int)));
                dtProb.Columns.Add(new DataColumn("IsMust", typeof(bool)));
                dtProb.Columns.Add(new DataColumn("Selection", typeof(string)));

                DataRow drProb = dtProb.NewRow();
                drProb["ProbGuid"] = Guid.NewGuid();
                drProb["QuesGuid"] = idToGuid;
                drProb["Text"] = this.txtText.Text;
                drProb["SelectionType"] = this.ddlSelectionType.SelectedValue;
                drProb["IsMust"] = this.ckbIsMust.Checked;
                drProb["Selection"] = this.txtSelection.Text;
                dtProb.Rows.Add(drProb);


                ProblemDT.Merge(dtProb); // 原本加上新的

                Session["ProblemList"] = ProblemDT;


                Response.Redirect($"/SystemAdminPages/Detail.aspx?ID={id}#tabs2");
            }



        }

        protected void gvProb_RowCommand(object sender, GridViewCommandEventArgs e)
        {

        }


        protected void gvProb_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {

        }
        protected void gvProb_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

        }
    }
}