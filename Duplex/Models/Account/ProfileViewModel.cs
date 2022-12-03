using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace Duplex.Models.Account
{
    public class ProfileViewModel
    {
        public string Id { get; set; } = null!;

        [Required]
        [MinLength(5), MaxLength(20)]
        public string UserName { get; set; } = null!;

        [Required]
        [EmailAddress]
        [MinLength(10), MaxLength(60)]
        public string Email { get; set; } = null!;

        [Phone]
        public string? PhoneNumber { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool PhoneConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public string? Image { get; set; }
        public string? Region { get; set; }
        public IEnumerable<string> Ranks { get; set; } = new List<string>();
        public int Coins { get; set; } = 0;
        public int Wins { get; set; } = 0;
        public int Loses { get; set; } = 0;
    }
}
