using System;
using System.Threading.Tasks;
using Thiebheloye.Domain.Core.Interfaces;
using Thiebheloye.Domain.Core.Interfaces;
using Thiebheloye.Domain.Core.Interfaces.Commands;

namespace Thiebheloye.Identity.Domain.Commands.Core
{
    public abstract class BaseCommand<TCommandArgs, TResult> : ICommand<TCommandArgs, TResult>
        where TCommandArgs : ICommandArgument
        where TResult : IResult, new()
    {
        protected readonly TResult Result;

        protected BaseCommand()
        {
            Result = new TResult();
        }

        public async Task<TResult> Execute(TCommandArgs argument)
        {
            if (await Validate(argument))
            {
                return await Proceed(argument);
            }
            return await Task.FromResult<TResult>(Result);
        }

        protected virtual Task<bool> Validate(TCommandArgs argument)
        {
            return Task.FromResult(true);
        }

        protected virtual Task<TResult> Proceed(TCommandArgs argument)
        {
            throw new NotImplementedException();
        }

    }
}