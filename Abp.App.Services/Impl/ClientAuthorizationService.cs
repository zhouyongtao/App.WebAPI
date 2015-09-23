using Abp.App.Repositories;
using Apb.App.Entities.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abp.App.Services.Impl
{
    public class ClientAuthorizationService : IClientAuthorizationService
    {
        /// <summary>
        /// 客户端仓库服务
        /// </summary>
        private readonly IClientAuthorizationRepository _clientAuthorizationRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="clientAuthorizationRepository"></param>
        public ClientAuthorizationService(IClientAuthorizationRepository clientAuthorizationRepository)
        {
            _clientAuthorizationRepository = clientAuthorizationRepository;
        }

        /// <summary>
        /// 生成OAuth2 clientSecret
        /// </summary>
        /// <returns></returns>
        public async Task<string> GenerateOAuthClientSecretAsync(string client_id = "")
        {
            return await _clientAuthorizationRepository.GenerateOAuthClientSecretAsync(client_id);
        }

        /// <summary>
        /// 验证客户端[Authorization Basic Base64(clientId:clientSecret)]
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <returns></returns>
        public async Task<bool> ValidateClientAuthorizationSecretAsync(string client_id, string client_secret)
        {
            if (client_id.IsNullOrEmpty() || client_secret.IsNullOrEmpty())
                return false;
            return await _clientAuthorizationRepository.ValidateClientAuthorizationSecretAsync(client_id, client_secret);
        }

        /// <summary>
        /// 保持票据
        /// </summary>
        /// <param name="token">票据</param>
        /// <returns></returns>
        public async Task<bool> SaveTokenAsync(Token token)
        {
            return await _clientAuthorizationRepository.SaveTokenAsync(token);
        }
    }
}
