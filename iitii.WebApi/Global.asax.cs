using System.Web.Http;

namespace iitii.WebApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(Thiebheloye.Identity.WebApi.WebApiConfig.Register);
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
