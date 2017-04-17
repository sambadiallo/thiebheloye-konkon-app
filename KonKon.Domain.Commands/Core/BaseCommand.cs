using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KonKon.Domain.Core.Interfaces;
using KonKon.Domain.Core.Interfaces.Commands;
using KonKon.Domain.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace KonKon.Domain.Commands.Core
{
    public abstract class BaseCommand<TCommandArgs, TResult> : ICommand<TCommandArgs, TResult>
        where TCommandArgs : ICommandArgument
        where TResult : IResult, new()
    {
        protected readonly TResult Result;

        protected BaseCommand()
        {
            Result = new TResult();
        }

        public async Task<TResult> Execute(TCommandArgs argument)
        {
            if (await Validate(argument))
            {
                await DoExecute(argument);
            }
            return await Task.FromResult<TResult>(Result);
        }

        protected bool IsValidCommand(TCommandArgs argument)
        {
            if (argument == null)
            {
                Result.FailureDetails.Add(new ValidationDetail(FailureReason.Technical, "{required: {}})", new[] { "commandArgs" }));
                return false;
            }
            var results = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(
                argument,
                new ValidationContext(argument, null, null),
                results,
                true);
            // then we simply map the results to out FailureDetails

            foreach (var validationResult in results)
            {
                Result.FailureDetails.Add(new ValidationDetail(FailureReason.Technical,
                                                               validationResult.ErrorMessage,
                                                               validationResult.MemberNames));
            }
            return isValid;
        }

        /// <summary>
        /// Check the necessary business rules and update the FailureDetails in case of violations.
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        protected virtual Task<bool> Validate(TCommandArgs argument)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Call the necessary ESB services and update this.Result.FailureDetails in case of errors.
        /// </summary>
        /// <param name="argument"></param>
        protected virtual Task DoExecute(TCommandArgs argument)
        {
            throw new NotImplementedException();
        }

    }
}