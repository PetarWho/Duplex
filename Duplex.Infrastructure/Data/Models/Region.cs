using System.ComponentModel.DataAnnotations;

namespace Duplex.Infrastructure.Data.Models
{
    public class Region
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(2), MaxLength(40)]
        public string Name { get; set; } = null!;

        [Required]
        [MinLength(2), MaxLength(6)]
        public string Code { get; set; } = null!;

        public override string ToString()
        {
            return this.Name;
        }
    }
}
