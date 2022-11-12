using System.ComponentModel.DataAnnotations;

namespace Duplex.Core.Models
{
    public class RegionModel
    {
        public int Id { get; set; }

        [Required]
        [MinLength(2), MaxLength(40)]
        public string Name { get; set; } = null!;

        [Required]
        [MinLength(2), MaxLength(6)]
        public string Code { get; set; } = null!;
    }
}
