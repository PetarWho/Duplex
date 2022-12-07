using System.ComponentModel.DataAnnotations;

namespace Duplex.Core.Models.Administration.Rank
{
    public class SetRankModel
    {
        [Required]
        public string UserId { get; set; } = null!;

        public string RankId { get; set; } = null!;
        public IEnumerable<RankModel> Ranks { get; set; } = new List<RankModel>();
    }
}
