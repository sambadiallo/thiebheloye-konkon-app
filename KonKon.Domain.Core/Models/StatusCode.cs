using System.ComponentModel;

namespace KonKon.Domain.Core.Models
{
    public enum StatusCode
    {
        [Description("An external service call failed")]
        ExternalServiceFailure = 1001,
        [Description("The contract is not eligible or something along that line...")]
        NotEligible = 1002,
        [Description("The supplied information is invalid")]
        ValidationError = 1003,
        [Description("The requested information could not be found.")]
        NotFound = 1004,
        [Description("The operation or request timed out.")]
        Timeout = 1005,
        [Description("Not Available.")]
        NotImplemented = 1006,
        [Description("Update Failed.")]
        UpdateFailed = 1007,
        [Description("A similar command is already pending.")]
        AlreadyPending = 1008,
        [Description("Sorry, general API error.")]
        GeneralError = 1009,
        [Description("Sorry, no result error.")]
        NullQueryResult = 1010,
        UserCreating = 1011,
    }


}
