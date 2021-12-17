using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBSource
{
    public class DBHelper
    {
        /// <summary>
        /// 取得連線字串
        /// </summary>
        /// <returns></returns>
        public static string GetConnectionString()
        {
            string val = ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString;
            return val;
        }

        /// <summary>
        /// 讀取 DataTable
        /// </summary>
        /// <param name="connStr"></param>
        /// <param name="dbCommand"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DataTable ReadDataTable(string connStr, string dbCommand, List<SqlParameter> list)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand comm = new SqlCommand(dbCommand, conn))
                {
                    comm.Parameters.AddRange(list.ToArray());

                    conn.Open();
                    var reader = comm.ExecuteReader();

                    DataTable dt = new DataTable();
                    dt.Load(reader);

                    return dt;
                }
            }
        }

        /// <summary>
        ///  讀取 DataRow
        /// </summary>
        /// <param name="connStr"></param>
        /// <param name="dbCommand"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DataRow ReadDataRow(string connStr, string dbCommand, List<SqlParameter> list)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand comm = new SqlCommand(dbCommand, conn))
                {
                    comm.Parameters.AddRange(list.ToArray());

                    conn.Open();
                    var reader = comm.ExecuteReader();

                    DataTable dt = new DataTable();
                    dt.Load(reader);

                    if (dt.Rows.Count == 0)
                        return null;

                    DataRow dr = dt.Rows[0];
                    return dt.Rows[0];

                }
            }
        }

        /// <summary>
        /// 修改資料
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="dbCommandString"></param>
        /// <param name="paramList"></param>
        /// <returns></returns>
        public static int ModifyData(string connectionString, string dbCommandString, List<SqlParameter> paramList)
        {
            // connect db & execute
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand comm = new SqlCommand(dbCommandString, conn))
                {
                    comm.Parameters.AddRange(paramList.ToArray());

                    conn.Open();
                    int effectRowsCount = comm.ExecuteNonQuery();
                    return effectRowsCount;
                }
            }
        }

        /// <summary>
        /// 新增資料 >> 也可用 ModifyData(回傳值但不接收) 的方式代替 >> 多載 Overload
        /// </summary>
        /// <param name="connStr"></param>
        /// <param name="dbCommand"></param>
        /// <param name="paramList"></param>
        public static void CreatData(string connStr, string dbCommand, List<SqlParameter> paramList)
        {
            // connect db & execute
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand comm = new SqlCommand(dbCommand, conn))
                {
                    comm.Parameters.AddRange(paramList.ToArray());

                    conn.Open();
                    comm.ExecuteNonQuery();
                }
            }
        }
    }
}
