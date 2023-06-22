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
using SafeCity.Api.Services;
using SafeCity.Api.Utils;

namespace SafeCity.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly NewsService _newsService;

        public NewsController(NewsService newsService)
        {
            _newsService = newsService;
        }

        [Authorize(Roles = UserRoles.AdminClient)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NewsEntity>>> GetNews()
        {
            return Ok(await _newsService.GetNews());
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        public async Task<ActionResult<NewsEntity>> PostNewsEntity(NewsEntity newsEntity)
        {
            await _newsService.PostNewsEntity(newsEntity);
            return Ok();
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNewsEntity(int id)
        {
            var newsEntity = await _newsService.FindNewsEntity(id);
            if (newsEntity == null)
            {
                return NotFound();
            }

            await _newsService.DeleteNewsEntity(newsEntity);
            return Ok();
        }
    }
}
