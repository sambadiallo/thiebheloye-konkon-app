using System.Collections.Generic;
using KonKon.Domain.Core.Interfaces;

namespace KonKon.Domain.Core.Models
{
    public class ValidationDetail : FailureDetail, IValidationDetail
    {
        public IEnumerable<string> MemberNames { get; }

        public ValidationDetail(FailureReason reason,
                                string information,
                                IEnumerable<string> memberNames)
            : base(reason, StatusCode.ValidationError, information)
        {
            MemberNames = memberNames;

        }

    }
}
