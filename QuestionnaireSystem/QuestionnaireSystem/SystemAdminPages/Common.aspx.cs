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
                List<ORM.DBModels.Common> CommonList = CommonProblem.GetCommonEF(); // 從 DB 抓問題

                if (Session["CommonList"] == null)
                {
                    if (CommonList.Count == 0)
                        this.ltlMsg.Text = "<br /><br /><br />尚無問題";

                    for (int i = 0; i < CommonList.Count; i++)
                        CommonList[i].Count = i + 1;

                    this.gvComm.DataSource = CommonList;
                    this.gvComm.DataBind();
                }
                else
                {
                    List<ORM.DBModels.Common> NewCommonList = (List<ORM.DBModels.Common>)Session["CommonList"];

                    for (int i = 0; i < NewCommonList.Count; i++)
                        NewCommonList[i].Count = i + 1;

                    this.gvComm.DataSource = NewCommonList;
                    this.gvComm.DataBind();
                }


                this.btnDelete.Enabled = true;
                this.ltlMsg.Text = "";

                // 進入編輯模式：判斷是否為"新加入的問題"，或是原本 DB 就有
                if (Session["Count"] != null)
                {
                    this.btnDelete.Enabled = false;
                    this.ltlMsg.Text = "修改模式下無法刪除問題";

                    // 判斷 Session 是否有東西，有的話用 Session 回填，沒有則從 DB 抓
                    if (Session["CommonList"] != null) // Session
                    {
                        int CountID = Convert.ToInt32(Session["Count"].ToString());
                        List<ORM.DBModels.Common> CommList = (List<ORM.DBModels.Common>)Session["CommonList"];

                        for (int i = 0; i < CommList.Count; i++) // 找出要編輯的問題在第幾列
                        {
                            if (CountID == Convert.ToInt32(CommList[i].Count))
                            {
                                this.txtName.Text = CommList[i].Name.ToString();
                                this.txtQuestion.Text = CommList[i].Text.ToString();
                                this.ddlSelectionType.SelectedValue = CommList[i].SelectionType.ToString();
                                this.ckbIsMust.Checked = (bool)CommList[i].IsMust;
                                this.txtSelection.Text = CommList[i].Selection.ToString();
                                break;
                            }
                        }
                    }
                    else // 進頁面後直接修改 DB (只會跑第一次)
                    {
                        int CountID = Convert.ToInt32(Session["Count"].ToString());

                        ORM.DBModels.Common OneCommon = CommonProblem.GetCommonByCommIDEF(CountID);
                        if (OneCommon != null)
                        {
                            this.txtName.Text = OneCommon.Name.ToString();
                            this.txtQuestion.Text = OneCommon.Text.ToString();
                            this.txtSelection.Text = OneCommon.Selection.ToString();
                            this.ddlSelectionType.SelectedValue = OneCommon.SelectionType.ToString();
                            this.ckbIsMust.Checked = (bool)OneCommon.IsMust;
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

            List<ORM.DBModels.Common> CommonList = new List<ORM.DBModels.Common>();
            if (Session["CommonList"] == null)
                CommonList = CommonProblem.GetCommonEF(); // 最一開始從 DB 抓
            else
                CommonList = (List<ORM.DBModels.Common>)Session["CommonList"]; // Session 若有資料就直接用


            //DataTable dtComm = new DataTable();
            //dtComm.Columns.Add(new DataColumn("Count", typeof(int)));
            //dtComm.Columns.Add(new DataColumn("Name", typeof(string)));
            //dtComm.Columns.Add(new DataColumn("Text", typeof(string)));
            //dtComm.Columns.Add(new DataColumn("SelectionType", typeof(int)));
            //dtComm.Columns.Add(new DataColumn("IsMust", typeof(bool)));
            //dtComm.Columns.Add(new DataColumn("Selection", typeof(string)));

            int selectedType = Convert.ToInt32(this.ddlSelectionType.SelectedValue);

            if (Session["Count"] == null) // 新增問題
            {
                int count = 1;

                if (CommonList.Count != 0)
                    count = CommonList.Count + 1;

                //DataRow drComm = dtComm.NewRow();
                //drComm["Count"] = count;
                //drComm["Name"] = this.txtName.Text;
                //drComm["Text"] = this.txtQuestion.Text;
                //drComm["SelectionType"] = selectedType;
                //drComm["IsMust"] = this.ckbIsMust.Checked;
                //drComm["Selection"] = this.txtSelection.Text;

                ORM.DBModels.Common newComm = new ORM.DBModels.Common()
                {
                    Count = count,
                    Name = this.txtName.Text,
                    Text = this.txtQuestion.Text,
                    SelectionType = selectedType,
                    IsMust = this.ckbIsMust.Checked,
                    Selection = this.txtSelection.Text
                };

                CommonList.Add(newComm);
                //dtComm.Rows.Add(drComm);
                //CommonDT.Merge(dtComm); // 將原本資料庫的問題加上新的問題 (DataTable合併)
            }
            else // 編輯問題
            {
                int CommID = Convert.ToInt32(Session["Count"].ToString());

                for (int i = 0; i < CommonList.Count; i++)
                {
                    if (CommID == Convert.ToInt32(CommonList[i].Count)) //找到符合的那一筆做更新
                    {
                        CommonList[i].Name = this.txtName.Text;
                        CommonList[i].Text = this.txtQuestion.Text;
                        CommonList[i].SelectionType = selectedType;
                        CommonList[i].IsMust = this.ckbIsMust.Checked;
                        CommonList[i].Selection = this.txtSelection.Text;
                        break;
                    }
                }
                Session["Count"] = null;
            }
            HttpContext.Current.Session["CommonList"] = CommonList;
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
                var dr = row.DataItem as ORM.DBModels.Common;
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
        /// 刪除常用問題
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDelete_Click(object sender, ImageClickEventArgs e)
        {
            List<ORM.DBModels.Common> ListforDelete = new List<ORM.DBModels.Common>();

            if (Session["CommonList"] == null)
                ListforDelete = CommonProblem.GetCommonEF(); // 最一開始從 DB 抓
            else
                ListforDelete = (List<ORM.DBModels.Common>)Session["CommonList"]; // Session 若有資料就直接用

            List<int> deleteNum = new List<int>();

            for (int i = 0; i < ListforDelete.Count; i++)
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
                ListforDelete.Remove(ListforDelete[d]);

            Session["CommonList"] = ListforDelete;
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
                var CountID = e.CommandArgument.ToString();
                HttpContext.Current.Session["Count"] = CountID;
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
            List<ORM.DBModels.Common> SessionToDB;

            if (Session["CommonList"] == null)
            {
                Response.Write($"<Script language='JavaScript'>alert('好像什麼都沒變哦~'); location.href='Common.aspx'; </Script>");
                return;
            }
            else
                SessionToDB = (List<ORM.DBModels.Common>)Session["CommonList"]; // Session 若有資料就直接用

            // 先刪除後加入
            CommonProblem.DeleteCommonDataEF();

            int Count = 0;
            for (int i = 0; i < SessionToDB.Count; i++)
            {
                SessionToDB[i].Count = i + 1;
                Count = (int)SessionToDB[i].Count;
                string Name = (string)SessionToDB[i].Name;
                string Text = (string)SessionToDB[i].Text;
                int SelectionType = (int)SessionToDB[i].SelectionType;
                bool IsMust = (bool)SessionToDB[i].IsMust;

                string Selection = "";
                if (SelectionType == 0 || SelectionType == 1) // 只有單選和複選需要內容
                {
                    Selection = (string)SessionToDB[i].Selection;
                }

                ORM.DBModels.Common createComm = new ORM.DBModels.Common()
                {
                    Count = Count,
                    Name = Name,
                    Text = Text,
                    SelectionType = SelectionType,
                    IsMust = IsMust,
                    Selection = Selection
                };

                CommonProblem.CreateCommonEF(createComm);
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