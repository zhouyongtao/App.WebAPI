using Abp.App.Core;
using Abp.App.Services;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace Abp.App.WebAPI.Providers
{
    /// <summary>
    /// Resource Owner Password Credentials Grant 授权
    /// </summary>
    public class PasswordAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        /// <summary>
        /// Password Grant 授权服务
        /// </summary>
        private readonly IClientAuthorizationService _clientAuthorizationService;

        /// <summary>
        /// 用户服务
        /// </summary>
        private readonly IUserService _userService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="clientAuthorizationService">Password Grant 授权服务</param>
        /// <param name="userService">用户服务</param>
        public PasswordAuthorizationServerProvider(IClientAuthorizationService clientAuthorizationService, IUserService userService)
        {
            _clientAuthorizationService = clientAuthorizationService;
            _userService = userService;
        }

        /// <summary>
        /// 验证客户端 [Authorization Basic Base64(clientId:clientSecret)|Authorization: Basic 5zsd8ewF0MqapsWmDwFmQmeF0Mf2gJkW]
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            //validate client credentials should be stored securely (salted, hashed, iterated)
            string clientId;
            string clientSecret;
            context.TryGetBasicCredentials(out clientId, out clientSecret);
            var clientValid = await _clientAuthorizationService.ValidateClientAuthorizationSecret(clientId, clientSecret);
            if (!clientValid)
            {
                //context.Rejected();
                context.SetError(AbpConstants.InvalidClient, AbpConstants.UnauthorizedClient);
                return;
            }
            //need to make the client_id available for later security checks
            context.OwinContext.Set<string>("as:client_id", clientId);
            //context.OwinContext.Set<string>("as:clientRefreshTokenLifeTime", _clientAuthorizationProviderService.RefreshTokenLifeTime.ToString());
            context.Validated(clientId);
        }

        /// <summary>
        ///  验证用户名与密码 [Resource Owner Password Credentials Grant[username与password]|grant_type=password&username=irving&password=654321]
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            //validate user credentials (验证用户名与密码)  should be stored securely (salted, hashed, iterated) 
            var userValid = await _userService.ValidateUserNameAuthorizationPwd(context.UserName, context.Password);
            if (!userValid)
            {
                context.Rejected();
                return;
            }
            //create identity
            var claimsIdentity = new ClaimsIdentity(context.Options.AuthenticationType);
            claimsIdentity.AddClaim(new Claim("sub", context.UserName));
            claimsIdentity.AddClaim(new Claim("role", "user"));
            // create metadata to pass on to refresh token provider
            var props = new AuthenticationProperties(new Dictionary<string, string>
                            {
                                {"as:client_id", context.ClientId }
                            });
            var ticket = new AuthenticationTicket(claimsIdentity, props);
            context.Validated(ticket);
        }

        /// <summary>
        /// 刷新Token [刷新refresh_token]
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            if (context.Ticket == null || context.Ticket.Identity == null || !context.Ticket.Identity.IsAuthenticated)
            {
                context.Rejected();
                return base.GrantRefreshToken(context);
            }
            var originalClient = context.Ticket.Properties.Dictionary["as:client_id"];
            var currentClient = context.OwinContext.Get<string>("as:client_id");
            // enforce client binding of refresh token
            if (originalClient != currentClient)
            {
                context.Rejected();
                return base.GrantRefreshToken(context);
            }
            // chance to change authentication ticket for refresh token requests
            var newId = new ClaimsIdentity(context.Ticket.Identity);
            newId.AddClaim(new Claim("newClaim", "refreshToken"));
            var newTicket = new AuthenticationTicket(newId, context.Ticket.Properties);
            context.Validated(newTicket);
            return base.GrantRefreshToken(context);
        }
    }
}