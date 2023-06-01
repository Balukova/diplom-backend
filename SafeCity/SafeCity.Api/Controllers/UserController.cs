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
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly SafeCityContext _context;

        public UserController(SafeCityContext context)
        {
            _context = context;
        }


        // GET: api/AppUsers/5
        [HttpGet("GetUserInfo")]
        public async Task<ActionResult<AppUser>> GetUserInfo()
        {
            int userId = User.GetClUserId();
            var appUser = await _context.Users.FindAsync(userId);
            if (appUser == null)
            {
                return NotFound();
            }
            return Ok(appUser);
        }

    }
}
