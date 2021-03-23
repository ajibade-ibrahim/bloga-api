using System.Collections.Generic;
using System.Threading.Tasks;
using Bloga.Models;

namespace Bloga.Data.Repositories.Interfaces
{
    public interface IPhotoRepository
    {
        Task<int> DeleteAsync(int photoId);

        Task<List<Photo>> GetAllByUserIdAsync(int applicationUserId);

        Task<Photo> GetAsync(int photoId);
        Task<Photo> InsertAsync(PhotoCreate photoCreate, int applicationUserId);
    }
}