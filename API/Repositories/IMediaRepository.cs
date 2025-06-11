using API.Models;

namespace API.Repositories
{
    public interface IMediaRepository
    {
        Task<IEnumerable<Media>> GetAllAsync(string? author = null, string? title = null);
        Task<Media?> GetByIdAsync(int id);
        Task AddAsync(Media media);
        Task UpdateAsync(Media media);
        Task DeleteAsync(int id);
    }
}
