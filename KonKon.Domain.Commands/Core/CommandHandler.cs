using System.Net.Http;
using System.Threading.Tasks;
using KonKon.Domain.Core.Interfaces;
using KonKon.Domain.Core.Interfaces.Commands;

namespace KonKon.Domain.Commands.Core
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
                // request.AddDiagnosticsInformation(resultsWithExceptions);
                //foreach (var detail in resultsWithExceptions)
                //{
                //    ErrorSignal.FromCurrentContext().Raise(detail.Exception); //TODO: abstract elmah to be able to unit test the commandhandler
                //}
            }
            return result;
        }
    }
}