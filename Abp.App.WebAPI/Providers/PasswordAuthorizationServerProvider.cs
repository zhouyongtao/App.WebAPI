using Abp.App.Core;
using Abp.App.Services;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Abp.App.WebAPI.Providers
{
    /// <summary>
    /// Resource Owner Password Credentials Grant 授权 [http://stackoverflow.com/questions/26357054/return-more-info-to-the-client-using-oauth-bearer-tokens-generation-and-owin-in]
    /// </summary>
    public class PasswordAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        /// <summary>
        /// Password Grant 授权服务
        /// </summary>
        private readonly IClientAuthorizationService _clientAuthorizationService;

        /// <summary>
        ///账户服务
        /// </summary>
        private readonly IAccountService _accountService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="clientAuthorizationService">Password Grant 授权服务</param>
        /// <param name="accountService">用户服务</param>
        public PasswordAuthorizationServerProvider(IClientAuthorizationService clientAuthorizationService, IAccountService accountService)
        {
            _clientAuthorizationService = clientAuthorizationService;
            _accountService = accountService;
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
            var clientValid = await _clientAuthorizationService.ValidateClientAuthorizationSecretAsync(clientId, clientSecret);
            if (!clientValid)
            {
                //context.Rejected();
                context.SetError(AbpConstants.InvalidClient, AbpConstants.InvalidClientErrorDescription);
                return;
            }
            //need to make the client_id available for later security checks
            context.OwinContext.Set<string>("as:client_id", clientId);
            context.OwinContext.Set<string>("as:refresh_token_time", "36000");
            context.Validated(clientId);
        }

        /// <summary>
        ///  验证用户名与密码 [Resource Owner Password Credentials Grant[username与password]|grant_type=password&username=irving&password=654321]
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            //validate user credentials (验证用户名与密码)  should be stored securely (salted, hashed, iterated) 
            var userValid = await _accountService.ValidateUserNameAuthorizationPwdAsync(context.UserName, context.Password);
            if (!userValid)
            {
                //context.Rejected();
                context.SetError(AbpConstants.AccessDenied, AbpConstants.AccessDeniedErrorDescription);
                return;
            }
            var claimsIdentity = new ClaimsIdentity(context.Options.AuthenticationType);
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
            var ticket = new AuthenticationTicket(claimsIdentity, new AuthenticationProperties());
            context.Validated(ticket);
            /*
            //create identity
            var claimsIdentity = new ClaimsIdentity(context.Options.AuthenticationType);
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
            claimsIdentity.AddClaim(new Claim("sub", context.UserName));
            claimsIdentity.AddClaim(new Claim("role", "user"));
            // create metadata to pass on to refresh token provider
            var props = new AuthenticationProperties(new Dictionary<string, string>
                            {
                                {"as:client_id", context.ClientId }
                            });
            var ticket = new AuthenticationTicket(claimsIdentity, props);
            context.Validated(ticket);
            */
        }

        /*
              public override Task ValidateTokenRequest(OAuthValidateTokenRequestContext context)
              {
                  var token = context.TokenRequest;
                  return base.ValidateTokenRequest(context);
              }
        */

        /// <summary>
        /// 验证持有 refresh token 的客户端
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            if (context.Ticket == null || context.Ticket.Identity == null)
            {
                context.Rejected();
                return base.GrantRefreshToken(context);
            }
            var currentClient = context.OwinContext.Get<string>("as:client_id");
            var originalClient = context.ClientId;
            // enforce client binding of refresh token
            if (originalClient != currentClient)
            {
                context.Rejected();
                return base.GrantRefreshToken(context);
            }
            context.OwinContext.Set<string>("as:client_id", currentClient);
            context.OwinContext.Set<string>("as:refresh_token_time", "36000");
            // chance to change authentication ticket for refresh token requests
            var claimsIdentity = new ClaimsIdentity(context.Ticket.Identity);
            var newTicket = new AuthenticationTicket(claimsIdentity, context.Ticket.Properties);
            context.Validated(newTicket);
            return base.GrantRefreshToken(context);
        }
    }
}