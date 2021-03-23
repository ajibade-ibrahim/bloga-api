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
    public class PhotoRepository : IPhotoRepository
    {
        private const string DefaultConnection = "DefaultConnection";

        public PhotoRepository(IConfiguration config)
        {
            _config = config;
        }

        private readonly IConfiguration _config;

        public async Task<int> DeleteAsync(int photoId)
        {
            using (var connection = new SqlConnection(_config.GetConnectionString(DefaultConnection)))
            {
                await connection.OpenAsync();

                return await connection.ExecuteAsync(
                    "Photo_Delete",
                    new
                    {
                        PhotoId = photoId
                    },
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<List<Photo>> GetAllByUserIdAsync(int applicationUserId)
        {
            IEnumerable<Photo> photos;

            using (var connection = new SqlConnection(_config.GetConnectionString(DefaultConnection)))
            {
                await connection.OpenAsync();

                photos = await connection.QueryAsync<Photo>(
                    "Photo_GetByUserId",
                    new
                    {
                        ApplicationUserId = applicationUserId
                    },
                    commandType: CommandType.StoredProcedure);
            }

            return photos.ToList();
        }

        public async Task<Photo> GetAsync(int photoId)
        {
            using (var connection = new SqlConnection(_config.GetConnectionString(DefaultConnection)))
            {
                await connection.OpenAsync();

                return await connection.QueryFirstOrDefaultAsync<Photo>(
                    "Photo_Get",
                    new
                    {
                        PhotoId = photoId
                    },
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<Photo> InsertAsync(PhotoCreate photoCreate, int applicationUserId)
        {
            using (var dataTable = new DataTable())
            {
                dataTable.Columns.Add("PublicId", typeof(string));
                dataTable.Columns.Add("ImageUrl", typeof(string));
                dataTable.Columns.Add("Description", typeof(string));

                dataTable.Rows.Add(photoCreate.PublicId, photoCreate.ImageUrl, photoCreate.Description);

                using (var connection = new SqlConnection(_config.GetConnectionString(DefaultConnection)))
                {
                    await connection.OpenAsync();

                    var newPhotoId = await connection.ExecuteScalarAsync<int>(
                        "Photo_Insert",
                        new
                        {
                            Photo = dataTable.AsTableValuedParameter("dbo.PhotoType"),
                            ApplicationUserId = applicationUserId
                        },
                        commandType: CommandType.StoredProcedure);

                    return await GetAsync(newPhotoId);
                }
            }
        }
    }
}