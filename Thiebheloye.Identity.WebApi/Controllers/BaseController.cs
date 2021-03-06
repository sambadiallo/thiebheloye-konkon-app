﻿using System.Web.Http;
using Thiebheloye.Domain.Core.Interfaces;

namespace Thiebheloye.Identity.WebApi.Controllers
{
    [Authorize]
    public class BaseIdentityController : ApiController
    {
        protected IHttpActionResult GetErrorResult(IResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Success)
            {
                if (result.FailureDetails != null)
                {
                    foreach (var error in result.FailureDetails)
                    {
                        ModelState.AddModelError(error.StatusCode.ToString(), error.Information);
                    }
                }

                if (ModelState.IsValid)
                {
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }
    }
}
