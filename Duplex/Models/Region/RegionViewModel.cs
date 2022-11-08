using System.ComponentModel.DataAnnotations;

namespace Duplex.Models.Region
{
    public class RegionViewModel
    {
        [Required]
        [MinLength(2), MaxLength(40)]
        public string Name { get; set; } = null!;

        [Required]
        [MinLength(2), MaxLength(6)]
        public string Code { get; set; } = null!;
    }
}
