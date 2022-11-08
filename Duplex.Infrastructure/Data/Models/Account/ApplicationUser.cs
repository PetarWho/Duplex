using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Duplex.Infrastructure.Data.Models.Account
{
    public class ApplicationUser:IdentityUser
    {
        public byte[]? Image { get; set; } = File.ReadAllBytes(@"..\..\DefaultPics\dafault-avatar.png");

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
