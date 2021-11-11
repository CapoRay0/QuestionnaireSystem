using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBSource
{
    public class StaticData
    {
        #region 各選項統計資料

        /// <summary>
        /// 新增統計資料
        /// </summary>
        /// <param name="QuesGuid"></param>
        /// <param name="ProbGuid"></param>
        /// <param name="OptionText"></param>
        /// <param name="Count"></param>
        public static void CreateStaticData(Guid QuesGuid, Guid ProbGuid, string OptionText, int Count)
        {
            string connStr = DBHelper.GetConnectionString();
            string dbCommand =
                $@" INSERT INTO [Static]
                    (
                          [QuesGuid]
                        , [ProbGuid]
                        , [OptionText]
                        , [Count]
                    )
                    VALUES
                    (
                        @quesGuid
                       ,@probGuid
                       ,@optionText
                       ,@count
                    )
                ";
            List<SqlParameter> paramList = new List<SqlParameter>();
            paramList.Add(new SqlParameter("@quesGuid", QuesGuid));
            paramList.Add(new SqlParameter("@probGuid", ProbGuid));
            paramList.Add(new SqlParameter("@optionText", OptionText));
            paramList.Add(new SqlParameter("@count", Count));

            try
            {
                DBHelper.CreatData(connStr, dbCommand, paramList);
            }
            catch (Exception ex)
            {
                logger.WriteLog(ex);
            }
        }

        /// <summary>
        /// 以 QuesGuid 來刪除統計資料
        /// </summary>
        /// <param name="QuesGuid"></param>
        public static void DeleteStaticData(Guid QuesGuid)
        {
            string connectionString = DBHelper.GetConnectionString();
            string dbCommandString =
                $@" DELETE [Static]
                    WHERE [QuesGuid] = @quesGuid";

            List<SqlParameter> paramList = new List<SqlParameter>();
            paramList.Add(new SqlParameter("@quesGuid", QuesGuid));

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
        /// 若單選或複選被打勾，則將對應的各選項 Count + 1
        /// </summary>
        /// <param name="ProbGuid"></param>
        /// <param name="OptionText"></param>
        /// <returns></returns>
        public static bool UpdateStaticCount(Guid ProbGuid, string OptionText)
        {
            string connStr = DBHelper.GetConnectionString();
            string dbCommand =
                $@" UPDATE [Static]
                    SET
                        [Count]    = [Count] + 1
                    WHERE
                        [ProbGuid] = @probGuid AND [OptionText] = @optionText";

            List<SqlParameter> paramList = new List<SqlParameter>();
            paramList.Add(new SqlParameter("@probGuid", ProbGuid));
            paramList.Add(new SqlParameter("@optionText", OptionText));

            try
            {
                int effectRows = DBHelper.ModifyData(connStr, dbCommand, paramList);

                if (effectRows == 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                logger.WriteLog(ex);
                return false;
            }
        }

        /// <summary>
        /// 取該問題中所有選項 >> 需要["Count"]
        /// </summary>
        /// <param name="ProbGuid"></param>
        /// <returns></returns>
        public static DataTable GetStatic(Guid ProbGuid)
        {
            string connectionString = DBHelper.GetConnectionString();
            string dbCommandString =
                $@"SELECT [StaticID]
                        , [QuesGuid]
                        , [ProbGuid]
                        , [OptionText]
                        , [Count]
                    FROM [Static]
                    WHERE [ProbGuid] = @probGuid
                ";
            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(new SqlParameter("@probGuid", ProbGuid));
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
        /// 抓出選取總數 ["Sum"]
        /// </summary>
        /// <param name="ProbGuid"></param>
        /// <returns></returns>
        public static DataRow GetStaticSum(Guid ProbGuid)
        {
            string connectionString = DBHelper.GetConnectionString();
            string dbCommandString =
                $@"SELECT SUM([Count]) AS [Sum]
                    FROM [Static]
                    WHERE [ProbGuid] = @probGuid
                ";

            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(new SqlParameter("@probGuid", ProbGuid));
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
        #endregion
    }
}
