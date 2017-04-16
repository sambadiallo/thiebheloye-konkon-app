using System.Threading.Tasks;

namespace KonKon.Domain.Core.Interfaces.Commands
{
    public interface ICommand<in TCommandArgs, TResult>
        where TCommandArgs : ICommandArgument
    {
        Task<TResult> Execute(TCommandArgs argument);
    }
}
