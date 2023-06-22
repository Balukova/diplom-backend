using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SafeCity.Api.Data;
using SafeCity.Api.Entity;
using SafeCity.Api.Utils;

namespace SafeCity.Api.Controllers.Views
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = UserRoles.Admin)]
    public class NewsController : Controller
    {
        private readonly SafeCityContext _context;

        public NewsController(SafeCityContext context)
        {
            _context = context;
        }

        // GET: NewsView
        public async Task<IActionResult> Index()
        {
            return _context.News != null ?
                        View(await _context.News.ToListAsync()) :
                        Problem("Entity set 'SafeCityContext.News'  is null.");
        }

        // GET: NewsView/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.News == null)
            {
                return NotFound();
            }

            var newsEntity = await _context.News
                .FirstOrDefaultAsync(m => m.Id == id);
            if (newsEntity == null)
            {
                return NotFound();
            }

            return View(newsEntity);
        }

        // GET: NewsView/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: NewsView/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Image")] NewsEntity newsEntity)
        {
            if (ModelState.IsValid)
            {
                _context.Add(newsEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(newsEntity);
        }

        // GET: NewsView/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.News == null)
            {
                return NotFound();
            }

            var newsEntity = await _context.News.FindAsync(id);
            if (newsEntity == null)
            {
                return NotFound();
            }
            return View(newsEntity);
        }

        // POST: NewsView/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Image")] NewsEntity newsEntity)
        {
            if (id != newsEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(newsEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NewsEntityExists(newsEntity.Id))
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
            return View(newsEntity);
        }

        // GET: NewsView/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.News == null)
            {
                return NotFound();
            }

            var newsEntity = await _context.News
                .FirstOrDefaultAsync(m => m.Id == id);
            if (newsEntity == null)
            {
                return NotFound();
            }

            return View(newsEntity);
        }

        // POST: NewsView/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.News == null)
            {
                return Problem("Entity set 'SafeCityContext.News'  is null.");
            }
            var newsEntity = await _context.News.FindAsync(id);
            if (newsEntity != null)
            {
                _context.News.Remove(newsEntity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NewsEntityExists(int id)
        {
            return (_context.News?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
