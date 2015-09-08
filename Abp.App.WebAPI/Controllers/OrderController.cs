using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Abp.App.WebAPI.Controllers
{
    [RoutePrefix("api/v1/order")]
    public class OrderController : ApiController
    {
        /// <summary>
        /// 获得订单信息
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> GetOrderAsync(string id)
        {
            string username = User.Identity.Name;
            return Ok(new { IsError = true, Msg = username, Data = string.Empty });
        }
    }
}
