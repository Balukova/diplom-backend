using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using SafeCity.Api.Entity;

namespace SafeCity.Api.Data
{
    public class SafeCityContext : IdentityDbContext<AppUser, IdentityRole<int>, int>
    {
        public SafeCityContext(DbContextOptions<SafeCityContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MarkEntity>()
                .Property(x => x.Images)
                .HasConversion(new ValueConverter<List<string>, string>(
                    v => JsonConvert.SerializeObject(v), // Convert to string for persistence
                    v => JsonConvert.DeserializeObject<List<string>>(v))); // Convert to List<String> for use

            modelBuilder.Entity<MarkEntity>()
                .Property(x => x.Videos)
                .HasConversion(new ValueConverter<List<string>, string>(
                    v => JsonConvert.SerializeObject(v), // Convert to string for persistence
                    v => JsonConvert.DeserializeObject<List<string>>(v))); // Convert to List<String> for use
        }

        public DbSet<MarkEntity> Marks { get; set; } = default!;
        public DbSet<WarningEntity> Warnings { get; set; } = default!;
        public DbSet<NewsEntity> News { get; set; } = default!;
        public DbSet<OffenderEntity> Offenders { get; set; } = default!;
        public DbSet<OffenderMarkEntity> FoundedOffenders { get; set; } = default!;
    }
}
