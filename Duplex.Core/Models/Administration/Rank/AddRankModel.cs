using System.ComponentModel.DataAnnotations;

namespace Duplex.Core.Models.Administration.Rank
{
    public class AddRankModel
    {
        [Required]
        [MinLength(3), MaxLength(20)]
        public string Name { get; set; } = null!;
    }
}
