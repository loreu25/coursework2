using System;
using System.Collections.Generic;

namespace MusicCatalog.Models
{
    public class Playlist
    {
        public int PlaylistId { get; set; }
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }

        // Foreign Key
        public int UserId { get; set; }

        // Navigation properties
        public User User { get; set; }
        public ICollection<Music> Musics { get; set; } = new List<Music>();
    }
}
