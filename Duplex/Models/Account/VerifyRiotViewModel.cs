using System.ComponentModel.DataAnnotations;

namespace Duplex.Models.Account
{
    public class VerifyRiotViewModel
    {
        [Required]
        [MinLength(3), MaxLength(16)]
        public string SummonerName { get; set; } = null!;
    }
}
