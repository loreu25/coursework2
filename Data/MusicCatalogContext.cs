using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MusicCatalog.Models;

namespace MusicCatalog.Data
{
    public class MusicCatalogContext : IdentityDbContext<ApplicationUser>
    {
        public MusicCatalogContext(DbContextOptions<MusicCatalogContext> options) : base(options)
        {
        }

        public DbSet<Artist> Artists { get; set; }
        public DbSet<Composer> Composers { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Label> Labels { get; set; }
        public DbSet<MediaType> MediaTypes { get; set; }
        public DbSet<Music> Musics { get; set; }
        public DbSet<Playlist> Playlists { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the many-to-many relationship between Playlist and Music
            modelBuilder.Entity<Playlist>()
                .HasMany(p => p.Musics)
                .WithMany(m => m.Playlists)
                .UsingEntity(j => j.ToTable("PlaylistTracks"));

            // Configure the relationship between ApplicationUser and Playlist
            modelBuilder.Entity<Playlist>()
                .HasOne(p => p.User)
                .WithMany(u => u.Playlists)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
