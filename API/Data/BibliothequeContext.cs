using Microsoft.EntityFrameworkCore;
using API.Models;

namespace API.Data
{
    public class BibliothequeContext : DbContext
    {
        public BibliothequeContext(DbContextOptions<BibliothequeContext> options)
            : base(options)
        {
        }

        public DbSet<Media> Medias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Media>()
                .HasDiscriminator<string>("Discriminator")
                .HasValue<Ebook>("ebook")
                .HasValue<PaperBook>("paperbook");

            base.OnModelCreating(modelBuilder);
        }
    }
}
