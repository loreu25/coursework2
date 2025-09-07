using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MusicCatalog.Models
{
    public class Music
    {
        public int MusicId { get; set; } // Inventory number
        
        [Required(ErrorMessage = "Название обязательно")]
        public string Title { get; set; }
        
        public int? Year { get; set; } // Год записи

        // Foreign Keys
        [Required(ErrorMessage = "Выберите жанр")]
        [Range(1, int.MaxValue, ErrorMessage = "Выберите жанр")]
        public int GenreId { get; set; }
        
        [Required(ErrorMessage = "Выберите исполнителя")]
        [Range(1, int.MaxValue, ErrorMessage = "Выберите исполнителя")]
        public int ArtistId { get; set; }
        
        public int? ComposerId { get; set; } // A song might not have a separate composer
        
        [Required(ErrorMessage = "Выберите лейбл")]
        [Range(1, int.MaxValue, ErrorMessage = "Выберите лейбл")]
        public int LabelId { get; set; }
        
        [Required(ErrorMessage = "Выберите тип носителя")]
        [Range(1, int.MaxValue, ErrorMessage = "Выберите тип носителя")]
        public int MediaTypeId { get; set; }

        // Navigation properties
        public Genre? Genre { get; set; }
        public Artist? Artist { get; set; }
        public Composer? Composer { get; set; }
        public Label? Label { get; set; }
        public MediaType? MediaType { get; set; }

        public ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();
    }
}
