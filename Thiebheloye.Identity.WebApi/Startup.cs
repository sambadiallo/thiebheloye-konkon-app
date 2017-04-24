using Thiebheloye.iitii.WebApi;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Startup))]
namespace Thiebheloye.iitii.WebApi
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            AutofacConfig.Configure(app);
            ConfigureAuth(app);
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
        }
    }
}
