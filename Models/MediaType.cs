using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MusicCatalog.Models
{
    public class MediaType
    {
        public int MediaTypeId { get; set; }
        [Required(ErrorMessage = "Название обязательно")]
        [Display(Name = "Название")]
        public string Name { get; set; } 

        public ICollection<Music> Musics { get; set; } = new List<Music>();
    }
}
