using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SafeCity.Api.Enums;

namespace SafeCity.Api.Entity
{
    public class OffenderMarkEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        public int OffenderId { get; set; }
        [ForeignKey(nameof(OffenderId))]
        public OffenderEntity Offender { get; set; }

        public int MarkId { get; set; }
        [ForeignKey(nameof(MarkId))]
        public MarkEntity Mark { get; set; }

        public string Name { get; set; }
        public string Image { get; set; }

        public FoundedOffenderStatus Status { get; set; }
    }
}
