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
        public static DataRow GetUserInfoByAccount(string account)
        {
            string connectionString = DBHelper.GetConnectionString();
            string dbCommandString =
                $@"SELECT [SystemID]
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
    }
}
