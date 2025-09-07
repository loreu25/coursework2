using System.Collections.Generic;

namespace MusicCatalog.Models
{
    public class Label
    {
        public int LabelId { get; set; }
        public string Name { get; set; }

        public ICollection<Music> Musics { get; set; } = new List<Music>();
    }
}
