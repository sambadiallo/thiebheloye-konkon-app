using System.Reflection;
using Autofac;
using Module = Autofac.Module;

namespace Thiebheloye.Identity.Data.EF.Config
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var assembly = Assembly.Load("Thiebheloye.Identity.Data.EF");

            builder.RegisterAssemblyTypes(assembly)
                .Where(t => t.Name.EndsWith("Adapter"))
                .AsImplementedInterfaces();    
        }
    }
}
