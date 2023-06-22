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
    public class WarningService
    {

        private readonly SafeCityContext _context;

        public WarningService(SafeCityContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WarningEntity>> GetWarnings()
        {
            return await _context.Warnings.ToListAsync();
        }

        public async Task CreateWarning(WarningEntity warningEntity)
        {
            _context.Warnings.Add(warningEntity);
            await _context.SaveChangesAsync();
        }
    }
}
