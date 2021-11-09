using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBSource
{
    public class CommonProblem
    {
        #region 常用問題

        /// <summary>
        /// 取得常用問題資料表
        /// </summary>
        /// <returns></returns>
        public static DataTable GetCommon()
        {
            string connectionString = DBHelper.GetConnectionString();
            string dbCommandString =
                $@"SELECT [CommID]
                        , [Count]
                        , [Name]
                        , [Text]
                        , [SelectionType]
                        , [IsMust]
                        , [Selection]
                    FROM [Common]
                    ORDER BY [CommID] ASC
                ";
            List<SqlParameter> list = new List<SqlParameter>();

            try
            {
                return DBHelper.ReadDataTable(connectionString, dbCommandString, list);
            }
            catch (Exception ex)
            {
                logger.WriteLog(ex);
                return null;
            }
        }

        /// <summary>
        /// 以 CommID 取得該筆常用問題的內容
        /// </summary>
        /// <param name="CommID"></param>
        /// <returns></returns>
        public static DataRow GetCommonByCommID(int CommID)
        {
            string connectionString = DBHelper.GetConnectionString();
            string dbCommandString =
                $@"SELECT [CommID]
                        , [Count]
                        , [Name]
                        , [Text]
                        , [SelectionType]
                        , [IsMust]
                        , [Selection]
                    FROM [Common]
                    WHERE [CommID] = @commID
                ";

            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(new SqlParameter("@commID", CommID));

            try
            {
                return DBHelper.ReadDataRow(connectionString, dbCommandString, list);
            }
            catch (Exception ex)
            {
                logger.WriteLog(ex);
                return null;
            }
        }

        /// <summary>
        /// 刪除常用問題中所有內容
        /// </summary>
        public static void DeleteCommonData()
        {
            string connectionString = DBHelper.GetConnectionString();
            string dbCommandString =
                $@" DELETE [Common]";

            List<SqlParameter> paramList = new List<SqlParameter>();

            try
            {
                DBHelper.ModifyData(connectionString, dbCommandString, paramList);
            }
            catch (Exception ex)
            {
                logger.WriteLog(ex);
            }
        }

        /// <summary>
        /// 以 Session["CommonDT"] 進資料庫新增常用問題
        /// </summary>
        /// <param name="Count"></param>
        /// <param name="Name"></param>
        /// <param name="Text"></param>
        /// <param name="SelectionType"></param>
        /// <param name="IsMust"></param>
        /// <param name="Selection"></param>
        public static void CreateCommon(int Count, string Name, string Text, int SelectionType, bool IsMust, string Selection)
        {
            string connStr = DBHelper.GetConnectionString();
            string dbCommand =
                $@" INSERT INTO [Common]
                    (
                          [Count]
                        , [Name]
                        , [Text]
                        , [SelectionType]
                        , [IsMust]
                        , [Selection]
                    )
                    VALUES
                    (
                        @count
                       ,@name
                       ,@text
                       ,@selectionType
                       ,@isMust
                       ,@selection
                    )
                ";
            List<SqlParameter> paramList = new List<SqlParameter>();
            paramList.Add(new SqlParameter("@count", Count));
            paramList.Add(new SqlParameter("@name", Name));
            paramList.Add(new SqlParameter("@text", Text));
            paramList.Add(new SqlParameter("@selectionType", SelectionType));
            paramList.Add(new SqlParameter("@isMust", IsMust));
            paramList.Add(new SqlParameter("@selection", Selection));

            try
            {
                DBHelper.CreatData(connStr, dbCommand, paramList);
            }
            catch (Exception ex)
            {
                logger.WriteLog(ex);
            }
        }

        #endregion
    }
}
