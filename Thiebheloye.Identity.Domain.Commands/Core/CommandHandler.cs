using System.Net.Http;
using System.Threading.Tasks;
using Thiebheloye.Domain.Core.Interfaces;
using Thiebheloye.Domain.Core.Interfaces;
using Thiebheloye.Domain.Core.Interfaces.Commands;

namespace Thiebheloye.Identity.Domain.Commands.Core
{
    public class CommandHandler<TCommandArgs, TResult> : ICommandHandler<TCommandArgs, TResult>
       where TCommandArgs : ICommandArgument
       where TResult : IResult, new()
    {
        private readonly ICommand<TCommandArgs, TResult> _command;

        public CommandHandler(ICommand<TCommandArgs, TResult> command)
        {
            _command = command;
        }

        public async Task<TResult> Handle(HttpRequestMessage request, TCommandArgs argument)
        {
            var result = await _command.Execute(argument);
            if (!result.Success)
            {
                var resultsWithExceptions = result.FailureDetails.FindAll(o => o.Exception != null);
            }
            return result;
        }
    }
}