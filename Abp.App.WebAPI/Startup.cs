using Abp.App.WebAPI.App_Start;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Startup))]
namespace Abp.App.WebAPI.App_Start
{
    public partial class Startup
    {
        /// <summary>
        /// 配置OWIN
        /// </summary>
        /// <param name="app"></param>
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}