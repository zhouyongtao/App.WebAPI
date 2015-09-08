using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
namespace Abp.App.WebAPI.Controllers
{
    /// <summary>
    /// 账户控制器
    /// </summary>
    public class AccountController : Controller
    {
        ///// <summary>
        ///// 测试Session
        ///// </summary>
        ///// <param name="model">参数</param>
        ///// <param name="returnUrl">返回地址</param>
        ///// <returns></returns>
        //public async Task<ActionResult> SignIn(LoginViewModel model, string returnUrl)
        //{
        //    try
        //    {
        //        string json = model.ToJson();
        //        Session["user"] = model;
        //        Session["json"] = json;
        //        return Content("success");
        //    }
        //    catch (Exception ex)
        //    {
        //        return Content("fail");
        //    }
        //}


        /// <summary>
        /// 设置
        /// </summary>
        /// <returns></returns>
        public ActionResult Setting()
        {
            return Content("fail");
        }
    }
}