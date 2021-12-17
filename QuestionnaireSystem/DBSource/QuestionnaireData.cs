using QuestionnaireSystem.ORM.DBModels;
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
        public static List<Questionnaire> SearchQuestionnaireEF(string Search)
        {
            try
            {
                using (ContextModel context = new ContextModel())
                {
                    var query = (from item in context.Questionnaires
                                where item.Caption.Contains(Search) || item.Description.Contains(Search)
                                select item);

                    var list = query.ToList();
                    return list;
                }
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
        public static List<Questionnaire> GetQuestionnaireEF()
        {
            try
            {
                using (ContextModel context = new ContextModel())
                {
                    var query =
                        (from item in context.Questionnaires
                         orderby item.QuesID descending
                         select item);

                    var list = query.ToList();
                    return list;
                }
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
        public static Questionnaire GetQuestionnaireDataRowEF(Guid QuesGuid)
        {
            try
            {
                using (ContextModel context = new ContextModel())
                {
                    var query =
                        (from item in context.Questionnaires
                         where item.QuesGuid == QuesGuid
                         select item);

                    var list = query.FirstOrDefault();
                    return list;
                }
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
        public static void DeleteQuestionnaireDataEF(int QuesID)
        {
            try
            {
                using (ContextModel context = new ContextModel())
                {
                    var dbObjects = context.Questionnaires.Where(obj => obj.QuesID == QuesID);
                    if (dbObjects != null)
                    {
                        foreach (var dbObject in dbObjects)
                            context.Questionnaires.Remove(dbObject);

                        context.SaveChanges();
                    }
                }
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
        public static Questionnaire GetQuesIDForDeleteProblemEF(int QuesID)
        {
            try
            {
                using (ContextModel context = new ContextModel())
                {
                    var query =
                        (from item in context.Questionnaires
                         where item.QuesID == QuesID
                         select item);

                    var list = query.FirstOrDefault();
                    return list;
                }
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
        public static void CloseQuesStateByTimeEF(int QuesID)
        {
            try
            {
                using (ContextModel context = new ContextModel())
                {
                    var query =
                        (from item in context.Questionnaires
                         where item.QuesID == QuesID
                         select item);

                    var list = query.FirstOrDefault();
                    if (list != null)
                    {
                        list.State = 0;
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                logger.WriteLog(ex);
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

        /// <summary>
        /// 從資料庫取得要匯出的 DataTable
        /// </summary>
        /// <param name="QuesGuid"></param>
        /// <returns></returns>
        public static DataTable OutputToCSV(Guid QuesGuid)
        {
            string connectionString = DBHelper.GetConnectionString();
            string dbCommandString =
                $@"SELECT [User].[Name] AS 姓名
                         ,[User].[Phone] AS 電話
	                     ,[User].[Email] AS Email
	                     ,[User].[Age] AS 年齡
	                     ,[Ques].[Caption] AS 問卷名稱
	                     ,[Prob].[Text] AS 問題
	                     ,[Prob].[Selection] AS 問題選項
	                     ,[Ans].[AnswerText] AS 回答
	                     ,[User].CreateDate AS 填寫時間
                   FROM [Questionnaire] AS [Ques]
                     JOIN [ReplyInfo] AS [User] ON [Ques].QuesGuid = [User].QuesGuid
                     JOIN [Reply] AS [Ans] ON [User].UserGuid = [Ans].UserGuid
                     JOIN [Problem] AS [Prob] ON [Ans].ProbGuid = [Prob].ProbGuid
                   WHERE [Ques].QuesGuid = @quesGuid
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

    }
}
