using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Abp.App.TestMvc.Startup))]
namespace Abp.App.TestMvc
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
