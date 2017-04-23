using System.Reflection;
using Autofac;
using Thiebheloye.Domain.Core.Interfaces.Queries;
using Thiebheloye.Identity.Domain.Queries.Core;
using Module = Autofac.Module;

namespace Thiebheloye.Identity.Domain.Queries.Config
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var queryAssembly = Assembly.Load("Thiebheloye.Identity.Domain.Queries");
            builder.RegisterAssemblyTypes(queryAssembly).AsClosedTypesOf(typeof(IQuery<,>)).AsImplementedInterfaces();
            builder.RegisterGeneric(typeof(QueryHandler<,>)).As(typeof(IQueryHandler<,>));
        }
    }
}
