using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SafeCity.Api.Data;
using SafeCity.Api.Entity;
using SafeCity.Api.Enums;
using SafeCity.Api.Utils;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using SafeCity.Api.Services;

namespace SafeCity.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MarkController : ControllerBase
    {
        private readonly MarkService _markService;

        public MarkController(MarkService markService)
        {
            _markService = markService;
        }

        [HttpPost("GetMarks")]
        public async Task<ActionResult<IEnumerable<MarkEntity>>> GetMarks(GetMarksRequest request)
        {
            var currentTime = DateTime.UtcNow;
            return Ok(await _markService.GetMarks(request, currentTime));
        }

        [HttpPost("CreateMark")]
        public async Task<ActionResult> CreateMark(CreateMarkRequest request)
        {
            int userId = User.GetClUserId();
            await _markService.CreateMark(request, userId);
            return Ok();
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteMark(int id)
        {
            int userId = User.GetClUserId();
            var markEntity = await _markService.FindMark(id);
            if (markEntity == null)
            {
                return NotFound();
            }
            if (markEntity.UserId != userId)
            {
                return BadRequest("You dont have permissions!");
            }

            await _markService.DeleteMark(markEntity);
            return Ok();
        }
    }

   
}
