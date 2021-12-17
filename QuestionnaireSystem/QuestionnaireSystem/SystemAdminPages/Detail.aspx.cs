using DBSource;
using DataFormatTransfer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using QuestionnaireSystem.Extensions;
using QuestionnaireSystem.ORM.DBModels;

namespace QuestionnaireSystem.SystemAdminPages
{
    public partial class Detail : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            #region 開發時隱藏
            if (!AuthManager.IsLogined())
            {
                Response.Redirect("../Login.aspx");
                return;
            }
            #endregion

            string id = this.Request.QueryString["ID"];

            if (!IsPostBack)
            {
                if (string.IsNullOrWhiteSpace(id)) // 新增問卷
                {
                    this.txtStartDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                    this.chkStatic.Checked = true;

                    // 套用常用問題 (此為顯示，在編輯問卷才能真正套用)
                    //var commonProb = CommonProblem.GetCommon();
                    //this.ddlCommon.DataSource = commonProb;
                    var commonProb = CommonProblem.GetCommonEF();
                    this.ddlCommon.DataSource = commonProb;
                    this.ddlCommon.DataTextField = "Name";
                    this.ddlCommon.DataValueField = "CommID";
                    this.ddlCommon.DataBind();
                    this.lkbCommon.PostBackUrl = $"/SystemAdminPages/Detail.aspx?ID={id}#tabs2";
                }
                else if (!string.IsNullOrWhiteSpace(id) && id.Length == 36) // 編輯問卷
                {
                    // 套用常用問題
                    //var commonProb = CommonProblem.GetCommon();
                    //this.ddlCommon.DataSource = commonProb;
                    var commonProb = CommonProblem.GetCommonEF();
                    this.ddlCommon.DataSource = commonProb;
                    this.ddlCommon.DataTextField = "Name";
                    this.ddlCommon.DataValueField = "CommID";
                    this.ddlCommon.DataBind();

                    // 用 LinkButton 才可處理 PostBack 問題
                    this.lkbCommon.PostBackUrl = $"/SystemAdminPages/Detail.aspx?ID={id}#tabs2";

                    #region 問卷
                    //DataRow QuesRow = QuestionnaireData.GetQuestionnaireDataRow(Guid.Parse(id)); // 從 DB 抓問卷
                    Questionnaire QuesRow = QuestionnaireData.GetQuestionnaireDataRowEF(id.ToGuid()); // 從 DB 抓問卷

                    //先判斷 QuesGuid 是否有誤
                    //if (QuesRow == null || QuesRow["QuesGuid"].ToString() != id)
                    if (QuesRow == null || QuesRow.QuesGuid.ToString() != id)
                    {
                        Response.Write("<Script language='JavaScript'>alert(' Guid 錯誤，將您導向回列表頁'); location.href='SList.aspx'; </Script>");
                        return;
                    }

                    //this.txtCaption.Text = QuesRow["Caption"].ToString(); // 帶入標題
                    //this.txtDescription.Text = QuesRow["Description"].ToString(); // 帶入描述
                    this.txtCaption.Text = QuesRow.Caption.ToString(); // 帶入標題
                    this.txtDescription.Text = QuesRow.Description.ToString(); // 帶入描述

                    //DateTime startDate = DateTime.Parse(QuesRow["StartDate"].ToString()); // 帶入開始日期
                    DateTime startDate = DateTime.Parse(QuesRow.StartDate.ToString()); // 帶入開始日期
                    string StartString = startDate.ToString("yyyy-MM-dd");
                    if (StartString != "1800-01-01")
                        this.txtStartDate.Text = StartString;

                    //DateTime endDate = DateTime.Parse(QuesRow["EndDate"].ToString()); // 帶入結束日期
                    DateTime endDate = DateTime.Parse(QuesRow.EndDate.ToString()); // 帶入結束日期
                    string EndString = endDate.ToString("yyyy-MM-dd");
                    if (EndString != "3000-12-31")
                        this.txtEndDate.Text = EndString;

                    //if (QuesRow["State"].ToString() == "1") // 帶入開放與否
                    if (QuesRow.State.ToString() == "1") // 帶入開放與否
                        this.chkStatic.Checked = true;
                    else
                        this.chkStatic.Checked = false;
                    #endregion

                    #region 問題
                    //DataTable ProblemDT = ProblemData.GetProblem(Guid.Parse(id)); // 從 DB 抓問題
                    List<Problem> ProblemList = ProblemData.GetProblemEF(id.ToGuid()); // 從 DB 抓問題

                    if (Session["ProblemDT"] == null)
                    {
                        //if (ProblemDT.Rows.Count == 0)
                        if (ProblemList.Count == 0)
                            this.ltlMsg.Text = "<br /><br /><br />尚無問題";

                        //for (int i = 0; i < ProblemDT.Rows.Count; i++)
                        //    ProblemDT.Rows[i]["Count"] = i + 1;
                        for (int i = 0; i < ProblemList.Count; i++)
                            ProblemList[i].Count = i + 1;

                        //this.gvProb.DataSource = ProblemDT;
                        this.gvProb.DataSource = ProblemList;
                        this.gvProb.DataBind();
                    }
                    else
                    {
                        //DataTable NewProblemDT = (DataTable)Session["ProblemDT"];
                        List<Problem> NewProblemDT = (List<Problem>)Session["ProblemDT"];

                        //for (int i = 0; i < NewProblemDT.Rows.Count; i++)
                        //    NewProblemDT.Rows[i]["Count"] = i + 1;
                        for (int i = 0; i < NewProblemDT.Count; i++)
                            NewProblemDT[i].Count = i + 1;

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

                        // 判斷 Session 是否有東西，有的話用 Session 回填，沒有則從 DB 抓
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
                        else // 從資料庫抓值
                        {
                            Guid PbGuid = Guid.Parse(Session["PbGuid"].ToString());
                            //DataRow OneProblem = ProblemData.GetProblemDataRow(PbGuid);
                            Problem OneProblem = ProblemData.GetProblemDataRowEF(PbGuid);
                            if (OneProblem != null)
                            {
                                //this.txtQuestion.Text = OneProblem["Text"].ToString();
                                //this.txtSelection.Text = OneProblem["Selection"].ToString();
                                //this.ddlSelectionType.SelectedValue = OneProblem["SelectionType"].ToString();
                                //this.ckbIsMust.Checked = (bool)OneProblem["IsMust"];
                                this.txtQuestion.Text = OneProblem.Text.ToString();
                                this.txtSelection.Text = OneProblem.Selection.ToString();
                                this.ddlSelectionType.SelectedValue = OneProblem.SelectionType.ToString();
                                this.ckbIsMust.Checked = (bool)OneProblem.IsMust;
                            }
                        }
                    }
                    #endregion

                    #region 填寫資料
                    DataTable ReplyDT = UserInfoManager.GetReplyInfo(Guid.Parse(id));
                    if (ReplyDT.Rows.Count > 0)
                    {
                        this.btnOutput.Enabled = true;
                        this.gvReply.DataSource = ReplyDT;
                        this.gvReply.DataBind();
                    }

                    if (this.Request.QueryString["UID"] != null)
                    {
                        this.gvReply.Visible = false;
                        string Uid = this.Request.QueryString["UID"];
                        DataRow ReplyInfoDataRow = ReplyData.GetReplyInfoDataRow(Guid.Parse(Uid));

                        if (ReplyInfoDataRow != null)
                        {
                            this.lblName.Visible = true;
                            this.txtName.Visible = true;
                            this.txtName.Text = ReplyInfoDataRow["Name"].ToString();

                            this.lblPhone.Visible = true;
                            this.txtPhone.Visible = true;
                            this.txtPhone.Text = ReplyInfoDataRow["Phone"].ToString();

                            this.lblEmail.Visible = true;
                            this.txtEmail.Visible = true;
                            this.txtEmail.Text = ReplyInfoDataRow["Email"].ToString();

                            this.lblAge.Visible = true;
                            this.txtAge.Visible = true;
                            this.txtAge.Text = ReplyInfoDataRow["Age"].ToString();

                            this.lblFillOut.Visible = true;
                            this.lblCreateDate.Visible = true;
                            this.lblCreateDate.Text = ReplyInfoDataRow["CreateDate"].ToString();

                            //使用 PlaceHolder 新增控制項：https://codertw.com/%E5%89%8D%E7%AB%AF%E9%96%8B%E7%99%BC/210860/

                            //for (int i = 0; i < ProblemDT.Rows.Count; i++) // 迴圈印出所有問題與回答
                            for (int i = 0; i < ProblemList.Count; i++) // 迴圈印出所有問題與回答
                            {
                                Literal ltlProbText = new Literal();
                                //ltlProbText.Text = (i + 1).ToString() + ". " + ProblemDT.Rows[i]["Text"].ToString() + "<br />";
                                ltlProbText.Text = (i + 1).ToString() + ". " + ProblemList[i].Text.ToString() + "<br />";
                                phReply.Controls.Add(ltlProbText); // 印出問題名

                                Literal ltlProbAnswer = new Literal();
                                //Guid pbGuid = Guid.Parse(ProblemDT.Rows[i]["ProbGuid"].ToString());
                                Guid pbGuid = Guid.Parse(ProblemList[i].ProbGuid.ToString());
                                DataRow AnsDR = ReplyData.GetReplyDataRow(Guid.Parse(Uid), pbGuid);

                                ltlProbAnswer.Text = "&nbsp &nbsp " + AnsDR["AnswerText"].ToString() + "<br /><br />";
                                phReply.Controls.Add(ltlProbAnswer); // 印出回答
                            }

                            this.btnBackToReplyInfo.Visible = true;
                        }
                    }
                    #endregion
                }
                else
                    Response.Write("<Script language='JavaScript'>alert(' QueryString 錯誤，將您導向回列表頁'); location.href='SList.aspx'; </Script>");
            }
            // if (!IsPostBack) 結束

            #region 統計
            if (!string.IsNullOrWhiteSpace(id) && id.Length == 36)
            {
                //DataTable ProblemDT = ProblemData.GetProblem(Guid.Parse(id)); // 從 DB 抓問題
                List<Problem> ProblemDT = ProblemData.GetProblemEF(id.ToGuid()); // 從 DB 抓問題
                //for (int i = 0; i < ProblemDT.Rows.Count; i++) // 每個問題
                for (int i = 0; i < ProblemDT.Count; i++) // 每個問題
                {
                    Literal ltlStaticText = new Literal();
                    //ltlStaticText.Text = (i + 1).ToString() + ". " + ProblemDT.Rows[i]["Text"].ToString();
                    ltlStaticText.Text = (i + 1).ToString() + ". " + ProblemDT[i].Text.ToString();
                    //if ((bool)ProblemDT.Rows[i]["IsMust"] == true)
                    if ((bool)ProblemDT[i].IsMust == true)
                        ltlStaticText.Text += " (必填)";
                    ltlStaticText.Text += "<br />";

                    phStatic.Controls.Add(ltlStaticText); // 印出欲統計問題名

                    //int type = Convert.ToInt32(ProblemDT.Rows[i]["SelectionType"]);
                    int type = Convert.ToInt32(ProblemDT[i].SelectionType);
                    if (type == 0 || type == 1) // 單選、複選
                    {
                        //Guid pbGuid = Guid.Parse(ProblemDT.Rows[i]["ProbGuid"].ToString());
                        Guid pbGuid = Guid.Parse(ProblemDT[i].ProbGuid.ToString());

                        DataTable option = StaticData.GetStatic(pbGuid); // 該問題單一選項 (分子)

                        DataRow sumDR = StaticData.GetStaticSum(pbGuid); // 該問題選擇數加總 (分母)
                        int sum = Convert.ToInt32(sumDR["Sum"]);

                        for (int j = 0; j < option.Rows.Count; j++)
                        {
                            int count = Convert.ToInt32(option.Rows[j]["Count"]);
                            string probtext = option.Rows[j]["OptionText"].ToString();

                            double percent = 0;
                            string percentStr = "0";
                            if (sum != 0)
                            {
                                percent = ((double)count / (double)sum) * 100; // 轉成 double 型別後再運算
                                percentStr = percent.ToString("0.00"); // 保留小數點後2位
                            }
                            ltlStaticText.Text += "&nbsp &nbsp " + $"{probtext} {percentStr}% ({count})<br />";
                        }
                    }
                    else
                        ltlStaticText.Text += "&nbsp &nbsp -<br />";

                    ltlStaticText.Text += "<br />";
                    phStatic.Controls.Add(ltlStaticText);
                }
            }
            #endregion
        }

        #region 問卷

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
                Guid newQuesGuid = Guid.NewGuid();
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
                    {
                        QuestionnaireData.CreateQuestionnaire(newQuesGuid, inpCaption, inpDescription, inpStartDate, inpEndDate, inpState, inpCount);
                    } 
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

        #endregion

        #region 問題

        /// <summary>
        /// 用 LinkButton 套用常用問題
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lkbCommon_Click(object sender, EventArgs e)
        {
            int commonID;
            if (int.TryParse(this.ddlCommon.SelectedValue, out commonID))
            {
                //DataRow commRow = CommonProblem.GetCommonByCommID(commonID);

                //this.txtQuestion.Text = commRow["Text"].ToString();

                //int type = Convert.ToInt32(commRow["SelectionType"]);
                //this.ddlSelectionType.SelectedValue = type.ToString();

                //this.ckbIsMust.Checked = (bool)commRow["IsMust"];

                //if (type == 0 || type == 1)
                //    this.txtSelection.Text = commRow["Selection"].ToString();
                //else
                //    this.txtSelection.Text = "";
                ORM.DBModels.Common commRow = CommonProblem.GetCommonByCommIDEF(commonID);

                this.txtQuestion.Text = commRow.Text.ToString();

                int type = Convert.ToInt32(commRow.SelectionType);
                this.ddlSelectionType.SelectedValue = type.ToString();

                this.ckbIsMust.Checked = (bool)commRow.IsMust;

                if (type == 0 || type == 1)
                    this.txtSelection.Text = commRow.Selection.ToString();
                else
                    this.txtSelection.Text = "";
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
            if (string.IsNullOrWhiteSpace(this.txtQuestion.Text))
            {
                Response.Write($"<Script language='JavaScript'>alert('請輸入問題名稱'); location.href='Detail.aspx?ID={id}#tabs2'; </Script>");
                return;
            }
            // 單選或複選的回答不能為空值
            int type = Convert.ToInt32(this.ddlSelectionType.SelectedValue);
            if (string.IsNullOrWhiteSpace(this.txtSelection.Text) && (type == 0 || type == 1))
            {
                Response.Write($"<Script language='JavaScript'>alert('請輸入回答'); location.href='Detail.aspx?ID={id}#tabs2'; </Script>");
                return;
            }

            //Guid idToGuid = Guid.Parse(id);

            // 將資料庫的舊問題及頁面上加入的新問題一起放至新增的 DataTable 並存入 Session 中
            //DataTable ProblemDT = new DataTable();
            List<Problem> ProblemList = new List<Problem>();

            if (Session["ProblemDT"] == null)
                //ProblemDT = ProblemData.GetProblem(idToGuid); // 最一開始從 DB 抓
                ProblemList = ProblemData.GetProblemEF(id.ToGuid()); // 最一開始從 DB 抓
            else
                //ProblemDT = (DataTable)Session["ProblemDT"]; // Session 若有資料就直接用
                ProblemList = (List<Problem>)Session["ProblemDT"]; // Session 若有資料就直接用

            // 創建一個新的 DataTable
            //DataTable dtProb = new DataTable();
            //dtProb.Columns.Add(new DataColumn("ProbGuid", typeof(Guid)));
            //dtProb.Columns.Add(new DataColumn("QuesGuid", typeof(Guid)));
            //dtProb.Columns.Add(new DataColumn("Count", typeof(int)));
            //dtProb.Columns.Add(new DataColumn("Text", typeof(string)));
            //dtProb.Columns.Add(new DataColumn("SelectionType", typeof(int)));
            //dtProb.Columns.Add(new DataColumn("IsMust", typeof(bool)));
            //dtProb.Columns.Add(new DataColumn("Selection", typeof(string)));

            int selectedType = Convert.ToInt32(this.ddlSelectionType.SelectedValue);

            if (Session["PbGuid"] == null) // 新增問題
            {
                int count = 1;

                //if (ProblemDT.Rows.Count != 0)
                //    count = ProblemDT.Rows.Count + 1;
                if (ProblemList.Count != 0)
                    count = ProblemList.Count + 1;

                //DataRow drProb = dtProb.NewRow();
                //drProb["ProbGuid"] = Guid.NewGuid();
                //drProb["QuesGuid"] = idToGuid;
                //drProb["Count"] = count;
                //drProb["Text"] = this.txtQuestion.Text;
                //drProb["SelectionType"] = selectedType;
                //drProb["IsMust"] = this.ckbIsMust.Checked;
                //drProb["Selection"] = this.txtSelection.Text;
                Problem listProb = new Problem()
                {
                    ProbGuid = Guid.NewGuid(),
                    QuesGuid = id.ToGuid(),
                    Count = count,
                    Text = this.txtQuestion.Text,
                    SelectionType = selectedType,
                    IsMust = this.ckbIsMust.Checked,
                    Selection = this.txtSelection.Text
                };

                //dtProb.Rows.Add(drProb);
                //ProblemDT.Merge(dtProb); // 將原本資料庫的問題加上新的問題 (DataTable合併)
                ProblemList.Add(listProb);
            }
            else // 編輯問題
            {
                //Guid ProblemGuid = Guid.Parse(Session["PbGuid"].ToString());
                Guid ProblemGuid = Session["PbGuid"].ToString().ToGuid();

                //for (int i = 0; i < ProblemDT.Rows.Count; i++)
                //{
                //    if (Guid.Equals(ProblemGuid, ProblemDT.Rows[i]["ProbGuid"])) //找到符合的那一筆做更新
                //    {
                //        ProblemDT.Rows[i]["Text"] = this.txtQuestion.Text;
                //        ProblemDT.Rows[i]["SelectionType"] = selectedType;
                //        ProblemDT.Rows[i]["IsMust"] = this.ckbIsMust.Checked;
                //        ProblemDT.Rows[i]["Selection"] = this.txtSelection.Text;
                //        break;
                //    }
                //}
                for (int i = 0; i < ProblemList.Count; i++)
                {
                    if (Guid.Equals(ProblemGuid, ProblemList[i].ProbGuid)) //找到符合的那一筆做更新
                    {
                        ProblemList[i].Text = this.txtQuestion.Text;
                        ProblemList[i].SelectionType = selectedType;
                        ProblemList[i].IsMust = this.ckbIsMust.Checked;
                        ProblemList[i].Selection = this.txtSelection.Text;
                        break;
                    }
                }
                Session["PbGuid"] = null;
            }
            //HttpContext.Current.Session["ProblemDT"] = ProblemDT;
            HttpContext.Current.Session["ProblemDT"] = ProblemList;
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
                //var dr = row.DataItem as DataRowView;
                var dr = row.DataItem as Problem;
                //int ProbType = dr.Row.Field<int>("SelectionType");
                int ProbType = dr.SelectionType;
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
           // Guid idToGuid = Guid.Parse(id);

            //DataTable DTforDelete = new DataTable();
            List<Problem> DTforDelete = new List<Problem>();

            if (Session["ProblemDT"] == null)
                //DTforDelete = ProblemData.GetProblem(idToGuid); // 最一開始從 DB 抓
                DTforDelete = ProblemData.GetProblemEF(id.ToGuid()); // 最一開始從 DB 抓
            else
                //DTforDelete = (DataTable)Session["ProblemDT"]; // Session 若有資料就直接用
                DTforDelete = (List<Problem>)Session["ProblemDT"]; // Session 若有資料就直接用

            List<int> deleteNum = new List<int>();

            //for (int i = 0; i < DTforDelete.Rows.Count; i++)
            for (int i = 0; i < DTforDelete.Count; i++)
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
                //DTforDelete.Rows.Remove(DTforDelete.Rows[d]);
                DTforDelete.Remove(DTforDelete[d]);

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
            Response.Redirect("/SystemAdminPages/SList.aspx"); // Session 將於列表頁進行 Abandon
            return;
        }

        /// <summary>
        /// 送出問題：將 Session 送進資料庫做真正的新增與更改
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

            //Guid idToGuid = Guid.Parse(id);
            //DataTable SessionToDB;
            List<Problem> SessionToDB;

            if (Session["ProblemDT"] == null)
            {
                //SessionToDB = ProblemData.GetProblem(idToGuid); // 最一開始從 DB 抓
                SessionToDB = ProblemData.GetProblemEF(id.ToGuid()); // 最一開始從 DB 抓
                Response.Write($"<Script language='JavaScript'>alert('好像什麼都沒變哦~'); location.href='Detail.aspx?ID={id}#tabs2'; </Script>");
                return;
            }
            else
                //SessionToDB = (DataTable)Session["ProblemDT"]; // Session 若有資料就直接用
                //SessionToDB = (DataTable)Session["ProblemDT"]; // Session 若有資料就直接用
                SessionToDB = (List<Problem>)Session["ProblemDT"]; // Session 若有資料就直接用
                SessionToDB = (List<Problem>)Session["ProblemDT"]; // Session 若有資料就直接用

            // 先刪除後加入
            //ProblemData.DeleteProblemData(idToGuid);
            ProblemData.DeleteProblemDataEF(id.ToGuid());

            int Count = 0;

            StaticData.DeleteStaticData(id.ToGuid()); // *先刪除統計資料

            UserInfoManager.DeleteReplyInfo(id.ToGuid()); // 刪除填答人資料


            //for (int i = 0; i < SessionToDB.Rows.Count; i++)
            //{
            //    SessionToDB.Rows[i]["Count"] = i + 1;
            //    Guid ProbGuid = (Guid)SessionToDB.Rows[i]["ProbGuid"];
            //    Guid QuesGuid = (Guid)SessionToDB.Rows[i]["QuesGuid"];
            //    Count = (int)SessionToDB.Rows[i]["Count"];
            //    string Text = (string)SessionToDB.Rows[i]["Text"];
            //    int SelectionType = (int)SessionToDB.Rows[i]["SelectionType"];
            //    bool IsMust = (bool)SessionToDB.Rows[i]["IsMust"];

            //    string Selection = "";
            //    if (SelectionType == 0 || SelectionType == 1) // 只有單選和複選需要內容
            //    {
            //        Selection = (string)SessionToDB.Rows[i]["Selection"];

            //        string[] OptionText = Selection.Split(';');

            //        for (int j = 0; j < OptionText.Length; j++)
            //        {
            //            int StaticCount = 0;
            //            StaticData.CreateStaticData(QuesGuid, ProbGuid, OptionText[j], StaticCount); // *後新增出來
            //        }
            //    }
            //    ProblemData.CreateProblem(ProbGuid, QuesGuid, Count, Text, SelectionType, IsMust, Selection); // 將 Session 寫進資料庫
            //}
            for (int i = 0; i < SessionToDB.Count; i++)
            {
                SessionToDB[i].Count = i + 1;
                Guid ProbGuid = (Guid)SessionToDB[i].ProbGuid;
                Guid QuesGuid = (Guid)SessionToDB[i].QuesGuid;
                Count = (int)SessionToDB[i].Count;
                string Text = (string)SessionToDB[i].Text;
                int SelectionType = (int)SessionToDB[i].SelectionType;
                bool IsMust = (bool)SessionToDB[i].IsMust;

                string Selection = "";
                if (SelectionType == 0 || SelectionType == 1) // 只有單選和複選需要內容
                {
                    Selection = (string)SessionToDB[i].Selection;

                    string[] OptionText = Selection.Split(';');

                    for (int j = 0; j < OptionText.Length; j++)
                    {
                        int StaticCount = 0;
                        StaticData.CreateStaticData(QuesGuid, ProbGuid, OptionText[j], StaticCount); // *後新增出來
                    }
                }
                //ProblemData.CreateProblem(ProbGuid, QuesGuid, Count, Text, SelectionType, IsMust, Selection); // 將 Session 寫進資料庫
                Problem CreateProblem = new Problem()
                {
                    ProbGuid = ProbGuid,
                    QuesGuid = QuesGuid,
                    Count = Count,
                    Text = Text,
                    SelectionType = SelectionType,
                    IsMust = IsMust,
                    Selection = Selection,
                };
                ProblemData.CreateProblemEF(CreateProblem); // 將 Session 寫進資料庫
            }

            //ProblemData.UpdateQuestionnaireCount(idToGuid, Count);// 更新回問卷的 Count 問題數
            ProblemData.UpdateQuestionnaireCountEF(id.ToGuid(), Count);// 更新回問卷的 Count 問題數
            Response.Write("<Script language='JavaScript'>alert('問題編輯成功!! 新增(重置)統計及填答人資料表'); location.href='SList.aspx'; </Script>");
        }


        protected void gvProb_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {

        }
        protected void gvProb_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

        }

        #endregion

        #region 填寫資料

        /// <summary>
        /// 內建分頁
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvReply_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvReply.PageIndex = e.NewPageIndex;
            string id = this.Request.QueryString["ID"];
            Guid idToGuid = Guid.Parse(id);
            DataTable ReplyDT = UserInfoManager.GetReplyInfo(idToGuid);
            this.gvReply.DataSource = ReplyDT;
            this.gvReply.DataBind();
        }

        /// <summary>
        /// 編號以倒序方式顯示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvReply_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            string id = this.Request.QueryString["ID"];
            Guid idToGuid = Guid.Parse(id);
            DataTable ReplyDT = UserInfoManager.GetReplyInfo(idToGuid);
            int count = ReplyDT.Rows.Count;
            var row = e.Row;
            if (row.RowType == DataControlRowType.DataRow)
            {
                Label lbl = row.FindControl("lblReplyCount") as Label;
                int replyCount = gvReply.Rows.Count;
                lbl.Text = (count - replyCount).ToString();

            }
        }

        /// <summary>
        /// 返回使用者作答列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnBackToReplyInfo_Click(object sender, EventArgs e)
        {
            string id = this.Request.QueryString["ID"];
            Response.Redirect($"/SystemAdminPages/Detail.aspx?ID={id}#tabs3");
        }

        /// <summary>
        /// 匯出.csv檔案
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnOutput_Click(object sender, EventArgs e)
        {
            Guid idToGuid = Guid.Parse(this.Request.QueryString["ID"]);
            DataTable dt = QuestionnaireData.OutputToCSV(idToGuid);

            //取得機器+user name
            var loginAccount = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            //取得機器名
            var machineName = Environment.MachineName;
            //讓loginAccount減去機器名
            loginAccount = loginAccount.Remove(0, machineName.Length + 1);
            string now = DateTime.Now.ToString("yyyyMMddHHmmssfff");

            string outputPath = $@"C:\Users\{loginAccount}\Downloads\使用者作答{now}.csv";

            //if (ExcelDataManager.DataTableToExcel(dt, outputPath))
            if (ExcelDataManager.DataTableToCsv(dt, outputPath))
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "", "<script>alert('報表轉換成功，請至本機下載查看!! ')</script>");
            }
            else
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "", "<script>alert('報表轉換失敗!')</script>");
            }
        }

        #endregion


    }
}