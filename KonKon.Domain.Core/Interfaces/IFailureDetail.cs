using System;
using KonKon.Domain.Core.Models;

namespace KonKon.Domain.Core.Interfaces
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