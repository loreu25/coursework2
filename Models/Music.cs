using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MusicCatalog.Models
{
    public class Music
    {
        public int MusicId { get; set; }
        
        [Required(ErrorMessage = "Название обязательно")]
        [Display(Name = "Название")]
        public string Title { get; set; }
        
        [Display(Name = "Дата записи")]
        public DateOnly? RecordingDate { get; set; } // Дата записи

        // Foreign Keys
        [Required(ErrorMessage = "Выберите жанр")]
        [Range(1, int.MaxValue, ErrorMessage = "Выберите жанр")]
        public int GenreId { get; set; }
        
        [Required(ErrorMessage = "Выберите исполнителя")]
        [Range(1, int.MaxValue, ErrorMessage = "Выберите исполнителя")]
        public int ArtistId { get; set; }
        
        public int? ComposerId { get; set; } 
        
        [Required(ErrorMessage = "Выберите лейбл")]
        [Range(1, int.MaxValue, ErrorMessage = "Выберите лейбл")]
        public int LabelId { get; set; }
        
        [Required(ErrorMessage = "Выберите тип носителя")]
        [Range(1, int.MaxValue, ErrorMessage = "Выберите тип носителя")]
        public int MediaTypeId { get; set; }

        public Genre? Genre { get; set; }
        public Artist? Artist { get; set; }
        public Composer? Composer { get; set; }
        public Label? Label { get; set; }
        public MediaType? MediaType { get; set; }

        public ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();
    }
}
