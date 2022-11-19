namespace Duplex.Core.Models.Administration.Rank
{
    public class RankModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public Guid ConcurrencyStamp { get; set; }
    }
}
