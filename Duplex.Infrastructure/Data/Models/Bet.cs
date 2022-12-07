using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Duplex.Infrastructure.Data.Models
{
    public class Bet
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public DateTime CreatedOnUTC { get; set; }

        [Required]
        [ForeignKey(nameof(Event))]
        public Guid EventId { get; set; }
        public Event Event { get; set; } = null!;

        [Required]
        [Range(0, 10000, ConvertValueInInvariantCulture = true)]
        public int PrizePool { get; set; } = 0;
        public IEnumerable<UserBet> UsersBets { get; set; } = new List<UserBet>();
    }
}
