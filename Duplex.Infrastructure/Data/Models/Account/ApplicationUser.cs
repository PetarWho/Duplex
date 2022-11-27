using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Duplex.Infrastructure.Data.Models.Account
{
    public class ApplicationUser:IdentityUser
    {
        public string? Image { get; set; } = @"https://firebasestorage.googleapis.com/v0/b/duplex-aff1b.appspot.com/o/dafault-avatar.png?alt=media&token=ed4ed983-89d1-4811-a9b7-c9235b409526";

        [Required]
        [ForeignKey(nameof(Region))]
        public int RegionId { get; set; }
        public Region Region { get; set; } = null!;
        public int Coins { get; set; } = 0;
        public int Wins { get; set; } = 0;
        public int Loses { get; set; } = 0;
        public ICollection<Bet> Bets { get; set; } = new HashSet<Bet>();
        public ICollection<Prize> Prizes { get; set; } = new HashSet<Prize>();
        public ICollection<Event> Events { get; set; } = new HashSet<Event>();
    }
}
