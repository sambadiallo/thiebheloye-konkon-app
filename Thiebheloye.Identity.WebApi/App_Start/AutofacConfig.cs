using System.Reflection;
using System.Web;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Thiebheloye.Identity.Data.EF.Models.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.DataHandler.Serializer;
using Microsoft.Owin.Security.DataProtection;
using Owin;

namespace Thiebheloye.Identity.WebApi
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
            
            //identity
            RegisterIdentityTypes(builder);
 
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

        private static void RegisterModules(ContainerBuilder builder)
        {
            builder.RegisterModule(new Data.EF.Config.AutofacModule());
            builder.RegisterModule(new Domain.Commands.Config.AutofacModule());
            builder.RegisterModule(new Domain.Queries.Config.AutofacModule());
        }
    }

}