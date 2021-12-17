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
    public class ProblemData
    {
        #region 新增與修改問題

        /// <summary>
        /// 以 QuesGuid 取得問題資料表
        /// </summary>
        /// <param name="QuesGuid"></param>
        /// <returns></returns>
        public static DataTable GetProblem(Guid QuesGuid)
        {
            string connectionString = DBHelper.GetConnectionString();
            string dbCommandString =
                $@"SELECT [ProbGuid]
                        , [QuesGuid]
                        , [Count]
                        , [Text]
                        , [SelectionType]
                        , [IsMust]
                        , [Selection]
                    FROM [Problem]
                    WHERE [QuesGuid] = @quesGuid
                    ORDER BY [Count] ASC
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
        public static List<Problem> GetProblemEF(Guid QuesGuid)
        {
            try
            {
                using (ContextModel context = new ContextModel())
                {
                    var query =
                        (from item in context.Problems
                         where item.QuesGuid == QuesGuid
                         orderby item.Count
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
        /// 以 ProbGuid 取得該筆問題的內容
        /// </summary>
        /// <param name="ProbGuid"></param>
        /// <returns></returns>
        public static DataRow GetProblemDataRow(Guid ProbGuid)
        {
            string connectionString = DBHelper.GetConnectionString();
            string dbCommandString =
                $@"SELECT [ProbGuid]
                        , [QuesGuid]
                        , [Count]
                        , [Text]
                        , [SelectionType]
                        , [IsMust]
                        , [Selection]
                    FROM [Problem]
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
        public static Problem GetProblemDataRowEF(Guid ProbGuid)
        {
            try
            {
                using (ContextModel context = new ContextModel())
                {
                    var query =
                        (from item in context.Problems
                         where item.ProbGuid == ProbGuid
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
        /// 以 QuesGuid 刪除所有問卷中的問題
        /// </summary>
        /// <param name="QuesGuid"></param>
        public static void DeleteProblemData(Guid QuesGuid)
        {
            string connectionString = DBHelper.GetConnectionString();
            string dbCommandString =
                $@" DELETE [Problem]
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
        public static void DeleteProblemDataEF(Guid QuesGuid)
        {
            try
            {
                using (ContextModel context = new ContextModel())
                {
                    var dbObjects = context.Problems.Where(obj => obj.QuesGuid == QuesGuid);
                    if (dbObjects != null)
                    {
                        foreach (var dbObject in dbObjects)
                            context.Problems.Remove(dbObject);

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
        /// 以 Session["ProblemDT"] 新增問題
        /// </summary>
        /// <param name="ProbGuid"></param>
        /// <param name="QuesGuid"></param>
        /// <param name="Count"></param>
        /// <param name="Text"></param>
        /// <param name="SelectionType"></param>
        /// <param name="IsMust"></param>
        /// <param name="Selection"></param>
        public static void CreateProblem(Guid ProbGuid, Guid QuesGuid, int Count, string Text, int SelectionType, bool IsMust, string Selection)
        {
            string connStr = DBHelper.GetConnectionString();
            string dbCommand =
                $@" INSERT INTO [Problem]
                    (
                          [ProbGuid]
                        , [QuesGuid]
                        , [Count]
                        , [Text]
                        , [SelectionType]
                        , [IsMust]
                        , [Selection]
                    )
                    VALUES
                    (
                        @probGuid
                       ,@quesGuid
                       ,@count
                       ,@text
                       ,@selectionType
                       ,@isMust
                       ,@selection
                    )
                ";
            List<SqlParameter> paramList = new List<SqlParameter>();
            paramList.Add(new SqlParameter("@probGuid", ProbGuid));
            paramList.Add(new SqlParameter("@quesGuid", QuesGuid));
            paramList.Add(new SqlParameter("@count", Count));
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
        public static void CreateProblemEF(Problem problem)
        {
            try
            {
                using (ContextModel context = new ContextModel())
                {
                    context.Problems.Add(problem);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                logger.WriteLog(ex);
            }
        }

        /// <summary>
        /// 新增問題後更新問卷的問題數
        /// </summary>
        /// <param name="QuesGuid"></param>
        /// <param name="Count"></param>
        /// <returns></returns>
        public static bool UpdateQuestionnaireCount(Guid QuesGuid, int Count)
        {
            string connStr = DBHelper.GetConnectionString();
            string dbCommand =
                $@" UPDATE [Questionnaire]
                    SET
                        [Count]    = @count
                    WHERE
                        [QuesGuid] = @quesGuid";

            List<SqlParameter> paramList = new List<SqlParameter>();
            paramList.Add(new SqlParameter("@quesGuid", QuesGuid));
            paramList.Add(new SqlParameter("@count", Count));

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
        public static void UpdateQuestionnaireCountEF(Guid QuesGuid, int Count)
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
                    if (list != null)
                        list.Count = Count;

                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                logger.WriteLog(ex);
            }
        }
        #endregion
    }
}
