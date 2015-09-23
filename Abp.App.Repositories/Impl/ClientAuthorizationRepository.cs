using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using System.Data.SqlClient;
using Abp.App.Core;
using Apb.App.Entities.Client;
using System.Security.Cryptography;

namespace Abp.App.Repositories.Impl
{
    public class ClientAuthorizationRepository : IClientAuthorizationRepository
    {
        /// <summary>
        /// 生成OAuth2 clientSecret
        /// </summary>
        /// <returns></returns>
        public async Task<string> GenerateOAuthClientSecretAsync(string client_id = "")
        {
            //！！！ http://stackoverflow.com/questions/23652166/how-to-generate-oauth-2-client-id-and-secret
            return await Task.Run(() =>
            {
                var cryptoRandomDataGenerator = new RNGCryptoServiceProvider();
                byte[] buffer = Guid.NewGuid().ToByteArray();
                if (client_id.IsNotNullOrEmpty())
                {
                    buffer = client_id.ToByteArray();
                }
                cryptoRandomDataGenerator.GetBytes(buffer);
                return Convert.ToBase64String(buffer).TrimEnd('=').Replace('+', '-').Replace('/', '_');
            });
        }

        /// <summary>
        /// 验证客户端[Authorization Basic Base64(clientId:clientSecret)]
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <returns></returns>
        public async Task<bool> ValidateClientAuthorizationSecretAsync(string client_id, string client_secret)
        {
            const string cmdText = @"SELECT COUNT(*) FROM [dbo].[clients] WHERE client_id=@clientId AND client_secret=@clientSecret";
            try
            {
                return await new SqlConnection(DbSetting.App).ExecuteScalarAsync<int>(cmdText, new { clientId = client_id, clientSecret = client_secret }) != 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 保持票据
        /// </summary>
        /// <param name="token">票据</param>
        /// <returns></returns>
        public async Task<bool> SaveTokenAsync(Token token)
        {
            const string cmdText = @"INSERT INTO Tokens(access_token,token_type, expires_in ,refresh_token ,userName, clientId ,issuedUtc ,expiresUtc) VALUES(@access_token,@token_type, @expires_in ,@refresh_token ,@userName, @clientId ,@issuedUtc ,@expiresUtc)";
            try
            {
                // var data = await new SqlConnection(DbSetting.App).InsertAsync(token);
                return await new SqlConnection(DbSetting.App).ExecuteAsync(cmdText, new
                {
                    access_token = token.Access_token,
                    token_type = token.Token_type,
                    expires_in = token.Expires_in,
                    refresh_token = token.Refresh_token,
                    userName = token.UserName,
                    clientId = token.ClientId,
                    issuedUtc = token.IssuedUtc,
                    expiresUtc = token.ExpiresUtc
                }) != 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
