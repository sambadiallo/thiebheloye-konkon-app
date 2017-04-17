using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using KonKon.Data.EF.Models.Identity;
using KonKon.Domain.Core.Interfaces.Adapters;
using KonKon.Domain.Core.Models.Identity;
using Microsoft.AspNet.Identity;

namespace KonKon.Mobile.WebApi.Controllers
{

    [RoutePrefix("api/User")]
    public class UserController : BaseController
    {
        private readonly IAccountAdapter _accountAdapter;
        public UserController(IAccountAdapter accountAdapter)
        {
            _accountAdapter = accountAdapter;
        }

        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _accountAdapter.Register(model);

            if (!result.Success)
            {
                //return GetErrorResult(result);
            }

            return Ok();
        }
    }
}
