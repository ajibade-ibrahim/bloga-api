using System.Threading;
using System.Threading.Tasks;
using Bloga.Models.Account;
using Microsoft.AspNetCore.Identity;

namespace Bloga.Data.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        Task<IdentityResult> CreateAsync(ApplicationUserIdentity user, CancellationToken cancellationToken);

        Task<ApplicationUserIdentity> GetByUsernameAsync(
            string normalizedUsername,
            CancellationToken cancellationToken);
    }
}