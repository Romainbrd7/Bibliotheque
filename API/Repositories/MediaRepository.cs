using API.Models;
using API.Data;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class MediaRepository : IMediaRepository
    {
        private readonly BibliothequeContext _context;

        public MediaRepository(BibliothequeContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Media>> GetAllAsync(string? author = null, string? title = null)
        {
            IQueryable<Media> query = _context.Medias;

            if (!string.IsNullOrWhiteSpace(author))
            {
                var authorLower = author.ToLower();
                query = query.Where(m => m.Author != null && m.Author.ToLower().Contains(authorLower));
            }

            if (!string.IsNullOrWhiteSpace(title))
            {
                var titleLower = title.ToLower();
                query = query.Where(m => m.Title != null && m.Title.ToLower().Contains(titleLower));
            }

            return await query.ToListAsync();
        }

        public async Task<Media?> GetByIdAsync(int id)
        {
            return await _context.Medias.FindAsync(id);
        }

        public async Task AddAsync(Media media)
        {
            _context.Medias.Add(media);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Media media)
{
    var tracked = _context.ChangeTracker.Entries<Media>()
        .FirstOrDefault(e => e.Entity.Id == media.Id);

    if (tracked != null)
        tracked.State = EntityState.Detached;

    _context.Medias.Update(media);
    await _context.SaveChangesAsync();
}


        public async Task DeleteAsync(int id)
        {
            var media = await _context.Medias.FindAsync(id);
            if (media != null)
            {
                _context.Medias.Remove(media);
                await _context.SaveChangesAsync();
            }
        }
    }
}
