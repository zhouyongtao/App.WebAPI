using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Abp.App.WebAPI.Providers
{
    /// <summary>
    /// Client Credentials 授权
    /// </summary>
    public class ClientAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        /*
         private OAuth2ClientService _oauthClientService;
         public ApplicationOAuthProvider()
         {
             this.OAuth2ClientService = new OAuth2ClientService();
         }
         */

        /// <summary>
        /// 验证Client Credentials[client_id与client_secret]
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            //http://localhost:48339/token
            //grant_type=client_credentials&client_id=irving&client_secret=123456
            string client_id;
            string client_secret;
            context.TryGetFormCredentials(out client_id, out client_secret);
            /*
                  if (!context.Options.AuthenticationType.Equals("client_credentials", StringComparison.OrdinalIgnoreCase))
                  {
                      context.SetError("invalid_grant_type", "invalid grant_type");
                      return base.ValidateClientAuthentication(context);
                  }
             */
            if (client_id == "irving" && client_secret == "123456")
            {
                context.Validated(client_id);
            }
            else
            {
                //context.Response.StatusCode = Convert.ToInt32(HttpStatusCode.OK);
                context.SetError("invalid_client", "client is not valid");
            }
            return base.ValidateClientAuthentication(context);
        }

        /// <summary>
        /// 客户端授权[生成access token]
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task GrantClientCredentials(OAuthGrantClientCredentialsContext context)
        {
            /*
               var client = _oauthClientService.GetClient(context.ClientId);
               claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, client.ClientName));
             */
            var claimsIdentity = new ClaimsIdentity(context.Options.AuthenticationType);
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, "iphone"));
            var ticket = new AuthenticationTicket(claimsIdentity, new AuthenticationProperties() { AllowRefresh = true });
            context.Validated(ticket);
            return base.GrantClientCredentials(context);
        }
    }
}