using System.Reflection;
using Autofac;
using Thiebheloye.Identity.Domain.Commands.Core;
using Thiebheloye.Domain.Core.Interfaces.Commands;
using Module = Autofac.Module;

namespace Thiebheloye.Identity.Domain.Commands.Config
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var commandAssembly = Assembly.Load("Thiebheloye.Identity.Domain.Commands");
            builder.RegisterAssemblyTypes(commandAssembly).AsClosedTypesOf(typeof(ICommand<,>)).AsImplementedInterfaces();
            builder.RegisterGeneric(typeof(CommandHandler<,>)).As(typeof(ICommandHandler<,>));
        }
    }
}
