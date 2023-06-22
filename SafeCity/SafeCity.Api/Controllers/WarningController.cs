using Microsoft.AspNetCore.Authorization;
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
    public class WarningController : ControllerBase
    {
        private readonly WarningService _warningService;

        public WarningController(WarningService warningService)
        {
            _warningService = warningService;
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet("GetWarnings")]
        public async Task<ActionResult<IEnumerable<WarningEntity>>> GetWarnings()
        {
            return Ok(await _warningService.GetWarnings());
        }

        [Authorize(Roles = UserRoles.Client)]
        [HttpPost("CreateWarning")]
        public async Task<ActionResult<WarningEntity>> CreateWarning(WarningEntity warningEntity)
        {
            await _warningService.CreateWarning(warningEntity);
            return Ok();
        }
    }
}
