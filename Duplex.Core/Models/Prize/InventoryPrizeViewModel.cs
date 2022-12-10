using System.ComponentModel.DataAnnotations;

namespace Duplex.Core.Models.Prize
{
    public class InventoryPrizeViewModel
    {
        public Guid PrizeId { get; set; }
        public string Name { get; set; } = null!;
        public int Cost { get; set; }
        public string Description { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public DateTime? AcquiredOnUTC { get; set; }
        public string UserName { get; set; } = null!;
    }
}
