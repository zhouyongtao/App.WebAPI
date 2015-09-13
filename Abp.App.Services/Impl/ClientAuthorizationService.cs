using Abp.App.Repositories;
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
        /// 验证客户端[Authorization Basic Base64(clientId:clientSecret)]
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <returns></returns>
        public async Task<bool> ValidateClientAuthorizationSecret(string clientId, string clientSecret)
        {
            if (clientId.IsNullOrEmpty() || clientSecret.IsNullOrEmpty())
                return false;
            return await _clientAuthorizationRepository.ValidateClientAuthorizationSecret(clientId, clientSecret);
        }

    }
}
