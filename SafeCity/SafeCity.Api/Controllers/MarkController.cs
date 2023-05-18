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

namespace SafeCity.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MarkController : ControllerBase
    {
        private readonly SafeCityContext _context;

        public MarkController(SafeCityContext context)
        {
            _context = context;
        }

        [HttpGet("GetMarks")]
        public async Task<ActionResult<IEnumerable<MarkEntity>>> GetMarks(GetMarksRequest request)
        {
            if (_context.Marks == null)
            {
                return NotFound();
            }
            var currentTime = DateTime.UtcNow;
            return await _context.Marks
                .Where(x => x.CreatedTime.AddHours(8) < currentTime 
                        && x.Status == MarkStatus.Active 
                        && CalculateDistance(request.Latitude, request.Longitude, x.Latitude, x.Longitude) <= request.Radius)
                .ToListAsync();
        }


        [HttpPost("CreateMark")]
        public async Task<ActionResult> CreateMark(CreateMarkRequest request)
        {
            int userId = User.GetClUserId();

            MarkEntity markEntity = request.Adapt<MarkEntity>();
            markEntity.UserId = userId;
            markEntity.CreatedTime = DateTime.UtcNow;
            markEntity.Status = MarkStatus.Active;

            _context.Marks.Add(markEntity);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteMark(int id)
        {
            int userId = User.GetClUserId();
            var markEntity = await _context.Marks.FindAsync(id);
            if (markEntity == null)
            {
                return NotFound();
            }
            if(markEntity.UserId != userId)
            {
                return BadRequest("You dont have permissions!");
            }

            markEntity.Status = MarkStatus.Deleted;
            _context.Entry(markEntity).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok();
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);

            lat1 = ToRadians(lat1);
            lat2 = ToRadians(lat2);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return 6371.0 * c; // Radius of earth in Km
        }

        private double ToRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }
    }

    public class GetMarksRequest
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public double Radius { get; set; }
        public MarkType Type { get; set; }
    }

    public class CreateMarkRequest
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> Images { get; set; }
        public List<string> Videos { get; set; }
        public MarkType Type { get; set; }
    }
}
