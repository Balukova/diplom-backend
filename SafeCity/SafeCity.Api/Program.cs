using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SafeCity.Api.Data;
using SafeCity.Api.Entity;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using SafeCity.Api.Utils;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using SafeCity.Api.Services;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddDbContext<SafeCityContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("SafeCityContext") ?? throw new InvalidOperationException("Connection string 'SafeCityContext' not found.")));


builder.Services.AddIdentity<AppUser, IdentityRole<int>>()
    .AddEntityFrameworkStores<SafeCityContext>()
    .AddDefaultTokenProviders();

builder.Services.AddTransient<AuthService>();
builder.Services.AddTransient<MarkService>();
builder.Services.AddTransient<OffenderService>();
builder.Services.AddTransient<WarningService>();
builder.Services.AddTransient<UserService>();
builder.Services.AddTransient<NewsService>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ElevatedRights", policy =>
          policy.RequireRole("Admin", "Client"));
});


builder.Services
    .AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(x =>
    {
        x.SaveToken = true;
        x.RequireHttpsMetadata = false;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = "SafeCityContextSafeCityContext",
            ValidIssuer = "SafeCityContextSafeCityContext",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SafeCityContextSafeCityContext")),
            //ValidateLifetime = true,
            //ClockSkew = TimeSpan.FromHours(720)
        };
        x.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("OnAuthenticationFailed: " + context.Exception.Message);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("OnTokenValidated: " + context.SecurityToken);
                return Task.CompletedTask;
            }
        };
    }).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Home/Error";
        //options.SlidingExpiration = true;
        //options.ExpireTimeSpan = new TimeSpan(0, 1, 0);
    });


builder.Services.AddControllers().AddJsonOptions(x =>
{
    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SparksLifeBackend API",
        Version = "v1"
    });
    c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please insert JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
       {
         new OpenApiSecurityScheme
         {
           Reference = new OpenApiReference
           {
             Type = ReferenceType.SecurityScheme,
             Id = "Bearer"
           }
          },
          new string[] { }
        }
    });

});


var app = builder.Build();

app.UseSwagger(options =>
{
    options.SerializeAsV2 = true;
});
app.UseSwaggerUI();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
};

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();


app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    //endpoints.MapRazorPages();
});

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var rolesManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
    await RoleInitializer.InitializeAsync(userManager, rolesManager);
}

app.Run();

