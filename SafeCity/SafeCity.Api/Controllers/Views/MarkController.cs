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
using SafeCity.Api.Enums;
using SafeCity.Api.Utils;

namespace SafeCity.Api.Controllers.Views
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = UserRoles.Admin)]
    public class MarkController : Controller
    {
        private readonly SafeCityContext _context;

        public MarkController(SafeCityContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<MarkEntity> safeCityContext = await _context.Marks.Include(m => m.User).ToListAsync();

            DateTime currentTime = DateTime.Now;

            foreach (var mark in safeCityContext)
            {
                if ((currentTime - mark.CreatedTime).TotalHours > 8)
                {
                    mark.Status = MarkStatus.Expired;
                }
            }

            _context.SaveChanges();

            return View(safeCityContext);
        }

        // GET: Mark/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Marks == null)
            {
                return NotFound();
            }

            var markEntity = await _context.Marks
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (markEntity == null)
            {
                return NotFound();
            }

            return View(markEntity);
        }

        // GET: Mark/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Mark/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,Longitude,Latitude,Title,Description,Images,Videos,Type,Status,CreatedTime")] MarkEntity markEntity)
        {
            if (ModelState.IsValid)
            {
                _context.Add(markEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", markEntity.UserId);
            return View(markEntity);
        }

        // GET: Mark/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Marks == null)
            {
                return NotFound();
            }

            var markEntity = await _context.Marks.FindAsync(id);
            if (markEntity == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", markEntity.UserId);
            return View(markEntity);
        }

        // POST: Mark/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,Longitude,Latitude,Title,Description,Images,Videos,Type,Status,CreatedTime")] MarkEntity markEntity)
        {
            if (id != markEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(markEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MarkEntityExists(markEntity.Id))
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", markEntity.UserId);
            return View(markEntity);
        }

        // GET: Mark/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Marks == null)
            {
                return NotFound();
            }

            var markEntity = await _context.Marks
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (markEntity == null)
            {
                return NotFound();
            }

            return View(markEntity);
        }

        // POST: Mark/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Marks == null)
            {
                return Problem("Entity set 'SafeCityContext.Marks'  is null.");
            }
            var markEntity = await _context.Marks.FindAsync(id);
            if (markEntity != null)
            {
                _context.Marks.Remove(markEntity);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MarkEntityExists(int id)
        {
          return (_context.Marks?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
