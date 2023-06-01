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
        public readonly SafeCityContext _context;

        public MarkController(SafeCityContext context)
        {
            _context = context;
        }

        [HttpPost("GetMarks")]
        public async Task<ActionResult<IEnumerable<MarkEntity>>> GetMarks(GetMarksRequest request)
        {
            var currentTime = DateTime.UtcNow;
            return (await _context.Marks
                .Where(x => x.CreatedTime.AddHours(8) > currentTime
                        && x.Status == MarkStatus.Active)
                .ToListAsync()).Where(x => CalculateDistance(request.Latitude, request.Longitude, x.Latitude, x.Longitude) <= request.Radius).ToList();
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
            if (markEntity.UserId != userId)
            {
                return BadRequest("You dont have permissions!");
            }

            markEntity.Status = MarkStatus.Deleted;
            _context.Entry(markEntity).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok();
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2, char unit = 'K')
        {
            if ((lat1 == lat2) && (lon1 == lon2))
            {
                return 0;
            }
            else
            {
                double theta = lon1 - lon2;
                double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
                dist = Math.Acos(dist);
                dist = rad2deg(dist);
                dist = dist * 60 * 1.1515;
                if (unit == 'K')
                {
                    dist = dist * 1.609344;
                }
                else if (unit == 'N')
                {
                    dist = dist * 0.8684;
                }
                return (dist);
            }
        }

        private double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        private double rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
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
