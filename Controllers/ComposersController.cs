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
    public class ComposersController : Controller
    {
        private readonly MusicCatalogContext _context;

        public ComposersController(MusicCatalogContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["CurrentFilter"] = searchString;

            var composers = from c in _context.Composers
                           select c;

            if (!String.IsNullOrEmpty(searchString))
            {
                composers = composers.Where(c => c.Name.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    composers = composers.OrderByDescending(c => c.Name);
                    break;
                default:
                    composers = composers.OrderBy(c => c.Name);
                    break;
            }

            return View(await composers.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var composer = await _context.Composers
                .FirstOrDefaultAsync(m => m.ComposerId == id);
            if (composer == null)
            {
                return NotFound();
            }

            return View(composer);
        }

        [Authorize(Roles = "Musician")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Musician")]
        public async Task<IActionResult> Create([Bind("Name")] Composer composer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(composer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(composer);
        }

        [Authorize(Roles = "Musician")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var composer = await _context.Composers.FindAsync(id);
            if (composer == null)
            {
                return NotFound();
            }
            return View(composer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Musician")]
        public async Task<IActionResult> Edit(int id, [Bind("ComposerId,Name")] Composer composer)
        {
            if (id != composer.ComposerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(composer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ComposerExists(composer.ComposerId))
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
            return View(composer);
        }

        [Authorize(Roles = "Musician")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var composer = await _context.Composers
                .FirstOrDefaultAsync(m => m.ComposerId == id);
            if (composer == null)
            {
                return NotFound();
            }

            return View(composer);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Musician")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var composer = await _context.Composers.FindAsync(id);
            if (composer != null)
            {
                _context.Composers.Remove(composer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ComposerExists(int id)
        {
            return _context.Composers.Any(e => e.ComposerId == id);
        }
    }
}
