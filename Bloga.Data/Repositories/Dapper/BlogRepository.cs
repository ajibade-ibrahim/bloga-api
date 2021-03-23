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
    public class BlogRepository : IBlogRepository
    {
        private const string DefaultConnection = "DefaultConnection";

        public BlogRepository(IConfiguration config)
        {
            _config = config;
        }

        private readonly IConfiguration _config;

        public async Task<int> DeleteAsync(int blogId)
        {
            using (var connection = new SqlConnection(_config.GetConnectionString(DefaultConnection)))
            {
                await connection.OpenAsync();

                return await connection.ExecuteAsync(
                    "Blog_Delete",
                    new
                    {
                        BlogId = blogId
                    },
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<PagedResults<Blog>> GetAllAsync(BlogPaging blogPaging)
        {
            var results = new PagedResults<Blog>();

            using (var connection = new SqlConnection(_config.GetConnectionString(DefaultConnection)))
            {
                await connection.OpenAsync();

                using (var gridReader = await connection.QueryMultipleAsync(
                    "Blog_GetAll",
                    new
                    {
                        Offset = (blogPaging.Page - 1) * blogPaging.PageSize,
                        blogPaging.PageSize
                    },
                    commandType: CommandType.StoredProcedure))
                {
                    results.Items = gridReader.Read<Blog>();
                    results.TotalCount = gridReader.ReadFirst<int>();
                }
            }

            return results;
        }

        public async Task<List<Blog>> GetAllByUserIdAsync(int applicationUserId)
        {
            using (var connection = new SqlConnection(_config.GetConnectionString(DefaultConnection)))
            {
                await connection.OpenAsync();

                var blogs = await connection.QueryAsync<Blog>(
                    "Blog_GetByUserId",
                    new
                    {
                        ApplicationUserId = applicationUserId
                    },
                    commandType: CommandType.StoredProcedure);

                return blogs.ToList();
            }
        }

        public async Task<List<Blog>> GetAllFamousAsync()
        {
            IEnumerable<Blog> famousBlogs;

            using (var connection = new SqlConnection(_config.GetConnectionString(DefaultConnection)))
            {
                await connection.OpenAsync();

                famousBlogs = await connection.QueryAsync<Blog>(
                    "Blog_GetAllFamous",
                    new
                    {
                    },
                    commandType: CommandType.StoredProcedure);
            }

            return famousBlogs.ToList();
        }

        public async Task<Blog> GetAsync(int blogId)
        {
            using (var connection = new SqlConnection(_config.GetConnectionString(DefaultConnection)))
            {
                await connection.OpenAsync();

                return await connection.QueryFirstOrDefaultAsync<Blog>(
                    "Blog_Get",
                    new
                    {
                        BlogId = blogId
                    },
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<Blog> UpsertAsync(BlogUpsert blogCreate, int applicationUserId)
        {
            int? newBlogId;

            using (var dataTable = new DataTable())
            {
                dataTable.Columns.Add("BlogId", typeof(int));
                dataTable.Columns.Add("Title", typeof(string));
                dataTable.Columns.Add("Content", typeof(string));
                dataTable.Columns.Add("PhotoId", typeof(int));

                dataTable.Rows.Add(blogCreate.BlogId, blogCreate.Title, blogCreate.Content, blogCreate.PhotoId);

                using (var connection = new SqlConnection(_config.GetConnectionString(DefaultConnection)))
                {
                    await connection.OpenAsync();

                    newBlogId = await connection.ExecuteScalarAsync<int?>(
                        "Blog_Upsert",
                        new
                        {
                            Blog = dataTable.AsTableValuedParameter("dbo.BlogType"),
                            ApplicationUserId = applicationUserId
                        },
                        commandType: CommandType.StoredProcedure);
                }
            }

            newBlogId = newBlogId ?? blogCreate.BlogId;

            return await GetAsync(newBlogId.Value);
        }
    }
}