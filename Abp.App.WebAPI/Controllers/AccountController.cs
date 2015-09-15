using Abp.App.WebAPI.Models.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Abp.App.WebAPI.Controllers
{
    /// <summary>
    /// 账户控制器
    /// </summary>
    [RoutePrefix("api/v1/account")]
    public class AccountController : ApiController
    {
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [Route("signin")]
        public async Task<IHttpActionResult> SignInAsync(LoginViewModel lg)
        {
            return Ok(new { IsError = true, Msg = string.Empty, Data = string.Empty });
        }

        /// <summary>
        /// 用户信息
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [Route("info")]
        public async Task<IHttpActionResult> InfoAsync()
        {
            return Ok(new { IsError = true, Msg = string.Empty, Data = string.Empty });
        }
    }
}