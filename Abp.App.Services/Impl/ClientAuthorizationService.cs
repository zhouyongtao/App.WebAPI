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
        private readonly IClientAuthorizationRepository _clientAuthorizationRepository;
        public ClientAuthorizationService(IClientAuthorizationRepository clientAuthorizationRepository)
        {
            _clientAuthorizationRepository = clientAuthorizationRepository;
        }

        /// <summary>
        /// 
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
