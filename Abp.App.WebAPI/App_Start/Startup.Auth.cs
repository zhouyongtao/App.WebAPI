using Abp.App.Services.Impl;
using Abp.App.WebAPI.Providers;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;

namespace Abp.App.WebAPI.App_Start
{
    public partial class Startup
    {
        /// <summary>
        /// IOS App OAuth2 Credential Grant Password Service
        /// </summary>
        /// <param name="app"></param>
        public void ConfigureAuth(IAppBuilder app)
        {
            /*
              //测试
              app.UseOAuthAuthorizationServer(new OAuthAuthorizationServerOptions
             {
                 TokenEndpointPath = new PathString("/token"),
                 Provider = new ClientApplicationOAuthProvider(),
                 AccessTokenExpireTimeSpan = TimeSpan.FromHours(2),
                 AuthenticationMode = AuthenticationMode.Active,
                 //HTTPS is allowed only AllowInsecureHttp = false
                 AllowInsecureHttp = true
                 //ApplicationCanDisplayErrors = false
             });
             app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
             */

            app.UseOAuthBearerTokens(new OAuthAuthorizationServerOptions
            {
                // /token  api/v1/account/signin
                TokenEndpointPath = new PathString("/token"),
                //Provider = new ClientApplicationOAuthProvider(),
                Provider = new PasswordAuthorizationServerProvider(new ClientAuthorizationService()),
                RefreshTokenProvider = new RefreshAuthenticationTokenProvider(),
                AccessTokenExpireTimeSpan = TimeSpan.FromHours(2),
                AuthenticationMode = AuthenticationMode.Active,
                //HTTPS is allowed only AllowInsecureHttp = false
                AllowInsecureHttp = true
            });
        }
    }
}