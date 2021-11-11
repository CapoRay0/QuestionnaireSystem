using DBSource;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataFormatTransfer
{
    public class ExcelDataManager
    {
        /// <summary>
        /// 將 DataTable 輸出成 csv 檔
        /// </summary>
        /// <param name="dtDataTable"></param>
        /// <param name="strFilePath"></param>
        /// <returns></returns>
        public static bool DataTableToCsv(DataTable dtDataTable, string strFilePath)
        {
            // Encoding.GetEncoding(65001) >> 處裡編碼問題(UTF-8)
            // 或是 System.Text.Encoding.Default 
            StreamWriter sw = new StreamWriter(strFilePath, false, Encoding.GetEncoding(65001));
            //headers    
            for (int i = 0; i < dtDataTable.Columns.Count; i++)
            {
                sw.Write(dtDataTable.Columns[i]);
                if (i < dtDataTable.Columns.Count - 1)
                {
                    sw.Write(",");
                }
            }
            sw.Write(sw.NewLine);
            foreach (DataRow dr in dtDataTable.Rows)
            {
                for (int i = 0; i < dtDataTable.Columns.Count; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
                        string value = dr[i].ToString();
                        if (value.Contains(','))
                        {
                            value = String.Format("\"{0}\"", value);
                            sw.Write(value);
                        }
                        else
                        {
                            sw.Write(dr[i].ToString());
                        }
                    }
                    if (i < dtDataTable.Columns.Count - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write(sw.NewLine);
            }
            sw.Close();
            return true;
        }

        // https://einboch.pixnet.net/blog/post/244317830
        // https://www.c-sharpcorner.com/UploadFile/deveshomar/export-datatable-to-csv-using-extension-method/


        /// <summary>
        /// 逗號導致溢位
        /// </summary>
        /// <param name="oTable"></param>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static bool SaveToCSV(DataTable oTable, string FilePath)
        {
            string data = "";
            StreamWriter wr = new StreamWriter(FilePath, false, System.Text.Encoding.Default);
            foreach (DataColumn column in oTable.Columns)
            {
                data += column.ColumnName + ",";
            }
            data += "\n";
            wr.Write(data);
            data = "";

            foreach (DataRow row in oTable.Rows)
            {
                foreach (DataColumn column in oTable.Columns)
                {
                    data += row[column].ToString().Trim() + ",";
                }
                data += "\n";
                wr.Write(data);
                data = "";
            }
            data += "\n";

            wr.Dispose();
            wr.Close();
            return true;
        }

        /// <summary>
        /// 舊專案 >> xlsx
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="Outpath"></param>
        /// <returns></returns>
        public static bool DataTableToExcel(DataTable dt, string Outpath)
        {
            bool result = false;
            try
            {
                if (dt == null || dt.Rows.Count == 0 || string.IsNullOrEmpty(Outpath))
                { throw new Exception("輸入的DataTable或路徑例外"); }
                int sheetIndex = 0;
                //根據輸出路徑的擴展名判斷workbook的實體型別
                IWorkbook workbook = null;
                string pathExtensionName = Outpath.Trim().Substring(Outpath.Length - 5); //檢查有沒有".csv"

                //if (pathExtensionName.Contains(".csv"))
                //{
                //    workbook = new XSSFWorkbook(); //如果有則不管他
                //}
                //else
                //{
                //    Outpath = Outpath.Trim() + ".csv";
                //    workbook = new XSSFWorkbook();
                //}
                if (pathExtensionName.Contains(".xlsx"))
                {
                    workbook = new XSSFWorkbook();
                }
                else if (pathExtensionName.Contains(".xls"))
                {
                    workbook = new HSSFWorkbook();
                }
                else
                {
                    Outpath = Outpath.Trim() + ".xls";
                    workbook = new HSSFWorkbook();
                }
                
                //將DataTable匯出為Excel
                // foreach (DataTable dt in dataSet.Tables)

                sheetIndex++;
                if (dt != null && dt.Rows.Count > 0)
                {
                    ISheet sheet = workbook.CreateSheet(string.IsNullOrEmpty(dt.TableName) ? ("sheet" + sheetIndex) : dt.TableName);//創建一個名稱為Sheet0的表
                    int rowCount = dt.Rows.Count;//行數
                    int columnCount = dt.Columns.Count;//列數

                    //設定列頭
                    IRow row = sheet.CreateRow(0);//excel第一行設為列頭
                    for (int c = 0; c < columnCount; c++)
                    {
                        ICell cell = row.CreateCell(c);
                        cell.SetCellValue(dt.Columns[c].ColumnName);
                    }

                    //設定每行每列的單元格,
                    for (int i = 0; i < rowCount; i++)
                    {
                        row = sheet.CreateRow(i + 1);
                        for (int j = 0; j < columnCount; j++)
                        {
                            ICell cell = row.CreateCell(j);//excel第二行開始寫入資料
                            cell.SetCellValue(dt.Rows[i][j].ToString());
                        }
                    }
                }


                //向outPath輸出資料
                using (FileStream fs = File.OpenWrite(Outpath))
                {
                    workbook.Write(fs);//向打開的這個xls檔案中寫入資料
                    result = true;
                }
                return result;
            }
            catch (Exception ex)
            {
                logger.WriteLog(ex);
                return false;
            }
        }

    }
}
