using Apb.App.Entities.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abp.App.Services
{
    public interface IClientAuthorizationService
    {
        /// <summary>
        /// 生成OAuth2 clientSecret
        /// </summary>
        /// <returns></returns>
        Task<string> GenerateOAuthClientSecretAsync(string client_id = "");

        /// <summary>
        /// 验证客户端[Authorization Basic Base64(clientId:clientSecret)]
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <returns></returns>
        Task<bool> ValidateClientAuthorizationSecretAsync(string client_id, string client_secret);

        /// <summary>
        /// 保持票据
        /// </summary>
        /// <param name="token">票据</param>
        /// <returns></returns>
        Task<bool> SaveTokenAsync(Token token);

        /// <summary>
        /// 获得Token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<Token> GetTokenAsync(string token);
    }
}
