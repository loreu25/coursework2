using System.Collections.Generic;

namespace MusicCatalog.Models
{
    public class Artist
    {
        public int ArtistId { get; set; }
        public string Name { get; set; }
        public string? Bio { get; set; } // Nullable

        public ICollection<Music> Musics { get; set; } = new List<Music>();
    }
}
