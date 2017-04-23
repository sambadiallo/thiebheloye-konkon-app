using System.Threading.Tasks;
using Thiebheloye.Identity.Domain.Commands.Core;
using Thiebheloye.Domain.Core.Interfaces.Adapters;
using Thiebheloye.Domain.Core.Interfaces.Adapters;
using Thiebheloye.Domain.Core.Models;
using Thiebheloye.Domain.Core.Models.Identity;
using Thiebheloye.Identity.Domain.Commands.Core;

namespace Thiebheloye.Identity.Domain.Commands.User
{
    public class LoginCommand : BaseCommand<LoginCommandArguments, CommandResult>
    {
        private readonly IAccountAdapter _loginAdapter;

        public LoginCommand(IAccountAdapter loginAdapter)
        {
            _loginAdapter = loginAdapter;
        }
        protected override async Task<CommandResult> Proceed(LoginCommandArguments argument)
        {
            return await _loginAdapter.Login(argument);
        }


    }
}
