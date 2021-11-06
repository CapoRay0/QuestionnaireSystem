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

                    #region 問卷部分
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

                    #region 問題部分
                    DataTable ProblemDT = QuestionnaireData.GetProblem(idToGuid); // 從DB抓

                    if (Session["ProblemDT"] == null)
                    {
                        if (ProblemDT.Rows.Count == 0)
                            this.ltlMsg.Text = "<br /><br /><br />尚無問題";

                        for (int i = 0; i < ProblemDT.Rows.Count; i++)
                            ProblemDT.Rows[i]["Count"] = i + 1;

                        this.gvProb.DataSource = ProblemDT;
                        this.gvProb.DataBind();
                    }
                    else
                    {
                        DataTable NewProblemDT = (DataTable)Session["ProblemDT"];

                        for (int i = 0; i < NewProblemDT.Rows.Count; i++)
                            NewProblemDT.Rows[i]["Count"] = i + 1;

                        this.gvProb.DataSource = NewProblemDT;
                        this.gvProb.DataBind();
                    }

                    this.btnDelete.Enabled = true;
                    this.ltlMsg.Text = "";

                    // 進入編輯模式：判斷是否為"新加入的問題"，或是原本 DB 就有
                    if (Session["PbGuid"] != null)
                    {
                        this.btnDelete.Enabled = false;
                        this.ltlMsg.Text = "修改模式下無法刪除問題";

                        //判斷 Session 是否有東西，有的話用 Session 回填，沒有則從 DB 抓
                        if (Session["ProblemDT"] != null) // Session
                        {
                            Guid PbGuid = Guid.Parse(Session["PbGuid"].ToString());
                            DataTable PbDT = (DataTable)Session["ProblemDT"];

                            for (int i = 0; i < PbDT.Rows.Count; i++) // 找出要編輯的問題在第幾列
                            {
                                if (Guid.Equals(PbGuid, PbDT.Rows[i]["ProbGuid"]))
                                {
                                    this.txtQuestion.Text = PbDT.Rows[i]["Text"].ToString();
                                    this.ddlSelectionType.SelectedValue = PbDT.Rows[i]["SelectionType"].ToString();
                                    this.ckbIsMust.Checked = (bool)PbDT.Rows[i]["IsMust"];
                                    this.txtSelection.Text = PbDT.Rows[i]["Selection"].ToString();
                                    break;
                                }
                            }
                        }
                        else // 進頁面後直接修改 DB (只會跑第一次)
                        {
                            Guid PbGuid = Guid.Parse(Session["PbGuid"].ToString());
                            DataRow OneProblem = QuestionnaireData.GetProblemDataRow(PbGuid);
                            if (OneProblem != null)
                            {
                                this.txtQuestion.Text = OneProblem["Text"].ToString();
                                this.txtSelection.Text = OneProblem["Selection"].ToString();
                                this.ddlSelectionType.SelectedValue = OneProblem["SelectionType"].ToString();
                                this.ckbIsMust.Checked = (bool)OneProblem["IsMust"];
                            }
                        }
                    }
                    #endregion
                }
            }
        }

        /// <summary>
        /// 清空問卷資料
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            this.txtCaption.Text = "";
            this.txtDescription.Text = "";
            this.txtStartDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            this.txtEndDate.Text = "";
            this.chkStatic.Checked = true;
        }

        /// <summary>
        /// 新增 / 修改問卷
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSend_Click(object sender, EventArgs e)
        {
            // 開始或結束時間若沒有填則預設為 1800/01/01 及 3000/12/31

            string id = this.Request.QueryString["ID"];

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

        }

        /// <summary>
        /// 新增 / 修改問題
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnAdd_Click(object sender, EventArgs e)
        {
            string id = this.Request.QueryString["ID"];
            if (string.IsNullOrWhiteSpace(id))
            {
                Response.Write($"<Script language='JavaScript'>alert('請先新增問卷'); location.href='SList.aspx'; </Script>");
                return;
            }
            Guid idToGuid = Guid.Parse(id);

            if (this.txtQuestion.Text == "")
            {
                Response.Write($"<Script language='JavaScript'>alert('請輸入問題名稱'); location.href='Detail.aspx?ID={id}#tabs2'; </Script>");
                return;
            }

            // 將資料庫的舊問題及頁面上加入的新問題一起放至新增的 DataTable 並存入 Session 中
            DataTable ProblemDT = new DataTable();

            if (Session["ProblemDT"] == null)
                ProblemDT = QuestionnaireData.GetProblem(idToGuid); // 最一開始從 DB 抓
            else
                ProblemDT = (DataTable)Session["ProblemDT"]; // Session 若有資料就直接用

            // 創建一個新的 DataTable
            DataTable dtProb = new DataTable();
            dtProb.Columns.Add(new DataColumn("ProbGuid", typeof(Guid)));
            dtProb.Columns.Add(new DataColumn("QuesGuid", typeof(Guid)));
            dtProb.Columns.Add(new DataColumn("Count", typeof(int)));
            dtProb.Columns.Add(new DataColumn("Text", typeof(string)));
            dtProb.Columns.Add(new DataColumn("SelectionType", typeof(int)));
            dtProb.Columns.Add(new DataColumn("IsMust", typeof(bool)));
            dtProb.Columns.Add(new DataColumn("Selection", typeof(string)));

            int selectedType = Convert.ToInt32(this.ddlSelectionType.SelectedValue);

            if (Session["PbGuid"] == null) // 新增問題
            {
                int count = 1;

                if (ProblemDT.Rows.Count != 0)
                    count = ProblemDT.Rows.Count + 1;

                DataRow drProb = dtProb.NewRow();
                drProb["ProbGuid"] = Guid.NewGuid();
                drProb["QuesGuid"] = idToGuid;
                drProb["Count"] = count;
                drProb["Text"] = this.txtQuestion.Text;
                drProb["SelectionType"] = selectedType;
                drProb["IsMust"] = this.ckbIsMust.Checked;
                drProb["Selection"] = this.txtSelection.Text;

                dtProb.Rows.Add(drProb);
                ProblemDT.Merge(dtProb); // 將原本資料庫的問題加上新的問題 (DataTable合併)
            }
            else // 編輯問題
            {
                Guid ProblemGuid = Guid.Parse(Session["PbGuid"].ToString());

                for (int i = 0; i < ProblemDT.Rows.Count; i++)
                {
                    if (Guid.Equals(ProblemGuid, ProblemDT.Rows[i]["ProbGuid"])) //找到符合的那一筆做更新
                    {
                        ProblemDT.Rows[i]["Text"] = this.txtQuestion.Text;
                        ProblemDT.Rows[i]["SelectionType"] = selectedType;
                        ProblemDT.Rows[i]["IsMust"] = this.ckbIsMust.Checked;
                        ProblemDT.Rows[i]["Selection"] = this.txtSelection.Text;
                        break;
                    }
                }
                Session["PbGuid"] = null;
            }
            Session["ProblemDT"] = ProblemDT;
            Response.Redirect($"/SystemAdminPages/Detail.aspx?ID={id}#tabs2");
        }

        /// <summary>
        /// GridView 呈現方式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// 刪除問題
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDelete_Click(object sender, ImageClickEventArgs e)
        {
            string id = this.Request.QueryString["ID"];
            if (string.IsNullOrWhiteSpace(id))
            {
                Response.Write($"<Script language='JavaScript'>alert('請先新增問卷'); location.href='SList.aspx'; </Script>");
                return;
            }
            Guid idToGuid = Guid.Parse(id);

            DataTable DTforDelete = new DataTable();

            if (Session["ProblemDT"] == null)
                DTforDelete = QuestionnaireData.GetProblem(idToGuid); // 最一開始從 DB 抓
            else
                DTforDelete = (DataTable)Session["ProblemDT"]; // Session 若有資料就直接用

            List<int> deleteNum = new List<int>();

            for (int i = 0; i < DTforDelete.Rows.Count; i++)
            {
                CheckBox cb = (CheckBox)gvProb.Rows[i].FindControl("ckbDelete");

                if (cb.Checked == true)
                {
                    int probIDToDelete = Convert.ToInt32(gvProb.Rows[i].RowIndex);
                    deleteNum.Add(probIDToDelete);
                }
            }

            deleteNum.Reverse(); // 從最後面開始刪除 (溢位)

            foreach (int d in deleteNum)
                DTforDelete.Rows.Remove(DTforDelete.Rows[d]);

            Session["ProblemDT"] = DTforDelete;
            this.gvProb.DataBind();

            Response.Redirect($"/SystemAdminPages/Detail.aspx?ID={id}#tabs2");
        }

        /// <summary>
        /// 編輯問題
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvProb_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ProbEdit")
            {
                string id = this.Request.QueryString["ID"];
                var pbGuid = e.CommandArgument.ToString();
                HttpContext.Current.Session["PbGuid"] = pbGuid;
                Response.Redirect($"/SystemAdminPages/Detail.aspx?ID={id}#tabs2");
            }
        }

        /// <summary>
        /// 取消問題管理，清除 Session
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCancelP_Click(object sender, EventArgs e)
        {
            Session["PbGuid"] = null;
            Session["ProblemDT"] = null;
            Response.Redirect("/SystemAdminPages/SList.aspx");
        }

        /// <summary>
        /// 將 Session 中的問題送進資料庫做真正的新增與更改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSendP_Click(object sender, EventArgs e)
        {
            string id = this.Request.QueryString["ID"];
            if (string.IsNullOrWhiteSpace(id))
            {
                Response.Write($"<Script language='JavaScript'>alert('請先新增問卷'); location.href='SList.aspx'; </Script>");
                return;
            }
            Guid idToGuid = Guid.Parse(id);

            DataTable SessionToDB;
            if (Session["ProblemDT"] == null)
            {
                SessionToDB = QuestionnaireData.GetProblem(idToGuid); // 最一開始從 DB 抓
                Response.Write($"<Script language='JavaScript'>alert('好像什麼都沒變哦~'); location.href='Detail.aspx?ID={id}#tabs2'; </Script>");
                return;
            }
            else
                SessionToDB = (DataTable)Session["ProblemDT"]; // Session 若有資料就直接用

            // 先刪除後加入
            QuestionnaireData.DeleteProblemData(idToGuid);

            for (int i = 0; i < SessionToDB.Rows.Count; i++)
            {
                SessionToDB.Rows[i]["Count"] = i + 1;
                Guid ProbGuid = (Guid)SessionToDB.Rows[i]["ProbGuid"];
                Guid QuesGuid = (Guid)SessionToDB.Rows[i]["QuesGuid"];
                int Count = (int)SessionToDB.Rows[i]["Count"];
                string Text = (string)SessionToDB.Rows[i]["Text"];
                int SelectionType = (int)SessionToDB.Rows[i]["SelectionType"];
                bool IsMust = (bool)SessionToDB.Rows[i]["IsMust"];

                string Selection = "";
                if (SelectionType == 0 || SelectionType == 1) // 只有單選和複選需要
                    Selection = (string)SessionToDB.Rows[i]["Selection"];

                QuestionnaireData.UpdateProblem(ProbGuid, QuesGuid, Count, Text, SelectionType, IsMust, Selection);
            }
            Response.Write("<Script language='JavaScript'>alert('問題編輯成功!!'); location.href='SList.aspx'; </Script>");
        }



        protected void gvProb_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {

        }
        protected void gvProb_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

        }


    }
}