using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data.SqlClient;
using Abp.App.Core;
using Apb.App.Entities.Client;

namespace Abp.App.Repositories.Impl
{
    public class UserRepository : IUserRepository
    {
        /// <summary>
        /// 验证用户名密码
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="pwd">密码</param>
        /// <returns></returns>
        public async Task<bool> ValidateUserNameAuthorizationPwdAsync(string userName, string pwd)
        {
            const string cmdText = @"SELECT COUNT(*) FROM [dbo].[users] WHERE username=@username AND pwd=@pwd";
            try
            {
                return await new SqlConnection(DbSetting.App).ExecuteScalarAsync<int>(cmdText, new { userName = userName, pwd = pwd }) != 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
