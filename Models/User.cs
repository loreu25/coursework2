using System.Collections.Generic;

namespace MusicCatalog.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string Login { get; set; }
        public string PasswordHash { get; set; }

        public ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();
    }
}
