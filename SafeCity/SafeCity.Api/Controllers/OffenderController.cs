using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SafeCity.Api.Data;
using SafeCity.Api.Entity;
using SafeCity.Api.Utils;
using FaceRecognitionDotNet;
using System.Drawing;
using Newtonsoft.Json;
using System.Net;

namespace SafeCity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OffenderController : ControllerBase
    {
        private readonly SafeCityContext _context;

        public OffenderController(SafeCityContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet("FindOffenders/{name}")]
        public async Task<ActionResult<List<OffenderEntity>>> FindOffenders(string name)
        {
            var offenders = await _context.Offenders.Where(x => x.Name.Contains(name)).ToListAsync();
            return Ok(offenders);
        }

        [Authorize]
        [HttpPost("FindOffenderByImage")]
        public async Task<ActionResult<List<OffenderEntity>>> FindOffenderByImage(IFormFile imageFile)
        {
            var faceRecognition = FaceRecognition.Create("Models");

            using var ms = new MemoryStream();
            await imageFile.CopyToAsync(ms);
            var imageBytes = ms.ToArray();
            using var imageStream = new MemoryStream(imageBytes);
            var bitmap = new Bitmap(imageStream);
            var image = FaceRecognition.LoadImage(bitmap);

            IEnumerable<FaceEncoding> faceEncodings = faceRecognition.FaceEncodings(image);

            List<OffenderEntity> result = new List<OffenderEntity>();
            if (faceEncodings.Any())
            {
                List<OffenderEntity> offenders = await _context.Offenders.ToListAsync();
                foreach (var offender in offenders)
                {
                    // Десериализация ImageEncoding из строки в FaceEncoding
                    var offenderFaceEncoding = JsonConvert.DeserializeObject<FaceEncoding>(offender.ImageEncoding);
                    var comparesResult = FaceRecognition.CompareFaces(new[] { faceEncodings.First() }, offenderFaceEncoding).ToList();
                    if (comparesResult[0])
                    {
                        result.Add(offender);
                    }
                }
            }

            return Ok(result);
        }


        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost("CreateOffender")]
        public async Task<ActionResult<OffenderEntity>> CreateOffender(CreateOffenderRerquest rerquest)
        {
            var faceRecognition = FaceRecognition.Create("Models");

            var webClient = new WebClient();
            var imageBytes = await webClient.DownloadDataTaskAsync(rerquest.ImageUrl);
            using var imageStream = new MemoryStream(imageBytes);
            var bitmap = new Bitmap(imageStream);
            var image = FaceRecognition.LoadImage(bitmap);

            IEnumerable<FaceEncoding> faceEncodings = faceRecognition.FaceEncodings(image);

            _context.Offenders.Add(new OffenderEntity
            {
                Name = rerquest.Name,
                Description = rerquest.Description,
                Image = rerquest.ImageUrl,
                ImageEncoding = JsonConvert.SerializeObject(faceEncodings.First()),
            });
            await _context.SaveChangesAsync();

            return Ok();
        }



        [Authorize(Roles = UserRoles.Admin)]
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

    public class CreateOffenderRerquest
    {
        public string Name { get; set; } 
        public string Description { get; set; }
        public  string ImageUrl { get; set; }
    }
}
