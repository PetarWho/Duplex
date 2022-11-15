namespace Duplex.Core.Models
{
    public class PrizeModel
    { 
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public int Cost { get; set; }
        public string Description { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public DateTime CreatedOnUTC { get; set; }
    }
}
