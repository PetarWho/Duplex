using System.ComponentModel.DataAnnotations;

namespace Duplex.Models.Account
{
    /// <summary>
    /// Model for Register View
    /// </summary>
    public class RegisterViewModel
    {
        [Required]
        [MinLength(5), MaxLength(20)]
        [RegularExpression(@"^[a-zA-Z0-9]{5,}$",ErrorMessage = "Username has to contain only latin letters and digits!")]
        public string UserName { get; set; } = null!;

        [Required]
        [EmailAddress]
        [MinLength(10), MaxLength(60)]
        public string Email { get; set; } = null!;

        [Required]
        [MinLength(5), MaxLength(20)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = null!;
        public int RegionId { get; set; }

        public IEnumerable<Infrastructure.Data.Models.Region> Regions { get; set; } = new List<Infrastructure.Data.Models.Region>();
    }
}
