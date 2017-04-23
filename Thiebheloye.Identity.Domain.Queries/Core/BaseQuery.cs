using System;
using System.Threading.Tasks;
using Thiebheloye.Domain.Core.Interfaces;
using Thiebheloye.Domain.Core.Interfaces.Queries;

namespace Thiebheloye.Identity.Domain.Queries.Core
{
    public abstract class BaseQuery<TQueryArgs, TResult> : IQuery<TQueryArgs, TResult>
        where TQueryArgs : IQueryArgument
        where TResult : IResult, new()
    {
        protected readonly TResult Result;

        protected BaseQuery()
        {
            Result = new TResult();
        }

        public async Task<TResult> GetAsync(TQueryArgs argument)
        {
            if (await Validate(argument))
            {
                await Proceed(argument);
            }
            return await Task.FromResult<TResult>(Result);
        }

        protected virtual Task<bool> Validate(TQueryArgs argument)
        {
            return Task.FromResult(true);
        }

        protected virtual Task<TResult> Proceed(TQueryArgs argument)
        {
            throw new NotImplementedException();
        }


    }
}