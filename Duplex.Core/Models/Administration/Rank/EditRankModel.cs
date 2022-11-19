using System.ComponentModel.DataAnnotations;

namespace Duplex.Core.Models.Administration.Rank
{
    public class EditRankModel
    {
        public Guid Id { get; set; }

        [Required]
        [MinLength(3), MaxLength(20)]
        public string Name { get; set; } = null!;
    }
}
