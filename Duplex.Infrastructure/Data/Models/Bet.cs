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
    public class Bet
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MinLength(3), MaxLength(40)]
        public string Name { get; set; } = null!;

        [Required]
        public DateTime CreatedOnUTC { get; set; }

        [Required]
        [ForeignKey(nameof(Event))]
        public Guid EventId { get; set; }
        public Event Event { get; set; } = null!;

        [Required]
        [Range(10, 2000, ConvertValueInInvariantCulture = true)]
        public int TotalPrize { get; set; }
    }
}
