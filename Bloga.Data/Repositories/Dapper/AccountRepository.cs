using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Bloga.Data.Repositories.Interfaces;
using Bloga.Models.Account;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Bloga.Data.Repositories.Dapper
{
    public class AccountRepository : IAccountRepository
    {
        private const string DefaultConnection = "DefaultConnection";

        public AccountRepository(IConfiguration config)
        {
            _config = config;
        }

        private readonly IConfiguration _config;

        public async Task<IdentityResult> CreateAsync(ApplicationUserIdentity user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var dataTable = new DataTable())
            {
                dataTable.Columns.Add("Username", typeof(string));
                dataTable.Columns.Add("NormalizedUsername", typeof(string));
                dataTable.Columns.Add("Email", typeof(string));
                dataTable.Columns.Add("NormalizedEmail", typeof(string));
                dataTable.Columns.Add("Fullname", typeof(string));
                dataTable.Columns.Add("PasswordHash", typeof(string));

                dataTable.Rows.Add(
                    user.Username,
                    user.NormalizedUsername,
                    user.Email,
                    user.NormalizedEmail,
                    user.Fullname,
                    user.PasswordHash);

                using (var connection = new SqlConnection(_config.GetConnectionString(DefaultConnection)))
                {
                    await connection.OpenAsync(cancellationToken);
                    await connection.ExecuteAsync(
                        "Account_Insert",
                        new
                        {
                            Account = dataTable.AsTableValuedParameter("dbo.AccountType")
                        },
                        commandType: CommandType.StoredProcedure);
                }
            }

            return IdentityResult.Success;
        }

        public async Task<ApplicationUserIdentity> GetByUsernameAsync(
            string normalizedUsername,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_config.GetConnectionString(DefaultConnection)))
            {
                await connection.OpenAsync(cancellationToken);

                return await connection.QuerySingleOrDefaultAsync<ApplicationUserIdentity>(
                    "Account_GetByUsername",
                    new
                    {
                        NormalizedUsername = normalizedUsername
                    },
                    commandType: CommandType.StoredProcedure);
            }
        }
    }
}