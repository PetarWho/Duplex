using Duplex.Infrastructure.Data.Models.Account;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Duplex.Infrastructure.Data.Models
{
    public class UserPrize
    {
        [Key]
        public Guid Id { get; set; }
        public string UserId { get; set; } = null!;

        [ForeignKey(nameof(UserId))]
        public ApplicationUser ApplicationUser { get; set; } = null!;

        public Guid PrizeId { get; set; }

        [ForeignKey(nameof(PrizeId))]
        public Prize Prize { get; set; } = null!;

        public DateTime? AcquiredOnUTC { get; set; }
    }
}
