using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Azure.Core;
using FaceRecognitionDotNet;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SafeCity.Api.Data;
using SafeCity.Api.Entity;
using SafeCity.Api.Services;
using SafeCity.Api.Utils;

namespace SafeCity.Api.Controllers.Views
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = UserRoles.Admin)]
    public class OffenderController : Controller
    {
        private readonly SafeCityContext _context;
        private readonly OffenderService _offenderService;

        public OffenderController(SafeCityContext context, OffenderService offenderService)
        {
            _context = context;
            _offenderService = offenderService;
        }

        // GET: Offender
        public async Task<IActionResult> Index()
        {
              return _context.Offenders != null ? 
                          View(await _context.Offenders.ToListAsync()) :
                          Problem("Entity set 'SafeCityContext.Offenders'  is null.");
        }

        // GET: Offender/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Offenders == null)
            {
                return NotFound();
            }

            var offenderEntity = await _context.Offenders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (offenderEntity == null)
            {
                return NotFound();
            }

            return View(offenderEntity);
        }

        // GET: Offender/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Offender/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Image")] OffenderEntity offenderEntity)
        {
            IEnumerable<FaceEncoding> faceEncodings = await _offenderService.GetFaceEncodings(offenderEntity.Image);

            _context.Offenders.Add(new OffenderEntity
            {
                Name = offenderEntity.Name,
                Description = offenderEntity.Description,
                Image = offenderEntity.Image,
                ImageEncoding = JsonConvert.SerializeObject(faceEncodings.First()),
            });
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // POST: Offender/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Image,ImageEncoding")] OffenderEntity offenderEntity)
        {
            if (id != offenderEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(offenderEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OffenderEntityExists(offenderEntity.Id))
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
            return View(offenderEntity);
        }

        // GET: Offender/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Offenders == null)
            {
                return NotFound();
            }

            var offenderEntity = await _context.Offenders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (offenderEntity == null)
            {
                return NotFound();
            }

            return View(offenderEntity);
        }

        // POST: Offender/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Offenders == null)
            {
                return Problem("Entity set 'SafeCityContext.Offenders'  is null.");
            }
            var offenderEntity = await _context.Offenders.FindAsync(id);
            if (offenderEntity != null)
            {
                _context.Offenders.Remove(offenderEntity);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OffenderEntityExists(int id)
        {
          return (_context.Offenders?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
