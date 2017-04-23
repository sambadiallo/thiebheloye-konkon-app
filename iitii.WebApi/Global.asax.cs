﻿using System.Web.Http;

namespace iitii.WebApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(Thiebheloye.Identity.WebApi.WebApiConfig.Register); //Register identity
            GlobalConfiguration.Configure(WebApiConfig.Register);//Register iitii webapi
        }
    }
}
