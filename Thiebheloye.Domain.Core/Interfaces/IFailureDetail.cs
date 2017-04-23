using System;
using Thiebheloye.Domain.Core.Models;

namespace Thiebheloye.Domain.Core.Interfaces
{
    public interface IFailureDetail
    {
        FailureReason Reason { get; set; }
        StatusCode StatusCode { get; set; }
        string Information { get; set; }
        string StatusCodeDescription { get; }
        Exception Exception { get; set; }
    }
}