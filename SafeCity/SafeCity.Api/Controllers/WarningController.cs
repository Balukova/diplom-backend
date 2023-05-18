using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SafeCity.Api.Data;
using SafeCity.Api.Entity;
using SafeCity.Api.Utils;

namespace SafeCity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarningController : ControllerBase
    {
        private readonly SafeCityContext _context;

        public WarningController(SafeCityContext context)
        {
            _context = context;
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet("GetWarnings")]
        public async Task<ActionResult<IEnumerable<WarningEntity>>> GetWarnings()
        {
            return await _context.Warnings.ToListAsync();
        }

        [Authorize(Roles = UserRoles.Client)]
        [HttpPost("CreateWarning")]
        public async Task<ActionResult<WarningEntity>> CreateWarning(WarningEntity warningEntity)
        {
            _context.Warnings.Add(warningEntity);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
