using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBSource
{
    public class UserInfoManager
    {
        #region 前端登入

        /// <summary>
        /// 透過帳號來取得使用者資訊
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static DataRow GetUserInfoByAccount(string account)
        {
            string connectionString = DBHelper.GetConnectionString();
            string dbCommandString =
                $@"SELECT [SystemGuid]
                        , [Name]
                        , [Phone]
                        , [Email]
                        , [Account]
                        , [Password]
                    FROM [SystemInfo]
                    WHERE [Account] = @account
                ";

            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(new SqlParameter("@account", account));

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
        /// 新增問卷管理帳號
        /// </summary>
        /// <param name="SystemGuid"></param>
        /// <param name="Name"></param>
        /// <param name="Phone"></param>
        /// <param name="Email"></param>
        /// <param name="Account"></param>
        /// <param name="Pwd"></param>
        public static void CreateReplyInfo(Guid SystemGuid, string Name, string Phone, string Email, string Account, string Pwd)
        {
            string connStr = DBHelper.GetConnectionString();
            string dbCommand =
                $@" INSERT INTO [SystemInfo]
                    (
                          [SystemGuid]
                        , [Name]
                        , [Phone]
                        , [Email]
                        , [Account]
                        , [Password]
                    )
                    VALUES
                    (
                        @systemGuid
                       ,@name
                       ,@phone
                       ,@email
                       ,@account
                       ,@pwd
                    )
                ";
            List<SqlParameter> paramList = new List<SqlParameter>();
            paramList.Add(new SqlParameter("@systemGuid", SystemGuid));
            paramList.Add(new SqlParameter("@name", Name));
            paramList.Add(new SqlParameter("@phone", Phone));
            paramList.Add(new SqlParameter("@email", Email));
            paramList.Add(new SqlParameter("@account", Account));
            paramList.Add(new SqlParameter("@pwd", Pwd));

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
        /// 透過手機與Email來更改密碼
        /// </summary>
        /// <param name="Phone"></param>
        /// <param name="Email"></param>
        /// <param name="Newpwd"></param>
        /// <returns></returns>
        public static bool ChangePwd(string Phone, string Email, string Newpwd)
        {
            string connStr = DBHelper.GetConnectionString();
            string dbCommand =
                $@" UPDATE [SystemInfo]
                    SET [Password] = @Newpwd                    
                    WHERE Phone = @phone AND Email = @email";

            List<SqlParameter> paramList = new List<SqlParameter>();
            paramList.Add(new SqlParameter("@phone", Phone));
            paramList.Add(new SqlParameter("@email", Email));
            paramList.Add(new SqlParameter("@Newpwd", Newpwd));

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
        /// 確認手機與Email是否正確
        /// </summary>
        /// <param name="Phone"></param>
        /// <param name="Email"></param>
        /// <returns></returns>
        public static bool CheckInfoIsCorrectForForgotPWD(string Phone, string Email)
        {
            string connStr = DBHelper.GetConnectionString();
            string dbCommand =
                $@" SELECT 
                        [Phone]
                        ,[Email]
                    FROM [SystemInfo]
                    WHERE Phone = @phone AND Email = @email";

            List<SqlParameter> paramList = new List<SqlParameter>();
            paramList.Add(new SqlParameter("@phone", Phone));
            paramList.Add(new SqlParameter("@email", Email));

            try
            {
                var dr = DBHelper.ReadDataRow(connStr, dbCommand, paramList);

                //if (!CheckDataRowIsNull(dr))
                if (dr != null)
                {
                    var OrigPhone = dr[0].ToString();
                    var OrigEmail = dr[1].ToString();
                    if (Phone.Trim() == OrigPhone.Trim() && Email.Trim() == OrigEmail.Trim())
                    {
                        return true;
                    }
                    else
                        return false;
                }
                return false;
            }
            catch (Exception ex)
            {
                logger.WriteLog(ex);
                return false;
            }
        }

        #endregion

        #region 答題人

        /// <summary>
        /// 檢查同一問卷中手機是否重複
        /// </summary>
        /// <param name="QuesGuid"></param>
        /// <param name="Phone"></param>
        /// <returns></returns>
        public static bool CheckPhoneIsRepeat(Guid QuesGuid, string Phone)
        {
            string connStr = DBHelper.GetConnectionString();
            string dbCommand =
                $@" SELECT [Phone]
                    FROM [ReplyInfo]
                    WHERE [QuesGuid] = @quesGuid AND [Phone] = @phone";

            List<SqlParameter> paramList = new List<SqlParameter>();
            paramList.Add(new SqlParameter("@quesGuid", QuesGuid));
            paramList.Add(new SqlParameter("@phone", Phone));

            try
            {
                var dr = DBHelper.ReadDataRow(connStr, dbCommand, paramList);

                if (dr != null)
                {
                    var OrigPhone = dr[0].ToString();
                    if (Phone.Trim() == OrigPhone.Trim())
                    {
                        return true;
                    }
                    else
                        return false;
                }
                return false;
            }
            catch (Exception ex)
            {
                logger.WriteLog(ex);
                return false;
            }
        }

        /// <summary>
        /// 檢查同一問卷中Email是否重複
        /// </summary>
        /// <param name="QuesGuid"></param>
        /// <param name="Email"></param>
        /// <returns></returns>
        public static bool CheckEmailIsRepeat(Guid QuesGuid, string Email)
        {
            string connStr = DBHelper.GetConnectionString();
            string dbCommand =
                $@" SELECT [Email]
                    FROM [ReplyInfo]
                    WHERE [QuesGuid] = @quesGuid AND [Email] = @email";

            List<SqlParameter> paramList = new List<SqlParameter>();
            paramList.Add(new SqlParameter("@quesGuid", QuesGuid));
            paramList.Add(new SqlParameter("@email", Email));

            try
            {
                var dr = DBHelper.ReadDataRow(connStr, dbCommand, paramList);

                if (dr != null)
                {
                    var OrigEmail = dr[0].ToString();
                    if (Email.Trim() == OrigEmail.Trim())
                    {
                        return true;
                    }
                    else
                        return false;
                }
                return false;
            }
            catch (Exception ex)
            {
                logger.WriteLog(ex);
                return false;
            }
        }

        /// <summary>
        /// 列出使用者作答內容(填寫資料頁)
        /// </summary>
        /// <param name="QuesGuid"></param>
        /// <returns></returns>
        public static DataTable GetReplyInfo(Guid QuesGuid)
        {
            string connectionString = DBHelper.GetConnectionString();
            string dbCommandString =
                $@"SELECT [UserGuid]
                        , [QuesGuid]
                        , [Name]
                        , [Phone]
                        , [Email]
                        , [Age]
                        , [CreateDate]
                    FROM [ReplyInfo]
                    WHERE [QuesGuid] = @quesGuid
                    ORDER BY [CreateDate] DESC
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

        #endregion

    }
}
