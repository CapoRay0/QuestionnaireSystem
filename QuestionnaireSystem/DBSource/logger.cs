using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.IO;

namespace DBSource
{
    public class logger
    {
        //錯誤測試
        //Exception ex = new NullReferenceException();
        //logger.WriteLog(ex);

        /// <summary>
        /// 指定路徑
        /// </summary>
        public static string logPath
        {
            get{
                string thisFilePath = HttpContext.Current.Server.MapPath("~/");   //取得自己.aspx的路徑
                string thisFilePathFather = GetUpLevelDirectory(thisFilePath, 2) + "\\DBSource\\Logs\\Log.log";   //找上2層，Logs
                return thisFilePathFather;
            }
        }

        /// <summary>
        /// 將錯誤內容寫到 DBSource\Logs\Log.log 之中
        /// </summary>
        /// <param name="ex"></param>
        public static void WriteLog(Exception ex)
        {
            string msg =
                $@" {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}
                    {ex.ToString()}
                ";

            //string logPath = "D:\\Logs\\Log.log";
            string folderPath = System.IO.Path.GetDirectoryName(logPath);

            if (!System.IO.Directory.Exists(folderPath))
                System.IO.Directory.CreateDirectory(folderPath);

            if (!System.IO.File.Exists(logPath))
                System.IO.File.Create(logPath);

            System.IO.File.AppendAllText(logPath, msg);

            throw ex;
        }

        /// <summary>
        /// 回傳指定路徑上n層的絕對路徑
        /// </summary>
        /// <param name="path">指定路徑</param>
        /// <param name="upLevel">上n層</param>
        /// <returns></returns>
        public static string GetUpLevelDirectory(string path, int upLevel)
        {
            var directory = File.GetAttributes(path).HasFlag(FileAttributes.Directory)
                ? path
                : Path.GetDirectoryName(path);

            upLevel = upLevel < 0 ? 0 : upLevel;

            for (var i = 0; i < upLevel; i++)
            {
                directory = Path.GetDirectoryName(directory);
            }

            return directory;
        }
    }

}
