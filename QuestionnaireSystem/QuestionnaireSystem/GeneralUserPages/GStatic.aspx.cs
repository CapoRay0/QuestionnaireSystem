using DBSource;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls;
//    System.Web.UI.DataVisualization.Charting <== 給Asp.Net使用
//    System.Windows.Forms.DataVisualization.Charting <== 給Windows Forms使用

namespace QuestionnaireSystem.GeneralUserPages
{
    public partial class GStatic : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // https://www.796t.com/post/OW9iaXM=.html  //使用ASP.NET CHART CONTROL新增動態圖表
            // https://www.c-sharpcorner.com/UploadFile/0c1bb2/pie-type-chart-in-Asp-Net/ //Pie Chart In ASP.Net
            // http://blog.sina.com.cn/s/blog_51beaf0e0100yffo.html //C# 中 MSCHART 餅狀圖顯示百分比
            // https://www.syscom.com.tw/ePaper_Content_EPArticledetail.aspx?id=108&EPID=164&j=4 //MSChart 基本介面運用介紹
            // https://github.com/FakeStandard/ChartControl // 找不到要求類型 'GET' 的 HTTP 處理常式 >> Web.config
            // https://devil0827.pixnet.net/blog/post/296913272 // ASP.net C# 長條圖 & 圓餅圖 & 折線圖

            string id = this.Request.QueryString["ID"];

            if (!IsPostBack)
            {
                if (!string.IsNullOrWhiteSpace(id) && id.Length == 36)
                {
                    DataRow QuesRow = QuestionnaireData.GetQuestionnaireDataRow(Guid.Parse(id)); // 從 DB 抓問卷
                    DataTable ProblemDT = ProblemData.GetProblem(Guid.Parse(id)); // 從 DB 抓問題

                    //先判斷 QuesGuid 是否有誤
                    if (QuesRow == null || QuesRow["QuesGuid"].ToString() != id)
                    {
                        Response.Write("<Script language='JavaScript'>alert(' Guid 錯誤，將您導向回列表頁'); location.href='GList.aspx'; </Script>");
                        return;
                    }
                    else // 印出問卷名稱及描述內容
                    {
                        this.ltlCaption.Text = "<h2>" + QuesRow["Caption"].ToString() + "</h2><br />";
                        this.ltlDescription.Text = QuesRow["Description"].ToString();
                    }

                    for (int perProblem = 0; perProblem < ProblemDT.Rows.Count; perProblem++) // 跑每個問題
                    {
                        string probGuid = ProblemDT.Rows[perProblem]["ProbGuid"].ToString(); // 先以 QuesGuid 找問題DT
                        DataTable StaticDT = StaticData.GetStatic(Guid.Parse(probGuid)); // 再以問題DT的 ProbGuid 找統計DT >> 每一題

                        Label problemText = new Label();
                        int type = Convert.ToInt32(ProblemDT.Rows[perProblem]["SelectionType"]);
                        if (type == 0 || type == 1) // 單選、複選
                        {
                            problemText.Text = (perProblem + 1).ToString() + "." + ProblemDT.Rows[perProblem]["Text"];
                            PlaceHolder1.Controls.Add(problemText); // 印出問題名，替代Chart的Title(圖形的標題集合)

                            Panel perChart = new Panel();// 一個圖表給他一個Panel，可佔去一行(自動換行)
                            //BindChart(perProblem, perChart);
                            BindChart(perChart, StaticDT);
                            PlaceHolder1.Controls.Add(perChart); // Panel再裝進PlaceHolder(可裝控制項的容器)
                        }
                        else // 文字
                        {
                            problemText.Text = (perProblem + 1).ToString() + "." + ProblemDT.Rows[perProblem]["Text"] + "<br />&nbsp &nbsp -<br /><br /><br />";
                            PlaceHolder1.Controls.Add(problemText);
                        }
                    }
                }
                else
                    Response.Write("<Script language='JavaScript'>alert(' QueryString 錯誤，將您導向回列表頁'); location.href='GList.aspx'; </Script>");
            }
        }

        // 葉師傅
        private void BindChart(Panel panelChart, DataTable StaticDT)
        {
            //一個Chart之中，可以有多個ChartArea，一個ChartArea可以有多個Series，一個Series對應一個Legend。
            Chart chart = new Chart(); // 圖表本身(根類別)
            ChartArea area = new ChartArea("Area"); // 圖表的區域集合(圖表的繪圖區)，要放進上一行的Chart
            Series series = new Series("Series"); // 圖形集合(匯入資料實際呈現的圖形樣式、形狀)，可於ChartArea中添加多個Series
            Legend legend = new Legend("Legend"); // Series的圖例集合，標注圖形中各個線條或顏色的含義(圖例說明)
            // Annotations : 圖形的註解集合，可設置註解物件的放置位置、呈現顏色、大小、文字內容樣式等常見屬性。
            
            chart.ChartAreas.Add(area); // 圖表區域集合
            chart.Series.Add(series); // 數據序列集合
            chart.Legends.Add(legend); // 圖例集合說明

            chart.Width = 570;
            chart.Height = 300;
            chart.ChartAreas["Area"].Area3DStyle.Enable3D = true; // 3D
            chart.ChartAreas["Area"].AxisX.Interval = 1;
            chart.Series["Series"].ChartType = SeriesChartType.Pie; // 圓餅圖
            chart.Series["Series"].Label = "#PERCENT{P2}"; // 顯示百分比

            // 參考來源：http://blog.sina.com.cn/s/blog_51beaf0e0100yffo.html
            LegendCellColumn CellColumns1 = new LegendCellColumn();
            LegendCellColumn CellColumns2 = new LegendCellColumn();
            chart.Legends["Legend"].CellColumns.Add(CellColumns1);
            chart.Legends["Legend"].CellColumns.Add(CellColumns2);
            chart.Legends["Legend"].CellColumns[0].ColumnType = LegendCellColumnType.SeriesSymbol;
            chart.Legends["Legend"].CellColumns[1].ColumnType = LegendCellColumnType.Text;
            chart.Legends["Legend"].CellColumns[1].Text = "#VALX  =>  #VALY 票";

            for (int j = 0; j < StaticDT.Rows.Count; j++) // X 軸為選項， Y 軸為數量
                chart.Series["Series"].Points.AddXY(StaticDT.Rows[j]["OptionText"], StaticDT.Rows[j]["Count"]);

            panelChart.Controls.Add(chart);
        }

        // #VALX           = 顯示當前圖例的X軸的對應文字(或資料)
        // #VAL, #VALY,    = 顯示當前圖例的Y軸的對應文字(或資料)
        // #VALY2, #VALY3, = 顯示當前圖例的輔助Y軸的對應文字(或資料)
        // #SER:           = 顯示當前圖例的名稱
        // #LABEL          = 顯示當前圖例的標籤文字
        // #INDEX          = 顯示當前圖例的索引
        // #PERCENT        = 顯示當前圖例的所佔的百分比
        // #TOTAL          = 總數量
        // #LEGENDTEXT     = 圖例文字
    }
}