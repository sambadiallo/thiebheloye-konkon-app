using System;
using System.ComponentModel;
using Thiebheloye.Domain.Core.Extensions;
using Thiebheloye.Domain.Core.Interfaces;

namespace Thiebheloye.Domain.Core.Models
{
    public class FailureDetail : IFailureDetail
    {
        public FailureDetail() { }

        public FailureDetail(FailureReason reason, StatusCode statusCode, string information = "")
        {
            Reason = reason;
            StatusCode = statusCode;
            Information = information;
        }

        public FailureReason Reason { get; set; }

        public StatusCode StatusCode { get; set; }

        public string Information { get; set; }
        public string StatusCodeDescription => StatusCode.GetAttributeOfType<DescriptionAttribute>().Description; public Exception Exception { get; set; }

        public static FailureDetail Create(string information, FailureReason reason, StatusCode status)
        {
            return new FailureDetail(reason, status, information);
        }

        public static FailureDetail Create(string information, FailureReason reason, StatusCode status, Exception exception)
        {
            var failure = new FailureDetail(reason, status, information)
            {
                Exception = exception
            };
            return failure;
        }

    }
}