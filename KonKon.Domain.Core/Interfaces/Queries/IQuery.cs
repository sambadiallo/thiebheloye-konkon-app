using System.Threading.Tasks;

namespace KonKon.Domain.Core.Interfaces.Queries
{
    public interface IQuery<in TQueryArgs, TResult>
        where TQueryArgs : IQueryArgument
    {
        Task<TResult> GetAsync(TQueryArgs argument);
    }
}
