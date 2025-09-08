using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MusicCatalog.Data;
using MusicCatalog.Models;

namespace MusicCatalog.Controllers
{
    public class MusicsController : Controller
    {
        private readonly MusicCatalogContext _context;

        public MusicsController(MusicCatalogContext context)
        {
            _context = context;
        }

        // GET: Musics
        public async Task<IActionResult> Index(string sortOrder, string searchString, string genreFilter, string artistFilter)
        {
            ViewData["TitleSortParm"] = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["GenreSortParm"] = sortOrder == "Genre" ? "genre_desc" : "Genre";
            ViewData["ArtistSortParm"] = sortOrder == "Artist" ? "artist_desc" : "Artist";
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentGenreFilter"] = genreFilter;
            ViewData["CurrentArtistFilter"] = artistFilter;

            // Получаем списки для фильтров
            ViewData["Genres"] = new SelectList(await _context.Genres.OrderBy(g => g.Name).ToListAsync(), "Name", "Name");
            ViewData["Artists"] = new SelectList(await _context.Artists.OrderBy(a => a.Name).ToListAsync(), "Name", "Name");

            var musics = from m in _context.Musics.Include(m => m.Artist).Include(m => m.Composer).Include(m => m.Genre).Include(m => m.Label).Include(m => m.MediaType)
                        select m;

            // Фильтрация по поиску
            if (!String.IsNullOrEmpty(searchString))
            {
                musics = musics.Where(m => m.Title.Contains(searchString));
            }

            // Фильтрация по жанру
            if (!String.IsNullOrEmpty(genreFilter))
            {
                musics = musics.Where(m => m.Genre.Name == genreFilter);
            }

            // Фильтрация по исполнителю
            if (!String.IsNullOrEmpty(artistFilter))
            {
                musics = musics.Where(m => m.Artist.Name == artistFilter);
            }

            // Сортировка
            switch (sortOrder)
            {
                case "title_desc":
                    musics = musics.OrderByDescending(m => m.Title);
                    break;
                case "Date":
                    musics = musics.OrderBy(m => m.RecordingDate);
                    break;
                case "date_desc":
                    musics = musics.OrderByDescending(m => m.RecordingDate);
                    break;
                case "Genre":
                    musics = musics.OrderBy(m => m.Genre.Name);
                    break;
                case "genre_desc":
                    musics = musics.OrderByDescending(m => m.Genre.Name);
                    break;
                case "Artist":
                    musics = musics.OrderBy(m => m.Artist.Name);
                    break;
                case "artist_desc":
                    musics = musics.OrderByDescending(m => m.Artist.Name);
                    break;
                default:
                    musics = musics.OrderBy(m => m.Title);
                    break;
            }

            return View(await musics.ToListAsync());
        }

        // GET: Musics/Details/5
        public async Task<IActionResult> Details(int? id, string returnController = null, string returnAction = null, int? returnId = null)
        {
            if (id == null)
            {
                return NotFound();
            }

            var music = await _context.Musics
                .Include(m => m.Artist)
                .Include(m => m.Composer)
                .Include(m => m.Genre)
                .Include(m => m.Label)
                .Include(m => m.MediaType)
                .FirstOrDefaultAsync(m => m.MusicId == id);
            if (music == null)
            {
                return NotFound();
            }

            // Передаем параметры возврата в представление
            ViewBag.ReturnController = returnController;
            ViewBag.ReturnAction = returnAction;
            ViewBag.ReturnId = returnId;

            return View(music);
        }

        // GET: Musics/Create
        [Authorize(Roles = "Musician")]
        public IActionResult Create()
        {
            ViewData["ArtistId"] = new SelectList(_context.Artists, "ArtistId", "Name");
            ViewData["ComposerId"] = new SelectList(_context.Composers, "ComposerId", "Name");
            ViewData["GenreId"] = new SelectList(_context.Genres, "GenreId", "Name");
            ViewData["LabelId"] = new SelectList(_context.Labels, "LabelId", "Name");
            ViewData["MediaTypeId"] = new SelectList(_context.MediaTypes, "MediaTypeId", "Name");
            return View();
        }

        // POST: Musics/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Musician")]
        public async Task<IActionResult> Create([Bind("Title,RecordingDate,GenreId,ArtistId,ComposerId,LabelId,MediaTypeId")] Music music)
        {
            if (ModelState.IsValid)
            {
                _context.Add(music);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            ViewData["ArtistId"] = new SelectList(_context.Artists, "ArtistId", "Name", music.ArtistId);
            ViewData["ComposerId"] = new SelectList(_context.Composers, "ComposerId", "Name", music.ComposerId);
            ViewData["GenreId"] = new SelectList(_context.Genres, "GenreId", "Name", music.GenreId);
            ViewData["LabelId"] = new SelectList(_context.Labels, "LabelId", "Name", music.LabelId);
            ViewData["MediaTypeId"] = new SelectList(_context.MediaTypes, "MediaTypeId", "Name", music.MediaTypeId);
            return View(music);
        }

        // GET: Musics/Edit/5
        [Authorize(Roles = "Musician")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var music = await _context.Musics.FindAsync(id);
            if (music == null)
            {
                return NotFound();
            }
            ViewData["ArtistId"] = new SelectList(_context.Artists, "ArtistId", "Name", music.ArtistId);
            ViewData["ComposerId"] = new SelectList(_context.Composers, "ComposerId", "Name", music.ComposerId);
            ViewData["GenreId"] = new SelectList(_context.Genres, "GenreId", "Name", music.GenreId);
            ViewData["LabelId"] = new SelectList(_context.Labels, "LabelId", "Name", music.LabelId);
            ViewData["MediaTypeId"] = new SelectList(_context.MediaTypes, "MediaTypeId", "Name", music.MediaTypeId);
            return View(music);
        }

        // POST: Musics/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Musician")]
        public async Task<IActionResult> Edit(int id, [Bind("MusicId,Title,RecordingDate,GenreId,ArtistId,ComposerId,LabelId,MediaTypeId")] Music music)
        {
            if (id != music.MusicId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(music);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MusicExists(music.MusicId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ArtistId"] = new SelectList(_context.Artists, "ArtistId", "Name", music.ArtistId);
            ViewData["ComposerId"] = new SelectList(_context.Composers, "ComposerId", "Name", music.ComposerId);
            ViewData["GenreId"] = new SelectList(_context.Genres, "GenreId", "Name", music.GenreId);
            ViewData["LabelId"] = new SelectList(_context.Labels, "LabelId", "Name", music.LabelId);
            ViewData["MediaTypeId"] = new SelectList(_context.MediaTypes, "MediaTypeId", "Name", music.MediaTypeId);
            return View(music);
        }

        // GET: Musics/Delete/5
        [Authorize(Roles = "Musician")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var music = await _context.Musics
                .Include(m => m.Artist)
                .Include(m => m.Composer)
                .Include(m => m.Genre)
                .Include(m => m.Label)
                .Include(m => m.MediaType)
                .FirstOrDefaultAsync(m => m.MusicId == id);
            if (music == null)
            {
                return NotFound();
            }

            return View(music);
        }

        // POST: Musics/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Musician")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var music = await _context.Musics.FindAsync(id);
            if (music != null)
            {
                _context.Musics.Remove(music);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MusicExists(int id)
        {
            return _context.Musics.Any(e => e.MusicId == id);
        }
    }
}
