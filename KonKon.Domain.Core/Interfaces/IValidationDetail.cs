using System.Collections.Generic;

namespace KonKon.Domain.Core.Interfaces
{
    public interface IValidationDetail : IFailureDetail
    {
        IEnumerable<string> MemberNames { get; }
    }
}
