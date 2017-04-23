using System;
using System.Collections.Generic;
using System.Linq;
using Thiebheloye.Domain.Core.Interfaces;

namespace Thiebheloye.Domain.Core.Models
{
    public class QueryResult : IResult
    {
        public bool Success => !FailureDetails.Any();
        public List<IFailureDetail> FailureDetails { get; set; }
        public QueryResult()
        {
            FailureDetails = new List<IFailureDetail>();
        }

        public void AddFailure(FailureReason reason, StatusCode statusCode, string information = "", Exception exception = null)
        {
            var failure = new FailureDetail(reason, statusCode, information) { Exception = exception };
            FailureDetails.Add(failure);
        }
    }

    public class QueryResult<T> : IResult<T>
    {
        public bool Success => !FailureDetails.Any();

        public List<IFailureDetail> FailureDetails { get; set; }
        public T Response { get; set; }
        public QueryResult()
        {
            FailureDetails = new List<IFailureDetail>();
        }

        public void AddFailure(FailureReason reason, StatusCode statusCode, string information = "", Exception exception = null)
        {
            var failure = new FailureDetail(reason, statusCode, information)
            {
                Exception = exception
            };
            FailureDetails.Add(failure);
        }
    }
}