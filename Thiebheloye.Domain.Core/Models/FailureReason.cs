﻿namespace Thiebheloye.Domain.Core.Models
{
    public enum FailureReason
    {
        Technical,
        Business,
        Unhandled,
        Security,
        Authorization,
        SignInFailure
    }
}