using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task<IActionResult> Index()
        {
            var musicCatalogContext = _context.Musics.Include(m => m.Artist).Include(m => m.Composer).Include(m => m.Genre).Include(m => m.Label).Include(m => m.MediaType);
            return View(await musicCatalogContext.ToListAsync());
        }

        // GET: Musics/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: Musics/Create
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
        public async Task<IActionResult> Create([Bind("Title,Year,GenreId,ArtistId,ComposerId,LabelId,MediaTypeId")] Music music)
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
        public async Task<IActionResult> Edit(int id, [Bind("MusicId,Title,Year,GenreId,ArtistId,ComposerId,LabelId,MediaTypeId")] Music music)
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
