using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using AmazonBBS.BLL;
using AmazonBBS.Model;
using Autofac;
using Autofac.Integration.Mvc;


namespace AmazonBBS
{
    public class AutofacConfig
    {
        public static void Register()
        {
            // Autofac注入
            var builder = new ContainerBuilder();
            //类名.EndsWith("Controller")
            builder.RegisterControllers(Assembly.GetExecutingAssembly())
                //自动注入属性，无需通过构造函数进行赋予
                .PropertiesAutowired();

            ////这样既支持接口 又支持自己的类型
            //builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
            //    .AsImplementedInterfaces().AsSelf();
            builder.RegisterType<UserService>().As<IUserService>().InstancePerLifetimeScope();
            builder.RegisterType<ScoreService>().As<IScoreService>().InstancePerLifetimeScope();
            builder.RegisterType<NoticeService>().As<INoticeService>().InstancePerLifetimeScope();
            builder.RegisterType<AutoSendService>().As<IAutoSendService>().InstancePerLifetimeScope();
            builder.RegisterType<OrderService>().As<IOrderService>().InstancePerLifetimeScope();
            builder.Register(o => new AmazonBBSDBContext()).InstancePerRequest();

            //容器
            var container = builder.Build();
            //注入改为Autofac注入
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}