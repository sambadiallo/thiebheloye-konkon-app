using System.Reflection;
using System.Web;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Thiebheloye.Identity.Data.EF.Models.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataHandler.Serializer;
using Microsoft.Owin.Security.DataProtection;
using Owin;

namespace Thiebheloye.iitii.WebApi
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
            RegisterIdentityTypes(builder);

            //Caching
            // builder.RegisterType<ApiOutputCache>().As<IApiOutputCache>().SingleInstance();

            //Set the dependency resolver to be Autofac.
            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }

        private static void RegisterIdentityTypes(ContainerBuilder builder)
        {
            builder.Register(c => HttpContext.Current.GetOwinContext().Authentication).As<IAuthenticationManager>();
            builder.Register(c => HttpContext.Current.GetOwinContext().GetUserManager<UserManager>()).AsSelf();
            builder.Register(c => HttpContext.Current.GetOwinContext().GetUserManager<SignInManager>()).AsSelf();
            builder.Register(c => HttpContext.Current.GetOwinContext().GetUserManager<RoleManager>()).AsSelf();
            builder.RegisterType<TicketSerializer>().As<IDataSerializer<AuthenticationTicket>>();
            builder.Register(c => new DpapiDataProtectionProvider().Create("ASP.NET Identity")).As<IDataProtector>();
            builder.RegisterType<TicketDataFormat>().As<ISecureDataFormat<AuthenticationTicket>>();
        }

        private static void RegisterFilters(ContainerBuilder builder)
        {

        }

        private static void RegisterModules(ContainerBuilder builder)
        {
            //builder.RegisterModule(new Thiebheloye.Caching.InMemory.Config.AutofacModule());
            //builder.RegisterModule(new Thiebheloye.Caching.Redis.Config.AutofacModule());
            //builder.RegisterModule(new Thiebheloye.Caching.Sync.Config.AutofacModule());

            //builder.RegisterModule(new Thiebheloye.Data.Dapper.Config.AutofacModule());

            builder.RegisterModule(new Thiebheloye.Identity.Data.EF.Config.AutofacModule());
            builder.RegisterModule(new Thiebheloye.Identity.Domain.Commands.Config.AutofacModule());
            builder.RegisterModule(new Thiebheloye.Identity.Domain.Queries.Config.AutofacModule());

            //builder.RegisterModule(new Thiebheloye.Logging.NLog.Config.AutofacModule());

            //builder.RegisterModule(new Thiebheloye.Security.Filters.Config.AutofacModule());
            //builder.RegisterModule(new Thiebheloye.Security.Filters.Config.AutofacModule());
            //builder.RegisterModule(new Thiebheloye.Security.Identity.Config.AutofacModule());
        }
    }

}