using System.Threading.Tasks;
using Thiebheloye.Identity.Domain.Commands.Core;
using Thiebheloye.Domain.Core.Interfaces.Adapters;
using Thiebheloye.Domain.Core.Interfaces.Adapters;
using Thiebheloye.Domain.Core.Models;
using Thiebheloye.Domain.Core.Models.Identity;
using Thiebheloye.Identity.Domain.Commands.Core;

namespace Thiebheloye.Identity.Domain.Commands.User
{
    public class RegisterCommand : BaseCommand<RegisterCommandArguments, CommandResult>
    {
        private readonly IAccountAdapter _accountAdapter;

        public RegisterCommand(IAccountAdapter accountAdapter)
        {
            _accountAdapter = accountAdapter;
        }

        protected override async Task<CommandResult> Proceed(RegisterCommandArguments argument)
        {
            return await _accountAdapter.Register(argument);
        }
    }
}
