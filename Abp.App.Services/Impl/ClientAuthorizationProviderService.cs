using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abp.App.Services.Impl
{
    public class ClientAuthorizationProviderService : IClientAuthorizationProviderService
    {
        public async Task<bool> ValidateClientAuthorizationSecret(string clientId, string clientSecret)
        {
            return true;
        }
    }
}
