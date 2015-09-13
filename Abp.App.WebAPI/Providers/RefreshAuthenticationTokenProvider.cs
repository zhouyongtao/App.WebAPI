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
        private static ConcurrentDictionary<string, AuthenticationTicket> _refreshTokens = new ConcurrentDictionary<string, AuthenticationTicket>();

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
            var refreshTokenLifeTime = context.OwinContext.Get<string>("as:clientRefreshTokenLifeTime");
            //if (string.IsNullOrEmpty(refreshTokenLifeTime)) return;
       
            string ip = context.Request.RemoteIpAddress;
            int? port = context.Request.RemotePort;
            var token = new Token()
            {
                ClientId = clietId,
                UserName = context.Ticket.Identity.Name,
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.AddSeconds(Convert.ToDouble(refreshTokenLifeTime)),
            };
            context.Ticket.Properties.IssuedUtc = token.IssuedUtc;
            context.Ticket.Properties.ExpiresUtc = token.ExpiresUtc;
            token.Access_token= context.SerializeTicket();
            token.Refresh_token = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).TrimEnd('=').Replace('+', '-').Replace('/', '_');
            if (await _clientAuthorizationService.SaveTokenAsync(token))
            {
                context.SetToken(token.Refresh_token);
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
        public override Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            string token = context.Token;
            AuthenticationTicket ticket;
            if (_refreshTokens.TryRemove(context.Token, out ticket))
            {
                context.SetTicket(ticket);
            }
            return base.ReceiveAsync(context);
        }
    }
}