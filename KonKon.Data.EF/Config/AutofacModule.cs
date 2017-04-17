using System.Reflection;
using Autofac;
using Module = Autofac.Module;

namespace KonKon.Data.EF.Config
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var assembly = Assembly.Load("KonKon.Data.EF");

            builder.RegisterAssemblyTypes(assembly)
                .Where(t => t.Name.EndsWith("Adapter"))
                .AsImplementedInterfaces();    
        }
    }
}
