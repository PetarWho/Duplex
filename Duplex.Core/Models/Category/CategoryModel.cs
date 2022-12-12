using System.ComponentModel.DataAnnotations;

namespace Duplex.Core.Models.Category
{
    public class CategoryModel
    {
        public int Id { get; set; }

        [Required]
        [MinLength(3), MaxLength(20)]
        public string Name { get; set; } = null!;
    }
}
