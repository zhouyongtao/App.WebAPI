using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin.Security.Infrastructure;
using Abp.App.Services;
using Apb.App.Entities.Client;
using Microsoft.Owin.Security.OAuth;

namespace Abp.App.WebAPI.Providers
{
    /// <summary>
    /// 生成Token
    /// </summary>
    public class AccessTokenAuthorizationServerProvider : AuthenticationTokenProvider
    {
        /// <summary>
        /// 授权服务
        /// </summary>
        private readonly IClientAuthorizationService _clientAuthorizationService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="clientAuthorizationService">授权服务</param>
        public AccessTokenAuthorizationServerProvider(IClientAuthorizationService clientAuthorizationService)
        {
            _clientAuthorizationService = clientAuthorizationService;
        }

        //<summary>
        //创建Token
        //</summary>
        //<param name="context">上下文</param>
        //<returns></returns>
        public override async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            if (string.IsNullOrEmpty(context.Ticket.Identity.Name)) return;
            string IpAddress = context.Request.RemoteIpAddress + ":" + context.Request.RemotePort;
            var token = new Token()
            {
                ClientId = context.Ticket.Identity.Name,
                ClientType = "client_credentials",
                Scope = context.Ticket.Properties.Dictionary["scope"],
                UserName = context.Ticket.Identity.Name,
                IssuedUtc = DateTime.Parse(context.Ticket.Properties.IssuedUtc.ToString()),
                ExpiresUtc = DateTime.Parse(context.Ticket.Properties.IssuedUtc.ToString()),
                IpAddress = IpAddress
            };
            token.AccessToken = context.SerializeTicket();
            token.RefreshToken = string.Empty;//await _clientAuthorizationService.GenerateOAuthClientSecretAsync();
            //删除老的Token
            //保存新的Token
            if (await _clientAuthorizationService.SaveTokenAsync(token))
            {
                context.SetToken(token.AccessToken);
            }
        }


        //<summary>
        //验证Token
        //</summary>
        //<param name="context">上下文</param>
        //<returns></returns>
        public override async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            var request = new OAuthRequestTokenContext(context.OwinContext, context.Token);
            if (request == null || request.Token.IsNullOrEmpty())
            {
                return;
            }
            context.DeserializeTicket(context.Token);
            //验证最新的Token
        }
    }
}