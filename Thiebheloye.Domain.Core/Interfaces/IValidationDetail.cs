using System.Collections.Generic;

namespace Thiebheloye.Domain.Core.Interfaces
{
    public interface IValidationDetail : IFailureDetail
    {
        IEnumerable<string> MemberNames { get; }
    }
}
