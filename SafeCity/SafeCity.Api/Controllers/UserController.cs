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
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet("GetUserInfo")]
        public async Task<ActionResult<AppUser>> GetUserInfo()
        {
            int userId = User.GetClUserId();
            var appUser = await _userService.GetUserInfo(userId);
            if (appUser == null)
            {
                return NotFound();
            }
            return Ok(appUser);
        }
    }

}
