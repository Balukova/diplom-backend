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
    public class UserService
    {
        private readonly SafeCityContext _context;

        public UserService(SafeCityContext context)
        {
            _context = context;
        }

        public async Task<AppUser> GetUserInfo(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }
    }

}
