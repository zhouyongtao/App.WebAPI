using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abp.App.Repositories
{
    public interface IUserRepository
    {
        /// <summary>
        /// 验证用户名密码
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="pwd">密码</param>
        /// <returns></returns>
        Task<bool> ValidateUserNameAuthorizationPwd(string userName, string pwd);
    }
}
