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
    public partial class Form : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string id = this.Request.QueryString["ID"];

            if (!IsPostBack)
            {
                if (string.IsNullOrWhiteSpace(id) || id.Length != 36)
                {
                    Response.Write("<Script language='JavaScript'>alert(' QueryString 錯誤，將您導向回列表頁'); location.href='GList.aspx'; </Script>");
                    return;
                }

                Guid idToGuid = Guid.Parse(id);
                DataRow QuesRow = QuestionnaireData.GetQuestionnaireDataRow(idToGuid);  // 從 DB 抓問卷

                if (QuesRow == null || QuesRow["QuesGuid"].ToString() != id)
                {
                    Response.Write("<Script language='JavaScript'>alert(' Guid 錯誤，將您導向回列表頁'); location.href='GList.aspx'; </Script>");
                    return;
                }

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

                // 判斷問卷日期是否過期
                DateTime nowDate = DateTime.Now;
                if ((nowDate - startDate).Days < 0 || (nowDate - endDate).Days > 0)
                    Response.Write("<Script language='JavaScript'>alert('此問卷已經過期，將您導向回列表頁'); location.href='GList.aspx'; </Script>");

                // 判斷問卷狀態是否關閉
                if (Convert.ToInt32(QuesRow["State"]) == 0)
                    Response.Write("<Script language='JavaScript'>alert('此問卷已經關閉，將您導向回列表頁'); location.href='GList.aspx'; </Script>");

                this.lblCaption.Text = QuesRow["Caption"].ToString(); // 帶入標題
                this.lblDescription.Text = QuesRow["Description"].ToString(); // 帶入描述


                this.lblCount.Text = "共 " + QuesRow["Count"].ToString() + " 個問題";


                // 從確認頁返回問卷填寫頁修改資料時回填
                if (Session["Name"] != null)
                    this.txtName.Text = Session["Name"].ToString();
                if (Session["Phone"] != null)
                    this.txtPhone.Text = Session["Phone"].ToString();
                if (Session["Email"] != null)
                    this.txtEmail.Text = Session["Email"].ToString();
                if (Session["Age"] != null)
                    this.txtAge.Text = Session["Age"].ToString();

            }
        }

        /// <summary>
        /// 取消問卷填寫並返回列表頁
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCancelF_Click(object sender, EventArgs e)
        {
            Response.Redirect("GList.aspx"); // Session 將於列表頁進行 Abandon
            return;
        }

        /// <summary>
        /// 將個人資料及問題回答放進 Session 並跳至確認頁
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSendF_Click(object sender, EventArgs e)
        {
            // https://dotblogs.com.tw/topcat/2008/03/05/1200
            // Request.QueryString >> 使用 Get  >> 透過網址(URL)後面的變數來接收參數
            // Request.Form        >> 使用 Post >> 表單送出資料後，從控制項接收參數

            string id = this.Request.QueryString["ID"];
            DataTable ProblemDT = ProblemData.GetProblem(Guid.Parse(id)); // 從 DB 抓問題

            string reply = string.Empty; // 裝回答 (以分號分隔)

            // 將單一問卷中每個問題的回答，一題一題加入 reply 字串中
            for (int i = 0; i < ProblemDT.Rows.Count; i++)
            {
                // 用 Request.Form 以問題資料表之 ProbGuid 抓取每個控制項的 name >> 回答分割；檢查必填
                if (string.IsNullOrWhiteSpace(this.Request.Form[ProblemDT.Rows[i]["ProbGuid"].ToString()])) // 沒有填 (null / "") 的話那一題就是 null
                {
                    if ((bool)ProblemDT.Rows[i]["IsMust"] == true) // 沒填但是必填
                    {
                        Response.Write("<Script language='JavaScript'>alert('必填問題尚未填妥'); </Script>");
                        return;
                    }
                    reply += " "; // 若回答未填則以空白表示
                    reply += ";"; // 以 ; 分割每個回答
                }
                else
                {
                    string inpValue = this.Request.Form[ProblemDT.Rows[i]["ProbGuid"].ToString()]; // 表單送出資料後，從控制項接收參數
                    if (inpValue.Contains(";")) // 文字中不能含分號
                    {
                        Response.Write("<Script language='JavaScript'>alert('回答中不能包含分號'); </Script>");
                        return;
                    }

                    reply += inpValue; // 取每題的回答值

                    
                    if (i < ProblemDT.Rows.Count - 1) // i 從 0 開始，所以永遠會比題號少，除非最後一圈跑完 i + 1 後變成兩者相等 >> 最後一題才不加分號
                        reply += ";"; // 以 ; 分割每個回答
                }
            }

            string name = this.txtName.Text;
            string phone = this.txtPhone.Text;
            string email = this.txtEmail.Text;
            string age = this.txtAge.Text;

            // 將基本資料及回答都放進 Session
            if (!string.IsNullOrWhiteSpace(name))
                Session["Name"] = name;
            if (!string.IsNullOrWhiteSpace(phone))
                Session["Phone"] = phone;
            if (!string.IsNullOrWhiteSpace(email))
                Session["Email"] = email;
            if (!string.IsNullOrWhiteSpace(age))
                Session["Age"] = age;



            Session["Reply"] = reply; // 全部回答裝進 Session



            // 個人基本資料檢查
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(age))
            {
                Response.Write("<Script language='JavaScript'>alert('請確認個人基本資料是否正確'); </Script>");
                return;
            }

            if (name.Contains(";") || phone.Contains(";") || email.Contains(";") || age.Contains(";"))
            {
                Response.Write("<Script language='JavaScript'>alert('個人基本資料不能包含分號'); </Script>");
                return;
            }

            if (UserInfoManager.CheckPhoneIsRepeat(Guid.Parse(id), phone))
            {
                Response.Write("<Script language='JavaScript'>alert('不好意思，此問卷中手機已經使用過了'); </Script>");
                return;
            }

            if (UserInfoManager.CheckEmailIsRepeat(Guid.Parse(id), email))
            {
                Response.Write("<Script language='JavaScript'>alert('不好意思，此問卷中Email已經使用過了'); </Script>");
                return;
            }

            Response.Redirect("Confirm.aspx?ID=" + id);

            //Literal1.Text += Session["Reply"].ToString();
        }
    }
}