using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QuestionnaireSystem.UserControls
{
    public partial class UcPager : System.Web.UI.UserControl
    {
        /// <summary> 頁面 url </summary>
        public string Url { get; set; }
        /// <summary> 總筆數 </summary>
        public int TotalSize { get; set; }
        /// <summary> 頁面筆數 </summary>
        public int PageSize { get; set; }
        /// <summary> 目前頁數 </summary>
        public int CurrentPage { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        public void Bind()
        {
            // 檢查一頁筆數
            if (this.PageSize <= 0)
                throw new DivideByZeroException();

            // 算總頁數
            int totalPage = this.TotalSize / this.PageSize;
            if (this.TotalSize % this.PageSize > 0)
                totalPage += 1;

            // aaa.aspx?page=1
            this.aLinkFirst.HRef = $"{this.Url}?page=1";
            this.aLinkLast.HRef = $"{this.Url}?page={totalPage}";

            // 依目前頁數計算
            this.CurrentPage = this.GetCurrentPage();
            this.ltlCurrentPage.Text = this.CurrentPage.ToString();

            // 計算頁數
            int prevM1 = this.CurrentPage - 1; // 頁碼2
            int prevM2 = this.CurrentPage - 2; // 頁碼1
            int nextP1 = this.CurrentPage + 1; // 頁碼4
            int nextP2 = this.CurrentPage + 2; // 頁碼5

            this.aLink2.HRef = $"{this.Url}?page={prevM1}";
            this.aLink2.InnerText = prevM1.ToString();
            this.aLink1.HRef = $"{this.Url}?page={prevM2}";
            this.aLink1.InnerText = prevM2.ToString();


            this.aLink4.HRef = $"{this.Url}?page={nextP1}";
            this.aLink4.InnerText = nextP1.ToString();
            this.aLink5.HRef = $"{this.Url}?page={nextP2}";
            this.aLink5.InnerText = nextP2.ToString();


            // 依頁數，決定是否隱藏超連結，並處裡提示文字
            this.aLink1.Visible = (prevM2 > 0);
            this.aLink2.Visible = (prevM1 > 0);
            this.aLink4.Visible = (nextP1 <= totalPage);
            this.aLink5.Visible = (nextP2 <= totalPage);

            this.ltpager.Text = $"共 {this.TotalSize} 筆，共 {totalPage} 頁，目前在第 {this.GetCurrentPage()} 頁<br/>";

            //int totalPages = this.GetTotalPages();
            //this.ltpager.Text = $"共 {this.TotalSize} 筆，共 {totalPages} 頁，目前在第 {this.GetCurrentPage()} 頁<br/>";
            //for (var i = 1; i <= totalPages; i++)
            //{
            //    this.ltpager.Text += $"<a href='{this.Url}?page={i}'>{i}</a>&nbsp";
            //}
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

        //private int GetTotalPages()
        //{
        //    int pagers = this.TotalSize / this.PageSize;

        //    if ((this.TotalSize % this.PageSize) > 0)
        //        pagers += 1;

        //    return pagers;
        //}

    }
}