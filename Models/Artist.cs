using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MusicCatalog.Models
{
    public class Artist
    {
        public int ArtistId { get; set; }
        
        [Required(ErrorMessage = "Название обязательно")]
        [Display(Name = "Название")]
        public string Name { get; set; }
        
        [Display(Name = "Биография")]
        public string? Bio { get; set; } // Nullable

        public ICollection<Music> Musics { get; set; } = new List<Music>();
    }
}
