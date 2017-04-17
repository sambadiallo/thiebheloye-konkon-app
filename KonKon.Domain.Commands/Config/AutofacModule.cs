using System.Reflection;
using Autofac;
using KonKon.Domain.Commands.Core;
using KonKon.Domain.Core.Interfaces.Commands;
using Module = Autofac.Module;

namespace KonKon.Domain.Commands.Config
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var commandAssembly = Assembly.Load("KonKon.Domain.Commands");
            builder.RegisterAssemblyTypes(commandAssembly).AsClosedTypesOf(typeof(ICommand<,>)).AsImplementedInterfaces();
            builder.RegisterGeneric(typeof(CommandHandler<,>)).As(typeof(ICommandHandler<,>));
        }
    }
}
