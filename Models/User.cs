using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace MusicCatalog.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "ФИО")]
        [Required(ErrorMessage = "Поле ФИО обязательно для заполнения")]
        [StringLength(100, ErrorMessage = "ФИО не может быть длиннее 100 символов")]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "Адрес")]
        [StringLength(200, ErrorMessage = "Адрес не может быть длиннее 200 символов")]
        public string? Address { get; set; }

        [Display(Name = "Телефон")]
        [StringLength(20, ErrorMessage = "Телефон не может быть длиннее 20 символов")]
        public override string? PhoneNumber { get; set; }

        [Display(Name = "Роль")]
        public string Role { get; set; } = "User";

        public ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();
    }

    [Obsolete("Используйте ApplicationUser вместо User")]
    public class User
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string Login { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();
    }
}
