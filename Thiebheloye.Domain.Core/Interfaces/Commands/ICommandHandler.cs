using System.Net.Http;
using System.Threading.Tasks;

namespace Thiebheloye.Domain.Core.Interfaces.Commands
{
    public interface ICommandHandler<in TCommandArgs, TResult>
     where TCommandArgs : ICommandArgument
     where TResult : IResult, new()
    {
        Task<TResult> Handle(HttpRequestMessage request, TCommandArgs argument);
    }
}
