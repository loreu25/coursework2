using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MusicCatalog.Models
{
    public class Playlist
    {
        public int PlaylistId { get; set; }
        
        [Display(Name = "Название плейлиста")]
        [Required(ErrorMessage = "Название плейлиста обязательно")]
        [StringLength(100, ErrorMessage = "Название не может быть длиннее 100 символов")]
        public string Name { get; set; } = string.Empty;
        
        [Display(Name = "Дата создания")]
        public DateTime CreationDate { get; set; }

        public string UserId { get; set; } = string.Empty;

        public ApplicationUser User { get; set; } = null!;
        public ICollection<Music> Musics { get; set; } = new List<Music>();
    }
}
