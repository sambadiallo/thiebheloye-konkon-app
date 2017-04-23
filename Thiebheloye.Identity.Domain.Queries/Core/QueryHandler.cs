using System.Net.Http;
using System.Threading.Tasks;
using Thiebheloye.Domain.Core.Interfaces;
using Thiebheloye.Domain.Core.Interfaces.Queries;

namespace Thiebheloye.Identity.Domain.Queries.Core
{
    public class QueryHandler<TQueryArgs, TResult> : IQueryHandler<TQueryArgs, TResult>
       where TQueryArgs : IQueryArgument
       where TResult : IResult, new()
    {
        private readonly IQuery<TQueryArgs, TResult> _query;

        public QueryHandler(IQuery<TQueryArgs, TResult> query)
        {
            _query = query;
        }

        public async Task<TResult> Handle(HttpRequestMessage request, TQueryArgs argument)
        {
            var result = await _query.GetAsync(argument);
            if (!result.Success)
            {
                var resultsWithExceptions = result.FailureDetails.FindAll(o => o.Exception != null);
            }
            return result;
        }
    }
}