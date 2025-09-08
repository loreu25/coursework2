using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MusicCatalog.Data;
using MusicCatalog.Models;

namespace MusicCatalog.Controllers
{
    [Authorize]
    public class PlaylistsController : Controller
    {
        private readonly MusicCatalogContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PlaylistsController(MusicCatalogContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Playlists
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            var playlists = await _context.Playlists
                .Where(p => p.UserId == currentUser.Id)
                .Include(p => p.Musics)
                .OrderByDescending(p => p.CreationDate)
                .ToListAsync();

            return View(playlists);
        }

        // GET: Playlists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            var playlist = await _context.Playlists
                .Include(p => p.Musics)
                    .ThenInclude(m => m.Artist)
                .Include(p => p.Musics)
                    .ThenInclude(m => m.Genre)
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.PlaylistId == id && p.UserId == currentUser.Id);

            if (playlist == null)
            {
                return NotFound();
            }

            return View(playlist);
        }

        // GET: Playlists/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Playlists/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] Playlist playlist)
        {
            Console.WriteLine($"POST Create called with playlist name: {playlist?.Name}");
            
            var currentUser = await _userManager.GetUserAsync(User);
            Console.WriteLine($"Current user: {currentUser?.UserName}, ID: {currentUser?.Id}");
            
            if (currentUser == null)
            {
                Console.WriteLine("User is null, returning Challenge");
                return Challenge();
            }

            // Убираем валидацию для навигационных свойств
            ModelState.Remove("User");
            ModelState.Remove("Musics");
            
            Console.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"ModelState error: {error.ErrorMessage}");
                }
            }

            if (ModelState.IsValid)
            {
                playlist.UserId = currentUser.Id;
                playlist.CreationDate = DateTime.UtcNow;
                Console.WriteLine($"Adding playlist: {playlist.Name} for user: {playlist.UserId}");
                _context.Add(playlist);
                await _context.SaveChangesAsync();
                Console.WriteLine("Playlist saved successfully");
                return RedirectToAction(nameof(Index));
            }
            return View(playlist);
        }

        // GET: Playlists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            var playlist = await _context.Playlists
                .FirstOrDefaultAsync(p => p.PlaylistId == id && p.UserId == currentUser.Id);

            if (playlist == null)
            {
                return NotFound();
            }
            return View(playlist);
        }

        // POST: Playlists/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PlaylistId,Name,CreationDate,UserId")] Playlist playlist)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            if (id != playlist.PlaylistId || playlist.UserId != currentUser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(playlist);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlaylistExists(playlist.PlaylistId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Details), new { id = playlist.PlaylistId });
            }
            return View(playlist);
        }

        // GET: Playlists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            var playlist = await _context.Playlists
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.PlaylistId == id && p.UserId == currentUser.Id);

            if (playlist == null)
            {
                return NotFound();
            }

            return View(playlist);
        }

        // POST: Playlists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            var playlist = await _context.Playlists
                .FirstOrDefaultAsync(p => p.PlaylistId == id && p.UserId == currentUser.Id);

            if (playlist != null)
            {
                _context.Playlists.Remove(playlist);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Playlists/AddMusic/5
        public async Task<IActionResult> AddMusic(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            var playlist = await _context.Playlists
                .Include(p => p.Musics)
                .FirstOrDefaultAsync(p => p.PlaylistId == id && p.UserId == currentUser.Id);

            if (playlist == null)
            {
                return NotFound();
            }

            // Получаем музыку, которой еще нет в плейлисте
            var existingMusicIds = playlist.Musics.Select(m => m.MusicId).ToList();
            var availableMusic = await _context.Musics
                .Include(m => m.Artist)
                .Include(m => m.Genre)
                .Where(m => !existingMusicIds.Contains(m.MusicId))
                .ToListAsync();

            ViewBag.PlaylistId = id;
            ViewBag.PlaylistName = playlist.Name;
            return View(availableMusic);
        }

        // POST: Playlists/AddMusic/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMusic(int playlistId, int musicId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            var playlist = await _context.Playlists
                .Include(p => p.Musics)
                .FirstOrDefaultAsync(p => p.PlaylistId == playlistId && p.UserId == currentUser.Id);

            if (playlist == null)
            {
                return NotFound();
            }

            var music = await _context.Musics.FindAsync(musicId);
            if (music == null)
            {
                return NotFound();
            }

            if (!playlist.Musics.Contains(music))
            {
                playlist.Musics.Add(music);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Details), new { id = playlistId });
        }

        // POST: Playlists/RemoveMusic
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveMusic(int playlistId, int musicId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            var playlist = await _context.Playlists
                .Include(p => p.Musics)
                .FirstOrDefaultAsync(p => p.PlaylistId == playlistId && p.UserId == currentUser.Id);

            if (playlist == null)
            {
                return NotFound();
            }

            var music = playlist.Musics.FirstOrDefault(m => m.MusicId == musicId);
            if (music != null)
            {
                playlist.Musics.Remove(music);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Details), new { id = playlistId });
        }

        private bool PlaylistExists(int id)
        {
            return _context.Playlists.Any(e => e.PlaylistId == id);
        }
    }
}
