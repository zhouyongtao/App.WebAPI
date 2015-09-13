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
    public class ClientAuthorizationRepository : IClientAuthorizationRepository
    {
        /// <summary>
        /// 验证客户端[Authorization Basic Base64(clientId:clientSecret)]
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <returns></returns>
        public async Task<bool> ValidateClientAuthorizationSecret(string clientId, string clientSecret)
        {
            const string cmdText = @"SELECT COUNT(*) FROM [dbo].[app_client] WHERE clientId=@clientId AND clientSecret=@clientSecret";
            try
            {
                return await new SqlConnection(DbSetting.App).ExecuteScalarAsync<int>(cmdText, new { clientId = clientId, clientSecret = clientSecret }) != 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
