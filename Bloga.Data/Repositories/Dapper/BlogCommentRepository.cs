using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Bloga.Data.Repositories.Interfaces;
using Bloga.Models;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace Bloga.Data.Repositories.Dapper
{
    public class BlogCommentRepository : IBlogCommentRepository
    {
        private const string DefaultConnection = "DefaultConnection";

        public BlogCommentRepository(IConfiguration config)
        {
            _config = config;
        }

        private readonly IConfiguration _config;

        public async Task<int> DeleteAsync(int blogCommentId)
        {
            using (var connection = new SqlConnection(_config.GetConnectionString(DefaultConnection)))
            {
                await connection.OpenAsync();

                return await connection.ExecuteAsync(
                    "BlogComment_Delete",
                    new
                    {
                        BlogCommentId = blogCommentId
                    },
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<List<BlogComment>> GetAllAsync(int blogId)
        {
            using (var connection = new SqlConnection(_config.GetConnectionString(DefaultConnection)))
            {
                await connection.OpenAsync();

                var blogComments = await connection.QueryAsync<BlogComment>(
                    "BlogComment_GetAll",
                    new
                    {
                        BlogId = blogId
                    },
                    commandType: CommandType.StoredProcedure);

                return blogComments.ToList();
            }
        }

        public async Task<BlogComment> GetAsync(int blogCommentId)
        {
            using (var connection = new SqlConnection(_config.GetConnectionString(DefaultConnection)))
            {
                await connection.OpenAsync();

                return await connection.QueryFirstOrDefaultAsync<BlogComment>(
                    "BlogComment_Get",
                    new
                    {
                        BlogCommentId = blogCommentId
                    },
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<BlogComment> UpsertAsync(BlogCommentUpsert blogCommentUpsert, int applicationUserId)
        {
            int? newBlogCommentId;

            using (var dataTable = new DataTable())
            {
                dataTable.Columns.Add("BlogCommentId", typeof(int));
                dataTable.Columns.Add("ParentBlogCommentId", typeof(int));
                dataTable.Columns.Add("BlogId", typeof(int));
                dataTable.Columns.Add("Content", typeof(string));

                dataTable.Rows.Add(
                    blogCommentUpsert.BlogCommentId,
                    blogCommentUpsert.ParentBlogCommentId,
                    blogCommentUpsert.BlogId,
                    blogCommentUpsert.Content);

                using (var connection = new SqlConnection(_config.GetConnectionString(DefaultConnection)))
                {
                    await connection.OpenAsync();

                    newBlogCommentId = await connection.ExecuteScalarAsync<int?>(
                        "BlogComment_Upsert",
                        new
                        {
                            BlogComment = dataTable.AsTableValuedParameter("dbo.BlogCommentType"),
                            ApplicationUserId = applicationUserId
                        },
                        commandType: CommandType.StoredProcedure);
                }
            }

            newBlogCommentId = newBlogCommentId ?? blogCommentUpsert.BlogCommentId;

            return await GetAsync(newBlogCommentId.Value);
        }
    }
}