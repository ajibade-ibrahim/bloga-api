using System.Collections.Generic;
using System.Threading.Tasks;
using Bloga.Models;

namespace Bloga.Data.Repositories.Interfaces
{
    public interface IBlogCommentRepository
    {
        Task<int> DeleteAsync(int blogCommentId);

        Task<List<BlogComment>> GetAllAsync(int blogId);

        Task<BlogComment> GetAsync(int blogCommentId);
        Task<BlogComment> UpsertAsync(BlogCommentUpsert blogCommentUpsert, int applicationUserId);
    }
}