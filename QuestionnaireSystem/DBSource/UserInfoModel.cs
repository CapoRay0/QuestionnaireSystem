using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBSource
{
    public class UserInfoModel
    {
        #region 帳號下轄參數

        public Guid SystemGuid { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }

        #endregion
    }
}
