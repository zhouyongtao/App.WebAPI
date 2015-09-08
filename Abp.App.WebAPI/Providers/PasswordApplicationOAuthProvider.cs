using Abp.App.Core;
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
    public class PasswordApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        /// <summary>
        /// 验证客户端[Authorization Basic Base64(username:password)]
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // validate client credentials (demo) should be stored securely (salted, hashed, iterated)
            string clientId;
            string clientSecret;
            context.TryGetBasicCredentials(out clientId, out clientSecret);
            if (clientId != "irving" && clientSecret != "123456")
            {
                //context.Rejected();
                context.SetError(AbpConstants.InvalidClient, AbpConstants.UnauthorizedClient);
                return base.ValidateClientAuthentication(context);
            }
            //need to make the client_id available for later security checks
            context.OwinContext.Set<string>("as:client_id", clientId);
            context.Validated(clientId);
            return base.ValidateClientAuthentication(context);
        }


        /// <summary>
        ///  验证Resource Owner Password Credentials Grant[username与password]
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            //var user = userService.Query(context.UserName, context.Password);
            //validate user credentials (验证用户名与密码)  should be stored securely (salted, hashed, iterated)       
            //var user = userService.Query(context.UserName, context.Password);
            if (context.UserName != "irving" && context.Password != "123456")
            {
                context.Rejected();
                return base.GrantResourceOwnerCredentials(context);
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
            return base.GrantResourceOwnerCredentials(context);
        }

        /// <summary>
        /// 刷新Token[刷新refresh_token]
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