using System.Threading.Tasks;
using Thiebheloye.Domain.Core.Models;
using Thiebheloye.Domain.Core.Models.Identity;

namespace Thiebheloye.Domain.Core.Interfaces.Adapters
{
    public interface IAccountAdapter
    {
        Task<CommandResult> Register(RegisterCommandArguments model);
        Task<CommandResult> Login(LoginCommandArguments argument);
    }
}
