using System.Net.Http;
using System.Threading.Tasks;

namespace Thiebheloye.Domain.Core.Interfaces.Queries
{
    public interface IQueryHandler<in TQueryArgs, TResult>
     where TQueryArgs : IQueryArgument
     where TResult : IResult, new()
    {
        Task<TResult> Handle(HttpRequestMessage request, TQueryArgs argument);
    }
}
