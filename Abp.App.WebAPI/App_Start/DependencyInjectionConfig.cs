using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Integration.WebApi;
using System.Web.Http;
using System.Reflection;
using System.Web.Compilation;
using Abp.App.Services.Impl;

namespace Abp.App.WebAPI.App_Start
{
    /// <summary>
    /// 依赖注入[DI]
    /// </summary>
    public class DependencyInjectionConfig
    {
        /// <summary>
        ///注册依赖注入
        /// </summary>
        public static void Register()
        {
            var builder = new ContainerBuilder();
            //注册api容器的实现
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            //注册mvc容器的实现
            //builder.RegisterControllers(Assembly.GetExecutingAssembly());
            var assemblys = BuildManager.GetReferencedAssemblies().Cast<Assembly>().ToList();
            builder.RegisterAssemblyTypes(assemblys.ToArray()).Where(t => t.Name.EndsWith("Service")).AsImplementedInterfaces();
            builder.RegisterAssemblyTypes(assemblys.ToArray()).Where(t => t.Name.EndsWith("Repository")).AsImplementedInterfaces();
            //注册 Password Grant 授权服务
            //在Autofac中注册Redis的连接，并设置为Singleton (官方建議保留Connection，重複使用)
            //builder.Register(r =>{ return ConnectionMultiplexer.Connect(DBSetting.Redis);}).AsSelf().SingleInstance();
            var container = builder.Build();
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}