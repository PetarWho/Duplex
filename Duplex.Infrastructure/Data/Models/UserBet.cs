using Duplex.Infrastructure.Data.Models.Account;
using System.ComponentModel.DataAnnotations.Schema;

namespace Duplex.Infrastructure.Data.Models
{
    public class UserBet
    {
        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        [ForeignKey(nameof(Bet))]
        public Guid BetId { get; set; }

        public Bet Bet { get; set; } = null!;
    }
}
