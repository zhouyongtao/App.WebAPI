using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Abp.App.WebAPI.Providers
{
    /// <summary>
    /// 刷新Token
    /// </summary>
    public class ApplicationRefreshTokenProvider : AuthenticationTokenProvider
    {
        private static ConcurrentDictionary<string, AuthenticationTicket> _refreshTokens = new ConcurrentDictionary<string, AuthenticationTicket>();

        /// <summary>
        /// 创建refreshToken
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            var clietId = context.OwinContext.Get<string>("as:client_id");
            if (clietId.IsNullOrEmpty())
            {
                return base.CreateAsync(context);
            }
            var token = context.SerializeTicket();
            // maybe only create a handle the first time, then re-use for same client
            // copy properties and set the desired lifetime of refresh token
            var tokenProperties = new AuthenticationProperties(context.Ticket.Properties.Dictionary)
            {
                IssuedUtc = context.Ticket.Properties.IssuedUtc,
                ExpiresUtc = DateTime.UtcNow.AddMonths(1)
            };
            var refreshTicket = new AuthenticationTicket(context.Ticket.Identity, tokenProperties);
            _refreshTokens.TryAdd(token, refreshTicket);
            // consider storing only the hash of the handle
            context.SetToken(token);
            return base.CreateAsync(context);
        }

        /// <summary>
        /// 刷新refreshToken
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            AuthenticationTicket ticket;
            if (_refreshTokens.TryRemove(context.Token, out ticket))
            {
                context.SetTicket(ticket);
            }
            return base.ReceiveAsync(context);
        }
    }

    /// <summary>
    /// 刷新Token
    /// </summary>
    public class RefreshToken
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public Guid ClientId { get; set; }
        public DateTime IssuedUtc { get; set; }
        public DateTime ExpiresUtc { get; set; }
        public string ProtectedTicket { get; set; }
    }
}