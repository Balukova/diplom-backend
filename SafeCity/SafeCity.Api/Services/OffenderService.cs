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


namespace SafeCity.Api.Services
{
    public class OffenderService
    {

        public OffenderService()
        {
        }

        public async Task<IEnumerable<FaceEncoding>> GetFaceEncodings(string imageUrl)
        {
            var faceRecognition = FaceRecognition.Create("NNModels");

            var webClient = new WebClient();
            var imageBytes = await webClient.DownloadDataTaskAsync(imageUrl);
            using var imageStream = new MemoryStream(imageBytes);
            var bitmap = new Bitmap(imageStream);
            var image = FaceRecognition.LoadImage(bitmap);

            IEnumerable<FaceEncoding> faceEncodings = faceRecognition.FaceEncodings(image);

            return faceEncodings;
        }

        public async Task<IEnumerable<FaceEncoding>> GetFaceEncodings(IFormFile imageFile)
        {
            var faceRecognition = FaceRecognition.Create("NNModels");

            using var ms = new MemoryStream();
            await imageFile.CopyToAsync(ms);
            var imageBytes = ms.ToArray();
            using var imageStream = new MemoryStream(imageBytes);
            var bitmap = new Bitmap(imageStream);
            var image = FaceRecognition.LoadImage(bitmap);

            IEnumerable<FaceEncoding> faceEncodings = faceRecognition.FaceEncodings(image);

            return faceEncodings;
        }

    }
}
