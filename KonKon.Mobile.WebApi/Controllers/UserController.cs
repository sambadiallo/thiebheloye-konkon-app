using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using KonKon.Data.EF.Models.Identity;
using KonKon.Domain.Core.Interfaces.Adapters;
using KonKon.Domain.Core.Interfaces.Commands;
using KonKon.Domain.Core.Models;
using KonKon.Domain.Core.Models.Identity;
using Microsoft.AspNet.Identity;

namespace KonKon.Mobile.WebApi.Controllers
{

    [RoutePrefix("api/User")]
    public class UserController : BaseController
    {
        private readonly ICommandHandler<RegisterCommandArguments, CommandResult> _registerCommandHandler;
        public UserController(ICommandHandler<RegisterCommandArguments, CommandResult> registerCommandHandler)
        {
            _registerCommandHandler = registerCommandHandler;
        }

        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterCommandArguments model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _registerCommandHandler.Handle(Request, model);

            if (!result.Success)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }
    }
}
