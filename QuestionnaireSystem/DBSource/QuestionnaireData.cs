using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBSource
{
    public class QuestionnaireData
    {
        #region 列表頁與新增修改問卷

        /// <summary>
        /// 問卷查詢
        /// </summary>
        /// <param name="Search"></param>
        /// <returns></returns>
        public static DataTable SearchQuestionnaire(string Search)
        {
            string connectionString = DBHelper.GetConnectionString();
            string dbCommandString =
                $@"SELECT [QuesID]
                        , [QuesGuid]
                        , [Caption]
                        , [Description]
                        , [StartDate]
                        , [EndDate]
                        , [CreateDate]
                        , [State]
                        , [Count]
                    FROM [Questionnaire]
                    WHERE [Caption] LIKE '%'+@search+'%'
                    OR [Description] LIKE '%'+@search+'%'
                ";

            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(new SqlParameter("@search", Search));
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
        /// 取得問卷資料表
        /// </summary>
        /// <returns></returns>
        public static DataTable GetQuestionnaire()
        {
            string connectionString = DBHelper.GetConnectionString();
            string dbCommandString =
                $@"SELECT [QuesID]
                        , [QuesGuid]
                        , [Caption]
                        , [Description]
                        , [StartDate]
                        , [EndDate]
                        , [CreateDate]
                        , [State]
                        , [Count]
                    FROM [Questionnaire]
                    ORDER BY [QuesID] DESC
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
        /// 以 QuesGuid 取得問卷資料行
        /// </summary>
        /// <param name="QuesGuid"></param>
        /// <returns></returns>
        public static DataRow GetQuestionnaireDataRow(Guid QuesGuid)
        {
            string connectionString = DBHelper.GetConnectionString();
            string dbCommandString =
                $@"SELECT [QuesID]
                        , [QuesGuid]
                        , [Caption]
                        , [Description]
                        , [StartDate]
                        , [EndDate]
                        , [CreateDate]
                        , [State]
                        , [Count]
                    FROM [Questionnaire]
                    WHERE [QuesGuid] = @quesGuid
                ";

            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(new SqlParameter("@quesGuid", QuesGuid));
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
        /// 以 QuesID 刪除資料行
        /// </summary>
        /// <param name="QuesID"></param>
        public static void DeleteQuestionnaireData(int QuesID)
        {
            string connectionString = DBHelper.GetConnectionString();
            string dbCommandString =
                $@" DELETE [Questionnaire]
                    WHERE [QuesID] = @quesID";

            List<SqlParameter> paramList = new List<SqlParameter>();
            paramList.Add(new SqlParameter("@quesID", QuesID));

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
        /// 透過 QuesID 取得 QuesGuid 來刪除問卷中的所有問題
        /// </summary>
        /// <param name="QuesID"></param>
        /// <returns></returns>
        public static DataRow GetQuesIDForDeleteProblem(int QuesID)
        {
            string connectionString = DBHelper.GetConnectionString();
            string dbCommandString =
                $@"SELECT [QuesGuid]
                    FROM [Questionnaire]
                    WHERE [QuesID] = @quesID
                ";

            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(new SqlParameter("@quesID", QuesID));

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
        /// 因結束時間已到而關閉問卷
        /// </summary>
        /// <param name="QuesID"></param>
        /// <returns></returns>
        public static bool CloseQuesStateByTime(int QuesID)
        {
            string connStr = DBHelper.GetConnectionString();
            string dbCommand =
                $@" UPDATE [Questionnaire]
                    SET
                        [State] = 0
                    WHERE [QuesID] = @quesID";

            List<SqlParameter> paramList = new List<SqlParameter>();
            paramList.Add(new SqlParameter("@quesID", QuesID));

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
        /// 新增問卷
        /// </summary>
        /// <param name="QuesGuid"></param>
        /// <param name="Caption"></param>
        /// <param name="Description"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="State"></param>
        /// <param name="Count"></param>
        public static void CreateQuestionnaire(Guid QuesGuid, string Caption, string Description, DateTime StartDate, DateTime EndDate ,int State, int Count)
        {
            string connStr = DBHelper.GetConnectionString();
            string dbCommand =
                $@" INSERT INTO [Questionnaire]
                    (
                        QuesGuid
                       ,Caption
                       ,Description
                       ,StartDate
                       ,EndDate
                       ,State
                       ,Count
                    )
                    VALUES
                    (
                        @quesGuid
                       ,@caption
                       ,@description
                       ,@startDate
                       ,@endDate
                       ,@state
                       ,@count
                    )
                ";
            List<SqlParameter> paramList = new List<SqlParameter>();
            paramList.Add(new SqlParameter("@quesGuid", QuesGuid));
            paramList.Add(new SqlParameter("@caption", Caption));
            paramList.Add(new SqlParameter("@description", Description));
            paramList.Add(new SqlParameter("@startDate", StartDate));
            paramList.Add(new SqlParameter("@endDate", EndDate));
            paramList.Add(new SqlParameter("@state", State));
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
        /// 修改問卷
        /// </summary>
        /// <param name="QuesGuid"></param>
        /// <param name="Caption"></param>
        /// <param name="Description"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="State"></param>
        /// <returns></returns>
        public static bool EditQuestionnaire(Guid QuesGuid, string Caption, string Description, DateTime StartDate, DateTime EndDate, int State)
        {
            string connStr = DBHelper.GetConnectionString();
            string dbCommand =
                $@" UPDATE [Questionnaire]
                    SET
                        [Caption]     = @caption
                       ,[Description] = @description
                       ,[StartDate]   = @startDate
                       ,[EndDate]     = @endDate
                       ,[State]       = @state
                    WHERE
                        [QuesGuid] = @quesGuid ";

            List<SqlParameter> paramList = new List<SqlParameter>();
            paramList.Add(new SqlParameter("@quesGuid", QuesGuid));
            paramList.Add(new SqlParameter("@caption", Caption));
            paramList.Add(new SqlParameter("@description", Description));
            paramList.Add(new SqlParameter("@startDate", StartDate));
            paramList.Add(new SqlParameter("@endDate", EndDate));
            paramList.Add(new SqlParameter("@state", State));

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

        #endregion
        
    }
}
