using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SafeCity.Api.Enums;

namespace SafeCity.Api.Entity
{
    public class WarningEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public AppUser User { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public WarningStatus WarningStatus { get; set; }
    }
}
