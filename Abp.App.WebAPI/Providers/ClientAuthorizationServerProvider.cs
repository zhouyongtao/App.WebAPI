using Abp.App.Services;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Abp.App.WebAPI.Providers
{
    /*
      0.DI Autofac dependency injection in implementation of OAuthAuthorizationServerProvider http://stackoverflow.com/questions/25871392/autofac-dependency-injection-in-implementation-of-oauthauthorizationserverprovid
      1.启用TryGetBasicCredentials认证 validate client credentials should be stored securely (salted, hashed, iterated)，参考PDF设计
      2.增加额外字段
      3.增加scope授权权限
      4.持久化Token
      5.刷新Token后失效老的Token
      6.自定义验证【重启IIS池Token失效,验证权限】
    */

    /// <summary>
    /// Client Credentials 授权
    /// </summary>
    public class ClientAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        /// <summary>
        /// 授权服务
        /// </summary>
        private readonly IClientAuthorizationService _clientAuthorizationService;

        /// <summary>
        /// 账户服务
        /// </summary>
        private readonly IAccountService _accountService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="clientAuthorizationService">授权服务</param>
        /// <param name="accountService">用户服务</param>
        public ClientAuthorizationServerProvider(IClientAuthorizationService clientAuthorizationService, IAccountService accountService)
        {
            _clientAuthorizationService = clientAuthorizationService;
            _accountService = accountService;
        }

        /// <summary>
        /// 验证Client Credentials[client_id与client_secret]
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            //http://localhost:48339/token
            //grant_type=client_credentials&client_id=irving&client_secret=123456&scope=user order
            /*
            grant_type     授与方式（固定为 “client_credentials”）
            client_id 	   分配的调用oauth的应用端ID
            client_secret  分配的调用oaut的应用端Secret
            scope 	       授权权限。以空格分隔的权限列表，若不传递此参数，代表请求用户的默认权限
            */

            //based authentication context.TryGetBasicCredentials

            string client_id;
            string client_secret;
            context.TryGetFormCredentials(out client_id, out client_secret);
            //验证用户名密码
            if (client_id.Equals("irving", StringComparison.OrdinalIgnoreCase) && client_secret.Equals("123456", StringComparison.OrdinalIgnoreCase))
            {
                context.Validated(context.ClientId);
            }
            else
            {
                //Flurl 404 问题
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
            //验证权限
            int scopeCount = context.Scope.Count;
            if (scopeCount > 0)
            {
                string name = context.Scope[0].ToString();
            }
            //默认权限
            var claimsIdentity = new ClaimsIdentity(context.Options.AuthenticationType);
            //!!!
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, context.ClientId));
            var props = new AuthenticationProperties(new Dictionary<string, string> {
                            {
                                "client_id",context.ClientId
                            },
                            {
                                "scope",string.Join(" ",context.Scope)
                            }
                        });
            var ticket = new AuthenticationTicket(claimsIdentity, props);
            context.Validated(ticket);
            return base.GrantClientCredentials(context);
        }

        /// <summary>
        /// http://stackoverflow.com/questions/26357054/return-more-info-to-the-client-using-oauth-bearer-tokens-generation-and-owin-in
        /// My recommendation is not to add extra claims to the token if not needed, because will increase the size of the token and you will keep sending it with each request. As LeftyX advised add them as properties but make sure you override TokenEndPoint method to get those properties as a response when you obtain the toke successfully, without this end point the properties will not return in the response.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }
            return base.TokenEndpoint(context);
        }
    }
}