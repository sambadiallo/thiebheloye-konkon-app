using System.Threading.Tasks;
using System.Web.Http;
using Thiebheloye.Domain.Core.Interfaces.Commands;
using Thiebheloye.Domain.Core.Models;
using Thiebheloye.Domain.Core.Models.Identity;

namespace Thiebheloye.iitii.WebApi.Controllers
{

    [RoutePrefix("api/User")]
    public class UserController : BaseController
    {
        private readonly ICommandHandler<RegisterCommandArguments, CommandResult> _registerCommandHandler;
        private readonly ICommandHandler<LoginCommandArguments, CommandResult> _loginCommandHandler;

        public UserController(ICommandHandler<RegisterCommandArguments, CommandResult> registerCommandHandler, ICommandHandler<LoginCommandArguments, CommandResult> loginCommandHandler)
        {
            _registerCommandHandler = registerCommandHandler;
            _loginCommandHandler = loginCommandHandler;
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

        [AllowAnonymous]
        [Route("Login")]
        public async Task<IHttpActionResult> Login(LoginCommandArguments model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _loginCommandHandler.Handle(Request, model);

            if (!result.Success)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }
    }
}
