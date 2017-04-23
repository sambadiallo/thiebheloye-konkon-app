using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Owin;

namespace iitii.WebApi
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

            //Set the dependency resolver to be Autofac.
            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }


        private static void RegisterModules(ContainerBuilder builder)
        {
            //builder.RegisterModule(new Thiebheloye.Domain.Commands.Config.AutofacModule());
        }
    }

}