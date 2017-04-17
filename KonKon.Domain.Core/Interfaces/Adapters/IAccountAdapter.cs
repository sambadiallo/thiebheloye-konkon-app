using System.Threading.Tasks;
using KonKon.Domain.Core.Models;
using KonKon.Domain.Core.Models.Identity;

namespace KonKon.Domain.Core.Interfaces.Adapters
{
    public interface IAccountAdapter
    {
        Task<CommandResult> Register(RegisterCommandArguments model);
    }
}
