using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KonKon.Domain.Core.Interfaces.Adapters;
using KonKon.Domain.Core.Interfaces.Commands;
using KonKon.Domain.Core.Models;
using KonKon.Domain.Core.Models.Identity;

namespace KonKon.Domain.Commands.User
{
    public class RegisterCommand : ICommand<RegisterCommandArguments, CommandResult>
    {
        private readonly IAccountAdapter _accountAdapter;

        public RegisterCommand(IAccountAdapter accountAdapter)
        {
            _accountAdapter = accountAdapter;
        }

        public async Task<CommandResult> Execute(RegisterCommandArguments argument)
        {
            return await _accountAdapter.Register(argument);
        }
    }
}
