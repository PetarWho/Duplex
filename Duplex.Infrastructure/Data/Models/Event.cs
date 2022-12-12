using Duplex.Infrastructure.Data.Models.Account;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public DateTime CreatedOnUTC { get; set; }

        [Required]
        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        [Required]
        [MaxLength(2048)]
        [Url]
        public string ImageUrl { get; set; } = null!;

        public ICollection<EventUser> Participants { get; set; } = new HashSet<EventUser>();
        public ICollection<Bet> Bets { get; set; } = new HashSet<Bet>();
    }
}
