using Abp.App.WebAPI.Providers;
using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Web.Http;
using System.Web.Mvc;

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
            /*    */
            //测试 ClientApplicationOAuthProvider
            app.UseOAuthAuthorizationServer(new OAuthAuthorizationServerOptions
            {
                //AuthorizeEndpointPath = new PathString("/authorize")
                TokenEndpointPath = new PathString("/token"),
                Provider = new ClientAuthorizationServerProvider(),
                AccessTokenExpireTimeSpan = TimeSpan.FromHours(2),
                AuthenticationMode = AuthenticationMode.Active,
                //HTTPS is allowed only AllowInsecureHttp = false
#if DEBUG
                AllowInsecureHttp = true,
#endif
                ApplicationCanDisplayErrors = true,
            });
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

            /*
               //PasswordAuthorizationServerProvider
               app.UseOAuthBearerTokens(new OAuthAuthorizationServerOptions
               {
                   //!!!
                   // AccessTokenProvider=
                   TokenEndpointPath = new PathString("/token"),
                   //Provider = new ClientApplicationOAuthProvider(),
                   //Provider = new PasswordAuthorizationServerProvider(),
                   //Provider = DependencyInjectionConfig.container.Resolve<PasswordAuthorizationServerProvider>(),
                   //Provider = DependencyResolver.Current.GetService<PasswordAuthorizationServerProvider>(),
                   Provider = GlobalConfiguration.Configuration.DependencyResolver.GetRootLifetimeScope().Resolve<PasswordAuthorizationServerProvider>(),
                   RefreshTokenProvider = GlobalConfiguration.Configuration.DependencyResolver.GetRootLifetimeScope().Resolve<RefreshAuthenticationTokenProvider>(),
                   AccessTokenExpireTimeSpan = TimeSpan.FromHours(2),
                   AuthenticationMode = AuthenticationMode.Active,
                   //HTTPS is allowed only AllowInsecureHttp = false
   #if DEBUG
                   AllowInsecureHttp = true,
   #endif
               });
               */
        }
    }
}