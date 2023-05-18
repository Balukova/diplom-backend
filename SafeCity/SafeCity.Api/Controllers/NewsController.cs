using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SafeCity.Api.Data;
using SafeCity.Api.Entity;
using SafeCity.Api.Utils;

namespace SafeCity.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly SafeCityContext _context;

        public NewsController(SafeCityContext context)
        {
            _context = context;
        }

        [Authorize(Roles = UserRoles.AdminClient)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NewsEntity>>> GetNews()
        {
            return await _context.News.ToListAsync();
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        public async Task<ActionResult<NewsEntity>> PostNewsEntity(NewsEntity newsEntity)
        {
            _context.News.Add(newsEntity);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNewsEntity(int id)
        {
            var newsEntity = await _context.News.FindAsync(id);
            if (newsEntity == null)
            {
                return NotFound();
            }

            _context.News.Remove(newsEntity);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
