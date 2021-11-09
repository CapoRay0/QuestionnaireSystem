using DBSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QuestionnaireSystem
{
    public partial class CreateSystemInfo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            List<string> msgList = new List<string>();
            if (!this.CheckInput(out msgList))
            {
                this.lblMsg.Text = string.Join("<br/>", msgList);
                return;
            }

            Guid SystemGuid = Guid.NewGuid();
            string Name = this.txtName.Text;
            string Phone = this.txtPhone.Text;
            string Email = this.txtEmail.Text;
            string Account = this.txtAccount.Text;
            string Pwd = this.txtPwd.Text;

            UserInfoManager.CreateReplyInfo(SystemGuid, Name, Phone, Email, Account, Pwd);

            Response.Write("<Script language='JavaScript'>alert('您的帳號已新增成功!'); location.href='Login.aspx'; </Script>");
        }

        private bool CheckInput(out List<string> errorMsgList)
        {
            List<string> msgList = new List<string>();

            // 姓名 必填
            if (string.IsNullOrWhiteSpace(this.txtName.Text))
            {
                msgList.Add("請輸入姓名");
                errorMsgList = msgList;
                return false;
            }

            // 手機號碼 必填
            if (string.IsNullOrWhiteSpace(this.txtPhone.Text))
            {
                msgList.Add("請輸入手機號碼");
                errorMsgList = msgList;
                return false;
            }

            // Email 必填
            if (string.IsNullOrWhiteSpace(this.txtEmail.Text))
            {
                msgList.Add("請輸入Email");
                errorMsgList = msgList;
                return false;
            }

            // 帳號 必填
            if (string.IsNullOrWhiteSpace(this.txtAccount.Text))
            {
                msgList.Add("請輸入帳號");
                errorMsgList = msgList;
                return false;
            }

            // 密碼 必填
            if (string.IsNullOrWhiteSpace(this.txtPwd.Text))
            {
                msgList.Add("請輸入密碼");
                errorMsgList = msgList;
                return false;
            }

            // 確認密碼 必填
            if (string.IsNullOrWhiteSpace(this.txtPwdConf.Text))
            {
                msgList.Add("請輸入確認密碼");
                errorMsgList = msgList;
                return false;
            }


            // 新密碼及確認新密碼須為一致
            if (this.txtPwd.Text.Trim() != this.txtPwdConf.Text.Trim())
            {
                msgList.Add("請確認新密碼是否一致");
                errorMsgList = msgList;
                return false;
            }

            // 密碼長度限制 (8~16)
            if (this.txtPwd.Text.Length < 8 || this.txtPwd.Text.Length > 16)
            {
                msgList.Add("密碼長度限制 (需要8~16碼)");
                errorMsgList = msgList;
                return false;
            }


            errorMsgList = msgList;


            if (msgList.Count == 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 返回登入頁
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnBackToLogin_Click(object sender, EventArgs e)
        {
            Response.Redirect("Login.aspx");
        }

    }
}