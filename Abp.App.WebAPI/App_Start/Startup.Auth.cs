using Abp.App.WebAPI.Providers;
using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.Owin;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Threading.Tasks;
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
            app.UseOAuthAuthorizationServer(new OAuthAuthorizationServerOptions()
            {
                AuthorizeEndpointPath = new PathString("/authorize"),
                TokenEndpointPath = new PathString("/token"),
                AuthenticationMode = AuthenticationMode.Passive,
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(1),
                //HTTPS is allowed only AllowInsecureHttp = false
#if DEBUG
                AllowInsecureHttp = true,
#endif
                ApplicationCanDisplayErrors = true,
                // AccessTokenProvider = GlobalConfiguration.Configuration.DependencyResolver.GetRootLifetimeScope().Resolve<AccessTokenAuthorizationServerProvider>(),
                // Provider = GlobalConfiguration.Configuration.DependencyResolver.GetRootLifetimeScope().Resolve<ClientAuthorizationServerProvider>(),
                Provider = new OAuthAuthorizationServerProvider()
                {
                    OnValidateClientRedirectUri = context =>
                    {
                        context.Validated();
                        return Task.FromResult(0);
                    },
                    OnValidateClientAuthentication = context =>
                    {
                        string clientId;
                        string clientSecret;
                        if (context.TryGetBasicCredentials(out clientId, out clientSecret) || context.TryGetFormCredentials(out clientId, out clientSecret)) context.Validated();
                        return Task.FromResult(0);
                    }
                },
                AuthorizationCodeProvider = new AuthenticationTokenProvider()
                {
                    OnCreate = context =>
                    {
                        context.SetToken(DateTime.Now.Ticks.ToString());
                        string token = context.Token;
                        string ticket = context.SerializeTicket();
                        // _authenticationCodes[token] = ticket;
                    },
                    OnReceive = context =>
                    {
                        string token = context.Token;
                        string ticket;
                        //if (_authenticationCodes.TryRemove(token, out ticket))
                        //{
                        //    context.DeserializeTicket(ticket);
                        //}
                    },
                },
                RefreshTokenProvider = new AuthenticationTokenProvider()
                {
                    OnCreate = context =>
                    {
                        context.SetToken(context.SerializeTicket());
                    },

                    OnReceive = context =>
                    {
                        context.DeserializeTicket(context.Token);
                    },
                }
            });

            /*
                  //ClientApplicationOAuthProvider
                  app.UseOAuthBearerTokens(new OAuthAuthorizationServerOptions
                  {
                      //AuthorizeEndpointPath = new PathString("/authorize")
                      TokenEndpointPath = new PathString("/token"),
                      Provider = GlobalConfiguration.Configuration.DependencyResolver.GetRootLifetimeScope().Resolve<ClientAuthorizationServerProvider>(),
                      AccessTokenProvider = GlobalConfiguration.Configuration.DependencyResolver.GetRootLifetimeScope().Resolve<AccessTokenAuthorizationServerProvider>(),
                      AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(1),
                      AuthenticationMode = AuthenticationMode.Active,
                      //HTTPS is allowed only AllowInsecureHttp = false
      #if DEBUG
                      AllowInsecureHttp = true,
      #endif
                      ApplicationCanDisplayErrors = true,
                  });

                  */


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

            //app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
        }
    }
}