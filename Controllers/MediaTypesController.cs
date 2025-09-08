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
    public class MediaTypesController : Controller
    {
        private readonly MusicCatalogContext _context;

        public MediaTypesController(MusicCatalogContext context)
        {
            _context = context;
        }

        // GET: MediaTypes
        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["CurrentFilter"] = searchString;

            var mediaTypes = from m in _context.MediaTypes
                            select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                mediaTypes = mediaTypes.Where(m => m.Name.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    mediaTypes = mediaTypes.OrderByDescending(m => m.Name);
                    break;
                default:
                    mediaTypes = mediaTypes.OrderBy(m => m.Name);
                    break;
            }

            return View(await mediaTypes.ToListAsync());
        }

        // GET: MediaTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mediaType = await _context.MediaTypes
                .FirstOrDefaultAsync(m => m.MediaTypeId == id);
            if (mediaType == null)
            {
                return NotFound();
            }

            return View(mediaType);
        }

        // GET: MediaTypes/Create
        [Authorize(Roles = "Musician")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: MediaTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Musician")]
        public async Task<IActionResult> Create([Bind("Name")] MediaType mediaType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(mediaType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(mediaType);
        }

        // GET: MediaTypes/Edit/5
        [Authorize(Roles = "Musician")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mediaType = await _context.MediaTypes.FindAsync(id);
            if (mediaType == null)
            {
                return NotFound();
            }
            return View(mediaType);
        }

        // POST: MediaTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Musician")]
        public async Task<IActionResult> Edit(int id, [Bind("MediaTypeId,Name")] MediaType mediaType)
        {
            if (id != mediaType.MediaTypeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mediaType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MediaTypeExists(mediaType.MediaTypeId))
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
            return View(mediaType);
        }

        // GET: MediaTypes/Delete/5
        [Authorize(Roles = "Musician")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mediaType = await _context.MediaTypes
                .FirstOrDefaultAsync(m => m.MediaTypeId == id);
            if (mediaType == null)
            {
                return NotFound();
            }

            return View(mediaType);
        }

        // POST: MediaTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Musician")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mediaType = await _context.MediaTypes.FindAsync(id);
            if (mediaType != null)
            {
                _context.MediaTypes.Remove(mediaType);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MediaTypeExists(int id)
        {
            return _context.MediaTypes.Any(e => e.MediaTypeId == id);
        }
    }
}
