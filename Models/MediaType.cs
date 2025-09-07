using System.Collections.Generic;

namespace MusicCatalog.Models
{
    public class MediaType
    {
        public int MediaTypeId { get; set; }
        public string Name { get; set; } // e.g., CD, Cassette, Vinyl

        public ICollection<Music> Musics { get; set; } = new List<Music>();
    }
}
