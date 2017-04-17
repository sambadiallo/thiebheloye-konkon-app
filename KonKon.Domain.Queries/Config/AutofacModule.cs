using System.Reflection;
using Autofac;
using Module = Autofac.Module;
using KonKon.Domain.Core.Interfaces.Queries;
using KonKon.Domain.Queries.Core;

namespace KonKon.Domain.Queries.Config
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var queryAssembly = Assembly.Load("KonKon.Domain.Queries");
            builder.RegisterAssemblyTypes(queryAssembly).AsClosedTypesOf(typeof(IQuery<,>)).AsImplementedInterfaces();
            builder.RegisterGeneric(typeof(QueryHandler<,>)).As(typeof(IQueryHandler<,>));
        }
    }
}
