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
        public async Task<bool> ValidateClientAuthorizationSecret(string clientId, string clientSecret)
        {
            const string cmdText = @"SELECT *FROM [dbo].[app_client] WHERE clientId=@clientId AND clientSecret=@clientSecret";
            try
            {
                var client = await new SqlConnection(DbSetting.App).QueryAsync<AppClient>(cmdText, new { clientId = clientId, clientSecret = clientSecret }).ContinueWith(t => t.Result.SingleOrDefault());
                return client != null;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
