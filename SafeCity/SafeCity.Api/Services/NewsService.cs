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
    public class NewsService
    {
        private readonly SafeCityContext _context;

        public NewsService(SafeCityContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<NewsEntity>> GetNews()
        {
            return await _context.News.ToListAsync();
        }

        public async Task PostNewsEntity(NewsEntity newsEntity)
        {
            _context.News.Add(newsEntity);
            await _context.SaveChangesAsync();
        }

        public async Task<NewsEntity> FindNewsEntity(int id)
        {
            return await _context.News.FindAsync(id);
        }

        public async Task DeleteNewsEntity(NewsEntity newsEntity)
        {
            _context.News.Remove(newsEntity);
            await _context.SaveChangesAsync();
        }
    }
}
