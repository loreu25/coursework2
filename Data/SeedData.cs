using Microsoft.EntityFrameworkCore;
using MusicCatalog.Models;

namespace MusicCatalog.Data
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new MusicCatalogContext(
                serviceProvider.GetRequiredService<DbContextOptions<MusicCatalogContext>>()))
            {
                // Проверяем, есть ли уже данные
                if (context.Genres.Any())
                {
                    return; // База уже заполнена
                }

                // Добавляем жанры
                var genres = new Genre[]
                {
                    new Genre { Name = "Rock" },
                    new Genre { Name = "Pop" },
                    new Genre { Name = "Jazz" },
                    new Genre { Name = "Classical" },
                    new Genre { Name = "Electronic" },
                    new Genre { Name = "Hip-Hop" },
                    new Genre { Name = "Blues" },
                    new Genre { Name = "Country" }
                };
                context.Genres.AddRange(genres);
                context.SaveChanges();

                // Добавляем исполнителей
                var artists = new Artist[]
                {
                    new Artist { Name = "The Beatles", Bio = "Британская рок-группа из Ливерпуля" },
                    new Artist { Name = "Queen", Bio = "Британская рок-группа" },
                    new Artist { Name = "Michael Jackson", Bio = "Король поп-музыки" },
                    new Artist { Name = "Led Zeppelin", Bio = "Английская рок-группа" },
                    new Artist { Name = "Pink Floyd", Bio = "Английская рок-группа" },
                    new Artist { Name = "Elvis Presley", Bio = "Король рок-н-ролла" },
                    new Artist { Name = "Bob Dylan", Bio = "Американский певец и автор песен" },
                    new Artist { Name = "The Rolling Stones", Bio = "Английская рок-группа" }
                };
                context.Artists.AddRange(artists);
                context.SaveChanges();

                // Добавляем композиторов
                var composers = new Composer[]
                {
                    new Composer { Name = "John Lennon" },
                    new Composer { Name = "Paul McCartney" },
                    new Composer { Name = "Freddie Mercury" },
                    new Composer { Name = "Jimmy Page" },
                    new Composer { Name = "David Gilmour" },
                    new Composer { Name = "Roger Waters" }
                };
                context.Composers.AddRange(composers);
                context.SaveChanges();

                // Добавляем лейблы
                var labels = new Label[]
                {
                    new Label { Name = "Apple Records" },
                    new Label { Name = "EMI" },
                    new Label { Name = "Columbia Records" },
                    new Label { Name = "Atlantic Records" },
                    new Label { Name = "Capitol Records" },
                    new Label { Name = "Warner Bros. Records" }
                };
                context.Labels.AddRange(labels);
                context.SaveChanges();

                // Добавляем типы носителей
                var mediaTypes = new MediaType[]
                {
                    new MediaType { Name = "CD" },
                    new MediaType { Name = "Vinyl" },
                    new MediaType { Name = "Cassette" },
                    new MediaType { Name = "Digital" }
                };
                context.MediaTypes.AddRange(mediaTypes);
                context.SaveChanges();

                // Добавляем музыкальные треки
                var musics = new Music[]
                {
                    new Music 
                    { 
                        Title = "Hey Jude", 
                        RecordingDate = new DateOnly(1969, 8, 26),
                        GenreId = genres.First(g => g.Name == "Rock").GenreId,
                        ArtistId = artists.First(a => a.Name == "The Beatles").ArtistId,
                        ComposerId = composers.FirstOrDefault(c => c.Name == "Paul McCartney")?.ComposerId,
                        LabelId = labels.First(l => l.Name == "Apple Records").LabelId,
                        MediaTypeId = mediaTypes.First(m => m.Name == "Vinyl").MediaTypeId
                    },
                    new Music 
                    { 
                        Title = "Bohemian Rhapsody", 
                        RecordingDate = new DateOnly(1975, 10, 31),
                        GenreId = genres.First(g => g.Name == "Rock").GenreId,
                        ArtistId = artists.First(a => a.Name == "Queen").ArtistId,
                        ComposerId = composers.FirstOrDefault(c => c.Name == "Freddie Mercury")?.ComposerId,
                        LabelId = labels.First(l => l.Name == "EMI").LabelId,
                        MediaTypeId = mediaTypes.First(m => m.Name == "Vinyl").MediaTypeId
                    },
                    new Music 
                    { 
                        Title = "Billie Jean", 
                        RecordingDate = new DateOnly(1983, 1, 2),
                        GenreId = genres.First(g => g.Name == "Pop").GenreId,
                        ArtistId = artists.First(a => a.Name == "Michael Jackson").ArtistId,
                        ComposerId = null, // Майкл Джексон сам композитор
                        LabelId = labels.First(l => l.Name == "Columbia Records").LabelId,
                        MediaTypeId = mediaTypes.First(m => m.Name == "CD").MediaTypeId
                    },
                    new Music 
                    { 
                        Title = "Stairway to Heaven", 
                        RecordingDate = new DateOnly(1971, 11, 8),
                        GenreId = genres.First(g => g.Name == "Rock").GenreId,
                        ArtistId = artists.First(a => a.Name == "Led Zeppelin").ArtistId,
                        ComposerId = composers.FirstOrDefault(c => c.Name == "Jimmy Page")?.ComposerId,
                        LabelId = labels.First(l => l.Name == "Atlantic Records").LabelId,
                        MediaTypeId = mediaTypes.First(m => m.Name == "Vinyl").MediaTypeId
                    },
                    new Music 
                    { 
                        Title = "Wish You Were Here", 
                        RecordingDate = new DateOnly(1975, 9, 12),
                        GenreId = genres.First(g => g.Name == "Rock").GenreId,
                        ArtistId = artists.First(a => a.Name == "Pink Floyd").ArtistId,
                        ComposerId = composers.FirstOrDefault(c => c.Name == "David Gilmour")?.ComposerId,
                        LabelId = labels.First(l => l.Name == "Columbia Records").LabelId,
                        MediaTypeId = mediaTypes.First(m => m.Name == "Vinyl").MediaTypeId
                    }
                };
                context.Musics.AddRange(musics);
                context.SaveChanges();

                // Пользователи и плейлисты будут создаваться через Identity после регистрации
                // Пока что оставляем только базовые данные каталога
            }
        }
    }
}
