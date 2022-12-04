using Duplex.Infrastructure.Data.Models;
using Duplex.Infrastructure.Data.Models.Account;
using System.ComponentModel.DataAnnotations;

namespace Duplex.Core.Models.Event
{
    public class DetailsEventModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int EntryCost { get; set; }
        public int TeamSize { get; set; }
        public DateTime CreatedOnUTC { get; set; }
        public string ImageUrl { get; set; } = null!;
        public IEnumerable<EventUser> Participants { get; set; } = new List<EventUser>();
    }
}
