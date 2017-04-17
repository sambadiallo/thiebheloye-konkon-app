using System.Reflection;
using System.Web;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using KonKon.Data.EF.Models.Identity;
using KonKon.Mobile.WebApi.Controllers;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Owin;

namespace KonKon.Mobile.WebApi
{
    public static class AutofacConfig
    {
        public static void Configure(IAppBuilder app)
        {
            var builder = new ContainerBuilder();

            // Get your HttpConfiguration.
            var config = GlobalConfiguration.Configuration;

            // Register your Web API controllers.
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            // OPTIONAL: Register the Autofac filter provider.
            builder.RegisterWebApiFilterProvider(config);


            //Modules
            RegisterModules(builder);

            //Filters 
            //RegisterFilters(builder);

            //identity
            builder.Register(c => HttpContext.Current.GetOwinContext().Authentication).As<IAuthenticationManager>();
            builder.Register(c => HttpContext.Current.GetOwinContext().GetUserManager<UserManager>()).AsSelf();
            builder.Register(c => HttpContext.Current.GetOwinContext().GetUserManager<SignInManager>()).AsSelf();
            builder.Register(c => HttpContext.Current.GetOwinContext().GetUserManager<RoleManager>()).AsSelf();

            //Caching
            // builder.RegisterType<ApiOutputCache>().As<IApiOutputCache>().SingleInstance();


            //Set the dependency resolver to be Autofac.
            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }

        private static void RegisterFilters(ContainerBuilder builder)
        {

        }

        private static void RegisterModules(ContainerBuilder builder)
        {
            builder.RegisterModule(new Caching.InMemory.Config.AutofacModule());
            builder.RegisterModule(new Caching.Redis.Config.AutofacModule());
            builder.RegisterModule(new Caching.Sync.Config.AutofacModule());

            builder.RegisterModule(new Data.Dapper.Config.AutofacModule());
            builder.RegisterModule(new Data.EF.Config.AutofacModule());

            builder.RegisterModule(new Domain.Commands.Config.AutofacModule());
            builder.RegisterModule(new Domain.Queries.Config.AutofacModule());

            builder.RegisterModule(new Logging.NLog.Config.AutofacModule());

            builder.RegisterModule(new Security.Filters.Config.AutofacModule());
            builder.RegisterModule(new Security.Filters.Config.AutofacModule());
            builder.RegisterModule(new Security.Identity.Config.AutofacModule());
        }
    }

}