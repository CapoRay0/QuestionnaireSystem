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
        /// 以QuesGuid來取得問題資料表
        /// </summary>
        /// <param name="QuesGuid"></param>
        /// <returns></returns>
        public static DataTable GetProblemForBind(Guid QuesGuid)
        {
            string connectionString = DBHelper.GetConnectionString();
            string dbCommandString =
                $@"SELECT [ProbGuid]
                        , [QuesGuid]
                        , [Text]
                        , [SelectionType]
                        , [IsMust]
                        , [Selection]
                    FROM [Problem]
                    WHERE [QuesGuid] = @quesGuid
                ";
            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(new SqlParameter("@quesGuid", QuesGuid));
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
        /// 以QuesGuid來取得問卷資料行
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
        /// 以QuesID來刪除資料行
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

        #region 新增與修改問卷
        public static void CreateQuestionnaire(Guid QuesGuid, string Caption, string Description, DateTime StartDate, DateTime EndDate, DateTime CreateDate,int State,int Count)
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
                       ,CreateDate
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
                       ,@createDate
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
            paramList.Add(new SqlParameter("@createDate", CreateDate));
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
        public static void CreateQuestionnaire(Guid QuesGuid, string Caption, string Description, DateTime StartDate, DateTime CreateDate, int State, int Count)
        {
            string connStr = DBHelper.GetConnectionString();
            string dbCommand =
                $@" INSERT INTO [Questionnaire]
                    (
                        QuesGuid
                       ,Caption
                       ,Description
                       ,StartDate
                       ,CreateDate
                       ,State
                       ,Count
                    )
                    VALUES
                    (
                        @quesGuid
                       ,@caption
                       ,@description
                       ,@startDate
                       ,@createDate
                       ,@state
                       ,@count
                    )
                ";
            List<SqlParameter> paramList = new List<SqlParameter>();
            paramList.Add(new SqlParameter("@quesGuid", QuesGuid));
            paramList.Add(new SqlParameter("@caption", Caption));
            paramList.Add(new SqlParameter("@description", Description));
            paramList.Add(new SqlParameter("@startDate", StartDate));
            paramList.Add(new SqlParameter("@createDate", CreateDate));
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

        public static bool EditQuestionnaire(Guid QuesGuid, string Caption, string Description, DateTime StartDate, int State)
        {
            string connStr = DBHelper.GetConnectionString();
            string dbCommand =
                $@" UPDATE [Questionnaire]
                    SET
                        [Caption]     = @caption
                       ,[Description] = @description
                       ,[StartDate]   = @startDate
                       ,[State]       = @state
                    WHERE
                        [QuesGuid] = @quesGuid ";

            List<SqlParameter> paramList = new List<SqlParameter>();
            paramList.Add(new SqlParameter("@quesGuid", QuesGuid));
            paramList.Add(new SqlParameter("@caption", Caption));
            paramList.Add(new SqlParameter("@description", Description));
            paramList.Add(new SqlParameter("@startDate", StartDate));
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

        #region 新增與修改問題


        #endregion
    }
}
