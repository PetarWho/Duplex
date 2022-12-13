using System.ComponentModel.DataAnnotations;
using Duplex.Core.Models.Category;

namespace Duplex.Core.Models.Event
{
    public class AddEventModel
    {
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
        [MaxLength(2048)]
        [Url]
        public string ImageUrl { get; set; } = null!;

        [Required]
        public int CategoryId { get; set; }

        public IEnumerable<CategoryModel> Categories { get; set; } = new List<CategoryModel>();
    }
}
