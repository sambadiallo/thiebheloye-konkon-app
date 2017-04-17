using KonKon.Domain.Core.Interfaces;
using System.Collections.Generic;

namespace KonKon.Domain.Core.Interfaces
{
    public interface IResult
    {
        bool Success { get; }
        List<IFailureDetail> FailureDetails { get; set; }
    }

    public interface IResult<T> : IResult
    {
        T Response { get; set; }
    }

    public interface ICachableResult<T> : IResult<T>
    {
        bool Cached { get; }
    }
}
