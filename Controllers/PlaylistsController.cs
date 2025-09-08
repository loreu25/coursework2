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

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] Playlist playlist)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            // Exclude navigation properties from validation (not posted from form)
            ModelState.Remove("User");
            ModelState.Remove("Musics");

            if (ModelState.IsValid)
            {
                playlist.UserId = currentUser.Id;
                playlist.CreationDate = DateTime.UtcNow;
                _context.Add(playlist);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(playlist);
        }

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
