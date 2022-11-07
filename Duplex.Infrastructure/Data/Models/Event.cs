using Duplex.Infrastructure.Data.Models.Account;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Duplex.Infrastructure.Data.Models
{
    public class Event
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MinLength(3), MaxLength(40)]
        public string Name { get; set; } = null!;

        [Required]
        [MinLength(5), MaxLength(1000)]
        public string Description { get; set; } = null!;

        [Required]
        [Range(5, 1000, ConvertValueInInvariantCulture = true)]
        public int EntryCost { get; set; }

        [Required]
        [Range(1, 5)]
        public int TeamSize { get; set; }

        [Required]
        public DateTime CreatedOnUTC { get; set; }

        public ICollection<ApplicationUser> Participants { get; set; } = new HashSet<ApplicationUser>();
    }
}
