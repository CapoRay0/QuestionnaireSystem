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
    public partial class Common : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                #region 問題部分
                DataTable CommonDT = CommonProblem.GetCommon(); // 從 DB 抓問題

                if (Session["CommonDT"] == null)
                {
                    if (CommonDT.Rows.Count == 0)
                        this.ltlMsg.Text = "<br /><br /><br />尚無問題";

                    for (int i = 0; i < CommonDT.Rows.Count; i++)
                        CommonDT.Rows[i]["Count"] = i + 1;

                    this.gvComm.DataSource = CommonDT;
                    this.gvComm.DataBind();
                }
                else
                {
                    DataTable NewCommonDT = (DataTable)Session["CommonDT"];

                    for (int i = 0; i < NewCommonDT.Rows.Count; i++)
                        NewCommonDT.Rows[i]["Count"] = i + 1;

                    this.gvComm.DataSource = NewCommonDT;
                    this.gvComm.DataBind();
                }


                this.btnDelete.Enabled = true;
                this.ltlMsg.Text = "";

                // 進入編輯模式：判斷是否為"新加入的問題"，或是原本 DB 就有
                if (Session["CommID"] != null)
                {
                    this.btnDelete.Enabled = false;
                    this.ltlMsg.Text = "修改模式下無法刪除問題";

                    // 判斷 Session 是否有東西，有的話用 Session 回填，沒有則從 DB 抓
                    if (Session["CommonDT"] != null) // Session
                    {
                        int CommID = Convert.ToInt32(Session["CommID"].ToString());
                        DataTable CommDT = (DataTable)Session["CommonDT"];

                        for (int i = 0; i < CommDT.Rows.Count; i++) // 找出要編輯的問題在第幾列
                        {
                            if (CommID == Convert.ToInt32(CommDT.Rows[i]["CommID"]))
                            {
                                this.txtName.Text = CommDT.Rows[i]["Name"].ToString();
                                this.txtQuestion.Text = CommDT.Rows[i]["Text"].ToString();
                                this.ddlSelectionType.SelectedValue = CommDT.Rows[i]["SelectionType"].ToString();
                                this.ckbIsMust.Checked = (bool)CommDT.Rows[i]["IsMust"];
                                this.txtSelection.Text = CommDT.Rows[i]["Selection"].ToString();
                                break;
                            }
                        }
                    }
                    else // 進頁面後直接修改 DB (只會跑第一次)
                    {
                        int CommID = Convert.ToInt32(Session["CommID"].ToString());

                        DataRow OneCommon = CommonProblem.GetCommonByCommID(CommID);
                        if (OneCommon != null)
                        {
                            this.txtName.Text = OneCommon["Name"].ToString();
                            this.txtQuestion.Text = OneCommon["Text"].ToString();
                            this.txtSelection.Text = OneCommon["Selection"].ToString();
                            this.ddlSelectionType.SelectedValue = OneCommon["SelectionType"].ToString();
                            this.ckbIsMust.Checked = (bool)OneCommon["IsMust"];
                        }
                    }
                }
                #endregion
            }
        }

        /// <summary>
        /// 新增常用問題
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.txtName.Text))
            {
                Response.Write($"<Script language='JavaScript'>alert('請輸入名稱'); location.href='Common.aspx'; </Script>");
                return;
            }
            if (string.IsNullOrWhiteSpace(this.txtQuestion.Text))
            {
                Response.Write($"<Script language='JavaScript'>alert('請輸入問題'); location.href='Common.aspx'; </Script>");
                return;
            }

            int type = Convert.ToInt32(this.ddlSelectionType.SelectedValue);
            if (string.IsNullOrWhiteSpace(this.txtSelection.Text) && (type == 0 || type == 1))
            {
                Response.Write($"<Script language='JavaScript'>alert('請輸入回答'); location.href='Common.aspx'; </Script>");
                return;
            }

            DataTable CommonDT = new DataTable();
            if (Session["CommonDT"] == null)
                CommonDT = CommonProblem.GetCommon(); // 最一開始從 DB 抓
            else
                CommonDT = (DataTable)Session["CommonDT"]; // Session 若有資料就直接用


            DataTable dtComm = new DataTable();
            dtComm.Columns.Add(new DataColumn("Count", typeof(int)));
            dtComm.Columns.Add(new DataColumn("Name", typeof(string)));
            dtComm.Columns.Add(new DataColumn("Text", typeof(string)));
            dtComm.Columns.Add(new DataColumn("SelectionType", typeof(int)));
            dtComm.Columns.Add(new DataColumn("IsMust", typeof(bool)));
            dtComm.Columns.Add(new DataColumn("Selection", typeof(string)));

            int selectedType = Convert.ToInt32(this.ddlSelectionType.SelectedValue);

            if (Session["CommID"] == null) // 新增問題
            {
                int count = 1;

                if (CommonDT.Rows.Count != 0)
                    count = CommonDT.Rows.Count + 1;

                DataRow drComm = dtComm.NewRow();
                drComm["Count"] = count;
                drComm["Name"] = this.txtName.Text;
                drComm["Text"] = this.txtQuestion.Text;
                drComm["SelectionType"] = selectedType;
                drComm["IsMust"] = this.ckbIsMust.Checked;
                drComm["Selection"] = this.txtSelection.Text;

                dtComm.Rows.Add(drComm);
                CommonDT.Merge(dtComm); // 將原本資料庫的問題加上新的問題 (DataTable合併)
            }
            else // 編輯問題
            {
                int CommID = Convert.ToInt32(Session["CommID"].ToString());

                for (int i = 0; i < CommonDT.Rows.Count; i++)
                {
                    if (CommID == Convert.ToInt32(CommonDT.Rows[i]["CommID"])) //找到符合的那一筆做更新
                    {
                        CommonDT.Rows[i]["Name"] = this.txtName.Text;
                        CommonDT.Rows[i]["Text"] = this.txtQuestion.Text;
                        CommonDT.Rows[i]["SelectionType"] = selectedType;
                        CommonDT.Rows[i]["IsMust"] = this.ckbIsMust.Checked;
                        CommonDT.Rows[i]["Selection"] = this.txtSelection.Text;
                        break;
                    }
                }
                Session["CommID"] = null;
            }
            HttpContext.Current.Session["CommonDT"] = CommonDT;
            Response.Redirect(this.Request.RawUrl);
        }

        /// <summary>
        /// GridView 呈現方式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvComm_RowDataBound(object sender, GridViewRowEventArgs e)
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
        /// 刪除常用問題
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDelete_Click(object sender, ImageClickEventArgs e)
        {
            DataTable DTforDelete = new DataTable();

            if (Session["CommonDT"] == null)
                DTforDelete = CommonProblem.GetCommon(); // 最一開始從 DB 抓
            else
                DTforDelete = (DataTable)Session["CommonDT"]; // Session 若有資料就直接用

            List<int> deleteNum = new List<int>();

            for (int i = 0; i < DTforDelete.Rows.Count; i++)
            {
                CheckBox cb = (CheckBox)gvComm.Rows[i].FindControl("ckbDelete");

                if (cb.Checked == true)
                {
                    int commIDToDelete = Convert.ToInt32(gvComm.Rows[i].RowIndex);
                    deleteNum.Add(commIDToDelete);
                }
            }

            deleteNum.Reverse(); // 從最後面開始刪除 (溢位)

            foreach (int d in deleteNum)
                DTforDelete.Rows.Remove(DTforDelete.Rows[d]);

            Session["CommonDT"] = DTforDelete;
            this.gvComm.DataBind();

            Response.Redirect(this.Request.RawUrl);
        }

        /// <summary>
        /// 編輯常用問題
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvComm_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "CommEdit")
            {
                var CommID = e.CommandArgument.ToString();
                HttpContext.Current.Session["CommID"] = CommID;
                Response.Redirect(this.Request.RawUrl);
            }
        }

        /// <summary>
        /// 取消常用問題管理，回到列表頁
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCancelP_Click(object sender, EventArgs e)
        {
            Response.Redirect("/SystemAdminPages/SList.aspx"); // Session 將於列表頁進行 Abandon
            return;
        }

        /// <summary>
        /// 送出變更，將 Session 寫進資料庫
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSendP_Click(object sender, EventArgs e)
        {
            DataTable SessionToDB;

            if (Session["CommonDT"] == null)
            {
                Response.Write($"<Script language='JavaScript'>alert('好像什麼都沒變哦~'); location.href='Common.aspx'; </Script>");
                return;
            }
            else
                SessionToDB = (DataTable)Session["CommonDT"]; // Session 若有資料就直接用

            // 先刪除後加入
            CommonProblem.DeleteCommonData();

            int Count = 0;
            for (int i = 0; i < SessionToDB.Rows.Count; i++)
            {
                SessionToDB.Rows[i]["Count"] = i + 1;
                Count = (int)SessionToDB.Rows[i]["Count"];
                string Name = (string)SessionToDB.Rows[i]["Name"];
                string Text = (string)SessionToDB.Rows[i]["Text"];
                int SelectionType = (int)SessionToDB.Rows[i]["SelectionType"];
                bool IsMust = (bool)SessionToDB.Rows[i]["IsMust"];

                string Selection = "";
                if (SelectionType == 0 || SelectionType == 1) // 只有單選和複選需要內容
                {
                    Selection = (string)SessionToDB.Rows[i]["Selection"];
                }

                CommonProblem.CreateCommon(Count, Name, Text, SelectionType, IsMust, Selection);
            }

            Response.Write("<Script language='JavaScript'>alert('常用問題編輯成功!!'); location.href='SList.aspx'; </Script>");
        }



        protected void gvComm_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {

        }
        protected void gvComm_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

        }

    }
}