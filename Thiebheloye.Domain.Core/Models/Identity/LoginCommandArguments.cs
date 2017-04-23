﻿using System.ComponentModel.DataAnnotations;
using Thiebheloye.Domain.Core.Interfaces.Commands;

namespace Thiebheloye.Domain.Core.Models.Identity
{
    public class LoginCommandArguments : ICommandArgument
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

    }
}