using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SafeCity.Api.Enums;

namespace SafeCity.Api.Entity
{
    public class MarkEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public AppUser User { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> Images { get; set; }
        public List<string> Videos { get; set; }
        public MarkType Type { get; set; }
        public MarkStatus Status { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
