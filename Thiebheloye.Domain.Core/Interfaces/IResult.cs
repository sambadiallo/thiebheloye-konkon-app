﻿using System.Collections.Generic;

namespace Thiebheloye.Domain.Core.Interfaces
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
