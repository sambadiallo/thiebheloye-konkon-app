using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using KonKon.Domain.Core.Interfaces;
using KonKon.Domain.Core.Interfaces.Queries;
using KonKon.Domain.Core.Models;

namespace KonKon.Domain.Queries.Core
{
    public abstract class BaseQuery<TQueryArgs, TResult> : IQuery<TQueryArgs, TResult>
        where TQueryArgs : IQueryArgument
        where TResult : IResult, new()
    {
        protected readonly TResult Result;

        protected BaseQuery()
        {
            Result = new TResult();
        }

        public async Task<TResult> GetAsync(TQueryArgs argument)
        {
            if (await Validate(argument))
            {
                await ProcessQuery(argument);
            }
            return await Task.FromResult<TResult>(Result);
        }

        protected bool IsValidCommand(TQueryArgs argument)
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
        protected virtual Task<bool> Validate(TQueryArgs argument)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Call the necessary ESB services and update this.Result.FailureDetails in case of errors.
        /// </summary>
        /// <param name="argument"></param>
        protected virtual Task ProcessQuery(TQueryArgs argument)
        {
            throw new NotImplementedException();
        }


    }
}