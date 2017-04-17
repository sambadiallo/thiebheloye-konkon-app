using System.Net.Http;
using System.Threading.Tasks;
using KonKon.Domain.Core.Interfaces;
using KonKon.Domain.Core.Interfaces.Queries;

namespace KonKon.Domain.Queries.Core
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