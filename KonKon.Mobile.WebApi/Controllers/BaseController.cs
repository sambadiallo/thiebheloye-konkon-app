using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using KonKon.Domain.Core.Interfaces;
using Microsoft.AspNet.Identity;

namespace KonKon.Mobile.WebApi.Controllers
{
    [Authorize]
    public class BaseController : ApiController
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
