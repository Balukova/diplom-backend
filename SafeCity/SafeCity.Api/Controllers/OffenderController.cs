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
    [Authorize(Roles = UserRoles.Admin)]
    [Route("api/[controller]")]
    [ApiController]
    public class OffenderController : ControllerBase
    {
        private readonly SafeCityContext _context;

        public OffenderController(SafeCityContext context)
        {
            _context = context;
        }

        [HttpPost("CreateOffender")]
        public async Task<ActionResult<OffenderEntity>> CreateOffender(OffenderEntity offenderEntity)
        {
            _context.Offenders.Add(offenderEntity);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("DeleteOffender/{id}")]
        public async Task<IActionResult> DeleteOffender(int id)
        {
            var offenderEntity = await _context.Offenders.FindAsync(id);
            if (offenderEntity == null)
            {
                return NotFound();
            }

            _context.Offenders.Remove(offenderEntity);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
