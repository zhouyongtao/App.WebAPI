using Abp.App.Services;
using Apb.App.Entities.Client;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;

namespace Abp.App.WebAPI.Providers
{
    /// <summary>
    /// 刷新Token
    /// </summary>
    public class RefreshAuthenticationTokenProvider : AuthenticationTokenProvider
    {
        private readonly ConcurrentDictionary<string, string> _authenticationCodes = new ConcurrentDictionary<string, string>(StringComparer.Ordinal);

        /// <summary>
        /// Password Grant 授权服务
        /// </summary>
        private readonly IClientAuthorizationService _clientAuthorizationService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="clientAuthorizationService">Password Grant 授权服务</param>
        public RefreshAuthenticationTokenProvider(IClientAuthorizationService clientAuthorizationService)
        {
            _clientAuthorizationService = clientAuthorizationService;
        }

        /// <summary>
        /// 创建refreshToken
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            if (string.IsNullOrEmpty(context.Ticket.Identity.Name)) return;
            var clietId = context.OwinContext.Get<string>("as:client_id");
            if (string.IsNullOrEmpty(clietId)) return;
            var refresh_token_time = context.OwinContext.Get<string>("as:refresh_token_time");
            if (string.IsNullOrEmpty(refresh_token_time)) return;
            string IpAddress = context.Request.RemoteIpAddress + ":" + context.Request.RemotePort;
            var token = new Token()
            {
                ClientId = clietId,
                UserName = context.Ticket.Identity.Name,
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.AddSeconds(Convert.ToDouble(refresh_token_time)),
                IpAddress = IpAddress
            };
            context.Ticket.Properties.IssuedUtc = token.IssuedUtc;
            context.Ticket.Properties.ExpiresUtc = token.ExpiresUtc;
            token.AccessToken = context.SerializeTicket();
            token.RefreshToken = await _clientAuthorizationService.GenerateOAuthClientSecretAsync();
            if (await _clientAuthorizationService.SaveTokenAsync(token))
            {
                context.SetToken(token.RefreshToken);
            }
            /*
            // maybe only create a handle the first time, then re-use for same client
            // copy properties and set the desired lifetime of refresh token
            var tokenProperties = new AuthenticationProperties(context.Ticket.Properties.Dictionary)
            {
                IssuedUtc = context.Ticket.Properties.IssuedUtc,
                ExpiresUtc = context.Ticket.Properties.ExpiresUtc
            };
            var token = context.SerializeTicket();
            var refreshTicket = new AuthenticationTicket(context.Ticket.Identity, tokenProperties);
            _refreshTokens.TryAdd(token, refreshTicket);
            // consider storing only the hash of the handle
            context.SetToken(token);
            */
        }

        /// <summary>
        /// 刷新refreshToken[刷新access token时，refresh token也会重新生成]
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            var token = await _clientAuthorizationService.GetTokenAsync(context.Token);
            if (token.IsNotNull() && token.AccessToken.IsNotNullOrEmpty())
            {
                await _clientAuthorizationService.RemoveTokenAsync(context.Token);
                context.DeserializeTicket(token.AccessToken);
            }
            /*
            string token = context.Token;
            string value;
            if (_authenticationCodes.TryRemove(context.Token, out value))
            {
                context.DeserializeTicket(value);
            }
            */
        }
    }
}