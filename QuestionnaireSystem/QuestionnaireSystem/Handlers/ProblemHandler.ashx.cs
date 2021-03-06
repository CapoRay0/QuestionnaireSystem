using DBSource;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace QuestionnaireSystem.Handlers
{
    /// <summary>
    /// ProblemHandler 的摘要描述
    /// </summary>
    public class ProblemHandler : IHttpHandler
    {
        /// <summary>
        /// 透過 QuesGuid 取得問題資料表，送到 Form.aspx 讓AJAX動態新增控制項
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest(HttpContext context)
        {

            string id = context.Request.QueryString["QuesGuid"];
            if (string.IsNullOrEmpty(id))
            {
                context.Response.StatusCode = 400;
                context.Response.ContentType = "text/plain";
                context.Response.Write("required");
                context.Response.End();
            }

            Guid idToGuid = Guid.Parse(id);
            DataTable dt = ProblemData.GetProblem(idToGuid); // 取得問題資料表
            string jsonText = Newtonsoft.Json.JsonConvert.SerializeObject(dt);

            context.Response.ContentType = "application/json";
            context.Response.Write(jsonText);

            //context.Response.ContentType = "text/plain";
            //context.Response.Write("Hello World");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}