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

        public static string logPath
        {
            get{
                string thisFilePath = HttpContext.Current.Server.MapPath("~/");   //取得自己.aspx的路徑
                string thisFilePathFather = GetUpLevelDirectory(thisFilePath, 2) + "\\DBSource\\Logs\\Log.log";   //找上2層，Logs
                return thisFilePathFather;
            }
        }
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
